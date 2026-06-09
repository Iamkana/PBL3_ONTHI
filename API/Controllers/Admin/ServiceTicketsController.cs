using System; // Sử dụng các kiểu dữ liệu cơ bản của hệ thống .NET.
using System.Collections.Generic; // Sử dụng các lớp tập hợp danh sách.
using System.Security.Claims; // Sử dụng các lớp quản lý danh tính và claims bảo mật.
using System.Threading.Tasks; // Sử dụng lập trình bất đồng bộ Task.
using Microsoft.AspNetCore.Authorization; // Sử dụng phân quyền truy cập.
using Microsoft.AspNetCore.Mvc; // Sử dụng các thành phần MVC cho Web API.
using PBL3.Application.ServiceTickets; // Sử dụng tầng dịch vụ quản lý phiếu sửa chữa IServiceTicketService.
using PBL3.Shared.DTOs.Common; // Sử dụng các DTO dùng chung.
using PBL3.Shared.DTOs.ServiceTickets; // Sử dụng các DTO liên quan đến phiếu sửa chữa và bảo hành.

namespace PBL3.API.Controllers.Admin // Khai báo namespace cho Controllers quản lý dịch vụ bảo hành/sửa chữa.
{
    [ApiController] // Khai báo đây là một API Controller hỗ trợ tự động validate dữ liệu đầu vào.
    [Route("api/service-tickets")] // Định nghĩa route truy cập mặc định: api/service-tickets.
    [Authorize] // Yêu cầu người dùng phải đăng nhập để truy cập (ngoại trừ các endpoint cho phép nặc danh).
    public class ServiceTicketsController : ControllerBase // Định nghĩa lớp ServiceTicketsController kế thừa từ ControllerBase.
    {
        private readonly IServiceTicketService _service; // Khai báo trường lưu trữ dịch vụ phiếu sửa chữa.
        private readonly ILogger<ServiceTicketsController> _logger; // Khai báo trường lưu trữ dịch vụ ghi log.

        public ServiceTicketsController(IServiceTicketService service, ILogger<ServiceTicketsController> logger) // Constructor injection tiêm dịch vụ và logger.
        {
            _service = service; // Gán dịch vụ được tiêm.
            _logger = logger; // Gán logger được tiêm.
        }

        private Guid GetCurrentUserId() // Hàm phụ trợ nội bộ lấy UserId của người dùng hiện tại từ Claims.
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier); // Tìm Claim chứa định danh người dùng.
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId)) // Kiểm tra xem claim có tồn tại hoặc hợp lệ không.
                throw new UnauthorizedAccessException("Không tìm thấy thông tin người dùng."); // Ném ra ngoại lệ nếu chưa xác thực.
            return userId; // Trả về UserId kiểu Guid hợp lệ.
        }

        private bool IsCustomer => User.IsInRole("Customer"); // Kiểm tra nhanh người dùng hiện tại có vai trò Customer (Khách hàng) hay không.
        private bool IsAdmin => User.IsInRole("Admin"); // Kiểm tra nhanh người dùng hiện tại có vai trò Admin (Quản trị viên) hay không.

        [HttpPost("intake")] // Định nghĩa HTTP POST Method tiếp nhận và kiểm tra thiết bị ban đầu (api/service-tickets/intake).
        [AllowAnonymous] // Cho phép khách hàng hoặc khách vãng lai tự kiểm tra thông tin bảo hành của máy bằng số Serial.
        public async Task<ApiResult<ServiceTicketIntakeEvaluationDto>> EvaluateIntake([FromBody] string serialNumber) // Đánh giá bảo hành của thiết bị dựa trên số Serial.
        {
            try // Bắt đầu khối xử lý an toàn.
            {
                if (string.IsNullOrEmpty(serialNumber)) // Kiểm tra số Serial gửi lên có bị trống không.
                    return ApiResult<ServiceTicketIntakeEvaluationDto>.Fail("Mã Serial không được để trống."); // Trả về thông báo lỗi.

                var result = await _service.GetWarrantyEvaluationAsync(serialNumber); // Gọi service lấy thông tin đánh giá bảo hành thiết bị.
                if (result.BlockingReason != null) // Nếu thiết bị bị từ chối bảo hành (ví dụ: mất cắp, đã hủy...).
                    return ApiResult<ServiceTicketIntakeEvaluationDto>.Fail(result.BlockingReason); // Trả về lỗi kèm lý do chặn.

                return ApiResult<ServiceTicketIntakeEvaluationDto>.Ok(result); // Trả về thông tin đánh giá bảo hành thành công.
            }
            catch (Exception ex) // Bắt lỗi ngoại lệ.
            {
                _logger.LogError(ex, "Error evaluating intake"); // Ghi log lỗi.
                return ApiResult<ServiceTicketIntakeEvaluationDto>.Fail("Lỗi khi kiểm tra Serial: " + ex.Message); // Trả về lỗi hệ thống kèm thông báo.
            }
        }

        [HttpPost] // Định nghĩa HTTP POST Method tạo mới phiếu sửa chữa (api/service-tickets).
        [Authorize(Roles = "Admin, Employee")] // Yêu cầu vai trò Admin hoặc Employee mới được tạo phiếu.
        public async Task<ApiResult<ServiceTicketDetailDto>> CreateTicket([FromBody] ServiceTicketIntakeRequestDto request) // Tạo phiếu sửa chữa/bảo hành mới.
        {
            try // Bẫy lỗi.
            {
                var userId = GetCurrentUserId(); // Lấy mã nhân viên tạo phiếu.
                var ticket = await _service.CreateTicketFromSerialScanAsync(request, userId); // Gọi service thực hiện tạo phiếu sửa chữa dựa trên thông tin quét serial.
                return ApiResult<ServiceTicketDetailDto>.Ok(ticket, "Tạo phiếu sửa chữa thành công."); // Trả về kết quả thành công kèm thông tin phiếu.
            }
            catch (InvalidOperationException ex) // Bắt các lỗi nghiệp vụ không hợp lệ.
            {
                return ApiResult<ServiceTicketDetailDto>.Fail(ex.Message); // Trả về lỗi nghiệp vụ.
            }
            catch (Exception ex) // Bắt lỗi hệ thống.
            {
                _logger.LogError(ex, "Error creating ticket"); // Ghi log.
                return ApiResult<ServiceTicketDetailDto>.Fail("Lỗi khi tạo phiếu: " + ex.Message); // Trả về lỗi hệ thống.
            }
        }

        [HttpGet] // Định nghĩa HTTP GET Method lấy danh sách phiếu sửa chữa (api/service-tickets).
        [Authorize(Roles = "Admin, Employee")] // Chỉ cho phép Admin và Employee quản lý danh sách phiếu sửa chữa.
        public async Task<ApiResult<PagedResult<ServiceTicketListDto>>> GetTickets( // Lấy danh sách phiếu sửa chữa có phân trang và bộ lọc.
            [FromQuery] string? keyword, // Từ khóa tìm kiếm theo mã phiếu, SĐT khách... từ Query.
            [FromQuery] byte? status, // Trạng thái phiếu sửa chữa từ Query.
            [FromQuery] byte? resolutionType, // Loại giải pháp (Sửa chữa/Đổi trả...) từ Query.
            [FromQuery] Guid? assignedEmployeeId, // Id nhân viên kỹ thuật được giao sửa từ Query.
            [FromQuery] Guid? customerId, // Id khách hàng từ Query.
            [FromQuery] DateTime? fromDate, // Ngày bắt đầu lọc.
            [FromQuery] DateTime? toDate, // Ngày kết thúc lọc.
            [FromQuery] int pageNumber = 1, // Số trang mặc định là 1.
            [FromQuery] int pageSize = 10, // Kích thước trang mặc định là 10.
            [FromQuery] string? sortBy = "code", // Cột sắp xếp mặc định là mã phiếu.
            [FromQuery] bool sortDescending = true) // Sắp xếp giảm dần mặc định.
        {
            try // Bẫy lỗi.
            {
                var (items, totalCount) = await _service.GetPagedTicketsAsync( // Gọi service lấy danh sách phân trang phiếu sửa chữa và tổng số lượng.
                    keyword, status, resolutionType, assignedEmployeeId, customerId, // Truyền các bộ lọc tìm kiếm.
                    fromDate, toDate, pageNumber, pageSize, sortBy, sortDescending); // Truyền thông tin phân trang và sắp xếp.

                var paged = new PagedResult<ServiceTicketListDto> // Khởi tạo đối tượng phân trang trả về.
                {
                    Items = items, // Danh sách phiếu.
                    PageNumber = pageNumber, // Trang hiện tại.
                    PageSize = pageSize, // Số bản ghi mỗi trang.
                    TotalCount = totalCount // Tổng số bản ghi thỏa mãn.
                };

                return ApiResult<PagedResult<ServiceTicketListDto>>.Ok(paged); // Trả về HTTP 200 OK dạng ApiResult.
            }
            catch (Exception ex) // Bắt lỗi ngoại lệ.
            {
                _logger.LogError(ex, "Error listing tickets"); // Ghi log lỗi.
                return ApiResult<PagedResult<ServiceTicketListDto>>.Fail("Lỗi khi tải danh sách: " + ex.Message); // Trả về lỗi hệ thống.
            }
        }

        [HttpGet("my")] // Định nghĩa HTTP GET Method lấy danh sách phiếu sửa chữa của tôi (api/service-tickets/my).
        [Authorize(Roles = "Customer")] // Chỉ cho phép vai trò Customer (khách hàng đang đăng nhập) truy cập.
        public async Task<ApiResult<PagedResult<ServiceTicketListDto>>> GetMyTickets( // Lấy danh sách phiếu sửa chữa của riêng khách hàng hiện tại.
            [FromQuery] string? keyword, // Bộ lọc từ khóa.
            [FromQuery] byte? status, // Bộ lọc trạng thái.
            [FromQuery] int pageNumber = 1, // Số trang mặc định.
            [FromQuery] int pageSize = 10, // Số bản ghi mỗi trang mặc định.
            [FromQuery] string? sortBy = "date", // Sắp xếp theo ngày.
            [FromQuery] bool sortDescending = true) // Sắp xếp giảm dần.
        {
            try // Bẫy lỗi.
            {
                var userId = GetCurrentUserId(); // Lấy UserId của khách hàng đang đăng nhập.
                var (items, totalCount) = await _service.GetMyTicketsAsync( // Gọi service lấy danh sách phiếu thuộc về UserId này.
                    userId, keyword, status, pageNumber, pageSize, sortBy, sortDescending); // Truyền tham số phân trang và lọc.

                var paged = new PagedResult<ServiceTicketListDto> // Khởi tạo đối tượng phân trang.
                {
                    Items = items, // Danh sách phiếu của khách hàng.
                    PageNumber = pageNumber, // Trang hiện tại.
                    PageSize = pageSize, // Số bản ghi mỗi trang.
                    TotalCount = totalCount // Tổng số phiếu sửa chữa của khách hàng này.
                };

                return ApiResult<PagedResult<ServiceTicketListDto>>.Ok(paged); // Trả về kết quả phân trang thành công.
            }
            catch (Exception ex) // Bắt lỗi.
            {
                _logger.LogError(ex, "Error listing customer tickets"); // Ghi log.
                return ApiResult<PagedResult<ServiceTicketListDto>>.Fail("Lỗi khi tải danh sách: " + ex.Message); // Trả về lỗi hệ thống.
            }
        }

        [HttpGet("{id}")] // Định nghĩa HTTP GET Method lấy chi tiết phiếu sửa chữa (api/service-tickets/{id}).
        [Authorize(Roles = "Admin, Employee, Customer")] // Cho phép cả Admin, Employee và Customer truy cập.
        public async Task<ApiResult<ServiceTicketDetailDto>> GetTicketDetail(int id) // Lấy thông tin chi tiết một phiếu sửa chữa theo Id.
        {
            try // Bẫy lỗi.
            {
                var userId = GetCurrentUserId(); // Lấy UserId hiện tại để phân quyền xem (khách hàng chỉ được xem phiếu của chính họ).
                var ticket = await _service.GetTicketByIdAsync(id, userId, IsCustomer); // Gọi service lấy thông tin chi tiết phiếu và thực hiện kiểm tra quyền sở hữu đối với khách hàng.
                if (ticket == null) // Nếu không tìm thấy phiếu sửa chữa.
                    return ApiResult<ServiceTicketDetailDto>.Fail("Không tìm thấy phiếu."); // Trả về lỗi 404.

                return ApiResult<ServiceTicketDetailDto>.Ok(ticket); // Trả về chi tiết phiếu sửa chữa.
            }
            catch (UnauthorizedAccessException ex) // Bắt lỗi không có quyền truy cập (khách hàng xem phiếu của người khác).
            {
                return ApiResult<ServiceTicketDetailDto>.Fail(ex.Message); // Trả về lỗi không có quyền.
            }
            catch (Exception ex) // Bắt lỗi hệ thống.
            {
                _logger.LogError(ex, "Error getting ticket detail"); // Ghi log.
                return ApiResult<ServiceTicketDetailDto>.Fail("Lỗi khi tải chi tiết: " + ex.Message); // Trả về lỗi.
            }
        }

        [HttpPut("{id}/assign")] // Định nghĩa HTTP PUT Method giao phiếu cho nhân viên kỹ thuật (api/service-tickets/{id}/assign).
        [Authorize(Roles = "Admin, Employee")] // Yêu cầu vai trò điều phối là Admin hoặc Employee.
        public async Task<ApiResult<bool>> AssignTechnician(int id, [FromBody] ServiceTicketAssignDto request) // Giao phó một phiếu sửa chữa cho kỹ thuật viên xử lý.
        {
            try // Bẫy lỗi.
            {
                var userId = GetCurrentUserId(); // Lấy mã người thực hiện giao việc.
                var result = await _service.AssignTechnicianAsync(id, request.EmployeeId, userId); // Gọi service thực hiện gán kỹ thuật viên cho phiếu sửa chữa và lưu lịch sử.
                return ApiResult<bool>.Ok(result, "Giao phó kỹ thuật viên thành công."); // Trả về kết quả thành công.
            }
            catch (InvalidOperationException ex) // Bắt lỗi nghiệp vụ (ví dụ: nhân viên không có vai trò kỹ thuật hoặc phiếu đã hoàn thành).
            {
                return ApiResult<bool>.Fail(ex.Message); // Trả về lỗi nghiệp vụ.
            }
            catch (Exception ex) // Bắt lỗi hệ thống.
            {
                _logger.LogError(ex, "Error assigning technician"); // Ghi log.
                return ApiResult<bool>.Fail("Lỗi khi giao phó: " + ex.Message); // Trả về lỗi hệ thống.
            }
        }

        [HttpPut("{id}/diagnosis")] // Định nghĩa HTTP PUT Method ghi nhận kết quả chẩn đoán lỗi (api/service-tickets/{id}/diagnosis).
        [Authorize(Roles = "Admin, Employee")] // Chỉ cho phép nhân viên kỹ thuật hoặc quản trị viên ghi nhận chẩn đoán.
        public async Task<ApiResult<bool>> RecordDiagnosis(int id, [FromBody] ServiceTicketDiagnosisDto request) // Ghi chẩn đoán lỗi phần cứng, phần mềm của máy sau khi kiểm tra kỹ thuật.
        {
            try // Bẫy lỗi.
            {
                var userId = GetCurrentUserId(); // Lấy UserId người thực hiện chẩn đoán.
                var result = await _service.RecordDiagnosisAsync(id, request, userId, IsAdmin); // Gọi service lưu thông tin chẩn đoán lỗi và tình trạng linh kiện cần thay thế.
                return ApiResult<bool>.Ok(result, "Ghi nhận chẩn đoán thành công."); // Trả về thành công.
            }
            catch (UnauthorizedAccessException ex) // Lỗi phân quyền (kỹ thuật viên không được phân công sửa phiếu này).
            {
                return ApiResult<bool>.Fail(ex.Message); // Trả về lỗi phân quyền.
            }
            catch (InvalidOperationException ex) // Vi phạm trạng thái phiếu (phiếu không ở trạng thái chẩn đoán).
            {
                return ApiResult<bool>.Fail(ex.Message); // Trả về lỗi trạng thái.
            }
            catch (Exception ex) // Lỗi hệ thống khác.
            {
                _logger.LogError(ex, "Error recording diagnosis"); // Ghi log.
                return ApiResult<bool>.Fail("Lỗi khi ghi nhận: " + ex.Message); // Trả về lỗi.
            }
        }

        [HttpPut("{id}/branch")] // Định nghĩa HTTP PUT Method chọn nhánh/phương án xử lý (api/service-tickets/{id}/branch).
        [Authorize(Roles = "Admin, Employee")] // Chỉ cho phép quản trị/nhân viên điều phối hoặc kỹ thuật viên lựa chọn.
        public async Task<ApiResult<bool>> ChooseBranch(int id, [FromBody] ServiceTicketBranchDto request) // Lựa chọn phương án giải quyết (Sửa chữa phần cứng, gửi hãng sản xuất bảo hành RMA, hay đổi mới 1-1).
        {
            try // Bẫy lỗi.
            {
                var userId = GetCurrentUserId(); // Lấy UserId người thao tác.
                var result = await _service.ChooseBranchAsync(id, request, userId, IsAdmin); // Gọi service điều hướng quy trình sửa chữa theo loại giải pháp đã chọn.
                return ApiResult<bool>.Ok(result, "Chọn loại giải pháp thành công."); // Trả về kết quả thành công.
            }
            catch (UnauthorizedAccessException ex) // Lỗi phân quyền.
            {
                return ApiResult<bool>.Fail(ex.Message); // Trả về lỗi phân quyền.
            }
            catch (InvalidOperationException ex) // Lỗi nghiệp vụ.
            {
                return ApiResult<bool>.Fail(ex.Message); // Trả về lỗi nghiệp vụ.
            }
            catch (Exception ex) // Lỗi hệ thống.
            {
                _logger.LogError(ex, "Error choosing branch"); // Ghi log.
                return ApiResult<bool>.Fail("Lỗi khi chọn giải pháp: " + ex.Message); // Trả về lỗi hệ thống.
            }
        }

        [HttpPost("{id}/quotation")] // Định nghĩa HTTP POST Method tạo báo giá sửa chữa (api/service-tickets/{id}/quotation).
        [Authorize(Roles = "Admin, Employee")] // Chỉ cho phép Admin hoặc Employee tạo báo giá cho khách hàng.
        public async Task<ApiResult<QuotationDetailDto>> CreateQuotation(int id, [FromBody] QuotationCreateDto request) // Tạo báo giá chi phí linh kiện thay thế và tiền công sửa chữa.
        {
            try // Bẫy lỗi.
            {
                var userId = GetCurrentUserId(); // Lấy UserId người tạo báo giá.
                var quotation = await _service.CreateQuotationAsync(id, request, userId, IsAdmin); // Gọi service tính toán tiền công, linh kiện và sinh phiếu báo giá (trạng thái Pending).
                return ApiResult<QuotationDetailDto>.Ok(quotation, "Tạo báo giá thành công."); // Trả về thông tin báo giá vừa tạo.
            }
            catch (UnauthorizedAccessException ex) // Lỗi phân quyền.
            {
                return ApiResult<QuotationDetailDto>.Fail(ex.Message); // Trả về lỗi.
            }
            catch (InvalidOperationException ex) // Lỗi trạng thái quy trình.
            {
                return ApiResult<QuotationDetailDto>.Fail(ex.Message); // Trả về lỗi.
            }
            catch (Exception ex) // Lỗi hệ thống.
            {
                _logger.LogError(ex, "Error creating quotation"); // Ghi log.
                return ApiResult<QuotationDetailDto>.Fail("Lỗi khi tạo báo giá: " + ex.Message); // Trả về lỗi.
            }
        }

        [HttpPost("{id}/quotation/{qid}/accept")] // Định nghĩa HTTP POST Method đồng ý báo giá (api/service-tickets/{id}/quotation/{qid}/accept).
        [Authorize(Roles = "Admin, Employee, Customer")] // Cho phép cả Admin, nhân viên hoặc chính Khách hàng xác nhận đồng ý báo giá.
        public async Task<ApiResult<bool>> AcceptQuotation(int id, int qid, [FromBody] QuotationAcceptDto request) // Xác nhận đồng ý chi phí sửa chữa để kỹ thuật viên tiến hành thay thế linh kiện.
        {
            try // Bẫy lỗi.
            {
                var userId = GetCurrentUserId(); // Lấy UserId xác nhận.
                var result = await _service.AcceptQuotationAsync(id, qid, request, userId, IsAdmin); // Gọi service ghi nhận đồng ý báo giá và chuyển phiếu sửa chữa sang trạng thái chuẩn bị linh kiện/tiến hành sửa.
                return ApiResult<bool>.Ok(result, "Chấp nhận báo giá thành công."); // Trả về kết quả thành công.
            }
            catch (UnauthorizedAccessException ex) // Lỗi phân quyền xem báo giá.
            {
                return ApiResult<bool>.Fail(ex.Message); // Trả về lỗi.
            }
            catch (InvalidOperationException ex) // Lỗi báo giá không ở trạng thái chờ duyệt hoặc đã quá hạn.
            {
                return ApiResult<bool>.Fail(ex.Message); // Trả về lỗi nghiệp vụ.
            }
            catch (Exception ex) // Lỗi hệ thống.
            {
                _logger.LogError(ex, "Error accepting quotation"); // Ghi log.
                return ApiResult<bool>.Fail("Lỗi khi chấp nhận: " + ex.Message); // Trả về lỗi hệ thống.
            }
        }

        [HttpPost("{id}/quotation/{qid}/reject")] // Định nghĩa HTTP POST Method từ chối báo giá (api/service-tickets/{id}/quotation/{qid}/reject).
        [Authorize(Roles = "Admin, Employee, Customer")] // Cho phép Admin, Employee hoặc Customer từ chối báo giá.
        public async Task<ApiResult<bool>> RejectQuotation(int id, int qid, [FromBody] QuotationRejectDto request) // Từ chối chi phí sửa chữa (máy sẽ được lắp ráp lại để trả về cho khách).
        {
            try // Bẫy lỗi.
            {
                var userId = GetCurrentUserId(); // Lấy UserId thực hiện thao tác.
                var result = await _service.RejectQuotationAsync(id, qid, request, userId, IsAdmin); // Gọi service cập nhật từ chối báo giá và ghi chú lý do từ chối của khách.
                return ApiResult<bool>.Ok(result, "Từ chối báo giá thành công."); // Trả về kết quả thành công.
            }
            catch (UnauthorizedAccessException ex) // Lỗi phân quyền.
            {
                return ApiResult<bool>.Fail(ex.Message); // Trả về lỗi.
            }
            catch (InvalidOperationException ex) // Lỗi nghiệp vụ.
            {
                return ApiResult<bool>.Fail(ex.Message); // Trả về lỗi nghiệp vụ.
            }
            catch (Exception ex) // Lỗi hệ thống.
            {
                _logger.LogError(ex, "Error rejecting quotation"); // Ghi log.
                return ApiResult<bool>.Fail("Lỗi khi từ chối: " + ex.Message); // Trả về lỗi hệ thống.
            }
        }

        [HttpPost("{id}/rma")] // Định nghĩa HTTP POST Method tạo phiếu chuyển bảo hành hãng RMA (api/service-tickets/{id}/rma).
        [Authorize(Roles = "Admin, Employee")] // Chỉ cho phép nhân viên vận hành hoặc Admin tạo phiếu gửi bảo hành hãng.
        public async Task<ApiResult<RmaShipmentDetailDto>> CreateRmaShipment(int id, [FromBody] RmaShipmentCreateDto request) // Tạo phiếu vận chuyển máy lỗi đến trung tâm bảo hành của hãng sản xuất.
        {
            try // Bẫy lỗi.
            {
                var userId = GetCurrentUserId(); // Lấy UserId của nhân viên tạo phiếu RMA.
                var rma = await _service.CreateRmaShipmentAsync(id, request, userId, IsAdmin); // Gọi service tạo phiếu xuất gửi hãng và lưu mã vận đơn tracking gửi đi.
                return ApiResult<RmaShipmentDetailDto>.Ok(rma, "Tạo phiếu RMA thành công."); // Trả về chi tiết phiếu RMA.
            }
            catch (UnauthorizedAccessException ex) // Lỗi phân quyền.
            {
                return ApiResult<RmaShipmentDetailDto>.Fail(ex.Message); // Trả về lỗi.
            }
            catch (InvalidOperationException ex) // Phiếu sửa chữa không thuộc nhánh RMA hoặc không có quyền gửi đi.
            {
                return ApiResult<RmaShipmentDetailDto>.Fail(ex.Message); // Trả về lỗi.
            }
            catch (Exception ex) // Lỗi hệ thống.
            {
                _logger.LogError(ex, "Error creating RMA"); // Ghi log.
                return ApiResult<RmaShipmentDetailDto>.Fail("Lỗi khi tạo RMA: " + ex.Message); // Trả về lỗi hệ thống.
            }
        }

        [HttpPut("{id}/rma/resolution")] // Định nghĩa HTTP PUT Method cập nhật kết quả RMA từ hãng (api/service-tickets/{id}/rma/resolution).
        [Authorize(Roles = "Admin, Employee")] // Chỉ nhân viên hoặc Admin mới có quyền cập nhật kết quả trả về của hãng.
        public async Task<ApiResult<bool>> UpdateRmaResolution(int id, [FromBody] RmaResolutionUpdateDto request) // Ghi nhận phản hồi từ hãng (Ví dụ: Hãng sửa xong gửi lại thiết bị, hãng đổi máy mới khác, hay hãng từ chối bảo hành).
        {
            try // Bẫy lỗi.
            {
                var userId = GetCurrentUserId(); // Lấy UserId người ghi nhận kết quả.
                var result = await _service.RecordRmaResolutionAsync(id, request, userId, IsAdmin); // Gọi service xử lý cập nhật thông tin thiết bị trả về hoặc máy mới đổi từ hãng.
                return ApiResult<bool>.Ok(result, "Cập nhật kết quả RMA thành công."); // Trả về thành công.
            }
            catch (UnauthorizedAccessException ex) // Lỗi quyền hạn.
            {
                return ApiResult<bool>.Fail(ex.Message); // Trả về lỗi.
            }
            catch (InvalidOperationException ex) // Trạng thái phiếu hoặc thông tin RMA không khớp.
            {
                return ApiResult<bool>.Fail(ex.Message); // Trả về lỗi nghiệp vụ.
            }
            catch (Exception ex) // Lỗi hệ thống.
            {
                _logger.LogError(ex, "Error updating RMA resolution"); // Ghi log.
                return ApiResult<bool>.Fail("Lỗi khi cập nhật: " + ex.Message); // Trả về lỗi hệ thống.
            }
        }

        [HttpPost("{id}/start-repair")] // Định nghĩa HTTP POST Method bắt đầu sửa chữa thiết bị (api/service-tickets/{id}/start-repair).
        [Authorize(Roles = "Admin, Employee")] // Yêu cầu kỹ thuật viên được phân công hoặc Admin.
        public async Task<ApiResult<bool>> StartRepair(int id) // Chuyển trạng thái phiếu sửa chữa sang "Đang tiến hành sửa" (UnderRepair).
        {
            try // Bẫy lỗi.
            {
                var userId = GetCurrentUserId(); // Lấy UserId của kỹ thuật viên.
                var result = await _service.StartRepairAsync(id, userId, IsAdmin); // Gọi service kích hoạt bắt đầu sửa chữa, ghi nhận mốc thời gian thực hiện.
                return ApiResult<bool>.Ok(result, "Bắt đầu sửa chữa thành công."); // Trả về thành công.
            }
            catch (UnauthorizedAccessException ex) // Kỹ thuật viên không được phân công sửa phiếu này.
            {
                return ApiResult<bool>.Fail(ex.Message); // Trả về lỗi phân quyền.
            }
            catch (InvalidOperationException ex) // Trạng thái phiếu chưa đồng ý báo giá hoặc linh kiện chưa sẵn sàng.
            {
                return ApiResult<bool>.Fail(ex.Message); // Trả về lỗi.
            }
            catch (Exception ex) // Lỗi hệ thống.
            {
                _logger.LogError(ex, "Error starting repair"); // Ghi log.
                return ApiResult<bool>.Fail("Lỗi khi bắt đầu sửa chữa: " + ex.Message); // Trả về lỗi hệ thống.
            }
        }

        [HttpPost("{id}/swap")] // Định nghĩa HTTP POST Method đổi thiết bị mới 1-1 (api/service-tickets/{id}/swap).
        [Authorize(Roles = "Admin, Employee")] // Chỉ cho phép nhân viên vận hành hoặc Admin thực hiện đổi máy mới.
        public async Task<ApiResult<bool>> Perform1For1Swap(int id, [FromBody] Perform1For1SwapDto request) // Thực hiện đổi thiết bị lỗi lấy một máy mới từ kho hàng.
        {
            try // Bẫy lỗi.
            {
                var userId = GetCurrentUserId(); // Lấy UserId người thực hiện đổi.
                var result = await _service.Perform1For1SwapAsync(id, request.ReplacementSerialId, userId, IsAdmin); // Gọi service thực hiện xuất kho máy mới và hủy thu hồi máy lỗi.
                return ApiResult<bool>.Ok(result, "Đổi 1-1 thành công."); // Trả về thành công.
            }
            catch (UnauthorizedAccessException ex) // Quyền hạn không hợp lệ.
            {
                return ApiResult<bool>.Fail(ex.Message); // Trả về lỗi phân quyền.
            }
            catch (InvalidOperationException ex) // Lỗi serial máy mới không tồn tại trong kho hoặc phiếu không ở trạng thái đổi máy.
            {
                return ApiResult<bool>.Fail(ex.Message); // Trả về lỗi nghiệp vụ.
            }
            catch (Exception ex) // Lỗi hệ thống.
            {
                _logger.LogError(ex, "Error performing swap"); // Ghi log.
                return ApiResult<bool>.Fail("Lỗi khi đổi 1-1: " + ex.Message); // Trả về lỗi hệ thống.
            }
        }

        [HttpPost("{id}/waiting-parts")] // Định nghĩa HTTP POST Method đánh dấu chờ linh kiện (api/service-tickets/{id}/waiting-parts).
        [Authorize(Roles = "Admin, Employee")] // Yêu cầu vai trò kỹ thuật viên hoặc Admin.
        public async Task<ApiResult<bool>> MarkWaitingParts(int id) // Đánh dấu tạm dừng sửa chữa do thiếu linh kiện thay thế và phải chờ nhập về kho.
        {
            try // Bẫy lỗi.
            {
                var userId = GetCurrentUserId(); // Lấy UserId của kỹ thuật viên.
                var result = await _service.MarkWaitingPartsAsync(id, userId, IsAdmin); // Gọi service chuyển đổi trạng thái phiếu sang Chờ linh kiện (WaitingForParts).
                return ApiResult<bool>.Ok(result, "Cập nhật trạng thái thành công."); // Trả về kết quả thành công.
            }
            catch (UnauthorizedAccessException ex) // Lỗi phân quyền truy cập.
            {
                return ApiResult<bool>.Fail(ex.Message); // Trả về lỗi.
            }
            catch (InvalidOperationException ex) // Trạng thái phiếu không hợp lệ.
            {
                return ApiResult<bool>.Fail(ex.Message); // Trả về lỗi nghiệp vụ.
            }
            catch (Exception ex) // Lỗi hệ thống.
            {
                _logger.LogError(ex, "Error marking waiting parts"); // Ghi log.
                return ApiResult<bool>.Fail("Lỗi khi cập nhật: " + ex.Message); // Trả về lỗi.
            }
        }

        [HttpPost("{id}/resume-repair")] // Định nghĩa HTTP POST Method tiếp tục sửa chữa (api/service-tickets/{id}/resume-repair).
        [Authorize(Roles = "Admin, Employee")] // Chỉ kỹ thuật viên được phân công hoặc Admin.
        public async Task<ApiResult<bool>> ResumeRepair(int id) // Tiếp tục quy trình sửa chữa thiết bị sau khi linh kiện đã được nhập kho đầy đủ.
        {
            try // Bẫy lỗi.
            {
                var userId = GetCurrentUserId(); // Lấy UserId.
                var result = await _service.ResumeRepairAsync(id, userId, IsAdmin); // Gọi service đưa phiếu trở lại trạng thái Đang tiến hành sửa (UnderRepair).
                return ApiResult<bool>.Ok(result, "Tiếp tục sửa chữa thành công."); // Trả về kết quả thành công.
            }
            catch (UnauthorizedAccessException ex) // Lỗi quyền hạn.
            {
                return ApiResult<bool>.Fail(ex.Message); // Trả về lỗi.
            }
            catch (InvalidOperationException ex) // Lỗi nghiệp vụ.
            {
                return ApiResult<bool>.Fail(ex.Message); // Trả về lỗi.
            }
            catch (Exception ex) // Lỗi hệ thống.
            {
                _logger.LogError(ex, "Error resuming repair"); // Ghi log.
                return ApiResult<bool>.Fail("Lỗi khi tiếp tục: " + ex.Message); // Trả về lỗi.
            }
        }

        [HttpPost("{id}/complete")] // Định nghĩa HTTP POST Method hoàn tất sửa chữa (api/service-tickets/{id}/complete).
        [Authorize(Roles = "Admin, Employee")] // Chỉ cho phép kỹ thuật viên xử lý phiếu sửa chữa hoặc Admin thực hiện.
        public async Task<ApiResult<bool>> CompleteTicket(int id, [FromBody] ServiceTicketCompleteDto request) // Ghi nhận sửa chữa xong phần cứng và chạy kiểm thử hoạt động bình thường của máy.
        {
            try // Bẫy lỗi.
            {
                var userId = GetCurrentUserId(); // Lấy UserId.
                var result = await _service.MarkInternalRepairCompletedAsync(id, request, userId, IsAdmin); // Gọi service cập nhật trạng thái phiếu sang Sửa xong (RepairCompleted) và ghi lại mô tả hành động kỹ thuật.
                return ApiResult<bool>.Ok(result, "Hoàn tát sửa chữa thành công."); // Trả về kết quả thành công.
            }
            catch (UnauthorizedAccessException ex) // Lỗi phân quyền.
            {
                return ApiResult<bool>.Fail(ex.Message); // Trả về lỗi.
            }
            catch (InvalidOperationException ex) // Trạng thái phiếu sửa chữa không hợp lệ.
            {
                return ApiResult<bool>.Fail(ex.Message); // Trả về lỗi nghiệp vụ.
            }
            catch (Exception ex) // Lỗi hệ thống.
            {
                _logger.LogError(ex, "Error completing ticket"); // Ghi log.
                return ApiResult<bool>.Fail("Lỗi khi hoàn tất: " + ex.Message); // Trả về lỗi hệ thống.
            }
        }

        [HttpPost("{id}/invoice")] // Định nghĩa HTTP POST Method xuất hóa đơn dịch vụ (api/service-tickets/{id}/invoice).
        [Authorize(Roles = "Admin, Employee")] // Chỉ cho phép nhân viên thu ngân hoặc Admin xuất hóa đơn dịch vụ.
        public async Task<ApiResult<ServiceInvoiceDetailDto>> IssueServiceInvoice(int id, [FromBody] ServiceInvoiceCreateDto request) // Phát hành hóa đơn dịch vụ thanh toán công sửa chữa và tiền linh kiện.
        {
            try // Bẫy lỗi.
            {
                var userId = GetCurrentUserId(); // Lấy UserId người xuất hóa đơn.
                var invoice = await _service.IssueServiceInvoiceAsync(id, request, userId, IsAdmin); // Gọi service lập hóa đơn dịch vụ sửa chữa và chuẩn bị bàn giao thiết bị lại cho khách.
                return ApiResult<ServiceInvoiceDetailDto>.Ok(invoice, "Tạo hóa đơn dịch vụ thành công."); // Trả về chi tiết hóa đơn vừa tạo.
            }
            catch (UnauthorizedAccessException ex) // Lỗi phân quyền.
            {
                return ApiResult<ServiceInvoiceDetailDto>.Fail(ex.Message); // Trả về lỗi.
            }
            catch (InvalidOperationException ex) // Lỗi nghiệp vụ (ví dụ: máy chưa sửa xong hoặc đã xuất hóa đơn rồi).
            {
                return ApiResult<ServiceInvoiceDetailDto>.Fail(ex.Message); // Trả về lỗi nghiệp vụ.
            }
            catch (Exception ex) // Lỗi hệ thống.
            {
                _logger.LogError(ex, "Error issuing invoice"); // Ghi log.
                return ApiResult<ServiceInvoiceDetailDto>.Fail("Lỗi khi tạo hóa đơn: " + ex.Message); // Trả về lỗi hệ thống.
            }
        }

        [HttpPost("{id}/cancel")] // Định nghĩa HTTP POST Method hủy phiếu sửa chữa (api/service-tickets/{id}/cancel).
        [Authorize(Roles = "Admin")] // Chỉ tài khoản Admin tối cao mới có quyền hủy phiếu sửa chữa/bảo hành.
        public async Task<ApiResult<bool>> CancelTicket(int id, [FromBody] ServiceTicketCancelDto request) // Hủy phiếu sửa chữa do sự cố hoặc khách hàng rút máy đột ngột trước khi chẩn đoán.
        {
            try // Bẫy lỗi.
            {
                var userId = GetCurrentUserId(); // Lấy UserId của Admin thực hiện hủy.
                var result = await _service.CancelTicketAsync(id, request.CancelReason, userId); // Gọi service thực hiện hủy phiếu, giải phóng các tài nguyên linh kiện tạm giữ.
                return ApiResult<bool>.Ok(result, "Hủy phiếu thành công."); // Trả về kết quả thành công.
            }
            catch (InvalidOperationException ex) // Lỗi nghiệp vụ (ví dụ: phiếu đã sửa xong hoặc đã thanh toán không thể hủy).
            {
                return ApiResult<bool>.Fail(ex.Message); // Trả về lỗi nghiệp vụ.
            }
            catch (Exception ex) // Lỗi hệ thống.
            {
                _logger.LogError(ex, "Error cancelling ticket"); // Ghi log.
                return ApiResult<bool>.Fail("Lỗi khi hủy: " + ex.Message); // Trả về lỗi hệ thống.
            }
        }

        [HttpGet("{id}/history")] // Định nghĩa HTTP GET Method lấy lịch sử trạng thái phiếu sửa (api/service-tickets/{id}/history).
        [Authorize(Roles = "Admin, Employee, Customer")] // Cho phép cả Admin, nhân viên hoặc khách hàng tự tra cứu.
        public async Task<ApiResult<List<ServiceTicketStatusHistoryDto>>> GetTicketHistory(int id) // Lấy toàn bộ quá trình thay đổi trạng thái (từ lúc tiếp nhận, chẩn đoán, báo giá đến hoàn tất).
        {
            try // Bẫy lỗi.
            {
                var history = await _service.GetTicketHistoryAsync(id); // Gọi service lấy danh sách lịch sử chuyển trạng thái của phiếu sửa chữa.
                return ApiResult<List<ServiceTicketStatusHistoryDto>>.Ok(history); // Trả về danh sách lịch sử.
            }
            catch (Exception ex) // Lỗi hệ thống.
            {
                _logger.LogError(ex, "Error getting ticket history"); // Ghi log.
                return ApiResult<List<ServiceTicketStatusHistoryDto>>.Fail("Lỗi khi tải lịch sử: " + ex.Message); // Trả về lỗi hệ thống.
            }
        }

        [HttpGet("serials/{serialNumber}/repair-history")] // Định nghĩa HTTP GET Method lấy lịch sử sửa chữa của thiết bị theo số serial (api/service-tickets/serials/{serialNumber}/repair-history).
        [Authorize(Roles = "Admin, Employee")] // Chỉ nhân viên hoặc Admin mới có quyền truy cập lịch sử sửa chữa thiết bị.
        public async Task<ApiResult<List<SerialRepairHistoryDto>>> GetSerialRepairHistory(string serialNumber) // Tra cứu xem thiết bị này đã từng sửa những gì ở cửa hàng trong quá khứ.
        {
            try // Bẫy lỗi.
            {
                var history = await _service.GetSerialRepairHistoryAsync(serialNumber); // Gọi service truy vấn lịch sử sửa chữa của thiết bị theo số serial từ trước tới nay.
                return ApiResult<List<SerialRepairHistoryDto>>.Ok(history); // Trả về danh sách lịch sử sửa chữa.
            }
            catch (Exception ex) // Lỗi hệ thống.
            {
                _logger.LogError(ex, "Error getting serial repair history"); // Ghi log.
                return ApiResult<List<SerialRepairHistoryDto>>.Fail("Lỗi khi tải lịch sử: " + ex.Message); // Trả về lỗi hệ thống.
            }
        }
    }
}

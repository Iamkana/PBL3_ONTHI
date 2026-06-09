using System; // Sử dụng các kiểu dữ liệu cơ bản của hệ thống.
using System.Threading.Tasks; // Sử dụng lập trình bất đồng bộ Task.
using Microsoft.AspNetCore.Authorization; // Sử dụng phân quyền truy cập (Authorize).
using Microsoft.AspNetCore.Mvc; // Sử dụng các thành phần MVC cho Web API.
using PBL3.Application.ServiceInvoices; // Sử dụng tầng dịch vụ hóa đơn dịch vụ sửa chữa IServiceInvoiceService.
using PBL3.Shared.DTOs.Common; // Sử dụng các DTO dùng chung.
using PBL3.Shared.DTOs.ServiceTickets; // Sử dụng các DTO liên quan đến phiếu và hóa đơn dịch vụ.

namespace PBL3.API.Controllers.Admin // Khai báo namespace cho Controllers quản trị hóa đơn dịch vụ sửa chữa.
{
    [ApiController] // Khai báo đây là một API Controller có sẵn cơ chế tự động validate dữ liệu.
    [Route("api/service-invoices")] // Định nghĩa route truy cập: api/service-invoices.
    [Authorize(Roles = "Admin, Employee")] // Chỉ cho phép các vai trò Admin hoặc Employee truy cập.
    public class ServiceInvoicesController : ControllerBase // Định nghĩa lớp ServiceInvoicesController kế thừa từ ControllerBase.
    {
        private readonly IServiceInvoiceService _service; // Khai báo trường dịch vụ hóa đơn sửa chữa.
        private readonly ILogger<ServiceInvoicesController> _logger; // Khai báo trường ghi log.

        public ServiceInvoicesController(IServiceInvoiceService service, ILogger<ServiceInvoicesController> logger) // Constructor injection tiêm IServiceInvoiceService và ILogger.
        {
            _service = service; // Gán dịch vụ được tiêm.
            _logger = logger; // Gán logger.
        }

        [HttpGet] // Định nghĩa HTTP GET Method lấy danh sách hóa đơn dịch vụ sửa chữa (api/service-invoices).
        public async Task<ApiResult<PagedResult<ServiceInvoiceListDto>>> GetList( // Lấy danh sách hóa đơn phân trang có lọc.
            [FromQuery] string? keyword, // Từ khóa tìm kiếm (tên khách hàng, SĐT...) từ Query.
            [FromQuery] byte? paymentStatus, // Trạng thái thanh toán từ Query.
            [FromQuery] DateTime? fromDate, // Thời gian bắt đầu từ Query.
            [FromQuery] DateTime? toDate, // Thời gian kết thúc từ Query.
            [FromQuery] int pageNumber = 1, // Số trang mặc định là 1.
            [FromQuery] int pageSize = 10, // Kích thước trang mặc định là 10.
            [FromQuery] string? sortBy = "IssuedDate", // Trường sắp xếp mặc định là ngày phát hành.
            [FromQuery] bool sortDescending = true) // Sắp xếp giảm dần mặc định.
        {
            try // Khối bắt đầu xử lý có bẫy lỗi try-catch.
            {
                if (pageNumber < 1) pageNumber = 1; // Đảm bảo số trang tối thiểu là 1.
                if (pageSize < 1 || pageSize > 100) pageSize = 10; // Giới hạn kích thước trang từ 1 đến 100.

                var (items, totalCount) = await _service.GetPagedListAsync( // Gọi service lấy danh sách phân trang và tổng số bản ghi.
                    keyword, paymentStatus, fromDate, toDate, pageNumber, pageSize, sortBy, sortDescending); // Truyền tham số lọc.
                var result = new PagedResult<ServiceInvoiceListDto> // Khởi tạo đối tượng phân trang trả về.
                {
                    Items = items, // Danh sách hóa đơn sửa chữa của trang hiện tại.
                    PageNumber = pageNumber, // Trang hiện tại.
                    PageSize = pageSize, // Số bản ghi mỗi trang.
                    TotalCount = totalCount // Tổng số hóa đơn thỏa mãn bộ lọc.
                };
                return ApiResult<PagedResult<ServiceInvoiceListDto>>.Ok(result); // Trả về HTTP 200 OK dạng ApiResult.
            }
            catch (Exception ex) // Bắt các lỗi xảy ra trong quá trình truy vấn.
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách hóa đơn dịch vụ."); // Ghi log lỗi hệ thống kèm mô tả chi tiết.
                return ApiResult<PagedResult<ServiceInvoiceListDto>>.Fail("Lỗi khi lấy danh sách hóa đơn dịch vụ."); // Trả về ApiResult thông báo thất bại.
            }
        }

        [HttpPatch("{id:int}/mark-paid")] // Định nghĩa HTTP PATCH Method xác nhận thanh toán (api/service-invoices/{id}/mark-paid).
        public async Task<ApiResult<bool>> MarkInvoicePaid(int id) // Xác nhận khách hàng đã thanh toán hóa đơn sửa chữa.
        {
            try // Bẫy lỗi try-catch.
            {
                if (id <= 0) // Kiểm tra tính hợp lệ của Id hóa đơn.
                    return ApiResult<bool>.Fail("ID hóa đơn không hợp lệ."); // Trả về thông báo lỗi.

                await _service.MarkInvoicePaidAsync(id); // Gọi service thực hiện đánh dấu hóa đơn đã thanh toán.
                return ApiResult<bool>.Ok(true, "Đã xác nhận thanh toán hóa đơn."); // Trả về kết quả thành công.
            }
            catch (InvalidOperationException ex) // Bắt các lỗi vi phạm nghiệp vụ (ví dụ hóa đơn đã hủy hoặc đã thanh toán trước đó).
            {
                return ApiResult<bool>.Fail(ex.Message); // Trả về lỗi nghiệp vụ cho client.
            }
            catch (Exception ex) // Bắt các lỗi không xác định khác.
            {
                _logger.LogError(ex, "Lỗi khi xác nhận thanh toán hóa đơn {InvoiceId}.", id); // Ghi log lỗi.
                return ApiResult<bool>.Fail("Lỗi khi xác nhận thanh toán."); // Trả về lỗi hệ thống.
            }
        }

        [HttpGet("{id:int}")] // Định nghĩa HTTP GET Method lấy chi tiết hóa đơn (api/service-invoices/{id}).
        public async Task<ApiResult<ServiceInvoiceDetailDto>> GetById(int id) // Lấy thông tin chi tiết hóa đơn dịch vụ sửa chữa theo Id.
        {
            try // Bẫy lỗi try-catch.
            {
                if (id <= 0) // Kiểm tra tính hợp lệ của Id.
                    return ApiResult<ServiceInvoiceDetailDto>.Fail("ID hóa đơn không hợp lệ."); // Trả về lỗi.

                var invoice = await _service.GetByIdAsync(id); // Gọi service lấy chi tiết hóa đơn sửa chữa.
                if (invoice == null) // Nếu không tìm thấy hóa đơn.
                    return ApiResult<ServiceInvoiceDetailDto>.Fail("Không tìm thấy hóa đơn dịch vụ yêu cầu."); // Trả về lỗi 404.

                return ApiResult<ServiceInvoiceDetailDto>.Ok(invoice); // Trả về kết quả thành công kèm thông tin hóa đơn.
            }
            catch (Exception ex) // Bắt lỗi ngoại lệ.
            {
                _logger.LogError(ex, "Lỗi khi lấy chi tiết hóa đơn dịch vụ {InvoiceId}.", id); // Ghi log.
                return ApiResult<ServiceInvoiceDetailDto>.Fail("Lỗi khi lấy chi tiết hóa đơn dịch vụ."); // Trả về lỗi.
            }
        }
    }
}

using FluentValidation; // Sử dụng thư viện FluentValidation để kiểm tra tính hợp lệ dữ liệu.
using Microsoft.AspNetCore.Authorization; // Sử dụng để phân quyền và cấu hình xác thực (Authorize).
using Microsoft.AspNetCore.Mvc; // Sử dụng các thành phần MVC cho Web API (ApiController, Controller, HttpGet...).
using PBL3.Application.Inventory; // Sử dụng tầng dịch vụ kiểm kê kho IInventoryCheckService.
using PBL3.Shared.DTOs.Common; // Sử dụng các DTO dùng chung như ApiResult, PagedResult.
using PBL3.Shared.DTOs.Inventory; // Sử dụng các DTO liên quan đến kiểm kê kho.
using System.Security.Claims; // Sử dụng Claims để trích xuất UserId từ JWT.
using PBL3.API.Extensions; // Sử dụng extension ToActionResult.

namespace PBL3.API.Controllers.Admin // Khai báo namespace cho Controllers quản lý kiểm kê kho.
{
    [ApiController] // Khai báo đây là một API Controller có sẵn cơ chế tự động validate Model.
    [Route("api/inventory-checks")] // Định nghĩa route truy cập: api/inventory-checks.
    [Produces("application/json")] // Thiết lập định dạng dữ liệu trả về mặc định dạng JSON.
    [Authorize(Roles = "Admin, Employee")] // Yêu cầu vai trò Admin hoặc Employee mới được truy cập.
    public class InventoryChecksController : ControllerBase // Định nghĩa lớp InventoryChecksController kế thừa từ ControllerBase.
    {
        private readonly IInventoryCheckService _checkService; // Dịch vụ nghiệp vụ kiểm kê kho.
        private readonly IValidator<CreateInventoryCheckRequest> _createValidator; // Validator tạo phiếu kiểm kê.
        private readonly IValidator<ScanSerialRequest> _scanValidator; // Validator quét mã serial khi kiểm kê.
        private readonly IValidator<UpdateScanReasonRequest> _reasonValidator; // Validator cập nhật lý do chênh lệch khi quét.
        private readonly IValidator<RejectInventoryCheckRequest> _rejectValidator; // Validator từ chối phiếu kiểm kê.

        public InventoryChecksController( // Constructor injection tiêm các dịch vụ và validators từ DI container.
            IInventoryCheckService checkService, // Tiêm check service.
            IValidator<CreateInventoryCheckRequest> createValidator, // Tiêm validator tạo phiếu.
            IValidator<ScanSerialRequest> scanValidator, // Tiêm validator quét serial.
            IValidator<UpdateScanReasonRequest> reasonValidator, // Tiêm validator lý do chênh lệch.
            IValidator<RejectInventoryCheckRequest> rejectValidator) // Tiêm validator từ chối.
        {
            _checkService = checkService; // Gán check service.
            _createValidator = createValidator; // Gán validator tạo phiếu.
            _scanValidator = scanValidator; // Gán validator quét.
            _reasonValidator = reasonValidator; // Gán validator lý do.
            _rejectValidator = rejectValidator; // Gán validator từ chối.
        }

        // ─── GET LIST ───
        [HttpGet] // Định nghĩa HTTP GET Method lấy danh sách phiếu kiểm kê (api/inventory-checks).
        [ProducesResponseType(typeof(ApiResult<PagedResult<InventoryCheckListItemDto>>), StatusCodes.Status200OK)] // Thành công.
        public async Task<IActionResult> GetList([FromQuery] InventoryCheckFilterRequest filter) // Lấy danh sách phiếu kiểm kê phân trang.
        {
            var result = await _checkService.GetPagedListAsync(filter); // Gọi service lấy danh sách phân trang theo bộ lọc.
            return Ok(result); // Trả về HTTP 200 OK.
        }

        // ─── GET BY ID ───
        [HttpGet("{id:int}")] // Định nghĩa HTTP GET Method lấy chi tiết (api/inventory-checks/{id}).
        [ProducesResponseType(typeof(ApiResult<InventoryCheckDto>), StatusCodes.Status200OK)] // Tìm thấy phiếu.
        [ProducesResponseType(typeof(ApiResult<InventoryCheckDto>), StatusCodes.Status404NotFound)] // Không tìm thấy.
        public async Task<IActionResult> GetById(int id) // Lấy thông tin chi tiết phiếu kiểm kê.
        {
            var result = await _checkService.GetByIdAsync(id); // Gọi service lấy chi tiết phiếu kiểm kê.
            return result.ToActionResult(this); // Trả về kết quả qua extension ToActionResult.
        }

        // ─── DASHBOARD ───
        [HttpGet("{id:int}/dashboard")] // Định nghĩa HTTP GET Method lấy dashboard kiểm kê (api/inventory-checks/{id}/dashboard).
        [ProducesResponseType(typeof(ApiResult<InventoryCheckDashboardDto>), StatusCodes.Status200OK)] // Thành công.
        public async Task<IActionResult> GetDashboard(int id) // Lấy số liệu tổng hợp trong quá trình kiểm kê (quét được bao nhiêu, lệch bao nhiêu...).
        {
            var result = await _checkService.GetDashboardAsync(id); // Gọi service lấy dữ liệu dashboard.
            return result.ToActionResult(this); // Trả về kết quả qua extension ToActionResult.
        }

        // ─── GET SERIALS ───
        [HttpGet("{id:int}/serials")] // Định nghĩa HTTP GET Method lấy danh sách serial kiểm kê (api/inventory-checks/{id}/serials).
        [ProducesResponseType(typeof(ApiResult<PagedResult<InventoryCheckSerialDto>>), StatusCodes.Status200OK)] // Thành công.
        public async Task<IActionResult> GetSerials(int id, [FromQuery] InventoryCheckSerialFilterRequest filter) // Lấy danh sách chi tiết các mã serial đã quét hoặc hệ thống trong phiếu kiểm kê.
        {
            var result = await _checkService.GetSerialsAsync(id, filter); // Gọi service lấy danh sách serial phân trang theo bộ lọc.
            return result.ToActionResult(this); // Trả về kết quả.
        }

        // ─── CREATE ───
        [HttpPost] // Định nghĩa HTTP POST Method tạo phiếu kiểm kê (api/inventory-checks).
        [ProducesResponseType(typeof(ApiResult<InventoryCheckDto>), StatusCodes.Status201Created)] // Tạo thành công.
        [ProducesResponseType(typeof(ApiResult<InventoryCheckDto>), StatusCodes.Status400BadRequest)] // Lỗi đầu vào.
        public async Task<IActionResult> Create([FromBody] CreateInventoryCheckRequest request) // Tạo phiếu kiểm kê mới.
        {
            var validation = await _createValidator.ValidateAsync(request); // Thực hiện kiểm tra tính hợp lệ của request tạo phiếu.
            if (!validation.IsValid) // Nếu dữ liệu không hợp lệ.
            {
                var errors = string.Join("; ", validation.Errors.Select(e => e.ErrorMessage)); // Hợp nhất các lỗi thành chuỗi.
                return BadRequest(ApiResult<InventoryCheckDto>.Fail(errors)); // Trả về lỗi HTTP 400 BadRequest.
            }

            var userId = GetCurrentUserId(); // Lấy UserId của nhân viên tạo phiếu.
            if (userId == null) // Nếu chưa đăng nhập.
                return Unauthorized(ApiResult<InventoryCheckDto>.Fail("Người dùng chưa đăng nhập.")); // Trả về lỗi 401 Unauthorized.

            var result = await _checkService.CreateAsync(request, userId.Value); // Gọi service tạo phiếu kiểm kê mới (trạng thái tạm - Draft).
            if (!result.Success) return result.ToActionResult(this); // Nếu tạo thất bại, trả về lỗi nghiệp vụ.

            return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result); // Trả về 201 Created kèm link xem chi tiết.
        }

        // ─── SCAN SERIAL ───
        [HttpPost("{id:int}/scan")] // Định nghĩa HTTP POST Method quét mã serial (api/inventory-checks/{id}/scan).
        [ProducesResponseType(typeof(ApiResult<ScanResultDto>), StatusCodes.Status200OK)] // Quét thành công.
        [ProducesResponseType(typeof(ApiResult<ScanResultDto>), StatusCodes.Status400BadRequest)] // Quét lỗi.
        public async Task<IActionResult> ScanSerial(int id, [FromBody] ScanSerialRequest request) // Quét mã serial kiểm tra thực tế trong kho.
        {
            var validation = await _scanValidator.ValidateAsync(request); // Thực hiện kiểm tra request quét serial.
            if (!validation.IsValid) // Nếu request lỗi.
            {
                var errors = string.Join("; ", validation.Errors.Select(e => e.ErrorMessage)); // Gom lỗi.
                return BadRequest(ApiResult<ScanResultDto>.Fail(errors)); // Trả về lỗi 400.
            }

            var userId = GetCurrentUserId(); // Lấy UserId.
            if (userId == null) // Nếu chưa đăng nhập.
                return Unauthorized(ApiResult<ScanResultDto>.Fail("Người dùng chưa đăng nhập.")); // Trả về 401.

            var result = await _checkService.ScanSerialAsync(id, request, userId.Value); // Gọi service ghi nhận việc quét serial (cập nhật trạng thái Tìm thấy / Thừa / Thiếu).
            if (!result.Success && result.Data == null) // Nếu thất bại hoàn toàn không có dữ liệu trả về.
                return result.ToActionResult(this); // Map lỗi HTTP tương ứng.
            return Ok(result); // Trả về HTTP 200 OK kèm chi tiết kết quả quét.
        }

        // ─── MARK DEFECTIVE ───
        [HttpPut("{id:int}/serials/{detailSerialId:int}/mark-defective")] // Định nghĩa HTTP PUT Method đánh dấu hỏng (api/inventory-checks/{id}/serials/{detailSerialId}/mark-defective).
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status200OK)] // Đánh dấu thành công.
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status400BadRequest)] // Thất bại.
        public async Task<IActionResult> MarkDefective(int id, int detailSerialId) // Đánh dấu một sản phẩm kiểm kê thực tế bị lỗi/hỏng.
        {
            var userId = GetCurrentUserId(); // Lấy UserId.
            if (userId == null) // Nếu chưa đăng nhập.
                return Unauthorized(ApiResult<bool>.Fail("Người dùng chưa đăng nhập.")); // Trả về lỗi 401.

            var result = await _checkService.MarkDefectiveAsync(id, detailSerialId, userId.Value); // Gọi service cập nhật trạng thái lỗi hỏng cho serial trong phiếu kiểm kê.
            return result.ToActionResult(this); // Trả về kết quả.
        }

        // ─── UPDATE REASON ───
        [HttpPut("{id:int}/serials/{detailSerialId:int}/reason")] // Định nghĩa HTTP PUT Method cập nhật lý do chênh lệch (api/inventory-checks/{id}/serials/{detailSerialId}/reason).
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status200OK)] // Cập nhật thành công.
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status400BadRequest)] // Cập nhật lỗi.
        public async Task<IActionResult> UpdateReason(int id, int detailSerialId, [FromBody] UpdateScanReasonRequest request) // Ghi chú lý do thừa/thiếu sản phẩm.
        {
            var validation = await _reasonValidator.ValidateAsync(request); // Kiểm tra request cập nhật lý do.
            if (!validation.IsValid) // Nếu dữ liệu không hợp lệ.
            {
                var errors = string.Join("; ", validation.Errors.Select(e => e.ErrorMessage)); // Gom lỗi.
                return BadRequest(ApiResult<bool>.Fail(errors)); // Trả về lỗi 400.
            }

            var userId = GetCurrentUserId(); // Lấy UserId.
            if (userId == null) // Nếu chưa đăng nhập.
                return Unauthorized(ApiResult<bool>.Fail("Người dùng chưa đăng nhập.")); // Trả về lỗi 401.

            var result = await _checkService.UpdateReasonAsync(id, detailSerialId, request, userId.Value); // Gọi service cập nhật lý do chênh lệch.
            return result.ToActionResult(this); // Trả về kết quả.
        }

        // ─── SUBMIT ───
        [HttpPost("{id:int}/submit")] // Định nghĩa HTTP POST Method gửi duyệt phiếu kiểm kê (api/inventory-checks/{id}/submit).
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status200OK)] // Gửi thành công.
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status400BadRequest)] // Gửi lỗi.
        public async Task<IActionResult> Submit(int id) // Nhân viên gửi duyệt sau khi hoàn thành quét kiểm kê thực tế.
        {
            var userId = GetCurrentUserId(); // Lấy UserId.
            if (userId == null) // Nếu chưa đăng nhập.
                return Unauthorized(ApiResult<bool>.Fail("Người dùng chưa đăng nhập.")); // Trả về lỗi 401.

            var result = await _checkService.SubmitAsync(id, userId.Value); // Gọi service chuyển trạng thái phiếu sang Chờ duyệt (PendingApproval).
            return result.ToActionResult(this); // Trả về kết quả.
        }

        // ─── APPROVE (Admin only) ───
        [HttpPost("{id:int}/approve")] // Định nghĩa HTTP POST Method phê duyệt (api/inventory-checks/{id}/approve).
        [Authorize(Roles = "Admin")] // Chỉ tài khoản Admin mới có quyền phê duyệt phiếu kiểm kê.
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status200OK)] // Duyệt thành công.
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status400BadRequest)] // Duyệt lỗi.
        public async Task<IActionResult> Approve(int id) // Admin phê duyệt kết quả kiểm kê.
        {
            var userId = GetCurrentUserId(); // Lấy UserId.
            if (userId == null) // Nếu chưa đăng nhập.
                return Unauthorized(ApiResult<bool>.Fail("Người dùng chưa đăng nhập.")); // Trả về lỗi 401.

            var result = await _checkService.ApproveAsync(id, userId.Value); // Gọi service thực hiện đồng bộ lại tồn kho thực tế, lưu Log chênh lệch (Approved).
            return result.ToActionResult(this); // Trả về kết quả.
        }

        // ─── REJECT (Admin only) ───
        [HttpPost("{id:int}/reject")] // Định nghĩa HTTP POST Method từ chối phê duyệt (api/inventory-checks/{id}/reject).
        [Authorize(Roles = "Admin")] // Chỉ Admin mới có quyền từ chối phê duyệt.
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status200OK)] // Từ chối thành công.
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status400BadRequest)] // Từ chối lỗi.
        public async Task<IActionResult> Reject(int id, [FromBody] RejectInventoryCheckRequest request) // Admin từ chối phê duyệt phiếu kiểm kê kèm lý do từ chối.
        {
            var validation = await _rejectValidator.ValidateAsync(request); // Kiểm tra lý do từ chối có được gửi lên không.
            if (!validation.IsValid) // Nếu không hợp lệ.
            {
                var errors = string.Join("; ", validation.Errors.Select(e => e.ErrorMessage)); // Gom lỗi.
                return BadRequest(ApiResult<bool>.Fail(errors)); // Trả về lỗi 400.
            }

            var userId = GetCurrentUserId(); // Lấy UserId.
            if (userId == null) // Nếu chưa đăng nhập.
                return Unauthorized(ApiResult<bool>.Fail("Người dùng chưa đăng nhập.")); // Trả về lỗi 401.

            var result = await _checkService.RejectAsync(id, request, userId.Value); // Gọi service chuyển trạng thái phiếu sang Bị từ chối (Rejected).
            return result.ToActionResult(this); // Trả về kết quả.
        }

        // ─── CANCEL ───
        [HttpPost("{id:int}/cancel")] // Định nghĩa HTTP POST Method hủy bỏ phiếu kiểm kê (api/inventory-checks/{id}/cancel).
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status200OK)] // Hủy thành công.
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status400BadRequest)] // Hủy lỗi.
        public async Task<IActionResult> Cancel(int id) // Hủy phiếu kiểm kê khi không tiến hành nữa.
        {
            var userId = GetCurrentUserId(); // Lấy UserId.
            if (userId == null) // Nếu chưa đăng nhập.
                return Unauthorized(ApiResult<bool>.Fail("Người dùng chưa đăng nhập.")); // Trả về lỗi 401.

            var isAdmin = User.IsInRole("Admin"); // Kiểm tra xem người yêu cầu hủy có phải Admin không.
            var result = await _checkService.CancelAsync(id, userId.Value, isAdmin); // Gọi service thực hiện hủy phiếu kiểm kê.
            return result.ToActionResult(this); // Trả về kết quả.
        }

        // ─── HELPERS ───
        private Guid? GetCurrentUserId() // Hàm phụ trợ lấy UserId.
        {
            var idStr = User.FindFirstValue(ClaimTypes.NameIdentifier); // Tìm Claim NameIdentifier.
            if (string.IsNullOrEmpty(idStr) || !Guid.TryParse(idStr, out var userId)) // Kiểm tra định dạng Guid.
                return null; // Không hợp lệ trả về null.
            return userId; // Trả về Guid hợp lệ.
        }
    }
}

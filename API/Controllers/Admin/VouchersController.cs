using System.Security.Claims; // Sử dụng để truy xuất các Claims định danh người dùng.
using Microsoft.AspNetCore.Authorization; // Sử dụng phân quyền truy cập.
using Microsoft.AspNetCore.Mvc; // Sử dụng các thành phần MVC cho Web API.
using PBL3.Application.Vouchers; // Sử dụng tầng dịch vụ Voucher IVoucherService.
using PBL3.Shared.DTOs.Common; // Sử dụng các DTO dùng chung.
using PBL3.Shared.DTOs.Vouchers; // Sử dụng các DTO liên quan đến voucher.
using PBL3.API.Extensions; // Sử dụng extension ToActionResult.

namespace PBL3.API.Controllers.Admin // Khai báo namespace cho Controllers quản lý khuyến mãi/voucher.
{
    [ApiController] // Khai báo lớp là một API Controller có sẵn cơ chế tự động validate dữ liệu.
    [Route("api/[controller]")] // Định nghĩa route truy cập: api/vouchers.
    [Produces("application/json")] // Thiết lập định dạng dữ liệu trả về mặc định dạng JSON.
    public class VouchersController : ControllerBase // Định nghĩa lớp VouchersController kế thừa từ ControllerBase.
    {
        private readonly IVoucherService _voucherService; // Khai báo trường dịch vụ voucher.

        public VouchersController(IVoucherService voucherService) // Constructor injection tiêm IVoucherService.
        {
            _voucherService = voucherService; // Gán dịch vụ được tiêm.
        }

        /// <summary>
        /// Lấy danh sách voucher có phân trang, hỗ trợ tìm kiếm theo Code/Tên, lọc trạng thái và khoảng thời gian.
        /// </summary>
        [HttpGet] // Định nghĩa HTTP GET Method lấy danh sách (api/vouchers).
        [Authorize(Roles = "Admin, Employee")] // Yêu cầu vai trò Admin hoặc Employee được phép truy cập.
        [ProducesResponseType(typeof(ApiResult<PagedResult<VoucherDto>>), StatusCodes.Status200OK)] // Trả về danh sách voucher phân trang.
        public async Task<IActionResult> GetList([FromQuery] VoucherFilterRequest filter) // Lọc danh sách dựa trên tham số Query.
        {
            var result = await _voucherService.GetPagedListAsync(filter); // Gọi service lấy danh sách voucher phân trang.
            return Ok(result); // Trả về HTTP 200 OK.
        }

        /// <summary>
        /// Lấy chi tiết voucher theo Id.
        /// </summary>
        [HttpGet("{id:int}")] // Định nghĩa HTTP GET Method lấy chi tiết (api/vouchers/{id}).
        [Authorize(Roles = "Admin, Employee")] // Yêu cầu quyền quản lý.
        [ProducesResponseType(typeof(ApiResult<VoucherDto>), StatusCodes.Status200OK)] // Thành công.
        [ProducesResponseType(typeof(ApiResult<VoucherDto>), StatusCodes.Status404NotFound)] // Không tìm thấy.
        public async Task<IActionResult> GetById(int id) // Lấy thông tin chi tiết voucher theo Id.
        {
            var result = await _voucherService.GetByIdAsync(id); // Gọi service lấy chi tiết voucher.
            return result.ToActionResult(this); // Trả về kết quả qua extension ToActionResult.
        }

        /// <summary>
        /// Tạo mới voucher. Chỉ Admin được phép.
        /// </summary>
        [HttpPost] // Định nghĩa HTTP POST Method thêm voucher mới (api/vouchers).
        [Authorize(Roles = "Admin")] // Chỉ tài khoản Admin mới được tạo voucher mới.
        [ProducesResponseType(typeof(ApiResult<VoucherDto>), StatusCodes.Status201Created)] // Tạo thành công trả về 201 Created.
        [ProducesResponseType(typeof(ApiResult<VoucherDto>), StatusCodes.Status400BadRequest)] // Đầu vào không hợp lệ.
        public async Task<IActionResult> Create([FromBody] CreateVoucherRequest request) // Nhận thông tin voucher từ Request Body.
        {
            var result = await _voucherService.CreateAsync(request); // Gọi service tạo voucher mới.

            if (!result.Success) return result.ToActionResult(this); // Trả về kết quả lỗi tương ứng nếu thất bại.

            return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result); // Trả về HTTP 201 Created kèm route lấy chi tiết.
        }

        /// <summary>
        /// Cập nhật voucher. Chỉ Admin được phép. Mã Code không thể thay đổi.
        /// </summary>
        [HttpPut("{id:int}")] // Định nghĩa HTTP PUT Method cập nhật voucher (api/vouchers/{id}).
        [Authorize(Roles = "Admin")] // Chỉ tài khoản Admin mới được cập nhật voucher.
        [ProducesResponseType(typeof(ApiResult<VoucherDto>), StatusCodes.Status200OK)] // Thành công.
        [ProducesResponseType(typeof(ApiResult<VoucherDto>), StatusCodes.Status400BadRequest)] // Lỗi đầu vào/nghiệp vụ.
        [ProducesResponseType(typeof(ApiResult<VoucherDto>), StatusCodes.Status404NotFound)] // Không tìm thấy.
        public async Task<IActionResult> Update(int id, [FromBody] UpdateVoucherRequest request) // Cập nhật voucher theo Id.
        {
            var result = await _voucherService.UpdateAsync(id, request); // Gọi service thực hiện cập nhật thông tin voucher.
            return result.ToActionResult(this); // Trả về kết quả qua extension ToActionResult.
        }

        /// <summary>
        /// Xóa mềm voucher. Chỉ Admin được phép.
        /// </summary>
        [HttpDelete("{id:int}")] // Định nghĩa HTTP DELETE Method xóa mềm voucher (api/vouchers/{id}).
        [Authorize(Roles = "Admin")] // Chỉ Admin được xóa.
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status200OK)] // Xóa thành công.
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status404NotFound)] // Không tìm thấy.
        public async Task<IActionResult> Delete(int id) // Xóa mềm voucher theo Id.
        {
            var result = await _voucherService.DeleteAsync(id); // Gọi service thực hiện xóa mềm.
            return result.ToActionResult(this); // Trả về kết quả qua extension ToActionResult.
        }

        /// <summary>
        /// Bật/tắt trạng thái hoạt động của voucher. Chỉ Admin được phép.
        /// </summary>
        [HttpPatch("{id:int}/toggle-status")] // Định nghĩa HTTP PATCH Method thay đổi trạng thái (api/vouchers/{id}/toggle-status).
        [Authorize(Roles = "Admin")] // Chỉ tài khoản Admin được cấu hình.
        [ProducesResponseType(typeof(ApiResult<VoucherDto>), StatusCodes.Status200OK)] // Thành công.
        [ProducesResponseType(typeof(ApiResult<VoucherDto>), StatusCodes.Status404NotFound)] // Không tìm thấy.
        public async Task<IActionResult> ToggleStatus(int id) // Bật/tắt trạng thái hoạt động của voucher theo Id.
        {
            var result = await _voucherService.ToggleStatusAsync(id); // Gọi service kích hoạt/vô hiệu hóa voucher.
            return result.ToActionResult(this); // Trả về kết quả qua extension ToActionResult.
        }

        /// <summary>
        /// Lấy danh sách tất cả voucher kèm thông tin có thể áp dụng cho đơn hàng.
        /// AllowAnonymous — nếu đã đăng nhập sẽ kiểm tra MaxUsesPerUser.
        /// </summary>
        [HttpPost("available-for-order")] // Định nghĩa HTTP POST Method lấy voucher khả dụng cho đơn (api/vouchers/available-for-order).
        [AllowAnonymous] // Cho phép cả người dùng chưa đăng nhập gọi API để xem danh sách voucher khả dụng.
        [ProducesResponseType(typeof(ApiResult<List<VoucherAvailabilityDto>>), StatusCodes.Status200OK)] // Trả về danh sách voucher có cờ khả dụng.
        public async Task<IActionResult> GetAvailableForOrder([FromBody] GetAvailableVouchersRequest request) // Nhận thông tin đơn hàng hiện tại từ body.
        {
            Guid? userId = null; // Khởi tạo UserId là null nếu là khách vãng lai.
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; // Trích xuất UserId từ Claims (nếu có).
            if (!string.IsNullOrEmpty(claim) && Guid.TryParse(claim, out var id)) // Nếu định danh hợp lệ.
                userId = id; // Gán UserId của người dùng đang đăng nhập.

            var result = await _voucherService.GetAvailableForOrderAsync(request, userId); // Gọi service lấy danh sách các voucher phù hợp giá trị đơn hàng và số lượt dùng của user.
            return Ok(result); // Trả về HTTP 200 OK.
        }

        /// <summary>
        /// Kiểm tra tính hợp lệ của mã voucher và preview số tiền giảm trước khi đặt hàng.
        /// Không yêu cầu đăng nhập, nhưng nếu đã đăng nhập sẽ kiểm tra MaxUsesPerUser.
        /// </summary>
        [HttpPost("validate-code")] // Định nghĩa HTTP POST Method kiểm tra mã voucher (api/vouchers/validate-code).
        [AllowAnonymous] // Cho phép tất cả người dùng (hoặc khách) kiểm tra mã.
        [ProducesResponseType(typeof(ApiResult<ValidateVoucherResponse>), StatusCodes.Status200OK)] // Trả về kết quả xác thực.
        public async Task<IActionResult> ValidateCode([FromBody] ValidateVoucherRequest request) // Nhận mã voucher và giá trị đơn hàng hiện tại.
        {
            Guid? userId = null; // Khởi tạo UserId là null.
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; // Lấy UserId từ Claims (nếu có).
            if (!string.IsNullOrEmpty(userIdClaim) && Guid.TryParse(userIdClaim, out var parsedId)) // Nếu định dạng Guid hợp lệ.
                userId = parsedId; // Gán UserId của người dùng đang đăng nhập.

            var result = await _voucherService.ValidateVoucherCodeAsync(request, userId); // Gọi service xác thực mã voucher và tính toán giá trị giảm giá.
            return Ok(result); // Trả về HTTP 200 OK.
        }
    }
}

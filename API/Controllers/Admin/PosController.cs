using Microsoft.AspNetCore.Authorization; // Sử dụng phân quyền truy cập.
using Microsoft.AspNetCore.Mvc; // Sử dụng các thành phần MVC cho Web API.
using PBL3.Application.Pos; // Sử dụng tầng dịch vụ bán hàng tại quầy IPosService.
using PBL3.Shared.DTOs.Pos; // Sử dụng các DTO liên quan đến POS.
using System; // Sử dụng các kiểu dữ liệu cơ bản của hệ thống.
using System.Security.Claims; // Sử dụng Claims để lấy mã người dùng hiện tại từ token JWT.
using System.Threading.Tasks; // Sử dụng lập trình bất đồng bộ Task.
using PBL3.API.Extensions; // Sử dụng các phương thức mở rộng.

namespace PBL3.API.Controllers.Admin // Khai báo namespace cho Controllers thuộc khu vực quản trị/bán hàng tại quầy.
{
    [Route("api/[controller]")] // Định nghĩa route truy cập mặc định: api/pos.
    [ApiController] // Khai báo lớp là một API Controller có cơ chế tự động validate dữ liệu.
    [Authorize(Roles = "Admin,Employee")] // Chỉ cho phép vai trò Admin hoặc Employee được phép truy cập.
    public class PosController : ControllerBase // Định nghĩa lớp PosController kế thừa từ ControllerBase.
    {
        private readonly IPosService _posService; // Khai báo trường dịch vụ bán hàng tại quầy (POS).

        public PosController(IPosService posService) // Constructor injection tiêm IPosService từ DI container.
        {
            _posService = posService; // Gán dịch vụ được tiêm.
        }

        private Guid GetCurrentUserId() // Hàm phụ trợ nội bộ để lấy mã UserId của nhân viên đang đăng nhập.
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; // Tìm và lấy giá trị của Claim NameIdentifier (UserId).
            if (Guid.TryParse(idClaim, out Guid id)) return id; // Nếu ép kiểu sang Guid thành công thì trả về Id.
            return Guid.Empty; // Ngược lại trả về Guid.Empty.
        }

        [HttpPost("scan")] // Định nghĩa HTTP POST Method quét mã vạch (api/pos/scan).
        public async Task<IActionResult> ScanSerial([FromBody] PosScanRequest request) // Hành động quét mã serial của sản phẩm khi tính tiền tại quầy.
        {
            var result = await _posService.ScanSerialAsync(request.SerialNumber); // Gọi service tìm thông tin sản phẩm dựa trên số serial vừa quét.
            return result.ToActionResult(this); // Trả về kết quả qua extension ToActionResult.
        }

        [HttpGet("customer")] // Định nghĩa HTTP GET Method tra cứu khách hàng (api/pos/customer).
        public async Task<IActionResult> LookupCustomer([FromQuery] string phone) // Tra cứu khách hàng thành viên tại quầy bằng số điện thoại.
        {
            var result = await _posService.LookupCustomerAsync(phone); // Gọi service tìm kiếm thông tin khách hàng dựa trên SĐT.
            return result.ToActionResult(this); // Trả về kết quả qua extension ToActionResult.
        }

        [HttpPost("voucher/validate")] // Định nghĩa HTTP POST Method kiểm tra voucher tại quầy (api/pos/voucher/validate).
        public async Task<IActionResult> ValidateVoucher([FromQuery] string code, [FromQuery] decimal subTotal) // Xác thực mã giảm giá dựa trên mã code và tổng tiền hóa đơn tạm tính.
        {
            var result = await _posService.ValidateVoucherAsync(code, subTotal); // Gọi service xác thực voucher và tính số tiền được giảm.
            return result.ToActionResult(this); // Trả về kết quả qua extension ToActionResult.
        }

        [HttpPost("checkout")] // Định nghĩa HTTP POST Method thanh toán tại quầy (api/pos/checkout).
        public async Task<IActionResult> Checkout([FromBody] PosCheckoutRequest request) // Thực hiện thanh toán và in hóa đơn tại quầy.
        {
            var employeeId = GetCurrentUserId(); // Lấy UserId của nhân viên đang thực hiện giao dịch.
            var result = await _posService.CheckoutAsync(request, employeeId); // Gọi service tạo đơn hàng trực tiếp tại quầy (POS).
            return result.ToActionResult(this); // Trả về kết quả qua extension ToActionResult.
        }

        [HttpPost("drafts")] // Định nghĩa HTTP POST Method lưu hóa đơn nháp (api/pos/drafts).
        public async Task<IActionResult> SaveDraft([FromBody] PosCheckoutRequest request) // Lưu hóa đơn đang tính tiền dở dang vào danh sách nháp để phục vụ khách khác trước.
        {
            var employeeId = GetCurrentUserId(); // Lấy UserId của nhân viên.
            var result = await _posService.SaveDraftAsync(request, employeeId); // Gọi service lưu hóa đơn nháp.
            return result.ToActionResult(this); // Trả về kết quả qua extension ToActionResult.
        }

        [HttpGet("drafts")] // Định nghĩa HTTP GET Method lấy danh sách đơn nháp (api/pos/drafts).
        public async Task<IActionResult> GetDrafts() // Lấy danh sách các hóa đơn nháp của nhân viên hiện tại.
        {
            var employeeId = GetCurrentUserId(); // Lấy UserId của nhân viên.
            var result = await _posService.GetDraftsAsync(employeeId); // Gọi service lấy danh sách hóa đơn nháp.
            return result.ToActionResult(this); // Trả về kết quả.
        }

        [HttpGet("drafts/{id}")] // Định nghĩa HTTP GET Method lấy chi tiết đơn nháp theo Id (api/pos/drafts/{id}).
        public async Task<IActionResult> GetDraftById(int id) // Phục hồi hóa đơn nháp theo Id để tiếp tục thanh toán.
        {
            var employeeId = GetCurrentUserId(); // Lấy UserId của nhân viên.
            var result = await _posService.GetDraftByIdAsync(id, employeeId); // Gọi service lấy chi tiết hóa đơn nháp.
            return result.ToActionResult(this); // Trả về kết quả.
        }

        [HttpDelete("drafts/{id}")] // Định nghĩa HTTP DELETE Method xóa đơn nháp theo Id (api/pos/drafts/{id}).
        public async Task<IActionResult> DeleteDraft(int id) // Xóa hóa đơn nháp khỏi danh sách chờ.
        {
            var employeeId = GetCurrentUserId(); // Lấy UserId của nhân viên.
            var result = await _posService.DeleteDraftAsync(id, employeeId); // Gọi service thực hiện xóa đơn nháp.
            return result.ToActionResult(this); // Trả về kết quả.
        }
    }
}

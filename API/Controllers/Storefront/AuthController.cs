using Microsoft.AspNetCore.Authorization; // Sử dụng phân quyền truy cập.
using Microsoft.AspNetCore.Mvc; // Sử dụng các thành phần MVC.
using Microsoft.AspNetCore.RateLimiting; // Sử dụng middleware giới hạn tần suất.
using PBL3.Application.Auth; // Sử dụng tầng dịch vụ xác thực IAuthService.
using PBL3.Shared.DTOs.Auth; // Sử dụng các DTO liên quan đến xác thực.
using PBL3.Shared.DTOs.Common; // Sử dụng các DTO dùng chung như ApiResult.
using PBL3.Shared.DTOs.Customers; // Sử dụng các DTO liên quan đến khách hàng.
using System.Security.Claims; // Sử dụng Claims để xác định danh tính.
using PBL3.API.Extensions; // Sử dụng các phương thức mở rộng.

namespace PBL3.API.Controllers.Storefront // Khai báo namespace cho Controllers thuộc Storefront (Cửa hàng trực tuyến).
{
    [Route("api/[controller]")] // Định nghĩa route truy cập: api/auth.
    [ApiController] // Khai báo đây là một API Controller có tự động validate dữ liệu đầu vào.
    public class AuthController : ControllerBase // Định nghĩa lớp AuthController kế thừa từ ControllerBase.
    {
        private readonly IAuthService _authService; // Khai báo service quản lý xác thực.

        public AuthController(IAuthService authService) // Constructor injection tiêm IAuthService.
        {
            _authService = authService; // Gán service được tiêm.
        }

        /// <summary>
        /// Đăng nhập: Nhận Email + Password, trả về cặp Access Token + Refresh Token.
        /// </summary>
        [HttpPost("login")] // Định nghĩa POST Method cho endpoint api/auth/login.
        [EnableRateLimiting("LoginRateLimit")] // Kích hoạt giới hạn tần suất theo chính sách "LoginRateLimit" để chống tấn công dò mật khẩu.
        [ProducesResponseType(typeof(ApiResult<TokenResponse>), StatusCodes.Status200OK)] // Thành công.
        [ProducesResponseType(typeof(ApiResult<TokenResponse>), StatusCodes.Status400BadRequest)] // Thất bại.
        public async Task<IActionResult> Login([FromBody] LoginRequest request) // Đăng nhập nhận tham số Email, Mật khẩu từ Body.
        {
            var result = await _authService.LoginAsync(request); // Gọi service xử lý đăng nhập.
            return result.ToActionResult(this); // Ánh xạ ApiResult sang HTTP Response tương ứng.
        }

        /// <summary>
        /// Làm mới Token: Nhận cặp Access Token (hết hạn) + Refresh Token, trả về cặp Token mới.
        /// </summary>
        [HttpPost("refresh-token")] // Định nghĩa POST Method cho endpoint api/auth/refresh-token.
        [ProducesResponseType(typeof(ApiResult<TokenResponse>), StatusCodes.Status200OK)] // Thành công.
        [ProducesResponseType(typeof(ApiResult<TokenResponse>), StatusCodes.Status400BadRequest)] // Thất bại.
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request) // Làm mới JWT token bằng Refresh Token.
        {
            var result = await _authService.RefreshTokenAsync(request); // Gọi service xử lý tạo Token mới.
            return result.ToActionResult(this); // Trả về kết quả qua extension ToActionResult.
        }

        /// <summary>
        /// Đổi mật khẩu cho người dùng đang đăng nhập.
        /// </summary>
        [HttpPut("change-password")] // Định nghĩa PUT Method cho endpoint api/auth/change-password.
        [Authorize] // Yêu cầu người dùng phải đăng nhập mới được gọi hành động này.
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status200OK)] // Thành công.
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status400BadRequest)] // Thất bại.
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request) // Đổi mật khẩu tài khoản hiện tại.
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier); // Lấy UserId dạng chuỗi từ các Claims trong JWT token hiện tại.
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId)) // Kiểm tra xem UserId có hợp lệ không.
            {
                return Unauthorized(ApiResult<bool>.Fail("Người dùng chưa đăng nhập.")); // Trả về lỗi 401 Unauthorized nếu không tìm thấy UserId hợp lệ.
            }

            if (request.NewPassword != request.ConfirmPassword) // Kiểm tra mật khẩu mới và mật khẩu xác nhận có trùng khớp không.
            {
                return BadRequest(ApiResult<bool>.Fail("Mật khẩu xác nhận không khớp.")); // Trả về lỗi 400 BadRequest nếu không khớp.
            }

            var result = await _authService.ChangePasswordAsync(userId, request.CurrentPassword, request.NewPassword); // Gọi service đổi mật khẩu trong DB.
            return result.ToActionResult(this); // Trả về kết quả qua extension ToActionResult.
        }

        /// <summary>
        /// Đăng ký tài khoản (UC001): Khách hàng tự đăng ký. Trả về thông báo thành công (yêu cầu khách tự đăng nhập).
        /// </summary>
        [HttpPost("register")] // Định nghĩa POST Method cho endpoint api/auth/register.
        [AllowAnonymous] // Cho phép khách vãng lai chưa đăng nhập gọi endpoint này để đăng ký tài khoản mới.
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status200OK)] // Thành công.
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status400BadRequest)] // Thất bại.
        public async Task<IActionResult> Register([FromBody] RegisterCustomerRequest request) // Đăng ký tài khoản khách hàng.
        {
            var result = await _authService.RegisterAsync(request); // Gọi service thực hiện đăng ký khách hàng mới.
            return result.ToActionResult(this); // Trả về kết quả qua extension ToActionResult.
        }
    }
}

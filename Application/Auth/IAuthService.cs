using PBL3.Shared.DTOs.Auth; // Sử dụng các DTO xác thực.
using PBL3.Shared.DTOs.Common; // Sử dụng DTO dùng chung ApiResult.
using PBL3.Shared.DTOs.Customers; // Sử dụng DTO đăng ký khách hàng.

namespace PBL3.Application.Auth // Khai báo namespace cho tầng Application của module xác thực.
{
    public interface IAuthService // Định nghĩa giao diện dịch vụ xác thực IAuthService.
    {
        Task<ApiResult<TokenResponse>> LoginAsync(LoginRequest request); // Khai báo phương thức đăng nhập và trả về cặp Token.
        Task<ApiResult<TokenResponse>> RefreshTokenAsync(RefreshTokenRequest request); // Khai báo phương thức cấp mới Token bằng Refresh Token.
        Task<ApiResult<bool>> RegisterAsync(RegisterCustomerRequest request); // Khai báo phương thức khách hàng đăng ký tài khoản mới.
        Task<ApiResult<bool>> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword); // Khai báo phương thức đổi mật khẩu cho người dùng.
    }
}

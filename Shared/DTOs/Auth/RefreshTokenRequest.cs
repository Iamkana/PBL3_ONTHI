using FluentValidation; // Nhập thư viện FluentValidation để hỗ trợ kiểm định dữ liệu người dùng nhập vào.

namespace PBL3.Shared.DTOs.Auth // Định nghĩa namespace PBL3.Shared.DTOs.Auth quản lý cấu trúc code truyền tải và validator.
{
    /// <summary>
    /// DTO gửi lên để xin cấp lại cặp Token mới khi Access Token hết hạn.
    /// </summary>
    public class RefreshTokenRequest // Định nghĩa lớp DTO truyền tải dữ liệu RefreshTokenRequest.
    {
        public string AccessToken { get; set; } = string.Empty; // Thuộc tính AccessToken kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public string RefreshToken { get; set; } = string.Empty; // Thuộc tính RefreshToken kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
    }

    public class RefreshTokenRequestValidator : AbstractValidator<RefreshTokenRequest> // Định nghĩa lớp kiểm định dữ liệu RefreshTokenRequestValidator kế thừa từ AbstractValidator cho RefreshTokenRequest.
    {
        public RefreshTokenRequestValidator() // Thực thi dòng lệnh nghiệp vụ.
        {
            RuleFor(x => x.AccessToken) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính AccessToken.
                .NotEmpty().WithMessage("Access Token không được để trống."); // Ràng buộc kiểm tra thuộc tính không được để trống (Not Empty).

            RuleFor(x => x.RefreshToken) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính RefreshToken.
                .NotEmpty().WithMessage("Refresh Token không được để trống."); // Ràng buộc kiểm tra thuộc tính không được để trống (Not Empty).
        }
    }
}

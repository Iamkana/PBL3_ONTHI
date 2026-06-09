using FluentValidation; // Nhập thư viện FluentValidation để hỗ trợ kiểm định dữ liệu người dùng nhập vào.

namespace PBL3.Shared.DTOs.Auth // Định nghĩa namespace PBL3.Shared.DTOs.Auth quản lý cấu trúc code truyền tải và validator.
{
    /// <summary>
    /// DTO cho yêu cầu đăng nhập.
    /// </summary>
    public class LoginRequest // Định nghĩa lớp DTO truyền tải dữ liệu LoginRequest.
    {
        public string Email { get; set; } = string.Empty; // Thuộc tính Email kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public string Password { get; set; } = string.Empty; // Thuộc tính Password kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
    }

    public class LoginRequestValidator : AbstractValidator<LoginRequest> // Định nghĩa lớp kiểm định dữ liệu LoginRequestValidator kế thừa từ AbstractValidator cho LoginRequest.
    {
        public LoginRequestValidator() // Thực thi dòng lệnh nghiệp vụ.
        {
            RuleFor(x => x.Email) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính Email.
                .NotEmpty().WithMessage("Email không được để trống.") // Ràng buộc kiểm tra thuộc tính không được để trống (Not Empty).
                .EmailAddress().WithMessage("Email không đúng định dạng."); // Thiết lập thông báo lỗi khi kiểm tra thất bại: 'Email không đúng định dạng.'.

            RuleFor(x => x.Password) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính Password.
                .NotEmpty().WithMessage("Mật khẩu không được để trống."); // Ràng buộc kiểm tra thuộc tính không được để trống (Not Empty).
        }
    }
}

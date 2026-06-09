using FluentValidation; // Nhập thư viện FluentValidation để hỗ trợ kiểm định dữ liệu người dùng nhập vào.
using PBL3.Shared.DTOs.Employees; // Nhập (import) namespace PBL3.Shared.DTOs.Employees để sử dụng các thành phần bên trong.
using System; // Nhập thư viện hệ thống cơ bản.

namespace PBL3.Shared.Validators.Employees // Định nghĩa namespace PBL3.Shared.Validators.Employees quản lý cấu trúc code truyền tải và validator.
{
    public class CreateEmployeeRequestValidator : AbstractValidator<CreateEmployeeRequest> // Định nghĩa lớp kiểm định dữ liệu CreateEmployeeRequestValidator kế thừa từ AbstractValidator cho CreateEmployeeRequest.
    {
        public CreateEmployeeRequestValidator() // Thực thi dòng lệnh nghiệp vụ.
        {
            RuleFor(x => x.FullName) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính FullName.
                .NotEmpty().WithMessage("Họ tên không được để trống.") // Ràng buộc kiểm tra thuộc tính không được để trống (Not Empty).
                .MaximumLength(110).WithMessage("Họ tên không được vượt quá 110 ký tự."); // Ràng buộc độ dài chuỗi tối đa không được vượt quá 110 ký tự.

            RuleFor(x => x.Email) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính Email.
                .NotEmpty().WithMessage("Email không được để trống.") // Ràng buộc kiểm tra thuộc tính không được để trống (Not Empty).
                .EmailAddress().WithMessage("Email không đúng định dạng.") // Thiết lập thông báo lỗi khi kiểm tra thất bại: 'Email không đúng định dạng.'.
                .MaximumLength(256).WithMessage("Email không được vượt quá 256 ký tự."); // Ràng buộc độ dài chuỗi tối đa không được vượt quá 256 ký tự.

            RuleFor(x => x.PhoneNumber) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính PhoneNumber.
                .NotEmpty().WithMessage("Số điện thoại không được để trống.") // Ràng buộc kiểm tra thuộc tính không được để trống (Not Empty).
                .Matches(@"^0[3-9][0-9]{8}$").WithMessage("Số điện thoại không hợp lệ. Vui lòng nhập số điện thoại Việt Nam gồm 10 chữ số (bắt đầu bằng 03x, 05x, 07x, 08x, 09x)."); // Thiết lập thông báo lỗi khi kiểm tra thất bại: 'Số điện thoại không hợp lệ. Vui lòng nhập số điện thoại Việt Nam gồm 10 chữ số (bắt đầu bằng 03x, 05x, 07x, 08x, 09x).'.

            RuleFor(x => x.Address) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính Address.
                .MaximumLength(255).WithMessage("Địa chỉ không được vượt quá 255 ký tự."); // Ràng buộc độ dài chuỗi tối đa không được vượt quá 255 ký tự.

            RuleFor(x => x.City) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính City.
                .MaximumLength(100).WithMessage("Thành phố không được vượt quá 100 ký tự."); // Ràng buộc độ dài chuỗi tối đa không được vượt quá 100 ký tự.

            RuleFor(x => x.DateOfBirth) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính DateOfBirth.
                .LessThan(DateTime.Today).WithMessage("Ngày sinh phải nhỏ hơn ngày hiện tại.") // Thiết lập thông báo lỗi khi kiểm tra thất bại: 'Ngày sinh phải nhỏ hơn ngày hiện tại.'.
                .When(x => x.DateOfBirth.HasValue); // Chỉ áp dụng các quy tắc kiểm tra này khi thỏa mãn điều kiện phía dưới.

            RuleFor(x => x.Password) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính Password.
                .NotEmpty().WithMessage("Mật khẩu không được để trống.") // Ràng buộc kiểm tra thuộc tính không được để trống (Not Empty).
                .MinimumLength(8).WithMessage("Mật khẩu phải có ít nhất 8 ký tự.") // Ràng buộc độ dài chuỗi tối thiểu không được nhỏ hơn 8 ký tự.
                .Matches("[A-Z]").WithMessage("Mật khẩu phải có ít nhất 1 chữ hoa.") // Thiết lập thông báo lỗi khi kiểm tra thất bại: 'Mật khẩu phải có ít nhất 1 chữ hoa.'.
                .Matches("[a-z]").WithMessage("Mật khẩu phải có ít nhất 1 chữ thường.") // Thiết lập thông báo lỗi khi kiểm tra thất bại: 'Mật khẩu phải có ít nhất 1 chữ thường.'.
                .Matches("[0-9]").WithMessage("Mật khẩu phải có ít nhất 1 chữ số."); // Thiết lập thông báo lỗi khi kiểm tra thất bại: 'Mật khẩu phải có ít nhất 1 chữ số.'.

            RuleFor(x => x.ConfirmPassword) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính ConfirmPassword.
                .NotEmpty().WithMessage("Xác nhận mật khẩu không được để trống.") // Ràng buộc kiểm tra thuộc tính không được để trống (Not Empty).
                .Equal(x => x.Password).WithMessage("Xác nhận mật khẩu không khớp."); // Thiết lập thông báo lỗi khi kiểm tra thất bại: 'Xác nhận mật khẩu không khớp.'.
        }
    }

    public class UpdateEmployeeRequestValidator : AbstractValidator<UpdateEmployeeRequest> // Định nghĩa lớp kiểm định dữ liệu UpdateEmployeeRequestValidator kế thừa từ AbstractValidator cho UpdateEmployeeRequest.
    {
        public UpdateEmployeeRequestValidator() // Thực thi dòng lệnh nghiệp vụ.
        {
            RuleFor(x => x.FullName) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính FullName.
                .NotEmpty().WithMessage("Họ tên không được để trống.") // Ràng buộc kiểm tra thuộc tính không được để trống (Not Empty).
                .MaximumLength(110).WithMessage("Họ tên không được vượt quá 110 ký tự."); // Ràng buộc độ dài chuỗi tối đa không được vượt quá 110 ký tự.

            RuleFor(x => x.Address) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính Address.
                .MaximumLength(255).WithMessage("Địa chỉ không được vượt quá 255 ký tự."); // Ràng buộc độ dài chuỗi tối đa không được vượt quá 255 ký tự.

            RuleFor(x => x.City) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính City.
                .MaximumLength(100).WithMessage("Thành phố không được vượt quá 100 ký tự."); // Ràng buộc độ dài chuỗi tối đa không được vượt quá 100 ký tự.

            RuleFor(x => x.AvatarUrl) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính AvatarUrl.
                .MaximumLength(500).WithMessage("Đường dẫn ảnh đại diện không quá 500 ký tự."); // Ràng buộc độ dài chuỗi tối đa không được vượt quá 500 ký tự.

            RuleFor(x => x.DateOfBirth) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính DateOfBirth.
                .LessThan(DateTime.Today).WithMessage("Ngày sinh phải nhỏ hơn ngày hiện tại.") // Thiết lập thông báo lỗi khi kiểm tra thất bại: 'Ngày sinh phải nhỏ hơn ngày hiện tại.'.
                .When(x => x.DateOfBirth.HasValue); // Chỉ áp dụng các quy tắc kiểm tra này khi thỏa mãn điều kiện phía dưới.
        }
    }
}

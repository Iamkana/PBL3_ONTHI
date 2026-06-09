using FluentValidation; // Nhập thư viện FluentValidation để hỗ trợ kiểm định dữ liệu người dùng nhập vào.
using PBL3.Shared.DTOs.Categories; // Nhập (import) namespace PBL3.Shared.DTOs.Categories để sử dụng các thành phần bên trong.

namespace PBL3.Shared.Validators.Categories // Định nghĩa namespace PBL3.Shared.Validators.Categories quản lý cấu trúc code truyền tải và validator.
{
    public class CreateCategoryRequestValidator : AbstractValidator<CreateCategoryRequest> // Định nghĩa lớp kiểm định dữ liệu CreateCategoryRequestValidator kế thừa từ AbstractValidator cho CreateCategoryRequest.
    {
        public CreateCategoryRequestValidator() // Thực thi dòng lệnh nghiệp vụ.
        {
            RuleFor(x => x.Name) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính Name.
                .NotEmpty().WithMessage("Tên danh mục không được để trống.") // Ràng buộc kiểm tra thuộc tính không được để trống (Not Empty).
                .MaximumLength(100).WithMessage("Tên danh mục không được vượt quá 100 ký tự."); // Ràng buộc độ dài chuỗi tối đa không được vượt quá 100 ký tự.

            RuleFor(x => x.Slug) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính Slug.
                .NotEmpty().WithMessage("Slug không được để trống.") // Ràng buộc kiểm tra thuộc tính không được để trống (Not Empty).
                .MaximumLength(150).WithMessage("Slug không được vượt quá 150 ký tự.") // Ràng buộc độ dài chuỗi tối đa không được vượt quá 150 ký tự.
                .Matches("^[a-z0-9]+(?:-[a-z0-9]+)*$").WithMessage("Slug chỉ chấp nhận chữ thường, số và dấu gạch ngang."); // Thiết lập thông báo lỗi khi kiểm tra thất bại: 'Slug chỉ chấp nhận chữ thường, số và dấu gạch ngang.'.

            RuleFor(x => x.SortOrder) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính SortOrder.
                .GreaterThanOrEqualTo(0).WithMessage("Thứ tự hiển thị phải >= 0."); // Thiết lập thông báo lỗi khi kiểm tra thất bại: 'Thứ tự hiển thị phải >= 0.'.
        }
    }

    public class UpdateCategoryRequestValidator : AbstractValidator<UpdateCategoryRequest> // Định nghĩa lớp kiểm định dữ liệu UpdateCategoryRequestValidator kế thừa từ AbstractValidator cho UpdateCategoryRequest.
    {
        public UpdateCategoryRequestValidator() // Thực thi dòng lệnh nghiệp vụ.
        {
            RuleFor(x => x.Name) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính Name.
                .NotEmpty().WithMessage("Tên danh mục không được để trống.") // Ràng buộc kiểm tra thuộc tính không được để trống (Not Empty).
                .MaximumLength(100).WithMessage("Tên danh mục không được vượt quá 100 ký tự."); // Ràng buộc độ dài chuỗi tối đa không được vượt quá 100 ký tự.

            RuleFor(x => x.Slug) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính Slug.
                .NotEmpty().WithMessage("Slug không được để trống.") // Ràng buộc kiểm tra thuộc tính không được để trống (Not Empty).
                .MaximumLength(150).WithMessage("Slug không được vượt quá 150 ký tự.") // Ràng buộc độ dài chuỗi tối đa không được vượt quá 150 ký tự.
                .Matches("^[a-z0-9]+(?:-[a-z0-9]+)*$").WithMessage("Slug chỉ chấp nhận chữ thường, số và dấu gạch ngang."); // Thiết lập thông báo lỗi khi kiểm tra thất bại: 'Slug chỉ chấp nhận chữ thường, số và dấu gạch ngang.'.

            RuleFor(x => x.SortOrder) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính SortOrder.
                .GreaterThanOrEqualTo(0).WithMessage("Thứ tự hiển thị phải >= 0."); // Thiết lập thông báo lỗi khi kiểm tra thất bại: 'Thứ tự hiển thị phải >= 0.'.
        }
    }
}

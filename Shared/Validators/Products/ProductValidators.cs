using FluentValidation; // Nhập thư viện FluentValidation để hỗ trợ kiểm định dữ liệu người dùng nhập vào.
using PBL3.Shared.DTOs.Products; // Nhập (import) namespace PBL3.Shared.DTOs.Products để sử dụng các thành phần bên trong.

namespace PBL3.Shared.Validators.Products // Định nghĩa namespace PBL3.Shared.Validators.Products quản lý cấu trúc code truyền tải và validator.
{
    public class CreateProductRequestValidator : AbstractValidator<CreateProductRequest> // Định nghĩa lớp kiểm định dữ liệu CreateProductRequestValidator kế thừa từ AbstractValidator cho CreateProductRequest.
    {
        public CreateProductRequestValidator() // Thực thi dòng lệnh nghiệp vụ.
        {
            RuleFor(x => x.Name) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính Name.
                .NotEmpty().WithMessage("Tên sản phẩm không được để trống.") // Ràng buộc kiểm tra thuộc tính không được để trống (Not Empty).
                .MaximumLength(255).WithMessage("Tên sản phẩm không được vượt quá 255 ký tự."); // Ràng buộc độ dài chuỗi tối đa không được vượt quá 255 ký tự.

            RuleFor(x => x.ManufacturerId) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính ManufacturerId.
                .GreaterThan(0).WithMessage("Vui lòng chọn nhà sản xuất."); // Thiết lập thông báo lỗi khi kiểm tra thất bại: 'Vui lòng chọn nhà sản xuất.'.

            RuleFor(x => x.CategoryId) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính CategoryId.
                .GreaterThan(0).WithMessage("Vui lòng chọn danh mục."); // Thiết lập thông báo lỗi khi kiểm tra thất bại: 'Vui lòng chọn danh mục.'.

            RuleFor(x => x.Variants) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính Variants.
                .NotEmpty().WithMessage("Sản phẩm phải có ít nhất một phiên bản.") // Ràng buộc kiểm tra thuộc tính không được để trống (Not Empty).
                .Must(v => v != null && v.Count > 0).WithMessage("Sản phẩm phải có ít nhất một phiên bản."); // Thiết lập thông báo lỗi khi kiểm tra thất bại: 'Sản phẩm phải có ít nhất một phiên bản.'.

            RuleForEach(x => x.Variants) // Thực thi dòng lệnh nghiệp vụ.
                .SetValidator(new CreateVariantRequestValidator()); // Thực thi dòng lệnh nghiệp vụ.
        }
    }

    public class UpdateProductRequestValidator : AbstractValidator<UpdateProductRequest> // Định nghĩa lớp kiểm định dữ liệu UpdateProductRequestValidator kế thừa từ AbstractValidator cho UpdateProductRequest.
    {
        public UpdateProductRequestValidator() // Thực thi dòng lệnh nghiệp vụ.
        {
            RuleFor(x => x.Name) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính Name.
                .NotEmpty().WithMessage("Tên sản phẩm không được để trống.") // Ràng buộc kiểm tra thuộc tính không được để trống (Not Empty).
                .MaximumLength(255).WithMessage("Tên sản phẩm không được vượt quá 255 ký tự."); // Ràng buộc độ dài chuỗi tối đa không được vượt quá 255 ký tự.

            RuleFor(x => x.ManufacturerId) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính ManufacturerId.
                .GreaterThan(0).WithMessage("Vui lòng chọn nhà sản xuất."); // Thiết lập thông báo lỗi khi kiểm tra thất bại: 'Vui lòng chọn nhà sản xuất.'.

            RuleFor(x => x.CategoryId) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính CategoryId.
                .GreaterThan(0).WithMessage("Vui lòng chọn danh mục."); // Thiết lập thông báo lỗi khi kiểm tra thất bại: 'Vui lòng chọn danh mục.'.

            RuleFor(x => x.Status) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính Status.
                .IsInEnum().WithMessage("Trạng thái sản phẩm không hợp lệ."); // Thiết lập thông báo lỗi khi kiểm tra thất bại: 'Trạng thái sản phẩm không hợp lệ.'.
        }
    }
}

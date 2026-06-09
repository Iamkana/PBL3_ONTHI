using FluentValidation; // Nhập thư viện FluentValidation để hỗ trợ kiểm định dữ liệu người dùng nhập vào.
using PBL3.Shared.DTOs.Products; // Nhập (import) namespace PBL3.Shared.DTOs.Products để sử dụng các thành phần bên trong.

namespace PBL3.Shared.Validators.Products // Định nghĩa namespace PBL3.Shared.Validators.Products quản lý cấu trúc code truyền tải và validator.
{
    public class CreateVariantRequestValidator : AbstractValidator<CreateVariantRequest> // Định nghĩa lớp kiểm định dữ liệu CreateVariantRequestValidator kế thừa từ AbstractValidator cho CreateVariantRequest.
    {
        public CreateVariantRequestValidator() // Thực thi dòng lệnh nghiệp vụ.
        {
            RuleFor(x => x.SKU) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính SKU.
                .NotEmpty().WithMessage("Mã SKU không được để trống.") // Ràng buộc kiểm tra thuộc tính không được để trống (Not Empty).
                .MaximumLength(50).WithMessage("Mã SKU không được vượt quá 50 ký tự.") // Ràng buộc độ dài chuỗi tối đa không được vượt quá 50 ký tự.
                .Matches(@"^\S+$").WithMessage("Mã SKU không được chứa khoảng trắng."); // Thiết lập thông báo lỗi khi kiểm tra thất bại: 'Mã SKU không được chứa khoảng trắng.'.

            RuleFor(x => x.VariantName) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính VariantName.
                .NotEmpty().WithMessage("Tên phiên bản không được để trống.") // Ràng buộc kiểm tra thuộc tính không được để trống (Not Empty).
                .MaximumLength(200).WithMessage("Tên phiên bản không được vượt quá 200 ký tự."); // Ràng buộc độ dài chuỗi tối đa không được vượt quá 200 ký tự.

            RuleFor(x => x.Price) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính Price.
                .GreaterThan(0).WithMessage("Giá bán phải lớn hơn 0."); // Thiết lập thông báo lỗi khi kiểm tra thất bại: 'Giá bán phải lớn hơn 0.'.

            RuleFor(x => x.OriginalPrice) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính OriginalPrice.
                .GreaterThan(0).When(x => x.OriginalPrice.HasValue) // Ràng buộc giá trị số phải lớn hơn 0.
                .WithMessage("Giá gốc phải lớn hơn 0."); // Thiết lập thông báo lỗi khi kiểm tra thất bại: 'Giá gốc phải lớn hơn 0.'.

            RuleFor(x => x.WarrantyMonth) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính WarrantyMonth.
                .GreaterThanOrEqualTo(0).WithMessage("Thời gian bảo hành phải >= 0 tháng."); // Thiết lập thông báo lỗi khi kiểm tra thất bại: 'Thời gian bảo hành phải >= 0 tháng.'.
        }
    }

    public class SaveVariantRequestValidator : AbstractValidator<SaveVariantRequest> // Định nghĩa lớp kiểm định dữ liệu SaveVariantRequestValidator kế thừa từ AbstractValidator cho SaveVariantRequest.
    {
        public SaveVariantRequestValidator() // Thực thi dòng lệnh nghiệp vụ.
        {
            RuleFor(x => x.SKU) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính SKU.
                .NotEmpty().WithMessage("Mã SKU không được để trống.") // Ràng buộc kiểm tra thuộc tính không được để trống (Not Empty).
                .MaximumLength(50).WithMessage("Mã SKU không được vượt quá 50 ký tự.") // Ràng buộc độ dài chuỗi tối đa không được vượt quá 50 ký tự.
                .Matches(@"^\S+$").WithMessage("Mã SKU không được chứa khoảng trắng."); // Thiết lập thông báo lỗi khi kiểm tra thất bại: 'Mã SKU không được chứa khoảng trắng.'.

            RuleFor(x => x.VariantName) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính VariantName.
                .NotEmpty().WithMessage("Tên phiên bản không được để trống.") // Ràng buộc kiểm tra thuộc tính không được để trống (Not Empty).
                .MaximumLength(200).WithMessage("Tên phiên bản không được vượt quá 200 ký tự."); // Ràng buộc độ dài chuỗi tối đa không được vượt quá 200 ký tự.

            RuleFor(x => x.Price) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính Price.
                .GreaterThan(0).WithMessage("Giá bán phải lớn hơn 0."); // Thiết lập thông báo lỗi khi kiểm tra thất bại: 'Giá bán phải lớn hơn 0.'.

            RuleFor(x => x.OriginalPrice) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính OriginalPrice.
                .GreaterThan(0).When(x => x.OriginalPrice.HasValue) // Ràng buộc giá trị số phải lớn hơn 0.
                .WithMessage("Giá gốc phải lớn hơn 0."); // Thiết lập thông báo lỗi khi kiểm tra thất bại: 'Giá gốc phải lớn hơn 0.'.

            RuleFor(x => x.WarrantyMonth) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính WarrantyMonth.
                .GreaterThanOrEqualTo(0).WithMessage("Thời gian bảo hành phải >= 0 tháng."); // Thiết lập thông báo lỗi khi kiểm tra thất bại: 'Thời gian bảo hành phải >= 0 tháng.'.
        }
    }
}

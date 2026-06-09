using FluentValidation; // Nhập thư viện FluentValidation để hỗ trợ kiểm định dữ liệu người dùng nhập vào.
using PBL3.Shared.DTOs.Suppliers; // Nhập (import) namespace PBL3.Shared.DTOs.Suppliers để sử dụng các thành phần bên trong.

namespace PBL3.Shared.Validators.Suppliers // Định nghĩa namespace PBL3.Shared.Validators.Suppliers quản lý cấu trúc code truyền tải và validator.
{
    public class CreateSupplierRequestValidator : AbstractValidator<CreateSupplierRequest> // Định nghĩa lớp kiểm định dữ liệu CreateSupplierRequestValidator kế thừa từ AbstractValidator cho CreateSupplierRequest.
    {
        public CreateSupplierRequestValidator() // Thực thi dòng lệnh nghiệp vụ.
        {
            RuleFor(x => x.Name) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính Name.
                .NotEmpty().WithMessage("Tên nhà cung cấp không được để trống.") // Ràng buộc kiểm tra thuộc tính không được để trống (Not Empty).
                .MaximumLength(200).WithMessage("Tên nhà cung cấp không được vượt quá 200 ký tự."); // Ràng buộc độ dài chuỗi tối đa không được vượt quá 200 ký tự.

            RuleFor(x => x.PhoneNumber) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính PhoneNumber.
                .NotEmpty().WithMessage("Số điện thoại không được để trống.") // Ràng buộc kiểm tra thuộc tính không được để trống (Not Empty).
                .MaximumLength(20).WithMessage("Số điện thoại không được vượt quá 20 ký tự.") // Ràng buộc độ dài chuỗi tối đa không được vượt quá 20 ký tự.
                .Matches(@"^[0-9]+$").WithMessage("Số điện thoại chỉ được chứa chữ số."); // Thiết lập thông báo lỗi khi kiểm tra thất bại: 'Số điện thoại chỉ được chứa chữ số.'.

            RuleFor(x => x.Email) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính Email.
                .EmailAddress().WithMessage("Email không đúng định dạng.") // Thiết lập thông báo lỗi khi kiểm tra thất bại: 'Email không đúng định dạng.'.
                .When(x => !string.IsNullOrWhiteSpace(x.Email)); // Chỉ áp dụng các quy tắc kiểm tra này khi thỏa mãn điều kiện phía dưới.

            RuleFor(x => x.TaxCode) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính TaxCode.
                .MaximumLength(20).WithMessage("Mã số thuế không được vượt quá 20 ký tự.") // Ràng buộc độ dài chuỗi tối đa không được vượt quá 20 ký tự.
                .When(x => !string.IsNullOrWhiteSpace(x.TaxCode)); // Chỉ áp dụng các quy tắc kiểm tra này khi thỏa mãn điều kiện phía dưới.

            RuleFor(x => x.ContactPerson) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính ContactPerson.
                .MaximumLength(100).WithMessage("Tên người liên hệ không được vượt quá 100 ký tự.") // Ràng buộc độ dài chuỗi tối đa không được vượt quá 100 ký tự.
                .When(x => !string.IsNullOrWhiteSpace(x.ContactPerson)); // Chỉ áp dụng các quy tắc kiểm tra này khi thỏa mãn điều kiện phía dưới.

            RuleFor(x => x.Address) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính Address.
                .MaximumLength(255).WithMessage("Địa chỉ không được vượt quá 255 ký tự.") // Ràng buộc độ dài chuỗi tối đa không được vượt quá 255 ký tự.
                .When(x => !string.IsNullOrWhiteSpace(x.Address)); // Chỉ áp dụng các quy tắc kiểm tra này khi thỏa mãn điều kiện phía dưới.
        }
    }

    public class UpdateSupplierRequestValidator : AbstractValidator<UpdateSupplierRequest> // Định nghĩa lớp kiểm định dữ liệu UpdateSupplierRequestValidator kế thừa từ AbstractValidator cho UpdateSupplierRequest.
    {
        public UpdateSupplierRequestValidator() // Thực thi dòng lệnh nghiệp vụ.
        {
            RuleFor(x => x.Name) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính Name.
                .NotEmpty().WithMessage("Tên nhà cung cấp không được để trống.") // Ràng buộc kiểm tra thuộc tính không được để trống (Not Empty).
                .MaximumLength(200).WithMessage("Tên nhà cung cấp không được vượt quá 200 ký tự."); // Ràng buộc độ dài chuỗi tối đa không được vượt quá 200 ký tự.

            RuleFor(x => x.PhoneNumber) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính PhoneNumber.
                .NotEmpty().WithMessage("Số điện thoại không được để trống.") // Ràng buộc kiểm tra thuộc tính không được để trống (Not Empty).
                .MaximumLength(20).WithMessage("Số điện thoại không được vượt quá 20 ký tự.") // Ràng buộc độ dài chuỗi tối đa không được vượt quá 20 ký tự.
                .Matches(@"^[0-9]+$").WithMessage("Số điện thoại chỉ được chứa chữ số."); // Thiết lập thông báo lỗi khi kiểm tra thất bại: 'Số điện thoại chỉ được chứa chữ số.'.

            RuleFor(x => x.Email) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính Email.
                .EmailAddress().WithMessage("Email không đúng định dạng.") // Thiết lập thông báo lỗi khi kiểm tra thất bại: 'Email không đúng định dạng.'.
                .When(x => !string.IsNullOrWhiteSpace(x.Email)); // Chỉ áp dụng các quy tắc kiểm tra này khi thỏa mãn điều kiện phía dưới.

            RuleFor(x => x.TaxCode) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính TaxCode.
                .MaximumLength(20).WithMessage("Mã số thuế không được vượt quá 20 ký tự.") // Ràng buộc độ dài chuỗi tối đa không được vượt quá 20 ký tự.
                .When(x => !string.IsNullOrWhiteSpace(x.TaxCode)); // Chỉ áp dụng các quy tắc kiểm tra này khi thỏa mãn điều kiện phía dưới.

            RuleFor(x => x.ContactPerson) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính ContactPerson.
                .MaximumLength(100).WithMessage("Tên người liên hệ không được vượt quá 100 ký tự.") // Ràng buộc độ dài chuỗi tối đa không được vượt quá 100 ký tự.
                .When(x => !string.IsNullOrWhiteSpace(x.ContactPerson)); // Chỉ áp dụng các quy tắc kiểm tra này khi thỏa mãn điều kiện phía dưới.

            RuleFor(x => x.Address) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính Address.
                .MaximumLength(255).WithMessage("Địa chỉ không được vượt quá 255 ký tự.") // Ràng buộc độ dài chuỗi tối đa không được vượt quá 255 ký tự.
                .When(x => !string.IsNullOrWhiteSpace(x.Address)); // Chỉ áp dụng các quy tắc kiểm tra này khi thỏa mãn điều kiện phía dưới.
        }
    }
}

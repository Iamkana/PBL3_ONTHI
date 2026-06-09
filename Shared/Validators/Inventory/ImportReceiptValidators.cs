using FluentValidation; // Nhập thư viện FluentValidation để hỗ trợ kiểm định dữ liệu người dùng nhập vào.
using PBL3.Shared.DTOs.Inventory; // Nhập (import) namespace PBL3.Shared.DTOs.Inventory để sử dụng các thành phần bên trong.

namespace PBL3.Shared.Validators.Inventory // Định nghĩa namespace PBL3.Shared.Validators.Inventory quản lý cấu trúc code truyền tải và validator.
{
    /// <summary>
    /// Validator cho từng dòng chi tiết phiếu nhập.
    /// </summary>
    public class ImportReceiptDetailRequestValidator : AbstractValidator<ImportReceiptDetailRequest> // Định nghĩa lớp kiểm định dữ liệu ImportReceiptDetailRequestValidator kế thừa từ AbstractValidator cho ImportReceiptDetailRequest.
    {
        public ImportReceiptDetailRequestValidator() // Thực thi dòng lệnh nghiệp vụ.
        {
            RuleFor(x => x.VariantId) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính VariantId.
                .GreaterThan(0).WithMessage("Mã biến thể sản phẩm không hợp lệ."); // Thiết lập thông báo lỗi khi kiểm tra thất bại: 'Mã biến thể sản phẩm không hợp lệ.'.

            RuleFor(x => x.Quantity) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính Quantity.
                .GreaterThan(0).WithMessage("Số lượng nhập phải lớn hơn 0."); // Thiết lập thông báo lỗi khi kiểm tra thất bại: 'Số lượng nhập phải lớn hơn 0.'.

            RuleFor(x => x.ImportPrice) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính ImportPrice.
                .GreaterThanOrEqualTo(0).WithMessage("Giá nhập phải lớn hơn hoặc bằng 0."); // Thiết lập thông báo lỗi khi kiểm tra thất bại: 'Giá nhập phải lớn hơn hoặc bằng 0.'.

            RuleFor(x => x.SerialNumbers) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính SerialNumbers.
                .NotEmpty().WithMessage("Danh sách Serial không được để trống."); // Ràng buộc kiểm tra thuộc tính không được để trống (Not Empty).

            // **CRITICAL**: Số lượng Serial phải khớp tuyệt đối với Quantity
            RuleFor(x => x) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính thuộc tính.
                .Must(x => x.SerialNumbers != null && x.SerialNumbers.Count == x.Quantity) // Áp dụng điều kiện kiểm tra logic tùy chỉnh (Must) cho thuộc tính.
                .WithMessage("Số lượng Serial phải khớp chính xác với số lượng nhập (Quantity).") // Thiết lập thông báo lỗi khi kiểm tra thất bại: 'Số lượng Serial phải khớp chính xác với số lượng nhập (Quantity).'.
                .When(x => x.SerialNumbers != null && x.SerialNumbers.Count > 0); // Chỉ áp dụng các quy tắc kiểm tra này khi thỏa mãn điều kiện phía dưới.

            // Mỗi Serial không được trống
            RuleForEach(x => x.SerialNumbers) // Thực thi dòng lệnh nghiệp vụ.
                .NotEmpty().WithMessage("Mã Serial không được để trống.") // Ràng buộc kiểm tra thuộc tính không được để trống (Not Empty).
                .MaximumLength(100).WithMessage("Mã Serial không được vượt quá 100 ký tự."); // Ràng buộc độ dài chuỗi tối đa không được vượt quá 100 ký tự.
        }
    }

    /// <summary>
    /// Validator cho request tạo phiếu nhập kho.
    /// </summary>
    public class CreateImportReceiptRequestValidator : AbstractValidator<CreateImportReceiptRequest> // Định nghĩa lớp kiểm định dữ liệu CreateImportReceiptRequestValidator kế thừa từ AbstractValidator cho CreateImportReceiptRequest.
    {
        public CreateImportReceiptRequestValidator() // Thực thi dòng lệnh nghiệp vụ.
        {
            RuleFor(x => x.SupplierId) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính SupplierId.
                .GreaterThan(0).WithMessage("Nhà cung cấp không hợp lệ."); // Thiết lập thông báo lỗi khi kiểm tra thất bại: 'Nhà cung cấp không hợp lệ.'.

            RuleFor(x => x.Note) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính Note.
                .MaximumLength(500).WithMessage("Ghi chú không được vượt quá 500 ký tự.") // Ràng buộc độ dài chuỗi tối đa không được vượt quá 500 ký tự.
                .When(x => !string.IsNullOrWhiteSpace(x.Note)); // Chỉ áp dụng các quy tắc kiểm tra này khi thỏa mãn điều kiện phía dưới.

            RuleFor(x => x.Details) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính Details.
                .NotEmpty().WithMessage("Phiếu nhập phải có ít nhất 1 dòng chi tiết."); // Ràng buộc kiểm tra thuộc tính không được để trống (Not Empty).

            RuleForEach(x => x.Details) // Thực thi dòng lệnh nghiệp vụ.
                .SetValidator(new ImportReceiptDetailRequestValidator()); // Thực thi dòng lệnh nghiệp vụ.

            // Check trùng Serial nội bộ trong toàn bộ request
            RuleFor(x => x.Details) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính Details.
                .Must(details => // Áp dụng điều kiện kiểm tra logic tùy chỉnh (Must) cho thuộc tính.
                {
                    if (details == null || details.Count == 0) return true; // Kiểm tra điều kiện: 'details == null || details.Count == 0'.
                    var allSerials = details.SelectMany(d => d.SerialNumbers ?? new List<string>()).ToList(); // Thực thi dòng lệnh nghiệp vụ.
                    return allSerials.Count == allSerials.Distinct(StringComparer.OrdinalIgnoreCase).Count(); // Trả về kết quả: 'allSerials.Count == allSerials.Distinct(StringComparer.OrdinalIgnoreCase).Count()'.
                }) // Thực thi dòng lệnh nghiệp vụ.
                .WithMessage("Có mã Serial bị trùng lặp trong phiếu nhập. Vui lòng kiểm tra lại."); // Thiết lập thông báo lỗi khi kiểm tra thất bại: 'Có mã Serial bị trùng lặp trong phiếu nhập. Vui lòng kiểm tra lại.'.
        }
    }
}

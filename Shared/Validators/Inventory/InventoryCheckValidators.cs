using FluentValidation; // Nhập thư viện FluentValidation để hỗ trợ kiểm định dữ liệu người dùng nhập vào.
using PBL3.Shared.DTOs.Inventory; // Nhập (import) namespace PBL3.Shared.DTOs.Inventory để sử dụng các thành phần bên trong.

namespace PBL3.Shared.Validators.Inventory // Định nghĩa namespace PBL3.Shared.Validators.Inventory quản lý cấu trúc code truyền tải và validator.
{
    public class CreateInventoryCheckRequestValidator : AbstractValidator<CreateInventoryCheckRequest> // Định nghĩa lớp kiểm định dữ liệu CreateInventoryCheckRequestValidator kế thừa từ AbstractValidator cho CreateInventoryCheckRequest.
    {
        public CreateInventoryCheckRequestValidator() // Thực thi dòng lệnh nghiệp vụ.
        {
            RuleFor(x => x.ScopeType) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính ScopeType.
                .InclusiveBetween((byte)0, (byte)1) // Thực thi dòng lệnh nghiệp vụ.
                .WithMessage("Phạm vi kiểm kê không hợp lệ. Chỉ chấp nhận 0 (Toàn kho) hoặc 1 (Theo danh mục)."); // Thiết lập thông báo lỗi khi kiểm tra thất bại: 'Phạm vi kiểm kê không hợp lệ. Chỉ chấp nhận 0 (Toàn kho) hoặc 1 (Theo danh mục).'.

            RuleFor(x => x.ScopeCategoryId) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính ScopeCategoryId.
                .NotNull() // Ràng buộc kiểm tra thuộc tính không được có giá trị null.
                .GreaterThan(0) // Ràng buộc giá trị số phải lớn hơn 0.
                .WithMessage("Phải chọn danh mục khi phạm vi kiểm kê là theo danh mục.") // Thiết lập thông báo lỗi khi kiểm tra thất bại: 'Phải chọn danh mục khi phạm vi kiểm kê là theo danh mục.'.
                .When(x => x.ScopeType == 1); // Chỉ áp dụng các quy tắc kiểm tra này khi thỏa mãn điều kiện phía dưới.

            RuleFor(x => x.Note) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính Note.
                .MaximumLength(500) // Ràng buộc độ dài chuỗi tối đa không được vượt quá 500 ký tự.
                .WithMessage("Ghi chú không được vượt quá 500 ký tự.") // Thiết lập thông báo lỗi khi kiểm tra thất bại: 'Ghi chú không được vượt quá 500 ký tự.'.
                .When(x => !string.IsNullOrWhiteSpace(x.Note)); // Chỉ áp dụng các quy tắc kiểm tra này khi thỏa mãn điều kiện phía dưới.
        }
    }

    public class ScanSerialRequestValidator : AbstractValidator<ScanSerialRequest> // Định nghĩa lớp kiểm định dữ liệu ScanSerialRequestValidator kế thừa từ AbstractValidator cho ScanSerialRequest.
    {
        public ScanSerialRequestValidator() // Thực thi dòng lệnh nghiệp vụ.
        {
            RuleFor(x => x.SerialNumber) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính SerialNumber.
                .NotEmpty() // Ràng buộc kiểm tra thuộc tính không được để trống (Not Empty).
                .WithMessage("Mã Serial không được để trống.") // Thiết lập thông báo lỗi khi kiểm tra thất bại: 'Mã Serial không được để trống.'.
                .MaximumLength(100) // Ràng buộc độ dài chuỗi tối đa không được vượt quá 100 ký tự.
                .WithMessage("Mã Serial không được vượt quá 100 ký tự.") // Thiết lập thông báo lỗi khi kiểm tra thất bại: 'Mã Serial không được vượt quá 100 ký tự.'.
                .Must(s => !s.Contains('\n') && !s.Contains('\r')) // Áp dụng điều kiện kiểm tra logic tùy chỉnh (Must) cho thuộc tính.
                .WithMessage("Mã Serial không được chứa ký tự xuống dòng."); // Thiết lập thông báo lỗi khi kiểm tra thất bại: 'Mã Serial không được chứa ký tự xuống dòng.'.

            RuleFor(x => x.VariantIdForUnknown) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính VariantIdForUnknown.
                .GreaterThan(0) // Ràng buộc giá trị số phải lớn hơn 0.
                .WithMessage("Id biến thể sản phẩm phải lớn hơn 0.") // Thiết lập thông báo lỗi khi kiểm tra thất bại: 'Id biến thể sản phẩm phải lớn hơn 0.'.
                .When(x => x.VariantIdForUnknown.HasValue); // Chỉ áp dụng các quy tắc kiểm tra này khi thỏa mãn điều kiện phía dưới.
        }
    }

    public class UpdateScanReasonRequestValidator : AbstractValidator<UpdateScanReasonRequest> // Định nghĩa lớp kiểm định dữ liệu UpdateScanReasonRequestValidator kế thừa từ AbstractValidator cho UpdateScanReasonRequest.
    {
        public UpdateScanReasonRequestValidator() // Thực thi dòng lệnh nghiệp vụ.
        {
            RuleFor(x => x.Reason) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính Reason.
                .NotEmpty() // Ràng buộc kiểm tra thuộc tính không được để trống (Not Empty).
                .WithMessage("Lý do chênh lệch không được để trống.") // Thiết lập thông báo lỗi khi kiểm tra thất bại: 'Lý do chênh lệch không được để trống.'.
                .MaximumLength(500) // Ràng buộc độ dài chuỗi tối đa không được vượt quá 500 ký tự.
                .WithMessage("Lý do không được vượt quá 500 ký tự."); // Thiết lập thông báo lỗi khi kiểm tra thất bại: 'Lý do không được vượt quá 500 ký tự.'.

            RuleFor(x => x.ProposedActionNote) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính ProposedActionNote.
                .MaximumLength(200) // Ràng buộc độ dài chuỗi tối đa không được vượt quá 200 ký tự.
                .WithMessage("Hướng xử lý đề xuất không được vượt quá 200 ký tự.") // Thiết lập thông báo lỗi khi kiểm tra thất bại: 'Hướng xử lý đề xuất không được vượt quá 200 ký tự.'.
                .When(x => !string.IsNullOrWhiteSpace(x.ProposedActionNote)); // Chỉ áp dụng các quy tắc kiểm tra này khi thỏa mãn điều kiện phía dưới.
        }
    }

    public class RejectInventoryCheckRequestValidator : AbstractValidator<RejectInventoryCheckRequest> // Định nghĩa lớp kiểm định dữ liệu RejectInventoryCheckRequestValidator kế thừa từ AbstractValidator cho RejectInventoryCheckRequest.
    {
        public RejectInventoryCheckRequestValidator() // Thực thi dòng lệnh nghiệp vụ.
        {
            RuleFor(x => x.Reason) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính Reason.
                .NotEmpty() // Ràng buộc kiểm tra thuộc tính không được để trống (Not Empty).
                .WithMessage("Lý do từ chối không được để trống.") // Thiết lập thông báo lỗi khi kiểm tra thất bại: 'Lý do từ chối không được để trống.'.
                .MaximumLength(500) // Ràng buộc độ dài chuỗi tối đa không được vượt quá 500 ký tự.
                .WithMessage("Lý do từ chối không được vượt quá 500 ký tự."); // Thiết lập thông báo lỗi khi kiểm tra thất bại: 'Lý do từ chối không được vượt quá 500 ký tự.'.
        }
    }
}

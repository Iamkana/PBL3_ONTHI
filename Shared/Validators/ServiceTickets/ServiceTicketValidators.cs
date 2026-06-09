using FluentValidation; // Nhập thư viện FluentValidation để hỗ trợ kiểm định dữ liệu người dùng nhập vào.
using PBL3.Shared.DTOs.ServiceTickets; // Nhập (import) namespace PBL3.Shared.DTOs.ServiceTickets để sử dụng các thành phần bên trong.

namespace PBL3.Shared.Validators.ServiceTickets // Định nghĩa namespace PBL3.Shared.Validators.ServiceTickets quản lý cấu trúc code truyền tải và validator.
{
    public class ServiceTicketIntakeRequestValidator : AbstractValidator<ServiceTicketIntakeRequestDto> // Định nghĩa lớp kiểm định dữ liệu ServiceTicketIntakeRequestValidator kế thừa từ AbstractValidator cho ServiceTicketIntakeRequestDto.
    {
        public ServiceTicketIntakeRequestValidator() // Thực thi dòng lệnh nghiệp vụ.
        {
            RuleFor(x => x.SerialNumber) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính SerialNumber.
                .NotEmpty().WithMessage("Mã Serial không được để trống."); // Ràng buộc kiểm tra thuộc tính không được để trống (Not Empty).

            RuleFor(x => x.CustomerReportedIssue) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính CustomerReportedIssue.
                .NotEmpty().WithMessage("Vui lòng nhập mô tả vấn đề khách báo cáo.") // Ràng buộc kiểm tra thuộc tính không được để trống (Not Empty).
                .MaximumLength(2000).WithMessage("Mô tả không được vượt quá 2000 ký tự."); // Ràng buộc độ dài chuỗi tối đa không được vượt quá 2000 ký tự.

            RuleFor(x => x.CosmeticNotes) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính CosmeticNotes.
                .MaximumLength(1000).WithMessage("Ghi chú tình trạng không được vượt quá 1000 ký tự."); // Ràng buộc độ dài chuỗi tối đa không được vượt quá 1000 ký tự.

            RuleFor(x => x.WalkInCustomerName) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính WalkInCustomerName.
                .MaximumLength(100).WithMessage("Tên khách hàng không được vượt quá 100 ký tự."); // Ràng buộc độ dài chuỗi tối đa không được vượt quá 100 ký tự.

            RuleFor(x => x.WalkInCustomerPhone) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính WalkInCustomerPhone.
                .MaximumLength(20).WithMessage("Số điện thoại không được vượt quá 20 ký tự."); // Ràng buộc độ dài chuỗi tối đa không được vượt quá 20 ký tự.
        }
    }

    public class ServiceTicketDiagnosisValidator : AbstractValidator<ServiceTicketDiagnosisDto> // Định nghĩa lớp kiểm định dữ liệu ServiceTicketDiagnosisValidator kế thừa từ AbstractValidator cho ServiceTicketDiagnosisDto.
    {
        public ServiceTicketDiagnosisValidator() // Thực thi dòng lệnh nghiệp vụ.
        {
            RuleFor(x => x.DiagnosisFindings) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính DiagnosisFindings.
                .NotEmpty().WithMessage("Vui lòng nhập kết luận chẩn đoán.") // Ràng buộc kiểm tra thuộc tính không được để trống (Not Empty).
                .MaximumLength(2000).WithMessage("Kết luận không được vượt quá 2000 ký tự."); // Ràng buộc độ dài chuỗi tối đa không được vượt quá 2000 ký tự.
        }
    }

    public class QuotationCreateValidator : AbstractValidator<QuotationCreateDto> // Định nghĩa lớp kiểm định dữ liệu QuotationCreateValidator kế thừa từ AbstractValidator cho QuotationCreateDto.
    {
        public QuotationCreateValidator() // Thực thi dòng lệnh nghiệp vụ.
        {
            RuleFor(x => x.LaborCost) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính LaborCost.
                .GreaterThanOrEqualTo(0).WithMessage("Phí công không được âm."); // Thiết lập thông báo lỗi khi kiểm tra thất bại: 'Phí công không được âm.'.

            RuleFor(x => x.Items) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính Items.
                .NotEmpty().WithMessage("Phải có ít nhất một mục trong báo giá.") // Ràng buộc kiểm tra thuộc tính không được để trống (Not Empty).
                .Must(x => x.All(i => i.Quantity > 0)).WithMessage("Số lượng phải lớn hơn 0.") // Thiết lập thông báo lỗi khi kiểm tra thất bại: 'Số lượng phải lớn hơn 0.'.
                .Must(x => x.All(i => i.UnitPrice >= 0)).WithMessage("Giá không được âm.") // Thiết lập thông báo lỗi khi kiểm tra thất bại: 'Giá không được âm.'.
                .Must(x => x.All(i => !string.IsNullOrWhiteSpace(i.Description))).WithMessage("Mô tả mục không được để trống."); // Thiết lập thông báo lỗi khi kiểm tra thất bại: 'Mô tả mục không được để trống.'.
        }
    }

    public class RmaShipmentCreateValidator : AbstractValidator<RmaShipmentCreateDto> // Định nghĩa lớp kiểm định dữ liệu RmaShipmentCreateValidator kế thừa từ AbstractValidator cho RmaShipmentCreateDto.
    {
        public RmaShipmentCreateValidator() // Thực thi dòng lệnh nghiệp vụ.
        {
            RuleFor(x => x.CarrierName) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính CarrierName.
                .NotEmpty().WithMessage("Tên hãng vận chuyển không được để trống.") // Ràng buộc kiểm tra thuộc tính không được để trống (Not Empty).
                .MaximumLength(100).WithMessage("Tên hãng không được vượt quá 100 ký tự."); // Ràng buộc độ dài chuỗi tối đa không được vượt quá 100 ký tự.

            RuleFor(x => x.TrackingCode) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính TrackingCode.
                .NotEmpty().WithMessage("Mã vận đơn không được để trống.") // Ràng buộc kiểm tra thuộc tính không được để trống (Not Empty).
                .MaximumLength(100).WithMessage("Mã vận đơn không được vượt quá 100 ký tự."); // Ràng buộc độ dài chuỗi tối đa không được vượt quá 100 ký tự.
        }
    }

    public class RmaResolutionUpdateValidator : AbstractValidator<RmaResolutionUpdateDto> // Định nghĩa lớp kiểm định dữ liệu RmaResolutionUpdateValidator kế thừa từ AbstractValidator cho RmaResolutionUpdateDto.
    {
        public RmaResolutionUpdateValidator() // Thực thi dòng lệnh nghiệp vụ.
        {
            RuleFor(x => x.ManufacturerResolution) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính ManufacturerResolution.
                .Must(x => x >= 1 && x <= 3).WithMessage("Kết quả xử lý không hợp lệ."); // Thiết lập thông báo lỗi khi kiểm tra thất bại: 'Kết quả xử lý không hợp lệ.'.

            RuleFor(x => x.ReplacementSerialId) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính ReplacementSerialId.
                .NotNull().When(x => x.ManufacturerResolution == 2).WithMessage("Phải chọn Serial thay thế khi hãng đổi máy."); // Ràng buộc kiểm tra thuộc tính không được có giá trị null.

            RuleFor(x => x.ManufacturerNotes) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính ManufacturerNotes.
                .MaximumLength(1000).WithMessage("Ghi chú từ hãng không được vượt quá 1000 ký tự."); // Ràng buộc độ dài chuỗi tối đa không được vượt quá 1000 ký tự.
        }
    }

    public class ServiceTicketCancelValidator : AbstractValidator<ServiceTicketCancelDto> // Định nghĩa lớp kiểm định dữ liệu ServiceTicketCancelValidator kế thừa từ AbstractValidator cho ServiceTicketCancelDto.
    {
        public ServiceTicketCancelValidator() // Thực thi dòng lệnh nghiệp vụ.
        {
            RuleFor(x => x.CancelReason) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính CancelReason.
                .NotEmpty().WithMessage("Vui lòng nhập lý do hủy phiếu.") // Ràng buộc kiểm tra thuộc tính không được để trống (Not Empty).
                .MaximumLength(500).WithMessage("Lý do hủy không được vượt quá 500 ký tự."); // Ràng buộc độ dài chuỗi tối đa không được vượt quá 500 ký tự.
        }
    }

    public class QuotationAcceptValidator : AbstractValidator<QuotationAcceptDto> // Định nghĩa lớp kiểm định dữ liệu QuotationAcceptValidator kế thừa từ AbstractValidator cho QuotationAcceptDto.
    {
        public QuotationAcceptValidator() // Thực thi dòng lệnh nghiệp vụ.
        {
            RuleFor(x => x.NextStatus) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính NextStatus.
                .Must(x => x == 4 || x == 5).WithMessage("Trạng thái tiếp theo không hợp lệ."); // Thiết lập thông báo lỗi khi kiểm tra thất bại: 'Trạng thái tiếp theo không hợp lệ.'.
        }
    }

    public class QuotationRejectValidator : AbstractValidator<QuotationRejectDto> // Định nghĩa lớp kiểm định dữ liệu QuotationRejectValidator kế thừa từ AbstractValidator cho QuotationRejectDto.
    {
        public QuotationRejectValidator() // Thực thi dòng lệnh nghiệp vụ.
        {
            RuleFor(x => x.Reason) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính Reason.
                .NotEmpty().WithMessage("Vui lòng nhập lý do từ chối báo giá.") // Ràng buộc kiểm tra thuộc tính không được để trống (Not Empty).
                .MaximumLength(1000).WithMessage("Lý do từ chối không được vượt quá 1000 ký tự."); // Ràng buộc độ dài chuỗi tối đa không được vượt quá 1000 ký tự.
        }
    }
}

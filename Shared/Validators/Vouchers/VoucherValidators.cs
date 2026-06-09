using FluentValidation; // Nhập thư viện FluentValidation để hỗ trợ kiểm định dữ liệu người dùng nhập vào.
using PBL3.Shared.DTOs.Vouchers; // Nhập (import) namespace PBL3.Shared.DTOs.Vouchers để sử dụng các thành phần bên trong.

namespace PBL3.Shared.Validators.Vouchers // Định nghĩa namespace PBL3.Shared.Validators.Vouchers quản lý cấu trúc code truyền tải và validator.
{
    public class CreateVoucherRequestValidator : AbstractValidator<CreateVoucherRequest> // Định nghĩa lớp kiểm định dữ liệu CreateVoucherRequestValidator kế thừa từ AbstractValidator cho CreateVoucherRequest.
    {
        public CreateVoucherRequestValidator() // Thực thi dòng lệnh nghiệp vụ.
        {
            RuleFor(x => x.Code) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính Code.
                .NotEmpty().WithMessage("Mã voucher không được để trống.") // Ràng buộc kiểm tra thuộc tính không được để trống (Not Empty).
                .MaximumLength(50).WithMessage("Mã voucher không được vượt quá 50 ký tự.") // Ràng buộc độ dài chuỗi tối đa không được vượt quá 50 ký tự.
                .Matches("^[A-Z0-9_-]+$").WithMessage("Mã voucher chỉ được dùng chữ hoa, số và ký tự - _."); // Thiết lập thông báo lỗi khi kiểm tra thất bại: 'Mã voucher chỉ được dùng chữ hoa, số và ký tự - _.'.

            RuleFor(x => x.Name) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính Name.
                .NotEmpty().WithMessage("Tên voucher không được để trống.") // Ràng buộc kiểm tra thuộc tính không được để trống (Not Empty).
                .MaximumLength(200).WithMessage("Tên voucher không được vượt quá 200 ký tự."); // Ràng buộc độ dài chuỗi tối đa không được vượt quá 200 ký tự.

            RuleFor(x => x.DiscountType) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính DiscountType.
                .InclusiveBetween((byte)0, (byte)1).WithMessage("Loại giảm giá không hợp lệ. Chỉ chấp nhận 0 (tiền cố định) hoặc 1 (phần trăm)."); // Thiết lập thông báo lỗi khi kiểm tra thất bại: 'Loại giảm giá không hợp lệ. Chỉ chấp nhận 0 (tiền cố định) hoặc 1 (phần trăm).'.

            RuleFor(x => x.DiscountValue) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính DiscountValue.
                .GreaterThan(0).WithMessage("Giá trị giảm phải lớn hơn 0."); // Thiết lập thông báo lỗi khi kiểm tra thất bại: 'Giá trị giảm phải lớn hơn 0.'.

            RuleFor(x => x.DiscountValue) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính DiscountValue.
                .LessThanOrEqualTo(100) // Ràng buộc giá trị số phải nhỏ hơn hoặc bằng 100.
                .WithMessage("Phần trăm giảm không được vượt quá 100%.") // Thiết lập thông báo lỗi khi kiểm tra thất bại: 'Phần trăm giảm không được vượt quá 100%.'.
                .When(x => x.DiscountType == 1); // Chỉ áp dụng các quy tắc kiểm tra này khi thỏa mãn điều kiện phía dưới.

            RuleFor(x => x.MinOrderValue) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính MinOrderValue.
                .GreaterThanOrEqualTo(0).WithMessage("Giá trị đơn hàng tối thiểu không được âm."); // Thiết lập thông báo lỗi khi kiểm tra thất bại: 'Giá trị đơn hàng tối thiểu không được âm.'.

            RuleFor(x => x.MaxDiscountAmount) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính MaxDiscountAmount.
                .GreaterThan(0).WithMessage("Số tiền giảm tối đa phải lớn hơn 0.") // Thiết lập thông báo lỗi khi kiểm tra thất bại: 'Số tiền giảm tối đa phải lớn hơn 0.'.
                .When(x => x.MaxDiscountAmount.HasValue); // Chỉ áp dụng các quy tắc kiểm tra này khi thỏa mãn điều kiện phía dưới.

            RuleFor(x => x.StartDate) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính StartDate.
                .NotEmpty().WithMessage("Ngày bắt đầu không được để trống."); // Ràng buộc kiểm tra thuộc tính không được để trống (Not Empty).

            RuleFor(x => x.EndDate) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính EndDate.
                .NotEmpty().WithMessage("Ngày kết thúc không được để trống.") // Ràng buộc kiểm tra thuộc tính không được để trống (Not Empty).
                .GreaterThan(x => x.StartDate).WithMessage("Ngày kết thúc phải sau ngày bắt đầu."); // Thiết lập thông báo lỗi khi kiểm tra thất bại: 'Ngày kết thúc phải sau ngày bắt đầu.'.

            RuleFor(x => x.Quantity) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính Quantity.
                .GreaterThan(0).WithMessage("Số lượng phát hành phải lớn hơn 0.") // Thiết lập thông báo lỗi khi kiểm tra thất bại: 'Số lượng phát hành phải lớn hơn 0.'.
                .When(x => x.Quantity.HasValue); // Chỉ áp dụng các quy tắc kiểm tra này khi thỏa mãn điều kiện phía dưới.

            RuleFor(x => x.MaxUsesPerUser) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính MaxUsesPerUser.
                .GreaterThan(0).WithMessage("Số lần sử dụng tối đa mỗi khách phải lớn hơn 0.") // Thiết lập thông báo lỗi khi kiểm tra thất bại: 'Số lần sử dụng tối đa mỗi khách phải lớn hơn 0.'.
                .When(x => x.MaxUsesPerUser.HasValue); // Chỉ áp dụng các quy tắc kiểm tra này khi thỏa mãn điều kiện phía dưới.

            RuleFor(x => x.ApplyFor) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính ApplyFor.
                .InclusiveBetween((byte)0, (byte)2).WithMessage("Kênh áp dụng không hợp lệ. Chỉ chấp nhận 0 (Cả hai), 1 (Online), 2 (Quầy)."); // Thiết lập thông báo lỗi khi kiểm tra thất bại: 'Kênh áp dụng không hợp lệ. Chỉ chấp nhận 0 (Cả hai), 1 (Online), 2 (Quầy).'.

            RuleFor(x => x.Description) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính Description.
                .MaximumLength(500).WithMessage("Mô tả không được vượt quá 500 ký tự.") // Ràng buộc độ dài chuỗi tối đa không được vượt quá 500 ký tự.
                .When(x => !string.IsNullOrWhiteSpace(x.Description)); // Chỉ áp dụng các quy tắc kiểm tra này khi thỏa mãn điều kiện phía dưới.
        }
    }

    public class UpdateVoucherRequestValidator : AbstractValidator<UpdateVoucherRequest> // Định nghĩa lớp kiểm định dữ liệu UpdateVoucherRequestValidator kế thừa từ AbstractValidator cho UpdateVoucherRequest.
    {
        public UpdateVoucherRequestValidator() // Thực thi dòng lệnh nghiệp vụ.
        {
            RuleFor(x => x.Name) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính Name.
                .NotEmpty().WithMessage("Tên voucher không được để trống.") // Ràng buộc kiểm tra thuộc tính không được để trống (Not Empty).
                .MaximumLength(200).WithMessage("Tên voucher không được vượt quá 200 ký tự."); // Ràng buộc độ dài chuỗi tối đa không được vượt quá 200 ký tự.

            RuleFor(x => x.DiscountType) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính DiscountType.
                .InclusiveBetween((byte)0, (byte)1).WithMessage("Loại giảm giá không hợp lệ. Chỉ chấp nhận 0 (tiền cố định) hoặc 1 (phần trăm)."); // Thiết lập thông báo lỗi khi kiểm tra thất bại: 'Loại giảm giá không hợp lệ. Chỉ chấp nhận 0 (tiền cố định) hoặc 1 (phần trăm).'.

            RuleFor(x => x.DiscountValue) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính DiscountValue.
                .GreaterThan(0).WithMessage("Giá trị giảm phải lớn hơn 0."); // Thiết lập thông báo lỗi khi kiểm tra thất bại: 'Giá trị giảm phải lớn hơn 0.'.

            RuleFor(x => x.DiscountValue) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính DiscountValue.
                .LessThanOrEqualTo(100) // Ràng buộc giá trị số phải nhỏ hơn hoặc bằng 100.
                .WithMessage("Phần trăm giảm không được vượt quá 100%.") // Thiết lập thông báo lỗi khi kiểm tra thất bại: 'Phần trăm giảm không được vượt quá 100%.'.
                .When(x => x.DiscountType == 1); // Chỉ áp dụng các quy tắc kiểm tra này khi thỏa mãn điều kiện phía dưới.

            RuleFor(x => x.MinOrderValue) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính MinOrderValue.
                .GreaterThanOrEqualTo(0).WithMessage("Giá trị đơn hàng tối thiểu không được âm."); // Thiết lập thông báo lỗi khi kiểm tra thất bại: 'Giá trị đơn hàng tối thiểu không được âm.'.

            RuleFor(x => x.MaxDiscountAmount) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính MaxDiscountAmount.
                .GreaterThan(0).WithMessage("Số tiền giảm tối đa phải lớn hơn 0.") // Thiết lập thông báo lỗi khi kiểm tra thất bại: 'Số tiền giảm tối đa phải lớn hơn 0.'.
                .When(x => x.MaxDiscountAmount.HasValue); // Chỉ áp dụng các quy tắc kiểm tra này khi thỏa mãn điều kiện phía dưới.

            RuleFor(x => x.StartDate) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính StartDate.
                .NotEmpty().WithMessage("Ngày bắt đầu không được để trống."); // Ràng buộc kiểm tra thuộc tính không được để trống (Not Empty).

            RuleFor(x => x.EndDate) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính EndDate.
                .NotEmpty().WithMessage("Ngày kết thúc không được để trống.") // Ràng buộc kiểm tra thuộc tính không được để trống (Not Empty).
                .GreaterThan(x => x.StartDate).WithMessage("Ngày kết thúc phải sau ngày bắt đầu."); // Thiết lập thông báo lỗi khi kiểm tra thất bại: 'Ngày kết thúc phải sau ngày bắt đầu.'.

            RuleFor(x => x.Quantity) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính Quantity.
                .GreaterThan(0).WithMessage("Số lượng phát hành phải lớn hơn 0.") // Thiết lập thông báo lỗi khi kiểm tra thất bại: 'Số lượng phát hành phải lớn hơn 0.'.
                .When(x => x.Quantity.HasValue); // Chỉ áp dụng các quy tắc kiểm tra này khi thỏa mãn điều kiện phía dưới.

            RuleFor(x => x.MaxUsesPerUser) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính MaxUsesPerUser.
                .GreaterThan(0).WithMessage("Số lần sử dụng tối đa mỗi khách phải lớn hơn 0.") // Thiết lập thông báo lỗi khi kiểm tra thất bại: 'Số lần sử dụng tối đa mỗi khách phải lớn hơn 0.'.
                .When(x => x.MaxUsesPerUser.HasValue); // Chỉ áp dụng các quy tắc kiểm tra này khi thỏa mãn điều kiện phía dưới.

            RuleFor(x => x.ApplyFor) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính ApplyFor.
                .InclusiveBetween((byte)0, (byte)2).WithMessage("Kênh áp dụng không hợp lệ. Chỉ chấp nhận 0 (Cả hai), 1 (Online), 2 (Quầy)."); // Thiết lập thông báo lỗi khi kiểm tra thất bại: 'Kênh áp dụng không hợp lệ. Chỉ chấp nhận 0 (Cả hai), 1 (Online), 2 (Quầy).'.

            RuleFor(x => x.Description) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính Description.
                .MaximumLength(500).WithMessage("Mô tả không được vượt quá 500 ký tự.") // Ràng buộc độ dài chuỗi tối đa không được vượt quá 500 ký tự.
                .When(x => !string.IsNullOrWhiteSpace(x.Description)); // Chỉ áp dụng các quy tắc kiểm tra này khi thỏa mãn điều kiện phía dưới.
        }
    }
}

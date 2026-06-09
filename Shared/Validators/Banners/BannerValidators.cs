using FluentValidation; // Nhập thư viện FluentValidation để hỗ trợ kiểm định dữ liệu người dùng nhập vào.
using PBL3.Shared.DTOs.Banners; // Nhập (import) namespace PBL3.Shared.DTOs.Banners để sử dụng các thành phần bên trong.

namespace PBL3.Shared.Validators.Banners // Định nghĩa namespace PBL3.Shared.Validators.Banners quản lý cấu trúc code truyền tải và validator.
{
    /// <summary>
    /// Validator cho banner. <see cref="UpdateBannerRequest"/> kế thừa <see cref="CreateBannerRequest"/>
    /// nên dùng chung validator này thông qua <see cref="UpdateBannerRequestValidator"/>.
    /// </summary>
    public class CreateBannerRequestValidator : AbstractValidator<CreateBannerRequest> // Định nghĩa lớp kiểm định dữ liệu CreateBannerRequestValidator kế thừa từ AbstractValidator cho CreateBannerRequest.
    {
        public CreateBannerRequestValidator() // Thực thi dòng lệnh nghiệp vụ.
        {
            RuleFor(x => x.Title) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính Title.
                .NotEmpty().WithMessage("Tiêu đề banner không được để trống.") // Ràng buộc kiểm tra thuộc tính không được để trống (Not Empty).
                .MaximumLength(200).WithMessage("Tiêu đề banner không được vượt quá 200 ký tự."); // Ràng buộc độ dài chuỗi tối đa không được vượt quá 200 ký tự.

            RuleFor(x => x.ImageUrl) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính ImageUrl.
                .NotEmpty().WithMessage("Ảnh banner không được để trống. Vui lòng tải lên ảnh trước khi lưu.") // Ràng buộc kiểm tra thuộc tính không được để trống (Not Empty).
                .MaximumLength(500).WithMessage("Đường dẫn ảnh không được vượt quá 500 ký tự."); // Ràng buộc độ dài chuỗi tối đa không được vượt quá 500 ký tự.

            RuleFor(x => x.LinkUrl) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính LinkUrl.
                .MaximumLength(500).WithMessage("Đường dẫn liên kết không được vượt quá 500 ký tự.") // Ràng buộc độ dài chuỗi tối đa không được vượt quá 500 ký tự.
                .Must(BeAValidLink).WithMessage("Đường dẫn liên kết phải bắt đầu bằng http://, https:// hoặc / (đường dẫn nội bộ).") // Thiết lập thông báo lỗi khi kiểm tra thất bại: 'Đường dẫn liên kết phải bắt đầu bằng http://, https:// hoặc / (đường dẫn nội bộ).'.
                .When(x => !string.IsNullOrWhiteSpace(x.LinkUrl)); // Chỉ áp dụng các quy tắc kiểm tra này khi thỏa mãn điều kiện phía dưới.

            RuleFor(x => x.SortOrder) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính SortOrder.
                .GreaterThanOrEqualTo(0).WithMessage("Thứ tự hiển thị phải là số nguyên không âm."); // Thiết lập thông báo lỗi khi kiểm tra thất bại: 'Thứ tự hiển thị phải là số nguyên không âm.'.

            RuleFor(x => x) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính thuộc tính.
                .Must(x => !x.StartDate.HasValue || !x.EndDate.HasValue || x.EndDate.Value >= x.StartDate.Value) // Áp dụng điều kiện kiểm tra logic tùy chỉnh (Must) cho thuộc tính.
                .WithMessage("Ngày kết thúc phải lớn hơn hoặc bằng ngày bắt đầu."); // Thiết lập thông báo lỗi khi kiểm tra thất bại: 'Ngày kết thúc phải lớn hơn hoặc bằng ngày bắt đầu.'.
        }

        private static bool BeAValidLink(string? url) // Thực hiện xử lý phương thức 'BeAValidLink' nhận tham số (url) trả về kiểu bool.
        {
            if (string.IsNullOrWhiteSpace(url)) return true; // Kiểm tra điều kiện: 'string.IsNullOrWhiteSpace(url'.
            return url.StartsWith("http://") || url.StartsWith("https://") || url.StartsWith("/"); // Trả về kết quả: 'url.StartsWith("http://") || url.StartsWith("https://") || url.StartsWith("/")'.
        }
    }

    /// <summary>
    /// Update dùng chung rule với Create (DTO kế thừa).
    /// </summary>
    public class UpdateBannerRequestValidator : AbstractValidator<UpdateBannerRequest> // Định nghĩa lớp kiểm định dữ liệu UpdateBannerRequestValidator kế thừa từ AbstractValidator cho UpdateBannerRequest.
    {
        public UpdateBannerRequestValidator() // Thực thi dòng lệnh nghiệp vụ.
        {
            Include(new CreateBannerRequestValidator()); // Nhúng bộ quy tắc kiểm định của một Validator khác vào lớp này.
        }
    }
}

using FluentValidation; // Nhập thư viện FluentValidation để hỗ trợ kiểm định dữ liệu người dùng nhập vào.
using PBL3.Shared.DTOs.Reviews; // Nhập (import) namespace PBL3.Shared.DTOs.Reviews để sử dụng các thành phần bên trong.

namespace PBL3.Shared.Validators.Reviews // Định nghĩa namespace PBL3.Shared.Validators.Reviews quản lý cấu trúc code truyền tải và validator.
{
    public class CreateReviewRequestValidator : AbstractValidator<CreateReviewRequest> // Định nghĩa lớp kiểm định dữ liệu CreateReviewRequestValidator kế thừa từ AbstractValidator cho CreateReviewRequest.
    {
        public CreateReviewRequestValidator() // Thực thi dòng lệnh nghiệp vụ.
        {
            RuleFor(x => x.ProductId) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính ProductId.
                .GreaterThan(0).WithMessage("Id sản phẩm không hợp lệ."); // Thiết lập thông báo lỗi khi kiểm tra thất bại: 'Id sản phẩm không hợp lệ.'.

            RuleFor(x => x.Rating) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính Rating.
                .InclusiveBetween((byte)1, (byte)5) // Thực thi dòng lệnh nghiệp vụ.
                .WithMessage("Đánh giá phải từ 1 đến 5 sao."); // Thiết lập thông báo lỗi khi kiểm tra thất bại: 'Đánh giá phải từ 1 đến 5 sao.'.

            RuleFor(x => x.Title) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính Title.
                .MaximumLength(200) // Ràng buộc độ dài chuỗi tối đa không được vượt quá 200 ký tự.
                .WithMessage("Tiêu đề không được vượt quá 200 ký tự.") // Thiết lập thông báo lỗi khi kiểm tra thất bại: 'Tiêu đề không được vượt quá 200 ký tự.'.
                .When(x => !string.IsNullOrEmpty(x.Title)); // Chỉ áp dụng các quy tắc kiểm tra này khi thỏa mãn điều kiện phía dưới.

            RuleFor(x => x.Content) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính Content.
                .MaximumLength(2000) // Ràng buộc độ dài chuỗi tối đa không được vượt quá 2000 ký tự.
                .WithMessage("Nội dung không được vượt quá 2000 ký tự.") // Thiết lập thông báo lỗi khi kiểm tra thất bại: 'Nội dung không được vượt quá 2000 ký tự.'.
                .When(x => !string.IsNullOrEmpty(x.Content)); // Chỉ áp dụng các quy tắc kiểm tra này khi thỏa mãn điều kiện phía dưới.
        }
    }
}

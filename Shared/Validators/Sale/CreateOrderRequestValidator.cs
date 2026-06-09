using System.Linq; // Nhập thư viện LINQ hỗ trợ truy vấn dữ liệu nhanh chóng.
using FluentValidation; // Nhập thư viện FluentValidation để hỗ trợ kiểm định dữ liệu người dùng nhập vào.
using PBL3.Shared.DTOs.Sale; // Nhập (import) namespace PBL3.Shared.DTOs.Sale để sử dụng các thành phần bên trong.

namespace PBL3.Shared.Validators.Sale // Định nghĩa namespace PBL3.Shared.Validators.Sale quản lý cấu trúc code truyền tải và validator.
{
    public class CreateOrderRequestValidator : AbstractValidator<CreateOrderRequest> // Định nghĩa lớp kiểm định dữ liệu CreateOrderRequestValidator kế thừa từ AbstractValidator cho CreateOrderRequest.
    {
        public CreateOrderRequestValidator() // Hàm khởi tạo (Constructor) của lớp CreateOrderRequestValidator.
        {
            RuleFor(x => x.ShipName).NotEmpty().WithMessage("Tên người nhận không được để trống."); // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính ShipName.
            RuleFor(x => x.ShipPhone).NotEmpty().WithMessage("Số điện thoại không được để trống.") // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính ShipPhone.
                .Matches(@"^0\d{9}$").WithMessage("Số điện thoại không hợp lệ."); // Thiết lập thông báo lỗi khi kiểm tra thất bại: 'Số điện thoại không hợp lệ.'.
            RuleFor(x => x.ShipAddress).NotEmpty().WithMessage("Địa chỉ giao hàng không được để trống."); // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính ShipAddress.
            RuleFor(x => x.ShipCity).NotEmpty().WithMessage("Thành phố không được để trống."); // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính ShipCity.

            RuleFor(x => x.Items).NotEmpty().WithMessage("Đơn hàng phải có ít nhất một sản phẩm."); // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính Items.
            RuleForEach(x => x.Items).ChildRules(item => // Thực thi dòng lệnh nghiệp vụ.
            {
                item.RuleFor(i => i.VariantId).GreaterThan(0).WithMessage("Mã sản phẩm không hợp lệ."); // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính VariantId.
                item.RuleFor(i => i.Quantity).GreaterThan(0).WithMessage("Số lượng phải lớn hơn 0."); // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính Quantity.
            }); // Thực thi dòng lệnh nghiệp vụ.

            // Voucher codes: Nếu có thì không được trùng lặp nội bộ
            RuleFor(x => x.VoucherCodes) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính VoucherCodes.
                .Must(codes => codes == null || codes.Distinct().Count() == codes.Count) // Áp dụng điều kiện kiểm tra logic tùy chỉnh (Must) cho thuộc tính.
                .WithMessage("Danh sách mã giảm giá không được chứa mã trùng lặp."); // Thiết lập thông báo lỗi khi kiểm tra thất bại: 'Danh sách mã giảm giá không được chứa mã trùng lặp.'.
        }
    }
}

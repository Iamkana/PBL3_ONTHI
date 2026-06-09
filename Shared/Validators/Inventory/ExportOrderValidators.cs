using FluentValidation; // Nhập thư viện FluentValidation để hỗ trợ kiểm định dữ liệu người dùng nhập vào.
using PBL3.Shared.DTOs.Inventory; // Nhập (import) namespace PBL3.Shared.DTOs.Inventory để sử dụng các thành phần bên trong.
using System.Linq; // Nhập thư viện LINQ hỗ trợ truy vấn dữ liệu nhanh chóng.

namespace PBL3.Shared.Validators.Inventory // Định nghĩa namespace PBL3.Shared.Validators.Inventory quản lý cấu trúc code truyền tải và validator.
{
    public class ExportOrderRequestValidator : AbstractValidator<ExportOrderRequest> // Định nghĩa lớp kiểm định dữ liệu ExportOrderRequestValidator kế thừa từ AbstractValidator cho ExportOrderRequest.
    {
        public ExportOrderRequestValidator() // Thực thi dòng lệnh nghiệp vụ.
        {
            RuleFor(x => x.OrderId) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính OrderId.
                .GreaterThan(0).WithMessage("Mã đơn hàng không hợp lệ."); // Thiết lập thông báo lỗi khi kiểm tra thất bại: 'Mã đơn hàng không hợp lệ.'.

            RuleFor(x => x.Details) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính Details.
                .NotEmpty().WithMessage("Danh sách sản phẩm xuất kho không được để trống."); // Ràng buộc kiểm tra thuộc tính không được để trống (Not Empty).

            RuleForEach(x => x.Details) // Thực thi dòng lệnh nghiệp vụ.
                .SetValidator(new ExportOrderDetailRequestValidator()); // Thực thi dòng lệnh nghiệp vụ.

            // Check if there are any duplicate serial numbers across the entire request
            RuleFor(x => x.Details) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính Details.
                .Must(details => // Áp dụng điều kiện kiểm tra logic tùy chỉnh (Must) cho thuộc tính.
                {
                    if (details == null || !details.Any()) return true; // Kiểm tra điều kiện: 'details == null || !details.Any('.
                    var allSerials = details.SelectMany(d => d.SerialNumbers ?? new System.Collections.Generic.List<string>()).ToList(); // Thực thi dòng lệnh nghiệp vụ.
                    return allSerials.Count == allSerials.Distinct().Count(); // Trả về kết quả: 'allSerials.Count == allSerials.Distinct().Count()'.
                }) // Thực thi dòng lệnh nghiệp vụ.
                .WithMessage("Có mã Serial bị trùng lặp trong yêu cầu xuất kho.") // Thiết lập thông báo lỗi khi kiểm tra thất bại: 'Có mã Serial bị trùng lặp trong yêu cầu xuất kho.'.
                .When(x => x.Details != null && x.Details.Any()); // Chỉ áp dụng các quy tắc kiểm tra này khi thỏa mãn điều kiện phía dưới.
        }
    }

    public class ExportOrderDetailRequestValidator : AbstractValidator<ExportOrderDetailRequest> // Định nghĩa lớp kiểm định dữ liệu ExportOrderDetailRequestValidator kế thừa từ AbstractValidator cho ExportOrderDetailRequest.
    {
        public ExportOrderDetailRequestValidator() // Thực thi dòng lệnh nghiệp vụ.
        {
            RuleFor(x => x.OrderDetailId) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính OrderDetailId.
                .GreaterThan(0).WithMessage("Mã chi tiết đơn hàng không hợp lệ."); // Thiết lập thông báo lỗi khi kiểm tra thất bại: 'Mã chi tiết đơn hàng không hợp lệ.'.

            RuleFor(x => x.SerialNumbers) // Bắt đầu thiết lập quy tắc kiểm tra dữ liệu cho thuộc tính SerialNumbers.
                .NotEmpty().WithMessage("Danh sách Serial không được để trống."); // Ràng buộc kiểm tra thuộc tính không được để trống (Not Empty).

            RuleForEach(x => x.SerialNumbers) // Thực thi dòng lệnh nghiệp vụ.
                .NotEmpty().WithMessage("Mã Serial không được để trống."); // Ràng buộc kiểm tra thuộc tính không được để trống (Not Empty).
        }
    }
}

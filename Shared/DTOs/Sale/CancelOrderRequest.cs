using System; // Nhập thư viện hệ thống cơ bản.
using PBL3.Shared.DTOs.Common; // Nhập (import) namespace PBL3.Shared.DTOs.Common để sử dụng các thành phần bên trong.

namespace PBL3.Shared.DTOs.Sale // Định nghĩa namespace PBL3.Shared.DTOs.Sale quản lý cấu trúc code truyền tải và validator.
{
    public class CancelOrderRequest // Định nghĩa lớp DTO truyền tải dữ liệu CancelOrderRequest.
    {
        public string CancelReason { get; set; } = string.Empty; // Thuộc tính CancelReason kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
    }
}

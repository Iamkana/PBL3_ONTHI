using System; // Nhập thư viện hệ thống cơ bản.
using PBL3.Shared.DTOs.Common; // Nhập (import) namespace PBL3.Shared.DTOs.Common để sử dụng các thành phần bên trong.

namespace PBL3.Shared.DTOs.Sale // Định nghĩa namespace PBL3.Shared.DTOs.Sale quản lý cấu trúc code truyền tải và validator.
{
    public class OrderSummaryResponse // Định nghĩa lớp DTO truyền tải dữ liệu OrderSummaryResponse.
    {
        public int Id { get; set; } // Thuộc tính Id kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public string OrderCode { get; set; } = string.Empty; // Thuộc tính OrderCode kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public string CustomerName { get; set; } = string.Empty; // Thuộc tính CustomerName kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public string CustomerPhone { get; set; } = string.Empty; // Thuộc tính CustomerPhone kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public string? CustomerAvatarUrl { get; set; } // Thuộc tính CustomerAvatarUrl kiểu dữ liệu string? lưu trữ thông tin truyền tải.
        public decimal TotalAmount { get; set; } // Thuộc tính TotalAmount kiểu dữ liệu decimal lưu trữ thông tin truyền tải.
        public DateTime CreatedDate { get; set; } // Thuộc tính CreatedDate kiểu dữ liệu DateTime lưu trữ thông tin truyền tải.
        public byte Status { get; set; } // Thuộc tính Status kiểu dữ liệu byte lưu trữ thông tin truyền tải.
        public byte PaymentStatus { get; set; } // Thuộc tính PaymentStatus kiểu dữ liệu byte lưu trữ thông tin truyền tải.
    }
}

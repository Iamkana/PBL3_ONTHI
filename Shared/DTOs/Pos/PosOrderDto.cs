using System.Collections.Generic; // Nhập thư viện làm việc với các kiểu danh sách, bộ sưu tập.
using System.ComponentModel.DataAnnotations; // Nhập (import) namespace System.ComponentModel.DataAnnotations để sử dụng các thành phần bên trong.
using System; // Nhập thư viện hệ thống cơ bản.

namespace PBL3.Shared.DTOs.Pos // Định nghĩa namespace PBL3.Shared.DTOs.Pos quản lý cấu trúc code truyền tải và validator.
{
    public class PosOrderDto // Định nghĩa lớp DTO truyền tải dữ liệu PosOrderDto.
    {
        public int OrderId { get; set; } // Thuộc tính OrderId kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public string OrderCode { get; set; } = string.Empty; // Thuộc tính OrderCode kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public DateTime OrderDate { get; set; } // Thuộc tính OrderDate kiểu dữ liệu DateTime lưu trữ thông tin truyền tải.
        public decimal SubTotal { get; set; } // Thuộc tính SubTotal kiểu dữ liệu decimal lưu trữ thông tin truyền tải.
        public decimal DiscountAmount { get; set; } // Thuộc tính DiscountAmount kiểu dữ liệu decimal lưu trữ thông tin truyền tải.
        public decimal TotalAmount { get; set; } // Thuộc tính TotalAmount kiểu dữ liệu decimal lưu trữ thông tin truyền tải.
        public string? CustomerName { get; set; } // Thuộc tính CustomerName kiểu dữ liệu string? lưu trữ thông tin truyền tải.
        public string? CustomerPhone { get; set; } // Thuộc tính CustomerPhone kiểu dữ liệu string? lưu trữ thông tin truyền tải.
    }
}

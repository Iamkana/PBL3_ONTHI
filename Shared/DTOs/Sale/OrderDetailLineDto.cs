using System; // Nhập thư viện hệ thống cơ bản.
using System.Collections.Generic; // Nhập thư viện làm việc với các kiểu danh sách, bộ sưu tập.

namespace PBL3.Shared.DTOs.Sale // Định nghĩa namespace PBL3.Shared.DTOs.Sale quản lý cấu trúc code truyền tải và validator.
{
    public class OrderDetailLineDto // Định nghĩa lớp DTO truyền tải dữ liệu OrderDetailLineDto.
    {
        public int Id { get; set; } // Thuộc tính Id kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public int VariantId { get; set; } // Thuộc tính VariantId kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public string VariantName { get; set; } = string.Empty; // Thuộc tính VariantName kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public string SKU { get; set; } = string.Empty; // Thuộc tính SKU kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public int Quantity { get; set; } // Thuộc tính Quantity kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public decimal UnitPrice { get; set; } // Thuộc tính UnitPrice kiểu dữ liệu decimal lưu trữ thông tin truyền tải.
        public decimal TotalLine { get; set; } // Thuộc tính TotalLine kiểu dữ liệu decimal lưu trữ thông tin truyền tải.
        public string? MainImageUrl { get; set; } // Thuộc tính MainImageUrl kiểu dữ liệu string? lưu trữ thông tin truyền tải.
        public List<string> Serials { get; set; } = new(); // Thuộc tính Serials kiểu dữ liệu List<string> lưu trữ thông tin truyền tải với giá trị mặc định là new().
    }
}

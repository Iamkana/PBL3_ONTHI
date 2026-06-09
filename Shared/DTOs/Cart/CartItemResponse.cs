using System.Collections.Generic; // Nhập thư viện làm việc với các kiểu danh sách, bộ sưu tập.
using System.ComponentModel.DataAnnotations; // Nhập (import) namespace System.ComponentModel.DataAnnotations để sử dụng các thành phần bên trong.

namespace PBL3.Shared.DTOs.Cart // Định nghĩa namespace PBL3.Shared.DTOs.Cart quản lý cấu trúc code truyền tải và validator.
{
    /// <summary>
    /// Response chi tiết 1 item trong giỏ hàng.
    public class CartItemResponse // Định nghĩa lớp DTO truyền tải dữ liệu CartItemResponse.
    {
        public int Id { get; set; } // Thuộc tính Id kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public int VariantId { get; set; } // Thuộc tính VariantId kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public string ProductName { get; set; } = string.Empty; // Thuộc tính ProductName kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public string VariantName { get; set; } = string.Empty; // Thuộc tính VariantName kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public string? ImageUrl { get; set; } // Thuộc tính ImageUrl kiểu dữ liệu string? lưu trữ thông tin truyền tải.
        public decimal UnitPrice { get; set; } // Thuộc tính UnitPrice kiểu dữ liệu decimal lưu trữ thông tin truyền tải.
        public int Quantity { get; set; } // Thuộc tính Quantity kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public decimal SubTotal { get; set; } // Thuộc tính SubTotal kiểu dữ liệu decimal lưu trữ thông tin truyền tải.
        public int StockQuantity { get; set; } // Thuộc tính StockQuantity kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public string ProductSlug { get; set; } = string.Empty; // Thuộc tính ProductSlug kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
    }
}

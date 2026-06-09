using System.Collections.Generic; // Nhập thư viện làm việc với các kiểu danh sách, bộ sưu tập.
using System.ComponentModel.DataAnnotations; // Nhập (import) namespace System.ComponentModel.DataAnnotations để sử dụng các thành phần bên trong.

namespace PBL3.Shared.DTOs.Sale // Định nghĩa namespace PBL3.Shared.DTOs.Sale quản lý cấu trúc code truyền tải và validator.
{
    public class CheckoutResponse // Định nghĩa lớp DTO truyền tải dữ liệu CheckoutResponse.
    {
        public int OrderId { get; set; } // Thuộc tính OrderId kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public string OrderCode { get; set; } = string.Empty; // Thuộc tính OrderCode kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public decimal TotalAmount { get; set; } // Thuộc tính TotalAmount kiểu dữ liệu decimal lưu trữ thông tin truyền tải.
        public byte Status { get; set; } // Thuộc tính Status kiểu dữ liệu byte lưu trữ thông tin truyền tải.
        public byte PaymentMethod { get; set; } // Thuộc tính PaymentMethod kiểu dữ liệu byte lưu trữ thông tin truyền tải.
        // URL redirect cho Online Payment (nullable — chỉ có khi PaymentMethod == 1)
        public string? PaymentUrl { get; set; } // Thuộc tính PaymentUrl kiểu dữ liệu string? lưu trữ thông tin truyền tải.
    }
}

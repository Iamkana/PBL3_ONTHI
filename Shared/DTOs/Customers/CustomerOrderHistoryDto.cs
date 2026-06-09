using System; // Nhập thư viện hệ thống cơ bản.
using System.Collections.Generic; // Nhập thư viện làm việc với các kiểu danh sách, bộ sưu tập.
using PBL3.Shared.DTOs.Products; // Nhập (import) namespace PBL3.Shared.DTOs.Products để sử dụng các thành phần bên trong.
using PBL3.Shared.DTOs.Sale; // Nhập (import) namespace PBL3.Shared.DTOs.Sale để sử dụng các thành phần bên trong.

namespace PBL3.Shared.DTOs.Customers // Định nghĩa namespace PBL3.Shared.DTOs.Customers quản lý cấu trúc code truyền tải và validator.
{
    public class CustomerOrderHistoryDto // Định nghĩa lớp DTO truyền tải dữ liệu CustomerOrderHistoryDto.
    {
        public int Id { get; set; } // Thuộc tính Id kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public string OrderCode { get; set; } = string.Empty; // Thuộc tính OrderCode kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public DateTime OrderDate { get; set; } // Thuộc tính OrderDate kiểu dữ liệu DateTime lưu trữ thông tin truyền tải.
        public decimal TotalAmount { get; set; } // Thuộc tính TotalAmount kiểu dữ liệu decimal lưu trữ thông tin truyền tải.
        public byte Status { get; set; } // Thuộc tính Status kiểu dữ liệu byte lưu trữ thông tin truyền tải.
    }
}

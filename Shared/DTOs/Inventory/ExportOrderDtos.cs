using System.Collections.Generic; // Nhập thư viện làm việc với các kiểu danh sách, bộ sưu tập.

namespace PBL3.Shared.DTOs.Inventory // Định nghĩa namespace PBL3.Shared.DTOs.Inventory quản lý cấu trúc code truyền tải và validator.
{
    public class ExportOrderRequest // Định nghĩa lớp DTO truyền tải dữ liệu ExportOrderRequest.
    {
        public int OrderId { get; set; } // Thuộc tính OrderId kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public List<ExportOrderDetailRequest> Details { get; set; } = new(); // Thuộc tính Details kiểu dữ liệu List<ExportOrderDetailRequest> lưu trữ thông tin truyền tải với giá trị mặc định là new().
    }

    public class ExportOrderDetailRequest // Định nghĩa lớp DTO truyền tải dữ liệu ExportOrderDetailRequest.
    {
        public int OrderDetailId { get; set; } // Thuộc tính OrderDetailId kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public List<string> SerialNumbers { get; set; } = new(); // Thuộc tính SerialNumbers kiểu dữ liệu List<string> lưu trữ thông tin truyền tải với giá trị mặc định là new().
    }
}

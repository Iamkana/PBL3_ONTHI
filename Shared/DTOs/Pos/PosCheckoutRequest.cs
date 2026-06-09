using System.Collections.Generic; // Nhập thư viện làm việc với các kiểu danh sách, bộ sưu tập.
using System.ComponentModel.DataAnnotations; // Nhập (import) namespace System.ComponentModel.DataAnnotations để sử dụng các thành phần bên trong.
using System; // Nhập thư viện hệ thống cơ bản.

namespace PBL3.Shared.DTOs.Pos // Định nghĩa namespace PBL3.Shared.DTOs.Pos quản lý cấu trúc code truyền tải và validator.
{
    public class PosCheckoutRequest // Định nghĩa lớp DTO truyền tải dữ liệu PosCheckoutRequest.
    {
        public string? CustomerPhone { get; set; } // Thuộc tính CustomerPhone kiểu dữ liệu string? lưu trữ thông tin truyền tải.
        public string? VoucherCode { get; set; } // Thuộc tính VoucherCode kiểu dữ liệu string? lưu trữ thông tin truyền tải.
        public byte PaymentMethod { get; set; } // Thuộc tính PaymentMethod kiểu dữ liệu byte lưu trữ thông tin truyền tải.
        public string? EmployeeNote { get; set; } // Thuộc tính EmployeeNote kiểu dữ liệu string? lưu trữ thông tin truyền tải.
        public string? ShipAddress { get; set; } // Thuộc tính ShipAddress kiểu dữ liệu string? lưu trữ thông tin truyền tải.
        public string? ShipCity { get; set; } // Thuộc tính ShipCity kiểu dữ liệu string? lưu trữ thông tin truyền tải.
        public List<PosCheckoutItemRequest> Items { get; set; } = new(); // Thuộc tính Items kiểu dữ liệu List<PosCheckoutItemRequest> lưu trữ thông tin truyền tải với giá trị mặc định là new().
    }
}

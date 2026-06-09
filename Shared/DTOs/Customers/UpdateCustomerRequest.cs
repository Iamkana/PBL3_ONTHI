using System; // Nhập thư viện hệ thống cơ bản.
using System.Collections.Generic; // Nhập thư viện làm việc với các kiểu danh sách, bộ sưu tập.
using PBL3.Shared.DTOs.Products; // Nhập (import) namespace PBL3.Shared.DTOs.Products để sử dụng các thành phần bên trong.
using PBL3.Shared.DTOs.Sale; // Nhập (import) namespace PBL3.Shared.DTOs.Sale để sử dụng các thành phần bên trong.

namespace PBL3.Shared.DTOs.Customers // Định nghĩa namespace PBL3.Shared.DTOs.Customers quản lý cấu trúc code truyền tải và validator.
{
    public class UpdateCustomerRequest // Định nghĩa lớp DTO truyền tải dữ liệu UpdateCustomerRequest.
    {
        public string FullName { get; set; } = string.Empty; // Thuộc tính FullName kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public byte Gender { get; set; } // Thuộc tính Gender kiểu dữ liệu byte lưu trữ thông tin truyền tải.
        public DateTime? DateOfBirth { get; set; } // Thuộc tính DateOfBirth kiểu dữ liệu DateTime? lưu trữ thông tin truyền tải.
        public string? AvatarUrl { get; set; } // Thuộc tính AvatarUrl kiểu dữ liệu string? lưu trữ thông tin truyền tải.
        public string? Address { get; set; } // Thuộc tính Address kiểu dữ liệu string? lưu trữ thông tin truyền tải.
        public string? City { get; set; } // Thuộc tính City kiểu dữ liệu string? lưu trữ thông tin truyền tải.
    }
}

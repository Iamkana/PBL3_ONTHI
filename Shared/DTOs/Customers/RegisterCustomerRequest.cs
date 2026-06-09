using System; // Nhập thư viện hệ thống cơ bản.
using System.Collections.Generic; // Nhập thư viện làm việc với các kiểu danh sách, bộ sưu tập.
using PBL3.Shared.DTOs.Products; // Nhập (import) namespace PBL3.Shared.DTOs.Products để sử dụng các thành phần bên trong.
using PBL3.Shared.DTOs.Sale; // Nhập (import) namespace PBL3.Shared.DTOs.Sale để sử dụng các thành phần bên trong.

namespace PBL3.Shared.DTOs.Customers // Định nghĩa namespace PBL3.Shared.DTOs.Customers quản lý cấu trúc code truyền tải và validator.
{
    public class RegisterCustomerRequest // Định nghĩa lớp DTO truyền tải dữ liệu RegisterCustomerRequest.
    {
        public string FullName { get; set; } = string.Empty; // Thuộc tính FullName kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public string Email { get; set; } = string.Empty; // Thuộc tính Email kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public string PhoneNumber { get; set; } = string.Empty; // Thuộc tính PhoneNumber kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public string Password { get; set; } = string.Empty; // Thuộc tính Password kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public string ConfirmPassword { get; set; } = string.Empty; // Thuộc tính ConfirmPassword kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
    }
}

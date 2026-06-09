using System; // Nhập thư viện hệ thống cơ bản.

namespace PBL3.Shared.DTOs.Employees // Định nghĩa namespace PBL3.Shared.DTOs.Employees quản lý cấu trúc code truyền tải và validator.
{
    public class EmployeeListDto // Định nghĩa lớp DTO truyền tải dữ liệu EmployeeListDto.
    {
        public Guid Id { get; set; } // Thuộc tính Id kiểu dữ liệu Guid lưu trữ thông tin truyền tải.
        public string Email { get; set; } = string.Empty; // Thuộc tính Email kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public string PhoneNumber { get; set; } = string.Empty; // Thuộc tính PhoneNumber kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public string FullName { get; set; } = string.Empty; // Thuộc tính FullName kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public byte Gender { get; set; } // Thuộc tính Gender kiểu dữ liệu byte lưu trữ thông tin truyền tải.
        public DateTime? DateOfBirth { get; set; } // Thuộc tính DateOfBirth kiểu dữ liệu DateTime? lưu trữ thông tin truyền tải.
        public string? AvatarUrl { get; set; } // Thuộc tính AvatarUrl kiểu dữ liệu string? lưu trữ thông tin truyền tải.
        public string? Address { get; set; } // Thuộc tính Address kiểu dữ liệu string? lưu trữ thông tin truyền tải.
        public string? City { get; set; } // Thuộc tính City kiểu dữ liệu string? lưu trữ thông tin truyền tải.
        public bool IsActive { get; set; } // Thuộc tính IsActive kiểu dữ liệu bool lưu trữ thông tin truyền tải.
        public string? LockReason { get; set; } // Thuộc tính LockReason kiểu dữ liệu string? lưu trữ thông tin truyền tải.
        public DateTime CreatedDate { get; set; } // Thuộc tính CreatedDate kiểu dữ liệu DateTime lưu trữ thông tin truyền tải.
        public bool IsTechnician { get; set; } // Thuộc tính IsTechnician kiểu dữ liệu bool lưu trữ thông tin truyền tải.
    }
}

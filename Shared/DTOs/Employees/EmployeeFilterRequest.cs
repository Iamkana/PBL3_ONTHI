using System; // Nhập thư viện hệ thống cơ bản.

namespace PBL3.Shared.DTOs.Employees // Định nghĩa namespace PBL3.Shared.DTOs.Employees quản lý cấu trúc code truyền tải và validator.
{
    public class EmployeeFilterRequest // Định nghĩa lớp DTO truyền tải dữ liệu EmployeeFilterRequest.
    {
        public string? Keyword { get; set; } // Thuộc tính Keyword kiểu dữ liệu string? lưu trữ thông tin truyền tải.
        public bool? IsActive { get; set; } // Thuộc tính IsActive kiểu dữ liệu bool? lưu trữ thông tin truyền tải.
        public byte? Gender { get; set; } // Thuộc tính Gender kiểu dữ liệu byte? lưu trữ thông tin truyền tải.
        public int PageNumber { get; set; } = 1; // Thuộc tính PageNumber kiểu dữ liệu int lưu trữ thông tin truyền tải với giá trị mặc định là 1.
        public int PageSize { get; set; } = 10; // Thuộc tính PageSize kiểu dữ liệu int lưu trữ thông tin truyền tải với giá trị mặc định là 10.
        public string? SortBy { get; set; } // Thuộc tính SortBy kiểu dữ liệu string? lưu trữ thông tin truyền tải.
        public bool SortDescending { get; set; } // Thuộc tính SortDescending kiểu dữ liệu bool lưu trữ thông tin truyền tải.
    }
}

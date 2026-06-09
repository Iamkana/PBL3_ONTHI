using System; // Nhập thư viện hệ thống cơ bản.
using System.Collections.Generic; // Nhập thư viện làm việc với các kiểu danh sách, bộ sưu tập.
using PBL3.Shared.DTOs.Products; // Nhập (import) namespace PBL3.Shared.DTOs.Products để sử dụng các thành phần bên trong.
using PBL3.Shared.DTOs.Sale; // Nhập (import) namespace PBL3.Shared.DTOs.Sale để sử dụng các thành phần bên trong.

namespace PBL3.Shared.DTOs.Customers // Định nghĩa namespace PBL3.Shared.DTOs.Customers quản lý cấu trúc code truyền tải và validator.
{
    public class CustomerFilterRequest // Định nghĩa lớp DTO truyền tải dữ liệu CustomerFilterRequest.
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

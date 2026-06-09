using System; // Nhập thư viện hệ thống cơ bản.
using System.Collections.Generic; // Nhập thư viện làm việc với các kiểu danh sách, bộ sưu tập.

namespace PBL3.Shared.DTOs.Common // Định nghĩa namespace PBL3.Shared.DTOs.Common quản lý cấu trúc code truyền tải và validator.
{
    /// <summary>
    /// Kết quả phân trang dùng chung cho toàn hệ thống.
    /// </summary>
    public class PagedResult<T> // Định nghĩa lớp DTO truyền tải dữ liệu PagedResult.
    {
        public List<T> Items { get; set; } = new(); // Thuộc tính Items kiểu dữ liệu List<T> lưu trữ thông tin truyền tải với giá trị mặc định là new().
        public int TotalCount { get; set; } // Thuộc tính TotalCount kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public int PageNumber { get; set; } // Thuộc tính PageNumber kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public int PageSize { get; set; } // Thuộc tính PageSize kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / (PageSize > 0 ? PageSize : 1)); // Thực thi dòng lệnh nghiệp vụ.
    }
}

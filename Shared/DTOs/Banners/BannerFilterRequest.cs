namespace PBL3.Shared.DTOs.Banners // Định nghĩa namespace PBL3.Shared.DTOs.Banners quản lý cấu trúc code truyền tải và validator.
{
    /// <summary>
    /// Bộ lọc danh sách banner.
    /// </summary>
    public class BannerFilterRequest // Định nghĩa lớp DTO truyền tải dữ liệu BannerFilterRequest.
    {
        public string? Keyword { get; set; } // Thuộc tính Keyword kiểu dữ liệu string? lưu trữ thông tin truyền tải.
        public int PageNumber { get; set; } = 1; // Thuộc tính PageNumber kiểu dữ liệu int lưu trữ thông tin truyền tải với giá trị mặc định là 1.
        public int PageSize { get; set; } = 10; // Thuộc tính PageSize kiểu dữ liệu int lưu trữ thông tin truyền tải với giá trị mặc định là 10.
        public string? SortBy { get; set; } // Thuộc tính SortBy kiểu dữ liệu string? lưu trữ thông tin truyền tải.
        public bool SortDescending { get; set; } // Thuộc tính SortDescending kiểu dữ liệu bool lưu trữ thông tin truyền tải.
    }
}

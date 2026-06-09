namespace PBL3.Shared.DTOs.Banners // Định nghĩa namespace PBL3.Shared.DTOs.Banners quản lý cấu trúc code truyền tải và validator.
{
    /// <summary>
    /// DTO đầy đủ thông tin banner (dùng cho Admin).
    /// </summary>
    public class BannerDto // Định nghĩa lớp DTO truyền tải dữ liệu BannerDto.
    {
        public int Id { get; set; } // Thuộc tính Id kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public string Title { get; set; } = string.Empty; // Thuộc tính Title kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public string ImageUrl { get; set; } = string.Empty; // Thuộc tính ImageUrl kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public string? LinkUrl { get; set; } // Thuộc tính LinkUrl kiểu dữ liệu string? lưu trữ thông tin truyền tải.
        public int SortOrder { get; set; } // Thuộc tính SortOrder kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public bool IsActive { get; set; } // Thuộc tính IsActive kiểu dữ liệu bool lưu trữ thông tin truyền tải.
        public DateTime? StartDate { get; set; } // Thuộc tính StartDate kiểu dữ liệu DateTime? lưu trữ thông tin truyền tải.
        public DateTime? EndDate { get; set; } // Thuộc tính EndDate kiểu dữ liệu DateTime? lưu trữ thông tin truyền tải.
        public DateTime CreatedDate { get; set; } // Thuộc tính CreatedDate kiểu dữ liệu DateTime lưu trữ thông tin truyền tải.
    }
}

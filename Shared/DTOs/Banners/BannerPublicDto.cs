namespace PBL3.Shared.DTOs.Banners // Định nghĩa namespace PBL3.Shared.DTOs.Banners quản lý cấu trúc code truyền tải và validator.
{
    /// <summary>
    /// DTO rút gọn dùng cho Storefront (trang chủ) — không lộ field IsActive/audit.
    /// </summary>
    public class BannerPublicDto // Định nghĩa lớp DTO truyền tải dữ liệu BannerPublicDto.
    {
        public int Id { get; set; } // Thuộc tính Id kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public string Title { get; set; } = string.Empty; // Thuộc tính Title kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public string ImageUrl { get; set; } = string.Empty; // Thuộc tính ImageUrl kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public string? LinkUrl { get; set; } // Thuộc tính LinkUrl kiểu dữ liệu string? lưu trữ thông tin truyền tải.
    }
}

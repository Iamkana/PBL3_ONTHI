namespace PBL3.Shared.DTOs.Products // Định nghĩa namespace PBL3.Shared.DTOs.Products quản lý cấu trúc code truyền tải và validator.
{
    /// <summary>
    /// Request tạo ảnh cho Variant.
    /// </summary>
    public class SaveImageRequest // Định nghĩa lớp DTO truyền tải dữ liệu SaveImageRequest.
    {
        public string ImageUrl { get; set; } = string.Empty; // Thuộc tính ImageUrl kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public bool IsMain { get; set; } // Thuộc tính IsMain kiểu dữ liệu bool lưu trữ thông tin truyền tải.
        public int SortOrder { get; set; } // Thuộc tính SortOrder kiểu dữ liệu int lưu trữ thông tin truyền tải.
    }
}

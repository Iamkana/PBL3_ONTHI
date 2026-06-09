namespace PBL3.Shared.DTOs.Categories // Định nghĩa namespace PBL3.Shared.DTOs.Categories quản lý cấu trúc code truyền tải và validator.
{
    /// <summary>
    /// Request tạo mới danh mục.
    /// </summary>
    public class CreateCategoryRequest // Định nghĩa lớp DTO truyền tải dữ liệu CreateCategoryRequest.
    {
        public string Name { get; set; } = string.Empty; // Thuộc tính Name kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public string Slug { get; set; } = string.Empty; // Thuộc tính Slug kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public int? ParentId { get; set; } // Thuộc tính ParentId kiểu dữ liệu int? lưu trữ thông tin truyền tải.
        public string? ImageUrl { get; set; } // Thuộc tính ImageUrl kiểu dữ liệu string? lưu trữ thông tin truyền tải.
        public int SortOrder { get; set; } // Thuộc tính SortOrder kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public bool IsVisible { get; set; } = true; // Thuộc tính IsVisible kiểu dữ liệu bool lưu trữ thông tin truyền tải với giá trị mặc định là true.
    }
}

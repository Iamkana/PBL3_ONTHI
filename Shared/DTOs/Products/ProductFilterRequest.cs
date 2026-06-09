namespace PBL3.Shared.DTOs.Products // Định nghĩa namespace PBL3.Shared.DTOs.Products quản lý cấu trúc code truyền tải và validator.
{
    /// <summary>
    /// Bộ lọc cho danh sách sản phẩm.
    /// </summary>
    public class ProductFilterRequest // Định nghĩa lớp DTO truyền tải dữ liệu ProductFilterRequest.
    {
        public string? Keyword { get; set; } // Thuộc tính Keyword kiểu dữ liệu string? lưu trữ thông tin truyền tải.
        public int? CategoryId { get; set; } // Thuộc tính CategoryId kiểu dữ liệu int? lưu trữ thông tin truyền tải.
        public int? ManufacturerId { get; set; } // Thuộc tính ManufacturerId kiểu dữ liệu int? lưu trữ thông tin truyền tải.
        public decimal? PriceMin { get; set; } // Thuộc tính PriceMin kiểu dữ liệu decimal? lưu trữ thông tin truyền tải.
        public decimal? PriceMax { get; set; } // Thuộc tính PriceMax kiểu dữ liệu decimal? lưu trữ thông tin truyền tải.
        public ProductStatus? Status { get; set; } // Thuộc tính Status kiểu dữ liệu ProductStatus? lưu trữ thông tin truyền tải.
        public int PageNumber { get; set; } = 1; // Thuộc tính PageNumber kiểu dữ liệu int lưu trữ thông tin truyền tải với giá trị mặc định là 1.
        public int PageSize { get; set; } = 10; // Thuộc tính PageSize kiểu dữ liệu int lưu trữ thông tin truyền tải với giá trị mặc định là 10.
        public string? SortBy { get; set; } // Thuộc tính SortBy kiểu dữ liệu string? lưu trữ thông tin truyền tải.
        public bool SortDescending { get; set; } // Thuộc tính SortDescending kiểu dữ liệu bool lưu trữ thông tin truyền tải.
    }
}

namespace PBL3.Shared.DTOs.Products // Định nghĩa namespace PBL3.Shared.DTOs.Products quản lý cấu trúc code truyền tải và validator.
{
    /// <summary>
    /// DTO chi tiết một Variant (dùng trong ProductDetailDto).
    /// </summary>
    public class ProductVariantDto // Định nghĩa lớp DTO truyền tải dữ liệu ProductVariantDto.
    {
        public int Id { get; set; } // Thuộc tính Id kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public string SKU { get; set; } = string.Empty; // Thuộc tính SKU kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public string VariantName { get; set; } = string.Empty; // Thuộc tính VariantName kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public string Slug { get; set; } = string.Empty; // Thuộc tính Slug kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public decimal Price { get; set; } // Thuộc tính Price kiểu dữ liệu decimal lưu trữ thông tin truyền tải.
        public decimal? OriginalPrice { get; set; } // Thuộc tính OriginalPrice kiểu dữ liệu decimal? lưu trữ thông tin truyền tải.
        public int StockQuantity { get; set; } // Thuộc tính StockQuantity kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public int WarrantyMonth { get; set; } // Thuộc tính WarrantyMonth kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public Dictionary<string, string>? Specifications { get; set; } // Thực thi dòng lệnh nghiệp vụ.
        public List<ProductImageDto> Images { get; set; } = new(); // Thuộc tính Images kiểu dữ liệu List<ProductImageDto> lưu trữ thông tin truyền tải với giá trị mặc định là new().
    }
}

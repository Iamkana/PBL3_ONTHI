namespace PBL3.Shared.DTOs.Products // Định nghĩa namespace PBL3.Shared.DTOs.Products quản lý cấu trúc code truyền tải và validator.
{
    /// <summary>
    /// DTO chi tiết sản phẩm (dùng cho trang detail).
    /// </summary>
    public class ProductDetailDto // Định nghĩa lớp DTO truyền tải dữ liệu ProductDetailDto.
    {
        public int Id { get; set; } // Thuộc tính Id kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public string Name { get; set; } = string.Empty; // Thuộc tính Name kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public string Slug { get; set; } = string.Empty; // Thuộc tính Slug kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public string? ShortDescription { get; set; } // Thuộc tính ShortDescription kiểu dữ liệu string? lưu trữ thông tin truyền tải.
        public string? Description { get; set; } // Thuộc tính Description kiểu dữ liệu string? lưu trữ thông tin truyền tải.
        public int ManufacturerId { get; set; } // Thuộc tính ManufacturerId kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public string ManufacturerName { get; set; } = string.Empty; // Thuộc tính ManufacturerName kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public int CategoryId { get; set; } // Thuộc tính CategoryId kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public string CategoryName { get; set; } = string.Empty; // Thuộc tính CategoryName kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public ProductStatus Status { get; set; } // Thuộc tính Status kiểu dữ liệu ProductStatus lưu trữ thông tin truyền tải.
        public DateTime CreatedDate { get; set; } // Thuộc tính CreatedDate kiểu dữ liệu DateTime lưu trữ thông tin truyền tải.
        public List<ProductVariantDto> Variants { get; set; } = new(); // Thuộc tính Variants kiểu dữ liệu List<ProductVariantDto> lưu trữ thông tin truyền tải với giá trị mặc định là new().
    }
}

namespace PBL3.Shared.DTOs.Products // Định nghĩa namespace PBL3.Shared.DTOs.Products quản lý cấu trúc code truyền tải và validator.
{
    /// <summary>
    /// DTO dùng cho trang danh sách sản phẩm (nhẹ hơn DetailDto).
    /// </summary>
    public class ProductListDto // Định nghĩa lớp DTO truyền tải dữ liệu ProductListDto.
    {
        public int Id { get; set; } // Thuộc tính Id kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public string Name { get; set; } = string.Empty; // Thuộc tính Name kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public string? ShortDescription { get; set; } // Thuộc tính ShortDescription kiểu dữ liệu string? lưu trữ thông tin truyền tải.
        public string ManufacturerName { get; set; } = string.Empty; // Thuộc tính ManufacturerName kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public string CategoryName { get; set; } = string.Empty; // Thuộc tính CategoryName kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public ProductStatus Status { get; set; } // Thuộc tính Status kiểu dữ liệu ProductStatus lưu trữ thông tin truyền tải.

        /// <summary>
        /// Giá thấp nhất trong tất cả Variants.
        /// </summary>
        public decimal MinPrice { get; set; } // Thuộc tính MinPrice kiểu dữ liệu decimal lưu trữ thông tin truyền tải.

        /// <summary>
        /// Giá cao nhất trong tất cả Variants.
        /// </summary>
        public decimal MaxPrice { get; set; } // Thuộc tính MaxPrice kiểu dữ liệu decimal lưu trữ thông tin truyền tải.

        /// <summary>
        /// Chuỗi khoảng giá hiển thị (VD: "20.000.000đ - 25.000.000đ").
        /// </summary>
        public string PriceRange { get; set; } = string.Empty; // Thuộc tính PriceRange kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.

        /// <summary>
        /// Ảnh đại diện (ảnh chính của Variant đầu tiên).
        /// </summary>
        public string? ThumbnailUrl { get; set; } // Thuộc tính ThumbnailUrl kiểu dữ liệu string? lưu trữ thông tin truyền tải.

        /// <summary>
        /// Tổng tồn kho (tạm = 0, chờ module Inventory).
        /// </summary>
        public int TotalStock { get; set; } // Thuộc tính TotalStock kiểu dữ liệu int lưu trữ thông tin truyền tải.

        /// <summary>
        /// Số lượng biến thể.
        /// </summary>
        public int VariantCount { get; set; } // Thuộc tính VariantCount kiểu dữ liệu int lưu trữ thông tin truyền tải.

        public DateTime CreatedDate { get; set; } // Thuộc tính CreatedDate kiểu dữ liệu DateTime lưu trữ thông tin truyền tải.
    }
}

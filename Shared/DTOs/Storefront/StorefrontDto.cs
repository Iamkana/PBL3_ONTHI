using PBL3.Shared.DTOs.Common; // Nhập (import) namespace PBL3.Shared.DTOs.Common để sử dụng các thành phần bên trong.

namespace PBL3.Shared.DTOs.Storefront // Định nghĩa namespace PBL3.Shared.DTOs.Storefront quản lý cấu trúc code truyền tải và validator.
{
    public class CategoryMenuResponse // Định nghĩa lớp DTO truyền tải dữ liệu CategoryMenuResponse.
    {
        public int Id { get; set; } // Thuộc tính Id kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public string Name { get; set; } = string.Empty; // Thuộc tính Name kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public string Slug { get; set; } = string.Empty; // Thuộc tính Slug kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public string? IconUrl { get; set; } // Thuộc tính IconUrl kiểu dữ liệu string? lưu trữ thông tin truyền tải.
        public int Level { get; set; } // Thuộc tính Level kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public int? ParentId { get; set; } // Thuộc tính ParentId kiểu dữ liệu int? lưu trữ thông tin truyền tải.
    }

    /// <summary>
    /// Thông tin chi tiết danh mục dùng cho breadcrumb và tiêu đề trang.
    /// </summary>
    public class CategoryDetailResponse // Định nghĩa lớp DTO truyền tải dữ liệu CategoryDetailResponse.
    {
        public int Id { get; set; } // Thuộc tính Id kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public string Name { get; set; } = string.Empty; // Thuộc tính Name kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public string Slug { get; set; } = string.Empty; // Thuộc tính Slug kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public string? IconUrl { get; set; } // Thuộc tính IconUrl kiểu dữ liệu string? lưu trữ thông tin truyền tải.
    }

    /// <summary>
    /// Query parameters cho API lấy danh sách sản phẩm theo danh mục.
    /// </summary>
    public class CategoryProductsRequest // Định nghĩa lớp DTO truyền tải dữ liệu CategoryProductsRequest.
    {
        public int Page { get; set; } = 1; // Thuộc tính Page kiểu dữ liệu int lưu trữ thông tin truyền tải với giá trị mặc định là 1.
        public int PageSize { get; set; } = 20; // Thuộc tính PageSize kiểu dữ liệu int lưu trữ thông tin truyền tải với giá trị mặc định là 20.
    }

    public class ProductCardResponse // Định nghĩa lớp DTO truyền tải dữ liệu ProductCardResponse.
    {
        public int Id { get; set; } // Thuộc tính Id kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public string Name { get; set; } = string.Empty; // Thuộc tính Name kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public string Slug { get; set; } = string.Empty; // Thuộc tính Slug kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public string? ThumbnailUrl { get; set; } // Thuộc tính ThumbnailUrl kiểu dữ liệu string? lưu trữ thông tin truyền tải.
        public string ManufacturerName { get; set; } = string.Empty; // Thuộc tính ManufacturerName kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public int CategoryId { get; set; } // Thuộc tính CategoryId kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public string CategoryName { get; set; } = string.Empty; // Thuộc tính CategoryName kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.

        public decimal OldPrice { get; set; } // Thuộc tính OldPrice kiểu dữ liệu decimal lưu trữ thông tin truyền tải.
        public decimal CurrentPrice { get; set; } // Thuộc tính CurrentPrice kiểu dữ liệu decimal lưu trữ thông tin truyền tải.
        public int DiscountPercent { get; set; } // Thuộc tính DiscountPercent kiểu dữ liệu int lưu trữ thông tin truyền tải.

        public bool IsAvailable { get; set; } // Thuộc tính IsAvailable kiểu dữ liệu bool lưu trữ thông tin truyền tải.

        public double Rating { get; set; } = 5.0; // Thuộc tính Rating kiểu dữ liệu double lưu trữ thông tin truyền tải với giá trị mặc định là 5.0.
        public int ReviewCount { get; set; } = 0; // Thuộc tính ReviewCount kiểu dữ liệu int lưu trữ thông tin truyền tải với giá trị mặc định là 0.
    }

    /// <summary>
    /// Thông tin phân loại sản phẩm cho trang chi tiết.
    public class StorefrontVariantResponse // Định nghĩa lớp DTO truyền tải dữ liệu StorefrontVariantResponse.
    {
        public int Id { get; set; } // Thuộc tính Id kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public string VariantName { get; set; } = string.Empty; // Thuộc tính VariantName kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public decimal Price { get; set; } // Thuộc tính Price kiểu dữ liệu decimal lưu trữ thông tin truyền tải.
        public decimal? OriginalPrice { get; set; } // Thuộc tính OriginalPrice kiểu dữ liệu decimal? lưu trữ thông tin truyền tải.
        public bool IsAvailable { get; set; } // Thuộc tính IsAvailable kiểu dữ liệu bool lưu trữ thông tin truyền tải.
        public int StockQuantity { get; set; } // Thuộc tính StockQuantity kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public Dictionary<string, string> Specifications { get; set; } = new(); // Thực thi dòng lệnh nghiệp vụ.
    }

    /// <summary>
    /// Toàn bộ dữ liệu chi tiết sản phẩm cho Storefront.
    /// </summary>
    public class ProductDetailResponse // Định nghĩa lớp DTO truyền tải dữ liệu ProductDetailResponse.
    {
        public int Id { get; set; } // Thuộc tính Id kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public string Name { get; set; } = string.Empty; // Thuộc tính Name kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public string Slug { get; set; } = string.Empty; // Thuộc tính Slug kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public int CategoryId { get; set; } // Thuộc tính CategoryId kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public string CategoryName { get; set; } = string.Empty; // Thuộc tính CategoryName kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.

        /// <summary>Thương hiệu (VD: "MSI")</summary>
        public string ManufacturerName { get; set; } = string.Empty; // Thuộc tính ManufacturerName kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.

        /// <summary>Bài viết HTML mô tả chi tiết sản phẩm.</summary>
        public string? Description { get; set; } // Thuộc tính Description kiểu dữ liệu string? lưu trữ thông tin truyền tải.

        /// <summary>Chuỗi JSON thông số kỹ thuật.</summary>
        public string? Specifications { get; set; } // Thuộc tính Specifications kiểu dữ liệu string? lưu trữ thông tin truyền tải.

        /// <summary>Danh sách URL ảnh để làm Gallery.</summary>
        public List<string> Images { get; set; } = new(); // Thuộc tính Images kiểu dữ liệu List<string> lưu trữ thông tin truyền tải với giá trị mặc định là new().

        /// <summary>Danh sách phân loại (BẮT BUỘC CÓ).</summary>
        public List<StorefrontVariantResponse> Variants { get; set; } = new(); // Thuộc tính Variants kiểu dữ liệu List<StorefrontVariantResponse> lưu trữ thông tin truyền tải với giá trị mặc định là new().

        /// <summary>Các gạch đầu dòng mô tả ngắn, parse từ Specifications.</summary>
        public List<string> ShortFeatures { get; set; } = new(); // Thuộc tính ShortFeatures kiểu dữ liệu List<string> lưu trữ thông tin truyền tải với giá trị mặc định là new().

        public double Rating { get; set; } = 5.0; // Thuộc tính Rating kiểu dữ liệu double lưu trữ thông tin truyền tải với giá trị mặc định là 5.0.
        public int ReviewCount { get; set; } = 13; // Thuộc tính ReviewCount kiểu dữ liệu int lưu trữ thông tin truyền tải với giá trị mặc định là 13.
    }
}

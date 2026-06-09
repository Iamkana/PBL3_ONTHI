namespace PBL3.Shared.DTOs.Products // Định nghĩa namespace PBL3.Shared.DTOs.Products quản lý cấu trúc code truyền tải và validator.
{
    /// <summary>
    /// Request tạo sản phẩm mới (bao gồm Variants lồng nhau).
    /// </summary>
    public class CreateProductRequest // Định nghĩa lớp DTO truyền tải dữ liệu CreateProductRequest.
    {
        public string Name { get; set; } = string.Empty; // Thuộc tính Name kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public string? ShortDescription { get; set; } // Thuộc tính ShortDescription kiểu dữ liệu string? lưu trữ thông tin truyền tải.
        public string? Description { get; set; } // Thuộc tính Description kiểu dữ liệu string? lưu trữ thông tin truyền tải.
        public int ManufacturerId { get; set; } // Thuộc tính ManufacturerId kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public int CategoryId { get; set; } // Thuộc tính CategoryId kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public List<CreateVariantRequest> Variants { get; set; } = new(); // Thuộc tính Variants kiểu dữ liệu List<CreateVariantRequest> lưu trữ thông tin truyền tải với giá trị mặc định là new().
    }
}

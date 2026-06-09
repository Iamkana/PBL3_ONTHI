namespace PBL3.Shared.DTOs.Products // Định nghĩa namespace PBL3.Shared.DTOs.Products quản lý cấu trúc code truyền tải và validator.
{
    /// <summary>
    /// Request thêm/sửa Variant trong Product đã tồn tại.
    /// Nếu Id == null → Thêm mới. Nếu Id != null → Cập nhật.
    /// </summary>
    public class SaveVariantRequest // Định nghĩa lớp DTO truyền tải dữ liệu SaveVariantRequest.
    {
        public int? Id { get; set; } // Thuộc tính Id kiểu dữ liệu int? lưu trữ thông tin truyền tải.
        public string SKU { get; set; } = string.Empty; // Thuộc tính SKU kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public string VariantName { get; set; } = string.Empty; // Thuộc tính VariantName kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public decimal Price { get; set; } // Thuộc tính Price kiểu dữ liệu decimal lưu trữ thông tin truyền tải.
        public decimal? OriginalPrice { get; set; } // Thuộc tính OriginalPrice kiểu dữ liệu decimal? lưu trữ thông tin truyền tải.
        public int WarrantyMonth { get; set; } = 12; // Thuộc tính WarrantyMonth kiểu dữ liệu int lưu trữ thông tin truyền tải với giá trị mặc định là 12.
        public Dictionary<string, string>? Specifications { get; set; } // Thực thi dòng lệnh nghiệp vụ.
        public List<SaveImageRequest> Images { get; set; } = new(); // Thuộc tính Images kiểu dữ liệu List<SaveImageRequest> lưu trữ thông tin truyền tải với giá trị mặc định là new().
    }
}

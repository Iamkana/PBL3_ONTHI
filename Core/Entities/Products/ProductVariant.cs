using System.ComponentModel.DataAnnotations; // Nhập thư viện hỗ trợ kiểm tra ràng buộc dữ liệu (DataAnnotations).
using System.ComponentModel.DataAnnotations.Schema; // Nhập thư viện hỗ trợ cấu hình ánh xạ thực thể (Schema).

namespace PBL3.Core.Entities // Định nghĩa namespace PBL3.Core.Entities để quản lý thực thể/giao diện.
{
    [Table("ProductVariants")] // Ánh xạ thực thể hiện tại với bảng dữ liệu 'ProductVariants' trong cơ sở dữ liệu.
    public class ProductVariant // Định nghĩa thực thể/lớp nghiệp vụ ProductVariant.
    {
        [Key] // Đánh dấu thuộc tính ngay phía dưới là Khóa chính (Primary Key).
        public int Id { get; set; } // Thuộc tính Id kiểu int đóng vai trò khóa chính định danh bản ghi.
        public int ProductId { get; set; } // Thuộc tính khóa ngoại ProductId kiểu int liên kết sang thực thể liên quan.

        [Required] // Đánh dấu thuộc tính phía dưới là bắt buộc nhập (NOT NULL).
        [MaxLength(50)] // Ràng buộc độ dài ký tự tối đa của cột trong CSDL là 50 ký tự.
        public string SKU { get; set; } = string.Empty; // Thuộc tính SKU kiểu dữ liệu string lưu trữ thông tin thực thể với giá trị mặc định là string.Empty.
        [Required] // Đánh dấu thuộc tính phía dưới là bắt buộc nhập (NOT NULL).
        [MaxLength(200)] // Ràng buộc độ dài ký tự tối đa của cột trong CSDL là 200 ký tự.
        public string VariantName { get; set; } = string.Empty; // Thuộc tính VariantName kiểu dữ liệu string lưu trữ thông tin thực thể với giá trị mặc định là string.Empty.
        [Required] // Đánh dấu thuộc tính phía dưới là bắt buộc nhập (NOT NULL).
        [MaxLength(250)] // Ràng buộc độ dài ký tự tối đa của cột trong CSDL là 250 ký tự.
        public string Slug { get; set; } = string.Empty; // Thuộc tính Slug kiểu dữ liệu string lưu trữ thông tin thực thể với giá trị mặc định là string.Empty.

        public decimal Price { get; set; } // Thuộc tính Price kiểu dữ liệu decimal lưu trữ thông tin thực thể.
        public decimal? OriginalPrice { get; set; } // Thuộc tính OriginalPrice kiểu dữ liệu decimal? lưu trữ thông tin thực thể.
        public int WarrantyMonth { get; set; } // Thuộc tính WarrantyMonth kiểu dữ liệu int lưu trữ thông tin thực thể.
        public Dictionary<string, string> Specifications { get; set; } = new(); // Khai báo thành phần cấu trúc nghiệp vụ.

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow; // Thuộc tính CreatedDate lưu thời điểm bản ghi được tạo lập ban đầu.
        [MaxLength(100)] // Ràng buộc độ dài ký tự tối đa của cột trong CSDL là 100 ký tự.
        public string? CreatedBy { get; set; } // Thuộc tính CreatedBy lưu người dùng đã khởi tạo bản ghi này.
        public DateTime? ModifiedDate { get; set; } // Thuộc tính ModifiedDate lưu thời điểm bản ghi được cập nhật gần nhất.
        [MaxLength(100)] // Ràng buộc độ dài ký tự tối đa của cột trong CSDL là 100 ký tự.
        public string? ModifiedBy { get; set; } // Thuộc tính ModifiedBy lưu người dùng đã cập nhật bản ghi này sau cùng.
        public bool IsDeleted { get; set; } // Thuộc tính IsDeleted đánh dấu bản ghi đã bị xóa mềm (soft deleted).
        public DateTime? DeletedDate { get; set; } // Thuộc tính DeletedDate lưu thời điểm bản ghi bị xóa mềm khỏi hệ thống.

        [ForeignKey("ProductId")] // Chỉ định thuộc tính điều hướng tương ứng sử dụng khóa ngoại 'ProductId'.
        public virtual Product Product { get; set; } = null!; // Thuộc tính quan hệ điều hướng ảo (virtual navigation property) tham chiếu tới thực thể Product.
        public virtual ICollection<ProductImage> Images { get; set; } = new List<ProductImage>(); // Thuộc tính quan hệ điều hướng (navigation collection) danh sách các ProductImage liên quan.
        public virtual ICollection<ProductSerial> Serials { get; set; } = new List<ProductSerial>(); // Thuộc tính quan hệ điều hướng (navigation collection) danh sách các ProductSerial liên quan.

        /// <summary>
        /// Số lượng tồn kho — cột vật lý, được đồng bộ bởi InventorySyncService.
        /// KHÔNG tự đếm on-the-fly. Luồng Read chỉ đọc giá trị này.
        /// </summary>
        public int StockQuantity { get; set; } // Thuộc tính StockQuantity kiểu dữ liệu int lưu trữ thông tin thực thể.
    }
}

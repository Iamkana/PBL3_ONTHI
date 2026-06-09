using System.ComponentModel.DataAnnotations; // Nhập thư viện hỗ trợ kiểm tra ràng buộc dữ liệu (DataAnnotations).
using System.ComponentModel.DataAnnotations.Schema; // Nhập thư viện hỗ trợ cấu hình ánh xạ thực thể (Schema).

namespace PBL3.Core.Entities // Định nghĩa namespace PBL3.Core.Entities để quản lý thực thể/giao diện.
{
    [Table("ProductImages")] // Ánh xạ thực thể hiện tại với bảng dữ liệu 'ProductImages' trong cơ sở dữ liệu.
    public class ProductImage // Định nghĩa thực thể/lớp nghiệp vụ ProductImage.
    {
        [Key] // Đánh dấu thuộc tính ngay phía dưới là Khóa chính (Primary Key).
        public int Id { get; set; } // Thuộc tính Id kiểu int đóng vai trò khóa chính định danh bản ghi.
        public int VariantId { get; set; } // Thuộc tính khóa ngoại VariantId kiểu int liên kết sang thực thể liên quan.

        [Required] // Đánh dấu thuộc tính phía dưới là bắt buộc nhập (NOT NULL).
        [MaxLength(500)] // Ràng buộc độ dài ký tự tối đa của cột trong CSDL là 500 ký tự.
        public string ImageUrl { get; set; } = string.Empty; // Thuộc tính ImageUrl kiểu dữ liệu string lưu trữ thông tin thực thể với giá trị mặc định là string.Empty.
        public bool IsMain { get; set; } // Thuộc tính IsMain kiểu dữ liệu bool lưu trữ thông tin thực thể.
        public int SortOrder { get; set; } // Thuộc tính SortOrder kiểu dữ liệu int lưu trữ thông tin thực thể.

        [ForeignKey("VariantId")] // Chỉ định thuộc tính điều hướng tương ứng sử dụng khóa ngoại 'VariantId'.
        public virtual ProductVariant Variant { get; set; } = null!; // Thuộc tính quan hệ điều hướng ảo (virtual navigation property) tham chiếu tới thực thể ProductVariant.
    }
}

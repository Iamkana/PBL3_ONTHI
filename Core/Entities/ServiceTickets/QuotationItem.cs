using System.ComponentModel.DataAnnotations; // Nhập thư viện hỗ trợ kiểm tra ràng buộc dữ liệu (DataAnnotations).
using System.ComponentModel.DataAnnotations.Schema; // Nhập thư viện hỗ trợ cấu hình ánh xạ thực thể (Schema).

namespace PBL3.Core.Entities // Định nghĩa namespace PBL3.Core.Entities để quản lý thực thể/giao diện.
{
    [Table("QuotationItems")] // Ánh xạ thực thể hiện tại với bảng dữ liệu 'QuotationItems' trong cơ sở dữ liệu.
    public class QuotationItem // Định nghĩa thực thể/lớp nghiệp vụ QuotationItem.
    {
        [Key] // Đánh dấu thuộc tính ngay phía dưới là Khóa chính (Primary Key).
        public int Id { get; set; } // Thuộc tính Id kiểu int đóng vai trò khóa chính định danh bản ghi.

        public int QuotationId { get; set; } // Thuộc tính khóa ngoại QuotationId kiểu int liên kết sang thực thể liên quan.
        public int? VariantId { get; set; } // Thuộc tính khóa ngoại VariantId kiểu int? liên kết sang thực thể liên quan.

        [Required] // Đánh dấu thuộc tính phía dưới là bắt buộc nhập (NOT NULL).
        [MaxLength(200)] // Ràng buộc độ dài ký tự tối đa của cột trong CSDL là 200 ký tự.
        public string Description { get; set; } = string.Empty; // Thuộc tính Description kiểu dữ liệu string lưu trữ thông tin thực thể với giá trị mặc định là string.Empty.

        public int Quantity { get; set; } = 1; // Thuộc tính Quantity kiểu dữ liệu int lưu trữ thông tin thực thể với giá trị mặc định là 1.
        public decimal UnitPrice { get; set; } // Thuộc tính UnitPrice kiểu dữ liệu decimal lưu trữ thông tin thực thể.
        public decimal LineTotal { get; set; } // Thuộc tính LineTotal kiểu dữ liệu decimal lưu trữ thông tin thực thể.

        [ForeignKey("QuotationId")] // Chỉ định thuộc tính điều hướng tương ứng sử dụng khóa ngoại 'QuotationId'.
        public virtual Quotation Quotation { get; set; } = null!; // Thuộc tính quan hệ điều hướng ảo (virtual navigation property) tham chiếu tới thực thể Quotation.

        [ForeignKey("VariantId")] // Chỉ định thuộc tính điều hướng tương ứng sử dụng khóa ngoại 'VariantId'.
        public virtual ProductVariant? Variant { get; set; } // Thuộc tính quan hệ điều hướng ảo (virtual navigation property) tham chiếu tới thực thể ProductVariant?.
    }
}

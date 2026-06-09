using System.ComponentModel.DataAnnotations; // Nhập thư viện hỗ trợ kiểm tra ràng buộc dữ liệu (DataAnnotations).
using System.ComponentModel.DataAnnotations.Schema; // Nhập thư viện hỗ trợ cấu hình ánh xạ thực thể (Schema).

namespace PBL3.Core.Entities // Định nghĩa namespace PBL3.Core.Entities để quản lý thực thể/giao diện.
{
    // 3. ImportReceiptDetails
    [Table("ImportReceiptDetails")] // Ánh xạ thực thể hiện tại với bảng dữ liệu 'ImportReceiptDetails' trong cơ sở dữ liệu.
    public class ImportReceiptDetail // Định nghĩa thực thể/lớp nghiệp vụ ImportReceiptDetail.
    {
        [Key] // Đánh dấu thuộc tính ngay phía dưới là Khóa chính (Primary Key).
        public int Id { get; set; } // Thuộc tính Id kiểu int đóng vai trò khóa chính định danh bản ghi.
        public int ReceiptId { get; set; } // Thuộc tính khóa ngoại ReceiptId kiểu int liên kết sang thực thể liên quan.
        public int VariantId { get; set; } // Thuộc tính khóa ngoại VariantId kiểu int liên kết sang thực thể liên quan.

        public int Quantity { get; set; } // Thuộc tính Quantity kiểu dữ liệu int lưu trữ thông tin thực thể.
        public decimal ImportPrice { get; set; } // Thuộc tính ImportPrice kiểu dữ liệu decimal lưu trữ thông tin thực thể.

        [ForeignKey("ReceiptId")] // Chỉ định thuộc tính điều hướng tương ứng sử dụng khóa ngoại 'ReceiptId'.
        public virtual ImportReceipt Receipt { get; set; } = null!; // Thuộc tính quan hệ điều hướng ảo (virtual navigation property) tham chiếu tới thực thể ImportReceipt.
        [ForeignKey("VariantId")] // Chỉ định thuộc tính điều hướng tương ứng sử dụng khóa ngoại 'VariantId'.
        public virtual ProductVariant Variant { get; set; } = null!; // Thuộc tính quan hệ điều hướng ảo (virtual navigation property) tham chiếu tới thực thể ProductVariant.
    }
}

using System.ComponentModel.DataAnnotations; // Nhập thư viện hỗ trợ kiểm tra ràng buộc dữ liệu (DataAnnotations).
using System.ComponentModel.DataAnnotations.Schema; // Nhập thư viện hỗ trợ cấu hình ánh xạ thực thể (Schema).

namespace PBL3.Core.Entities // Định nghĩa namespace PBL3.Core.Entities để quản lý thực thể/giao diện.
{
    // 4. ProductSerials
    [Table("ProductSerials")] // Ánh xạ thực thể hiện tại với bảng dữ liệu 'ProductSerials' trong cơ sở dữ liệu.
    public class ProductSerial // Định nghĩa thực thể/lớp nghiệp vụ ProductSerial.
    {
        [Key] // Đánh dấu thuộc tính ngay phía dưới là Khóa chính (Primary Key).
        public int Id { get; set; } // Thuộc tính Id kiểu int đóng vai trò khóa chính định danh bản ghi.
        [Required] // Đánh dấu thuộc tính phía dưới là bắt buộc nhập (NOT NULL).
        [MaxLength(100)] // Ràng buộc độ dài ký tự tối đa của cột trong CSDL là 100 ký tự.
        public string SerialNumber { get; set; } = string.Empty; // Thuộc tính SerialNumber kiểu dữ liệu string lưu trữ thông tin thực thể với giá trị mặc định là string.Empty.

        public int VariantId { get; set; } // Thuộc tính khóa ngoại VariantId kiểu int liên kết sang thực thể liên quan.
        public int ImportReceiptId { get; set; } // Thuộc tính khóa ngoại ImportReceiptId kiểu int liên kết sang thực thể liên quan.

        // 0: Available, 1: Reserved, 2: Sold, 3: Defective, 4: Returned, 5: Lost
        public byte Status { get; set; } // Thuộc tính Status kiểu dữ liệu byte lưu trữ thông tin thực thể.

        public int? OrderId { get; set; } // Thuộc tính khóa ngoại OrderId kiểu int? liên kết sang thực thể liên quan.

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow; // Thuộc tính CreatedDate lưu thời điểm bản ghi được tạo lập ban đầu.
        public DateTime? SoldDate { get; set; } // Thuộc tính SoldDate kiểu dữ liệu DateTime? lưu trữ thông tin thực thể.

        [ForeignKey("VariantId")] // Chỉ định thuộc tính điều hướng tương ứng sử dụng khóa ngoại 'VariantId'.
        public virtual ProductVariant Variant { get; set; } = null!; // Thuộc tính quan hệ điều hướng ảo (virtual navigation property) tham chiếu tới thực thể ProductVariant.
        [ForeignKey("ImportReceiptId")] // Chỉ định thuộc tính điều hướng tương ứng sử dụng khóa ngoại 'ImportReceiptId'.
        public virtual ImportReceipt ImportReceipt { get; set; } = null!; // Thuộc tính quan hệ điều hướng ảo (virtual navigation property) tham chiếu tới thực thể ImportReceipt.
    }
}

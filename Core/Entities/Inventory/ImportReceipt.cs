using System.ComponentModel.DataAnnotations; // Nhập thư viện hỗ trợ kiểm tra ràng buộc dữ liệu (DataAnnotations).
using System.ComponentModel.DataAnnotations.Schema; // Nhập thư viện hỗ trợ cấu hình ánh xạ thực thể (Schema).

namespace PBL3.Core.Entities // Định nghĩa namespace PBL3.Core.Entities để quản lý thực thể/giao diện.
{
    // 2. ImportReceipts
    [Table("ImportReceipts")] // Ánh xạ thực thể hiện tại với bảng dữ liệu 'ImportReceipts' trong cơ sở dữ liệu.
    public class ImportReceipt // Định nghĩa thực thể/lớp nghiệp vụ ImportReceipt.
    {
        [Key] // Đánh dấu thuộc tính ngay phía dưới là Khóa chính (Primary Key).
        public int Id { get; set; } // Thuộc tính Id kiểu int đóng vai trò khóa chính định danh bản ghi.
        [Required] // Đánh dấu thuộc tính phía dưới là bắt buộc nhập (NOT NULL).
        [MaxLength(20)] // Ràng buộc độ dài ký tự tối đa của cột trong CSDL là 20 ký tự.
        public string ReceiptCode { get; set; } = string.Empty; // Thuộc tính ReceiptCode kiểu dữ liệu string lưu trữ thông tin thực thể với giá trị mặc định là string.Empty.

        public int SupplierId { get; set; } // Thuộc tính khóa ngoại SupplierId kiểu int liên kết sang thực thể liên quan.
        public Guid EmployeeId { get; set; } // Thuộc tính khóa ngoại EmployeeId kiểu Guid liên kết sang thực thể liên quan.

        public DateTime ImportDate { get; set; } = DateTime.UtcNow; // Thuộc tính ImportDate kiểu dữ liệu DateTime lưu trữ thông tin thực thể với giá trị mặc định là DateTime.UtcNow.
        public decimal TotalAmount { get; set; } // Thuộc tính TotalAmount kiểu dữ liệu decimal lưu trữ thông tin thực thể.
        [MaxLength(500)] // Ràng buộc độ dài ký tự tối đa của cột trong CSDL là 500 ký tự.
        public string? Note { get; set; } // Thuộc tính Note kiểu dữ liệu string? lưu trữ thông tin thực thể.
        public bool IsDeleted { get; set; } // Thuộc tính IsDeleted đánh dấu bản ghi đã bị xóa mềm (soft deleted).

        [ForeignKey("SupplierId")] // Chỉ định thuộc tính điều hướng tương ứng sử dụng khóa ngoại 'SupplierId'.
        public virtual Supplier Supplier { get; set; } = null!; // Thuộc tính quan hệ điều hướng ảo (virtual navigation property) tham chiếu tới thực thể Supplier.

        public virtual ICollection<ImportReceiptDetail> Details { get; set; } = new List<ImportReceiptDetail>(); // Thuộc tính quan hệ điều hướng (navigation collection) danh sách các ImportReceiptDetail liên quan.
    }
}

using System.ComponentModel.DataAnnotations; // Nhập thư viện hỗ trợ kiểm tra ràng buộc dữ liệu (DataAnnotations).
using System.ComponentModel.DataAnnotations.Schema; // Nhập thư viện hỗ trợ cấu hình ánh xạ thực thể (Schema).

namespace PBL3.Core.Entities // Định nghĩa namespace PBL3.Core.Entities để quản lý thực thể/giao diện.
{
    // 1. Suppliers
    [Table("Suppliers")] // Ánh xạ thực thể hiện tại với bảng dữ liệu 'Suppliers' trong cơ sở dữ liệu.
    public class Supplier // Định nghĩa thực thể/lớp nghiệp vụ Supplier.
    {
        [Key] // Đánh dấu thuộc tính ngay phía dưới là Khóa chính (Primary Key).
        public int Id { get; set; } // Thuộc tính Id kiểu int đóng vai trò khóa chính định danh bản ghi.
        [Required] // Đánh dấu thuộc tính phía dưới là bắt buộc nhập (NOT NULL).
        [MaxLength(200)] // Ràng buộc độ dài ký tự tối đa của cột trong CSDL là 200 ký tự.
        public string Name { get; set; } = string.Empty; // Thuộc tính Name kiểu dữ liệu string lưu trữ thông tin thực thể với giá trị mặc định là string.Empty.
        [MaxLength(100)] // Ràng buộc độ dài ký tự tối đa của cột trong CSDL là 100 ký tự.
        public string? ContactPerson { get; set; } // Thuộc tính ContactPerson kiểu dữ liệu string? lưu trữ thông tin thực thể.
        [Required] // Đánh dấu thuộc tính phía dưới là bắt buộc nhập (NOT NULL).
        [MaxLength(20)] // Ràng buộc độ dài ký tự tối đa của cột trong CSDL là 20 ký tự.
        public string PhoneNumber { get; set; } = string.Empty; // Thuộc tính PhoneNumber kiểu dữ liệu string lưu trữ thông tin thực thể với giá trị mặc định là string.Empty.
        [MaxLength(100)] // Ràng buộc độ dài ký tự tối đa của cột trong CSDL là 100 ký tự.
        public string? Email { get; set; } // Thuộc tính Email kiểu dữ liệu string? lưu trữ thông tin thực thể.
        [MaxLength(255)] // Ràng buộc độ dài ký tự tối đa của cột trong CSDL là 255 ký tự.
        public string? Address { get; set; } // Thuộc tính Address kiểu dữ liệu string? lưu trữ thông tin thực thể.
        [MaxLength(20)] // Ràng buộc độ dài ký tự tối đa của cột trong CSDL là 20 ký tự.
        public string? TaxCode { get; set; } // Thuộc tính TaxCode kiểu dữ liệu string? lưu trữ thông tin thực thể.

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow; // Thuộc tính CreatedDate lưu thời điểm bản ghi được tạo lập ban đầu.
        public bool IsDeleted { get; set; } // Thuộc tính IsDeleted đánh dấu bản ghi đã bị xóa mềm (soft deleted).

        public virtual ICollection<ImportReceipt> ImportReceipts { get; set; } = new List<ImportReceipt>(); // Thuộc tính quan hệ điều hướng (navigation collection) danh sách các ImportReceipt liên quan.
    }
}

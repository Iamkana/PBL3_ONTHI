using System.ComponentModel.DataAnnotations; // Nhập thư viện hỗ trợ kiểm tra ràng buộc dữ liệu (DataAnnotations).
using System.ComponentModel.DataAnnotations.Schema; // Nhập thư viện hỗ trợ cấu hình ánh xạ thực thể (Schema).

namespace PBL3.Core.Entities // Định nghĩa namespace PBL3.Core.Entities để quản lý thực thể/giao diện.
{
    [Table("Manufacturers")] // Ánh xạ thực thể hiện tại với bảng dữ liệu 'Manufacturers' trong cơ sở dữ liệu.
    public class Manufacturer // Định nghĩa thực thể/lớp nghiệp vụ Manufacturer.
    {
        [Key] // Đánh dấu thuộc tính ngay phía dưới là Khóa chính (Primary Key).
        public int Id { get; set; } // Thuộc tính Id kiểu int đóng vai trò khóa chính định danh bản ghi.
        [Required] // Đánh dấu thuộc tính phía dưới là bắt buộc nhập (NOT NULL).
        [MaxLength(100)] // Ràng buộc độ dài ký tự tối đa của cột trong CSDL là 100 ký tự.
        public string Name { get; set; } = string.Empty; // Thuộc tính Name kiểu dữ liệu string lưu trữ thông tin thực thể với giá trị mặc định là string.Empty.
        [MaxLength(500)] // Ràng buộc độ dài ký tự tối đa của cột trong CSDL là 500 ký tự.
        public string? LogoUrl { get; set; } // Thuộc tính LogoUrl kiểu dữ liệu string? lưu trữ thông tin thực thể.
        [MaxLength(255)] // Ràng buộc độ dài ký tự tối đa của cột trong CSDL là 255 ký tự.
        public string? Website { get; set; } // Thuộc tính Website kiểu dữ liệu string? lưu trữ thông tin thực thể.
        [MaxLength(100)] // Ràng buộc độ dài ký tự tối đa của cột trong CSDL là 100 ký tự.
        public string? SupportEmail { get; set; } // Thuộc tính SupportEmail kiểu dữ liệu string? lưu trữ thông tin thực thể.

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow; // Thuộc tính CreatedDate lưu thời điểm bản ghi được tạo lập ban đầu.
        [MaxLength(100)] // Ràng buộc độ dài ký tự tối đa của cột trong CSDL là 100 ký tự.
        public string? CreatedBy { get; set; } // Thuộc tính CreatedBy lưu người dùng đã khởi tạo bản ghi này.
        public DateTime? ModifiedDate { get; set; } // Thuộc tính ModifiedDate lưu thời điểm bản ghi được cập nhật gần nhất.
        [MaxLength(100)] // Ràng buộc độ dài ký tự tối đa của cột trong CSDL là 100 ký tự.
        public string? ModifiedBy { get; set; } // Thuộc tính ModifiedBy lưu người dùng đã cập nhật bản ghi này sau cùng.
        public bool IsDeleted { get; set; } // Thuộc tính IsDeleted đánh dấu bản ghi đã bị xóa mềm (soft deleted).
        public DateTime? DeletedDate { get; set; } // Thuộc tính DeletedDate lưu thời điểm bản ghi bị xóa mềm khỏi hệ thống.

        public virtual ICollection<Product> Products { get; set; } = new List<Product>(); // Thuộc tính quan hệ điều hướng (navigation collection) danh sách các Product liên quan.
    }
}

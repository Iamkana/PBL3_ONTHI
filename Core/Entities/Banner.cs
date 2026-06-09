using System.ComponentModel.DataAnnotations; // Nhập thư viện hỗ trợ kiểm tra ràng buộc dữ liệu (DataAnnotations).
using System.ComponentModel.DataAnnotations.Schema; // Nhập thư viện hỗ trợ cấu hình ánh xạ thực thể (Schema).

namespace PBL3.Core.Entities // Định nghĩa namespace PBL3.Core.Entities để quản lý thực thể/giao diện.
{
    /// <summary>
    /// Banner hiển thị trên carousel của trang chủ. Kích thước cố định 1200x400 (tỉ lệ 3:1).
    /// </summary>
    [Table("Banners")] // Ánh xạ thực thể hiện tại với bảng dữ liệu 'Banners' trong cơ sở dữ liệu.
    public class Banner // Định nghĩa thực thể/lớp nghiệp vụ Banner.
    {
        [Key] // Đánh dấu thuộc tính ngay phía dưới là Khóa chính (Primary Key).
        public int Id { get; set; } // Thuộc tính Id kiểu int đóng vai trò khóa chính định danh bản ghi.

        [Required] // Đánh dấu thuộc tính phía dưới là bắt buộc nhập (NOT NULL).
        [MaxLength(200)] // Ràng buộc độ dài ký tự tối đa của cột trong CSDL là 200 ký tự.
        public string Title { get; set; } = string.Empty; // Thuộc tính Title kiểu dữ liệu string lưu trữ thông tin thực thể với giá trị mặc định là string.Empty.

        [Required] // Đánh dấu thuộc tính phía dưới là bắt buộc nhập (NOT NULL).
        [MaxLength(500)] // Ràng buộc độ dài ký tự tối đa của cột trong CSDL là 500 ký tự.
        public string ImageUrl { get; set; } = string.Empty; // Thuộc tính ImageUrl kiểu dữ liệu string lưu trữ thông tin thực thể với giá trị mặc định là string.Empty.

        [MaxLength(500)] // Ràng buộc độ dài ký tự tối đa của cột trong CSDL là 500 ký tự.
        public string? LinkUrl { get; set; } // Thuộc tính LinkUrl kiểu dữ liệu string? lưu trữ thông tin thực thể.

        public int SortOrder { get; set; } // Thuộc tính SortOrder kiểu dữ liệu int lưu trữ thông tin thực thể.

        public bool IsActive { get; set; } = true; // Thuộc tính IsActive kiểu dữ liệu bool lưu trữ thông tin thực thể với giá trị mặc định là true.

        public DateTime? StartDate { get; set; } // Thuộc tính StartDate kiểu dữ liệu DateTime? lưu trữ thông tin thực thể.
        public DateTime? EndDate { get; set; } // Thuộc tính EndDate kiểu dữ liệu DateTime? lưu trữ thông tin thực thể.

        // Audit
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow; // Thuộc tính CreatedDate lưu thời điểm bản ghi được tạo lập ban đầu.
        [MaxLength(100)] // Ràng buộc độ dài ký tự tối đa của cột trong CSDL là 100 ký tự.
        public string? CreatedBy { get; set; } // Thuộc tính CreatedBy lưu người dùng đã khởi tạo bản ghi này.
        public DateTime? ModifiedDate { get; set; } // Thuộc tính ModifiedDate lưu thời điểm bản ghi được cập nhật gần nhất.
        [MaxLength(100)] // Ràng buộc độ dài ký tự tối đa của cột trong CSDL là 100 ký tự.
        public string? ModifiedBy { get; set; } // Thuộc tính ModifiedBy lưu người dùng đã cập nhật bản ghi này sau cùng.
        public bool IsDeleted { get; set; } // Thuộc tính IsDeleted đánh dấu bản ghi đã bị xóa mềm (soft deleted).
        public DateTime? DeletedDate { get; set; } // Thuộc tính DeletedDate lưu thời điểm bản ghi bị xóa mềm khỏi hệ thống.
    }
}

using System.ComponentModel.DataAnnotations; // Nhập thư viện hỗ trợ kiểm tra ràng buộc dữ liệu (DataAnnotations).
using System.ComponentModel.DataAnnotations.Schema; // Nhập thư viện hỗ trợ cấu hình ánh xạ thực thể (Schema).

namespace PBL3.Core.Entities // Định nghĩa namespace PBL3.Core.Entities để quản lý thực thể/giao diện.
{
    [Table("Categories")] // Ánh xạ thực thể hiện tại với bảng dữ liệu 'Categories' trong cơ sở dữ liệu.
    public class Category // Định nghĩa thực thể/lớp nghiệp vụ Category.
    {
        [Key] // Đánh dấu thuộc tính ngay phía dưới là Khóa chính (Primary Key).
        public int Id { get; set; } // Thuộc tính Id kiểu int đóng vai trò khóa chính định danh bản ghi.
        [Required] // Đánh dấu thuộc tính phía dưới là bắt buộc nhập (NOT NULL).
        [MaxLength(100)] // Ràng buộc độ dài ký tự tối đa của cột trong CSDL là 100 ký tự.
        public string Name { get; set; } = string.Empty; // Thuộc tính Name kiểu dữ liệu string lưu trữ thông tin thực thể với giá trị mặc định là string.Empty.
        [Required] // Đánh dấu thuộc tính phía dưới là bắt buộc nhập (NOT NULL).
        [MaxLength(150)] // Ràng buộc độ dài ký tự tối đa của cột trong CSDL là 150 ký tự.
        public string Slug { get; set; } = string.Empty; // Thuộc tính Slug kiểu dữ liệu string lưu trữ thông tin thực thể với giá trị mặc định là string.Empty.

        public int? ParentId { get; set; } // Thuộc tính khóa ngoại ParentId kiểu int? liên kết sang thực thể liên quan.
        public int Level { get; set; } // Thuộc tính Level kiểu dữ liệu int lưu trữ thông tin thực thể.

        [MaxLength(500)] // Ràng buộc độ dài ký tự tối đa của cột trong CSDL là 500 ký tự.
        public string? ImageUrl { get; set; } // Thuộc tính ImageUrl kiểu dữ liệu string? lưu trữ thông tin thực thể.
        public int SortOrder { get; set; } // Thuộc tính SortOrder kiểu dữ liệu int lưu trữ thông tin thực thể.
        public bool IsVisible { get; set; } = true; // Thuộc tính IsVisible kiểu dữ liệu bool lưu trữ thông tin thực thể với giá trị mặc định là true.

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow; // Thuộc tính CreatedDate lưu thời điểm bản ghi được tạo lập ban đầu.
        [MaxLength(100)] // Ràng buộc độ dài ký tự tối đa của cột trong CSDL là 100 ký tự.
        public string? CreatedBy { get; set; } // Thuộc tính CreatedBy lưu người dùng đã khởi tạo bản ghi này.
        public DateTime? ModifiedDate { get; set; } // Thuộc tính ModifiedDate lưu thời điểm bản ghi được cập nhật gần nhất.
        [MaxLength(100)] // Ràng buộc độ dài ký tự tối đa của cột trong CSDL là 100 ký tự.
        public string? ModifiedBy { get; set; } // Thuộc tính ModifiedBy lưu người dùng đã cập nhật bản ghi này sau cùng.
        public bool IsDeleted { get; set; } // Thuộc tính IsDeleted đánh dấu bản ghi đã bị xóa mềm (soft deleted).
        public DateTime? DeletedDate { get; set; } // Thuộc tính DeletedDate lưu thời điểm bản ghi bị xóa mềm khỏi hệ thống.

        [ForeignKey("ParentId")] // Chỉ định thuộc tính điều hướng tương ứng sử dụng khóa ngoại 'ParentId'.
        public virtual Category? Parent { get; set; } // Thuộc tính quan hệ điều hướng ảo (virtual navigation property) tham chiếu tới thực thể Category?.
        public virtual ICollection<Category> Children { get; set; } = new List<Category>(); // Thuộc tính quan hệ điều hướng (navigation collection) danh sách các Category liên quan.
        public virtual ICollection<Product> Products { get; set; } = new List<Product>(); // Thuộc tính quan hệ điều hướng (navigation collection) danh sách các Product liên quan.
    }
}

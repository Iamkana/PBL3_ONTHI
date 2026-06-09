using System.ComponentModel.DataAnnotations; // Nhập thư viện hỗ trợ kiểm tra ràng buộc dữ liệu (DataAnnotations).
using System.ComponentModel.DataAnnotations.Schema; // Nhập thư viện hỗ trợ cấu hình ánh xạ thực thể (Schema).

namespace PBL3.Core.Entities // Định nghĩa namespace PBL3.Core.Entities để quản lý thực thể/giao diện.
{
    // 9. ProductReviews
    [Table("ProductReviews")] // Ánh xạ thực thể hiện tại với bảng dữ liệu 'ProductReviews' trong cơ sở dữ liệu.
    public class ProductReview // Định nghĩa thực thể/lớp nghiệp vụ ProductReview.
    {
        [Key] // Đánh dấu thuộc tính ngay phía dưới là Khóa chính (Primary Key).
        public int Id { get; set; } // Thuộc tính Id kiểu int đóng vai trò khóa chính định danh bản ghi.

        public int ProductId { get; set; } // Thuộc tính khóa ngoại ProductId kiểu int liên kết sang thực thể liên quan.
        public Guid UserId { get; set; } // Thuộc tính khóa ngoại UserId kiểu Guid liên kết sang thực thể liên quan.

        public byte Rating { get; set; } // Thuộc tính Rating kiểu dữ liệu byte lưu trữ thông tin thực thể.

        [MaxLength(200)] // Ràng buộc độ dài ký tự tối đa của cột trong CSDL là 200 ký tự.
        public string? Title { get; set; } // Thuộc tính Title kiểu dữ liệu string? lưu trữ thông tin thực thể.
        [MaxLength(2000)] // Ràng buộc độ dài ký tự tối đa của cột trong CSDL là 2000 ký tự.
        public string? Content { get; set; } // Thuộc tính Content kiểu dữ liệu string? lưu trữ thông tin thực thể.

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow; // Thuộc tính CreatedDate lưu thời điểm bản ghi được tạo lập ban đầu.
        public bool IsDeleted { get; set; } // Thuộc tính IsDeleted đánh dấu bản ghi đã bị xóa mềm (soft deleted).
        public DateTime? DeletedDate { get; set; } // Thuộc tính DeletedDate lưu thời điểm bản ghi bị xóa mềm khỏi hệ thống.

        [ForeignKey("ProductId")] // Chỉ định thuộc tính điều hướng tương ứng sử dụng khóa ngoại 'ProductId'.
        public virtual Product Product { get; set; } = null!; // Thuộc tính quan hệ điều hướng ảo (virtual navigation property) tham chiếu tới thực thể Product.
        [ForeignKey("UserId")] // Chỉ định thuộc tính điều hướng tương ứng sử dụng khóa ngoại 'UserId'.
        public virtual AppUser User { get; set; } = null!; // Thuộc tính quan hệ điều hướng ảo (virtual navigation property) tham chiếu tới thực thể AppUser.
    }
}

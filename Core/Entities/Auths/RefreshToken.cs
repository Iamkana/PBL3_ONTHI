using System.ComponentModel.DataAnnotations; // Nhập thư viện hỗ trợ kiểm tra ràng buộc dữ liệu (DataAnnotations).
using System.ComponentModel.DataAnnotations.Schema; // Nhập thư viện hỗ trợ cấu hình ánh xạ thực thể (Schema).

namespace PBL3.Core.Entities // Định nghĩa namespace PBL3.Core.Entities để quản lý thực thể/giao diện.
{
    // 3. RefreshTokens
    [Table("RefreshTokens")] // Ánh xạ thực thể hiện tại với bảng dữ liệu 'RefreshTokens' trong cơ sở dữ liệu.
    public class RefreshToken // Định nghĩa thực thể/lớp nghiệp vụ RefreshToken.
    {
        [Key] // Đánh dấu thuộc tính ngay phía dưới là Khóa chính (Primary Key).
        public int Id { get; set; } // Thuộc tính Id kiểu int đóng vai trò khóa chính định danh bản ghi.

        public Guid UserId { get; set; } // Thuộc tính khóa ngoại UserId kiểu Guid liên kết sang thực thể liên quan.

        [Required] // Đánh dấu thuộc tính phía dưới là bắt buộc nhập (NOT NULL).
        public string Token { get; set; } = string.Empty; // Thuộc tính Token kiểu dữ liệu string lưu trữ thông tin thực thể với giá trị mặc định là string.Empty.
        [Required] // Đánh dấu thuộc tính phía dưới là bắt buộc nhập (NOT NULL).
        [MaxLength(100)] // Ràng buộc độ dài ký tự tối đa của cột trong CSDL là 100 ký tự.
        public string JwtId { get; set; } = string.Empty; // Thuộc tính khóa ngoại JwtId kiểu string liên kết sang thực thể liên quan.
        public bool IsUsed { get; set; } // Thuộc tính IsUsed kiểu dữ liệu bool lưu trữ thông tin thực thể.
        public bool IsRevoked { get; set; } // Thuộc tính IsRevoked kiểu dữ liệu bool lưu trữ thông tin thực thể.
        public DateTime ExpiryDate { get; set; } // Thuộc tính ExpiryDate kiểu dữ liệu DateTime lưu trữ thông tin thực thể.

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow; // Thuộc tính CreatedDate lưu thời điểm bản ghi được tạo lập ban đầu.
        public bool IsDeleted { get; set; } // Thuộc tính IsDeleted đánh dấu bản ghi đã bị xóa mềm (soft deleted).

        [ForeignKey("UserId")] // Chỉ định thuộc tính điều hướng tương ứng sử dụng khóa ngoại 'UserId'.
        public virtual AppUser User { get; set; } = null!; // Thuộc tính quan hệ điều hướng ảo (virtual navigation property) tham chiếu tới thực thể AppUser.
    }
}

using System.ComponentModel.DataAnnotations; // Nhập thư viện hỗ trợ kiểm tra ràng buộc dữ liệu (DataAnnotations).
using System.ComponentModel.DataAnnotations.Schema; // Nhập thư viện hỗ trợ cấu hình ánh xạ thực thể (Schema).

namespace PBL3.Core.Entities // Định nghĩa namespace PBL3.Core.Entities để quản lý thực thể/giao diện.
{
    // 2. UserProfile — Thông tin cá nhân (1:1 với AppUser, shared primary key)
    [Table("UserProfiles")] // Ánh xạ thực thể hiện tại với bảng dữ liệu 'UserProfiles' trong cơ sở dữ liệu.
    public class UserProfile // Định nghĩa thực thể/lớp nghiệp vụ UserProfile.
    {
        [Key] // Đánh dấu thuộc tính ngay phía dưới là Khóa chính (Primary Key).
        public Guid UserId { get; set; } // Thuộc tính khóa ngoại UserId kiểu Guid liên kết sang thực thể liên quan.

        [Required] // Đánh dấu thuộc tính phía dưới là bắt buộc nhập (NOT NULL).
        [MaxLength(110)] // Ràng buộc độ dài ký tự tối đa của cột trong CSDL là 110 ký tự.
        public string FullName { get; set; } = string.Empty; // Thuộc tính FullName kiểu dữ liệu string lưu trữ thông tin thực thể với giá trị mặc định là string.Empty.
        public byte Gender { get; set; } // Thuộc tính Gender kiểu dữ liệu byte lưu trữ thông tin thực thể.
        public DateTime? DateOfBirth { get; set; } // Thuộc tính DateOfBirth kiểu dữ liệu DateTime? lưu trữ thông tin thực thể.
        [MaxLength(500)] // Ràng buộc độ dài ký tự tối đa của cột trong CSDL là 500 ký tự.
        public string? AvatarUrl { get; set; } // Thuộc tính AvatarUrl kiểu dữ liệu string? lưu trữ thông tin thực thể.

        // Địa chỉ mặc định
        [MaxLength(255)] // Ràng buộc độ dài ký tự tối đa của cột trong CSDL là 255 ký tự.
        public string? Address { get; set; } // Thuộc tính Address kiểu dữ liệu string? lưu trữ thông tin thực thể.
        [MaxLength(100)] // Ràng buộc độ dài ký tự tối đa của cột trong CSDL là 100 ký tự.
        public string? City { get; set; } // Thuộc tính City kiểu dữ liệu string? lưu trữ thông tin thực thể.

        [ForeignKey("UserId")] // Chỉ định thuộc tính điều hướng tương ứng sử dụng khóa ngoại 'UserId'.
        public virtual AppUser User { get; set; } = null!; // Thuộc tính quan hệ điều hướng ảo (virtual navigation property) tham chiếu tới thực thể AppUser.
    }
}

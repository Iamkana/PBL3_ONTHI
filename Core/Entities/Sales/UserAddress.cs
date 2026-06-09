using System.ComponentModel.DataAnnotations; // Nhập thư viện hỗ trợ kiểm tra ràng buộc dữ liệu (DataAnnotations).
using System.ComponentModel.DataAnnotations.Schema; // Nhập thư viện hỗ trợ cấu hình ánh xạ thực thể (Schema).

namespace PBL3.Core.Entities // Định nghĩa namespace PBL3.Core.Entities để quản lý thực thể/giao diện.
{
    // 8. UserAddresses
    [Table("UserAddresses")] // Ánh xạ thực thể hiện tại với bảng dữ liệu 'UserAddresses' trong cơ sở dữ liệu.
    public class UserAddress // Định nghĩa thực thể/lớp nghiệp vụ UserAddress.
    {
        [Key] // Đánh dấu thuộc tính ngay phía dưới là Khóa chính (Primary Key).
        public int Id { get; set; } // Thuộc tính Id kiểu int đóng vai trò khóa chính định danh bản ghi.
        public Guid UserId { get; set; } // Thuộc tính khóa ngoại UserId kiểu Guid liên kết sang thực thể liên quan.

        [Required] // Đánh dấu thuộc tính phía dưới là bắt buộc nhập (NOT NULL).
        [MaxLength(100)] // Ràng buộc độ dài ký tự tối đa của cột trong CSDL là 100 ký tự.
        public string ReceiverName { get; set; } = string.Empty; // Thuộc tính ReceiverName kiểu dữ liệu string lưu trữ thông tin thực thể với giá trị mặc định là string.Empty.
        [Required] // Đánh dấu thuộc tính phía dưới là bắt buộc nhập (NOT NULL).
        [MaxLength(20)] // Ràng buộc độ dài ký tự tối đa của cột trong CSDL là 20 ký tự.
        public string PhoneNumber { get; set; } = string.Empty; // Thuộc tính PhoneNumber kiểu dữ liệu string lưu trữ thông tin thực thể với giá trị mặc định là string.Empty.
        [Required] // Đánh dấu thuộc tính phía dưới là bắt buộc nhập (NOT NULL).
        [MaxLength(255)] // Ràng buộc độ dài ký tự tối đa của cột trong CSDL là 255 ký tự.
        public string AddressLine { get; set; } = string.Empty; // Thuộc tính AddressLine kiểu dữ liệu string lưu trữ thông tin thực thể với giá trị mặc định là string.Empty.
        [Required] // Đánh dấu thuộc tính phía dưới là bắt buộc nhập (NOT NULL).
        [MaxLength(100)] // Ràng buộc độ dài ký tự tối đa của cột trong CSDL là 100 ký tự.
        public string City { get; set; } = string.Empty; // Thuộc tính City kiểu dữ liệu string lưu trữ thông tin thực thể với giá trị mặc định là string.Empty.

        public bool IsDefault { get; set; } // Thuộc tính IsDefault kiểu dữ liệu bool lưu trữ thông tin thực thể.

        [ForeignKey("UserId")] // Chỉ định thuộc tính điều hướng tương ứng sử dụng khóa ngoại 'UserId'.
        public virtual AppUser User { get; set; } = null!; // Thuộc tính quan hệ điều hướng ảo (virtual navigation property) tham chiếu tới thực thể AppUser.
    }
}

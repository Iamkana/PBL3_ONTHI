using System.ComponentModel.DataAnnotations; // Nhập thư viện hỗ trợ kiểm tra ràng buộc dữ liệu (DataAnnotations).
using System.ComponentModel.DataAnnotations.Schema; // Nhập thư viện hỗ trợ cấu hình ánh xạ thực thể (Schema).
using Microsoft.AspNetCore.Identity; // Nhập thư viện Microsoft.AspNetCore.Identity để quản lý phân quyền và người dùng.

namespace PBL3.Core.Entities // Định nghĩa namespace PBL3.Core.Entities để quản lý thực thể/giao diện.
{
    // 1. AppUser kế thừa IdentityUser<Guid>
    public class AppUser : IdentityUser<Guid> // Định nghĩa thực thể/lớp nghiệp vụ AppUser kế thừa từ IdentityUser<Guid>.
    {
        // B. QUẢN TRỊ & TRẠNG THÁI
        public bool IsActive { get; set; } = true; // Thuộc tính IsActive kiểu dữ liệu bool lưu trữ thông tin thực thể với giá trị mặc định là true.
        [MaxLength(500)] // Ràng buộc độ dài ký tự tối đa của cột trong CSDL là 500 ký tự.
        public string? LockReason { get; set; } // Thuộc tính LockReason kiểu dữ liệu string? lưu trữ thông tin thực thể.
        public byte Type { get; set; } // Thuộc tính Type kiểu dữ liệu byte lưu trữ thông tin thực thể.

        // C. REFRESH TOKEN (Lưu trực tiếp trên User, 1-1)
        public string? RefreshToken { get; set; } // Thuộc tính RefreshToken kiểu dữ liệu string? lưu trữ thông tin thực thể.
        public DateTime? RefreshTokenExpiryTime { get; set; } // Thuộc tính RefreshTokenExpiryTime kiểu dữ liệu DateTime? lưu trữ thông tin thực thể.

        // D. AUDIT
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow; // Thuộc tính CreatedDate lưu thời điểm bản ghi được tạo lập ban đầu.
        public bool IsDeleted { get; set; } // Thuộc tính IsDeleted đánh dấu bản ghi đã bị xóa mềm (soft deleted).

        // Navigation Properties
        public virtual UserProfile? Profile { get; set; } // Thuộc tính quan hệ điều hướng ảo (virtual navigation property) tham chiếu tới thực thể UserProfile?.
        public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>(); // Thuộc tính quan hệ điều hướng (navigation collection) danh sách các RefreshToken liên quan.
    }
}

using System.ComponentModel.DataAnnotations; // Nhập thư viện hỗ trợ kiểm tra ràng buộc dữ liệu (DataAnnotations).
using System.ComponentModel.DataAnnotations.Schema; // Nhập thư viện hỗ trợ cấu hình ánh xạ thực thể (Schema).
using Microsoft.AspNetCore.Identity; // Nhập thư viện Microsoft.AspNetCore.Identity để quản lý phân quyền và người dùng.

namespace PBL3.Core.Entities // Định nghĩa namespace PBL3.Core.Entities để quản lý thực thể/giao diện.
{
    // 2. AppRole kế thừa IdentityRole<Guid>
    public class AppRole : IdentityRole<Guid> // Định nghĩa thực thể/lớp nghiệp vụ AppRole kế thừa từ IdentityRole<Guid>.
    {
        [MaxLength(250)] // Ràng buộc độ dài ký tự tối đa của cột trong CSDL là 250 ký tự.
        public string? Description { get; set; } // Thuộc tính Description kiểu dữ liệu string? lưu trữ thông tin thực thể.
        [Required] // Đánh dấu thuộc tính phía dưới là bắt buộc nhập (NOT NULL).
        [MaxLength(50)] // Ràng buộc độ dài ký tự tối đa của cột trong CSDL là 50 ký tự.
        public string RoleCode { get; set; } = string.Empty; // Thuộc tính RoleCode kiểu dữ liệu string lưu trữ thông tin thực thể với giá trị mặc định là string.Empty.
    }
}

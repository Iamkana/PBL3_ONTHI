using System.Collections.Generic; // Nhập thư viện làm việc với các kiểu danh sách, bộ sưu tập.
using System.ComponentModel.DataAnnotations; // Nhập (import) namespace System.ComponentModel.DataAnnotations để sử dụng các thành phần bên trong.
using System; // Nhập thư viện hệ thống cơ bản.

namespace PBL3.Shared.DTOs.Pos // Định nghĩa namespace PBL3.Shared.DTOs.Pos quản lý cấu trúc code truyền tải và validator.
{
    public class PosCustomerDto // Định nghĩa lớp DTO truyền tải dữ liệu PosCustomerDto.
    {
        public Guid UserId { get; set; } // Thuộc tính UserId kiểu dữ liệu Guid lưu trữ thông tin truyền tải.
        public string FullName { get; set; } = string.Empty; // Thuộc tính FullName kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public string PhoneNumber { get; set; } = string.Empty; // Thuộc tính PhoneNumber kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public string? Email { get; set; } // Thuộc tính Email kiểu dữ liệu string? lưu trữ thông tin truyền tải.
        // Thêm các trường khác như hạng, điểm nếu có sau này
    }
}

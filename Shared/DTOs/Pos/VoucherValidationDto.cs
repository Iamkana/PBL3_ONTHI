using System.Collections.Generic; // Nhập thư viện làm việc với các kiểu danh sách, bộ sưu tập.
using System.ComponentModel.DataAnnotations; // Nhập (import) namespace System.ComponentModel.DataAnnotations để sử dụng các thành phần bên trong.
using System; // Nhập thư viện hệ thống cơ bản.

namespace PBL3.Shared.DTOs.Pos // Định nghĩa namespace PBL3.Shared.DTOs.Pos quản lý cấu trúc code truyền tải và validator.
{
    public class VoucherValidationDto // Định nghĩa lớp DTO truyền tải dữ liệu VoucherValidationDto.
    {
        public bool IsValid { get; set; } // Thuộc tính IsValid kiểu dữ liệu bool lưu trữ thông tin truyền tải.
        public string Code { get; set; } = string.Empty; // Thuộc tính Code kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public string Message { get; set; } = string.Empty; // Thuộc tính Message kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public decimal DiscountAmount { get; set; } // Thuộc tính DiscountAmount kiểu dữ liệu decimal lưu trữ thông tin truyền tải.
    }
}

using System.Collections.Generic; // Nhập thư viện làm việc với các kiểu danh sách, bộ sưu tập.
using System.ComponentModel.DataAnnotations; // Nhập (import) namespace System.ComponentModel.DataAnnotations để sử dụng các thành phần bên trong.
using System; // Nhập thư viện hệ thống cơ bản.

namespace PBL3.Shared.DTOs.Pos // Định nghĩa namespace PBL3.Shared.DTOs.Pos quản lý cấu trúc code truyền tải và validator.
{
    public class PosScanRequest // Định nghĩa lớp DTO truyền tải dữ liệu PosScanRequest.
    {
        [Required] // Thực thi dòng lệnh nghiệp vụ.
        public string SerialNumber { get; set; } = string.Empty; // Thuộc tính SerialNumber kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
    }
}

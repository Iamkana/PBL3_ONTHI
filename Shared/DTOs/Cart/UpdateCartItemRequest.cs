using System.Collections.Generic; // Nhập thư viện làm việc với các kiểu danh sách, bộ sưu tập.
using System.ComponentModel.DataAnnotations; // Nhập (import) namespace System.ComponentModel.DataAnnotations để sử dụng các thành phần bên trong.

namespace PBL3.Shared.DTOs.Cart // Định nghĩa namespace PBL3.Shared.DTOs.Cart quản lý cấu trúc code truyền tải và validator.
{
    /// <summary>
    /// Request cập nhật số lượng một item trong giỏ.
    /// </summary>
    public class UpdateCartItemRequest // Định nghĩa lớp DTO truyền tải dữ liệu UpdateCartItemRequest.
    {
        [Required(ErrorMessage = "Số lượng không được để trống.")] // Thực thi dòng lệnh nghiệp vụ.
        public int Quantity { get; set; } // Thuộc tính Quantity kiểu dữ liệu int lưu trữ thông tin truyền tải.
    }
}

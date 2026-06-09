using System.Collections.Generic; // Nhập thư viện làm việc với các kiểu danh sách, bộ sưu tập.
using System.ComponentModel.DataAnnotations; // Nhập (import) namespace System.ComponentModel.DataAnnotations để sử dụng các thành phần bên trong.

namespace PBL3.Shared.DTOs.Cart // Định nghĩa namespace PBL3.Shared.DTOs.Cart quản lý cấu trúc code truyền tải và validator.
{
    /// <summary>
    /// Response toàn bộ giỏ hàng của User.
    /// </summary>
    public class CartResponse // Định nghĩa lớp DTO truyền tải dữ liệu CartResponse.
    {
        public List<CartItemResponse> Items { get; set; } = new(); // Thuộc tính Items kiểu dữ liệu List<CartItemResponse> lưu trữ thông tin truyền tải với giá trị mặc định là new().
        public decimal TotalAmount { get; set; } // Thuộc tính TotalAmount kiểu dữ liệu decimal lưu trữ thông tin truyền tải.
    }
}

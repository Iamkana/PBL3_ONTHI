using System.Collections.Generic; // Nhập thư viện làm việc với các kiểu danh sách, bộ sưu tập.
using System.ComponentModel.DataAnnotations; // Nhập (import) namespace System.ComponentModel.DataAnnotations để sử dụng các thành phần bên trong.

namespace PBL3.Shared.DTOs.Cart // Định nghĩa namespace PBL3.Shared.DTOs.Cart quản lý cấu trúc code truyền tải và validator.
{
    /// <summary>
    /// Request thêm sản phẩm vào giỏ hàng.
    /// </summary>
    public class AddToCartRequest // Định nghĩa lớp DTO truyền tải dữ liệu AddToCartRequest.
    {
        [Required(ErrorMessage = "Mã biến thể sản phẩm không được để trống.")] // Thực thi dòng lệnh nghiệp vụ.
        public int VariantId { get; set; } // Thuộc tính VariantId kiểu dữ liệu int lưu trữ thông tin truyền tải.

        [Required(ErrorMessage = "Số lượng không được để trống.")] // Thực thi dòng lệnh nghiệp vụ.
        [Range(1, 999, ErrorMessage = "Số lượng phải từ 1 đến 999.")] // Thực thi dòng lệnh nghiệp vụ.
        public int Quantity { get; set; } // Thuộc tính Quantity kiểu dữ liệu int lưu trữ thông tin truyền tải.
    }
}

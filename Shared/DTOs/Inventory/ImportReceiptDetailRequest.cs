namespace PBL3.Shared.DTOs.Inventory // Định nghĩa namespace PBL3.Shared.DTOs.Inventory quản lý cấu trúc code truyền tải và validator.
{
    /// <summary>
    /// Chi tiết 1 dòng sản phẩm trong phiếu nhập.
    /// </summary>
    public class ImportReceiptDetailRequest // Định nghĩa lớp DTO truyền tải dữ liệu ImportReceiptDetailRequest.
    {
        /// <summary>Biến thể sản phẩm được chọn.</summary>
        public int VariantId { get; set; } // Thuộc tính VariantId kiểu dữ liệu int lưu trữ thông tin truyền tải.

        /// <summary>Số lượng nhập. Phải > 0.</summary>
        public int Quantity { get; set; } // Thuộc tính Quantity kiểu dữ liệu int lưu trữ thông tin truyền tải.

        /// <summary>Giá nhập (giá vốn). Phải >= 0.</summary>
        public decimal ImportPrice { get; set; } // Thuộc tính ImportPrice kiểu dữ liệu decimal lưu trữ thông tin truyền tải.

        /// <summary>
        /// Danh sách mã Serial quét từ vỏ hộp.
        /// Số lượng phần tử BẮT BUỘC phải bằng Quantity.
        /// </summary>
        public List<string> SerialNumbers { get; set; } = new(); // Thuộc tính SerialNumbers kiểu dữ liệu List<string> lưu trữ thông tin truyền tải với giá trị mặc định là new().
    }
}

namespace PBL3.Shared.DTOs.Inventory // Định nghĩa namespace PBL3.Shared.DTOs.Inventory quản lý cấu trúc code truyền tải và validator.
{
    /// <summary>
    /// DTO chi tiết 1 dòng sản phẩm trong phiếu nhập.
    /// </summary>
    public class ImportReceiptDetailDto // Định nghĩa lớp DTO truyền tải dữ liệu ImportReceiptDetailDto.
    {
        public int Id { get; set; } // Thuộc tính Id kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public int VariantId { get; set; } // Thuộc tính VariantId kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public string VariantName { get; set; } = string.Empty; // Thuộc tính VariantName kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public string SKU { get; set; } = string.Empty; // Thuộc tính SKU kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public int Quantity { get; set; } // Thuộc tính Quantity kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public decimal ImportPrice { get; set; } // Thuộc tính ImportPrice kiểu dữ liệu decimal lưu trữ thông tin truyền tải.
        public decimal SubTotal { get; set; } // Thuộc tính SubTotal kiểu dữ liệu decimal lưu trữ thông tin truyền tải.

        /// <summary>Danh sách Serial đã nhập cho dòng này.</summary>
        public List<string> SerialNumbers { get; set; } = new(); // Thuộc tính SerialNumbers kiểu dữ liệu List<string> lưu trữ thông tin truyền tải với giá trị mặc định là new().
    }
}

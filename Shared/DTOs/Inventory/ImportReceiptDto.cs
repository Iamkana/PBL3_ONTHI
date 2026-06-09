namespace PBL3.Shared.DTOs.Inventory // Định nghĩa namespace PBL3.Shared.DTOs.Inventory quản lý cấu trúc code truyền tải và validator.
{
    /// <summary>
    /// DTO hiển thị thông tin phiếu nhập kho (danh sách).
    /// </summary>
    public class ImportReceiptDto // Định nghĩa lớp DTO truyền tải dữ liệu ImportReceiptDto.
    {
        public int Id { get; set; } // Thuộc tính Id kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public string ReceiptCode { get; set; } = string.Empty; // Thuộc tính ReceiptCode kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public int SupplierId { get; set; } // Thuộc tính SupplierId kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public string SupplierName { get; set; } = string.Empty; // Thuộc tính SupplierName kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public string EmployeeName { get; set; } = string.Empty; // Thuộc tính EmployeeName kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public DateTime ImportDate { get; set; } // Thuộc tính ImportDate kiểu dữ liệu DateTime lưu trữ thông tin truyền tải.
        public decimal TotalAmount { get; set; } // Thuộc tính TotalAmount kiểu dữ liệu decimal lưu trữ thông tin truyền tải.
        public string? Note { get; set; } // Thuộc tính Note kiểu dữ liệu string? lưu trữ thông tin truyền tải.

        /// <summary>Danh sách chi tiết (chỉ hiển thị ở API chi tiết).</summary>
        public List<ImportReceiptDetailDto>? Details { get; set; } // Thuộc tính Details kiểu dữ liệu List<ImportReceiptDetailDto>? lưu trữ thông tin truyền tải.
    }
}

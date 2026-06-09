namespace PBL3.Shared.DTOs.Inventory // Định nghĩa namespace PBL3.Shared.DTOs.Inventory quản lý cấu trúc code truyền tải và validator.
{
    /// <summary>
    /// Request tạo phiếu nhập kho.
    /// </summary>
    public class CreateImportReceiptRequest // Định nghĩa lớp DTO truyền tải dữ liệu CreateImportReceiptRequest.
    {
        /// <summary>Nhà cung cấp (bắt buộc).</summary>
        public int SupplierId { get; set; } // Thuộc tính SupplierId kiểu dữ liệu int lưu trữ thông tin truyền tải.

        /// <summary>Ghi chú phiếu nhập.</summary>
        public string? Note { get; set; } // Thuộc tính Note kiểu dữ liệu string? lưu trữ thông tin truyền tải.

        /// <summary>Danh sách chi tiết các sản phẩm nhập. Bắt buộc có ít nhất 1 dòng.</summary>
        public List<ImportReceiptDetailRequest> Details { get; set; } = new(); // Thuộc tính Details kiểu dữ liệu List<ImportReceiptDetailRequest> lưu trữ thông tin truyền tải với giá trị mặc định là new().
    }
}

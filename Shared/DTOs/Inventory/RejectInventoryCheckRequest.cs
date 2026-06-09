namespace PBL3.Shared.DTOs.Inventory // Định nghĩa namespace PBL3.Shared.DTOs.Inventory quản lý cấu trúc code truyền tải và validator.
{
    public class RejectInventoryCheckRequest // Định nghĩa lớp DTO truyền tải dữ liệu RejectInventoryCheckRequest.
    {
        public string Reason { get; set; } = string.Empty; // Thuộc tính Reason kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.

        /// <summary>
        /// true = trả về Draft để quét lại.
        /// false = Cancelled, đóng phiếu.
        /// </summary>
        public bool ReturnToDraft { get; set; } // Thuộc tính ReturnToDraft kiểu dữ liệu bool lưu trữ thông tin truyền tải.
    }
}

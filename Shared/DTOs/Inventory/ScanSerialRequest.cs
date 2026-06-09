namespace PBL3.Shared.DTOs.Inventory // Định nghĩa namespace PBL3.Shared.DTOs.Inventory quản lý cấu trúc code truyền tải và validator.
{
    public class ScanSerialRequest // Định nghĩa lớp DTO truyền tải dữ liệu ScanSerialRequest.
    {
        public string SerialNumber { get; set; } = string.Empty; // Thuộc tính SerialNumber kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.

        /// <summary>
        /// Bắt buộc khi serial không tồn tại trong DB (A3 — UnknownSurplus).
        /// Nhân viên chọn Variant tương ứng để ghi nhận.
        /// </summary>
        public int? VariantIdForUnknown { get; set; } // Thuộc tính VariantIdForUnknown kiểu dữ liệu int? lưu trữ thông tin truyền tải.
    }
}

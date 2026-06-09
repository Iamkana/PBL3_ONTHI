namespace PBL3.Shared.DTOs.Inventory // Định nghĩa namespace PBL3.Shared.DTOs.Inventory quản lý cấu trúc code truyền tải và validator.
{
    public class UpdateScanReasonRequest // Định nghĩa lớp DTO truyền tải dữ liệu UpdateScanReasonRequest.
    {
        public string Reason { get; set; } = string.Empty; // Thuộc tính Reason kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public string? ProposedActionNote { get; set; } // Thuộc tính ProposedActionNote kiểu dữ liệu string? lưu trữ thông tin truyền tải.
    }
}

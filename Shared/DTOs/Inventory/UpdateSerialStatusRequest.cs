namespace PBL3.Shared.DTOs.Inventory // Định nghĩa namespace PBL3.Shared.DTOs.Inventory quản lý cấu trúc code truyền tải và validator.
{
    public class UpdateSerialStatusRequest // Định nghĩa lớp DTO truyền tải dữ liệu UpdateSerialStatusRequest.
    {
        public byte NewStatus { get; set; } // Thuộc tính NewStatus kiểu dữ liệu byte lưu trữ thông tin truyền tải.
        public string? Note { get; set; } // Thuộc tính Note kiểu dữ liệu string? lưu trữ thông tin truyền tải.
    }
}

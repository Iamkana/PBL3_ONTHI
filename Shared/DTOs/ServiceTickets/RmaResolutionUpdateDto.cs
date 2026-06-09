namespace PBL3.Shared.DTOs.ServiceTickets // Định nghĩa namespace PBL3.Shared.DTOs.ServiceTickets quản lý cấu trúc code truyền tải và validator.
{
    public class RmaResolutionUpdateDto // Định nghĩa lớp DTO truyền tải dữ liệu RmaResolutionUpdateDto.
    {
        public byte ManufacturerResolution { get; set; } // Thuộc tính ManufacturerResolution kiểu dữ liệu byte lưu trữ thông tin truyền tải.
        public string? ManufacturerNotes { get; set; } // Thuộc tính ManufacturerNotes kiểu dữ liệu string? lưu trữ thông tin truyền tải.

        // If Replaced: which serial to swap in
        public int? ReplacementSerialId { get; set; } // Thuộc tính ReplacementSerialId kiểu dữ liệu int? lưu trữ thông tin truyền tải.
    }
}

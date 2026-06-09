namespace PBL3.Shared.DTOs.Manufacturers // Định nghĩa namespace PBL3.Shared.DTOs.Manufacturers quản lý cấu trúc code truyền tải và validator.
{
    /// <summary>
    /// DTO rút gọn dùng cho Dropdown / Lookup (Id + Name).
    /// </summary>
    public class ManufacturerSummaryDto // Định nghĩa lớp DTO truyền tải dữ liệu ManufacturerSummaryDto.
    {
        public int Id { get; set; } // Thuộc tính Id kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public string Name { get; set; } = string.Empty; // Thuộc tính Name kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public string? LogoUrl { get; set; } // Thuộc tính LogoUrl kiểu dữ liệu string? lưu trữ thông tin truyền tải.
    }
}

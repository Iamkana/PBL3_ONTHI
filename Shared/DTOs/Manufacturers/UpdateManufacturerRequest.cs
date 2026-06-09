namespace PBL3.Shared.DTOs.Manufacturers // Định nghĩa namespace PBL3.Shared.DTOs.Manufacturers quản lý cấu trúc code truyền tải và validator.
{
    /// <summary>
    /// Request cập nhật hãng sản xuất.
    /// </summary>
    public class UpdateManufacturerRequest // Định nghĩa lớp DTO truyền tải dữ liệu UpdateManufacturerRequest.
    {
        public string Name { get; set; } = string.Empty; // Thuộc tính Name kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public string? LogoUrl { get; set; } // Thuộc tính LogoUrl kiểu dữ liệu string? lưu trữ thông tin truyền tải.
        public string? Website { get; set; } // Thuộc tính Website kiểu dữ liệu string? lưu trữ thông tin truyền tải.
        public string? SupportEmail { get; set; } // Thuộc tính SupportEmail kiểu dữ liệu string? lưu trữ thông tin truyền tải.
    }
}

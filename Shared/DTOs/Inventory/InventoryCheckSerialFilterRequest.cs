namespace PBL3.Shared.DTOs.Inventory // Định nghĩa namespace PBL3.Shared.DTOs.Inventory quản lý cấu trúc code truyền tải và validator.
{
    public class InventoryCheckSerialFilterRequest // Định nghĩa lớp DTO truyền tải dữ liệu InventoryCheckSerialFilterRequest.
    {
        public byte? ScanStatus { get; set; } // Thuộc tính ScanStatus kiểu dữ liệu byte? lưu trữ thông tin truyền tải.
        public int? VariantId { get; set; } // Thuộc tính VariantId kiểu dữ liệu int? lưu trữ thông tin truyền tải.
        public int PageNumber { get; set; } = 1; // Thuộc tính PageNumber kiểu dữ liệu int lưu trữ thông tin truyền tải với giá trị mặc định là 1.
        public int PageSize { get; set; } = 20; // Thuộc tính PageSize kiểu dữ liệu int lưu trữ thông tin truyền tải với giá trị mặc định là 20.
    }
}

namespace PBL3.Shared.DTOs.Inventory // Định nghĩa namespace PBL3.Shared.DTOs.Inventory quản lý cấu trúc code truyền tải và validator.
{
    public class CreateInventoryCheckRequest // Định nghĩa lớp DTO truyền tải dữ liệu CreateInventoryCheckRequest.
    {
        /// <summary>0 = AllStore, 1 = Category</summary>
        public byte ScopeType { get; set; } // Thuộc tính ScopeType kiểu dữ liệu byte lưu trữ thông tin truyền tải.

        /// <summary>Bắt buộc khi ScopeType = 1.</summary>
        public int? ScopeCategoryId { get; set; } // Thuộc tính ScopeCategoryId kiểu dữ liệu int? lưu trữ thông tin truyền tải.

        public string? Note { get; set; } // Thuộc tính Note kiểu dữ liệu string? lưu trữ thông tin truyền tải.
    }
}

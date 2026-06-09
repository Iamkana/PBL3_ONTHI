namespace PBL3.Shared.DTOs.Analytics // Định nghĩa namespace PBL3.Shared.DTOs.Analytics quản lý cấu trúc code truyền tải và validator.
{
    public class InventorySummaryDto // Định nghĩa lớp DTO truyền tải dữ liệu InventorySummaryDto.
    {
        public int TotalSkus { get; set; } // Thuộc tính TotalSkus kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public int TotalAvailable { get; set; } // Thuộc tính TotalAvailable kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public int TotalReserved { get; set; } // Thuộc tính TotalReserved kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public int TotalSold { get; set; } // Thuộc tính TotalSold kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public int TotalDefective { get; set; } // Thuộc tính TotalDefective kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public int TotalReturned { get; set; } // Thuộc tính TotalReturned kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public int LowStockSkus { get; set; } // Thuộc tính LowStockSkus kiểu dữ liệu int lưu trữ thông tin truyền tải.
    }
}

namespace PBL3.Shared.DTOs.Inventory // Định nghĩa namespace PBL3.Shared.DTOs.Inventory quản lý cấu trúc code truyền tải và validator.
{
    public class ProductSerialStatisticsDto // Định nghĩa lớp DTO truyền tải dữ liệu ProductSerialStatisticsDto.
    {
        public int TotalCount { get; set; } // Thuộc tính TotalCount kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public int AvailableCount { get; set; } // Thuộc tính AvailableCount kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public int ReservedCount { get; set; } // Thuộc tính ReservedCount kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public int SoldCount { get; set; } // Thuộc tính SoldCount kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public int DefectiveCount { get; set; } // Thuộc tính DefectiveCount kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public int ReturnedCount { get; set; } // Thuộc tính ReturnedCount kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public int LostCount { get; set; } // Thuộc tính LostCount kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public int? ProductId { get; set; } // Thuộc tính ProductId kiểu dữ liệu int? lưu trữ thông tin truyền tải.
        public int? VariantId { get; set; } // Thuộc tính VariantId kiểu dữ liệu int? lưu trữ thông tin truyền tải.
    }
}

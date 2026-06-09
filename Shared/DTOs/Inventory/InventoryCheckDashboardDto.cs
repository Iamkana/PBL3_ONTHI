namespace PBL3.Shared.DTOs.Inventory // Định nghĩa namespace PBL3.Shared.DTOs.Inventory quản lý cấu trúc code truyền tải và validator.
{
    public class InventoryCheckDashboardDto // Định nghĩa lớp DTO truyền tải dữ liệu InventoryCheckDashboardDto.
    {
        public int CheckId { get; set; } // Thuộc tính CheckId kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public string CheckCode { get; set; } = string.Empty; // Thuộc tính CheckCode kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public byte Status { get; set; } // Thuộc tính Status kiểu dữ liệu byte lưu trữ thông tin truyền tải.
        public string StatusName { get; set; } = string.Empty; // Thuộc tính StatusName kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public DateTime SnapshotAt { get; set; } // Thuộc tính SnapshotAt kiểu dữ liệu DateTime lưu trữ thông tin truyền tải.

        public int TotalSystem { get; set; } // Thuộc tính TotalSystem kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public int TotalScanned { get; set; } // Thuộc tính TotalScanned kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public int MatchedCount { get; set; } // Thuộc tính MatchedCount kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public int MissingCount { get; set; } // Thuộc tính MissingCount kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public int SurplusCount { get; set; } // Thuộc tính SurplusCount kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public int UnknownSurplusCount { get; set; } // Thuộc tính UnknownSurplusCount kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public int DefectiveCount { get; set; } // Thuộc tính DefectiveCount kiểu dữ liệu int lưu trữ thông tin truyền tải.

        /// <summary>Phần trăm đã quét = TotalScanned / TotalSystem * 100.</summary>
        public decimal PercentComplete => TotalSystem > 0 // Thực thi dòng lệnh nghiệp vụ.
            ? Math.Round((decimal)TotalScanned / TotalSystem * 100, 1) // Thực thi dòng lệnh nghiệp vụ.
            : 0; // Thực thi dòng lệnh nghiệp vụ.
    }
}

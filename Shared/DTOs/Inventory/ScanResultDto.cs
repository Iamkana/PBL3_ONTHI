namespace PBL3.Shared.DTOs.Inventory // Định nghĩa namespace PBL3.Shared.DTOs.Inventory quản lý cấu trúc code truyền tải và validator.
{
    /// <summary>Kết quả trả về sau mỗi lần quét 1 Serial.</summary>
    public class ScanResultDto // Định nghĩa lớp DTO truyền tải dữ liệu ScanResultDto.
    {
        public bool Success { get; set; } // Thuộc tính Success kiểu dữ liệu bool lưu trữ thông tin truyền tải.
        public string Message { get; set; } = string.Empty; // Thuộc tính Message kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.

        /// <summary>Kết quả phân loại: Matched / Surplus / UnknownSurplus.</summary>
        public byte ScanStatus { get; set; } // Thuộc tính ScanStatus kiểu dữ liệu byte lưu trữ thông tin truyền tải.
        public string ScanStatusName { get; set; } = string.Empty; // Thuộc tính ScanStatusName kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.

        /// <summary>SerialNumber vừa quét.</summary>
        public string SerialNumberRaw { get; set; } = string.Empty; // Thuộc tính SerialNumberRaw kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.

        /// <summary>true nếu serial đã quét trước đó trong phiếu này.</summary>
        public bool IsDuplicateScan { get; set; } // Thuộc tính IsDuplicateScan kiểu dữ liệu bool lưu trữ thông tin truyền tải.

        /// <summary>true nếu serial không tồn tại DB và cần nhân viên chọn Variant (A3).</summary>
        public bool RequiresVariantInput { get; set; } // Thuộc tính RequiresVariantInput kiểu dữ liệu bool lưu trữ thông tin truyền tải.

        /// <summary>Thông tin serial tìm thấy trong DB (null nếu UnknownSurplus).</summary>
        public string? FoundSerialNumber { get; set; } // Thuộc tính FoundSerialNumber kiểu dữ liệu string? lưu trữ thông tin truyền tải.
        public string? FoundVariantName { get; set; } // Thuộc tính FoundVariantName kiểu dữ liệu string? lưu trữ thông tin truyền tải.
        public byte? FoundOriginalStatus { get; set; } // Thuộc tính FoundOriginalStatus kiểu dữ liệu byte? lưu trữ thông tin truyền tải.
        public string? FoundOriginalStatusName { get; set; } // Thuộc tính FoundOriginalStatusName kiểu dữ liệu string? lưu trữ thông tin truyền tải.
        public string? SurplusNote { get; set; } // Thuộc tính SurplusNote kiểu dữ liệu string? lưu trữ thông tin truyền tải.

        /// <summary>Mini snapshot tổng hợp counts sau lần scan này.</summary>
        public InventoryCheckDashboardDto? MiniDashboard { get; set; } // Thuộc tính MiniDashboard kiểu dữ liệu InventoryCheckDashboardDto? lưu trữ thông tin truyền tải.
    }
}

namespace PBL3.Shared.DTOs.Inventory // Định nghĩa namespace PBL3.Shared.DTOs.Inventory quản lý cấu trúc code truyền tải và validator.
{
    public class InventoryCheckSerialDto // Định nghĩa lớp DTO truyền tải dữ liệu InventoryCheckSerialDto.
    {
        public int Id { get; set; } // Thuộc tính Id kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public int? SerialId { get; set; } // Thuộc tính SerialId kiểu dữ liệu int? lưu trữ thông tin truyền tải.
        public string SerialNumberRaw { get; set; } = string.Empty; // Thuộc tính SerialNumberRaw kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public int? VariantId { get; set; } // Thuộc tính VariantId kiểu dữ liệu int? lưu trữ thông tin truyền tải.
        public string? VariantName { get; set; } // Thuộc tính VariantName kiểu dữ liệu string? lưu trữ thông tin truyền tải.
        public string? SKU { get; set; } // Thuộc tính SKU kiểu dữ liệu string? lưu trữ thông tin truyền tải.
        public byte? OriginalStatus { get; set; } // Thuộc tính OriginalStatus kiểu dữ liệu byte? lưu trữ thông tin truyền tải.
        public string? OriginalStatusName { get; set; } // Thuộc tính OriginalStatusName kiểu dữ liệu string? lưu trữ thông tin truyền tải.
        public byte ScanStatus { get; set; } // Thuộc tính ScanStatus kiểu dữ liệu byte lưu trữ thông tin truyền tải.
        public string ScanStatusName { get; set; } = string.Empty; // Thuộc tính ScanStatusName kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public DateTime? ScannedAt { get; set; } // Thuộc tính ScannedAt kiểu dữ liệu DateTime? lưu trữ thông tin truyền tải.
        public string? Note { get; set; } // Thuộc tính Note kiểu dữ liệu string? lưu trữ thông tin truyền tải.
        public string? ProposedActionNote { get; set; } // Thuộc tính ProposedActionNote kiểu dữ liệu string? lưu trữ thông tin truyền tải.
        public bool ResolvedDuringApproval { get; set; } // Thuộc tính ResolvedDuringApproval kiểu dữ liệu bool lưu trữ thông tin truyền tải.
    }
}

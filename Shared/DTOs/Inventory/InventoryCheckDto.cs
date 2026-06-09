namespace PBL3.Shared.DTOs.Inventory // Định nghĩa namespace PBL3.Shared.DTOs.Inventory quản lý cấu trúc code truyền tải và validator.
{
    public class InventoryCheckDto // Định nghĩa lớp DTO truyền tải dữ liệu InventoryCheckDto.
    {
        public int Id { get; set; } // Thuộc tính Id kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public string CheckCode { get; set; } = string.Empty; // Thuộc tính CheckCode kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public Guid EmployeeId { get; set; } // Thuộc tính EmployeeId kiểu dữ liệu Guid lưu trữ thông tin truyền tải.
        public string EmployeeName { get; set; } = string.Empty; // Thuộc tính EmployeeName kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public DateTime CheckDate { get; set; } // Thuộc tính CheckDate kiểu dữ liệu DateTime lưu trữ thông tin truyền tải.
        public DateTime SnapshotAt { get; set; } // Thuộc tính SnapshotAt kiểu dữ liệu DateTime lưu trữ thông tin truyền tải.
        public byte Status { get; set; } // Thuộc tính Status kiểu dữ liệu byte lưu trữ thông tin truyền tải.
        public string StatusName { get; set; } = string.Empty; // Thuộc tính StatusName kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public byte ScopeType { get; set; } // Thuộc tính ScopeType kiểu dữ liệu byte lưu trữ thông tin truyền tải.
        public string ScopeTypeName { get; set; } = string.Empty; // Thuộc tính ScopeTypeName kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public int? ScopeCategoryId { get; set; } // Thuộc tính ScopeCategoryId kiểu dữ liệu int? lưu trữ thông tin truyền tải.
        public string? ScopeCategoryName { get; set; } // Thuộc tính ScopeCategoryName kiểu dữ liệu string? lưu trữ thông tin truyền tải.
        public string? Note { get; set; } // Thuộc tính Note kiểu dữ liệu string? lưu trữ thông tin truyền tải.
        public string? RejectReason { get; set; } // Thuộc tính RejectReason kiểu dữ liệu string? lưu trữ thông tin truyền tải.
        public Guid? ApprovedByEmployeeId { get; set; } // Thuộc tính ApprovedByEmployeeId kiểu dữ liệu Guid? lưu trữ thông tin truyền tải.
        public string? ApprovedByEmployeeName { get; set; } // Thuộc tính ApprovedByEmployeeName kiểu dữ liệu string? lưu trữ thông tin truyền tải.
        public DateTime? ApprovedAt { get; set; } // Thuộc tính ApprovedAt kiểu dữ liệu DateTime? lưu trữ thông tin truyền tải.

        public List<InventoryCheckDetailLineDto> Details { get; set; } = new(); // Thuộc tính Details kiểu dữ liệu List<InventoryCheckDetailLineDto> lưu trữ thông tin truyền tải với giá trị mặc định là new().
    }
}

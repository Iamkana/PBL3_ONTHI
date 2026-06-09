namespace PBL3.Shared.DTOs.ServiceTickets // Định nghĩa namespace PBL3.Shared.DTOs.ServiceTickets quản lý cấu trúc code truyền tải và validator.
{
    public class ServiceTicketIntakeRequestDto // Định nghĩa lớp DTO truyền tải dữ liệu ServiceTicketIntakeRequestDto.
    {
        public string SerialNumber { get; set; } = string.Empty; // Thuộc tính SerialNumber kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.

        public bool HasScratches { get; set; } // Thuộc tính HasScratches kiểu dữ liệu bool lưu trữ thông tin truyền tải.
        public bool HasDents { get; set; } // Thuộc tính HasDents kiểu dữ liệu bool lưu trữ thông tin truyền tải.
        public bool HasBurnMarks { get; set; } // Thuộc tính HasBurnMarks kiểu dữ liệu bool lưu trữ thông tin truyền tải.
        public bool HasMissingAccessories { get; set; } // Thuộc tính HasMissingAccessories kiểu dữ liệu bool lưu trữ thông tin truyền tải.
        public string? CosmeticNotes { get; set; } // Thuộc tính CosmeticNotes kiểu dữ liệu string? lưu trữ thông tin truyền tải.

        public string CustomerReportedIssue { get; set; } = string.Empty; // Thuộc tính CustomerReportedIssue kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.

        public string? WalkInCustomerName { get; set; } // Thuộc tính WalkInCustomerName kiểu dữ liệu string? lưu trữ thông tin truyền tải.
        public string? WalkInCustomerPhone { get; set; } // Thuộc tính WalkInCustomerPhone kiểu dữ liệu string? lưu trữ thông tin truyền tải.
    }
}

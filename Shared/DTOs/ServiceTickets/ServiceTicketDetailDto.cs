namespace PBL3.Shared.DTOs.ServiceTickets // Định nghĩa namespace PBL3.Shared.DTOs.ServiceTickets quản lý cấu trúc code truyền tải và validator.
{
    public class ServiceTicketDetailDto // Định nghĩa lớp DTO truyền tải dữ liệu ServiceTicketDetailDto.
    {
        public int Id { get; set; } // Thuộc tính Id kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public string TicketCode { get; set; } = string.Empty; // Thuộc tính TicketCode kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.

        public string SerialNumber { get; set; } = string.Empty; // Thuộc tính SerialNumber kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public string ProductName { get; set; } = string.Empty; // Thuộc tính ProductName kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public int SerialVariantId { get; set; } // Thuộc tính SerialVariantId kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public string? CustomerName { get; set; } // Thuộc tính CustomerName kiểu dữ liệu string? lưu trữ thông tin truyền tải.

        public DateTime IntakeDate { get; set; } // Thuộc tính IntakeDate kiểu dữ liệu DateTime lưu trữ thông tin truyền tải.
        public byte Status { get; set; } // Thuộc tính Status kiểu dữ liệu byte lưu trữ thông tin truyền tải.
        public string StatusLabel { get; set; } = string.Empty; // Thuộc tính StatusLabel kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.

        public byte ResolutionType { get; set; } // Thuộc tính ResolutionType kiểu dữ liệu byte lưu trữ thông tin truyền tải.

        public bool HasScratches { get; set; } // Thuộc tính HasScratches kiểu dữ liệu bool lưu trữ thông tin truyền tải.
        public bool HasDents { get; set; } // Thuộc tính HasDents kiểu dữ liệu bool lưu trữ thông tin truyền tải.
        public bool HasBurnMarks { get; set; } // Thuộc tính HasBurnMarks kiểu dữ liệu bool lưu trữ thông tin truyền tải.
        public bool HasMissingAccessories { get; set; } // Thuộc tính HasMissingAccessories kiểu dữ liệu bool lưu trữ thông tin truyền tải.
        public string? CosmeticNotes { get; set; } // Thuộc tính CosmeticNotes kiểu dữ liệu string? lưu trữ thông tin truyền tải.

        public bool WasInWarrantyAtIntake { get; set; } // Thuộc tính WasInWarrantyAtIntake kiểu dữ liệu bool lưu trữ thông tin truyền tải.
        public DateTime? WarrantyEndDateAtIntake { get; set; } // Thuộc tính WarrantyEndDateAtIntake kiểu dữ liệu DateTime? lưu trữ thông tin truyền tải.

        public string CustomerReportedIssue { get; set; } = string.Empty; // Thuộc tính CustomerReportedIssue kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.

        public string? DiagnosisFindings { get; set; } // Thuộc tính DiagnosisFindings kiểu dữ liệu string? lưu trữ thông tin truyền tải.
        public DateTime? DiagnosedAt { get; set; } // Thuộc tính DiagnosedAt kiểu dữ liệu DateTime? lưu trữ thông tin truyền tải.

        public Guid? AssignedEmployeeId { get; set; } // Thuộc tính AssignedEmployeeId kiểu dữ liệu Guid? lưu trữ thông tin truyền tải.
        public string? AssignedEmployeeName { get; set; } // Thuộc tính AssignedEmployeeName kiểu dữ liệu string? lưu trữ thông tin truyền tải.

        public List<ServiceTicketStatusHistoryDto> StatusHistory { get; set; } = new(); // Thuộc tính StatusHistory kiểu dữ liệu List<ServiceTicketStatusHistoryDto> lưu trữ thông tin truyền tải với giá trị mặc định là new().
        public List<QuotationDetailDto> Quotations { get; set; } = new(); // Thuộc tính Quotations kiểu dữ liệu List<QuotationDetailDto> lưu trữ thông tin truyền tải với giá trị mặc định là new().
        public RmaShipmentDetailDto? RmaShipment { get; set; } // Thuộc tính RmaShipment kiểu dữ liệu RmaShipmentDetailDto? lưu trữ thông tin truyền tải.
        public ServiceInvoiceDetailDto? Invoice { get; set; } // Thuộc tính Invoice kiểu dữ liệu ServiceInvoiceDetailDto? lưu trữ thông tin truyền tải.
    }
}

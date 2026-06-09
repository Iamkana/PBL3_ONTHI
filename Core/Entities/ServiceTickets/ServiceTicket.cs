using System.ComponentModel.DataAnnotations; // Nhập thư viện hỗ trợ kiểm tra ràng buộc dữ liệu (DataAnnotations).
using System.ComponentModel.DataAnnotations.Schema; // Nhập thư viện hỗ trợ cấu hình ánh xạ thực thể (Schema).

namespace PBL3.Core.Entities // Định nghĩa namespace PBL3.Core.Entities để quản lý thực thể/giao diện.
{
    [Table("ServiceTickets")] // Ánh xạ thực thể hiện tại với bảng dữ liệu 'ServiceTickets' trong cơ sở dữ liệu.
    public class ServiceTicket // Định nghĩa thực thể/lớp nghiệp vụ ServiceTicket.
    {
        [Key] // Đánh dấu thuộc tính ngay phía dưới là Khóa chính (Primary Key).
        public int Id { get; set; } // Thuộc tính Id kiểu int đóng vai trò khóa chính định danh bản ghi.

        [Required] // Đánh dấu thuộc tính phía dưới là bắt buộc nhập (NOT NULL).
        [MaxLength(20)] // Ràng buộc độ dài ký tự tối đa của cột trong CSDL là 20 ký tự.
        public string TicketCode { get; set; } = string.Empty; // Thuộc tính TicketCode kiểu dữ liệu string lưu trữ thông tin thực thể với giá trị mặc định là string.Empty.

        public int SerialId { get; set; } // Thuộc tính khóa ngoại SerialId kiểu int liên kết sang thực thể liên quan.
        public int OriginalOrderId { get; set; } // Thuộc tính khóa ngoại OriginalOrderId kiểu int liên kết sang thực thể liên quan.
        public Guid? CustomerId { get; set; } // Thuộc tính CustomerId kiểu dữ liệu Guid? lưu trữ thông tin thực thể.

        public DateTime IntakeDate { get; set; } = DateTime.UtcNow; // Thuộc tính IntakeDate kiểu dữ liệu DateTime lưu trữ thông tin thực thể với giá trị mặc định là DateTime.UtcNow.
        public Guid IntakeEmployeeId { get; set; } // Thuộc tính khóa ngoại IntakeEmployeeId kiểu Guid liên kết sang thực thể liên quan.

        public bool HasScratches { get; set; } // Thuộc tính HasScratches kiểu dữ liệu bool lưu trữ thông tin thực thể.
        public bool HasDents { get; set; } // Thuộc tính HasDents kiểu dữ liệu bool lưu trữ thông tin thực thể.
        public bool HasBurnMarks { get; set; } // Thuộc tính HasBurnMarks kiểu dữ liệu bool lưu trữ thông tin thực thể.
        public bool HasMissingAccessories { get; set; } // Thuộc tính HasMissingAccessories kiểu dữ liệu bool lưu trữ thông tin thực thể.

        [MaxLength(1000)] // Ràng buộc độ dài ký tự tối đa của cột trong CSDL là 1000 ký tự.
        public string? CosmeticNotes { get; set; } // Thuộc tính CosmeticNotes kiểu dữ liệu string? lưu trữ thông tin thực thể.

        [MaxLength(2000)] // Ràng buộc độ dài ký tự tối đa của cột trong CSDL là 2000 ký tự.
        public string CustomerReportedIssue { get; set; } = string.Empty; // Thuộc tính CustomerReportedIssue kiểu dữ liệu string lưu trữ thông tin thực thể với giá trị mặc định là string.Empty.

        [MaxLength(100)] // Ràng buộc độ dài ký tự tối đa của cột trong CSDL là 100 ký tự.
        public string? WalkInCustomerName { get; set; } // Thuộc tính WalkInCustomerName kiểu dữ liệu string? lưu trữ thông tin thực thể.

        [MaxLength(20)] // Ràng buộc độ dài ký tự tối đa của cột trong CSDL là 20 ký tự.
        public string? WalkInCustomerPhone { get; set; } // Thuộc tính WalkInCustomerPhone kiểu dữ liệu string? lưu trữ thông tin thực thể.

        public bool WasInWarrantyAtIntake { get; set; } // Thuộc tính WasInWarrantyAtIntake kiểu dữ liệu bool lưu trữ thông tin thực thể.
        public DateTime? WarrantyEndDateAtIntake { get; set; } // Thuộc tính WarrantyEndDateAtIntake kiểu dữ liệu DateTime? lưu trữ thông tin thực thể.
        public byte WarrantyEvalSource { get; set; } // Thuộc tính WarrantyEvalSource kiểu dữ liệu byte lưu trữ thông tin thực thể.

        public byte Status { get; set; } = 0; // Thuộc tính Status kiểu dữ liệu byte lưu trữ thông tin thực thể với giá trị mặc định là 0.
        public byte ResolutionType { get; set; } = 0; // Thuộc tính ResolutionType kiểu dữ liệu byte lưu trữ thông tin thực thể với giá trị mặc định là 0.
        public Guid? AssignedEmployeeId { get; set; } // Thuộc tính AssignedEmployeeId kiểu dữ liệu Guid? lưu trữ thông tin thực thể.

        [MaxLength(2000)] // Ràng buộc độ dài ký tự tối đa của cột trong CSDL là 2000 ký tự.
        public string? DiagnosisFindings { get; set; } // Thuộc tính DiagnosisFindings kiểu dữ liệu string? lưu trữ thông tin thực thể.

        public DateTime? DiagnosedAt { get; set; } // Thuộc tính DiagnosedAt kiểu dữ liệu DateTime? lưu trữ thông tin thực thể.
        public Guid? DiagnosedByEmployeeId { get; set; } // Thuộc tính DiagnosedByEmployeeId kiểu dữ liệu Guid? lưu trữ thông tin thực thể.

        public int? ReplacementSerialId { get; set; } // Thuộc tính khóa ngoại ReplacementSerialId kiểu int? liên kết sang thực thể liên quan.

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow; // Thuộc tính CreatedDate lưu thời điểm bản ghi được tạo lập ban đầu.
        public DateTime? ModifiedDate { get; set; } // Thuộc tính ModifiedDate lưu thời điểm bản ghi được cập nhật gần nhất.
        public DateTime? CompletedDate { get; set; } // Thuộc tính CompletedDate kiểu dữ liệu DateTime? lưu trữ thông tin thực thể.

        [MaxLength(500)] // Ràng buộc độ dài ký tự tối đa của cột trong CSDL là 500 ký tự.
        public string? CancelReason { get; set; } // Thuộc tính CancelReason kiểu dữ liệu string? lưu trữ thông tin thực thể.

        public DateTime? CancelledAt { get; set; } // Thuộc tính CancelledAt kiểu dữ liệu DateTime? lưu trữ thông tin thực thể.
        public bool IsDeleted { get; set; } // Thuộc tính IsDeleted đánh dấu bản ghi đã bị xóa mềm (soft deleted).

        [ForeignKey("SerialId")] // Chỉ định thuộc tính điều hướng tương ứng sử dụng khóa ngoại 'SerialId'.
        public virtual ProductSerial Serial { get; set; } = null!; // Thuộc tính quan hệ điều hướng ảo (virtual navigation property) tham chiếu tới thực thể ProductSerial.

        [ForeignKey("OriginalOrderId")] // Chỉ định thuộc tính điều hướng tương ứng sử dụng khóa ngoại 'OriginalOrderId'.
        public virtual Order OriginalOrder { get; set; } = null!; // Thuộc tính quan hệ điều hướng ảo (virtual navigation property) tham chiếu tới thực thể Order.

        [ForeignKey("CustomerId")] // Chỉ định thuộc tính điều hướng tương ứng sử dụng khóa ngoại 'CustomerId'.
        public virtual AppUser? Customer { get; set; } // Thuộc tính quan hệ điều hướng ảo (virtual navigation property) tham chiếu tới thực thể AppUser?.

        [ForeignKey("ReplacementSerialId")] // Chỉ định thuộc tính điều hướng tương ứng sử dụng khóa ngoại 'ReplacementSerialId'.
        public virtual ProductSerial? ReplacementSerial { get; set; } // Thuộc tính quan hệ điều hướng ảo (virtual navigation property) tham chiếu tới thực thể ProductSerial?.

        public virtual ICollection<ServiceTicketStatusHistory> StatusHistory { get; set; } = new List<ServiceTicketStatusHistory>(); // Thuộc tính quan hệ điều hướng (navigation collection) danh sách các ServiceTicketStatusHistory liên quan.
        public virtual ICollection<Quotation> Quotations { get; set; } = new List<Quotation>(); // Thuộc tính quan hệ điều hướng (navigation collection) danh sách các Quotation liên quan.
        public virtual RmaShipment? RmaShipment { get; set; } // Thuộc tính quan hệ điều hướng ảo (virtual navigation property) tham chiếu tới thực thể RmaShipment?.
        public virtual ServiceInvoice? Invoice { get; set; } // Thuộc tính quan hệ điều hướng ảo (virtual navigation property) tham chiếu tới thực thể ServiceInvoice?.
    }
}

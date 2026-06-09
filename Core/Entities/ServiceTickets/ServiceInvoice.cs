using System.ComponentModel.DataAnnotations; // Nhập thư viện hỗ trợ kiểm tra ràng buộc dữ liệu (DataAnnotations).
using System.ComponentModel.DataAnnotations.Schema; // Nhập thư viện hỗ trợ cấu hình ánh xạ thực thể (Schema).

namespace PBL3.Core.Entities // Định nghĩa namespace PBL3.Core.Entities để quản lý thực thể/giao diện.
{
    [Table("ServiceInvoices")] // Ánh xạ thực thể hiện tại với bảng dữ liệu 'ServiceInvoices' trong cơ sở dữ liệu.
    public class ServiceInvoice // Định nghĩa thực thể/lớp nghiệp vụ ServiceInvoice.
    {
        [Key] // Đánh dấu thuộc tính ngay phía dưới là Khóa chính (Primary Key).
        public int Id { get; set; } // Thuộc tính Id kiểu int đóng vai trò khóa chính định danh bản ghi.

        [Required] // Đánh dấu thuộc tính phía dưới là bắt buộc nhập (NOT NULL).
        [MaxLength(20)] // Ràng buộc độ dài ký tự tối đa của cột trong CSDL là 20 ký tự.
        public string InvoiceCode { get; set; } = string.Empty; // Thuộc tính InvoiceCode kiểu dữ liệu string lưu trữ thông tin thực thể với giá trị mặc định là string.Empty.

        public int TicketId { get; set; } // Thuộc tính khóa ngoại TicketId kiểu int liên kết sang thực thể liên quan.
        public int? QuotationId { get; set; } // Thuộc tính khóa ngoại QuotationId kiểu int? liên kết sang thực thể liên quan.

        public DateTime IssuedDate { get; set; } = DateTime.UtcNow; // Thuộc tính IssuedDate kiểu dữ liệu DateTime lưu trữ thông tin thực thể với giá trị mặc định là DateTime.UtcNow.
        public Guid IssuedByEmployeeId { get; set; } // Thuộc tính khóa ngoại IssuedByEmployeeId kiểu Guid liên kết sang thực thể liên quan.

        public decimal LaborCost { get; set; } // Thuộc tính LaborCost kiểu dữ liệu decimal lưu trữ thông tin thực thể.
        public decimal PartsTotal { get; set; } // Thuộc tính PartsTotal kiểu dữ liệu decimal lưu trữ thông tin thực thể.
        public decimal GrandTotal { get; set; } // Thuộc tính GrandTotal kiểu dữ liệu decimal lưu trữ thông tin thực thể.

        public byte PaymentMethod { get; set; } // Thuộc tính PaymentMethod kiểu dữ liệu byte lưu trữ thông tin thực thể.
        public byte PaymentStatus { get; set; } // Thuộc tính PaymentStatus kiểu dữ liệu byte lưu trữ thông tin thực thể.

        [MaxLength(500)] // Ràng buộc độ dài ký tự tối đa của cột trong CSDL là 500 ký tự.
        public string? Note { get; set; } // Thuộc tính Note kiểu dữ liệu string? lưu trữ thông tin thực thể.

        public bool IsDeleted { get; set; } // Thuộc tính IsDeleted đánh dấu bản ghi đã bị xóa mềm (soft deleted).

        [ForeignKey("TicketId")] // Chỉ định thuộc tính điều hướng tương ứng sử dụng khóa ngoại 'TicketId'.
        public virtual ServiceTicket Ticket { get; set; } = null!; // Thuộc tính quan hệ điều hướng ảo (virtual navigation property) tham chiếu tới thực thể ServiceTicket.

        [ForeignKey("QuotationId")] // Chỉ định thuộc tính điều hướng tương ứng sử dụng khóa ngoại 'QuotationId'.
        public virtual Quotation? Quotation { get; set; } // Thuộc tính quan hệ điều hướng ảo (virtual navigation property) tham chiếu tới thực thể Quotation?.

        public virtual ICollection<ServiceInvoiceItem> Items { get; set; } = new List<ServiceInvoiceItem>(); // Thuộc tính quan hệ điều hướng (navigation collection) danh sách các ServiceInvoiceItem liên quan.
    }
}

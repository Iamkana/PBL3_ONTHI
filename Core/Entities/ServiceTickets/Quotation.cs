using System.ComponentModel.DataAnnotations; // Nhập thư viện hỗ trợ kiểm tra ràng buộc dữ liệu (DataAnnotations).
using System.ComponentModel.DataAnnotations.Schema; // Nhập thư viện hỗ trợ cấu hình ánh xạ thực thể (Schema).

namespace PBL3.Core.Entities // Định nghĩa namespace PBL3.Core.Entities để quản lý thực thể/giao diện.
{
    [Table("Quotations")] // Ánh xạ thực thể hiện tại với bảng dữ liệu 'Quotations' trong cơ sở dữ liệu.
    public class Quotation // Định nghĩa thực thể/lớp nghiệp vụ Quotation.
    {
        [Key] // Đánh dấu thuộc tính ngay phía dưới là Khóa chính (Primary Key).
        public int Id { get; set; } // Thuộc tính Id kiểu int đóng vai trò khóa chính định danh bản ghi.

        public int TicketId { get; set; } // Thuộc tính khóa ngoại TicketId kiểu int liên kết sang thực thể liên quan.
        public DateTime IssuedDate { get; set; } = DateTime.UtcNow; // Thuộc tính IssuedDate kiểu dữ liệu DateTime lưu trữ thông tin thực thể với giá trị mặc định là DateTime.UtcNow.
        public Guid IssuedByEmployeeId { get; set; } // Thuộc tính khóa ngoại IssuedByEmployeeId kiểu Guid liên kết sang thực thể liên quan.

        public decimal LaborCost { get; set; } // Thuộc tính LaborCost kiểu dữ liệu decimal lưu trữ thông tin thực thể.
        public decimal PartsTotal { get; set; } // Thuộc tính PartsTotal kiểu dữ liệu decimal lưu trữ thông tin thực thể.
        public decimal GrandTotal { get; set; } // Thuộc tính GrandTotal kiểu dữ liệu decimal lưu trữ thông tin thực thể.

        public byte Status { get; set; } = 0; // Thuộc tính Status kiểu dữ liệu byte lưu trữ thông tin thực thể với giá trị mặc định là 0.

        [MaxLength(1000)] // Ràng buộc độ dài ký tự tối đa của cột trong CSDL là 1000 ký tự.
        public string? CustomerDecisionNote { get; set; } // Thuộc tính CustomerDecisionNote kiểu dữ liệu string? lưu trữ thông tin thực thể.

        public DateTime? CustomerDecidedAt { get; set; } // Thuộc tính CustomerDecidedAt kiểu dữ liệu DateTime? lưu trữ thông tin thực thể.

        [ForeignKey("TicketId")] // Chỉ định thuộc tính điều hướng tương ứng sử dụng khóa ngoại 'TicketId'.
        public virtual ServiceTicket Ticket { get; set; } = null!; // Thuộc tính quan hệ điều hướng ảo (virtual navigation property) tham chiếu tới thực thể ServiceTicket.

        public virtual ICollection<QuotationItem> Items { get; set; } = new List<QuotationItem>(); // Thuộc tính quan hệ điều hướng (navigation collection) danh sách các QuotationItem liên quan.
    }
}

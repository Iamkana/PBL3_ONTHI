using System.ComponentModel.DataAnnotations; // Nhập thư viện hỗ trợ kiểm tra ràng buộc dữ liệu (DataAnnotations).
using System.ComponentModel.DataAnnotations.Schema; // Nhập thư viện hỗ trợ cấu hình ánh xạ thực thể (Schema).

namespace PBL3.Core.Entities // Định nghĩa namespace PBL3.Core.Entities để quản lý thực thể/giao diện.
{
    [Table("ServiceTicketStatusHistory")] // Ánh xạ thực thể hiện tại với bảng dữ liệu 'ServiceTicketStatusHistory' trong cơ sở dữ liệu.
    public class ServiceTicketStatusHistory // Định nghĩa thực thể/lớp nghiệp vụ ServiceTicketStatusHistory.
    {
        [Key] // Đánh dấu thuộc tính ngay phía dưới là Khóa chính (Primary Key).
        public int Id { get; set; } // Thuộc tính Id kiểu int đóng vai trò khóa chính định danh bản ghi.

        public int TicketId { get; set; } // Thuộc tính khóa ngoại TicketId kiểu int liên kết sang thực thể liên quan.
        public byte FromStatus { get; set; } // Thuộc tính FromStatus kiểu dữ liệu byte lưu trữ thông tin thực thể.
        public byte ToStatus { get; set; } // Thuộc tính ToStatus kiểu dữ liệu byte lưu trữ thông tin thực thể.
        public Guid ChangedByEmployeeId { get; set; } // Thuộc tính khóa ngoại ChangedByEmployeeId kiểu Guid liên kết sang thực thể liên quan.
        public DateTime ChangedAt { get; set; } = DateTime.UtcNow; // Thuộc tính ChangedAt kiểu dữ liệu DateTime lưu trữ thông tin thực thể với giá trị mặc định là DateTime.UtcNow.

        [MaxLength(500)] // Ràng buộc độ dài ký tự tối đa của cột trong CSDL là 500 ký tự.
        public string? Note { get; set; } // Thuộc tính Note kiểu dữ liệu string? lưu trữ thông tin thực thể.

        [ForeignKey("TicketId")] // Chỉ định thuộc tính điều hướng tương ứng sử dụng khóa ngoại 'TicketId'.
        public virtual ServiceTicket Ticket { get; set; } = null!; // Thuộc tính quan hệ điều hướng ảo (virtual navigation property) tham chiếu tới thực thể ServiceTicket.
    }
}

using System.ComponentModel.DataAnnotations; // Nhập thư viện hỗ trợ kiểm tra ràng buộc dữ liệu (DataAnnotations).
using System.ComponentModel.DataAnnotations.Schema; // Nhập thư viện hỗ trợ cấu hình ánh xạ thực thể (Schema).

namespace PBL3.Core.Entities // Định nghĩa namespace PBL3.Core.Entities để quản lý thực thể/giao diện.
{
    [Table("SerialRepairLogs")] // Ánh xạ thực thể hiện tại với bảng dữ liệu 'SerialRepairLogs' trong cơ sở dữ liệu.
    public class SerialRepairLog // Định nghĩa thực thể/lớp nghiệp vụ SerialRepairLog.
    {
        [Key] // Đánh dấu thuộc tính ngay phía dưới là Khóa chính (Primary Key).
        public int Id { get; set; } // Thuộc tính Id kiểu int đóng vai trò khóa chính định danh bản ghi.

        public int SerialId { get; set; } // Thuộc tính khóa ngoại SerialId kiểu int liên kết sang thực thể liên quan.
        public int? TicketId { get; set; } // Thuộc tính khóa ngoại TicketId kiểu int? liên kết sang thực thể liên quan.
        public byte ResolutionType { get; set; } // Thuộc tính ResolutionType kiểu dữ liệu byte lưu trữ thông tin thực thể.
        public DateTime LoggedAt { get; set; } = DateTime.UtcNow; // Thuộc tính LoggedAt kiểu dữ liệu DateTime lưu trữ thông tin thực thể với giá trị mặc định là DateTime.UtcNow.
        public Guid LoggedByEmployeeId { get; set; } // Thuộc tính khóa ngoại LoggedByEmployeeId kiểu Guid liên kết sang thực thể liên quan.

        [MaxLength(2000)] // Ràng buộc độ dài ký tự tối đa của cột trong CSDL là 2000 ký tự.
        public string Summary { get; set; } = string.Empty; // Thuộc tính Summary kiểu dữ liệu string lưu trữ thông tin thực thể với giá trị mặc định là string.Empty.

        public int? ReplacedBySerialId { get; set; } // Thuộc tính khóa ngoại ReplacedBySerialId kiểu int? liên kết sang thực thể liên quan.

        [ForeignKey("SerialId")] // Chỉ định thuộc tính điều hướng tương ứng sử dụng khóa ngoại 'SerialId'.
        public virtual ProductSerial Serial { get; set; } = null!; // Thuộc tính quan hệ điều hướng ảo (virtual navigation property) tham chiếu tới thực thể ProductSerial.

        [ForeignKey("TicketId")] // Chỉ định thuộc tính điều hướng tương ứng sử dụng khóa ngoại 'TicketId'.
        public virtual ServiceTicket? Ticket { get; set; } // Thuộc tính quan hệ điều hướng ảo (virtual navigation property) tham chiếu tới thực thể ServiceTicket?.

        [ForeignKey("ReplacedBySerialId")] // Chỉ định thuộc tính điều hướng tương ứng sử dụng khóa ngoại 'ReplacedBySerialId'.
        public virtual ProductSerial? ReplacedBySerial { get; set; } // Thuộc tính quan hệ điều hướng ảo (virtual navigation property) tham chiếu tới thực thể ProductSerial?.
    }
}

using System.ComponentModel.DataAnnotations; // Nhập thư viện hỗ trợ kiểm tra ràng buộc dữ liệu (DataAnnotations).
using System.ComponentModel.DataAnnotations.Schema; // Nhập thư viện hỗ trợ cấu hình ánh xạ thực thể (Schema).

namespace PBL3.Core.Entities // Định nghĩa namespace PBL3.Core.Entities để quản lý thực thể/giao diện.
{
    [Table("RmaShipments")] // Ánh xạ thực thể hiện tại với bảng dữ liệu 'RmaShipments' trong cơ sở dữ liệu.
    public class RmaShipment // Định nghĩa thực thể/lớp nghiệp vụ RmaShipment.
    {
        [Key] // Đánh dấu thuộc tính ngay phía dưới là Khóa chính (Primary Key).
        public int Id { get; set; } // Thuộc tính Id kiểu int đóng vai trò khóa chính định danh bản ghi.

        public int TicketId { get; set; } // Thuộc tính khóa ngoại TicketId kiểu int liên kết sang thực thể liên quan.

        [Required] // Đánh dấu thuộc tính phía dưới là bắt buộc nhập (NOT NULL).
        [MaxLength(100)] // Ràng buộc độ dài ký tự tối đa của cột trong CSDL là 100 ký tự.
        public string CarrierName { get; set; } = string.Empty; // Thuộc tính CarrierName kiểu dữ liệu string lưu trữ thông tin thực thể với giá trị mặc định là string.Empty.

        [Required] // Đánh dấu thuộc tính phía dưới là bắt buộc nhập (NOT NULL).
        [MaxLength(100)] // Ràng buộc độ dài ký tự tối đa của cột trong CSDL là 100 ký tự.
        public string TrackingCode { get; set; } = string.Empty; // Thuộc tính TrackingCode kiểu dữ liệu string lưu trữ thông tin thực thể với giá trị mặc định là string.Empty.

        public DateTime ShippedDate { get; set; } // Thuộc tính ShippedDate kiểu dữ liệu DateTime lưu trữ thông tin thực thể.
        public Guid ShippedByEmployeeId { get; set; } // Thuộc tính khóa ngoại ShippedByEmployeeId kiểu Guid liên kết sang thực thể liên quan.

        public DateTime? ReceivedBackDate { get; set; } // Thuộc tính ReceivedBackDate kiểu dữ liệu DateTime? lưu trữ thông tin thực thể.
        public Guid? ReceivedByEmployeeId { get; set; } // Thuộc tính ReceivedByEmployeeId kiểu dữ liệu Guid? lưu trữ thông tin thực thể.

        public byte ManufacturerResolution { get; set; } = 0; // Thuộc tính ManufacturerResolution kiểu dữ liệu byte lưu trữ thông tin thực thể với giá trị mặc định là 0.

        [MaxLength(1000)] // Ràng buộc độ dài ký tự tối đa của cột trong CSDL là 1000 ký tự.
        public string? ManufacturerNotes { get; set; } // Thuộc tính ManufacturerNotes kiểu dữ liệu string? lưu trữ thông tin thực thể.

        [ForeignKey("TicketId")] // Chỉ định thuộc tính điều hướng tương ứng sử dụng khóa ngoại 'TicketId'.
        public virtual ServiceTicket Ticket { get; set; } = null!; // Thuộc tính quan hệ điều hướng ảo (virtual navigation property) tham chiếu tới thực thể ServiceTicket.
    }
}

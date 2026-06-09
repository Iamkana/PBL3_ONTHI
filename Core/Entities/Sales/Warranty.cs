using System.ComponentModel.DataAnnotations; // Nhập thư viện hỗ trợ kiểm tra ràng buộc dữ liệu (DataAnnotations).
using System.ComponentModel.DataAnnotations.Schema; // Nhập thư viện hỗ trợ cấu hình ánh xạ thực thể (Schema).

namespace PBL3.Core.Entities // Định nghĩa namespace PBL3.Core.Entities để quản lý thực thể/giao diện.
{
    // 7. Warranties
    [Table("Warranties")] // Ánh xạ thực thể hiện tại với bảng dữ liệu 'Warranties' trong cơ sở dữ liệu.
    public class Warranty // Định nghĩa thực thể/lớp nghiệp vụ Warranty.
    {
        [Key] // Đánh dấu thuộc tính ngay phía dưới là Khóa chính (Primary Key).
        public int Id { get; set; } // Thuộc tính Id kiểu int đóng vai trò khóa chính định danh bản ghi.
        public int SerialId { get; set; } // Thuộc tính khóa ngoại SerialId kiểu int liên kết sang thực thể liên quan.
        public Guid? CustomerId { get; set; } // Thuộc tính CustomerId kiểu dữ liệu Guid? lưu trữ thông tin thực thể.
        public int OrderId { get; set; } // Thuộc tính khóa ngoại OrderId kiểu int liên kết sang thực thể liên quan.

        public DateTime StartDate { get; set; } // Thuộc tính StartDate kiểu dữ liệu DateTime lưu trữ thông tin thực thể.
        public DateTime EndDate { get; set; } // Thuộc tính EndDate kiểu dữ liệu DateTime lưu trữ thông tin thực thể.
        public byte Status { get; set; } // Thuộc tính Status kiểu dữ liệu byte lưu trữ thông tin thực thể.

        [ForeignKey("SerialId")] // Chỉ định thuộc tính điều hướng tương ứng sử dụng khóa ngoại 'SerialId'.
        public virtual ProductSerial Serial { get; set; } = null!; // Thuộc tính quan hệ điều hướng ảo (virtual navigation property) tham chiếu tới thực thể ProductSerial.
        [ForeignKey("CustomerId")] // Chỉ định thuộc tính điều hướng tương ứng sử dụng khóa ngoại 'CustomerId'.
        public virtual AppUser? Customer { get; set; } // Thuộc tính quan hệ điều hướng ảo (virtual navigation property) tham chiếu tới thực thể AppUser?.
        [ForeignKey("OrderId")] // Chỉ định thuộc tính điều hướng tương ứng sử dụng khóa ngoại 'OrderId'.
        public virtual Order Order { get; set; } = null!; // Thuộc tính quan hệ điều hướng ảo (virtual navigation property) tham chiếu tới thực thể Order.
    }
}

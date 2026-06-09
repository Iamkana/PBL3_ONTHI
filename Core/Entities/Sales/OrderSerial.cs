using System.ComponentModel.DataAnnotations; // Nhập thư viện hỗ trợ kiểm tra ràng buộc dữ liệu (DataAnnotations).
using System.ComponentModel.DataAnnotations.Schema; // Nhập thư viện hỗ trợ cấu hình ánh xạ thực thể (Schema).

namespace PBL3.Core.Entities // Định nghĩa namespace PBL3.Core.Entities để quản lý thực thể/giao diện.
{
    // 4. OrderSerials
    [Table("OrderSerials")] // Ánh xạ thực thể hiện tại với bảng dữ liệu 'OrderSerials' trong cơ sở dữ liệu.
    public class OrderSerial // Định nghĩa thực thể/lớp nghiệp vụ OrderSerial.
    {
        [Key] // Đánh dấu thuộc tính ngay phía dưới là Khóa chính (Primary Key).
        public int Id { get; set; } // Thuộc tính Id kiểu int đóng vai trò khóa chính định danh bản ghi.
        public int OrderDetailId { get; set; } // Thuộc tính khóa ngoại OrderDetailId kiểu int liên kết sang thực thể liên quan.
        public int SerialId { get; set; } // Thuộc tính khóa ngoại SerialId kiểu int liên kết sang thực thể liên quan.

        [ForeignKey("OrderDetailId")] // Chỉ định thuộc tính điều hướng tương ứng sử dụng khóa ngoại 'OrderDetailId'.
        public virtual OrderDetail OrderDetail { get; set; } = null!; // Thuộc tính quan hệ điều hướng ảo (virtual navigation property) tham chiếu tới thực thể OrderDetail.
        [ForeignKey("SerialId")] // Chỉ định thuộc tính điều hướng tương ứng sử dụng khóa ngoại 'SerialId'.
        public virtual ProductSerial Serial { get; set; } = null!; // Thuộc tính quan hệ điều hướng ảo (virtual navigation property) tham chiếu tới thực thể ProductSerial.
    }
}

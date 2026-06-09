using System.ComponentModel.DataAnnotations; // Nhập thư viện hỗ trợ kiểm tra ràng buộc dữ liệu (DataAnnotations).
using System.ComponentModel.DataAnnotations.Schema; // Nhập thư viện hỗ trợ cấu hình ánh xạ thực thể (Schema).

namespace PBL3.Core.Entities // Định nghĩa namespace PBL3.Core.Entities để quản lý thực thể/giao diện.
{
    // 3. OrderDetails
    [Table("OrderDetails")] // Ánh xạ thực thể hiện tại với bảng dữ liệu 'OrderDetails' trong cơ sở dữ liệu.
    public class OrderDetail // Định nghĩa thực thể/lớp nghiệp vụ OrderDetail.
    {
        [Key] // Đánh dấu thuộc tính ngay phía dưới là Khóa chính (Primary Key).
        public int Id { get; set; } // Thuộc tính Id kiểu int đóng vai trò khóa chính định danh bản ghi.
        public int OrderId { get; set; } // Thuộc tính khóa ngoại OrderId kiểu int liên kết sang thực thể liên quan.
        public int VariantId { get; set; } // Thuộc tính khóa ngoại VariantId kiểu int liên kết sang thực thể liên quan.

        public int Quantity { get; set; } // Thuộc tính Quantity kiểu dữ liệu int lưu trữ thông tin thực thể.
        public decimal UnitPrice { get; set; } // Thuộc tính UnitPrice kiểu dữ liệu decimal lưu trữ thông tin thực thể.

        // TotalLine is computed column
        public decimal TotalLine { get; set; } // Thuộc tính TotalLine kiểu dữ liệu decimal lưu trữ thông tin thực thể.

        [ForeignKey("OrderId")] // Chỉ định thuộc tính điều hướng tương ứng sử dụng khóa ngoại 'OrderId'.
        public virtual Order Order { get; set; } = null!; // Thuộc tính quan hệ điều hướng ảo (virtual navigation property) tham chiếu tới thực thể Order.
        [ForeignKey("VariantId")] // Chỉ định thuộc tính điều hướng tương ứng sử dụng khóa ngoại 'VariantId'.
        public virtual ProductVariant Variant { get; set; } = null!; // Thuộc tính quan hệ điều hướng ảo (virtual navigation property) tham chiếu tới thực thể ProductVariant.

        public virtual ICollection<OrderSerial> OrderSerials { get; set; } = new List<OrderSerial>(); // Thuộc tính quan hệ điều hướng (navigation collection) danh sách các OrderSerial liên quan.
    }
}

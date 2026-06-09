using System.ComponentModel.DataAnnotations; // Nhập thư viện hỗ trợ kiểm tra ràng buộc dữ liệu (DataAnnotations).
using System.ComponentModel.DataAnnotations.Schema; // Nhập thư viện hỗ trợ cấu hình ánh xạ thực thể (Schema).

namespace PBL3.Core.Entities // Định nghĩa namespace PBL3.Core.Entities để quản lý thực thể/giao diện.
{
    // 6. VoucherUsages (Bảng trung gian: User đã dùng Voucher nào, trong Order nào)
    [Table("VoucherUsages")] // Ánh xạ thực thể hiện tại với bảng dữ liệu 'VoucherUsages' trong cơ sở dữ liệu.
    public class VoucherUsage // Định nghĩa thực thể/lớp nghiệp vụ VoucherUsage.
    {
        [Key] // Đánh dấu thuộc tính ngay phía dưới là Khóa chính (Primary Key).
        public int Id { get; set; } // Thuộc tính Id kiểu int đóng vai trò khóa chính định danh bản ghi.

        public int VoucherId { get; set; } // Thuộc tính khóa ngoại VoucherId kiểu int liên kết sang thực thể liên quan.
        public Guid UserId { get; set; } // Thuộc tính khóa ngoại UserId kiểu Guid liên kết sang thực thể liên quan.
        public int OrderId { get; set; } // Thuộc tính khóa ngoại OrderId kiểu int liên kết sang thực thể liên quan.

        /// <summary>
        /// Số tiền thực tế được giảm bởi voucher này trong đơn hàng.
        /// VD: Voucher giảm 20%, MaxDiscount = 50k, đơn 300k -> DiscountApplied = 50k.
        /// </summary>
        public decimal DiscountApplied { get; set; } // Thuộc tính DiscountApplied kiểu dữ liệu decimal lưu trữ thông tin thực thể.

        public DateTime UsedDate { get; set; } = DateTime.UtcNow; // Thuộc tính UsedDate kiểu dữ liệu DateTime lưu trữ thông tin thực thể với giá trị mặc định là DateTime.UtcNow.

        [ForeignKey("VoucherId")] // Chỉ định thuộc tính điều hướng tương ứng sử dụng khóa ngoại 'VoucherId'.
        public virtual Voucher Voucher { get; set; } = null!; // Thuộc tính quan hệ điều hướng ảo (virtual navigation property) tham chiếu tới thực thể Voucher.
        [ForeignKey("UserId")] // Chỉ định thuộc tính điều hướng tương ứng sử dụng khóa ngoại 'UserId'.
        public virtual AppUser User { get; set; } = null!; // Thuộc tính quan hệ điều hướng ảo (virtual navigation property) tham chiếu tới thực thể AppUser.
        [ForeignKey("OrderId")] // Chỉ định thuộc tính điều hướng tương ứng sử dụng khóa ngoại 'OrderId'.
        public virtual Order Order { get; set; } = null!; // Thuộc tính quan hệ điều hướng ảo (virtual navigation property) tham chiếu tới thực thể Order.
    }
}

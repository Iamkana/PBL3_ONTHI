using System.ComponentModel.DataAnnotations; // Nhập thư viện hỗ trợ kiểm tra ràng buộc dữ liệu (DataAnnotations).
using System.ComponentModel.DataAnnotations.Schema; // Nhập thư viện hỗ trợ cấu hình ánh xạ thực thể (Schema).

namespace PBL3.Core.Entities // Định nghĩa namespace PBL3.Core.Entities để quản lý thực thể/giao diện.
{
    // 1b. VoucherCategories (danh mục sản phẩm được áp dụng voucher)
    [Table("VoucherCategories")] // Ánh xạ thực thể hiện tại với bảng dữ liệu 'VoucherCategories' trong cơ sở dữ liệu.
    public class VoucherCategory // Định nghĩa thực thể/lớp nghiệp vụ VoucherCategory.
    {
        public int VoucherId { get; set; } // Thuộc tính khóa ngoại VoucherId kiểu int liên kết sang thực thể liên quan.
        public int CategoryId { get; set; } // Thuộc tính khóa ngoại CategoryId kiểu int liên kết sang thực thể liên quan.

        [ForeignKey("VoucherId")] // Chỉ định thuộc tính điều hướng tương ứng sử dụng khóa ngoại 'VoucherId'.
        public virtual Voucher Voucher { get; set; } = null!; // Thuộc tính quan hệ điều hướng ảo (virtual navigation property) tham chiếu tới thực thể Voucher.
        [ForeignKey("CategoryId")] // Chỉ định thuộc tính điều hướng tương ứng sử dụng khóa ngoại 'CategoryId'.
        public virtual Category Category { get; set; } = null!; // Thuộc tính quan hệ điều hướng ảo (virtual navigation property) tham chiếu tới thực thể Category.
    }
}

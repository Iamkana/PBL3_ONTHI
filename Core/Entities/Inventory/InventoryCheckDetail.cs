using System.ComponentModel.DataAnnotations; // Nhập thư viện hỗ trợ kiểm tra ràng buộc dữ liệu (DataAnnotations).
using System.ComponentModel.DataAnnotations.Schema; // Nhập thư viện hỗ trợ cấu hình ánh xạ thực thể (Schema).

namespace PBL3.Core.Entities // Định nghĩa namespace PBL3.Core.Entities để quản lý thực thể/giao diện.
{
    // 6. InventoryCheckDetails
    [Table("InventoryCheckDetails")] // Ánh xạ thực thể hiện tại với bảng dữ liệu 'InventoryCheckDetails' trong cơ sở dữ liệu.
    public class InventoryCheckDetail // Định nghĩa thực thể/lớp nghiệp vụ InventoryCheckDetail.
    {
        [Key] // Đánh dấu thuộc tính ngay phía dưới là Khóa chính (Primary Key).
        public int Id { get; set; } // Thuộc tính Id kiểu int đóng vai trò khóa chính định danh bản ghi.
        public int CheckId { get; set; } // Thuộc tính khóa ngoại CheckId kiểu int liên kết sang thực thể liên quan.
        public int VariantId { get; set; } // Thuộc tính khóa ngoại VariantId kiểu int liên kết sang thực thể liên quan.

        public int SystemQuantity { get; set; } // Thuộc tính SystemQuantity kiểu dữ liệu int lưu trữ thông tin thực thể.
        public int ActualQuantity { get; set; } // Thuộc tính ActualQuantity kiểu dữ liệu int lưu trữ thông tin thực thể.
        public int Difference { get; set; } // Thuộc tính Difference kiểu dữ liệu int lưu trữ thông tin thực thể.

        public int MatchedQuantity { get; set; } // Thuộc tính MatchedQuantity kiểu dữ liệu int lưu trữ thông tin thực thể.
        public int MissingQuantity { get; set; } // Thuộc tính MissingQuantity kiểu dữ liệu int lưu trữ thông tin thực thể.
        public int SurplusQuantity { get; set; } // Thuộc tính SurplusQuantity kiểu dữ liệu int lưu trữ thông tin thực thể.
        public int DefectiveQuantity { get; set; } // Thuộc tính DefectiveQuantity kiểu dữ liệu int lưu trữ thông tin thực thể.

        [MaxLength(255)] // Ràng buộc độ dài ký tự tối đa của cột trong CSDL là 255 ký tự.
        public string? Reason { get; set; } // Thuộc tính Reason kiểu dữ liệu string? lưu trữ thông tin thực thể.

        [ForeignKey("CheckId")] // Chỉ định thuộc tính điều hướng tương ứng sử dụng khóa ngoại 'CheckId'.
        public virtual InventoryCheck Check { get; set; } = null!; // Thuộc tính quan hệ điều hướng ảo (virtual navigation property) tham chiếu tới thực thể InventoryCheck.
        [ForeignKey("VariantId")] // Chỉ định thuộc tính điều hướng tương ứng sử dụng khóa ngoại 'VariantId'.
        public virtual ProductVariant Variant { get; set; } = null!; // Thuộc tính quan hệ điều hướng ảo (virtual navigation property) tham chiếu tới thực thể ProductVariant.

        public virtual ICollection<InventoryCheckDetailSerial> DetailSerials { get; set; } = new List<InventoryCheckDetailSerial>(); // Thuộc tính quan hệ điều hướng (navigation collection) danh sách các InventoryCheckDetailSerial liên quan.
    }
}

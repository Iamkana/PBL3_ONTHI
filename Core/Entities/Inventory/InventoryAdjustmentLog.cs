using System.ComponentModel.DataAnnotations; // Nhập thư viện hỗ trợ kiểm tra ràng buộc dữ liệu (DataAnnotations).
using System.ComponentModel.DataAnnotations.Schema; // Nhập thư viện hỗ trợ cấu hình ánh xạ thực thể (Schema).

namespace PBL3.Core.Entities // Định nghĩa namespace PBL3.Core.Entities để quản lý thực thể/giao diện.
{
    // 8. InventoryAdjustmentLogs — bút toán điều chỉnh kho phục vụ kế toán
    [Table("InventoryAdjustmentLogs")] // Ánh xạ thực thể hiện tại với bảng dữ liệu 'InventoryAdjustmentLogs' trong cơ sở dữ liệu.
    public class InventoryAdjustmentLog // Định nghĩa thực thể/lớp nghiệp vụ InventoryAdjustmentLog.
    {
        [Key] // Đánh dấu thuộc tính ngay phía dưới là Khóa chính (Primary Key).
        public int Id { get; set; } // Thuộc tính Id kiểu int đóng vai trò khóa chính định danh bản ghi.

        public int AuditCheckId { get; set; } // Thuộc tính khóa ngoại AuditCheckId kiểu int liên kết sang thực thể liên quan.
        public int SerialId { get; set; } // Thuộc tính khóa ngoại SerialId kiểu int liên kết sang thực thể liên quan.
        public int VariantId { get; set; } // Thuộc tính khóa ngoại VariantId kiểu int liên kết sang thực thể liên quan.

        public byte OldStatus { get; set; } // Thuộc tính OldStatus kiểu dữ liệu byte lưu trữ thông tin thực thể.
        public byte NewStatus { get; set; } // Thuộc tính NewStatus kiểu dữ liệu byte lưu trữ thông tin thực thể.

        // 1: Lost, 2: Defective
        public byte AdjustmentType { get; set; } // Thuộc tính AdjustmentType kiểu dữ liệu byte lưu trữ thông tin thực thể.

        [Column(TypeName = "decimal(18,2)")] // Định hình kiểu dữ liệu vật lý của cột trong CSDL là 'decimal(18,2)'.
        public decimal CostImpact { get; set; } // Thuộc tính CostImpact kiểu dữ liệu decimal lưu trữ thông tin thực thể.

        [MaxLength(500)] // Ràng buộc độ dài ký tự tối đa của cột trong CSDL là 500 ký tự.
        public string? Reason { get; set; } // Thuộc tính Reason kiểu dữ liệu string? lưu trữ thông tin thực thể.

        public DateTime AdjustedDate { get; set; } = DateTime.UtcNow; // Thuộc tính AdjustedDate kiểu dữ liệu DateTime lưu trữ thông tin thực thể với giá trị mặc định là DateTime.UtcNow.
        public Guid AdjustedByEmployeeId { get; set; } // Thuộc tính khóa ngoại AdjustedByEmployeeId kiểu Guid liên kết sang thực thể liên quan.

        [ForeignKey("AuditCheckId")] // Chỉ định thuộc tính điều hướng tương ứng sử dụng khóa ngoại 'AuditCheckId'.
        public virtual InventoryCheck AuditCheck { get; set; } = null!; // Thuộc tính quan hệ điều hướng ảo (virtual navigation property) tham chiếu tới thực thể InventoryCheck.

        [ForeignKey("SerialId")] // Chỉ định thuộc tính điều hướng tương ứng sử dụng khóa ngoại 'SerialId'.
        public virtual ProductSerial Serial { get; set; } = null!; // Thuộc tính quan hệ điều hướng ảo (virtual navigation property) tham chiếu tới thực thể ProductSerial.

        [ForeignKey("VariantId")] // Chỉ định thuộc tính điều hướng tương ứng sử dụng khóa ngoại 'VariantId'.
        public virtual ProductVariant Variant { get; set; } = null!; // Thuộc tính quan hệ điều hướng ảo (virtual navigation property) tham chiếu tới thực thể ProductVariant.
    }
}

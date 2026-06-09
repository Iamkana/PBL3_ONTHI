using System.ComponentModel.DataAnnotations; // Nhập thư viện hỗ trợ kiểm tra ràng buộc dữ liệu (DataAnnotations).
using System.ComponentModel.DataAnnotations.Schema; // Nhập thư viện hỗ trợ cấu hình ánh xạ thực thể (Schema).

namespace PBL3.Core.Entities // Định nghĩa namespace PBL3.Core.Entities để quản lý thực thể/giao diện.
{
    // 7. InventoryCheckDetailSerials — serial-level audit log
    [Table("InventoryCheckDetailSerials")] // Ánh xạ thực thể hiện tại với bảng dữ liệu 'InventoryCheckDetailSerials' trong cơ sở dữ liệu.
    public class InventoryCheckDetailSerial // Định nghĩa thực thể/lớp nghiệp vụ InventoryCheckDetailSerial.
    {
        [Key] // Đánh dấu thuộc tính ngay phía dưới là Khóa chính (Primary Key).
        public int Id { get; set; } // Thuộc tính Id kiểu int đóng vai trò khóa chính định danh bản ghi.

        public int CheckId { get; set; } // Thuộc tính khóa ngoại CheckId kiểu int liên kết sang thực thể liên quan.

        // Nullable: UnknownSurplus chưa biết variant → DetailId = null
        public int? DetailId { get; set; } // Thuộc tính khóa ngoại DetailId kiểu int? liên kết sang thực thể liên quan.

        // Nullable: UnknownSurplus chưa pick variant
        public int? VariantId { get; set; } // Thuộc tính khóa ngoại VariantId kiểu int? liên kết sang thực thể liên quan.

        // Nullable: UnknownSurplus không tồn tại trong DB
        public int? SerialId { get; set; } // Thuộc tính khóa ngoại SerialId kiểu int? liên kết sang thực thể liên quan.

        [Required] // Đánh dấu thuộc tính phía dưới là bắt buộc nhập (NOT NULL).
        [MaxLength(100)] // Ràng buộc độ dài ký tự tối đa của cột trong CSDL là 100 ký tự.
        public string SerialNumberRaw { get; set; } = string.Empty; // Thuộc tính SerialNumberRaw kiểu dữ liệu string lưu trữ thông tin thực thể với giá trị mặc định là string.Empty.

        // SerialStatus tại thời điểm chốt snapshot hoặc quét; null = UnknownSurplus
        public byte? OriginalStatus { get; set; } // Thuộc tính OriginalStatus kiểu dữ liệu byte? lưu trữ thông tin thực thể.

        // 0: Pending, 1: Matched, 2: Missing, 3: Surplus, 4: UnknownSurplus, 5: Defective
        public byte ScanStatus { get; set; } // Thuộc tính ScanStatus kiểu dữ liệu byte lưu trữ thông tin thực thể.

        public DateTime? ScannedAt { get; set; } // Thuộc tính ScannedAt kiểu dữ liệu DateTime? lưu trữ thông tin thực thể.
        public Guid? ScannedByEmployeeId { get; set; } // Thuộc tính ScannedByEmployeeId kiểu dữ liệu Guid? lưu trữ thông tin thực thể.

        [MaxLength(500)] // Ràng buộc độ dài ký tự tối đa của cột trong CSDL là 500 ký tự.
        public string? Note { get; set; } // Thuộc tính Note kiểu dữ liệu string? lưu trữ thông tin thực thể.

        [MaxLength(200)] // Ràng buộc độ dài ký tự tối đa của cột trong CSDL là 200 ký tự.
        public string? ProposedActionNote { get; set; } // Thuộc tính ProposedActionNote kiểu dữ liệu string? lưu trữ thông tin thực thể.

        /// <summary>
        /// true nếu serial được nghiệp vụ tự động giải quyết trong cửa sổ kiểm kê
        /// (vd: serial Missing nhưng thực ra đã được bán trong thời gian kiểm kê).
        /// </summary>
        public bool ResolvedDuringApproval { get; set; } // Thuộc tính ResolvedDuringApproval kiểu dữ liệu bool lưu trữ thông tin thực thể.

        [ForeignKey("CheckId")] // Chỉ định thuộc tính điều hướng tương ứng sử dụng khóa ngoại 'CheckId'.
        public virtual InventoryCheck Check { get; set; } = null!; // Thuộc tính quan hệ điều hướng ảo (virtual navigation property) tham chiếu tới thực thể InventoryCheck.

        [ForeignKey("DetailId")] // Chỉ định thuộc tính điều hướng tương ứng sử dụng khóa ngoại 'DetailId'.
        public virtual InventoryCheckDetail? Detail { get; set; } // Thuộc tính quan hệ điều hướng ảo (virtual navigation property) tham chiếu tới thực thể InventoryCheckDetail?.

        [ForeignKey("VariantId")] // Chỉ định thuộc tính điều hướng tương ứng sử dụng khóa ngoại 'VariantId'.
        public virtual ProductVariant? Variant { get; set; } // Thuộc tính quan hệ điều hướng ảo (virtual navigation property) tham chiếu tới thực thể ProductVariant?.

        [ForeignKey("SerialId")] // Chỉ định thuộc tính điều hướng tương ứng sử dụng khóa ngoại 'SerialId'.
        public virtual ProductSerial? Serial { get; set; } // Thuộc tính quan hệ điều hướng ảo (virtual navigation property) tham chiếu tới thực thể ProductSerial?.
    }
}

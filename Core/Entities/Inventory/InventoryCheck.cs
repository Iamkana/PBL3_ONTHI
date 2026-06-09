using System.ComponentModel.DataAnnotations; // Nhập thư viện hỗ trợ kiểm tra ràng buộc dữ liệu (DataAnnotations).
using System.ComponentModel.DataAnnotations.Schema; // Nhập thư viện hỗ trợ cấu hình ánh xạ thực thể (Schema).

namespace PBL3.Core.Entities // Định nghĩa namespace PBL3.Core.Entities để quản lý thực thể/giao diện.
{
    // 5. InventoryChecks
    [Table("InventoryChecks")] // Ánh xạ thực thể hiện tại với bảng dữ liệu 'InventoryChecks' trong cơ sở dữ liệu.
    public class InventoryCheck // Định nghĩa thực thể/lớp nghiệp vụ InventoryCheck.
    {
        [Key] // Đánh dấu thuộc tính ngay phía dưới là Khóa chính (Primary Key).
        public int Id { get; set; } // Thuộc tính Id kiểu int đóng vai trò khóa chính định danh bản ghi.

        [Required] // Đánh dấu thuộc tính phía dưới là bắt buộc nhập (NOT NULL).
        [MaxLength(20)] // Ràng buộc độ dài ký tự tối đa của cột trong CSDL là 20 ký tự.
        public string CheckCode { get; set; } = string.Empty; // Thuộc tính CheckCode kiểu dữ liệu string lưu trữ thông tin thực thể với giá trị mặc định là string.Empty.

        public Guid EmployeeId { get; set; } // Thuộc tính khóa ngoại EmployeeId kiểu Guid liên kết sang thực thể liên quan.

        public DateTime CheckDate { get; set; } = DateTime.UtcNow; // Thuộc tính CheckDate kiểu dữ liệu DateTime lưu trữ thông tin thực thể với giá trị mặc định là DateTime.UtcNow.

        /// <summary>Thời điểm chốt snapshot chính xác (= CheckDate khi tạo phiếu).</summary>
        public DateTime SnapshotAt { get; set; } = DateTime.UtcNow; // Thuộc tính SnapshotAt kiểu dữ liệu DateTime lưu trữ thông tin thực thể với giá trị mặc định là DateTime.UtcNow.

        [MaxLength(500)] // Ràng buộc độ dài ký tự tối đa của cột trong CSDL là 500 ký tự.
        public string? Note { get; set; } // Thuộc tính Note kiểu dữ liệu string? lưu trữ thông tin thực thể.

        // 0: Draft, 1: AwaitingApproval, 2: Completed, 3: Cancelled
        public byte Status { get; set; } // Thuộc tính Status kiểu dữ liệu byte lưu trữ thông tin thực thể.

        // 0: AllStore, 1: Category
        public byte ScopeType { get; set; } // Thuộc tính ScopeType kiểu dữ liệu byte lưu trữ thông tin thực thể.

        public int? ScopeCategoryId { get; set; } // Thuộc tính khóa ngoại ScopeCategoryId kiểu int? liên kết sang thực thể liên quan.

        public Guid? ApprovedByEmployeeId { get; set; } // Thuộc tính ApprovedByEmployeeId kiểu dữ liệu Guid? lưu trữ thông tin thực thể.
        public DateTime? ApprovedAt { get; set; } // Thuộc tính ApprovedAt kiểu dữ liệu DateTime? lưu trữ thông tin thực thể.

        [MaxLength(500)] // Ràng buộc độ dài ký tự tối đa của cột trong CSDL là 500 ký tự.
        public string? RejectReason { get; set; } // Thuộc tính RejectReason kiểu dữ liệu string? lưu trữ thông tin thực thể.

        public bool IsDeleted { get; set; } // Thuộc tính IsDeleted đánh dấu bản ghi đã bị xóa mềm (soft deleted).

        [ForeignKey("ScopeCategoryId")] // Chỉ định thuộc tính điều hướng tương ứng sử dụng khóa ngoại 'ScopeCategoryId'.
        public virtual Category? ScopeCategory { get; set; } // Thuộc tính quan hệ điều hướng ảo (virtual navigation property) tham chiếu tới thực thể Category?.

        public virtual ICollection<InventoryCheckDetail> Details { get; set; } = new List<InventoryCheckDetail>(); // Thuộc tính quan hệ điều hướng (navigation collection) danh sách các InventoryCheckDetail liên quan.
        public virtual ICollection<InventoryCheckDetailSerial> DetailSerials { get; set; } = new List<InventoryCheckDetailSerial>(); // Thuộc tính quan hệ điều hướng (navigation collection) danh sách các InventoryCheckDetailSerial liên quan.
    }
}

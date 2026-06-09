using System.ComponentModel.DataAnnotations; // Nhập thư viện hỗ trợ kiểm tra ràng buộc dữ liệu (DataAnnotations).
using System.ComponentModel.DataAnnotations.Schema; // Nhập thư viện hỗ trợ cấu hình ánh xạ thực thể (Schema).

namespace PBL3.Core.Entities // Định nghĩa namespace PBL3.Core.Entities để quản lý thực thể/giao diện.
{
    // 1. Vouchers
    [Table("Vouchers")] // Ánh xạ thực thể hiện tại với bảng dữ liệu 'Vouchers' trong cơ sở dữ liệu.
    public class Voucher // Định nghĩa thực thể/lớp nghiệp vụ Voucher.
    {
        [Key] // Đánh dấu thuộc tính ngay phía dưới là Khóa chính (Primary Key).
        public int Id { get; set; } // Thuộc tính Id kiểu int đóng vai trò khóa chính định danh bản ghi.
        [Required] // Đánh dấu thuộc tính phía dưới là bắt buộc nhập (NOT NULL).
        [MaxLength(50)] // Ràng buộc độ dài ký tự tối đa của cột trong CSDL là 50 ký tự.
        public string Code { get; set; } = string.Empty; // Thuộc tính Code kiểu dữ liệu string lưu trữ thông tin thực thể với giá trị mặc định là string.Empty.
        [Required] // Đánh dấu thuộc tính phía dưới là bắt buộc nhập (NOT NULL).
        [MaxLength(200)] // Ràng buộc độ dài ký tự tối đa của cột trong CSDL là 200 ký tự.
        public string Name { get; set; } = string.Empty; // Thuộc tính Name kiểu dữ liệu string lưu trữ thông tin thực thể với giá trị mặc định là string.Empty.

        public byte DiscountType { get; set; } // Thuộc tính DiscountType kiểu dữ liệu byte lưu trữ thông tin thực thể.
        public decimal DiscountValue { get; set; } // Thuộc tính DiscountValue kiểu dữ liệu decimal lưu trữ thông tin thực thể.

        public decimal MinOrderValue { get; set; } // Thuộc tính MinOrderValue kiểu dữ liệu decimal lưu trữ thông tin thực thể.
        public decimal? MaxDiscountAmount { get; set; } // Thuộc tính MaxDiscountAmount kiểu dữ liệu decimal? lưu trữ thông tin thực thể.

        public DateTime StartDate { get; set; } // Thuộc tính StartDate kiểu dữ liệu DateTime lưu trữ thông tin thực thể.
        public DateTime EndDate { get; set; } // Thuộc tính EndDate kiểu dữ liệu DateTime lưu trữ thông tin thực thể.

        public int? Quantity { get; set; } // Thuộc tính Quantity kiểu dữ liệu int? lưu trữ thông tin thực thể.
        public int UsedCount { get; set; } // Thuộc tính UsedCount kiểu dữ liệu int lưu trữ thông tin thực thể.

        public bool IsActive { get; set; } = true; // Thuộc tính IsActive kiểu dữ liệu bool lưu trữ thông tin thực thể với giá trị mặc định là true.

        // Quản lý nâng cao
        public int? MaxUsesPerUser { get; set; } // Thuộc tính MaxUsesPerUser kiểu dữ liệu int? lưu trữ thông tin thực thể.
        public byte ApplyFor { get; set; } // Thuộc tính ApplyFor kiểu dữ liệu byte lưu trữ thông tin thực thể.
        public bool IsStackable { get; set; } // Thuộc tính IsStackable kiểu dữ liệu bool lưu trữ thông tin thực thể.
        [MaxLength(500)] // Ràng buộc độ dài ký tự tối đa của cột trong CSDL là 500 ký tự.
        public string? Description { get; set; } // Thuộc tính Description kiểu dữ liệu string? lưu trữ thông tin thực thể.

        // Soft delete & audit
        public DateTime CreatedDate { get; set; } // Thuộc tính CreatedDate lưu thời điểm bản ghi được tạo lập ban đầu.
        public bool IsDeleted { get; set; } // Thuộc tính IsDeleted đánh dấu bản ghi đã bị xóa mềm (soft deleted).
        public DateTime? DeletedDate { get; set; } // Thuộc tính DeletedDate lưu thời điểm bản ghi bị xóa mềm khỏi hệ thống.

        public virtual ICollection<VoucherUsage> VoucherUsages { get; set; } = new List<VoucherUsage>(); // Thuộc tính quan hệ điều hướng (navigation collection) danh sách các VoucherUsage liên quan.
        public virtual ICollection<VoucherCategory> VoucherCategories { get; set; } = new List<VoucherCategory>(); // Thuộc tính quan hệ điều hướng (navigation collection) danh sách các VoucherCategory liên quan.
    }
}

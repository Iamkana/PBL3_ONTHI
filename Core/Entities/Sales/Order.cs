using System.ComponentModel.DataAnnotations; // Nhập thư viện hỗ trợ kiểm tra ràng buộc dữ liệu (DataAnnotations).
using System.ComponentModel.DataAnnotations.Schema; // Nhập thư viện hỗ trợ cấu hình ánh xạ thực thể (Schema).

namespace PBL3.Core.Entities // Định nghĩa namespace PBL3.Core.Entities để quản lý thực thể/giao diện.
{
    // 2. Orders
    [Table("Orders")] // Ánh xạ thực thể hiện tại với bảng dữ liệu 'Orders' trong cơ sở dữ liệu.
    public class Order // Định nghĩa thực thể/lớp nghiệp vụ Order.
    {
        [Key] // Đánh dấu thuộc tính ngay phía dưới là Khóa chính (Primary Key).
        public int Id { get; set; } // Thuộc tính Id kiểu int đóng vai trò khóa chính định danh bản ghi.
        [Required] // Đánh dấu thuộc tính phía dưới là bắt buộc nhập (NOT NULL).
        [MaxLength(20)] // Ràng buộc độ dài ký tự tối đa của cột trong CSDL là 20 ký tự.
        public string OrderCode { get; set; } = string.Empty; // Thuộc tính OrderCode kiểu dữ liệu string lưu trữ thông tin thực thể với giá trị mặc định là string.Empty.

        public Guid? UserId { get; set; } // Thuộc tính UserId kiểu dữ liệu Guid? lưu trữ thông tin thực thể.
        public Guid? EmployeeId { get; set; } // Thuộc tính EmployeeId kiểu dữ liệu Guid? lưu trữ thông tin thực thể.

        public DateTime OrderDate { get; set; } = DateTime.UtcNow; // Thuộc tính OrderDate kiểu dữ liệu DateTime lưu trữ thông tin thực thể với giá trị mặc định là DateTime.UtcNow.

        public byte Status { get; set; } // Thuộc tính Status kiểu dữ liệu byte lưu trữ thông tin thực thể.

        [Required] // Đánh dấu thuộc tính phía dưới là bắt buộc nhập (NOT NULL).
        [MaxLength(100)] // Ràng buộc độ dài ký tự tối đa của cột trong CSDL là 100 ký tự.
        public string ShipName { get; set; } = string.Empty; // Thuộc tính ShipName kiểu dữ liệu string lưu trữ thông tin thực thể với giá trị mặc định là string.Empty.
        [Required] // Đánh dấu thuộc tính phía dưới là bắt buộc nhập (NOT NULL).
        [MaxLength(20)] // Ràng buộc độ dài ký tự tối đa của cột trong CSDL là 20 ký tự.
        public string ShipPhone { get; set; } = string.Empty; // Thuộc tính ShipPhone kiểu dữ liệu string lưu trữ thông tin thực thể với giá trị mặc định là string.Empty.
        [Required] // Đánh dấu thuộc tính phía dưới là bắt buộc nhập (NOT NULL).
        [MaxLength(255)] // Ràng buộc độ dài ký tự tối đa của cột trong CSDL là 255 ký tự.
        public string ShipAddress { get; set; } = string.Empty; // Thuộc tính ShipAddress kiểu dữ liệu string lưu trữ thông tin thực thể với giá trị mặc định là string.Empty.
        [Required] // Đánh dấu thuộc tính phía dưới là bắt buộc nhập (NOT NULL).
        [MaxLength(100)] // Ràng buộc độ dài ký tự tối đa của cột trong CSDL là 100 ký tự.
        public string ShipCity { get; set; } = string.Empty; // Thuộc tính ShipCity kiểu dữ liệu string lưu trữ thông tin thực thể với giá trị mặc định là string.Empty.

        public decimal SubTotal { get; set; } // Thuộc tính SubTotal kiểu dữ liệu decimal lưu trữ thông tin thực thể.
        public decimal ShippingFee { get; set; } // Thuộc tính ShippingFee kiểu dữ liệu decimal lưu trữ thông tin thực thể.

        public decimal DiscountAmount { get; set; } // Thuộc tính DiscountAmount kiểu dữ liệu decimal lưu trữ thông tin thực thể.

        public decimal TotalAmount { get; set; } // Thuộc tính TotalAmount kiểu dữ liệu decimal lưu trữ thông tin thực thể.

        public byte PaymentMethod { get; set; } // Thuộc tính PaymentMethod kiểu dữ liệu byte lưu trữ thông tin thực thể.
        public byte PaymentStatus { get; set; } // Thuộc tính PaymentStatus kiểu dữ liệu byte lưu trữ thông tin thực thể.

        public byte OrderType { get; set; } // Thuộc tính OrderType kiểu dữ liệu byte lưu trữ thông tin thực thể.

        [MaxLength(500)] // Ràng buộc độ dài ký tự tối đa của cột trong CSDL là 500 ký tự.
        public string? Note { get; set; } // Thuộc tính Note kiểu dữ liệu string? lưu trữ thông tin thực thể.
        [MaxLength(255)] // Ràng buộc độ dài ký tự tối đa của cột trong CSDL là 255 ký tự.
        public string? CancelReason { get; set; } // Thuộc tính CancelReason kiểu dữ liệu string? lưu trữ thông tin thực thể.


        [ForeignKey("UserId")] // Chỉ định thuộc tính điều hướng tương ứng sử dụng khóa ngoại 'UserId'.
        public virtual AppUser? User { get; set; } // Thuộc tính quan hệ điều hướng ảo (virtual navigation property) tham chiếu tới thực thể AppUser?.
        // EmployeeId fk to AppUser

        /// <summary>
        /// Danh sách voucher đã áp dụng cho đơn hàng này.
        /// </summary>
        public virtual ICollection<VoucherUsage> VoucherUsages { get; set; } = new List<VoucherUsage>(); // Thuộc tính quan hệ điều hướng (navigation collection) danh sách các VoucherUsage liên quan.

        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>(); // Thuộc tính quan hệ điều hướng (navigation collection) danh sách các OrderDetail liên quan.
    }
}

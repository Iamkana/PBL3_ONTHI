namespace PBL3.Shared.DTOs.Vouchers // Định nghĩa namespace PBL3.Shared.DTOs.Vouchers quản lý cấu trúc code truyền tải và validator.
{
    /// <summary>Voucher kèm thông tin có thể áp dụng hay không cho đơn hàng cụ thể.</summary>
    public class VoucherAvailabilityDto // Định nghĩa lớp DTO truyền tải dữ liệu VoucherAvailabilityDto.
    {
        public int Id { get; set; } // Thuộc tính Id kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public string Code { get; set; } = string.Empty; // Thuộc tính Code kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public string Name { get; set; } = string.Empty; // Thuộc tính Name kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public string? Description { get; set; } // Thuộc tính Description kiểu dữ liệu string? lưu trữ thông tin truyền tải.
        public byte DiscountType { get; set; } // Thuộc tính DiscountType kiểu dữ liệu byte lưu trữ thông tin truyền tải.
        public decimal DiscountValue { get; set; } // Thuộc tính DiscountValue kiểu dữ liệu decimal lưu trữ thông tin truyền tải.
        public decimal? MaxDiscountAmount { get; set; } // Thuộc tính MaxDiscountAmount kiểu dữ liệu decimal? lưu trữ thông tin truyền tải.
        public decimal MinOrderValue { get; set; } // Thuộc tính MinOrderValue kiểu dữ liệu decimal lưu trữ thông tin truyền tải.
        public DateTime StartDate { get; set; } // Thuộc tính StartDate kiểu dữ liệu DateTime lưu trữ thông tin truyền tải.
        public DateTime EndDate { get; set; } // Thuộc tính EndDate kiểu dữ liệu DateTime lưu trữ thông tin truyền tải.
        public bool IsStackable { get; set; } // Thuộc tính IsStackable kiểu dữ liệu bool lưu trữ thông tin truyền tải.
        public bool IsApplicable { get; set; } // Thuộc tính IsApplicable kiểu dữ liệu bool lưu trữ thông tin truyền tải.
        public decimal EstimatedDiscount { get; set; } // Thuộc tính EstimatedDiscount kiểu dữ liệu decimal lưu trữ thông tin truyền tải.
        public string? NotApplicableReason { get; set; } // Thuộc tính NotApplicableReason kiểu dữ liệu string? lưu trữ thông tin truyền tải.
    }
}

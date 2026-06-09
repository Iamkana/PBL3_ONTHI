namespace PBL3.Shared.DTOs.Vouchers // Định nghĩa namespace PBL3.Shared.DTOs.Vouchers quản lý cấu trúc code truyền tải và validator.
{
    /// <summary>
    /// DTO hiển thị đầy đủ thông tin voucher (dùng cho trang chi tiết và danh sách quản trị).
    /// </summary>
    public class VoucherDto // Định nghĩa lớp DTO truyền tải dữ liệu VoucherDto.
    {
        public int Id { get; set; } // Thuộc tính Id kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public string Code { get; set; } = string.Empty; // Thuộc tính Code kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public string Name { get; set; } = string.Empty; // Thuộc tính Name kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public byte DiscountType { get; set; } // Thuộc tính DiscountType kiểu dữ liệu byte lưu trữ thông tin truyền tải.
        public decimal DiscountValue { get; set; } // Thuộc tính DiscountValue kiểu dữ liệu decimal lưu trữ thông tin truyền tải.
        public decimal MinOrderValue { get; set; } // Thuộc tính MinOrderValue kiểu dữ liệu decimal lưu trữ thông tin truyền tải.
        public decimal? MaxDiscountAmount { get; set; } // Thuộc tính MaxDiscountAmount kiểu dữ liệu decimal? lưu trữ thông tin truyền tải.
        public DateTime StartDate { get; set; } // Thuộc tính StartDate kiểu dữ liệu DateTime lưu trữ thông tin truyền tải.
        public DateTime EndDate { get; set; } // Thuộc tính EndDate kiểu dữ liệu DateTime lưu trữ thông tin truyền tải.
        public int? Quantity { get; set; } // Thuộc tính Quantity kiểu dữ liệu int? lưu trữ thông tin truyền tải.
        public int UsedCount { get; set; } // Thuộc tính UsedCount kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public int? MaxUsesPerUser { get; set; } // Thuộc tính MaxUsesPerUser kiểu dữ liệu int? lưu trữ thông tin truyền tải.
        public byte ApplyFor { get; set; } // Thuộc tính ApplyFor kiểu dữ liệu byte lưu trữ thông tin truyền tải.
        public bool IsStackable { get; set; } // Thuộc tính IsStackable kiểu dữ liệu bool lưu trữ thông tin truyền tải.
        public bool IsActive { get; set; } // Thuộc tính IsActive kiểu dữ liệu bool lưu trữ thông tin truyền tải.
        public string? Description { get; set; } // Thuộc tính Description kiểu dữ liệu string? lưu trữ thông tin truyền tải.
        public DateTime CreatedDate { get; set; } // Thuộc tính CreatedDate kiểu dữ liệu DateTime lưu trữ thông tin truyền tải.
        public List<int> CategoryIds { get; set; } = new(); // Thuộc tính CategoryIds kiểu dữ liệu List<int> lưu trữ thông tin truyền tải với giá trị mặc định là new().
    }
}

namespace PBL3.Shared.DTOs.Vouchers // Định nghĩa namespace PBL3.Shared.DTOs.Vouchers quản lý cấu trúc code truyền tải và validator.
{
    /// <summary>
    /// DTO rút gọn dùng cho Dropdown / danh sách nhanh.
    /// </summary>
    public class VoucherSummaryDto // Định nghĩa lớp DTO truyền tải dữ liệu VoucherSummaryDto.
    {
        public int Id { get; set; } // Thuộc tính Id kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public string Code { get; set; } = string.Empty; // Thuộc tính Code kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public string Name { get; set; } = string.Empty; // Thuộc tính Name kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public byte DiscountType { get; set; } // Thuộc tính DiscountType kiểu dữ liệu byte lưu trữ thông tin truyền tải.
        public decimal DiscountValue { get; set; } // Thuộc tính DiscountValue kiểu dữ liệu decimal lưu trữ thông tin truyền tải.
        public decimal? MaxDiscountAmount { get; set; } // Thuộc tính MaxDiscountAmount kiểu dữ liệu decimal? lưu trữ thông tin truyền tải.
        public bool IsActive { get; set; } // Thuộc tính IsActive kiểu dữ liệu bool lưu trữ thông tin truyền tải.
        public DateTime EndDate { get; set; } // Thuộc tính EndDate kiểu dữ liệu DateTime lưu trữ thông tin truyền tải.
    }
}

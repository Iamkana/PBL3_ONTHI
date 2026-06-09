namespace PBL3.Shared.DTOs.Vouchers // Định nghĩa namespace PBL3.Shared.DTOs.Vouchers quản lý cấu trúc code truyền tải và validator.
{
    /// <summary>
    /// Kết quả kiểm tra voucher — trả về discount preview hoặc thông báo lỗi.
    /// </summary>
    public class ValidateVoucherResponse // Định nghĩa lớp DTO truyền tải dữ liệu ValidateVoucherResponse.
    {
        public bool IsValid { get; set; } // Thuộc tính IsValid kiểu dữ liệu bool lưu trữ thông tin truyền tải.
        public string? ErrorMessage { get; set; } // Thuộc tính ErrorMessage kiểu dữ liệu string? lưu trữ thông tin truyền tải.
        public decimal DiscountAmount { get; set; } // Thuộc tính DiscountAmount kiểu dữ liệu decimal lưu trữ thông tin truyền tải.
        public string? VoucherName { get; set; } // Thuộc tính VoucherName kiểu dữ liệu string? lưu trữ thông tin truyền tải.
        public string? Code { get; set; } // Thuộc tính Code kiểu dữ liệu string? lưu trữ thông tin truyền tải.
    }
}

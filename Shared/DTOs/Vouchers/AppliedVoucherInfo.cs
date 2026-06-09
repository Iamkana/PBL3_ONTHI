namespace PBL3.Shared.DTOs.Vouchers // Định nghĩa namespace PBL3.Shared.DTOs.Vouchers quản lý cấu trúc code truyền tải và validator.
{
    /// <summary>Voucher đã được chọn để áp dụng vào đơn hàng.</summary>
    public class AppliedVoucherInfo // Định nghĩa lớp DTO truyền tải dữ liệu AppliedVoucherInfo.
    {
        public string Code { get; set; } = string.Empty; // Thuộc tính Code kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public decimal DiscountAmount { get; set; } // Thuộc tính DiscountAmount kiểu dữ liệu decimal lưu trữ thông tin truyền tải.
    }
}

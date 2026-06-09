namespace PBL3.Shared.DTOs.Vouchers // Định nghĩa namespace PBL3.Shared.DTOs.Vouchers quản lý cấu trúc code truyền tải và validator.
{
    /// <summary>
    /// Request kiểm tra tính hợp lệ của voucher trước khi áp dụng vào đơn hàng.
    /// </summary>
    public class ValidateVoucherRequest // Định nghĩa lớp DTO truyền tải dữ liệu ValidateVoucherRequest.
    {
        public string Code { get; set; } = string.Empty; // Thuộc tính Code kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public decimal SubTotal { get; set; } // Thuộc tính SubTotal kiểu dữ liệu decimal lưu trữ thông tin truyền tải.
        public List<int>? OrderItemCategoryIds { get; set; } // Thuộc tính OrderItemCategoryIds kiểu dữ liệu List<int>? lưu trữ thông tin truyền tải.
        public bool IsOnlineOrder { get; set; } = true; // Thuộc tính IsOnlineOrder kiểu dữ liệu bool lưu trữ thông tin truyền tải với giá trị mặc định là true.
    }
}

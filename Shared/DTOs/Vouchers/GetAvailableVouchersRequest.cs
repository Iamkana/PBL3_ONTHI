namespace PBL3.Shared.DTOs.Vouchers // Định nghĩa namespace PBL3.Shared.DTOs.Vouchers quản lý cấu trúc code truyền tải và validator.
{
    /// <summary>Request lấy danh sách voucher có thể áp dụng cho đơn hàng hiện tại.</summary>
    public class GetAvailableVouchersRequest // Định nghĩa lớp DTO truyền tải dữ liệu GetAvailableVouchersRequest.
    {
        public decimal SubTotal { get; set; } // Thuộc tính SubTotal kiểu dữ liệu decimal lưu trữ thông tin truyền tải.
        public bool IsOnlineOrder { get; set; } = true; // Thuộc tính IsOnlineOrder kiểu dữ liệu bool lưu trữ thông tin truyền tải với giá trị mặc định là true.
    }
}

namespace PBL3.Shared.DTOs.ServiceTickets // Định nghĩa namespace PBL3.Shared.DTOs.ServiceTickets quản lý cấu trúc code truyền tải và validator.
{
    public class QuotationItemCreateDto // Định nghĩa lớp DTO truyền tải dữ liệu QuotationItemCreateDto.
    {
        public int? VariantId { get; set; } // Thuộc tính VariantId kiểu dữ liệu int? lưu trữ thông tin truyền tải.
        public string Description { get; set; } = string.Empty; // Thuộc tính Description kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public int Quantity { get; set; } = 1; // Thuộc tính Quantity kiểu dữ liệu int lưu trữ thông tin truyền tải với giá trị mặc định là 1.
        public decimal UnitPrice { get; set; } // Thuộc tính UnitPrice kiểu dữ liệu decimal lưu trữ thông tin truyền tải.
    }
}

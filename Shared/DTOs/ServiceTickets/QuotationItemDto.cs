namespace PBL3.Shared.DTOs.ServiceTickets // Định nghĩa namespace PBL3.Shared.DTOs.ServiceTickets quản lý cấu trúc code truyền tải và validator.
{
    public class QuotationItemDto // Định nghĩa lớp DTO truyền tải dữ liệu QuotationItemDto.
    {
        public int Id { get; set; } // Thuộc tính Id kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public string Description { get; set; } = string.Empty; // Thuộc tính Description kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public int Quantity { get; set; } // Thuộc tính Quantity kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public decimal UnitPrice { get; set; } // Thuộc tính UnitPrice kiểu dữ liệu decimal lưu trữ thông tin truyền tải.
        public decimal LineTotal { get; set; } // Thuộc tính LineTotal kiểu dữ liệu decimal lưu trữ thông tin truyền tải.
    }
}

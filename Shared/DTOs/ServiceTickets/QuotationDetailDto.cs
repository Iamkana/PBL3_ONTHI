namespace PBL3.Shared.DTOs.ServiceTickets // Định nghĩa namespace PBL3.Shared.DTOs.ServiceTickets quản lý cấu trúc code truyền tải và validator.
{
    public class QuotationDetailDto // Định nghĩa lớp DTO truyền tải dữ liệu QuotationDetailDto.
    {
        public int Id { get; set; } // Thuộc tính Id kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public int TicketId { get; set; } // Thuộc tính TicketId kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public DateTime IssuedDate { get; set; } // Thuộc tính IssuedDate kiểu dữ liệu DateTime lưu trữ thông tin truyền tải.

        public decimal LaborCost { get; set; } // Thuộc tính LaborCost kiểu dữ liệu decimal lưu trữ thông tin truyền tải.
        public decimal PartsTotal { get; set; } // Thuộc tính PartsTotal kiểu dữ liệu decimal lưu trữ thông tin truyền tải.
        public decimal GrandTotal { get; set; } // Thuộc tính GrandTotal kiểu dữ liệu decimal lưu trữ thông tin truyền tải.

        public byte Status { get; set; } // Thuộc tính Status kiểu dữ liệu byte lưu trữ thông tin truyền tải.
        public string StatusLabel { get; set; } = string.Empty; // Thuộc tính StatusLabel kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.

        public DateTime? CustomerDecidedAt { get; set; } // Thuộc tính CustomerDecidedAt kiểu dữ liệu DateTime? lưu trữ thông tin truyền tải.
        public string? CustomerDecisionNote { get; set; } // Thuộc tính CustomerDecisionNote kiểu dữ liệu string? lưu trữ thông tin truyền tải.

        public List<QuotationItemDto> Items { get; set; } = new(); // Thuộc tính Items kiểu dữ liệu List<QuotationItemDto> lưu trữ thông tin truyền tải với giá trị mặc định là new().
    }
}

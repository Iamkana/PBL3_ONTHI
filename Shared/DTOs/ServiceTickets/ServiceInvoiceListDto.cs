namespace PBL3.Shared.DTOs.ServiceTickets // Định nghĩa namespace PBL3.Shared.DTOs.ServiceTickets quản lý cấu trúc code truyền tải và validator.
{
    public class ServiceInvoiceListDto // Định nghĩa lớp DTO truyền tải dữ liệu ServiceInvoiceListDto.
    {
        public int Id { get; set; } // Thuộc tính Id kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public string InvoiceCode { get; set; } = string.Empty; // Thuộc tính InvoiceCode kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public int TicketId { get; set; } // Thuộc tính TicketId kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public DateTime IssuedDate { get; set; } // Thuộc tính IssuedDate kiểu dữ liệu DateTime lưu trữ thông tin truyền tải.
        public decimal GrandTotal { get; set; } // Thuộc tính GrandTotal kiểu dữ liệu decimal lưu trữ thông tin truyền tải.
        public byte PaymentStatus { get; set; } // Thuộc tính PaymentStatus kiểu dữ liệu byte lưu trữ thông tin truyền tải.
        public string PaymentStatusLabel { get; set; } = string.Empty; // Thuộc tính PaymentStatusLabel kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
    }
}

namespace PBL3.Shared.DTOs.ServiceTickets // Định nghĩa namespace PBL3.Shared.DTOs.ServiceTickets quản lý cấu trúc code truyền tải và validator.
{
    public class ServiceInvoiceDetailDto // Định nghĩa lớp DTO truyền tải dữ liệu ServiceInvoiceDetailDto.
    {
        public int Id { get; set; } // Thuộc tính Id kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public string InvoiceCode { get; set; } = string.Empty; // Thuộc tính InvoiceCode kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public int TicketId { get; set; } // Thuộc tính TicketId kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public int? QuotationId { get; set; } // Thuộc tính QuotationId kiểu dữ liệu int? lưu trữ thông tin truyền tải.
        public Guid? IssuedByEmployeeId { get; set; } // Thuộc tính IssuedByEmployeeId kiểu dữ liệu Guid? lưu trữ thông tin truyền tải.

        public DateTime IssuedDate { get; set; } // Thuộc tính IssuedDate kiểu dữ liệu DateTime lưu trữ thông tin truyền tải.

        public decimal LaborCost { get; set; } // Thuộc tính LaborCost kiểu dữ liệu decimal lưu trữ thông tin truyền tải.
        public decimal PartsTotal { get; set; } // Thuộc tính PartsTotal kiểu dữ liệu decimal lưu trữ thông tin truyền tải.
        public decimal GrandTotal { get; set; } // Thuộc tính GrandTotal kiểu dữ liệu decimal lưu trữ thông tin truyền tải.

        public byte PaymentMethod { get; set; } // Thuộc tính PaymentMethod kiểu dữ liệu byte lưu trữ thông tin truyền tải.
        public string PaymentMethodLabel { get; set; } = string.Empty; // Thuộc tính PaymentMethodLabel kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.

        public byte PaymentStatus { get; set; } // Thuộc tính PaymentStatus kiểu dữ liệu byte lưu trữ thông tin truyền tải.
        public string PaymentStatusLabel { get; set; } = string.Empty; // Thuộc tính PaymentStatusLabel kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.

        public string? Note { get; set; } // Thuộc tính Note kiểu dữ liệu string? lưu trữ thông tin truyền tải.

        public List<ServiceInvoiceItemDto> Items { get; set; } = new(); // Thuộc tính Items kiểu dữ liệu List<ServiceInvoiceItemDto> lưu trữ thông tin truyền tải với giá trị mặc định là new().
    }
}

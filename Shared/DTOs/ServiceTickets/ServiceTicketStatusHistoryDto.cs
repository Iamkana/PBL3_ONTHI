namespace PBL3.Shared.DTOs.ServiceTickets // Định nghĩa namespace PBL3.Shared.DTOs.ServiceTickets quản lý cấu trúc code truyền tải và validator.
{
    public class ServiceTicketStatusHistoryDto // Định nghĩa lớp DTO truyền tải dữ liệu ServiceTicketStatusHistoryDto.
    {
        public int Id { get; set; } // Thuộc tính Id kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public byte FromStatus { get; set; } // Thuộc tính FromStatus kiểu dữ liệu byte lưu trữ thông tin truyền tải.
        public string FromStatusLabel { get; set; } = string.Empty; // Thuộc tính FromStatusLabel kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public byte ToStatus { get; set; } // Thuộc tính ToStatus kiểu dữ liệu byte lưu trữ thông tin truyền tải.
        public string ToStatusLabel { get; set; } = string.Empty; // Thuộc tính ToStatusLabel kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public DateTime ChangedAt { get; set; } // Thuộc tính ChangedAt kiểu dữ liệu DateTime lưu trữ thông tin truyền tải.
        public string? Note { get; set; } // Thuộc tính Note kiểu dữ liệu string? lưu trữ thông tin truyền tải.
    }
}

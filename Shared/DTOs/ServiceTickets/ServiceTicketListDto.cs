namespace PBL3.Shared.DTOs.ServiceTickets // Định nghĩa namespace PBL3.Shared.DTOs.ServiceTickets quản lý cấu trúc code truyền tải và validator.
{
    public class ServiceTicketListDto // Định nghĩa lớp DTO truyền tải dữ liệu ServiceTicketListDto.
    {
        public int Id { get; set; } // Thuộc tính Id kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public string TicketCode { get; set; } = string.Empty; // Thuộc tính TicketCode kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public string SerialNumber { get; set; } = string.Empty; // Thuộc tính SerialNumber kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public string ProductName { get; set; } = string.Empty; // Thuộc tính ProductName kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public string? CustomerName { get; set; } // Thuộc tính CustomerName kiểu dữ liệu string? lưu trữ thông tin truyền tải.
        public DateTime IntakeDate { get; set; } // Thuộc tính IntakeDate kiểu dữ liệu DateTime lưu trữ thông tin truyền tải.
        public byte Status { get; set; } // Thuộc tính Status kiểu dữ liệu byte lưu trữ thông tin truyền tải.
        public string StatusLabel { get; set; } = string.Empty; // Thuộc tính StatusLabel kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public byte ResolutionType { get; set; } // Thuộc tính ResolutionType kiểu dữ liệu byte lưu trữ thông tin truyền tải.
    }
}

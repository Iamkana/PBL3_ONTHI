namespace PBL3.Shared.DTOs.ServiceTickets // Định nghĩa namespace PBL3.Shared.DTOs.ServiceTickets quản lý cấu trúc code truyền tải và validator.
{
    public class SerialRepairHistoryDto // Định nghĩa lớp DTO truyền tải dữ liệu SerialRepairHistoryDto.
    {
        public int Id { get; set; } // Thuộc tính Id kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public int SerialId { get; set; } // Thuộc tính SerialId kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public int? TicketId { get; set; } // Thuộc tính TicketId kiểu dữ liệu int? lưu trữ thông tin truyền tải.
        public string? TicketCode { get; set; } // Thuộc tính TicketCode kiểu dữ liệu string? lưu trữ thông tin truyền tải.

        public byte ResolutionType { get; set; } // Thuộc tính ResolutionType kiểu dữ liệu byte lưu trữ thông tin truyền tải.
        public string ResolutionTypeLabel { get; set; } = string.Empty; // Thuộc tính ResolutionTypeLabel kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.

        public DateTime LoggedAt { get; set; } // Thuộc tính LoggedAt kiểu dữ liệu DateTime lưu trữ thông tin truyền tải.
        public string Summary { get; set; } = string.Empty; // Thuộc tính Summary kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.

        public int? ReplacedBySerialId { get; set; } // Thuộc tính ReplacedBySerialId kiểu dữ liệu int? lưu trữ thông tin truyền tải.
        public string? ReplacedBySerialNumber { get; set; } // Thuộc tính ReplacedBySerialNumber kiểu dữ liệu string? lưu trữ thông tin truyền tải.
    }
}

namespace PBL3.Shared.DTOs.ServiceTickets // Định nghĩa namespace PBL3.Shared.DTOs.ServiceTickets quản lý cấu trúc code truyền tải và validator.
{
    public class RmaShipmentDetailDto // Định nghĩa lớp DTO truyền tải dữ liệu RmaShipmentDetailDto.
    {
        public int Id { get; set; } // Thuộc tính Id kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public int TicketId { get; set; } // Thuộc tính TicketId kiểu dữ liệu int lưu trữ thông tin truyền tải.

        public string CarrierName { get; set; } = string.Empty; // Thuộc tính CarrierName kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public string TrackingCode { get; set; } = string.Empty; // Thuộc tính TrackingCode kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.

        public DateTime? ShippedDate { get; set; } // Thuộc tính ShippedDate kiểu dữ liệu DateTime? lưu trữ thông tin truyền tải.

        public DateTime? ReceivedBackDate { get; set; } // Thuộc tính ReceivedBackDate kiểu dữ liệu DateTime? lưu trữ thông tin truyền tải.
        public byte ManufacturerResolution { get; set; } // Thuộc tính ManufacturerResolution kiểu dữ liệu byte lưu trữ thông tin truyền tải.
        public string ManufacturerResolutionLabel { get; set; } = string.Empty; // Thuộc tính ManufacturerResolutionLabel kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.

        public string? ManufacturerNotes { get; set; } // Thuộc tính ManufacturerNotes kiểu dữ liệu string? lưu trữ thông tin truyền tải.
    }
}

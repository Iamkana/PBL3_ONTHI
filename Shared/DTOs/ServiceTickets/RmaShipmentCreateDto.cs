namespace PBL3.Shared.DTOs.ServiceTickets // Định nghĩa namespace PBL3.Shared.DTOs.ServiceTickets quản lý cấu trúc code truyền tải và validator.
{
    public class RmaShipmentCreateDto // Định nghĩa lớp DTO truyền tải dữ liệu RmaShipmentCreateDto.
    {
        public string CarrierName { get; set; } = string.Empty; // Thuộc tính CarrierName kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public string TrackingCode { get; set; } = string.Empty; // Thuộc tính TrackingCode kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
    }
}

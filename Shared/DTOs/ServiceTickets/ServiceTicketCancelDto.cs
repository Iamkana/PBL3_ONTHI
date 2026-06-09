namespace PBL3.Shared.DTOs.ServiceTickets // Định nghĩa namespace PBL3.Shared.DTOs.ServiceTickets quản lý cấu trúc code truyền tải và validator.
{
    public class ServiceTicketCancelDto // Định nghĩa lớp DTO truyền tải dữ liệu ServiceTicketCancelDto.
    {
        public string CancelReason { get; set; } = string.Empty; // Thuộc tính CancelReason kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
    }
}

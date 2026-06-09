namespace PBL3.Shared.DTOs.Analytics // Định nghĩa namespace PBL3.Shared.DTOs.Analytics quản lý cấu trúc code truyền tải và validator.
{
    public class AnalyticsFilterRequest // Định nghĩa lớp DTO truyền tải dữ liệu AnalyticsFilterRequest.
    {
        public DateTime From { get; set; } // Thuộc tính From kiểu dữ liệu DateTime lưu trữ thông tin truyền tải.
        public DateTime To { get; set; } // Thuộc tính To kiểu dữ liệu DateTime lưu trữ thông tin truyền tải.
    }
}

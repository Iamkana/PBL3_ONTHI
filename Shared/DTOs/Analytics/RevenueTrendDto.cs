namespace PBL3.Shared.DTOs.Analytics // Định nghĩa namespace PBL3.Shared.DTOs.Analytics quản lý cấu trúc code truyền tải và validator.
{
    public class RevenueTrendDto // Định nghĩa lớp DTO truyền tải dữ liệu RevenueTrendDto.
    {
        public List<DailyRevenuePointDto> Points { get; set; } = new(); // Thuộc tính Points kiểu dữ liệu List<DailyRevenuePointDto> lưu trữ thông tin truyền tải với giá trị mặc định là new().
    }
}

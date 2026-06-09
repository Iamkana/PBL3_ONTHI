namespace PBL3.Shared.DTOs.Analytics // Định nghĩa namespace PBL3.Shared.DTOs.Analytics quản lý cấu trúc code truyền tải và validator.
{
    public class DailyRevenuePointDto // Định nghĩa lớp DTO truyền tải dữ liệu DailyRevenuePointDto.
    {
        public string Label { get; set; } = string.Empty; // Thuộc tính Label kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public decimal Revenue { get; set; } // Thuộc tính Revenue kiểu dữ liệu decimal lưu trữ thông tin truyền tải.
        public decimal Profit { get; set; } // Thuộc tính Profit kiểu dữ liệu decimal lưu trữ thông tin truyền tải.
    }
}

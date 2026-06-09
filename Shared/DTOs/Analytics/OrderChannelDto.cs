namespace PBL3.Shared.DTOs.Analytics // Định nghĩa namespace PBL3.Shared.DTOs.Analytics quản lý cấu trúc code truyền tải và validator.
{
    public class OrderChannelDto // Định nghĩa lớp DTO truyền tải dữ liệu OrderChannelDto.
    {
        public int OnlineCount { get; set; } // Thuộc tính OnlineCount kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public int PosCount { get; set; } // Thuộc tính PosCount kiểu dữ liệu int lưu trữ thông tin truyền tải.
    }
}

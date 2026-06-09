namespace PBL3.Shared.DTOs.Reviews // Định nghĩa namespace PBL3.Shared.DTOs.Reviews quản lý cấu trúc code truyền tải và validator.
{
    public class CreateReviewRequest // Định nghĩa lớp DTO truyền tải dữ liệu CreateReviewRequest.
    {
        public int ProductId { get; set; } // Thuộc tính ProductId kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public byte Rating { get; set; } // Thuộc tính Rating kiểu dữ liệu byte lưu trữ thông tin truyền tải.
        public string? Title { get; set; } // Thuộc tính Title kiểu dữ liệu string? lưu trữ thông tin truyền tải.
        public string? Content { get; set; } // Thuộc tính Content kiểu dữ liệu string? lưu trữ thông tin truyền tải.
    }
}

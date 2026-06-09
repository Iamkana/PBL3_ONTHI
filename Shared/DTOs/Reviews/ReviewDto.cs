namespace PBL3.Shared.DTOs.Reviews // Định nghĩa namespace PBL3.Shared.DTOs.Reviews quản lý cấu trúc code truyền tải và validator.
{
    public class ReviewDto // Định nghĩa lớp DTO truyền tải dữ liệu ReviewDto.
    {
        public int Id { get; set; } // Thuộc tính Id kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public Guid UserId { get; set; } // Thuộc tính UserId kiểu dữ liệu Guid lưu trữ thông tin truyền tải.
        public string UserFullName { get; set; } = string.Empty; // Thuộc tính UserFullName kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public string? UserAvatarUrl { get; set; } // Thuộc tính UserAvatarUrl kiểu dữ liệu string? lưu trữ thông tin truyền tải.
        public byte Rating { get; set; } // Thuộc tính Rating kiểu dữ liệu byte lưu trữ thông tin truyền tải.
        public string? Title { get; set; } // Thuộc tính Title kiểu dữ liệu string? lưu trữ thông tin truyền tải.
        public string? Content { get; set; } // Thuộc tính Content kiểu dữ liệu string? lưu trữ thông tin truyền tải.
        public DateTime CreatedDate { get; set; } // Thuộc tính CreatedDate kiểu dữ liệu DateTime lưu trữ thông tin truyền tải.
    }
}

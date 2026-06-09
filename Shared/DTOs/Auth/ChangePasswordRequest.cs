namespace PBL3.Shared.DTOs.Auth // Định nghĩa namespace PBL3.Shared.DTOs.Auth quản lý cấu trúc code truyền tải và validator.
{
    public class ChangePasswordRequest // Định nghĩa lớp DTO truyền tải dữ liệu ChangePasswordRequest.
    {
        public string CurrentPassword { get; set; } = string.Empty; // Thuộc tính CurrentPassword kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public string NewPassword { get; set; } = string.Empty; // Thuộc tính NewPassword kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public string ConfirmPassword { get; set; } = string.Empty; // Thuộc tính ConfirmPassword kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
    }
}

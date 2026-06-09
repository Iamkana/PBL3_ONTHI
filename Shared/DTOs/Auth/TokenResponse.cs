namespace PBL3.Shared.DTOs.Auth // Định nghĩa namespace PBL3.Shared.DTOs.Auth quản lý cấu trúc code truyền tải và validator.
{
    /// <summary>
    /// DTO trả về cặp Access Token + Refresh Token cho Client.
    /// KHÔNG chứa bất kỳ thông tin nhạy cảm nào (Password, v.v.).
    /// </summary>
    public class TokenResponse // Định nghĩa lớp DTO truyền tải dữ liệu TokenResponse.
    {
        public string AccessToken { get; set; } = string.Empty; // Thuộc tính AccessToken kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public string RefreshToken { get; set; } = string.Empty; // Thuộc tính RefreshToken kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
    }
}

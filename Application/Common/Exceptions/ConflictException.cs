namespace PBL3.Application.Common.Exceptions; // Khai báo namespace chứa các ngoại lệ tự định nghĩa trong tầng Application.

public class ConflictException : Exception // Định nghĩa lớp ngoại lệ xung đột dữ liệu kế thừa từ Exception.
{
    public ConflictException(string message) : base(message) { } // Hàm khởi tạo nhận thông điệp lỗi và chuyển tiếp lên Exception cha.
}

namespace PBL3.Application.Common.Exceptions; // Khai báo namespace chứa các ngoại lệ tự định nghĩa trong tầng Application.

public class ForbiddenException : Exception // Định nghĩa lớp ngoại lệ bị cấm truy cập (HTTP 403) kế thừa từ Exception.
{
    public ForbiddenException(string message = "Bạn không có quyền thực hiện hành động này.") // Hàm khởi tạo có giá trị mặc định cho thông điệp.
        : base(message) { } // Chuyển tiếp thông điệp lỗi lên lớp cơ sở Exception.
}

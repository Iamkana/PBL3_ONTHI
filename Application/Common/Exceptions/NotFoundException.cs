namespace PBL3.Application.Common.Exceptions; // Khai báo namespace chứa các ngoại lệ tự định nghĩa trong tầng Application.

public class NotFoundException : Exception // Định nghĩa lớp ngoại lệ không tìm thấy tài nguyên (HTTP 404) kế thừa từ Exception.
{
    public NotFoundException(string message) : base(message) { } // Hàm khởi tạo nhận trực tiếp thông điệp lỗi và chuyển tiếp lên Exception cha.

    public NotFoundException(string entityName, object key) // Hàm khởi tạo nạp chồng nhận tên thực thể và khóa tìm kiếm.
        : base($"Không tìm thấy {entityName} với ID '{key}'.") { } // Chuyển tiếp thông điệp lỗi tự tạo lên lớp cơ sở Exception.
}

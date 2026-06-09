namespace PBL3.Application.Common.Exceptions; // Khai báo namespace chứa các ngoại lệ tự định nghĩa trong tầng Application.

public class BusinessRuleException : Exception // Định nghĩa lớp ngoại lệ vi phạm quy tắc nghiệp vụ kế thừa từ Exception của hệ thống.
{
    public BusinessRuleException(string message) : base(message) { } // Hàm khởi tạo nhận thông điệp lỗi và chuyển tiếp lên Exception cha.
}

namespace PBL3.Shared.DTOs.Common; // Định nghĩa namespace PBL3.Shared.DTOs.Common quản lý cấu trúc code truyền tải và validator.

public enum ApiErrorCode // Định nghĩa kiểu liệt kê (enum) ApiErrorCode đại diện cho các trạng thái trong hệ thống.
{
    None       = 0, // Giá trị liệt kê 'None' có giá trị nguyên là 0.
    NotFound   = 1, // Giá trị liệt kê 'NotFound' có giá trị nguyên là 1.
    Forbidden  = 2, // Giá trị liệt kê 'Forbidden' có giá trị nguyên là 2.
    Validation = 3, // Giá trị liệt kê 'Validation' có giá trị nguyên là 3.
    Business   = 4, // Giá trị liệt kê 'Business' có giá trị nguyên là 4.
    Conflict   = 5, // Giá trị liệt kê 'Conflict' có giá trị nguyên là 5.
}

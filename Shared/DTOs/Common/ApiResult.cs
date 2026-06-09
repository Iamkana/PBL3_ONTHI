namespace PBL3.Shared.DTOs.Common; // Định nghĩa namespace PBL3.Shared.DTOs.Common quản lý cấu trúc code truyền tải và validator.

/// <summary>
/// Standard API response wrapper. Tất cả API trả về format này.
/// </summary>
public class ApiResult<T> // Định nghĩa lớp DTO truyền tải dữ liệu ApiResult.
{
    public bool Success { get; set; } // Thuộc tính Success kiểu dữ liệu bool lưu trữ thông tin truyền tải.
    public string Message { get; set; } = string.Empty; // Thuộc tính Message kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
    public T? Data { get; set; } // Thuộc tính Data kiểu dữ liệu T? lưu trữ thông tin truyền tải.
    public ApiErrorCode ErrorCode { get; set; } = ApiErrorCode.None; // Thuộc tính ErrorCode kiểu dữ liệu ApiErrorCode lưu trữ thông tin truyền tải với giá trị mặc định là ApiErrorCode.None.

    public static ApiResult<T> Ok(T data, string message = "Thao tác thành công.") // Thực thi dòng lệnh nghiệp vụ.
        => new() { Success = true, Message = message, Data = data }; // Thực thi dòng lệnh nghiệp vụ.

    public static ApiResult<T> Ok(string message = "Thao tác thành công.") // Thực thi dòng lệnh nghiệp vụ.
        => new() { Success = true, Message = message }; // Thực thi dòng lệnh nghiệp vụ.

    public static ApiResult<T> Fail(string message, ApiErrorCode errorCode = ApiErrorCode.Business) // Thực thi dòng lệnh nghiệp vụ.
        => new() { Success = false, Message = message, ErrorCode = errorCode }; // Thực thi dòng lệnh nghiệp vụ.
}

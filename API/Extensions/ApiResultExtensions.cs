using Microsoft.AspNetCore.Http; // Sử dụng thư viện HTTP của ASP.NET Core để thao tác mã trạng thái HTTP.
using Microsoft.AspNetCore.Mvc; // Sử dụng thư viện MVC để định nghĩa các đối tượng IActionResult.
using PBL3.Shared.DTOs.Common; // Sử dụng các DTO dùng chung của hệ thống như ApiResult, ApiErrorCode.

namespace PBL3.API.Extensions; // Định nghĩa namespace cho các lớp mở rộng ở tầng API.

public static class ApiResultExtensions // Định nghĩa lớp tĩnh ApiResultExtensions chứa các phương thức mở rộng cho ApiResult.
{
    public static IActionResult ToActionResult<T>( // Định nghĩa phương thức mở rộng chuyển đổi ApiResult sang IActionResult tương thích với MVC.
        this ApiResult<T> result, ControllerBase controller) // Sử dụng từ khóa this để biến hàm thành phương thức mở rộng của ApiResult, nhận tham chiếu đến Controller đang gọi.
    {
        if (result.Success) return controller.Ok(result); // Nếu kết quả thành công, trả về HTTP 200 Ok kèm theo dữ liệu kết quả.
        return result.ErrorCode switch // Sử dụng biểu thức switch để phân loại phản hồi lỗi dựa trên mã lỗi ApiErrorCode.
        {
            ApiErrorCode.NotFound  => controller.NotFound(result), // Mã NotFound: Trả về HTTP 404 NotFound.
            ApiErrorCode.Forbidden => controller.StatusCode(StatusCodes.Status403Forbidden, result), // Mã Forbidden: Trả về HTTP 403 Forbidden.
            ApiErrorCode.Conflict  => controller.Conflict(result), // Mã Conflict: Trả về HTTP 409 Conflict.
            _                      => controller.BadRequest(result), // Mặc định cho mọi lỗi khác: Trả về HTTP 400 BadRequest.
        };
    }
}

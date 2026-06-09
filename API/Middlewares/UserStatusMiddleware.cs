using System.Security.Claims; // Sử dụng thư viện bảo mật và quản lý thông tin danh tính (Claims) của người dùng.
using Microsoft.AspNetCore.Identity; // Sử dụng thư viện ASP.NET Core Identity để quản lý tài khoản người dùng.
using Microsoft.Extensions.Caching.Memory; // Sử dụng thư viện lưu trữ bộ nhớ đệm (caching) trong bộ nhớ RAM.
using PBL3.Core.Entities; // Sử dụng các thực thể cốt lõi như AppUser từ tầng Core.
using PBL3.Shared.DTOs.Common; // Sử dụng các DTO dùng chung của hệ thống như ApiResult.

namespace PBL3.API.Middlewares; // Định nghĩa namespace cho các Middleware của ứng dụng API.

public class UserStatusMiddleware // Định nghĩa lớp Middleware UserStatusMiddleware để kiểm tra trạng thái hoạt động của tài khoản người dùng.
{
    private readonly RequestDelegate _next; // Khai báo delegate đại diện cho middleware tiếp theo trong pipeline xử lý HTTP request.

    public UserStatusMiddleware(RequestDelegate next) // Constructor nhận vào delegate middleware tiếp theo.
    {
        _next = next; // Gán giá trị delegate nhận được.
    }

    public async Task InvokeAsync(HttpContext context) // Phương thức thực thi chính của middleware, xử lý bất đồng bộ HTTP Context.
    {
        if (context.User.Identity?.IsAuthenticated == true && // Điều kiện: Nếu người dùng đã được xác thực thành công (đã đăng nhập và JWT hợp lệ).
            !context.Request.Path.StartsWithSegments("/api/auth")) // Và đường dẫn request hiện tại không phải là endpoint xác thực /api/auth.
        {
            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier); // Tìm kiếm và trích xuất UserId từ các Claims của JWT token.
            if (!string.IsNullOrEmpty(userId)) // Nếu UserId lấy ra không rỗng.
            {
                var cache = context.RequestServices.GetRequiredService<IMemoryCache>(); // Lấy dịch vụ IMemoryCache từ DI container của HTTP Request.
                var cacheKey = $"user_isactive_{userId.ToLowerInvariant()}"; // Tạo mã khóa cache đại diện cho trạng thái hoạt động của người dùng cụ thể này.

                if (!cache.TryGetValue(cacheKey, out (bool IsActive, string? LockReason) userData)) // Kiểm tra xem trạng thái người dùng đã được lưu trong cache chưa.
                {
                    var userManager = context.RequestServices.GetRequiredService<UserManager<AppUser>>(); // Nếu chưa có trong cache (Cache miss), lấy dịch vụ UserManager từ DI.
                    var user = await userManager.FindByIdAsync(userId); // Truy vấn thông tin người dùng từ cơ sở dữ liệu dựa trên UserId.

                    userData = (user?.IsActive ?? false, user?.LockReason); // Ánh xạ trạng thái hoạt động (mặc định false nếu null) và lý do khóa của user vào tuple userData.

                    cache.Set(cacheKey, userData, TimeSpan.FromSeconds(30)); // Lưu tuple trạng thái hoạt động này vào Memory Cache với thời hạn hết hạn là 30 giây.
                }

                if (!userData.IsActive) // Nếu tài khoản người dùng đang ở trạng thái bị khóa (IsActive = false).
                {
                    var reason = !string.IsNullOrEmpty(userData.LockReason) // Kiểm tra xem có lý do khóa tài khoản cụ thể nào được ghi nhận không.
                        ? userData.LockReason // Lấy lý do khóa cụ thể từ DB.
                        : "Vui lòng liên hệ quản trị viên."; // Ngược lại hiển thị thông báo mặc định.
                    var message = $"Tài khoản của bạn đã bị khóa. Lý do: {reason}"; // Tạo thông điệp lỗi gửi cho người dùng.

                    context.Response.StatusCode = StatusCodes.Status403Forbidden; // Thiết lập mã trạng thái HTTP phản hồi là 403 Forbidden (Bị từ chối truy cập).
                    context.Response.ContentType = "application/json"; // Đặt định dạng dữ liệu phản hồi là JSON.

                    context.Response.Headers["X-Account-Status"] = "locked"; // Ghi thêm một HTTP Header tùy chỉnh "X-Account-Status" với giá trị "locked" để Client nhận diện.

                    await context.Response.WriteAsJsonAsync(ApiResult<object>.Fail(message)); // Ghi dữ liệu phản hồi JSON lỗi theo định dạng chuẩn ApiResult của hệ thống.

                    return; // Dừng pipeline xử lý HTTP request tại đây, không gọi middleware kế tiếp (không cho request chạy vào controller).
                }
            }
        }

        await _next(context); // Nếu tài khoản hợp lệ (hoặc không cần kiểm tra), gọi delegate để chuyển HTTP request xuống middleware tiếp theo.
    }
}

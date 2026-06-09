using System.Text.Json; // Thư viện xử lý định dạng dữ liệu JSON.
using Microsoft.AspNetCore.Identity; // Thư viện ASP.NET Core Identity quản lý User, Role.
using Microsoft.EntityFrameworkCore; // Thư viện truy vấn cơ sở dữ liệu Entity Framework Core.
using PBL3.API.Extensions; // Tham chiếu các lớp mở rộng (Extension Methods) cấu hình dịch vụ API.
using PBL3.API.Middlewares; // Tham chiếu các Middleware tùy chỉnh của API.
using PBL3.Application.Common.Exceptions; // Tham chiếu các ngoại lệ tự định nghĩa của tầng Application.
using PBL3.Core.Entities; // Tham chiếu các thực thể nghiệp vụ cốt lõi ở tầng Core.
using PBL3.Infrastructure.Data; // Tham chiếu HushStoreDbContext từ tầng Infrastructure.
using PBL3.Shared.DTOs.Common; // Tham chiếu các DTO dùng chung như ApiResult.

var builder = WebApplication.CreateBuilder(args); // Khởi tạo WebApplicationBuilder để cấu hình các dịch vụ và pipeline.

builder.Services.AddControllers(); // Đăng ký các dịch vụ Controller để định tuyến API.

builder.Services.AddMemoryCache(); // Đăng ký dịch vụ lưu trữ cache trong bộ nhớ RAM (IMemoryCache).

// CORS: Cho phép Frontend (Blazor WASM) gọi API
var allowedOrigins = builder.Configuration["AllowedOrigins"]?.Split(',') // Đọc danh sách tên miền được phép từ file cấu hình appsettings.json.
    ?? ["http://localhost:5214", "https://localhost:7107"]; // Giá trị mặc định nếu cấu hình trống.

builder.Services.AddCors(options => // Cấu hình chính sách CORS để cho phép ứng dụng khách truy cập API.
{
    options.AddPolicy("AllowClient", policy => // Tạo chính sách CORS mang tên "AllowClient".
    {
        policy.WithOrigins(allowedOrigins) // Chỉ cho phép các domain được liệt kê truy cập.
              .AllowAnyHeader() // Cho phép gửi bất kỳ HTTP Header nào.
              .AllowAnyMethod() // Cho phép bất kỳ HTTP Method nào (GET, POST, PUT, DELETE...).
              .WithExposedHeaders("X-Account-Status"); // Expose (lộ) header tùy chỉnh này cho client đọc được.
    });
});

builder.Services.AddOpenApi(); // Đăng ký dịch vụ tạo tài liệu OpenApi (Swagger).
builder.Services.AddSwaggerGen(); // Đăng ký Swagger Generator để tạo tài liệu API tương tác.

// Add DbContext
builder.Services.AddDbContext<HushStoreDbContext>(options => // Đăng ký lớp DbContext quản lý cơ sở dữ liệu.
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")); // Cấu hình kết nối SQL Server dựa trên chuỗi kết nối DefaultConnection.
    // ServiceInvoice intentionally omits the query filter so financial records
    // remain queryable even after the parent ServiceTicket is soft-deleted.
    options.ConfigureWarnings(w => w.Ignore( // Tắt cảnh báo về việc sử dụng Query Filter đi kèm quan hệ Required Navigation không đúng cách.
        Microsoft.EntityFrameworkCore.Diagnostics.CoreEventId // Định danh loại cảnh báo.
            .PossibleIncorrectRequiredNavigationWithQueryFilterInteractionWarning)); // Bỏ qua cảnh báo EF Core này để tránh gây nhiễu log hệ thống.
});

builder.Services.AddJwtAuthentication(builder.Configuration); // Gọi phương thức mở rộng cấu hình xác thực JWT.
builder.Services.AddRepositories(); // Gọi phương thức mở rộng đăng ký các lớp Repository vào DI Container.
builder.Services.AddApplicationServices(builder.Configuration); // Gọi phương thức mở rộng đăng ký các lớp Service và cấu hình liên quan (như AWS S3, Rate Limit).

var app = builder.Build(); // Xây dựng đối tượng WebApplication để thiết lập middleware pipeline.

// Auto-apply EF Core migrations khi khởi động — chỉ chạy trên Production
// (tránh lỗi khi dev chạy local với DB chưa up)
if (app.Environment.IsProduction()) // Kiểm tra nếu ứng dụng đang chạy trong môi trường Production.
{
    using var scope = app.Services.CreateScope(); // Khởi tạo một scope dịch vụ tạm thời.
    var db = scope.ServiceProvider.GetRequiredService<HushStoreDbContext>(); // Lấy thực thể HushStoreDbContext từ DI Container.
    await db.Database.MigrateAsync(); // Tự động áp dụng các Migration chưa chạy lên Database Production bất đồng bộ.
}

// Seed "Technician" role if it doesn't exist
using (var roleScope = app.Services.CreateScope()) // Tạo scope dịch vụ để seed dữ liệu vai trò.
{
    var roleManager = roleScope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>(); // Lấy RoleManager từ DI.
    if (!await roleManager.RoleExistsAsync("Technician")) // Kiểm tra xem vai trò "Technician" (Kỹ thuật viên) đã tồn tại trong DB chưa.
    {
        await roleManager.CreateAsync(new AppRole { Name = "Technician", RoleCode = "KTV" }); // Tạo mới vai trò Technician với mã KTV nếu chưa có.
    }
}

if (app.Environment.IsDevelopment()) // Nếu ứng dụng đang chạy trong môi trường Development (Phát triển).
{
    app.MapOpenApi(); // Định tuyến để hiển thị đặc tả tài liệu OpenAPI.
    app.UseSwagger(); // Bật middleware hiển thị tài liệu Swagger JSON.
    app.UseSwaggerUI(c => // Cấu hình giao diện người dùng Swagger UI.
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "HushStore API v1"); // Đường dẫn đến file đặc tả Swagger.
    });
}

app.UseExceptionHandler(errApp => errApp.Run(async ctx => // Đăng ký middleware xử lý ngoại lệ toàn cục (Global Exception Handler).
{
    var feature = ctx.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>(); // Lấy thông tin lỗi đã xảy ra từ HTTP Context.
    var ex = feature?.Error; // Trích xuất đối tượng lỗi Exception.

    ctx.Response.ContentType = "application/json"; // Đặt kiểu nội dung trả về là JSON.
    var opts = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }; // Định dạng JSON trả về dạng camelCase.

    switch (ex) // Xử lý trả về mã trạng thái HTTP khác nhau tùy theo kiểu lỗi nghiệp vụ.
    {
        case NotFoundException notFound: // Lỗi không tìm thấy tài nguyên (HTTP 404).
            ctx.Response.StatusCode = StatusCodes.Status404NotFound; // Thiết lập status code.
            await ctx.Response.WriteAsync(JsonSerializer.Serialize(ApiResult<object>.Fail(notFound.Message), opts)); // Viết phản hồi JSON lỗi.
            break; // Thoát khối switch.
        case ForbiddenException forbidden: // Lỗi không được quyền truy cập tài nguyên (HTTP 403).
            ctx.Response.StatusCode = StatusCodes.Status403Forbidden; // Thiết lập status code.
            await ctx.Response.WriteAsync(JsonSerializer.Serialize(ApiResult<object>.Fail(forbidden.Message), opts)); // Viết phản hồi JSON lỗi.
            break; // Thoát khối switch.
        case BusinessRuleException rule: // Lỗi vi phạm nghiệp vụ hệ thống (HTTP 422).
            ctx.Response.StatusCode = StatusCodes.Status422UnprocessableEntity; // Thiết lập status code.
            await ctx.Response.WriteAsync(JsonSerializer.Serialize(ApiResult<object>.Fail(rule.Message), opts)); // Viết phản hồi JSON lỗi.
            break; // Thoát khối switch.
        case ConflictException conflict: // Lỗi xung đột tài nguyên, ví dụ trùng Email/SĐT (HTTP 409).
            ctx.Response.StatusCode = StatusCodes.Status409Conflict; // Thiết lập status code.
            await ctx.Response.WriteAsync(JsonSerializer.Serialize(ApiResult<object>.Fail(conflict.Message), opts)); // Viết phản hồi JSON lỗi.
            break; // Thoát khối switch.
        default: // Mọi lỗi hệ thống không xác định khác (HTTP 500).
            ctx.Response.StatusCode = StatusCodes.Status500InternalServerError; // Thiết lập status code.
            var message = app.Environment.IsDevelopment() // Nếu đang ở môi trường Dev thì show chi tiết lỗi.
                ? ex?.Message ?? "Lỗi máy chủ nội bộ." // Hiển thị chi tiết thông điệp lỗi.
                : "Lỗi máy chủ nội bộ."; // Ngược lại, ẩn chi tiết lỗi để bảo mật thông tin hệ thống.
            await ctx.Response.WriteAsync(JsonSerializer.Serialize(ApiResult<object>.Fail(message), opts)); // Viết phản hồi JSON lỗi chung.
            break; // Thoát khối switch.
    }
}));

app.UseHttpsRedirection(); // Bật middleware tự động chuyển hướng HTTP sang HTTPS.
app.UseCors("AllowClient"); // Bật CORS với cấu hình chính sách "AllowClient" đã định nghĩa trước đó.
app.UseRateLimiter(); // Bật middleware kiểm soát tần suất gửi yêu cầu để chống DoS/Brute-force.
app.UseAuthentication(); // Bật middleware xác thực danh tính người dùng (đọc và giải mã JWT token).

// Kiểm tra IsActive sau khi JWT đã được xác thực — dùng MemoryCache 30 giây để giảm DB query
app.UseMiddleware<UserStatusMiddleware>(); // Bật middleware tùy chỉnh kiểm tra xem tài khoản người dùng có bị Admin khóa hay không.

app.UseAuthorization(); // Bật middleware phân quyền người dùng (kiểm tra vai trò Role/Policy).

// Health check endpoint for Docker
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow })) // Khai báo endpoint /health phục vụ kiểm tra trạng thái hoạt động của ứng dụng (Docker/K8s).
   .AllowAnonymous(); // Cho phép truy cập công khai không cần đăng nhập.

app.MapControllers(); // Định tuyến các Endpoint đến các Action cụ thể trong Controller.

app.Run(); // Bắt đầu chạy ứng dụng web.

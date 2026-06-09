using System.Text; // Sử dụng thư viện Encoding để chuyển đổi chuỗi sang mảng byte.
using Microsoft.AspNetCore.Authentication.JwtBearer; // Sử dụng thư viện xác thực JWT Bearer trong ASP.NET Core.
using Microsoft.AspNetCore.Identity; // Sử dụng thư viện cấu hình Identity người dùng.
using Microsoft.IdentityModel.Tokens; // Sử dụng thư viện định nghĩa TokenValidationParameters và SymmetricSecurityKey.
using PBL3.Core.Entities; // Sử dụng các thực thể cốt lõi như AppUser, AppRole.
using PBL3.Infrastructure.Data; // Sử dụng lớp cơ sở dữ liệu DbContext.

namespace PBL3.API.Extensions; // Định nghĩa namespace cho các lớp mở rộng ở tầng API.

public static class AuthenticationExtensions // Định nghĩa lớp tĩnh cấu hình các dịch vụ xác thực và danh tính.
{
    public static IServiceCollection AddJwtAuthentication( // Định nghĩa phương thức mở rộng cấu hình xác thực và danh tính cho ứng dụng.
        this IServiceCollection services, // Đối tượng IServiceCollection để đăng ký các dịch vụ.
        IConfiguration configuration) // Đối tượng IConfiguration để đọc thiết lập từ appsettings.json.
    {
        // ASP.NET Core Identity
        services.AddIdentity<AppUser, AppRole>(options => // Cấu hình hệ thống Identity cho người dùng và vai trò.
        {
            // Password settings (Tùy chỉnh độ khó theo thực tế)
            options.Password.RequireDigit = true; // Yêu cầu mật khẩu phải chứa ít nhất một chữ số.
            options.Password.RequireLowercase = true; // Yêu cầu mật khẩu phải chứa ít nhất một ký tự chữ thường.
            options.Password.RequireUppercase = true; // Yêu cầu mật khẩu phải chứa ít nhất một ký tự chữ hoa.
            options.Password.RequireNonAlphanumeric = false; // Không bắt buộc mật khẩu phải chứa ký tự đặc biệt.
            options.Password.RequiredLength = 8; // Yêu cầu mật khẩu có độ dài tối thiểu là 8 ký tự.

            // Lockout settings (Chống Brute-force)
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15); // Đặt thời gian khóa tài khoản tạm thời là 15 phút nếu nhập sai quá số lần quy định.
            options.Lockout.MaxFailedAccessAttempts = 5; // Khóa tài khoản sau tối đa 5 lần đăng nhập thất bại liên tiếp.

            // User settings
            options.User.RequireUniqueEmail = true; // Yêu cầu mỗi địa chỉ email chỉ được đăng ký một tài khoản duy nhất.
        })
        .AddEntityFrameworkStores<HushStoreDbContext>() // Đăng ký lưu trữ dữ liệu Identity trong cơ sở dữ liệu qua EF Core.
        .AddDefaultTokenProviders(); // Đăng ký các nhà cung cấp token mặc định (dùng cho xác nhận email, reset mật khẩu).

        // JWT Bearer Authentication
        var jwtSecretKey = configuration["JwtSettings:SecretKey"] // Đọc mã khóa bí mật (SecretKey) từ cấu hình.
            ?? throw new InvalidOperationException("JwtSettings:SecretKey chưa được cấu hình trong appsettings.json."); // Ném lỗi nếu chưa cấu hình khóa bí mật.

        services.AddAuthentication(options => // Cấu hình cơ chế xác thực mặc định của ứng dụng.
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; // Thiết lập schema xác thực mặc định là Jwt Bearer.
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; // Thiết lập schema thách thức (challenge) mặc định là Jwt Bearer.
        })
        .AddJwtBearer(options => // Cấu hình chi tiết các tham số xác thực JWT.
        {
            options.TokenValidationParameters = new TokenValidationParameters // Định nghĩa các tham số kiểm tra tính hợp lệ của JWT token.
            {
                ValidateIssuer = true, // Yêu cầu kiểm tra nhà phát hành token (Issuer).
                ValidIssuer = configuration["JwtSettings:Issuer"], // Tên nhà phát hành hợp lệ đọc từ cấu hình.
                ValidateAudience = true, // Yêu cầu kiểm tra đối tượng sử dụng token (Audience).
                ValidAudience = configuration["JwtSettings:Audience"], // Tên đối tượng sử dụng hợp lệ đọc từ cấu hình.
                ValidateIssuerSigningKey = true, // Yêu cầu kiểm tra khóa chữ ký của nhà phát hành.
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey)), // Tạo khóa ký đối xứng từ Secret Key.
                ValidateLifetime = true, // Yêu cầu kiểm tra hạn sử dụng của token.
                ClockSkew = TimeSpan.FromMinutes(1) // Đặt khoảng lệch thời gian tối đa cho phép giữa client và server là 1 phút.
            };
        });

        return services; // Trả về đối tượng IServiceCollection để tiếp tục cấu hình.
    }
}

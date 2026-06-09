using System.IdentityModel.Tokens.Jwt; // Sử dụng thư viện JWT để tạo và phân tích Token.
using System.Security.Claims; // Sử dụng để quản lý thông tin danh tính (Claims) của người dùng.
using System.Security.Cryptography; // Sử dụng các thuật toán mật mã hóa bảo mật (SHA256, RandomNumberGenerator).
using System.Text; // Sử dụng để mã hóa và giải mã chuỗi văn bản.
using Microsoft.AspNetCore.Identity; // Sử dụng Microsoft Identity quản lý tài khoản người dùng và băm mật khẩu.
using Microsoft.Extensions.Configuration; // Sử dụng để truy xuất các cấu hình hệ thống từ appsettings.json.
using Microsoft.EntityFrameworkCore; // Sử dụng Entity Framework Core cho các thao tác truy vấn Database.
using Microsoft.IdentityModel.Tokens; // Sử dụng các lớp cấu hình bảo mật Token.
using PBL3.Core.Entities; // Sử dụng các thực thể thực (AppUser, UserProfile).
using PBL3.Infrastructure.Data; // Sử dụng ngữ cảnh DB HushStoreDbContext.
using PBL3.Shared.DTOs.Auth; // Sử dụng các DTO của module xác thực.
using PBL3.Shared.DTOs.Common; // Sử dụng DTO dùng chung ApiResult.
using PBL3.Shared.DTOs.Customers; // Sử dụng các DTO quản lý đăng ký của khách hàng.

namespace PBL3.Application.Auth // Khai báo namespace cho lớp AuthService thuộc tầng Application.
{
    public class AuthService( // Định nghĩa lớp AuthService sử dụng Primary Constructor.
        UserManager<AppUser> userManager, // Tiêm UserManager quản lý tài khoản người dùng AppUser.
        IConfiguration configuration, // Tiêm cấu hình IConfiguration đọc cài đặt JWT.
        HushStoreDbContext context) : IAuthService // Tiêm HushStoreDbContext để giao tiếp với DB.
    {
        private readonly UserManager<AppUser> _userManager = // Khởi tạo trường thành viên UserManager.
            userManager ?? throw new ArgumentNullException(nameof(userManager)); // Kiểm tra null cho UserManager.
        private readonly IConfiguration _configuration = // Khởi tạo trường thành viên IConfiguration.
            configuration ?? throw new ArgumentNullException(nameof(configuration)); // Kiểm tra null cho IConfiguration.
        private readonly HushStoreDbContext _context = // Khởi tạo trường thành viên DbContext.
            context ?? throw new ArgumentNullException(nameof(context)); // Kiểm tra null cho DbContext.

        public async Task<ApiResult<TokenResponse>> LoginAsync(LoginRequest request) // Phương thức đăng nhập hệ thống bất đồng bộ.
        {
            var user = await _userManager.FindByEmailAsync(request.Email); // Tìm người dùng theo địa chỉ Email trong Database.
            if (user == null) // Nếu không tìm thấy người dùng.
            {
                return ApiResult<TokenResponse>.Fail("Tài khoản hoặc mật khẩu không đúng."); // Trả về thông báo lỗi chung để bảo mật thông tin.
            }

            if (!user.IsActive) // Kiểm tra tài khoản có đang hoạt động hay không.
            {
                var reason = !string.IsNullOrEmpty(user.LockReason) ? user.LockReason : "Vui lòng liên hệ quản trị viên."; // Lấy lý do khóa tài khoản.
                return ApiResult<TokenResponse>.Fail($"Tài khoản đã bị khóa. Lý do: {reason}"); // Trả về lỗi tài khoản bị khóa kèm lý do.
            }

            if (await _userManager.IsLockedOutAsync(user)) // Kiểm tra tài khoản có bị khóa tạm thời do nhập sai mật khẩu quá giới hạn hay không.
            {
                return ApiResult<TokenResponse>.Fail( // Trả về thông báo lỗi brute-force.
                    "Tài khoản đã bị tạm khóa do đăng nhập sai quá nhiều lần. Vui lòng thử lại sau."); // Thông báo người dùng chờ đợi.
            }

            var passwordValid = await _userManager.CheckPasswordAsync(user, request.Password); // Thực hiện xác thực mật khẩu qua Identity.
            if (!passwordValid) // Nếu mật khẩu không khớp.
            {
                await _userManager.AccessFailedAsync(user); // Tăng bộ đếm số lần đăng nhập sai của tài khoản.
                return ApiResult<TokenResponse>.Fail("Tài khoản hoặc mật khẩu không đúng."); // Trả về lỗi chung bảo mật.
            }

            await _userManager.ResetAccessFailedCountAsync(user); // Đăng nhập thành công, reset bộ đếm đăng nhập sai về 0.

            var accessToken = await GenerateJwtTokenAsync(user); // Tạo chuỗi JWT Access Token chứa các Claims phân quyền.

            var refreshToken = GenerateRefreshToken(); // Tạo chuỗi Refresh Token ngẫu nhiên bảo mật.

            var refreshTokenExpirationDays = _configuration.GetValue<int>("JwtSettings:RefreshTokenExpirationDays"); // Lấy thời gian hết hạn Refresh Token từ cấu hình.
            user.RefreshToken = HashToken(refreshToken); // Thực hiện băm SHA256 Refresh Token và gán vào AppUser.
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(refreshTokenExpirationDays); // Thiết lập thời điểm hết hạn của Refresh Token trong Database.
            await _userManager.UpdateAsync(user); // Cập nhật thông tin tài khoản AppUser xuống Database.

            return ApiResult<TokenResponse>.Ok(new TokenResponse // Trả về kết quả đăng nhập thành công.
            {
                AccessToken = accessToken, // Gán JWT Access Token.
                RefreshToken = refreshToken // Gán Refresh Token trần (được gửi về Client).
            }, "Đăng nhập thành công."); // Trả về kèm thông điệp thành công.
        }

        public async Task<ApiResult<TokenResponse>> RefreshTokenAsync(RefreshTokenRequest request) // Phương thức cấp mới Token bằng Refresh Token.
        {
            var principal = GetPrincipalFromExpiredToken(request.AccessToken); // Bóc tách Claims từ Access Token đã hết hạn.
            if (principal == null) // Nếu Token bị hỏng hoặc chữ ký không hợp lệ.
            {
                return ApiResult<TokenResponse>.Fail("Access Token không hợp lệ."); // Trả về lỗi Access Token không hợp lệ.
            }

            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier); // Tìm Claim NameIdentifier để định danh UserId.
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId)) // Nếu không tìm thấy hoặc UserId sai định dạng Guid.
            {
                return ApiResult<TokenResponse>.Fail("Không thể xác định người dùng từ Token."); // Trả về lỗi định danh.
            }

            var user = await _userManager.FindByIdAsync(userId.ToString()); // Tìm người dùng AppUser trong cơ sở dữ liệu bằng UserId.
            if (user == null) // Nếu người dùng không tồn tại.
            {
                return ApiResult<TokenResponse>.Fail("Người dùng không tồn tại."); // Trả về thông báo lỗi.
            }

            if (!user.IsActive) // Kiểm tra tài khoản người dùng có bị khóa giữa chừng không.
            {
                var reason = !string.IsNullOrEmpty(user.LockReason) ? user.LockReason : "Vui lòng liên hệ quản trị viên."; // Lấy lý do khóa.
                return ApiResult<TokenResponse>.Fail($"Tài khoản đã bị khóa. Lý do: {reason}"); // Trả về thông báo lỗi tài khoản bị khóa.
            }

            var hashedToken = HashToken(request.RefreshToken); // Tiến hành băm Refresh Token gửi lên để đối chiếu.
            if (user.RefreshToken != hashedToken) // Nếu không trùng khớp với mã băm lưu trong cơ sở dữ liệu.
            {
                return ApiResult<TokenResponse>.Fail("Refresh Token không hợp lệ."); // Báo lỗi token không khớp.
            }

            if (user.RefreshTokenExpiryTime <= DateTime.UtcNow) // Kiểm tra xem Refresh Token trong DB đã hết hạn chưa.
            {
                return ApiResult<TokenResponse>.Fail("Refresh Token đã hết hạn. Vui lòng đăng nhập lại."); // Trả về thông báo lỗi hết hạn.
            }

            var newAccessToken = await GenerateJwtTokenAsync(user); // Tạo mới chuỗi JWT Access Token.
            var newRefreshToken = GenerateRefreshToken(); // Tạo mới chuỗi Refresh Token ngẫu nhiên (Token Rotation).

            var refreshTokenExpirationDays = _configuration.GetValue<int>("JwtSettings:RefreshTokenExpirationDays"); // Lấy thời gian sống cấu hình.
            user.RefreshToken = HashToken(newRefreshToken); // Băm Refresh Token mới và cập nhật cho AppUser.
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(refreshTokenExpirationDays); // Gia hạn thời điểm hết hạn Refresh Token.
            await _userManager.UpdateAsync(user); // Lưu cập nhật thay đổi tài khoản vào cơ sở dữ liệu.

            return ApiResult<TokenResponse>.Ok(new TokenResponse // Trả về cặp Token mới cho khách hàng.
            {
                AccessToken = newAccessToken, // Access Token mới.
                RefreshToken = newRefreshToken // Refresh Token mới.
            }, "Làm mới Token thành công."); // Kèm thông báo thành công.
        }

        private async Task<string> GenerateJwtTokenAsync(AppUser user) // Hàm tạo JWT Access Token bất đồng bộ.
        {
            var secretKey = _configuration["JwtSettings:SecretKey"] // Đọc khóa bí mật từ cấu hình hệ thống.
                ?? throw new InvalidOperationException("JwtSettings:SecretKey chưa được cấu hình."); // Ném ra ngoại lệ nếu thiếu khóa bí mật.
            var issuer = _configuration["JwtSettings:Issuer"]; // Đọc Issuer cấu hình.
            var audience = _configuration["JwtSettings:Audience"]; // Đọc Audience cấu hình.
            var expirationMinutes = _configuration.GetValue<int>("JwtSettings:AccessTokenExpirationMinutes"); // Đọc thời gian hết hạn Access Token (phút).

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)); // Mã hóa khóa bí mật thành SymmetricSecurityKey.
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256); // Cấu hình thuật toán ký mã hóa SHA256.

            var profile = await _context.UserProfiles.AsNoTracking().FirstOrDefaultAsync(p => p.UserId == user.Id); // Lấy hồ sơ người dùng để tìm họ và tên.
            var fullName = profile?.FullName ?? string.Empty; // Nếu không có, mặc định là chuỗi rỗng.

            var claims = new List<Claim> // Khởi tạo danh sách các Claim thông tin.
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()), // Claim UserId.
                new(ClaimTypes.Email, user.Email ?? string.Empty), // Claim Email.
                new(ClaimTypes.Name, fullName), // Claim Họ và Tên.
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // Claim mã định danh duy nhất của Token chống trùng.
            };

            var roles = await _userManager.GetRolesAsync(user); // Lấy danh sách các vai trò (Roles) của tài khoản từ UserManager.
            foreach (var role in roles) // Lặp qua từng vai trò.
            {
                claims.Add(new Claim(ClaimTypes.Role, role)); // Thêm vai trò vào danh sách Claim phân quyền.
            }

            var token = new JwtSecurityToken( // Khởi tạo cấu trúc JWT Token.
                issuer: issuer, // Cấp bởi.
                audience: audience, // Nhận bởi.
                claims: claims, // Thông tin Claims đi kèm.
                expires: DateTime.UtcNow.AddMinutes(expirationMinutes), // Thời hạn hết hiệu lực.
                signingCredentials: credentials // Chữ ký xác thực.
            );

            return new JwtSecurityTokenHandler().WriteToken(token); // Chuyển đổi đối tượng JWT sang chuỗi văn bản Token.
        }

        private static string GenerateRefreshToken() // Hàm tạo Refresh Token ngẫu nhiên.
        {
            var randomBytes = new byte[64]; // Tạo mảng byte độ dài 64.
            using var rng = RandomNumberGenerator.Create(); // Sử dụng RandomNumberGenerator tạo dữ liệu an toàn mã hóa.
            rng.GetBytes(randomBytes); // Điền dữ liệu ngẫu nhiên vào mảng byte.
            return Convert.ToBase64String(randomBytes); // Đổi mảng byte ngẫu nhiên sang chuỗi Base64.
        }

        private static string HashToken(string token) // Hàm băm một chiều SHA256 cho Token.
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token)); // Thực hiện băm SHA256 chuỗi Token dạng byte.
            return Convert.ToBase64String(bytes); // Trả về kết quả dạng chuỗi Base64.
        }

        private ClaimsPrincipal? GetPrincipalFromExpiredToken(string token) // Hàm bóc tách lấy danh tính từ Token hết hạn.
        {
            var secretKey = _configuration["JwtSettings:SecretKey"] // Lấy khóa bí mật.
                ?? throw new InvalidOperationException("JwtSettings:SecretKey chưa được cấu hình."); // Báo lỗi nếu thiếu khóa.

            var tokenValidationParameters = new TokenValidationParameters // Thiết lập cấu hình kiểm định Token.
            {
                ValidateAudience = true, // Yêu cầu kiểm tra Audience.
                ValidAudience = _configuration["JwtSettings:Audience"], // Giá trị Audience hợp lệ.
                ValidateIssuer = true, // Yêu cầu kiểm tra Issuer.
                ValidIssuer = _configuration["JwtSettings:Issuer"], // Giá trị Issuer hợp lệ.
                ValidateIssuerSigningKey = true, // Yêu cầu kiểm tra khóa ký số.
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)), // Cung cấp khóa ký.
                ValidateLifetime = false // Không kiểm tra thời hạn sống của token để bóc tách thông tin khi đã hết hạn.
            };

            try // Bắt lỗi giải mã Token.
            {
                var tokenHandler = new JwtSecurityTokenHandler(); // Khởi tạo Handler xử lý token.
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken); // Giải mã và kiểm tra chữ ký token.

                if (securityToken is not JwtSecurityToken jwtToken || // Kiểm tra định dạng có đúng là JWT Token không.
                    !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, // Kiểm tra thuật toán ký có đúng là HmacSha256 không.
                        StringComparison.InvariantCultureIgnoreCase)) // So sánh chuỗi thuật toán bỏ qua chữ hoa/thường.
                {
                    return null; // Trả về null nếu thuật toán không khớp.
                }

                return principal; // Trả về đối tượng ClaimsPrincipal chứa các thông tin người dùng.
            }
            catch // Nếu token bị sửa đổi hoặc lỗi cấu trúc.
            {
                return null; // Trả về null.
            }
        }

        public async Task<ApiResult<bool>> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword) // Phương thức đổi mật khẩu tài khoản.
        {
            var user = await _userManager.FindByIdAsync(userId.ToString()); // Tìm người dùng theo ID trong DB.
            if (user == null) // Nếu không tìm thấy người dùng.
            {
                return ApiResult<bool>.Fail("Không tìm thấy người dùng."); // Trả về kết quả thất bại.
            }

            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword); // Gọi Identity thay đổi mật khẩu cũ thành mật khẩu mới.
            if (!result.Succeeded) // Nếu thay đổi thất bại (mật khẩu cũ sai hoặc mật khẩu mới vi phạm Policy).
            {
                var error = result.Errors.FirstOrDefault()?.Description ?? "Đổi mật khẩu thất bại."; // Lấy mô tả chi tiết lỗi.
                return ApiResult<bool>.Fail("Mật khẩu hiện tại không đúng hoặc mật khẩu mới không hợp lệ."); // Báo lỗi chi tiết.
            }

            return ApiResult<bool>.Ok(true, "Đổi mật khẩu thành công."); // Đổi mật khẩu thành công.
        }

        public async Task<ApiResult<bool>> RegisterAsync(RegisterCustomerRequest request) // Phương thức khách hàng đăng ký tài khoản mới.
        {
            var existingUser = await _userManager.FindByEmailAsync(request.Email); // Kiểm tra Email đăng ký đã tồn tại chưa.
            if (existingUser != null) // Nếu đã tồn tại.
            {
                return ApiResult<bool>.Fail("Email này đã được sử dụng.", ApiErrorCode.Conflict); // Trả về lỗi xung đột (Conflict).
            }

            var existingPhone = _context.Users.Any(u => u.PhoneNumber == request.PhoneNumber); // Kiểm tra số điện thoại đăng ký đã tồn tại chưa.
            if (existingPhone) // Nếu số điện thoại trùng.
            {
                return ApiResult<bool>.Fail("Số điện thoại này đã được sử dụng.", ApiErrorCode.Conflict); // Báo lỗi trùng số điện thoại.
            }

            var user = new AppUser // Khởi tạo thực thể người dùng AppUser mới.
            {
                Id = Guid.NewGuid(), // Tạo ngẫu nhiên ID mới kiểu Guid.
                UserName = request.Email, // UserName trùng Email đăng nhập.
                Email = request.Email, // Gán Email.
                PhoneNumber = request.PhoneNumber, // Gán số điện thoại.
                IsActive = true, // Tài khoản tự động kích hoạt.
                Type = 2, // Đánh dấu loại người dùng: 2 (Khách hàng).
                CreatedDate = DateTime.UtcNow, // Gán thời gian đăng ký hiện tại.
                IsDeleted = false // Mặc định chưa bị xóa.
            };

            var createResult = await _userManager.CreateAsync(user, request.Password); // Thực hiện tạo tài khoản và tự động băm mật khẩu.
            if (!createResult.Succeeded) // Nếu tạo tài khoản lỗi.
            {
                var errors = string.Join(", ", createResult.Errors.Select(e => e.Description)); // Ghép các thông báo lỗi.
                return ApiResult<bool>.Fail("Đăng ký thất bại: " + errors); // Trả về lỗi chi tiết.
            }

            _context.UserProfiles.Add(new UserProfile // Tạo hồ sơ chi tiết người dùng liên kết.
            {
                UserId = user.Id, // Liên kết khóa ngoại.
                FullName = request.FullName.Trim() // Gán họ tên đầy đủ đã lược bỏ khoảng trắng.
            });
            await _context.SaveChangesAsync(); // Lưu hồ sơ UserProfile xuống cơ sở dữ liệu.

            await _userManager.AddToRoleAsync(user, "Customer"); // Gán vai trò "Customer" mặc định cho tài khoản mới đăng ký.

            return ApiResult<bool>.Ok(true, "Đăng ký tài khoản thành công. Vui lòng đăng nhập."); // Trả về thông báo thành công.
        }
    }
}

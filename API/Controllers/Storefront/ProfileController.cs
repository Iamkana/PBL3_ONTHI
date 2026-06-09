using Microsoft.AspNetCore.Authorization; // Sử dụng để phân quyền truy cập.
using Microsoft.AspNetCore.Identity; // Sử dụng thư viện Microsoft Identity quản lý tài khoản người dùng.
using Microsoft.AspNetCore.Mvc; // Sử dụng các thành phần MVC cho Web API.
using Microsoft.EntityFrameworkCore; // Sử dụng Entity Framework Core cho các truy vấn mở rộng (Include, AsNoTracking...).
using PBL3.Core.Entities; // Sử dụng thực thể người dùng AppUser và UserProfile.
using PBL3.Shared.DTOs.Common; // Sử dụng các DTO dùng chung.
using PBL3.Shared.DTOs.Customers; // Sử dụng các DTO liên quan đến thông tin khách hàng.
using System.Security.Claims; // Sử dụng các Claims để nhận dạng người dùng hiện tại.

namespace PBL3.API.Controllers.Storefront // Khai báo namespace cho Controllers thuộc Storefront.
{
    [ApiController] // Khai báo đây là một Web API Controller hỗ trợ tự động validate model.
    [Route("api/storefront/profile")] // Định nghĩa route truy cập: api/storefront/profile.
    [Produces("application/json")] // Quy định định dạng trả về mặc định dạng JSON.
    [Authorize] // Chỉ cho phép người dùng đã đăng nhập (Customer, Employee, Admin) truy cập.
    public class ProfileController : ControllerBase // Định nghĩa lớp ProfileController kế thừa từ ControllerBase.
    {
        private readonly UserManager<AppUser> _userManager; // Khai báo dịch vụ UserManager của Identity để quản lý thông tin tài khoản.

        public ProfileController(UserManager<AppUser> userManager) // Constructor injection tiêm UserManager.
        {
            _userManager = userManager; // Gán UserManager được tiêm.
        }

        [HttpGet("me")] // Định nghĩa HTTP GET Method lấy thông tin cá nhân (api/storefront/profile/me).
        [ProducesResponseType(typeof(ApiResult<CustomerDto>), StatusCodes.Status200OK)] // Trả về thông tin hồ sơ khách hàng thành công.
        public async Task<IActionResult> GetMyProfile() // Lấy hồ sơ cá nhân của người dùng hiện tại đang đăng nhập.
        {
            var userId = GetCurrentUserId(); // Lấy UserId của người dùng hiện tại.
            if (userId == Guid.Empty) // Nếu chưa đăng nhập hoặc ID rỗng.
                return Unauthorized(ApiResult<CustomerDto>.Fail("Người dùng chưa đăng nhập.")); // Trả về lỗi 401 Unauthorized.

            var user = await _userManager.Users // Thực hiện truy vấn bảng Users.
                .Include(u => u.Profile) // Eager loading tải kèm thông tin Profile liên kết.
                .AsNoTracking() // Tối ưu hóa hiệu năng, không theo dõi thực thể thay đổi.
                .FirstOrDefaultAsync(u => u.Id == userId); // Tìm User theo UserId trong database.

            if (user == null) // Nếu không tìm thấy người dùng.
                return NotFound(ApiResult<CustomerDto>.Fail("Không tìm thấy thông tin người dùng.")); // Trả về lỗi 404 NotFound.

            return Ok(ApiResult<CustomerDto>.Ok(MapToDto(user))); // Map thông tin AppUser sang DTO và trả về kết quả thành công.
        }

        [HttpPut("me")] // Định nghĩa HTTP PUT Method cập nhật thông tin cá nhân (api/storefront/profile/me).
        [ProducesResponseType(typeof(ApiResult<CustomerDto>), StatusCodes.Status200OK)] // Cập nhật thành công.
        [ProducesResponseType(typeof(ApiResult<CustomerDto>), StatusCodes.Status400BadRequest)] // Cập nhật lỗi.
        public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateCustomerRequest request) // Cập nhật thông tin cá nhân của người dùng.
        {
            var userId = GetCurrentUserId(); // Lấy UserId của người dùng hiện tại.
            if (userId == Guid.Empty) // Nếu chưa đăng nhập hoặc ID không hợp lệ.
                return Unauthorized(ApiResult<CustomerDto>.Fail("Người dùng chưa đăng nhập.")); // Trả về lỗi 401.

            var user = await _userManager.Users // Truy vấn bảng Users.
                .Include(u => u.Profile) // Tải thông tin hồ sơ đính kèm.
                .FirstOrDefaultAsync(u => u.Id == userId); // Tìm User theo UserId.

            if (user == null) // Nếu không tìm thấy người dùng.
                return NotFound(ApiResult<CustomerDto>.Fail("Không tìm thấy thông tin người dùng.")); // Trả về lỗi 404.

            if (user.Profile == null) // Nếu người dùng này chưa từng có hồ sơ chi tiết.
                user.Profile = new UserProfile { UserId = userId }; // Khởi tạo một đối tượng hồ sơ mới.

            user.Profile.FullName = request.FullName.Trim(); // Gán họ tên đầy đủ đã lược bỏ khoảng trắng thừa.
            user.Profile.Gender = request.Gender; // Cập nhật giới tính.
            user.Profile.DateOfBirth = request.DateOfBirth; // Cập nhật ngày sinh.
            user.Profile.AvatarUrl = request.AvatarUrl; // Cập nhật đường dẫn ảnh đại diện.
            user.Profile.Address = request.Address?.Trim(); // Cập nhật địa chỉ liên hệ.
            user.Profile.City = request.City?.Trim(); // Cập nhật thành phố cư trú.

            var result = await _userManager.UpdateAsync(user); // Thực hiện cập nhật thông tin tài khoản qua UserManager.
            if (!result.Succeeded) // Nếu cập nhật cơ sở dữ liệu thất bại.
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description)); // Ghép các mô tả lỗi thành một chuỗi.
                return BadRequest(ApiResult<CustomerDto>.Fail("Cập nhật thất bại. " + errors)); // Trả về lỗi 400 kèm các thông tin chi tiết.
            }

            return Ok(ApiResult<CustomerDto>.Ok(MapToDto(user), "Cập nhật thông tin thành công.")); // Trả về thông tin hồ sơ mới cập nhật thành công.
        }

        private Guid GetCurrentUserId() // Hàm phụ trợ lấy UserId từ Claims.
        {
            var str = User.FindFirstValue(ClaimTypes.NameIdentifier); // Tìm Claim NameIdentifier.
            return Guid.TryParse(str, out var id) ? id : Guid.Empty; // Trả về Guid hợp lệ hoặc Guid.Empty nếu parse lỗi.
        }

        private static CustomerDto MapToDto(AppUser user) => new() // Hàm tĩnh map thủ công thực thể AppUser sang CustomerDto.
        {
            Id = user.Id, // Mã tài khoản.
            Email = user.Email ?? string.Empty, // Địa chỉ Email.
            PhoneNumber = user.PhoneNumber ?? string.Empty, // Số điện thoại.
            FullName = user.Profile?.FullName ?? string.Empty, // Họ tên từ bảng Profile.
            Gender = user.Profile?.Gender ?? 0, // Giới tính từ bảng Profile.
            DateOfBirth = user.Profile?.DateOfBirth, // Ngày sinh.
            AvatarUrl = user.Profile?.AvatarUrl, // Ảnh đại diện.
            Address = user.Profile?.Address, // Địa chỉ.
            City = user.Profile?.City, // Thành phố.
            IsActive = user.IsActive, // Trạng thái hoạt động tài khoản.
            CreatedDate = user.CreatedDate // Ngày tạo tài khoản.
        };
    }
}

using Microsoft.AspNetCore.Identity; // Sử dụng thư viện ASP.NET Core Identity để quản lý người dùng và vai trò.
using Microsoft.Extensions.Caching.Memory; // Sử dụng thư viện lưu trữ bộ nhớ đệm (cache) trong bộ nhớ RAM.
using Microsoft.Extensions.Logging; // Sử dụng thư viện ghi log hệ thống.
using PBL3.Core.Entities; // Sử dụng các thực thực nghiệp vụ cốt lõi như AppUser, UserProfile từ tầng Core.
using PBL3.Core.Interfaces; // Sử dụng các giao diện để kết nối repository từ tầng Core.
using PBL3.Shared.DTOs.Common; // Sử dụng các DTO dùng chung như ApiResult, PagedResult.
using PBL3.Shared.DTOs.Employees; // Sử dụng các DTO liên quan đến quản lý nhân viên.
using PBL3.Shared.DTOs.Products; // Sử dụng các DTO liên quan đến sản phẩm.
using Microsoft.EntityFrameworkCore; // Sử dụng các phương thức mở rộng của EF Core.
using System; // Sử dụng các thư viện và kiểu dữ liệu hệ thống cơ bản.
using System.Collections.Generic; // Sử dụng cấu trúc danh sách như List, Dictionary.
using System.Linq; // Sử dụng các phương thức truy vấn LINQ.
using System.Threading.Tasks; // Sử dụng thư viện lập trình bất đồng bộ Task.

namespace PBL3.Application.Employees // Khai báo namespace cho lớp dịch vụ EmployeeService.
{
    public class EmployeeService( // Khởi tạo lớp EmployeeService sử dụng Primary Constructor (C# 12).
        IEmployeeRepository employeeRepo, // Tiêm repository quản lý thông tin nhân viên.
        UserManager<AppUser> userManager, // Tiêm UserManager để quản lý tài khoản người dùng ASP.NET Identity.
        RoleManager<AppRole> roleManager, // Tiêm RoleManager để quản lý các vai trò (roles) trong hệ thống.
        IMemoryCache cache, // Tiêm IMemoryCache phục vụ lưu trữ đệm dữ liệu.
        ILogger<EmployeeService> logger) : IEmployeeService // Tiêm Logger ghi nhận lịch sử hoạt động và kế thừa IEmployeeService.
    {
        private readonly IEmployeeRepository _employeeRepo = // Khai báo trường readonly lưu trữ repo nhân viên.
            employeeRepo ?? throw new ArgumentNullException(nameof(employeeRepo)); // Kiểm tra null và gán giá trị cho repo nhân viên.
        private readonly UserManager<AppUser> _userManager = // Khai báo trường readonly lưu trữ UserManager.
            userManager ?? throw new ArgumentNullException(nameof(userManager)); // Kiểm tra null và gán giá trị cho UserManager.
        private readonly RoleManager<AppRole> _roleManager = // Khai báo trường readonly lưu trữ RoleManager.
            roleManager ?? throw new ArgumentNullException(nameof(roleManager)); // Kiểm tra null và gán giá trị cho RoleManager.
        private readonly IMemoryCache _cache = // Khai báo trường readonly lưu trữ MemoryCache.
            cache ?? throw new ArgumentNullException(nameof(cache)); // Kiểm tra null và gán giá trị cho MemoryCache.
        private readonly ILogger<EmployeeService> _logger = // Khai báo trường readonly lưu trữ Logger ghi log.
            logger ?? throw new ArgumentNullException(nameof(logger)); // Kiểm tra null và gán giá trị cho Logger.

        public async Task<ApiResult<PagedResult<EmployeeListDto>>> GetPagedListAsync(EmployeeFilterRequest filter) // Phương thức lấy danh sách nhân viên phân trang bất đồng bộ.
        {
            var (items, totalCount) = await _employeeRepo.GetPagedListAsync( // Gọi repo lấy danh sách nhân viên và tổng số lượng bản ghi khớp bộ lọc.
                filter.Keyword, // Lọc theo từ khóa (Tên, Email, SĐT...).
                filter.IsActive, // Lọc theo trạng thái hoạt động của nhân viên.
                filter.Gender, // Lọc theo giới tính.
                filter.PageNumber, // Số thứ tự trang cần lấy.
                filter.PageSize, // Số lượng nhân viên trên mỗi trang.
                filter.SortBy, // Tên thuộc tính dùng để sắp xếp.
                filter.SortDescending); // Định nghĩa hướng sắp xếp (tăng hay giảm dần).

            var techRole = await _roleManager.FindByNameAsync("Technician"); // Tìm thông tin vai trò kỹ thuật viên (Technician) trong DB.
            var techUserIds = techRole == null ? new HashSet<Guid>() : // Nếu không có vai trò Technician, khởi tạo danh sách rỗng.
                (await _userManager.GetUsersInRoleAsync("Technician")).Select(u => u.Id).ToHashSet(); // Ngược lại, lấy danh sách Id của tất cả user thuộc vai trò này.

            var result = new PagedResult<EmployeeListDto> // Khởi tạo đối tượng phân trang kết quả.
            {
                Items = items.Select(u => MapToDto(u, techUserIds.Contains(u.Id))).ToList(), // Ánh xạ danh sách thực thể nhân viên sang DTO danh sách và kiểm tra xem có là kỹ thuật viên hay không.
                TotalCount = totalCount, // Gán tổng số lượng bản ghi.
                PageNumber = filter.PageNumber, // Gán số trang hiện tại.
                PageSize = filter.PageSize // Gán kích thước trang hiện tại.
            };

            return ApiResult<PagedResult<EmployeeListDto>>.Ok(result); // Trả về kết quả thành công chứa dữ liệu phân trang.
        }

        public async Task<ApiResult<EmployeeListDto>> GetByIdAsync(Guid id) // Phương thức lấy thông tin chi tiết nhân viên theo Id.
        {
            var user = await _employeeRepo.GetByIdWithProfileAsync(id); // Gọi repo lấy nhân viên kèm profile theo Id.
            if (user == null) // Nếu không tìm thấy nhân viên trong hệ thống.
                return ApiResult<EmployeeListDto>.Fail("Không tìm thấy nhân viên yêu cầu.", ApiErrorCode.NotFound); // Trả về lỗi NotFound.

            var isTechnician = await _userManager.IsInRoleAsync(user, "Technician"); // Kiểm tra xem nhân viên này có thuộc vai trò kỹ thuật viên (Technician) hay không.
            return ApiResult<EmployeeListDto>.Ok(MapToDto(user, isTechnician)); // Trả về kết quả thành công kèm DTO chi tiết nhân viên.
        }

        public async Task<ApiResult<EmployeeListDto>> CreateAsync(CreateEmployeeRequest request) // Phương thức tạo mới tài khoản nhân viên.
        {
            var existing = await _userManager.FindByEmailAsync(request.Email); // Kiểm tra email yêu cầu đã tồn tại trong DB chưa.
            if (existing != null) // Nếu email đã được sử dụng.
                return ApiResult<EmployeeListDto>.Fail("Email đã được sử dụng.", ApiErrorCode.Conflict); // Trả về lỗi xung đột thông tin (Conflict).

            var phoneExists = await _userManager.Users // Kiểm tra số điện thoại có trùng lặp với nhân viên đang hoạt động nào không.
                .AnyAsync(u => u.PhoneNumber == request.PhoneNumber && !u.IsDeleted); // Sử dụng AnyAsync truy vấn database bất đồng bộ.
            if (phoneExists) // Nếu số điện thoại đã được sử dụng.
                return ApiResult<EmployeeListDto>.Fail("Số điện thoại đã được sử dụng.", ApiErrorCode.Conflict); // Trả về lỗi trùng lặp số điện thoại.

            var user = new AppUser // Khởi tạo thực thể AppUser cho nhân viên mới.
            {
                Id = Guid.NewGuid(), // Sinh Id mới kiểu Guid ngẫu nhiên.
                UserName = request.Email, // Gán email làm tên đăng nhập.
                Email = request.Email, // Gán email.
                PhoneNumber = request.PhoneNumber, // Gán số điện thoại.
                IsActive = true, // Gán trạng thái hoạt động mặc định là true.
                Type = 1, // Loại tài khoản 1 (nhân viên).
                CreatedDate = DateTime.UtcNow, // Gán thời gian tạo là giờ UTC hiện tại.
                IsDeleted = false, // Trạng thái xóa mặc định là false.
                Profile = new UserProfile // Khởi tạo profile đi kèm cho nhân viên mới.
                {
                    FullName = request.FullName.Trim(), // Gán họ tên đã cắt khoảng trắng thừa.
                    Gender = request.Gender, // Gán giới tính.
                    DateOfBirth = request.DateOfBirth, // Gán ngày sinh.
                    Address = request.Address?.Trim(), // Gán địa chỉ đã cắt khoảng trắng thừa.
                    City = request.City?.Trim() // Gán thành phố đã cắt khoảng trắng thừa.
                }
            };

            var createResult = await _userManager.CreateAsync(user, request.Password); // Tạo tài khoản nhân viên và lưu mật khẩu đã băm vào DB.
            if (!createResult.Succeeded) // Nếu việc tạo tài khoản gặp lỗi.
            {
                var errors = string.Join(", ", createResult.Errors.Select(e => e.Description)); // Ghép các mô tả lỗi thành một chuỗi.
                _logger.LogWarning("Tạo tài khoản nhân viên thất bại: {Errors}", errors); // Ghi log cảnh báo lỗi.
                return ApiResult<EmployeeListDto>.Fail("Khởi tạo tài khoản thất bại. " + errors); // Trả về lỗi cùng chi tiết từ Identity.
            }

            await _userManager.AddToRoleAsync(user, "Employee"); // Thêm nhân viên vào vai trò cơ bản "Employee".
            if (request.IsTechnician) // Nếu có đánh dấu là kỹ thuật viên.
                await _userManager.AddToRoleAsync(user, "Technician"); // Đăng ký thêm vai trò "Technician" cho nhân viên này.

            _logger.LogInformation("Tạo tài khoản nhân viên thành công: {Email} (Id: {UserId})", user.Email, user.Id); // Ghi log thông báo tạo tài khoản thành công.

            return ApiResult<EmployeeListDto>.Ok(MapToDto(user, request.IsTechnician), "Tạo tài khoản nhân viên thành công."); // Trả về kết quả thành công kèm DTO.
        }

        public async Task<ApiResult<EmployeeListDto>> UpdateAsync(Guid id, UpdateEmployeeRequest request) // Phương thức cập nhật thông tin nhân viên.
        {
            var user = await _employeeRepo.GetByIdWithProfileAsync(id); // Lấy thông tin nhân viên kèm Profile theo Id.
            if (user == null) // Nếu không tìm thấy nhân viên.
                return ApiResult<EmployeeListDto>.Fail("Không tìm thấy nhân viên yêu cầu.", ApiErrorCode.NotFound); // Trả về lỗi NotFound.

            if (user.Profile == null) // Nếu chưa có Profile đi kèm.
                user.Profile = new UserProfile { UserId = user.Id }; // Tạo mới bản ghi Profile liên kết với Id nhân viên.

            user.Profile.FullName = request.FullName.Trim(); // Cập nhật họ tên nhân viên.
            user.Profile.Gender = request.Gender; // Cập nhật giới tính.
            user.Profile.DateOfBirth = request.DateOfBirth; // Cập nhật ngày sinh.
            user.Profile.AvatarUrl = request.AvatarUrl; // Cập nhật ảnh đại diện.
            user.Profile.Address = request.Address?.Trim(); // Cập nhật địa chỉ.
            user.Profile.City = request.City?.Trim(); // Cập nhật thành phố.

            var currentlyTechnician = await _userManager.IsInRoleAsync(user, "Technician"); // Kiểm tra xem nhân viên hiện tại có là kỹ thuật viên không.
            if (request.IsTechnician && !currentlyTechnician) // Nếu yêu cầu đặt là kỹ thuật viên nhưng hiện tại chưa phải.
                await _userManager.AddToRoleAsync(user, "Technician"); // Thêm vai trò Technician cho nhân viên.
            else if (!request.IsTechnician && currentlyTechnician) // Nếu yêu cầu bỏ kỹ thuật viên nhưng hiện tại đang là kỹ thuật viên.
                await _userManager.RemoveFromRoleAsync(user, "Technician"); // Xóa vai trò Technician khỏi nhân viên.

            var updateResult = await _userManager.UpdateAsync(user); // Lưu các thông tin thay đổi xuống DB.
            if (!updateResult.Succeeded) // Nếu cập nhật thất bại.
            {
                var errors = string.Join(", ", updateResult.Errors.Select(e => e.Description)); // Ghép các mô tả lỗi thành chuỗi.
                return ApiResult<EmployeeListDto>.Fail("Cập nhật thông tin thất bại. " + errors); // Trả về kết quả lỗi kèm thông tin chi tiết.
            }

            _logger.LogInformation("Cập nhật thông tin nhân viên: {Email} (Id: {UserId})", user.Email, user.Id); // Ghi log thông báo cập nhật thành công.
            return ApiResult<EmployeeListDto>.Ok(MapToDto(user, request.IsTechnician), "Cập nhật tài khoản thành công."); // Trả về kết quả thành công kèm DTO.
        }

        public async Task<ApiResult<bool>> DeactivateAsync(Guid id, string? lockReason) // Phương thức khóa tài khoản nhân viên.
        {
            var user = await _employeeRepo.GetByIdWithProfileAsync(id); // Lấy thông tin nhân viên từ repo.
            if (user == null) // Nếu không tìm thấy nhân viên.
                return ApiResult<bool>.Fail("Không tìm thấy nhân viên yêu cầu.", ApiErrorCode.NotFound); // Trả về lỗi NotFound.

            user.IsActive = false; // Đặt trạng thái hoạt động thành false để khóa tài khoản.
            user.LockReason = lockReason; // Lưu lý do khóa tài khoản.
            user.RefreshToken = null; // Xóa Refresh Token để ngắt phiên đăng nhập hiện tại lập tức.
            user.RefreshTokenExpiryTime = null; // Xóa thời gian hết hạn Refresh Token.

            var updateResult = await _userManager.UpdateAsync(user); // Lưu thông tin khóa tài khoản xuống DB.
            if (!updateResult.Succeeded) // Nếu lưu thất bại.
                return ApiResult<bool>.Fail("Cập nhật trạng thái thất bại."); // Trả về kết quả thất bại.

            _cache.Remove($"user_isactive_{id.ToString().ToLowerInvariant()}"); // Xóa cache kiểm tra trạng thái hoạt động của user này.
            _logger.LogInformation("Khóa tài khoản nhân viên: {Email} (Id: {UserId})", user.Email, user.Id); // Ghi log thông báo khóa tài khoản.

            return ApiResult<bool>.Ok(true, "Khóa tài khoản thành công."); // Trả về kết quả thành công.
        }

        public async Task<ApiResult<bool>> ReactivateAsync(Guid id) // Phương thức mở khóa tài khoản nhân viên.
        {
            var user = await _employeeRepo.GetByIdWithProfileAsync(id); // Lấy thông tin nhân viên theo Id từ repo.
            if (user == null) // Nếu không tìm thấy nhân viên.
                return ApiResult<bool>.Fail("Không tìm thấy nhân viên yêu cầu.", ApiErrorCode.NotFound); // Trả về lỗi NotFound.

            user.IsActive = true; // Đặt trạng thái hoạt động thành true để mở khóa tài khoản.
            user.LockReason = null; // Xóa lý do khóa tài khoản trước đó.

            var updateResult = await _userManager.UpdateAsync(user); // Lưu trạng thái mở khóa xuống DB.
            if (!updateResult.Succeeded) // Nếu cập nhật thất bại.
                return ApiResult<bool>.Fail("Cập nhật trạng thái thất bại."); // Trả về kết quả thất bại.

            await _userManager.SetLockoutEndDateAsync(user, null); // Xóa ngày hết hạn khóa tạm thời của Identity.
            await _userManager.ResetAccessFailedCountAsync(user); // Đặt số lần đăng nhập sai về 0.
            _cache.Remove($"user_isactive_{id.ToString().ToLowerInvariant()}"); // Xóa cache kiểm tra trạng thái hoạt động cũ.
            _logger.LogInformation("Mở khóa tài khoản nhân viên: {Email} (Id: {UserId})", user.Email, user.Id); // Ghi log thông báo mở khóa tài khoản.
            return ApiResult<bool>.Ok(true, "Mở khóa tài khoản thành công."); // Trả về kết quả thành công.
        }

        public async Task<ApiResult<List<EmployeeDto>>> GetTechniciansSimpleAsync() // Phương thức lấy danh sách rút gọn kỹ thuật viên đang hoạt động.
        {
            var (items, _) = await _employeeRepo.GetPagedListAsync(null, true, null, 1, 500, null, false); // Lấy tối đa 500 nhân viên đang hoạt động từ repo.
            var techUsers = await _userManager.GetUsersInRoleAsync("Technician"); // Lấy danh sách toàn bộ người dùng có vai trò Technician.
            var techIds = techUsers.Select(u => u.Id).ToHashSet(); // Trích xuất danh sách Id kỹ thuật viên và đưa vào cấu trúc HashSet để tìm kiếm nhanh.
            var result = items.Where(u => techIds.Contains(u.Id)) // Lọc ra các nhân viên thực sự mang vai trò kỹ thuật viên.
                .Select(u => new EmployeeDto { Id = u.Id, FullName = u.Profile?.FullName ?? u.Email ?? "" }) // Ánh xạ sang DTO đơn giản (gồm Id và họ tên/email).
                .ToList(); // Chuyển đổi kết quả Select sang danh sách List.
            return ApiResult<List<EmployeeDto>>.Ok(result); // Trả về kết quả thành công chứa danh sách kỹ thuật viên rút gọn.
        }

        private static EmployeeListDto MapToDto(AppUser user, bool isTechnician = false) => new() // Hàm phụ trợ ánh xạ thực thể AppUser sang EmployeeListDto.
        {
            Id = user.Id, // Ánh xạ Id.
            Email = user.Email ?? string.Empty, // Ánh xạ email.
            PhoneNumber = user.PhoneNumber ?? string.Empty, // Ánh xạ số điện thoại.
            FullName = user.Profile?.FullName ?? string.Empty, // Ánh xạ họ tên.
            Gender = user.Profile?.Gender ?? 0, // Ánh xạ giới tính.
            DateOfBirth = user.Profile?.DateOfBirth, // Ánh xạ ngày sinh.
            AvatarUrl = user.Profile?.AvatarUrl, // Ánh xạ ảnh đại diện.
            Address = user.Profile?.Address, // Ánh xạ địa chỉ.
            City = user.Profile?.City, // Ánh xạ thành phố.
            IsActive = user.IsActive, // Ánh xạ trạng thái hoạt động.
            LockReason = user.LockReason, // Ánh xạ lý do khóa.
            CreatedDate = user.CreatedDate, // Ánh xạ ngày tạo.
            IsTechnician = isTechnician // Ánh xạ cờ phân biệt có phải là kỹ thuật viên hay không.
        };
    }
}

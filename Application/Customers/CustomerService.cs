using Microsoft.AspNetCore.Identity; // Sử dụng thư viện ASP.NET Core Identity để quản lý tài khoản người dùng.
using Microsoft.Extensions.Caching.Memory; // Sử dụng thư viện lưu trữ bộ nhớ đệm (caching) trong RAM.
using Microsoft.Extensions.Logging; // Sử dụng thư viện ghi log hệ thống.
using PBL3.Core.Entities; // Sử dụng các thực thể nghiệp vụ cốt lõi như AppUser, UserProfile, Cart.
using PBL3.Core.Interfaces; // Sử dụng các interface định nghĩa các Repository và Service ở tầng Core.
using PBL3.Shared.DTOs.Common; // Sử dụng các DTO dùng chung như ApiResult, PagedResult.
using PBL3.Shared.DTOs.Customers; // Sử dụng các DTO liên quan đến quản lý khách hàng.
using PBL3.Shared.DTOs.Products; // Sử dụng các DTO liên quan đến sản phẩm (nếu có).
using Microsoft.EntityFrameworkCore; // Sử dụng các hàm mở rộng của Entity Framework Core.
using System; // Sử dụng các kiểu dữ liệu và thư viện cơ bản của hệ thống C#.
using System.Collections.Generic; // Sử dụng kiểu dữ liệu danh sách như List, Dictionary.
using System.Linq; // Sử dụng các phương thức truy vấn LINQ.
using System.Threading.Tasks; // Sử dụng các kiểu dữ liệu hỗ trợ lập trình bất đồng bộ Task.

namespace PBL3.Application.Customers // Định nghĩa namespace cho lớp dịch vụ CustomerService.
{
    public class CustomerService( // Khởi tạo lớp CustomerService sử dụng Primary Constructor.
        ICustomerRepository customerRepo, // Tiêm repository quản lý khách hàng.
        ICartRepository cartRepo, // Tiêm repository quản lý giỏ hàng của khách hàng.
        UserManager<AppUser> userManager, // Tiêm UserManager của ASP.NET Core Identity để quản lý tài khoản người dùng.
        IMemoryCache cache, // Tiêm IMemoryCache để quản lý bộ nhớ đệm.
        ILogger<CustomerService> logger) : ICustomerService // Tiêm Logger ghi log nghiệp vụ và kế thừa ICustomerService.
    {
        private readonly ICustomerRepository _customerRepo = // Khai báo biến lưu trữ repo khách hàng.
            customerRepo ?? throw new ArgumentNullException(nameof(customerRepo)); // Gán giá trị và ném lỗi nếu tham số truyền vào bị null.
        private readonly ICartRepository _cartRepo = // Khai báo biến lưu trữ repo giỏ hàng.
            cartRepo ?? throw new ArgumentNullException(nameof(cartRepo)); // Gán giá trị và ném lỗi nếu tham số truyền vào bị null.
        private readonly UserManager<AppUser> _userManager = // Khai báo biến lưu trữ UserManager.
            userManager ?? throw new ArgumentNullException(nameof(userManager)); // Gán giá trị và ném lỗi nếu tham số truyền vào bị null.
        private readonly IMemoryCache _cache = // Khai báo biến lưu trữ MemoryCache.
            cache ?? throw new ArgumentNullException(nameof(cache)); // Gán giá trị và ném lỗi nếu tham số truyền vào bị null.
        private readonly ILogger<CustomerService> _logger = // Khai báo biến lưu trữ Logger.
            logger ?? throw new ArgumentNullException(nameof(logger)); // Gán giá trị và ném lỗi nếu tham số truyền vào bị null.

        public async Task<ApiResult<PagedResult<CustomerDto>>> GetPagedListAsync(CustomerFilterRequest filter) // Định nghĩa phương thức lấy danh sách khách hàng phân trang bất đồng bộ.
        {
            var (items, totalCount) = await _customerRepo.GetPagedListAsync( // Gọi repo lấy danh sách khách hàng và tổng số bản ghi khớp bộ lọc.
                filter.Keyword, // Lọc theo từ khóa (Tên, Email, SĐT...).
                filter.IsActive, // Lọc theo trạng thái hoạt động của tài khoản.
                filter.Gender, // Lọc theo giới tính.
                filter.PageNumber, // Số thứ tự trang cần lấy.
                filter.PageSize, // Số lượng bản ghi trên một trang.
                filter.SortBy, // Tên thuộc tính sắp xếp.
                filter.SortDescending); // Quyết định sắp xếp giảm dần hay tăng dần.

            var dtos = items.Select(MapToDto).ToList(); // Ánh xạ danh sách thực thể User sang danh sách DTO.

            var result = new PagedResult<CustomerDto> // Tạo đối tượng kết quả phân trang PagedResult.
            {
                Items = dtos, // Gán danh sách khách hàng đã ánh xạ.
                TotalCount = totalCount, // Gán tổng số lượng bản ghi tìm được.
                PageNumber = filter.PageNumber, // Gán số trang hiện tại.
                PageSize = filter.PageSize // Gán kích thước trang hiện tại.
            };

            return ApiResult<PagedResult<CustomerDto>>.Ok(result); // Trả về kết quả thành công kèm dữ liệu phân trang.
        }

        public async Task<ApiResult<CustomerDetailDto>> GetByIdAsync(Guid id) // Định nghĩa phương thức lấy chi tiết khách hàng theo Id.
        {
            var user = await _customerRepo.GetByIdWithProfileAsync(id); // Truy vấn thông tin người dùng kèm theo UserProfile tương ứng.
            if (user == null) // Nếu không tồn tại người dùng trong hệ thống.
            {
                return ApiResult<CustomerDetailDto>.Fail("Không tìm thấy khách hàng yêu cầu.", ApiErrorCode.NotFound); // Trả về kết quả thất bại với mã lỗi NotFound.
            }

            var dto = new CustomerDetailDto // Khởi tạo DTO chi tiết khách hàng và gán giá trị tương ứng.
            {
                Id = user.Id, // Gán Id người dùng.
                Email = user.Email ?? string.Empty, // Gán Email, mặc định chuỗi rỗng nếu null.
                PhoneNumber = user.PhoneNumber ?? string.Empty, // Gán SĐT, mặc định chuỗi rỗng nếu null.
                FullName = user.Profile?.FullName ?? string.Empty, // Gán họ tên từ Profile liên kết.
                Gender = user.Profile?.Gender ?? 0, // Gán giới tính từ Profile.
                DateOfBirth = user.Profile?.DateOfBirth, // Gán ngày sinh từ Profile.
                AvatarUrl = user.Profile?.AvatarUrl, // Gán link ảnh đại diện từ Profile.
                Address = user.Profile?.Address, // Gán địa chỉ từ Profile.
                City = user.Profile?.City, // Gán thành phố từ Profile.
                IsActive = user.IsActive, // Gán trạng thái hoạt động.
                CreatedDate = user.CreatedDate // Gán ngày tạo tài khoản.
            };

            var recentOrders = await _customerRepo.GetRecentOrdersAsync(id, 10); // Lấy danh sách tối đa 10 đơn hàng gần đây của khách hàng này.
            dto.RecentOrders = recentOrders.Select(o => new CustomerOrderHistoryDto // Ánh xạ danh sách đơn hàng lịch sử sang DTO tương ứng.
            {
                Id = o.Id, // Gán Id đơn hàng.
                OrderCode = o.OrderCode, // Gán mã đơn hàng.
                OrderDate = o.OrderDate, // Gán ngày đặt hàng.
                TotalAmount = o.TotalAmount, // Gán tổng tiền đơn hàng.
                Status = o.Status // Gán trạng thái đơn hàng.
            }).ToList(); // Chuyển đổi kết quả Select sang dạng danh sách.

            var cartItems = await _cartRepo.GetCartItemsByUserAsync(id); // Lấy danh sách các sản phẩm đang có trong giỏ hàng của khách hàng này.
            dto.CartItems = cartItems.Select(c => new CustomerCartItemDto // Ánh xạ danh sách giỏ hàng sang DTO giỏ hàng tương ứng.
            {
                Id = c.Id, // Gán Id bản ghi giỏ hàng.
                VariantId = c.VariantId, // Gán Id biến thể sản phẩm.
                ProductName = c.Variant?.Product?.Name ?? string.Empty, // Gán tên sản phẩm liên kết.
                VariantName = c.Variant?.VariantName ?? string.Empty, // Gán tên biến thể liên kết.
                Price = c.Variant?.Price ?? 0, // Gán đơn giá biến thể.
                Quantity = c.Quantity, // Gán số lượng sản phẩm trong giỏ.
                ThumbnailUrl = c.Variant?.Images?.FirstOrDefault(i => i.IsMain)?.ImageUrl // Lấy ảnh đại diện chính của biến thể sản phẩm.
            }).ToList(); // Chuyển kết quả Select sang List.

            return ApiResult<CustomerDetailDto>.Ok(dto); // Trả về kết quả thành công kèm DTO chi tiết của khách hàng.
        }

        public async Task<ApiResult<CustomerDto>> CreateAsync(CreateCustomerRequest request) // Định nghĩa phương thức tạo mới một tài khoản khách hàng.
        {
            if (!string.IsNullOrWhiteSpace(request.Email)) // Nếu email yêu cầu tạo mới không trống.
            {
                var existingUserByEmail = await _userManager.FindByEmailAsync(request.Email); // Kiểm tra xem email đã được sử dụng hay chưa qua UserManager.
                if (existingUserByEmail != null) // Nếu email đã tồn tại trong hệ thống.
                    return ApiResult<CustomerDto>.Fail("Email đã được sử dụng.", ApiErrorCode.Conflict); // Trả về kết quả thất bại với mã lỗi xung đột thông tin (Conflict).
            }

            if (!string.IsNullOrWhiteSpace(request.PhoneNumber)) // Nếu số điện thoại yêu cầu tạo mới không trống.
            {
                var phoneExists = await _userManager.Users // Kiểm tra xem số điện thoại có trùng lặp với tài khoản chưa bị xóa nào không.
                    .AnyAsync(u => u.PhoneNumber == request.PhoneNumber && !u.IsDeleted); // Sử dụng AnyAsync truy vấn database bất đồng bộ.
                if (phoneExists) // Nếu số điện thoại đã tồn tại.
                    return ApiResult<CustomerDto>.Fail("Số điện thoại đã được sử dụng.", ApiErrorCode.Conflict); // Trả về kết quả thất bại với mã lỗi xung đột thông tin.
            }

            var user = new AppUser // Khởi tạo đối tượng tài khoản người dùng AppUser mới.
            {
                Id = Guid.NewGuid(), // Sinh Id mới ngẫu nhiên kiểu Guid.
                UserName = request.Email, // Thiết kế sử dụng email làm tên đăng nhập (username).
                Email = request.Email, // Gán email người dùng.
                PhoneNumber = request.PhoneNumber, // Gán số điện thoại.
                IsActive = true, // Kích hoạt tài khoản mặc định.
                Type = 2, // Đánh dấu loại tài khoản là 2 (tương ứng với vai trò khách hàng).
                CreatedDate = DateTime.UtcNow, // Gán thời gian tạo tài khoản theo giờ UTC.
                IsDeleted = false, // Trạng thái xóa mặc định là false.
                Profile = new UserProfile // Khởi tạo UserProfile đi kèm cho tài khoản mới.
                {
                    FullName = request.FullName.Trim(), // Họ tên đã cắt bỏ khoảng trắng đầu/cuối.
                    Gender = request.Gender, // Gán giới tính.
                    DateOfBirth = request.DateOfBirth, // Gán ngày sinh.
                    Address = request.Address?.Trim(), // Gán địa chỉ đã cắt bỏ khoảng trắng thừa.
                    City = request.City?.Trim() // Gán thành phố đã cắt bỏ khoảng trắng thừa.
                }
            };

            var generatedPassword = GenerateRandomPassword(12); // Tự động sinh mật khẩu ngẫu nhiên có độ dài 12 ký tự cho khách hàng.

            var createResult = await _userManager.CreateAsync(user, generatedPassword); // Gọi UserManager để tạo tài khoản người dùng và mã hóa mật khẩu xuống DB.
            if (!createResult.Succeeded) // Nếu quá trình tạo tài khoản gặp lỗi.
            {
                var errors = string.Join(", ", createResult.Errors.Select(e => e.Description)); // Ghép danh sách lỗi thành chuỗi thông báo.
                _logger.LogWarning("Tạo tài khoản thất bại: {Errors}", errors); // Ghi log cảnh báo lỗi tạo tài khoản.
                return ApiResult<CustomerDto>.Fail("Khởi tạo tài khoản thất bại. " + errors); // Trả về kết quả thất bại kèm mô tả chi tiết lỗi từ ASP.NET Identity.
            }

            await _userManager.AddToRoleAsync(user, "Customer"); // Đăng ký vai trò "Customer" cho tài khoản người dùng mới tạo.

            _logger.LogInformation("Tạo tài khoản khách hàng thành công: {Email} (Id: {UserId})", user.Email, user.Id); // Ghi log thông báo tạo tài khoản thành công.

            return ApiResult<CustomerDto>.Ok(MapToDto(user), "Tạo tài khoản thành công."); // Trả về kết quả thành công kèm DTO người dùng mới và thông báo.
        }

        public async Task<ApiResult<CustomerDto>> UpdateAsync(Guid id, UpdateCustomerRequest request) // Định nghĩa phương thức cập nhật thông tin khách hàng.
        {
            var user = await _customerRepo.GetByIdWithProfileAsync(id); // Lấy thông tin khách hàng kèm Profile dựa trên Id.
            if (user == null) // Nếu không tìm thấy khách hàng.
                return ApiResult<CustomerDto>.Fail("Không tìm thấy khách hàng yêu cầu.", ApiErrorCode.NotFound); // Trả về kết quả thất bại với mã lỗi NotFound.

            if (user.Profile == null) // Nếu tài khoản này chưa từng có bản ghi Profile đi kèm.
            {
                user.Profile = new UserProfile { UserId = user.Id }; // Khởi tạo một đối tượng Profile mới liên kết với UserId.
            }

            user.Profile.FullName = request.FullName.Trim(); // Cập nhật họ tên khách hàng.
            user.Profile.Gender = request.Gender; // Cập nhật giới tính khách hàng.
            user.Profile.DateOfBirth = request.DateOfBirth; // Cập nhật ngày sinh.
            user.Profile.AvatarUrl = request.AvatarUrl; // Cập nhật đường dẫn ảnh đại diện mới.
            user.Profile.Address = request.Address?.Trim(); // Cập nhật địa chỉ.
            user.Profile.City = request.City?.Trim(); // Cập nhật thành phố.

            var updateResult = await _userManager.UpdateAsync(user); // Lưu các thông tin thay đổi của người dùng xuống cơ sở dữ liệu qua UserManager.
            if (!updateResult.Succeeded) // Nếu quá trình cập nhật cơ sở dữ liệu thất bại.
            {
                var errors = string.Join(", ", updateResult.Errors.Select(e => e.Description)); // Ghép các thông báo lỗi thành chuỗi.
                return ApiResult<CustomerDto>.Fail("Cập nhật thông tin thất bại. " + errors); // Trả về kết quả thất bại kèm thông báo lỗi chi tiết.
            }

            _logger.LogInformation("Cập nhật thông tin khách hàng: {Email} (Id: {UserId})", user.Email, user.Id); // Ghi log thông tin cập nhật thành công.

            return ApiResult<CustomerDto>.Ok(MapToDto(user), "Cập nhật tài khoản thành công."); // Trả về kết quả thành công kèm thông tin DTO mới của khách hàng.
        }

        public async Task<ApiResult<bool>> DeactivateAsync(Guid id, string? lockReason) // Định nghĩa phương thức vô hiệu hóa (khóa) tài khoản khách hàng.
        {
            var user = await _customerRepo.GetByIdWithProfileAsync(id); // Lấy thông tin khách hàng từ repository theo Id.
            if (user == null) // Nếu không tìm thấy khách hàng.
                return ApiResult<bool>.Fail("Không tìm thấy khách hàng yêu cầu.", ApiErrorCode.NotFound); // Trả về kết quả thất bại với mã lỗi không tìm thấy.

            var hasPendingOrders = await _customerRepo.HasPendingOrdersAsync(id); // Kiểm tra khách hàng có đơn hàng nào đang chờ xử lý hay không.
            if (hasPendingOrders) // Nếu có đơn hàng chờ xử lý.
            {
                return ApiResult<bool>.Fail("Tài khoản đang có đơn hàng chờ xử lý, không thể khóa."); // Trả về kết quả lỗi vì không thỏa mãn điều kiện khóa.
            }

            user.IsActive = false; // Thiết lập trạng thái hoạt động thành false để khóa tài khoản.
            user.LockReason = lockReason; // Lưu lý do khóa tài khoản.
            user.RefreshToken = null; // Xóa Refresh Token để bắt người dùng phải đăng nhập lại ngay lập tức.
            user.RefreshTokenExpiryTime = null; // Xóa thời gian hết hạn của Refresh Token.

            var updateResult = await _userManager.UpdateAsync(user); // Lưu các thay đổi khóa tài khoản xuống DB qua UserManager.
            if (!updateResult.Succeeded) // Nếu cập nhật DB thất bại.
            {
                return ApiResult<bool>.Fail("Cập nhật trạng thái thất bại."); // Trả về kết quả thất bại kiểu bool.
            }

            _cache.Remove($"user_isactive_{id.ToString().ToLowerInvariant()}"); // Xóa cache kiểm tra trạng thái hoạt động của user này để hệ thống cập nhật lập tức ở request sau.
            _logger.LogInformation("Khóa tài khoản khách hàng: {Email} (Id: {UserId})", user.Email, user.Id); // Ghi log thông báo khóa tài khoản thành công.

            return ApiResult<bool>.Ok(true, "Khóa tài khoản thành công."); // Trả về kết quả thành công.
        }

        public async Task<ApiResult<bool>> ReactivateAsync(Guid id) // Định nghĩa phương thức mở khóa tài khoản khách hàng.
        {
            var user = await _customerRepo.GetByIdWithProfileAsync(id); // Lấy thông tin khách hàng từ repository.
            if (user == null) // Nếu không tìm thấy khách hàng.
                return ApiResult<bool>.Fail("Không tìm thấy khách hàng yêu cầu.", ApiErrorCode.NotFound); // Trả về kết quả thất bại với mã lỗi NotFound.

            user.IsActive = true; // Thiết lập trạng thái hoạt động thành true để kích hoạt lại tài khoản.
            user.LockReason = null; // Xóa lý do khóa tài khoản trước đó.

            var updateResult = await _userManager.UpdateAsync(user); // Lưu trạng thái kích hoạt tài khoản xuống DB.
            if (!updateResult.Succeeded) // Nếu cập nhật thất bại.
                return ApiResult<bool>.Fail("Cập nhật trạng thái thất bại."); // Trả về kết quả thất bại kiểu bool.

            await _userManager.SetLockoutEndDateAsync(user, null); // Xóa cấu hình ngày hết hạn khóa tài khoản của ASP.NET Identity (nếu có).
            await _userManager.ResetAccessFailedCountAsync(user); // Đặt lại số lần đăng nhập sai của người dùng về 0.
            _cache.Remove($"user_isactive_{id.ToString().ToLowerInvariant()}"); // Xóa cache kiểm tra trạng thái hoạt động cũ của người dùng này.
            _logger.LogInformation("Mở khóa tài khoản khách hàng: {Email} (Id: {UserId})", user.Email, user.Id); // Ghi log thông báo mở khóa tài khoản thành công.
            return ApiResult<bool>.Ok(true, "Mở khóa tài khoản thành công."); // Trả về kết quả thành công.
        }

        private static CustomerDto MapToDto(AppUser user) // Định nghĩa phương thức phụ trợ ánh xạ từ thực thể AppUser sang CustomerDto.
        {
            return new CustomerDto // Khởi tạo DTO mới và gán dữ liệu.
            {
                Id = user.Id, // Ánh xạ Id.
                Email = user.Email ?? string.Empty, // Ánh xạ Email (mặc định trống nếu null).
                PhoneNumber = user.PhoneNumber ?? string.Empty, // Ánh xạ số điện thoại.
                FullName = user.Profile?.FullName ?? string.Empty, // Ánh xạ họ tên.
                Gender = user.Profile?.Gender ?? 0, // Ánh xạ giới tính.
                DateOfBirth = user.Profile?.DateOfBirth, // Ánh xạ ngày sinh.
                AvatarUrl = user.Profile?.AvatarUrl, // Ánh xạ đường dẫn ảnh đại diện.
                Address = user.Profile?.Address, // Ánh xạ địa chỉ.
                City = user.Profile?.City, // Ánh xạ thành phố.
                IsActive = user.IsActive, // Ánh xạ trạng thái hoạt động.
                LockReason = user.LockReason, // Ánh xạ lý do khóa tài khoản.
                CreatedDate = user.CreatedDate // Ánh xạ ngày tạo tài khoản.
            };
        }

        private static string GenerateRandomPassword(int length) // Định nghĩa hàm sinh mật khẩu ngẫu nhiên an toàn.
        {
            const string lowercase = "abcdefghijklmnopqrstuvwxyz"; // Tập hợp các ký tự chữ thường.
            const string uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ"; // Tập hợp các ký tự chữ hoa.
            const string digits = "1234567890"; // Tập hợp các ký tự số.
            const string special = "!@#$%^&*()"; // Tập hợp các ký tự đặc biệt.

            var random = new Random(); // Khởi tạo bộ sinh số ngẫu nhiên Random.
            var password = new char[length]; // Khởi tạo mảng ký tự lưu mật khẩu với chiều dài quy định.

            password[0] = lowercase[random.Next(lowercase.Length)]; // Chọn ngẫu nhiên 1 ký tự chữ thường cho ký tự đầu tiên để thỏa mãn yêu cầu phức tạp của mật khẩu.
            password[1] = uppercase[random.Next(uppercase.Length)]; // Chọn ngẫu nhiên 1 ký tự chữ hoa cho ký tự thứ hai.
            password[2] = digits[random.Next(digits.Length)]; // Chọn ngẫu nhiên 1 chữ số cho ký tự thứ ba.
            password[3] = special[random.Next(special.Length)]; // Chọn ngẫu nhiên 1 ký tự đặc biệt cho ký tự thứ tư.

            var allChars = lowercase + uppercase + digits + special; // Ghép tất cả các nhóm ký tự lại để chọn ngẫu nhiên cho các vị trí còn lại.
            for (int i = 4; i < length; i++) // Vòng lặp gán giá trị cho các vị trí từ index 4 đến hết chiều dài mật khẩu.
            {
                password[i] = allChars[random.Next(allChars.Length)]; // Chọn ngẫu nhiên một ký tự bất kỳ trong tập hợp chung.
            }

            for (int i = 0; i < length; i++) // Vòng lặp trộn (shuffle) các ký tự trong mảng mật khẩu để đảm bảo tính ngẫu nhiên của các ký tự đặc biệt ở đầu.
            {
                int r = i + random.Next(length - i); // Lấy một chỉ số ngẫu nhiên từ vị trí hiện tại đến cuối mảng.
                var temp = password[r]; // Lưu tạm giá trị ký tự tại chỉ số ngẫu nhiên.
                password[r] = password[i]; // Đổi chỗ ký tự hiện tại sang vị trí ngẫu nhiên.
                password[i] = temp; // Đổi ký tự tạm vào vị trí hiện tại.
            }

            return new string(password); // Tạo và trả về chuỗi mật khẩu hoàn chỉnh từ mảng ký tự.
        }
    }
}

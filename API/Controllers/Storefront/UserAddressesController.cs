using Microsoft.AspNetCore.Authorization; // Sử dụng để phân quyền truy cập.
using Microsoft.AspNetCore.Mvc; // Sử dụng các thành phần MVC cho Web API.
using PBL3.Core.Interfaces; // Sử dụng các giao diện tương tác với cơ sở dữ liệu (IUserAddressRepository, IUnitOfWork).
using PBL3.Shared.DTOs.Common; // Sử dụng các DTO dùng chung như ApiResult.
using PBL3.Shared.DTOs.Customers; // Sử dụng các DTO liên quan đến địa chỉ khách hàng.
using System.Security.Claims; // Sử dụng Claims để xác định định danh người dùng.

namespace PBL3.API.Controllers.Storefront // Khai báo namespace cho các Controller thuộc Storefront.
{
    [ApiController] // Khai báo đây là một Web API Controller hỗ trợ tự động validate model.
    [Route("api/storefront/user-addresses")] // Định nghĩa route truy cập: api/storefront/user-addresses.
    [Produces("application/json")] // Quy định định dạng trả về mặc định dạng JSON.
    [Authorize] // Yêu cầu người dùng đăng nhập mới được truy cập.
    public class UserAddressesController : ControllerBase // Định nghĩa lớp UserAddressesController kế thừa từ ControllerBase.
    {
        private readonly IUserAddressRepository _repository; // Khai báo trường repository tương tác dữ liệu địa chỉ.
        private readonly IUnitOfWork _unitOfWork; // Khai báo trường UnitOfWork quản lý transaction.

        public UserAddressesController(IUserAddressRepository repository, IUnitOfWork unitOfWork) // Constructor injection tiêm repository và unitOfWork.
        {
            _repository = repository; // Gán repository được tiêm.
            _unitOfWork = unitOfWork; // Gán unitOfWork được tiêm.
        }

        /// <summary>
        /// Lấy danh sách địa chỉ giao hàng của người dùng hiện tại
        /// </summary>
        [HttpGet("my-addresses")] // Định nghĩa HTTP GET Method lấy danh sách địa chỉ giao hàng của người dùng (api/storefront/user-addresses/my-addresses).
        [ProducesResponseType(typeof(ApiResult<List<UserAddressDto>>), StatusCodes.Status200OK)] // Trả về danh sách địa chỉ thành công.
        public async Task<IActionResult> GetMyAddresses() // Lấy toàn bộ địa chỉ giao hàng đã lưu của người dùng đang đăng nhập.
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier); // Lấy UserId từ JWT Claims.
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId)) // Kiểm tra xem người dùng đã đăng nhập chưa.
            {
                return Unauthorized(ApiResult<List<UserAddressDto>>.Fail("Người dùng chưa đăng nhập.")); // Trả về lỗi 401 Unauthorized.
            }

            var addresses = await _repository.GetByUserIdAsync(userId); // Gọi repository lấy danh sách địa chỉ từ Database theo UserId.

            var dtos = addresses.Select(a => new UserAddressDto // Ánh xạ danh sách thực thể UserAddress sang DTO.
            {
                Id = a.Id, // Mã địa chỉ.
                UserId = a.UserId, // Mã người dùng sở hữu.
                ReceiverName = a.ReceiverName, // Tên người nhận hàng.
                PhoneNumber = a.PhoneNumber, // Số điện thoại nhận hàng.
                AddressLine = a.AddressLine, // Địa chỉ chi tiết (số nhà, đường...).
                City = a.City, // Tỉnh/Thành phố.
                IsDefault = a.IsDefault // Đánh dấu là địa chỉ giao hàng mặc định.
            }).ToList(); // Chuyển đổi kết quả thành List.

            return Ok(ApiResult<List<UserAddressDto>>.Ok(dtos, "Lấy danh sách địa chỉ thành công.")); // Trả về kết quả thành công kèm danh sách DTO.
        }

        /// <summary>
        /// Thêm mới một địa chỉ giao hàng
        /// </summary>
        [HttpPost] // Định nghĩa HTTP POST Method thêm địa chỉ mới (api/storefront/user-addresses).
        [ProducesResponseType(typeof(ApiResult<int>), StatusCodes.Status201Created)] // Thêm mới thành công.
        public async Task<IActionResult> AddAddress([FromBody] UserAddressDto request) // Thêm mới địa chỉ giao nhận hàng.
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier); // Lấy UserId của tài khoản hiện tại từ JWT Claims.
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId)) // Kiểm tra trạng thái đăng nhập.
            {
                return Unauthorized(ApiResult<int>.Fail("Người dùng chưa đăng nhập.")); // Trả về lỗi 401.
            }

            if (request.IsDefault) // Nếu địa chỉ mới thêm được cài đặt làm mặc định.
            {
                await _repository.ClearUserDefaultsAsync(userId); // Xóa trạng thái mặc định của tất cả các địa chỉ cũ khác của người dùng này.
            }

            var entity = new PBL3.Core.Entities.UserAddress // Khởi tạo thực thể UserAddress mới từ dữ liệu gửi lên.
            {
                UserId = userId, // Gán UserId của người dùng hiện tại.
                ReceiverName = request.ReceiverName, // Gán tên người nhận.
                PhoneNumber = request.PhoneNumber, // Gán số điện thoại.
                AddressLine = request.AddressLine, // Gán địa chỉ chi tiết.
                City = request.City, // Gán tỉnh thành.
                IsDefault = request.IsDefault // Gán trạng thái mặc định.
            };

            await _repository.AddAsync(entity); // Đưa thực thể mới vào DbContext theo dõi.
            await _unitOfWork.SaveChangesAsync(); // Lưu thay đổi xuống cơ sở dữ liệu để tạo Id tự tăng cho bản ghi.

            return Ok(ApiResult<int>.Ok(entity.Id, "Thêm địa chỉ thành công.")); // Trả về HTTP 200 OK kèm mã địa chỉ vừa được tạo.
        }
    }
}

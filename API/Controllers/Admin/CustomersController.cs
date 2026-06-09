using Microsoft.AspNetCore.Authorization; // Sử dụng để phân quyền truy cập (Authorize).
using Microsoft.AspNetCore.Mvc; // Sử dụng các thành phần MVC như Controller, Action, HttpGet, HttpPost...
using PBL3.Application.Customers; // Sử dụng tầng dịch vụ quản lý khách hàng ICustomerService.
using PBL3.Shared.DTOs.Common; // Sử dụng các DTO dùng chung như ApiResult, PagedResult.
using PBL3.Shared.DTOs.Customers; // Sử dụng các DTO liên quan đến khách hàng.
using PBL3.Shared.DTOs.Products; // Sử dụng các DTO liên quan đến sản phẩm.
using System; // Sử dụng thư viện hệ thống cơ bản.
using System.Threading.Tasks; // Sử dụng thư viện lập trình bất đồng bộ.
using PBL3.API.Extensions; // Sử dụng phương thức mở rộng ToActionResult để map kết quả trả về.

namespace PBL3.API.Controllers.Admin // Khai báo namespace cho Controllers thuộc khu vực quản trị (Admin).
{
    [ApiController] // Khai báo đây là một API Controller có sẵn cơ chế tự động validate Model.
    [Route("api/[controller]")] // Định nghĩa route truy cập: api/customers.
    [Produces("application/json")] // Thiết lập định dạng dữ liệu trả về mặc định là JSON.
    [Authorize(Roles = "Admin, Employee")] // Yêu cầu xác thực tài khoản thuộc vai trò Admin hoặc Employee mới được truy cập.
    public class CustomersController : ControllerBase // Định nghĩa lớp CustomersController kế thừa từ ControllerBase.
    {
        private readonly ICustomerService _customerService; // Khai báo service quản lý khách hàng.

        public CustomersController(ICustomerService customerService) // Constructor injection tiêm ICustomerService từ DI container.
        {
            _customerService = customerService; // Gán service được tiêm vào trường nội bộ.
        }

        /// <summary>
        /// Lấy danh sách khách hàng (phân trang, tìm kiếm theo Tên, Email, SĐT).
        /// </summary>
        [HttpGet] // Định nghĩa HTTP GET Method cho endpoint api/customers.
        [ProducesResponseType(typeof(ApiResult<PagedResult<CustomerDto>>), StatusCodes.Status200OK)] // Định nghĩa kiểu dữ liệu trả về khi thành công (200 OK).
        public async Task<IActionResult> GetList([FromQuery] CustomerFilterRequest filter) // Lấy danh sách khách hàng dựa trên bộ lọc từ Query String.
        {
            var result = await _customerService.GetPagedListAsync(filter); // Gọi service để lấy danh sách khách hàng phân trang.
            return Ok(result); // Trả về HTTP 200 OK cùng kết quả.
        }

        /// <summary>
        /// Lấy chi tiết khách hàng theo Id (bao gồm 10 đơn hàng gần nhất + giỏ hàng).
        /// </summary>
        [HttpGet("{id:guid}")] // Định nghĩa GET Method với tham số id dạng Guid (api/customers/{id}).
        [ProducesResponseType(typeof(ApiResult<CustomerDetailDto>), StatusCodes.Status200OK)] // Phản hồi thành công.
        [ProducesResponseType(typeof(ApiResult<CustomerDetailDto>), StatusCodes.Status404NotFound)] // Phản hồi khi không tìm thấy.
        public async Task<IActionResult> GetById(Guid id) // Lấy thông tin chi tiết khách hàng theo Id.
        {
            var result = await _customerService.GetByIdAsync(id); // Gọi service lấy chi tiết khách hàng.
            return result.ToActionResult(this); // Sử dụng ToActionResult chuyển đổi ApiResult sang IActionResult tương ứng.
        }

        /// <summary>
        /// Tạo mới tài khoản khách hàng (Admin tạo, hệ thống tự sinh mật khẩu).
        /// </summary>
        [HttpPost] // Định nghĩa HTTP POST Method để tạo khách hàng mới (api/customers).
        [ProducesResponseType(typeof(ApiResult<CustomerDto>), StatusCodes.Status201Created)] // Tạo thành công trả về HTTP 201.
        [ProducesResponseType(typeof(ApiResult<CustomerDto>), StatusCodes.Status400BadRequest)] // Dữ liệu không hợp lệ trả về HTTP 400.
        public async Task<IActionResult> Create([FromBody] CreateCustomerRequest request) // Tạo khách hàng nhận thông tin từ Request Body.
        {
            var result = await _customerService.CreateAsync(request); // Gọi service tạo mới khách hàng.

            if (!result.Success) return result.ToActionResult(this); // Nếu tạo không thành công thì trả về lỗi tương ứng.

            return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result); // Trả về HTTP 201 Created cùng đường dẫn lấy thông tin vừa tạo.
        }

        /// <summary>
        /// Cập nhật thông tin khách hàng (không cho sửa Email/SĐT).
        /// </summary>
        [HttpPut("{id:guid}")] // Định nghĩa HTTP PUT Method cập nhật khách hàng theo Id (api/customers/{id}).
        [ProducesResponseType(typeof(ApiResult<CustomerDto>), StatusCodes.Status200OK)] // Cập nhật thành công.
        [ProducesResponseType(typeof(ApiResult<CustomerDto>), StatusCodes.Status400BadRequest)] // Lỗi đầu vào.
        [ProducesResponseType(typeof(ApiResult<CustomerDto>), StatusCodes.Status404NotFound)] // Không tìm thấy.
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCustomerRequest request) // Cập nhật khách hàng theo Id.
        {
            var result = await _customerService.UpdateAsync(id, request); // Gọi service cập nhật thông tin.
            return result.ToActionResult(this); // Trả về kết quả qua extension ToActionResult.
        }

        /// <summary>
        /// Khóa tài khoản khách hàng (Deactivate + thu hồi RefreshToken).
        /// </summary>
        [HttpDelete("{id:guid}")] // Định nghĩa HTTP DELETE Method khóa tài khoản theo Id (api/customers/{id}).
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status200OK)] // Khóa thành công.
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status400BadRequest)] // Yêu cầu lỗi.
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status404NotFound)] // Không tìm thấy.
        public async Task<IActionResult> Deactivate(Guid id, [FromQuery] string? lockReason = null) // Khóa tài khoản kèm lý do tùy chọn.
        {
            var result = await _customerService.DeactivateAsync(id, lockReason); // Gọi service khóa tài khoản.
            return result.ToActionResult(this); // Trả về kết quả qua extension ToActionResult.
        }

        /// <summary>
        /// Mở khóa tài khoản khách hàng.
        /// </summary>
        [HttpPut("{id:guid}/activate")] // Định nghĩa HTTP PUT Method mở khóa (api/customers/{id}/activate).
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status200OK)] // Thành công.
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status404NotFound)] // Không tìm thấy.
        public async Task<IActionResult> Reactivate(Guid id) // Mở khóa tài khoản khách hàng theo Id.
        {
            var result = await _customerService.ReactivateAsync(id); // Gọi service kích hoạt lại tài khoản.
            return result.ToActionResult(this); // Trả về kết quả qua extension ToActionResult.
        }
    }
}

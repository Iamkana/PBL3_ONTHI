using Microsoft.AspNetCore.Authorization; // Sử dụng để phân quyền truy cập (Authorize).
using Microsoft.AspNetCore.Mvc; // Sử dụng các lớp MVC của ASP.NET Core.
using PBL3.Application.Banners; // Sử dụng tầng dịch vụ banner IBannerService.
using PBL3.Shared.DTOs.Banners; // Sử dụng các DTO liên quan đến banner.
using PBL3.Shared.DTOs.Common; // Sử dụng các DTO dùng chung như ApiResult, PagedResult.
using PBL3.API.Extensions; // Sử dụng phương thức mở rộng ToActionResult.

namespace PBL3.API.Controllers.Admin // Khai báo namespace cho Controllers quản trị.
{
    [ApiController] // Khai báo lớp là một Web API Controller có sẵn cơ chế xác thực model.
    [Route("api/[controller]")] // Định nghĩa route truy cập: api/banners.
    [Produces("application/json")] // Định dạng dữ liệu phản hồi là JSON.
    public class BannersController : ControllerBase // Định nghĩa lớp BannersController kế thừa từ ControllerBase.
    {
        private readonly IBannerService _bannerService; // Khai báo service quản lý banner.

        public BannersController(IBannerService bannerService) // Constructor injection tiêm IBannerService từ DI container.
        {
            _bannerService = bannerService; // Gán service được tiêm.
        }

        /// <summary>
        /// Lấy danh sách banner (phân trang, tìm kiếm theo tiêu đề). Chỉ Admin.
        /// </summary>
        [HttpGet] // Định nghĩa HTTP GET Method cho api/banners.
        [Authorize(Roles = "Admin")] // Chỉ tài khoản có vai trò Admin được phép truy cập.
        [ProducesResponseType(typeof(ApiResult<PagedResult<BannerDto>>), StatusCodes.Status200OK)] // Trả về danh sách banner phân trang.
        public async Task<IActionResult> GetList([FromQuery] BannerFilterRequest filter) // Lấy danh sách dựa trên bộ lọc từ Query String.
        {
            var result = await _bannerService.GetPagedListAsync(filter); // Gọi service lấy danh sách phân trang.
            return Ok(result); // Trả về HTTP 200 OK.
        }

        /// <summary>
        /// Lấy danh sách banner đang hiệu lực. Public — dùng cho trang chủ.
        /// </summary>
        [HttpGet("active")] // Định nghĩa HTTP GET Method cho api/banners/active.
        [AllowAnonymous] // Cho phép truy cập công khai không cần đăng nhập (để hiển thị trên trang chủ client).
        [ProducesResponseType(typeof(ApiResult<List<BannerPublicDto>>), StatusCodes.Status200OK)] // Phản hồi thành công.
        public async Task<IActionResult> GetActive() // Lấy danh sách banner hoạt động.
        {
            var result = await _bannerService.GetActiveAsync(); // Gọi service lấy danh sách banner đang hiệu lực.
            return Ok(result); // Trả về HTTP 200 OK.
        }

        /// <summary>
        /// Lấy chi tiết banner theo Id. Chỉ Admin.
        /// </summary>
        [HttpGet("{id:int}")] // Định nghĩa HTTP GET Method lấy chi tiết theo id dạng int (api/banners/{id}).
        [Authorize(Roles = "Admin")] // Chỉ Admin truy cập.
        [ProducesResponseType(typeof(ApiResult<BannerDto>), StatusCodes.Status200OK)] // Thành công.
        [ProducesResponseType(typeof(ApiResult<BannerDto>), StatusCodes.Status404NotFound)] // Không tìm thấy.
        public async Task<IActionResult> GetById(int id) // Lấy chi tiết banner theo Id.
        {
            var result = await _bannerService.GetByIdAsync(id); // Gọi service lấy chi tiết banner.
            return result.ToActionResult(this); // Ánh xạ kết quả sang HTTP Action Result.
        }

        /// <summary>
        /// Tạo mới banner. Chỉ Admin.
        /// </summary>
        [HttpPost] // Định nghĩa HTTP POST Method tạo mới banner (api/banners).
        [Authorize(Roles = "Admin")] // Chỉ Admin truy cập.
        [ProducesResponseType(typeof(ApiResult<BannerDto>), StatusCodes.Status201Created)] // Tạo thành công.
        [ProducesResponseType(typeof(ApiResult<BannerDto>), StatusCodes.Status400BadRequest)] // Yêu cầu không hợp lệ.
        public async Task<IActionResult> Create([FromBody] CreateBannerRequest request) // Nhận thông tin banner từ Request Body.
        {
            var result = await _bannerService.CreateAsync(request); // Gọi service tạo banner mới.
            if (!result.Success) return result.ToActionResult(this); // Trả về lỗi tương ứng nếu tạo thất bại.

            return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result); // Trả về HTTP 201 Created cùng đường dẫn lấy chi tiết banner vừa tạo.
        }

        /// <summary>
        /// Cập nhật banner. Chỉ Admin.
        /// </summary>
        [HttpPut("{id:int}")] // Định nghĩa HTTP PUT Method cập nhật banner theo Id (api/banners/{id}).
        [Authorize(Roles = "Admin")] // Chỉ Admin truy cập.
        [ProducesResponseType(typeof(ApiResult<BannerDto>), StatusCodes.Status200OK)] // Cập nhật thành công.
        [ProducesResponseType(typeof(ApiResult<BannerDto>), StatusCodes.Status400BadRequest)] // Lỗi đầu vào.
        [ProducesResponseType(typeof(ApiResult<BannerDto>), StatusCodes.Status404NotFound)] // Không tìm thấy.
        public async Task<IActionResult> Update(int id, [FromBody] UpdateBannerRequest request) // Cập nhật banner theo Id.
        {
            var result = await _bannerService.UpdateAsync(id, request); // Gọi service cập nhật banner.
            return result.ToActionResult(this); // Trả về kết quả qua extension ToActionResult.
        }

        /// <summary>
        /// Xóa mềm banner. Chỉ Admin.
        /// </summary>
        [HttpDelete("{id:int}")] // Định nghĩa HTTP DELETE Method xóa banner theo Id (api/banners/{id}).
        [Authorize(Roles = "Admin")] // Chỉ Admin truy cập.
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status200OK)] // Xóa thành công.
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status404NotFound)] // Không tìm thấy.
        public async Task<IActionResult> Delete(int id) // Xóa mềm banner.
        {
            var result = await _bannerService.DeleteAsync(id); // Gọi service thực hiện xóa banner.
            return result.ToActionResult(this); // Trả về kết quả qua extension ToActionResult.
        }
    }
}

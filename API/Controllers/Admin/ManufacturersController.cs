using Microsoft.AspNetCore.Authorization; // Sử dụng phân quyền truy cập.
using Microsoft.AspNetCore.Mvc; // Sử dụng các thành phần MVC cho Web API.
using PBL3.Application.Manufacturers; // Sử dụng tầng dịch vụ hãng sản xuất IManufacturerService.
using PBL3.Shared.DTOs.Common; // Sử dụng các DTO dùng chung như ApiResult, PagedResult.
using PBL3.Shared.DTOs.Manufacturers; // Sử dụng các DTO của hãng sản xuất.
using PBL3.API.Extensions; // Sử dụng extension ToActionResult.

namespace PBL3.API.Controllers.Admin // Khai báo namespace cho Controllers quản lý hãng sản xuất.
{
    [ApiController] // Khai báo lớp là một Web API Controller có sẵn cơ chế tự động validate dữ liệu.
    [Route("api/[controller]")] // Định nghĩa route truy cập: api/manufacturers.
    [Produces("application/json")] // Quy định kiểu dữ liệu trả về mặc định dạng JSON.
    public class ManufacturersController : ControllerBase // Định nghĩa lớp ManufacturersController kế thừa từ ControllerBase.
    {
        private readonly IManufacturerService _manufacturerService; // Khai báo trường lưu trữ dịch vụ hãng sản xuất.

        public ManufacturersController(IManufacturerService manufacturerService) // Constructor injection tiêm IManufacturerService.
        {
            _manufacturerService = manufacturerService; // Gán dịch vụ được tiêm.
        }

        /// <summary>
        /// Lấy danh sách hãng sản xuất (phân trang, tìm kiếm theo Tên hoặc Website).
        /// Cho phép cả 3 role truy cập (dùng để xem/filter).
        /// </summary>
        [HttpGet] // Định nghĩa HTTP GET Method lấy danh sách (api/manufacturers).
        [Authorize(Roles = "Admin, Employee")] // Yêu cầu vai trò Admin hoặc Employee được phép truy cập.
        [ProducesResponseType(typeof(ApiResult<PagedResult<ManufacturerDto>>), StatusCodes.Status200OK)] // Trả về danh sách hãng sản xuất phân trang.
        public async Task<IActionResult> GetList([FromQuery] ManufacturerFilterRequest filter) // Lấy danh sách hãng sản xuất dựa trên bộ lọc Query.
        {
            var result = await _manufacturerService.GetPagedListAsync(filter); // Gọi service lấy danh sách phân trang bất đồng bộ.
            return Ok(result); // Trả về HTTP 200 OK.
        }

        /// <summary>
        /// Lấy danh sách hãng rút gọn (Id + Name + Logo) dùng cho Dropdown.
        /// Endpoint chuyên biệt để form "Thêm sản phẩm" load danh sách hãng.
        /// </summary>
        [HttpGet("dropdown")] // Định nghĩa HTTP GET Method lấy dữ liệu dropdown (api/manufacturers/dropdown).
        [Authorize(Roles = "Admin, Employee")] // Yêu cầu vai trò quản trị/nhân viên.
        [ProducesResponseType(typeof(ApiResult<List<ManufacturerSummaryDto>>), StatusCodes.Status200OK)] // Trả về danh sách rút gọn.
        public async Task<IActionResult> GetDropdown() // Lấy danh sách hãng phục vụ Dropdown.
        {
            var result = await _manufacturerService.GetAllForDropdownAsync(); // Gọi service lấy dữ liệu gọn nhẹ.
            return Ok(result); // Trả về HTTP 200 OK.
        }

        /// <summary>
        /// Lấy chi tiết hãng sản xuất theo Id.
        /// </summary>
        [HttpGet("{id:int}")] // Định nghĩa HTTP GET Method lấy chi tiết hãng (api/manufacturers/{id}).
        [Authorize(Roles = "Admin, Employee")] // Yêu cầu quyền quản lý.
        [ProducesResponseType(typeof(ApiResult<ManufacturerDto>), StatusCodes.Status200OK)] // Thành công.
        [ProducesResponseType(typeof(ApiResult<ManufacturerDto>), StatusCodes.Status404NotFound)] // Không tìm thấy.
        public async Task<IActionResult> GetById(int id) // Lấy chi tiết hãng sản xuất theo Id.
        {
            var result = await _manufacturerService.GetByIdAsync(id); // Gọi service lấy chi tiết.
            return result.ToActionResult(this); // Trả về kết quả qua extension ToActionResult.
        }

        /// <summary>
        /// Tạo mới hãng sản xuất. Chỉ Admin được phép.
        /// </summary>
        [HttpPost] // Định nghĩa HTTP POST Method thêm hãng mới (api/manufacturers).
        [Authorize(Roles = "Admin")] // Chỉ tài khoản Admin mới được tạo hãng sản xuất mới.
        [ProducesResponseType(typeof(ApiResult<ManufacturerDto>), StatusCodes.Status201Created)] // Tạo thành công trả về 201 Created.
        [ProducesResponseType(typeof(ApiResult<ManufacturerDto>), StatusCodes.Status400BadRequest)] // Đầu vào không hợp lệ.
        public async Task<IActionResult> Create([FromBody] CreateManufacturerRequest request) // Nhận thông tin hãng từ Request Body.
        {
            var result = await _manufacturerService.CreateAsync(request); // Gọi service tạo mới hãng sản xuất.

            if (!result.Success) return result.ToActionResult(this); // Trả về kết quả lỗi tương ứng nếu thất bại.

            return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result); // Trả về HTTP 201 Created cùng đường dẫn API lấy chi tiết.
        }

        /// <summary>
        /// Cập nhật hãng sản xuất. Chỉ Admin được phép.
        /// </summary>
        [HttpPut("{id:int}")] // Định nghĩa HTTP PUT Method cập nhật hãng theo Id (api/manufacturers/{id}).
        [Authorize(Roles = "Admin")] // Chỉ tài khoản Admin mới được sửa hãng sản xuất.
        [ProducesResponseType(typeof(ApiResult<ManufacturerDto>), StatusCodes.Status200OK)] // Thành công.
        [ProducesResponseType(typeof(ApiResult<ManufacturerDto>), StatusCodes.Status400BadRequest)] // Lỗi nghiệp vụ/đầu vào.
        [ProducesResponseType(typeof(ApiResult<ManufacturerDto>), StatusCodes.Status404NotFound)] // Không tìm thấy.
        public async Task<IActionResult> Update(int id, [FromBody] UpdateManufacturerRequest request) // Cập nhật hãng theo Id.
        {
            var result = await _manufacturerService.UpdateAsync(id, request); // Gọi service thực hiện cập nhật hãng.
            return result.ToActionResult(this); // Trả về kết quả qua extension ToActionResult.
        }

        /// <summary>
        /// Xóa mềm hãng sản xuất. Chỉ Admin được phép.
        /// Sẽ thất bại nếu hãng còn sản phẩm đang hoạt động.
        /// </summary>
        [HttpDelete("{id:int}")] // Định nghĩa HTTP DELETE Method xóa hãng theo Id (api/manufacturers/{id}).
        [Authorize(Roles = "Admin")] // Chỉ Admin mới được phép xóa hãng sản xuất.
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status200OK)] // Xóa thành công.
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status400BadRequest)] // Lỗi ràng buộc dữ liệu.
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status404NotFound)] // Không tìm thấy.
        public async Task<IActionResult> Delete(int id) // Xóa mềm hãng sản xuất.
        {
            var result = await _manufacturerService.DeleteAsync(id); // Gọi service thực hiện xóa mềm hãng sản xuất.
            return result.ToActionResult(this); // Trả về kết quả qua extension ToActionResult.
        }
    }
}

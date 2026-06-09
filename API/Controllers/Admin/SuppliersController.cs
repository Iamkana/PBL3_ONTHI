using Microsoft.AspNetCore.Authorization; // Sử dụng để phân quyền truy cập (Authorize).
using Microsoft.AspNetCore.Mvc; // Sử dụng các thành phần MVC cho Web API.
using PBL3.Application.Suppliers; // Sử dụng dịch vụ nhà cung cấp ISupplierService.
using PBL3.Shared.DTOs.Common; // Sử dụng các DTO dùng chung.
using PBL3.Shared.DTOs.Products; // Sử dụng các DTO sản phẩm.
using PBL3.Shared.DTOs.Suppliers; // Sử dụng các DTO liên quan đến nhà cung cấp.
using PBL3.API.Extensions; // Sử dụng extension ToActionResult.

namespace PBL3.API.Controllers.Admin // Khai báo namespace cho Controllers quản lý nhà cung cấp.
{
    [ApiController] // Khai báo đây là một API Controller có sẵn cơ chế validate model.
    [Route("api/[controller]")] // Định nghĩa route truy cập: api/suppliers.
    [Produces("application/json")] // Định dạng dữ liệu trả về mặc định là JSON.
    [Authorize(Roles = "Admin, Employee")] // Chỉ cho phép vai trò Admin hoặc Employee truy cập mọi API.
    public class SuppliersController : ControllerBase // Định nghĩa lớp SuppliersController kế thừa từ ControllerBase.
    {
        private readonly ISupplierService _supplierService; // Khai báo trường dịch vụ nhà cung cấp.

        public SuppliersController(ISupplierService supplierService) // Constructor injection tiêm ISupplierService từ DI container.
        {
            _supplierService = supplierService; // Gán dịch vụ được tiêm.
        }

        /// <summary>
        /// Lấy danh sách nhà cung cấp (phân trang, tìm kiếm theo Tên hoặc SĐT).
        /// </summary>
        [HttpGet] // Định nghĩa HTTP GET Method lấy danh sách (api/suppliers).
        [ProducesResponseType(typeof(ApiResult<PagedResult<SupplierDto>>), StatusCodes.Status200OK)] // Thành công.
        public async Task<IActionResult> GetList([FromQuery] SupplierFilterRequest filter) // Lấy danh sách phân trang theo bộ lọc.
        {
            var result = await _supplierService.GetPagedListAsync(filter); // Gọi service lấy danh sách phân trang.
            return Ok(result); // Trả về HTTP 200 OK.
        }

        /// <summary>
        /// Lấy chi tiết nhà cung cấp theo Id.
        /// </summary>
        [HttpGet("{id:int}")] // Định nghĩa HTTP GET Method lấy chi tiết (api/suppliers/{id}).
        [ProducesResponseType(typeof(ApiResult<SupplierDto>), StatusCodes.Status200OK)] // Tìm thấy nhà cung cấp.
        [ProducesResponseType(typeof(ApiResult<SupplierDto>), StatusCodes.Status404NotFound)] // Không tìm thấy.
        public async Task<IActionResult> GetById(int id) // Lấy thông tin chi tiết theo Id.
        {
            var result = await _supplierService.GetByIdAsync(id); // Gọi service lấy chi tiết nhà cung cấp.
            return result.ToActionResult(this); // Trả về kết quả qua extension ToActionResult.
        }

        /// <summary>
        /// Tạo mới nhà cung cấp.
        /// </summary>
        [HttpPost] // Định nghĩa HTTP POST Method thêm mới (api/suppliers).
        [ProducesResponseType(typeof(ApiResult<SupplierDto>), StatusCodes.Status201Created)] // Tạo thành công.
        [ProducesResponseType(typeof(ApiResult<SupplierDto>), StatusCodes.Status400BadRequest)] // Đầu vào không hợp lệ.
        public async Task<IActionResult> Create([FromBody] CreateSupplierRequest request) // Nhận thông tin nhà cung cấp từ Request Body.
        {
            var result = await _supplierService.CreateAsync(request); // Gọi service lưu nhà cung cấp mới.

            if (!result.Success) return result.ToActionResult(this); // Nếu lưu thất bại thì trả về kết quả lỗi.

            return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result); // Trả về HTTP 201 Created cùng link API lấy thông tin chi tiết.
        }

        /// <summary>
        /// Cập nhật nhà cung cấp.
        /// </summary>
        [HttpPut("{id:int}")] // Định nghĩa HTTP PUT Method cập nhật (api/suppliers/{id}).
        [ProducesResponseType(typeof(ApiResult<SupplierDto>), StatusCodes.Status200OK)] // Cập nhật thành công.
        [ProducesResponseType(typeof(ApiResult<SupplierDto>), StatusCodes.Status400BadRequest)] // Lỗi đầu vào.
        [ProducesResponseType(typeof(ApiResult<SupplierDto>), StatusCodes.Status404NotFound)] // Không tìm thấy.
        public async Task<IActionResult> Update(int id, [FromBody] UpdateSupplierRequest request) // Cập nhật nhà cung cấp theo Id.
        {
            var result = await _supplierService.UpdateAsync(id, request); // Gọi service cập nhật thông tin nhà cung cấp.
            return result.ToActionResult(this); // Trả về kết quả qua extension ToActionResult.
        }

        /// <summary>
        /// Xóa mềm nhà cung cấp (Soft Delete).
        /// </summary>
        [HttpDelete("{id:int}")] // Định nghĩa HTTP DELETE Method xóa mềm (api/suppliers/{id}).
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status200OK)] // Xóa thành công.
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status404NotFound)] // Không tìm thấy.
        public async Task<IActionResult> Delete(int id) // Xóa mềm nhà cung cấp.
        {
            var result = await _supplierService.DeleteAsync(id); // Gọi service đánh dấu xóa mềm nhà cung cấp.
            return result.ToActionResult(this); // Trả về kết quả qua extension ToActionResult.
        }
    }
}

using Microsoft.AspNetCore.Authorization; // Sử dụng phân quyền truy cập.
using Microsoft.AspNetCore.Mvc; // Sử dụng các thành phần MVC cho Web API.
using PBL3.Application.ProductSerials; // Sử dụng dịch vụ số Serial sản phẩm IProductSerialService.
using PBL3.Shared.DTOs.Common; // Sử dụng các DTO dùng chung.
using PBL3.Shared.DTOs.Inventory; // Sử dụng các DTO liên quan đến kho hàng/serial.
using PBL3.API.Extensions; // Sử dụng extension ToActionResult.

namespace PBL3.API.Controllers.Admin // Khai báo namespace cho Controllers quản lý số Serial.
{
    [ApiController] // Khai báo lớp là một API Controller có sẵn cơ chế tự động validate dữ liệu.
    [Route("api/product-serials")] // Định nghĩa route truy cập mặc định: api/product-serials.
    [Produces("application/json")] // Thiết lập định dạng dữ liệu trả về mặc định dạng JSON.
    [Authorize(Roles = "Admin, Employee")] // Yêu cầu vai trò Admin hoặc Employee được phép truy cập.
    public class ProductSerialsController : ControllerBase // Định nghĩa lớp ProductSerialsController kế thừa từ ControllerBase.
    {
        private readonly IProductSerialService _productSerialService; // Khai báo trường dịch vụ số Serial.

        public ProductSerialsController(IProductSerialService productSerialService) // Constructor injection tiêm IProductSerialService.
        {
            _productSerialService = productSerialService; // Gán dịch vụ được tiêm.
        }

        /// <summary>
        /// Kiểm tra mã Serial đã tồn tại trong DB chưa (Real-time check khi quét mã vạch).
        /// </summary>
        [HttpGet("check-exist")] // Định nghĩa HTTP GET Method kiểm tra mã tồn tại (api/product-serials/check-exist).
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status200OK)] // Trả về kết quả thành công.
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status400BadRequest)] // Trả về lỗi 400 BadRequest.
        public async Task<IActionResult> CheckExist( // Hành động kiểm tra sự tồn tại của số Serial.
            [FromQuery] string serialNumber, // Nhận số Serial từ Query String.
            [FromQuery] int variantId) // Nhận mã phiên bản sản phẩm từ Query String.
        {
            if (string.IsNullOrWhiteSpace(serialNumber)) // Kiểm tra xem số Serial gửi lên có bị trống không.
                return BadRequest(ApiResult<bool>.Fail("Mã Serial không được để trống.")); // Trả về lỗi HTTP 400 BadRequest.

            if (variantId <= 0) // Kiểm tra xem mã variantId có hợp lệ không (phải lớn hơn 0).
                return BadRequest(ApiResult<bool>.Fail("VariantId không hợp lệ.")); // Trả về lỗi HTTP 400 BadRequest.

            var result = await _productSerialService.CheckExistAsync(serialNumber.Trim(), variantId); // Gọi service kiểm tra sự tồn tại trong DB.
            return Ok(result); // Trả về HTTP 200 OK.
        }

        /// <summary>
        /// Thống kê số lượng Serial theo trạng thái. Có thể lọc theo productId hoặc variantId.
        /// </summary>
        [HttpGet("statistics")] // Định nghĩa HTTP GET Method lấy thống kê (api/product-serials/statistics).
        [ProducesResponseType(typeof(ApiResult<ProductSerialStatisticsDto>), StatusCodes.Status200OK)] // Trả về dữ liệu thống kê.
        public async Task<IActionResult> GetStatistics( // Hành động lấy số liệu thống kê.
            [FromQuery] int? productId, // Nhận mã sản phẩm tùy chọn từ Query.
            [FromQuery] int? variantId) // Nhận mã phiên bản sản phẩm tùy chọn từ Query.
        {
            var result = await _productSerialService.GetStatisticsAsync(productId, variantId); // Gọi service lấy thống kê theo bộ lọc.
            return Ok(result); // Trả về HTTP 200 OK.
        }

        /// <summary>
        /// Danh sách phân trang Serial với bộ lọc đa điều kiện.
        /// </summary>
        [HttpGet] // Định nghĩa HTTP GET Method lấy danh sách phân trang (api/product-serials).
        [ProducesResponseType(typeof(ApiResult<PagedResult<ProductSerialListDto>>), StatusCodes.Status200OK)] // Trả về kết quả phân trang.
        public async Task<IActionResult> GetList([FromQuery] ProductSerialFilterRequest filter) // Lọc danh sách số Serial.
        {
            var result = await _productSerialService.GetPagedListAsync(filter); // Gọi service lấy danh sách phân trang.
            return Ok(result); // Trả về HTTP 200 OK.
        }

        /// <summary>
        /// Lấy chi tiết một Serial kèm thông tin variant, phiếu nhập và đơn hàng (nếu đã bán).
        /// </summary>
        [HttpGet("{id:int}")] // Định nghĩa HTTP GET Method lấy chi tiết (api/product-serials/{id}).
        [ProducesResponseType(typeof(ApiResult<ProductSerialDetailDto>), StatusCodes.Status200OK)] // Thành công.
        [ProducesResponseType(typeof(ApiResult<ProductSerialDetailDto>), StatusCodes.Status404NotFound)] // Không tìm thấy.
        public async Task<IActionResult> GetById(int id) // Lấy thông tin chi tiết số Serial theo Id.
        {
            var result = await _productSerialService.GetByIdAsync(id); // Gọi service lấy chi tiết.
            return result.ToActionResult(this); // Trả về kết quả qua extension ToActionResult.
        }

        /// <summary>
        /// Thay đổi trạng thái Serial: đánh dấu lỗi (3), hoàn trả (4), hoặc khôi phục về kho (0).
        /// </summary>
        [HttpPatch("{id:int}/status")] // Định nghĩa HTTP PATCH Method cập nhật trạng thái (api/product-serials/{id}/status).
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status200OK)] // Cập nhật thành công.
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status400BadRequest)] // Yêu cầu lỗi đầu vào.
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status404NotFound)] // Không tìm thấy.
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateSerialStatusRequest request) // Cập nhật trạng thái số Serial theo Id.
        {
            if (id <= 0) // Kiểm tra tính hợp lệ của Id.
                return BadRequest(ApiResult<bool>.Fail("Id Serial không hợp lệ.")); // Trả về lỗi HTTP 400.

            var result = await _productSerialService.UpdateStatusAsync(id, request); // Gọi service thực hiện cập nhật trạng thái.
            return result.ToActionResult(this); // Trả về kết quả qua extension ToActionResult.
        }
    }
}

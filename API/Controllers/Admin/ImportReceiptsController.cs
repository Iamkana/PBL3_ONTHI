using Microsoft.AspNetCore.Authorization; // Sử dụng để cấu hình phân quyền truy cập.
using Microsoft.AspNetCore.Mvc; // Sử dụng các thành phần MVC cho Web API.
using PBL3.Application.ImportReceipts; // Sử dụng tầng dịch vụ nhập kho IImportReceiptService.
using PBL3.Shared.DTOs.Common; // Sử dụng DTO dùng chung ApiResult.
using PBL3.Shared.DTOs.Inventory; // Sử dụng các DTO liên quan đến quản lý kho/nhập kho.
using PBL3.Shared.DTOs.Products; // Sử dụng các DTO liên quan đến sản phẩm.
using PBL3.API.Extensions; // Sử dụng extension ToActionResult để map kết quả trả về.

namespace PBL3.API.Controllers.Admin // Khai báo namespace cho Controllers quản lý nhập kho.
{
    [ApiController] // Khai báo đây là một API Controller có sẵn cơ chế tự động validate dữ liệu.
    [Route("api/import-receipts")] // Định nghĩa route truy cập: api/import-receipts.
    [Produces("application/json")] // Thiết lập định dạng dữ liệu trả về mặc định dạng JSON.
    [Authorize(Roles = "Admin, Employee")] // Chỉ cho phép vai trò Admin hoặc Employee được phép truy cập.
    public class ImportReceiptsController : ControllerBase // Định nghĩa lớp ImportReceiptsController kế thừa từ ControllerBase.
    {
        private readonly IImportReceiptService _importReceiptService; // Khai báo trường dịch vụ nhập kho.

        public ImportReceiptsController(IImportReceiptService importReceiptService) // Constructor injection tiêm IImportReceiptService.
        {
            _importReceiptService = importReceiptService; // Gán dịch vụ được tiêm.
        }

        /// <summary>
        /// Tạo phiếu nhập kho mới.
        /// </summary>
        [HttpPost] // Định nghĩa HTTP POST Method tạo phiếu nhập (api/import-receipts).
        [ProducesResponseType(typeof(ApiResult<ImportReceiptDto>), StatusCodes.Status201Created)] // Tạo thành công trả về HTTP 201.
        [ProducesResponseType(typeof(ApiResult<ImportReceiptDto>), StatusCodes.Status400BadRequest)] // Trả về lỗi 400 BadRequest.
        public async Task<IActionResult> Create([FromBody] CreateImportReceiptRequest request) // Nhận thông tin phiếu nhập từ Body.
        {
            var result = await _importReceiptService.CreateAsync(request); // Gọi service thực hiện tạo phiếu nhập kho.

            if (!result.Success) return result.ToActionResult(this); // Trả về kết quả lỗi tương ứng nếu thất bại.

            return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result); // Trả về HTTP 201 Created kèm route lấy chi tiết.
        }

        /// <summary>
        /// Lấy danh sách phiếu nhập kho (Lịch sử nhập kho) - có phân trang và tìm kiếm.
        /// </summary>
        [HttpGet] // Định nghĩa HTTP GET Method lấy danh sách (api/import-receipts).
        [ProducesResponseType(typeof(ApiResult<PagedResult<ImportReceiptDto>>), StatusCodes.Status200OK)] // Trả về danh sách phân trang.
        public async Task<IActionResult> GetList([FromQuery] ImportReceiptFilterRequest filter) // Lọc danh sách phiếu nhập kho theo Query.
        {
            var result = await _importReceiptService.GetPagedListAsync(filter); // Gọi service lấy danh sách phân trang.
            return Ok(result); // Trả về HTTP 200 OK.
        }

        /// <summary>
        /// Xem chi tiết 1 phiếu nhập kho (bao gồm danh sách Serial đã nhập).
        /// </summary>
        [HttpGet("{id:int}")] // Định nghĩa HTTP GET Method lấy chi tiết (api/import-receipts/{id}).
        [ProducesResponseType(typeof(ApiResult<ImportReceiptDto>), StatusCodes.Status200OK)] // Thành công.
        [ProducesResponseType(typeof(ApiResult<ImportReceiptDto>), StatusCodes.Status404NotFound)] // Không tìm thấy.
        public async Task<IActionResult> GetById(int id) // Lấy thông tin chi tiết phiếu nhập theo Id.
        {
            var result = await _importReceiptService.GetByIdAsync(id); // Gọi service lấy thông tin chi tiết.
            return result.ToActionResult(this); // Trả về kết quả qua extension ToActionResult.
        }
    }
}

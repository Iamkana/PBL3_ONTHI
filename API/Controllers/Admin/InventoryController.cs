using System.Threading.Tasks; // Sử dụng thư viện lập trình bất đồng bộ Task.
using Microsoft.AspNetCore.Authorization; // Sử dụng để cấu hình phân quyền truy cập.
using Microsoft.AspNetCore.Http; // Sử dụng các lớp tương tác HTTP của ASP.NET Core.
using Microsoft.AspNetCore.Mvc; // Sử dụng các thành phần MVC cho Web API.
using PBL3.Application.Inventory; // Sử dụng tầng dịch vụ xuất kho IInventoryExportService.
using PBL3.Shared.DTOs.Common; // Sử dụng DTO dùng chung ApiResult.
using PBL3.Shared.DTOs.Inventory; // Sử dụng các DTO liên quan đến xuất kho và quản lý kho.
using PBL3.API.Extensions; // Sử dụng extension ToActionResult để map kết quả trả về.

namespace PBL3.API.Controllers.Admin // Khai báo namespace cho Controllers quản lý kho hàng.
{
    [ApiController] // Khai báo đây là một API Controller có sẵn cơ chế tự động validate dữ liệu.
    [Route("api/inventory")] // Định nghĩa route truy cập: api/inventory.
    [Produces("application/json")] // Thiết lập định dạng dữ liệu trả về mặc định dạng JSON.
    [Authorize(Roles = "Admin, Employee")] // Yêu cầu vai trò Admin hoặc Employee mới được phép truy cập.
    public class InventoryController : ControllerBase // Định nghĩa lớp InventoryController kế thừa từ ControllerBase.
    {
        private readonly IInventoryExportService _inventoryExportService; // Khai báo trường dịch vụ xuất kho.

        public InventoryController(IInventoryExportService inventoryExportService) // Constructor injection tiêm IInventoryExportService.
        {
            _inventoryExportService = inventoryExportService; // Gán dịch vụ được tiêm.
        }

        /// <summary>
        /// Xuất kho cho đơn hàng Online (Quét mã Serial).
        /// Chuyển trạng thái đơn hàng sang "Đang giao hàng" (Shipping).
        /// </summary>
        [HttpPost("export-order")] // Định nghĩa HTTP POST Method cho api/inventory/export-order.
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status200OK)] // Trả về kết quả thành công.
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status400BadRequest)] // Trả về lỗi 400 BadRequest.
        public async Task<IActionResult> ExportOrder([FromBody] ExportOrderRequest request) // Thực hiện xuất kho dựa trên danh sách serial quét cho đơn hàng.
        {
            var result = await _inventoryExportService.ExportOrderAsync(request); // Gọi service thực hiện xuất kho và chuyển trạng thái đơn hàng.
            return result.ToActionResult(this); // Ánh xạ kết quả sang IActionResult tương ứng.
        }

        [HttpGet("serials/validate")] // Định nghĩa HTTP GET Method xác thực số Serial khi xuất kho (api/inventory/serials/validate).
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status200OK)] // Thành công.
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status400BadRequest)] // Tham số không hợp lệ.
        public async Task<IActionResult> ValidateSerial([FromQuery] string serialNo, [FromQuery] int variantId) // Xác thực xem số serial có đang ở trạng thái khả dụng trong kho đối với phiên bản sản phẩm này không.
        {
            if (string.IsNullOrWhiteSpace(serialNo) || variantId <= 0) // Kiểm tra tham số đầu vào có hợp lệ không.
                return BadRequest(ApiResult<bool>.Fail("Tham số không hợp lệ.")); // Trả về lỗi HTTP 400 BadRequest.

            var result = await _inventoryExportService.ValidateSerialAsync(serialNo, variantId); // Gọi service kiểm tra tính khả dụng của serial.
            return result.ToActionResult(this); // Trả về kết quả qua extension ToActionResult.
        }
    }
}

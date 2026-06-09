using Microsoft.AspNetCore.Authorization; // Sử dụng để phân quyền truy cập.
using Microsoft.AspNetCore.Mvc; // Sử dụng các thành phần MVC cho Web API.
using PBL3.Application.Analytics; // Sử dụng tầng dịch vụ phân tích dữ liệu IAnalyticsService.
using PBL3.Shared.DTOs.Analytics; // Sử dụng các DTO liên quan đến thống kê/phân tích.
using PBL3.API.Extensions; // Sử dụng phương thức mở rộng ToActionResult để map kết quả trả về.

namespace PBL3.API.Controllers.Admin // Khai báo namespace cho Controllers thuộc khu vực quản trị.
{
    [ApiController] // Khai báo đây là một API Controller có sẵn cơ chế tự động validate Model.
    [Route("api/[controller]")] // Định nghĩa route truy cập: api/analytics.
    [Produces("application/json")] // Thiết lập định dạng dữ liệu trả về mặc định là JSON.
    [Authorize(Roles = "Admin")] // Yêu cầu chỉ tài khoản có vai trò Admin được truy cập vào các API thống kê này.
    public class AnalyticsController : ControllerBase // Định nghĩa lớp AnalyticsController kế thừa từ ControllerBase.
    {
        private readonly IAnalyticsService _service; // Khai báo trường dịch vụ phân tích.

        public AnalyticsController(IAnalyticsService service) // Constructor injection tiêm IAnalyticsService từ DI container.
        {
            _service = service; // Gán dịch vụ được tiêm vào trường nội bộ.
        }

        [HttpGet("summary")] // Định nghĩa HTTP GET Method lấy tổng hợp doanh thu/lợi nhuận (api/analytics/summary).
        public async Task<IActionResult> GetSummary([FromQuery] AnalyticsFilterRequest filter) // Nhận thời gian lọc From và To từ Query.
        {
            var result = await _service.GetSummaryAsync(filter.From, filter.To); // Gọi service tính toán tổng doanh thu, lợi nhuận, số đơn đặt hàng.
            return result.ToActionResult(this); // Trả về kết quả qua extension ToActionResult.
        }

        [HttpGet("revenue-trend")] // Định nghĩa HTTP GET Method lấy xu hướng doanh thu theo ngày (api/analytics/revenue-trend).
        public async Task<IActionResult> GetRevenueTrend([FromQuery] AnalyticsFilterRequest filter) // Nhận thời gian lọc.
        {
            var result = await _service.GetRevenueTrendAsync(filter.From, filter.To); // Gọi service lấy danh sách thống kê doanh thu theo từng ngày.
            return result.ToActionResult(this); // Trả về kết quả qua extension ToActionResult.
        }

        [HttpGet("order-channels")] // Định nghĩa HTTP GET Method lấy tỉ lệ đơn hàng theo kênh (api/analytics/order-channels).
        public async Task<IActionResult> GetOrderChannels([FromQuery] AnalyticsFilterRequest filter) // Nhận thời gian lọc.
        {
            var result = await _service.GetOrderChannelsAsync(filter.From, filter.To); // Gọi service phân tích tỉ lệ đơn hàng từ Online vs POS.
            return result.ToActionResult(this); // Trả về kết quả qua extension ToActionResult.
        }

        [HttpGet("top-products")] // Định nghĩa HTTP GET Method lấy top sản phẩm bán chạy (api/analytics/top-products).
        public async Task<IActionResult> GetTopProducts([FromQuery] AnalyticsFilterRequest filter, [FromQuery] int top = 10) // Nhận thời gian lọc và số lượng top (mặc định 10).
        {
            var result = await _service.GetTopProductsAsync(filter.From, filter.To, top); // Gọi service lấy danh sách sản phẩm bán chạy nhất kèm số lượng bán.
            return result.ToActionResult(this); // Trả về kết quả qua extension ToActionResult.
        }

        [HttpGet("category-revenue")] // Định nghĩa HTTP GET Method lấy doanh thu theo danh mục (api/analytics/category-revenue).
        public async Task<IActionResult> GetCategoryRevenue([FromQuery] AnalyticsFilterRequest filter, [FromQuery] int top = 5) // Nhận thời gian lọc và số lượng top danh mục (mặc định 5).
        {
            var result = await _service.GetCategoryRevenueAsync(filter.From, filter.To, top); // Gọi service thống kê doanh thu phân bổ theo các danh mục sản phẩm.
            return result.ToActionResult(this); // Trả về kết quả qua extension ToActionResult.
        }

        [HttpGet("inventory-summary")] // Định nghĩa HTTP GET Method lấy tổng hợp báo cáo kho hàng hiện tại (api/analytics/inventory-summary).
        public async Task<IActionResult> GetInventorySummary() // Thống kê kho hàng không cần lọc thời gian.
        {
            var result = await _service.GetInventorySummaryAsync(); // Gọi service lấy tổng số lượng sản phẩm trong kho, số sản phẩm sắp hết hàng...
            return result.ToActionResult(this); // Trả về kết quả qua extension ToActionResult.
        }
    }
}

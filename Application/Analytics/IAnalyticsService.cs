using PBL3.Shared.DTOs.Analytics; // Sử dụng các DTO của module thống kê.
using PBL3.Shared.DTOs.Common; // Sử dụng DTO dùng chung ApiResult.

namespace PBL3.Application.Analytics // Khai báo namespace cho tầng Application của module thống kê.
{
    public interface IAnalyticsService // Khai báo interface IAnalyticsService cung cấp các nghiệp vụ thống kê.
    {
        Task<ApiResult<AnalyticsSummaryDto>> GetSummaryAsync(DateTime from, DateTime to); // Khai báo phương thức lấy tổng quan báo cáo tài chính (Doanh thu, lợi nhuận gộp, đơn hàng).
        Task<ApiResult<RevenueTrendDto>> GetRevenueTrendAsync(DateTime from, DateTime to); // Khai báo phương thức lấy xu hướng biến động doanh số hàng ngày.
        Task<ApiResult<OrderChannelDto>> GetOrderChannelsAsync(DateTime from, DateTime to); // Khai báo phương thức lấy thống kê sản lượng đơn hàng theo kênh Online vs POS.
        Task<ApiResult<List<TopProductDto>>> GetTopProductsAsync(DateTime from, DateTime to, int top); // Khai báo phương thức lấy danh sách các sản phẩm bán chạy nhất.
        Task<ApiResult<List<CategoryRevenueDto>>> GetCategoryRevenueAsync(DateTime from, DateTime to, int top); // Khai báo phương thức lấy cơ cấu doanh thu theo danh mục sản phẩm.
        Task<ApiResult<InventorySummaryDto>> GetInventorySummaryAsync(); // Khai báo phương thức lấy báo cáo tổng hợp trạng thái kho hàng và số serial.
    }
}

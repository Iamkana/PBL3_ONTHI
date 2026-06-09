using PBL3.Shared.DTOs.Analytics; // Sử dụng các DTO của module Analytics thuộc tầng Shared.
using PBL3.Shared.DTOs.Common; // Sử dụng các DTO của module Common thuộc tầng Shared.

namespace Client.Services.Analytics; // Thiết lập namespace Client.Services.Analytics để tổ chức quản lý cấu trúc các lớp.

public interface IAnalyticsClientService // Định nghĩa giao diện (interface) IAnalyticsClientService quy định các hàm tương tác của client.
{
    Task<ApiResult<AnalyticsSummaryDto>> GetSummaryAsync(DateTime from, DateTime to); // Khai báo phương thức giao diện 'GetSummaryAsync' với tham số (from, to) có kết quả trả về kiểu Task<ApiResult<AnalyticsSummaryDto>>.
    Task<ApiResult<RevenueTrendDto>> GetRevenueTrendAsync(DateTime from, DateTime to); // Khai báo phương thức giao diện 'GetRevenueTrendAsync' với tham số (from, to) có kết quả trả về kiểu Task<ApiResult<RevenueTrendDto>>.
    Task<ApiResult<OrderChannelDto>> GetOrderChannelsAsync(DateTime from, DateTime to); // Khai báo phương thức giao diện 'GetOrderChannelsAsync' với tham số (from, to) có kết quả trả về kiểu Task<ApiResult<OrderChannelDto>>.
    Task<ApiResult<List<TopProductDto>>> GetTopProductsAsync(DateTime from, DateTime to, int top = 10); // Khai báo phương thức giao diện 'GetTopProductsAsync' với tham số (from, to, 10) có kết quả trả về kiểu Task<ApiResult<List<TopProductDto>>>.
    Task<ApiResult<List<CategoryRevenueDto>>> GetCategoryRevenueAsync(DateTime from, DateTime to, int top = 5); // Khai báo phương thức giao diện 'GetCategoryRevenueAsync' với tham số (from, to, 5) có kết quả trả về kiểu Task<ApiResult<List<CategoryRevenueDto>>>.
    Task<ApiResult<InventorySummaryDto>> GetInventorySummaryAsync(); // Khai báo phương thức giao diện 'GetInventorySummaryAsync' không tham số có kết quả trả về kiểu Task<ApiResult<InventorySummaryDto>>.
}

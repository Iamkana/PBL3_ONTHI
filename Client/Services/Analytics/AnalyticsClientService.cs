using System.Net.Http.Json; // Sử dụng thư viện hỗ trợ gửi nhận dữ liệu JSON qua HTTP.
using PBL3.Shared.DTOs.Analytics; // Sử dụng các DTO của module Analytics thuộc tầng Shared.
using PBL3.Shared.DTOs.Common; // Sử dụng các DTO của module Common thuộc tầng Shared.

namespace Client.Services.Analytics // Thiết lập namespace Client.Services.Analytics để tổ chức quản lý cấu trúc các lớp.
{
    public class AnalyticsClientService : IAnalyticsClientService // Định nghĩa lớp AnalyticsClientService triển khai các dịch vụ hoặc kế thừa từ IAnalyticsClientService.
    {
        private readonly HttpClient _httpClient; // Khai báo đối tượng HttpClient nội bộ dùng để thực hiện cuộc gọi API.
        private const string BaseUrl = "api/analytics"; // Khai báo hằng số BaseUrl có giá trị là "api/analytics".

        public AnalyticsClientService(HttpClient httpClient) // Hàm khởi tạo (Constructor) của lớp AnalyticsClientService tiêm các phụ thuộc: httpClient.
        {
            _httpClient = httpClient; // Thực hiện gán giá trị của biểu thức 'httpClient' cho biến/thuộc tính '_httpClient'.
        }

        private string DateRange(DateTime from, DateTime to) => // Thực thi dòng lệnh nghiệp vụ.
            $"from={from:yyyy-MM-dd}&to={to:yyyy-MM-dd}"; // Định dạng chuỗi truy vấn chứa tham số ngày bắt đầu và ngày kết thúc: $"from={from:yyyy-MM-dd}&to={to:yyyy-MM-dd}".

        public async Task<ApiResult<AnalyticsSummaryDto>> GetSummaryAsync(DateTime from, DateTime to) // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'GetSummaryAsync' nhận tham số (from, to) trả về kết quả kiểu Task<ApiResult<AnalyticsSummaryDto>>.
        {
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var result = await _httpClient // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'result'.
                    .GetFromJsonAsync<ApiResult<AnalyticsSummaryDto>>($"{BaseUrl}/summary?{DateRange(from, to)}"); // Gọi phương thức GET bất đồng bộ tới URL '{BaseUrl}/summary?{DateRange(from, to)}' nhận kết quả kiểu ApiResult<AnalyticsSummaryDto>.
                return result ?? ApiResult<AnalyticsSummaryDto>.Fail("Không nhận được phản hồi từ máy chủ."); // Trả về giá trị của 'result' nếu khác null, ngược lại trả về 'ApiResult<AnalyticsSummaryDto>.Fail("Không nhận được phản hồi từ máy chủ.")'.
            }
            catch (Exception ex) // Khối bắt lỗi ngoại lệ Exception nếu xảy ra lỗi trong quá trình xử lý.
            {
                return ApiResult<AnalyticsSummaryDto>.Fail($"Lỗi kết nối: {ex.Message}"); // Trả về giá trị của biểu thức 'ApiResult<AnalyticsSummaryDto>.Fail($"Lỗi kết nối: {ex.Message}")'.
            }
        }

        public async Task<ApiResult<RevenueTrendDto>> GetRevenueTrendAsync(DateTime from, DateTime to) // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'GetRevenueTrendAsync' nhận tham số (from, to) trả về kết quả kiểu Task<ApiResult<RevenueTrendDto>>.
        {
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var result = await _httpClient // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'result'.
                    .GetFromJsonAsync<ApiResult<RevenueTrendDto>>($"{BaseUrl}/revenue-trend?{DateRange(from, to)}"); // Gọi phương thức GET bất đồng bộ tới URL '{BaseUrl}/revenue-trend?{DateRange(from, to)}' nhận kết quả kiểu ApiResult<RevenueTrendDto>.
                return result ?? ApiResult<RevenueTrendDto>.Fail("Không nhận được phản hồi từ máy chủ."); // Trả về giá trị của 'result' nếu khác null, ngược lại trả về 'ApiResult<RevenueTrendDto>.Fail("Không nhận được phản hồi từ máy chủ.")'.
            }
            catch (Exception ex) // Khối bắt lỗi ngoại lệ Exception nếu xảy ra lỗi trong quá trình xử lý.
            {
                return ApiResult<RevenueTrendDto>.Fail($"Lỗi kết nối: {ex.Message}"); // Trả về giá trị của biểu thức 'ApiResult<RevenueTrendDto>.Fail($"Lỗi kết nối: {ex.Message}")'.
            }
        }

        public async Task<ApiResult<OrderChannelDto>> GetOrderChannelsAsync(DateTime from, DateTime to) // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'GetOrderChannelsAsync' nhận tham số (from, to) trả về kết quả kiểu Task<ApiResult<OrderChannelDto>>.
        {
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var result = await _httpClient // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'result'.
                    .GetFromJsonAsync<ApiResult<OrderChannelDto>>($"{BaseUrl}/order-channels?{DateRange(from, to)}"); // Gọi phương thức GET bất đồng bộ tới URL '{BaseUrl}/order-channels?{DateRange(from, to)}' nhận kết quả kiểu ApiResult<OrderChannelDto>.
                return result ?? ApiResult<OrderChannelDto>.Fail("Không nhận được phản hồi từ máy chủ."); // Trả về giá trị của 'result' nếu khác null, ngược lại trả về 'ApiResult<OrderChannelDto>.Fail("Không nhận được phản hồi từ máy chủ.")'.
            }
            catch (Exception ex) // Khối bắt lỗi ngoại lệ Exception nếu xảy ra lỗi trong quá trình xử lý.
            {
                return ApiResult<OrderChannelDto>.Fail($"Lỗi kết nối: {ex.Message}"); // Trả về giá trị của biểu thức 'ApiResult<OrderChannelDto>.Fail($"Lỗi kết nối: {ex.Message}")'.
            }
        }

        public async Task<ApiResult<List<TopProductDto>>> GetTopProductsAsync(DateTime from, DateTime to, int top = 10) // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'GetTopProductsAsync' nhận tham số (from, to, 10) trả về kết quả kiểu Task<ApiResult<List<TopProductDto>>>.
        {
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var result = await _httpClient // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'result'.
                    .GetFromJsonAsync<ApiResult<List<TopProductDto>>>($"{BaseUrl}/top-products?{DateRange(from, to)}&top={top}"); // Gọi phương thức GET bất đồng bộ tới URL '{BaseUrl}/top-products?{DateRange(from, to)}&top={top}' nhận kết quả kiểu ApiResult<List<TopProductDto>>.
                return result ?? ApiResult<List<TopProductDto>>.Fail("Không nhận được phản hồi từ máy chủ."); // Trả về giá trị của 'result' nếu khác null, ngược lại trả về 'ApiResult<List<TopProductDto>>.Fail("Không nhận được phản hồi từ máy chủ.")'.
            }
            catch (Exception ex) // Khối bắt lỗi ngoại lệ Exception nếu xảy ra lỗi trong quá trình xử lý.
            {
                return ApiResult<List<TopProductDto>>.Fail($"Lỗi kết nối: {ex.Message}"); // Trả về giá trị của biểu thức 'ApiResult<List<TopProductDto>>.Fail($"Lỗi kết nối: {ex.Message}")'.
            }
        }

        public async Task<ApiResult<List<CategoryRevenueDto>>> GetCategoryRevenueAsync(DateTime from, DateTime to, int top = 5) // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'GetCategoryRevenueAsync' nhận tham số (from, to, 5) trả về kết quả kiểu Task<ApiResult<List<CategoryRevenueDto>>>.
        {
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var result = await _httpClient // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'result'.
                    .GetFromJsonAsync<ApiResult<List<CategoryRevenueDto>>>($"{BaseUrl}/category-revenue?{DateRange(from, to)}&top={top}"); // Gọi phương thức GET bất đồng bộ tới URL '{BaseUrl}/category-revenue?{DateRange(from, to)}&top={top}' nhận kết quả kiểu ApiResult<List<CategoryRevenueDto>>.
                return result ?? ApiResult<List<CategoryRevenueDto>>.Fail("Không nhận được phản hồi từ máy chủ."); // Trả về giá trị của 'result' nếu khác null, ngược lại trả về 'ApiResult<List<CategoryRevenueDto>>.Fail("Không nhận được phản hồi từ máy chủ.")'.
            }
            catch (Exception ex) // Khối bắt lỗi ngoại lệ Exception nếu xảy ra lỗi trong quá trình xử lý.
            {
                return ApiResult<List<CategoryRevenueDto>>.Fail($"Lỗi kết nối: {ex.Message}"); // Trả về giá trị của biểu thức 'ApiResult<List<CategoryRevenueDto>>.Fail($"Lỗi kết nối: {ex.Message}")'.
            }
        }

        public async Task<ApiResult<InventorySummaryDto>> GetInventorySummaryAsync() // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'GetInventorySummaryAsync' không tham số trả về kết quả kiểu Task<ApiResult<InventorySummaryDto>>.
        {
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var result = await _httpClient // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'result'.
                    .GetFromJsonAsync<ApiResult<InventorySummaryDto>>($"{BaseUrl}/inventory-summary"); // Gọi phương thức GET bất đồng bộ tới URL '{BaseUrl}/inventory-summary' nhận kết quả kiểu ApiResult<InventorySummaryDto>.
                return result ?? ApiResult<InventorySummaryDto>.Fail("Không nhận được phản hồi từ máy chủ."); // Trả về giá trị của 'result' nếu khác null, ngược lại trả về 'ApiResult<InventorySummaryDto>.Fail("Không nhận được phản hồi từ máy chủ.")'.
            }
            catch (Exception ex) // Khối bắt lỗi ngoại lệ Exception nếu xảy ra lỗi trong quá trình xử lý.
            {
                return ApiResult<InventorySummaryDto>.Fail($"Lỗi kết nối: {ex.Message}"); // Trả về giá trị của biểu thức 'ApiResult<InventorySummaryDto>.Fail($"Lỗi kết nối: {ex.Message}")'.
            }
        }
    }
}

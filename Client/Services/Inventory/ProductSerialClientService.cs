using System.Net.Http.Json; // Sử dụng thư viện hỗ trợ gửi nhận dữ liệu JSON qua HTTP.
using PBL3.Shared.DTOs.Common; // Sử dụng các DTO của module Common thuộc tầng Shared.
using PBL3.Shared.DTOs.Inventory; // Sử dụng các DTO của module Inventory thuộc tầng Shared.

namespace Client.Services.Inventory // Thiết lập namespace Client.Services.Inventory để tổ chức quản lý cấu trúc các lớp.
{
    public class ProductSerialClientService : IProductSerialClientService // Định nghĩa lớp ProductSerialClientService triển khai các dịch vụ hoặc kế thừa từ IProductSerialClientService.
    {
        private readonly HttpClient _httpClient; // Khai báo đối tượng HttpClient nội bộ dùng để thực hiện cuộc gọi API.
        private const string BaseUrl = "api/product-serials"; // Khai báo hằng số BaseUrl có giá trị là "api/product-serials".

        public ProductSerialClientService(HttpClient httpClient) // Hàm khởi tạo (Constructor) của lớp ProductSerialClientService tiêm các phụ thuộc: httpClient.
        {
            _httpClient = httpClient; // Thực hiện gán giá trị của biểu thức 'httpClient' cho biến/thuộc tính '_httpClient'.
        }

        public async Task<ApiResult<bool>> CheckExistAsync(string serialNumber, int variantId) // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'CheckExistAsync' nhận tham số (serialNumber, variantId) trả về kết quả kiểu Task<ApiResult<bool>>.
        {
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var url = $"{BaseUrl}/check-exist?serialNumber={Uri.EscapeDataString(serialNumber)}&variantId={variantId}"; // Thực hiện gán giá trị của biểu thức '$"{BaseUrl}/check-exist?serialNumber={Uri.EscapeDataString(serialNumber)}&variantId={variantId}"' cho biến/thuộc tính 'url'.
                var response = await _httpClient.GetAsync(url); // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'response'.
                if (!response.IsSuccessStatusCode) // Kiểm tra xem điều kiện '!response.IsSuccessStatusCode' có thỏa mãn hay không.
                    return ApiResult<bool>.Fail($"Lỗi HTTP {(int)response.StatusCode}."); // Trả về giá trị của biểu thức 'ApiResult<bool>.Fail($"Lỗi HTTP {(int)response.StatusCode}.")'.
                var result = await response.Content.ReadFromJsonAsync<ApiResult<bool>>(); // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'result'.
                return result ?? ApiResult<bool>.Fail("Không thể kiểm tra mã Serial."); // Trả về giá trị của 'result' nếu khác null, ngược lại trả về 'ApiResult<bool>.Fail("Không thể kiểm tra mã Serial.")'.
            }
            catch (Exception ex) // Khối bắt lỗi ngoại lệ Exception nếu xảy ra lỗi trong quá trình xử lý.
            {
                return ApiResult<bool>.Fail($"Lỗi kết nối: {ex.Message}"); // Trả về giá trị của biểu thức 'ApiResult<bool>.Fail($"Lỗi kết nối: {ex.Message}")'.
            }
        }

        public async Task<ApiResult<PagedResult<ProductSerialListDto>>> GetPagedListAsync(ProductSerialFilterRequest filter) // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'GetPagedListAsync' nhận tham số (filter) trả về kết quả kiểu Task<ApiResult<PagedResult<ProductSerialListDto>>>.
        {
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var queryParams = new List<string>(); // Thực hiện gán giá trị của biểu thức 'new List<string>()' cho biến/thuộc tính 'queryParams'.
                if (!string.IsNullOrWhiteSpace(filter.Keyword)) // Kiểm tra xem điều kiện '!string.IsNullOrWhiteSpace(filter.Keyword' có thỏa mãn hay không.
                    queryParams.Add($"keyword={Uri.EscapeDataString(filter.Keyword)}"); // Thực thi dòng lệnh nghiệp vụ.
                if (filter.ProductId.HasValue) // Kiểm tra xem điều kiện 'filter.ProductId.HasValue' có thỏa mãn hay không.
                    queryParams.Add($"productId={filter.ProductId}"); // Thực thi dòng lệnh nghiệp vụ.
                if (filter.VariantId.HasValue) // Kiểm tra xem điều kiện 'filter.VariantId.HasValue' có thỏa mãn hay không.
                    queryParams.Add($"variantId={filter.VariantId}"); // Thực thi dòng lệnh nghiệp vụ.
                if (filter.Status.HasValue) // Kiểm tra xem điều kiện 'filter.Status.HasValue' có thỏa mãn hay không.
                    queryParams.Add($"status={filter.Status}"); // Thực thi dòng lệnh nghiệp vụ.
                if (filter.FromDate.HasValue) // Kiểm tra xem điều kiện 'filter.FromDate.HasValue' có thỏa mãn hay không.
                    queryParams.Add($"fromDate={filter.FromDate:yyyy-MM-dd}"); // Thực thi dòng lệnh nghiệp vụ.
                if (filter.ToDate.HasValue) // Kiểm tra xem điều kiện 'filter.ToDate.HasValue' có thỏa mãn hay không.
                    queryParams.Add($"toDate={filter.ToDate:yyyy-MM-dd}"); // Thực thi dòng lệnh nghiệp vụ.
                queryParams.Add($"pageNumber={filter.PageNumber}"); // Thực thi dòng lệnh nghiệp vụ.
                queryParams.Add($"pageSize={filter.PageSize}"); // Thực thi dòng lệnh nghiệp vụ.
                if (!string.IsNullOrWhiteSpace(filter.SortBy)) // Kiểm tra xem điều kiện '!string.IsNullOrWhiteSpace(filter.SortBy' có thỏa mãn hay không.
                    queryParams.Add($"sortBy={Uri.EscapeDataString(filter.SortBy)}"); // Thực thi dòng lệnh nghiệp vụ.
                queryParams.Add($"sortDescending={filter.SortDescending}"); // Thực thi dòng lệnh nghiệp vụ.

                var url = $"{BaseUrl}?{string.Join("&", queryParams)}"; // Thực hiện gán giá trị của biểu thức '$"{BaseUrl}?{string.Join("&", queryParams)}"' cho biến/thuộc tính 'url'.
                var response = await _httpClient.GetAsync(url); // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'response'.
                if (!response.IsSuccessStatusCode) // Kiểm tra xem điều kiện '!response.IsSuccessStatusCode' có thỏa mãn hay không.
                    return ApiResult<PagedResult<ProductSerialListDto>>.Fail($"Lỗi HTTP {(int)response.StatusCode}."); // Trả về giá trị của biểu thức 'ApiResult<PagedResult<ProductSerialListDto>>.Fail($"Lỗi HTTP {(int)response.StatusCode}.")'.
                var result = await response.Content.ReadFromJsonAsync<ApiResult<PagedResult<ProductSerialListDto>>>(); // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'result'.
                return result ?? ApiResult<PagedResult<ProductSerialListDto>>.Fail("Không thể lấy danh sách Serial."); // Trả về giá trị của 'result' nếu khác null, ngược lại trả về 'ApiResult<PagedResult<ProductSerialListDto>>.Fail("Không thể lấy danh sách Serial.")'.
            }
            catch (Exception ex) // Khối bắt lỗi ngoại lệ Exception nếu xảy ra lỗi trong quá trình xử lý.
            {
                return ApiResult<PagedResult<ProductSerialListDto>>.Fail($"Lỗi kết nối: {ex.Message}"); // Trả về giá trị của biểu thức 'ApiResult<PagedResult<ProductSerialListDto>>.Fail($"Lỗi kết nối: {ex.Message}")'.
            }
        }

        public async Task<ApiResult<ProductSerialStatisticsDto>> GetStatisticsAsync(int? productId = null, int? variantId = null) // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'GetStatisticsAsync' nhận tham số (null, null) trả về kết quả kiểu Task<ApiResult<ProductSerialStatisticsDto>>.
        {
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var queryParams = new List<string>(); // Thực hiện gán giá trị của biểu thức 'new List<string>()' cho biến/thuộc tính 'queryParams'.
                if (productId.HasValue) // Kiểm tra xem điều kiện 'productId.HasValue' có thỏa mãn hay không.
                    queryParams.Add($"productId={productId}"); // Thực thi dòng lệnh nghiệp vụ.
                if (variantId.HasValue) // Kiểm tra xem điều kiện 'variantId.HasValue' có thỏa mãn hay không.
                    queryParams.Add($"variantId={variantId}"); // Thực thi dòng lệnh nghiệp vụ.

                var url = queryParams.Count > 0 // Thực hiện gán giá trị của biểu thức 'queryParams.Count > 0' cho biến/thuộc tính 'url'.
                    ? $"{BaseUrl}/statistics?{string.Join("&", queryParams)}" // Thực thi dòng lệnh nghiệp vụ.
                    : $"{BaseUrl}/statistics"; // Thực thi dòng lệnh nghiệp vụ.

                var response = await _httpClient.GetAsync(url); // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'response'.
                if (!response.IsSuccessStatusCode) // Kiểm tra xem điều kiện '!response.IsSuccessStatusCode' có thỏa mãn hay không.
                    return ApiResult<ProductSerialStatisticsDto>.Fail($"Lỗi HTTP {(int)response.StatusCode}."); // Trả về giá trị của biểu thức 'ApiResult<ProductSerialStatisticsDto>.Fail($"Lỗi HTTP {(int)response.StatusCode}.")'.
                var result = await response.Content.ReadFromJsonAsync<ApiResult<ProductSerialStatisticsDto>>(); // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'result'.
                return result ?? ApiResult<ProductSerialStatisticsDto>.Fail("Không thể lấy thống kê Serial."); // Trả về giá trị của 'result' nếu khác null, ngược lại trả về 'ApiResult<ProductSerialStatisticsDto>.Fail("Không thể lấy thống kê Serial.")'.
            }
            catch (Exception ex) // Khối bắt lỗi ngoại lệ Exception nếu xảy ra lỗi trong quá trình xử lý.
            {
                return ApiResult<ProductSerialStatisticsDto>.Fail($"Lỗi kết nối: {ex.Message}"); // Trả về giá trị của biểu thức 'ApiResult<ProductSerialStatisticsDto>.Fail($"Lỗi kết nối: {ex.Message}")'.
            }
        }

        public async Task<ApiResult<ProductSerialDetailDto>> GetByIdAsync(int id) // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'GetByIdAsync' nhận tham số (id) trả về kết quả kiểu Task<ApiResult<ProductSerialDetailDto>>.
        {
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var url = $"{BaseUrl}/{id}"; // Thực hiện gán giá trị của biểu thức '$"{BaseUrl}/{id}"' cho biến/thuộc tính 'url'.
                var response = await _httpClient.GetAsync(url); // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'response'.
                if (!response.IsSuccessStatusCode) // Kiểm tra xem điều kiện '!response.IsSuccessStatusCode' có thỏa mãn hay không.
                    return ApiResult<ProductSerialDetailDto>.Fail($"Lỗi HTTP {(int)response.StatusCode}."); // Trả về giá trị của biểu thức 'ApiResult<ProductSerialDetailDto>.Fail($"Lỗi HTTP {(int)response.StatusCode}.")'.
                var result = await response.Content.ReadFromJsonAsync<ApiResult<ProductSerialDetailDto>>(); // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'result'.
                return result ?? ApiResult<ProductSerialDetailDto>.Fail("Không thể lấy chi tiết Serial."); // Trả về giá trị của 'result' nếu khác null, ngược lại trả về 'ApiResult<ProductSerialDetailDto>.Fail("Không thể lấy chi tiết Serial.")'.
            }
            catch (Exception ex) // Khối bắt lỗi ngoại lệ Exception nếu xảy ra lỗi trong quá trình xử lý.
            {
                return ApiResult<ProductSerialDetailDto>.Fail($"Lỗi kết nối: {ex.Message}"); // Trả về giá trị của biểu thức 'ApiResult<ProductSerialDetailDto>.Fail($"Lỗi kết nối: {ex.Message}")'.
            }
        }

        public async Task<ApiResult<bool>> UpdateStatusAsync(int id, UpdateSerialStatusRequest request) // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'UpdateStatusAsync' nhận tham số (id, request) trả về kết quả kiểu Task<ApiResult<bool>>.
        {
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var url = $"{BaseUrl}/{id}/status"; // Thực hiện gán giá trị của biểu thức '$"{BaseUrl}/{id}/status"' cho biến/thuộc tính 'url'.
                var response = await _httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Patch, url) // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'response'.
                {
                    Content = JsonContent.Create(request) // Thực hiện gán giá trị của biểu thức 'JsonContent.Create(request)' cho biến/thuộc tính 'Content'.
                }); // Thực thi dòng lệnh nghiệp vụ.
                if (!response.IsSuccessStatusCode) // Kiểm tra xem điều kiện '!response.IsSuccessStatusCode' có thỏa mãn hay không.
                    return ApiResult<bool>.Fail($"Lỗi HTTP {(int)response.StatusCode}."); // Trả về giá trị của biểu thức 'ApiResult<bool>.Fail($"Lỗi HTTP {(int)response.StatusCode}.")'.
                var result = await response.Content.ReadFromJsonAsync<ApiResult<bool>>(); // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'result'.
                return result ?? ApiResult<bool>.Fail("Không thể cập nhật trạng thái Serial."); // Trả về giá trị của 'result' nếu khác null, ngược lại trả về 'ApiResult<bool>.Fail("Không thể cập nhật trạng thái Serial.")'.
            }
            catch (Exception ex) // Khối bắt lỗi ngoại lệ Exception nếu xảy ra lỗi trong quá trình xử lý.
            {
                return ApiResult<bool>.Fail($"Lỗi kết nối: {ex.Message}"); // Trả về giá trị của biểu thức 'ApiResult<bool>.Fail($"Lỗi kết nối: {ex.Message}")'.
            }
        }
    }
}

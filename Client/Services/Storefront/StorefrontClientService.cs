using PBL3.Shared.DTOs.Common; // Sử dụng các DTO của module Common thuộc tầng Shared.
using PBL3.Shared.DTOs.Storefront; // Sử dụng các DTO của module Storefront thuộc tầng Shared.
using System.Net.Http.Json; // Sử dụng thư viện hỗ trợ gửi nhận dữ liệu JSON qua HTTP.

namespace Client.Services.Storefront // Thiết lập namespace Client.Services.Storefront để tổ chức quản lý cấu trúc các lớp.
{
    public class StorefrontClientService(HttpClient httpClient) : IStorefrontClientService // Định nghĩa lớp nghiệp vụ StorefrontClientService.
    {
        private readonly HttpClient _httpClient = httpClient; // Khai báo đối tượng HttpClient nội bộ dùng để thực hiện cuộc gọi API.

        public async Task<ApiResult<List<CategoryMenuResponse>>> GetActiveCategoriesAsync() // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'GetActiveCategoriesAsync' không tham số trả về kết quả kiểu Task<ApiResult<List<CategoryMenuResponse>>>.
        {
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var response = await _httpClient.GetAsync("/api/storefront/categories"); // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'response'.
                if (!response.IsSuccessStatusCode) // Kiểm tra xem điều kiện '!response.IsSuccessStatusCode' có thỏa mãn hay không.
                    return ApiResult<List<CategoryMenuResponse>>.Fail($"Lỗi HTTP {(int)response.StatusCode}."); // Trả về giá trị của biểu thức 'ApiResult<List<CategoryMenuResponse>>.Fail($"Lỗi HTTP {(int)response.StatusCode}.")'.
                var result = await response.Content.ReadFromJsonAsync<ApiResult<List<CategoryMenuResponse>>>(); // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'result'.
                return result ?? ApiResult<List<CategoryMenuResponse>>.Fail("Lỗi kết nối server."); // Trả về giá trị của 'result' nếu khác null, ngược lại trả về 'ApiResult<List<CategoryMenuResponse>>.Fail("Lỗi kết nối server.")'.
            }
            catch (Exception ex) // Khối bắt lỗi ngoại lệ Exception nếu xảy ra lỗi trong quá trình xử lý.
            {
                return ApiResult<List<CategoryMenuResponse>>.Fail($"Lỗi kết nối: {ex.Message}"); // Trả về giá trị của biểu thức 'ApiResult<List<CategoryMenuResponse>>.Fail($"Lỗi kết nối: {ex.Message}")'.
            }
        }

        public async Task<ApiResult<List<ProductCardResponse>>> GetFeaturedProductsAsync(int? categoryId, int take = 10) // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'GetFeaturedProductsAsync' nhận tham số (categoryId, 10) trả về kết quả kiểu Task<ApiResult<List<ProductCardResponse>>>.
        {
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var url = $"/api/storefront/products/featured?take={take}"; // Thực hiện gán giá trị của biểu thức '$"/api/storefront/products/featured?take={take}"' cho biến/thuộc tính 'url'.
                if (categoryId.HasValue) // Kiểm tra xem điều kiện 'categoryId.HasValue' có thỏa mãn hay không.
                {
                    url += $"&categoryId={categoryId.Value}"; // Thực thi dòng lệnh nghiệp vụ.
                }

                var response = await _httpClient.GetAsync(url); // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'response'.
                if (!response.IsSuccessStatusCode) // Kiểm tra xem điều kiện '!response.IsSuccessStatusCode' có thỏa mãn hay không.
                    return ApiResult<List<ProductCardResponse>>.Fail($"Lỗi HTTP {(int)response.StatusCode}."); // Trả về giá trị của biểu thức 'ApiResult<List<ProductCardResponse>>.Fail($"Lỗi HTTP {(int)response.StatusCode}.")'.
                var result = await response.Content.ReadFromJsonAsync<ApiResult<List<ProductCardResponse>>>(); // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'result'.
                return result ?? ApiResult<List<ProductCardResponse>>.Fail("Lỗi kết nối server."); // Trả về giá trị của 'result' nếu khác null, ngược lại trả về 'ApiResult<List<ProductCardResponse>>.Fail("Lỗi kết nối server.")'.
            }
            catch (Exception ex) // Khối bắt lỗi ngoại lệ Exception nếu xảy ra lỗi trong quá trình xử lý.
            {
                return ApiResult<List<ProductCardResponse>>.Fail($"Lỗi kết nối: {ex.Message}"); // Trả về giá trị của biểu thức 'ApiResult<List<ProductCardResponse>>.Fail($"Lỗi kết nối: {ex.Message}")'.
            }
        }

        public async Task<ApiResult<ProductDetailResponse>> GetProductDetailAsync(string slug) // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'GetProductDetailAsync' nhận tham số (slug) trả về kết quả kiểu Task<ApiResult<ProductDetailResponse>>.
        {
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var response = await _httpClient.GetAsync($"/api/storefront/products/{slug}"); // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'response'.
                if (!response.IsSuccessStatusCode) // Kiểm tra xem điều kiện '!response.IsSuccessStatusCode' có thỏa mãn hay không.
                    return ApiResult<ProductDetailResponse>.Fail($"Lỗi HTTP {(int)response.StatusCode}."); // Trả về giá trị của biểu thức 'ApiResult<ProductDetailResponse>.Fail($"Lỗi HTTP {(int)response.StatusCode}.")'.
                var result = await response.Content.ReadFromJsonAsync<ApiResult<ProductDetailResponse>>(); // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'result'.
                return result ?? ApiResult<ProductDetailResponse>.Fail("Lỗi kết nối server."); // Trả về giá trị của 'result' nếu khác null, ngược lại trả về 'ApiResult<ProductDetailResponse>.Fail("Lỗi kết nối server.")'.
            }
            catch (Exception ex) // Khối bắt lỗi ngoại lệ Exception nếu xảy ra lỗi trong quá trình xử lý.
            {
                return ApiResult<ProductDetailResponse>.Fail($"Lỗi kết nối: {ex.Message}"); // Trả về giá trị của biểu thức 'ApiResult<ProductDetailResponse>.Fail($"Lỗi kết nối: {ex.Message}")'.
            }
        }

        public async Task<ApiResult<List<ProductCardResponse>>> GetRelatedProductsAsync(string slug, int take = 5) // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'GetRelatedProductsAsync' nhận tham số (slug, 5) trả về kết quả kiểu Task<ApiResult<List<ProductCardResponse>>>.
        {
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var response = await _httpClient.GetAsync($"/api/storefront/products/{slug}/related?take={take}"); // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'response'.
                if (!response.IsSuccessStatusCode) // Kiểm tra xem điều kiện '!response.IsSuccessStatusCode' có thỏa mãn hay không.
                    return ApiResult<List<ProductCardResponse>>.Fail($"Lỗi HTTP {(int)response.StatusCode}."); // Trả về giá trị của biểu thức 'ApiResult<List<ProductCardResponse>>.Fail($"Lỗi HTTP {(int)response.StatusCode}.")'.
                var result = await response.Content.ReadFromJsonAsync<ApiResult<List<ProductCardResponse>>>(); // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'result'.
                return result ?? ApiResult<List<ProductCardResponse>>.Fail("Lỗi kết nối server."); // Trả về giá trị của 'result' nếu khác null, ngược lại trả về 'ApiResult<List<ProductCardResponse>>.Fail("Lỗi kết nối server.")'.
            }
            catch (Exception ex) // Khối bắt lỗi ngoại lệ Exception nếu xảy ra lỗi trong quá trình xử lý.
            {
                return ApiResult<List<ProductCardResponse>>.Fail($"Lỗi kết nối: {ex.Message}"); // Trả về giá trị của biểu thức 'ApiResult<List<ProductCardResponse>>.Fail($"Lỗi kết nối: {ex.Message}")'.
            }
        }

        public async Task<ApiResult<CategoryDetailResponse>> GetCategoryBySlugAsync(string slug) // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'GetCategoryBySlugAsync' nhận tham số (slug) trả về kết quả kiểu Task<ApiResult<CategoryDetailResponse>>.
        {
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var response = await _httpClient.GetAsync($"/api/storefront/categories/{slug}"); // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'response'.
                if (!response.IsSuccessStatusCode) // Kiểm tra xem điều kiện '!response.IsSuccessStatusCode' có thỏa mãn hay không.
                    return ApiResult<CategoryDetailResponse>.Fail($"Lỗi HTTP {(int)response.StatusCode}."); // Trả về giá trị của biểu thức 'ApiResult<CategoryDetailResponse>.Fail($"Lỗi HTTP {(int)response.StatusCode}.")'.
                var result = await response.Content.ReadFromJsonAsync<ApiResult<CategoryDetailResponse>>(); // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'result'.
                return result ?? ApiResult<CategoryDetailResponse>.Fail("Lỗi kết nối server."); // Trả về giá trị của 'result' nếu khác null, ngược lại trả về 'ApiResult<CategoryDetailResponse>.Fail("Lỗi kết nối server.")'.
            }
            catch (Exception ex) // Khối bắt lỗi ngoại lệ Exception nếu xảy ra lỗi trong quá trình xử lý.
            {
                return ApiResult<CategoryDetailResponse>.Fail($"Lỗi kết nối: {ex.Message}"); // Trả về giá trị của biểu thức 'ApiResult<CategoryDetailResponse>.Fail($"Lỗi kết nối: {ex.Message}")'.
            }
        }

        public async Task<ApiResult<PagedResult<ProductCardResponse>>> GetProductsByCategoryAsync( // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'GetProductsByCategoryAsync' không tham số trả về kết quả kiểu Task<ApiResult<PagedResult<ProductCardResponse>>>.
            string slug, int page = 1, int pageSize = 20) // Thực hiện gán giá trị của biểu thức '1, int pageSize = 20)' cho biến/thuộc tính 'string slug, int page'.
        {
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var response = await _httpClient.GetAsync( // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'response'.
                    $"/api/storefront/categories/{slug}/products?page={page}&pageSize={pageSize}"); // Thực thi dòng lệnh nghiệp vụ.
                if (!response.IsSuccessStatusCode) // Kiểm tra xem điều kiện '!response.IsSuccessStatusCode' có thỏa mãn hay không.
                    return ApiResult<PagedResult<ProductCardResponse>>.Fail($"Lỗi HTTP {(int)response.StatusCode}."); // Trả về giá trị của biểu thức 'ApiResult<PagedResult<ProductCardResponse>>.Fail($"Lỗi HTTP {(int)response.StatusCode}.")'.
                var result = await response.Content.ReadFromJsonAsync<ApiResult<PagedResult<ProductCardResponse>>>(); // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'result'.
                return result ?? ApiResult<PagedResult<ProductCardResponse>>.Fail("Lỗi kết nối server."); // Trả về giá trị của 'result' nếu khác null, ngược lại trả về 'ApiResult<PagedResult<ProductCardResponse>>.Fail("Lỗi kết nối server.")'.
            }
            catch (Exception ex) // Khối bắt lỗi ngoại lệ Exception nếu xảy ra lỗi trong quá trình xử lý.
            {
                return ApiResult<PagedResult<ProductCardResponse>>.Fail($"Lỗi kết nối: {ex.Message}"); // Trả về giá trị của biểu thức 'ApiResult<PagedResult<ProductCardResponse>>.Fail($"Lỗi kết nối: {ex.Message}")'.
            }
        }

        public async Task<ApiResult<PagedResult<ProductCardResponse>>> SearchProductsAsync( // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'SearchProductsAsync' không tham số trả về kết quả kiểu Task<ApiResult<PagedResult<ProductCardResponse>>>.
            string? keyword, int? categoryId, decimal? priceMin, decimal? priceMax, // Thực thi dòng lệnh nghiệp vụ.
            int page = 1, int pageSize = 20) // Thực hiện gán giá trị của biểu thức '1, int pageSize = 20)' cho biến/thuộc tính 'int page'.
        {
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var queryParams = new List<string>(); // Thực hiện gán giá trị của biểu thức 'new List<string>()' cho biến/thuộc tính 'queryParams'.

                if (!string.IsNullOrWhiteSpace(keyword)) // Kiểm tra xem điều kiện '!string.IsNullOrWhiteSpace(keyword' có thỏa mãn hay không.
                    queryParams.Add($"keyword={Uri.EscapeDataString(keyword)}"); // Thực thi dòng lệnh nghiệp vụ.
                if (categoryId.HasValue) // Kiểm tra xem điều kiện 'categoryId.HasValue' có thỏa mãn hay không.
                    queryParams.Add($"categoryId={categoryId.Value}"); // Thực thi dòng lệnh nghiệp vụ.
                if (priceMin.HasValue) // Kiểm tra xem điều kiện 'priceMin.HasValue' có thỏa mãn hay không.
                    queryParams.Add($"priceMin={priceMin.Value}"); // Thực thi dòng lệnh nghiệp vụ.
                if (priceMax.HasValue) // Kiểm tra xem điều kiện 'priceMax.HasValue' có thỏa mãn hay không.
                    queryParams.Add($"priceMax={priceMax.Value}"); // Thực thi dòng lệnh nghiệp vụ.

                queryParams.Add($"page={page}"); // Thực thi dòng lệnh nghiệp vụ.
                queryParams.Add($"pageSize={pageSize}"); // Thực thi dòng lệnh nghiệp vụ.

                var url = $"/api/storefront/products/search?{string.Join("&", queryParams)}"; // Thực hiện gán giá trị của biểu thức '$"/api/storefront/products/search?{string.Join("&", queryParams)}"' cho biến/thuộc tính 'url'.
                var response = await _httpClient.GetAsync(url); // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'response'.
                if (!response.IsSuccessStatusCode) // Kiểm tra xem điều kiện '!response.IsSuccessStatusCode' có thỏa mãn hay không.
                    return ApiResult<PagedResult<ProductCardResponse>>.Fail($"Lỗi HTTP {(int)response.StatusCode}."); // Trả về giá trị của biểu thức 'ApiResult<PagedResult<ProductCardResponse>>.Fail($"Lỗi HTTP {(int)response.StatusCode}.")'.
                var result = await response.Content.ReadFromJsonAsync<ApiResult<PagedResult<ProductCardResponse>>>(); // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'result'.
                return result ?? ApiResult<PagedResult<ProductCardResponse>>.Fail("Lỗi kết nối server."); // Trả về giá trị của 'result' nếu khác null, ngược lại trả về 'ApiResult<PagedResult<ProductCardResponse>>.Fail("Lỗi kết nối server.")'.
            }
            catch (Exception ex) // Khối bắt lỗi ngoại lệ Exception nếu xảy ra lỗi trong quá trình xử lý.
            {
                return ApiResult<PagedResult<ProductCardResponse>>.Fail($"Lỗi kết nối: {ex.Message}"); // Trả về giá trị của biểu thức 'ApiResult<PagedResult<ProductCardResponse>>.Fail($"Lỗi kết nối: {ex.Message}")'.
            }
        }
    }
}

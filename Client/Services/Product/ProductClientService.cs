using System.Net.Http.Json; // Sử dụng thư viện hỗ trợ gửi nhận dữ liệu JSON qua HTTP.
using PBL3.Shared.DTOs.Common; // Sử dụng các DTO của module Common thuộc tầng Shared.
using PBL3.Shared.DTOs.Products; // Sử dụng các DTO của module Products thuộc tầng Shared.

namespace Client.Services.Product // Thiết lập namespace Client.Services.Product để tổ chức quản lý cấu trúc các lớp.
{
    public class ProductClientService : IProductClientService // Định nghĩa lớp ProductClientService triển khai các dịch vụ hoặc kế thừa từ IProductClientService.
    {
        private readonly HttpClient _httpClient; // Khai báo đối tượng HttpClient nội bộ dùng để thực hiện cuộc gọi API.
        private const string BaseUrl = "api/products"; // Khai báo hằng số BaseUrl có giá trị là "api/products".

        public ProductClientService(HttpClient httpClient) // Hàm khởi tạo (Constructor) của lớp ProductClientService tiêm các phụ thuộc: httpClient.
        {
            _httpClient = httpClient; // Thực hiện gán giá trị của biểu thức 'httpClient' cho biến/thuộc tính '_httpClient'.
        }

        public async Task<ApiResult<PagedResult<ProductListDto>>> GetListAsync(ProductFilterRequest request) // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'GetListAsync' nhận tham số (request) trả về kết quả kiểu Task<ApiResult<PagedResult<ProductListDto>>>.
        {
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var queryParams = new List<string>(); // Thực hiện gán giá trị của biểu thức 'new List<string>()' cho biến/thuộc tính 'queryParams'.

                if (!string.IsNullOrWhiteSpace(request.Keyword)) // Kiểm tra xem điều kiện '!string.IsNullOrWhiteSpace(request.Keyword' có thỏa mãn hay không.
                    queryParams.Add($"Keyword={Uri.EscapeDataString(request.Keyword)}"); // Thực thi dòng lệnh nghiệp vụ.
                if (request.CategoryId.HasValue) // Kiểm tra xem điều kiện 'request.CategoryId.HasValue' có thỏa mãn hay không.
                    queryParams.Add($"CategoryId={request.CategoryId}"); // Thực thi dòng lệnh nghiệp vụ.
                if (request.ManufacturerId.HasValue) // Kiểm tra xem điều kiện 'request.ManufacturerId.HasValue' có thỏa mãn hay không.
                    queryParams.Add($"ManufacturerId={request.ManufacturerId}"); // Thực thi dòng lệnh nghiệp vụ.
                if (request.PriceMin.HasValue) // Kiểm tra xem điều kiện 'request.PriceMin.HasValue' có thỏa mãn hay không.
                    queryParams.Add($"PriceMin={request.PriceMin}"); // Thực thi dòng lệnh nghiệp vụ.
                if (request.PriceMax.HasValue) // Kiểm tra xem điều kiện 'request.PriceMax.HasValue' có thỏa mãn hay không.
                    queryParams.Add($"PriceMax={request.PriceMax}"); // Thực thi dòng lệnh nghiệp vụ.
                if (request.Status.HasValue) // Kiểm tra xem điều kiện 'request.Status.HasValue' có thỏa mãn hay không.
                    queryParams.Add($"Status={(int)request.Status}"); // Thực thi dòng lệnh nghiệp vụ.

                queryParams.Add($"PageNumber={request.PageNumber}"); // Thực thi dòng lệnh nghiệp vụ.
                queryParams.Add($"PageSize={request.PageSize}"); // Thực thi dòng lệnh nghiệp vụ.

                if (!string.IsNullOrWhiteSpace(request.SortBy)) // Kiểm tra xem điều kiện '!string.IsNullOrWhiteSpace(request.SortBy' có thỏa mãn hay không.
                    queryParams.Add($"SortBy={Uri.EscapeDataString(request.SortBy)}"); // Thực thi dòng lệnh nghiệp vụ.
                if (request.SortDescending) // Kiểm tra xem điều kiện 'request.SortDescending' có thỏa mãn hay không.
                    queryParams.Add("SortDescending=true"); // Thực thi dòng lệnh nghiệp vụ.

                var url = $"{BaseUrl}?{string.Join("&", queryParams)}"; // Thực hiện gán giá trị của biểu thức '$"{BaseUrl}?{string.Join("&", queryParams)}"' cho biến/thuộc tính 'url'.

                var result = await _httpClient // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'result'.
                    .GetFromJsonAsync<ApiResult<PagedResult<ProductListDto>>>(url); // Gọi phương thức GET bất đồng bộ tới URL 'url' nhận kết quả kiểu ApiResult<PagedResult<ProductListDto>>.
                return result ?? ApiResult<PagedResult<ProductListDto>>.Fail("Không thể tải danh sách sản phẩm."); // Trả về giá trị của 'result' nếu khác null, ngược lại trả về 'ApiResult<PagedResult<ProductListDto>>.Fail("Không thể tải danh sách sản phẩm.")'.
            }
            catch (Exception ex) // Khối bắt lỗi ngoại lệ Exception nếu xảy ra lỗi trong quá trình xử lý.
            {
                return ApiResult<PagedResult<ProductListDto>>.Fail($"Lỗi kết nối: {ex.Message}"); // Trả về giá trị của biểu thức 'ApiResult<PagedResult<ProductListDto>>.Fail($"Lỗi kết nối: {ex.Message}")'.
            }
        }

        public async Task<ApiResult<ProductDetailDto>> GetByIdAsync(int id) // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'GetByIdAsync' nhận tham số (id) trả về kết quả kiểu Task<ApiResult<ProductDetailDto>>.
        {
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var result = await _httpClient // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'result'.
                    .GetFromJsonAsync<ApiResult<ProductDetailDto>>($"{BaseUrl}/{id}"); // Gọi phương thức GET bất đồng bộ tới URL '{BaseUrl}/{id}' nhận kết quả kiểu ApiResult<ProductDetailDto>.
                return result ?? ApiResult<ProductDetailDto>.Fail("Không tìm thấy sản phẩm."); // Trả về giá trị của 'result' nếu khác null, ngược lại trả về 'ApiResult<ProductDetailDto>.Fail("Không tìm thấy sản phẩm.")'.
            }
            catch (Exception ex) // Khối bắt lỗi ngoại lệ Exception nếu xảy ra lỗi trong quá trình xử lý.
            {
                return ApiResult<ProductDetailDto>.Fail($"Lỗi kết nối: {ex.Message}"); // Trả về giá trị của biểu thức 'ApiResult<ProductDetailDto>.Fail($"Lỗi kết nối: {ex.Message}")'.
            }
        }

        public async Task<ApiResult<ProductDetailDto>> CreateAsync(CreateProductRequest request) // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'CreateAsync' nhận tham số (request) trả về kết quả kiểu Task<ApiResult<ProductDetailDto>>.
        {
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var response = await _httpClient.PostAsJsonAsync(BaseUrl, request); // Gọi phương thức POST (gửi dữ liệu JSON) bất đồng bộ tới URL 'BaseUrl' và gán kết quả cho biến 'response'.
                var result = await response.Content.ReadFromJsonAsync<ApiResult<ProductDetailDto>>(); // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'result'.
                return result ?? ApiResult<ProductDetailDto>.Fail("Không thể tạo sản phẩm."); // Trả về giá trị của 'result' nếu khác null, ngược lại trả về 'ApiResult<ProductDetailDto>.Fail("Không thể tạo sản phẩm.")'.
            }
            catch (Exception ex) // Khối bắt lỗi ngoại lệ Exception nếu xảy ra lỗi trong quá trình xử lý.
            {
                return ApiResult<ProductDetailDto>.Fail($"Lỗi kết nối: {ex.Message}"); // Trả về giá trị của biểu thức 'ApiResult<ProductDetailDto>.Fail($"Lỗi kết nối: {ex.Message}")'.
            }
        }

        public async Task<ApiResult<ProductDetailDto>> UpdateAsync(int id, UpdateProductRequest request) // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'UpdateAsync' nhận tham số (id, request) trả về kết quả kiểu Task<ApiResult<ProductDetailDto>>.
        {
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/{id}", request); // Gọi phương thức PUT (cập nhật dữ liệu JSON) bất đồng bộ tới URL '{BaseUrl}/{id}' và gán kết quả cho biến 'response'.
                var result = await response.Content.ReadFromJsonAsync<ApiResult<ProductDetailDto>>(); // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'result'.
                return result ?? ApiResult<ProductDetailDto>.Fail("Không thể cập nhật sản phẩm."); // Trả về giá trị của 'result' nếu khác null, ngược lại trả về 'ApiResult<ProductDetailDto>.Fail("Không thể cập nhật sản phẩm.")'.
            }
            catch (Exception ex) // Khối bắt lỗi ngoại lệ Exception nếu xảy ra lỗi trong quá trình xử lý.
            {
                return ApiResult<ProductDetailDto>.Fail($"Lỗi kết nối: {ex.Message}"); // Trả về giá trị của biểu thức 'ApiResult<ProductDetailDto>.Fail($"Lỗi kết nối: {ex.Message}")'.
            }
        }

        public async Task<ApiResult<ProductVariantDto>> AddVariantAsync(int productId, SaveVariantRequest request) // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'AddVariantAsync' nhận tham số (productId, request) trả về kết quả kiểu Task<ApiResult<ProductVariantDto>>.
        {
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/{productId}/variants", request); // Gọi phương thức POST (gửi dữ liệu JSON) bất đồng bộ tới URL '{BaseUrl}/{productId}/variants' và gán kết quả cho biến 'response'.
                var result = await response.Content.ReadFromJsonAsync<ApiResult<ProductVariantDto>>(); // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'result'.
                return result ?? ApiResult<ProductVariantDto>.Fail("Không thể lưu biến thể."); // Trả về giá trị của 'result' nếu khác null, ngược lại trả về 'ApiResult<ProductVariantDto>.Fail("Không thể lưu biến thể.")'.
            }
            catch (Exception ex) // Khối bắt lỗi ngoại lệ Exception nếu xảy ra lỗi trong quá trình xử lý.
            {
                return ApiResult<ProductVariantDto>.Fail($"Lỗi kết nối: {ex.Message}"); // Trả về giá trị của biểu thức 'ApiResult<ProductVariantDto>.Fail($"Lỗi kết nối: {ex.Message}")'.
            }
        }

        public async Task<ApiResult<bool>> UpdateProductImagesAsync(int productId, List<SaveImageRequest> images) // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'UpdateProductImagesAsync' nhận tham số (productId, images) trả về kết quả kiểu Task<ApiResult<bool>>.
        {
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/{productId}/images", images); // Gọi phương thức PUT (cập nhật dữ liệu JSON) bất đồng bộ tới URL '{BaseUrl}/{productId}/images' và gán kết quả cho biến 'response'.
                var result = await response.Content.ReadFromJsonAsync<ApiResult<bool>>(); // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'result'.
                return result ?? ApiResult<bool>.Fail("Không thể cập nhật ảnh sản phẩm."); // Trả về giá trị của 'result' nếu khác null, ngược lại trả về 'ApiResult<bool>.Fail("Không thể cập nhật ảnh sản phẩm.")'.
            }
            catch (Exception ex) // Khối bắt lỗi ngoại lệ Exception nếu xảy ra lỗi trong quá trình xử lý.
            {
                return ApiResult<bool>.Fail($"Lỗi kết nối: {ex.Message}"); // Trả về giá trị của biểu thức 'ApiResult<bool>.Fail($"Lỗi kết nối: {ex.Message}")'.
            }
        }

        public async Task<ApiResult<bool>> DeleteAsync(int id) // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'DeleteAsync' nhận tham số (id) trả về kết quả kiểu Task<ApiResult<bool>>.
        {
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var response = await _httpClient.DeleteAsync($"{BaseUrl}/{id}"); // Gọi phương thức DELETE (xóa tài nguyên) bất đồng bộ tới URL '{BaseUrl}/{id}' và gán kết quả cho biến 'response'.
                var result = await response.Content.ReadFromJsonAsync<ApiResult<bool>>(); // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'result'.
                return result ?? ApiResult<bool>.Fail("Không thể xóa sản phẩm."); // Trả về giá trị của 'result' nếu khác null, ngược lại trả về 'ApiResult<bool>.Fail("Không thể xóa sản phẩm.")'.
            }
            catch (Exception ex) // Khối bắt lỗi ngoại lệ Exception nếu xảy ra lỗi trong quá trình xử lý.
            {
                return ApiResult<bool>.Fail($"Lỗi kết nối: {ex.Message}"); // Trả về giá trị của biểu thức 'ApiResult<bool>.Fail($"Lỗi kết nối: {ex.Message}")'.
            }
        }
    }
}

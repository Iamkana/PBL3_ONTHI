using System.Net.Http.Json; // Sử dụng thư viện hỗ trợ gửi nhận dữ liệu JSON qua HTTP.
using PBL3.Shared.DTOs.Categories; // Sử dụng các DTO của module Categories thuộc tầng Shared.
using PBL3.Shared.DTOs.Common; // Sử dụng các DTO của module Common thuộc tầng Shared.

namespace Client.Services.Category // Thiết lập namespace Client.Services.Category để tổ chức quản lý cấu trúc các lớp.
{
    public class CategoryClientService : ICategoryClientService // Định nghĩa lớp CategoryClientService triển khai các dịch vụ hoặc kế thừa từ ICategoryClientService.
    {
        private readonly HttpClient _httpClient; // Khai báo đối tượng HttpClient nội bộ dùng để thực hiện cuộc gọi API.
        private const string BaseUrl = "api/categories"; // Khai báo hằng số BaseUrl có giá trị là "api/categories".

        public CategoryClientService(HttpClient httpClient) // Hàm khởi tạo (Constructor) của lớp CategoryClientService tiêm các phụ thuộc: httpClient.
        {
            _httpClient = httpClient; // Thực hiện gán giá trị của biểu thức 'httpClient' cho biến/thuộc tính '_httpClient'.
        }

        public async Task<ApiResult<List<CategoryTreeDto>>> GetTreeAsync() // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'GetTreeAsync' không tham số trả về kết quả kiểu Task<ApiResult<List<CategoryTreeDto>>>.
        {
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var result = await _httpClient // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'result'.
                    .GetFromJsonAsync<ApiResult<List<CategoryTreeDto>>>($"{BaseUrl}/tree"); // Gọi phương thức GET bất đồng bộ tới URL '{BaseUrl}/tree' nhận kết quả kiểu ApiResult<List<CategoryTreeDto>>.
                return result ?? ApiResult<List<CategoryTreeDto>>.Fail("Không thể tải danh sách danh mục."); // Trả về giá trị của 'result' nếu khác null, ngược lại trả về 'ApiResult<List<CategoryTreeDto>>.Fail("Không thể tải danh sách danh mục.")'.
            }
            catch (Exception ex) // Khối bắt lỗi ngoại lệ Exception nếu xảy ra lỗi trong quá trình xử lý.
            {
                return ApiResult<List<CategoryTreeDto>>.Fail($"Lỗi kết nối: {ex.Message}"); // Trả về giá trị của biểu thức 'ApiResult<List<CategoryTreeDto>>.Fail($"Lỗi kết nối: {ex.Message}")'.
            }
        }

        public async Task<ApiResult<CategoryDto>> GetByIdAsync(int id) // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'GetByIdAsync' nhận tham số (id) trả về kết quả kiểu Task<ApiResult<CategoryDto>>.
        {
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var result = await _httpClient // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'result'.
                    .GetFromJsonAsync<ApiResult<CategoryDto>>($"{BaseUrl}/{id}"); // Gọi phương thức GET bất đồng bộ tới URL '{BaseUrl}/{id}' nhận kết quả kiểu ApiResult<CategoryDto>.
                return result ?? ApiResult<CategoryDto>.Fail("Không tìm thấy danh mục."); // Trả về giá trị của 'result' nếu khác null, ngược lại trả về 'ApiResult<CategoryDto>.Fail("Không tìm thấy danh mục.")'.
            }
            catch (Exception ex) // Khối bắt lỗi ngoại lệ Exception nếu xảy ra lỗi trong quá trình xử lý.
            {
                return ApiResult<CategoryDto>.Fail($"Lỗi kết nối: {ex.Message}"); // Trả về giá trị của biểu thức 'ApiResult<CategoryDto>.Fail($"Lỗi kết nối: {ex.Message}")'.
            }
        }

        public async Task<ApiResult<CategoryDto>> CreateAsync(CreateCategoryRequest request) // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'CreateAsync' nhận tham số (request) trả về kết quả kiểu Task<ApiResult<CategoryDto>>.
        {
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var response = await _httpClient.PostAsJsonAsync(BaseUrl, request); // Gọi phương thức POST (gửi dữ liệu JSON) bất đồng bộ tới URL 'BaseUrl' và gán kết quả cho biến 'response'.
                var result = await response.Content.ReadFromJsonAsync<ApiResult<CategoryDto>>(); // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'result'.
                return result ?? ApiResult<CategoryDto>.Fail("Không thể tạo danh mục."); // Trả về giá trị của 'result' nếu khác null, ngược lại trả về 'ApiResult<CategoryDto>.Fail("Không thể tạo danh mục.")'.
            }
            catch (Exception ex) // Khối bắt lỗi ngoại lệ Exception nếu xảy ra lỗi trong quá trình xử lý.
            {
                return ApiResult<CategoryDto>.Fail($"Lỗi kết nối: {ex.Message}"); // Trả về giá trị của biểu thức 'ApiResult<CategoryDto>.Fail($"Lỗi kết nối: {ex.Message}")'.
            }
        }

        public async Task<ApiResult<CategoryDto>> UpdateAsync(int id, UpdateCategoryRequest request) // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'UpdateAsync' nhận tham số (id, request) trả về kết quả kiểu Task<ApiResult<CategoryDto>>.
        {
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/{id}", request); // Gọi phương thức PUT (cập nhật dữ liệu JSON) bất đồng bộ tới URL '{BaseUrl}/{id}' và gán kết quả cho biến 'response'.
                var result = await response.Content.ReadFromJsonAsync<ApiResult<CategoryDto>>(); // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'result'.
                return result ?? ApiResult<CategoryDto>.Fail("Không thể cập nhật danh mục."); // Trả về giá trị của 'result' nếu khác null, ngược lại trả về 'ApiResult<CategoryDto>.Fail("Không thể cập nhật danh mục.")'.
            }
            catch (Exception ex) // Khối bắt lỗi ngoại lệ Exception nếu xảy ra lỗi trong quá trình xử lý.
            {
                return ApiResult<CategoryDto>.Fail($"Lỗi kết nối: {ex.Message}"); // Trả về giá trị của biểu thức 'ApiResult<CategoryDto>.Fail($"Lỗi kết nối: {ex.Message}")'.
            }
        }

        public async Task<ApiResult<bool>> DeleteAsync(int id) // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'DeleteAsync' nhận tham số (id) trả về kết quả kiểu Task<ApiResult<bool>>.
        {
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var response = await _httpClient.DeleteAsync($"{BaseUrl}/{id}"); // Gọi phương thức DELETE (xóa tài nguyên) bất đồng bộ tới URL '{BaseUrl}/{id}' và gán kết quả cho biến 'response'.
                var result = await response.Content.ReadFromJsonAsync<ApiResult<bool>>(); // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'result'.
                return result ?? ApiResult<bool>.Fail("Không thể xóa danh mục."); // Trả về giá trị của 'result' nếu khác null, ngược lại trả về 'ApiResult<bool>.Fail("Không thể xóa danh mục.")'.
            }
            catch (Exception ex) // Khối bắt lỗi ngoại lệ Exception nếu xảy ra lỗi trong quá trình xử lý.
            {
                return ApiResult<bool>.Fail($"Lỗi kết nối: {ex.Message}"); // Trả về giá trị của biểu thức 'ApiResult<bool>.Fail($"Lỗi kết nối: {ex.Message}")'.
            }
        }
    }
}

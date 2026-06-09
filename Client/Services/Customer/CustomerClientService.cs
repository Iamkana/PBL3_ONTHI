using System.Net.Http.Json; // Sử dụng thư viện hỗ trợ gửi nhận dữ liệu JSON qua HTTP.
using PBL3.Shared.DTOs.Common; // Sử dụng các DTO của module Common thuộc tầng Shared.
using PBL3.Shared.DTOs.Customers; // Sử dụng các DTO của module Customers thuộc tầng Shared.
using PBL3.Shared.DTOs.Products; // Sử dụng các DTO của module Products thuộc tầng Shared.

namespace Client.Services.Customer // Thiết lập namespace Client.Services.Customer để tổ chức quản lý cấu trúc các lớp.
{
    public class CustomerClientService : ICustomerClientService // Định nghĩa lớp CustomerClientService triển khai các dịch vụ hoặc kế thừa từ ICustomerClientService.
    {
        private readonly HttpClient _httpClient; // Khai báo đối tượng HttpClient nội bộ dùng để thực hiện cuộc gọi API.
        private const string BaseUrl = "api/customers"; // Khai báo hằng số BaseUrl có giá trị là "api/customers".

        public CustomerClientService(HttpClient httpClient) // Hàm khởi tạo (Constructor) của lớp CustomerClientService tiêm các phụ thuộc: httpClient.
        {
            _httpClient = httpClient; // Thực hiện gán giá trị của biểu thức 'httpClient' cho biến/thuộc tính '_httpClient'.
        }

        public async Task<ApiResult<PagedResult<CustomerDto>>> GetListAsync(CustomerFilterRequest request) // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'GetListAsync' nhận tham số (request) trả về kết quả kiểu Task<ApiResult<PagedResult<CustomerDto>>>.
        {
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var queryParams = new List<string>(); // Thực hiện gán giá trị của biểu thức 'new List<string>()' cho biến/thuộc tính 'queryParams'.

                if (!string.IsNullOrWhiteSpace(request.Keyword)) // Kiểm tra xem điều kiện '!string.IsNullOrWhiteSpace(request.Keyword' có thỏa mãn hay không.
                    queryParams.Add($"Keyword={Uri.EscapeDataString(request.Keyword)}"); // Thực thi dòng lệnh nghiệp vụ.

                if (request.IsActive.HasValue) // Kiểm tra xem điều kiện 'request.IsActive.HasValue' có thỏa mãn hay không.
                    queryParams.Add($"IsActive={request.IsActive.Value.ToString().ToLower()}"); // Thực thi dòng lệnh nghiệp vụ.

                if (request.Gender.HasValue) // Kiểm tra xem điều kiện 'request.Gender.HasValue' có thỏa mãn hay không.
                    queryParams.Add($"Gender={request.Gender.Value}"); // Thực thi dòng lệnh nghiệp vụ.

                queryParams.Add($"PageNumber={request.PageNumber}"); // Thực thi dòng lệnh nghiệp vụ.
                queryParams.Add($"PageSize={request.PageSize}"); // Thực thi dòng lệnh nghiệp vụ.

                if (!string.IsNullOrWhiteSpace(request.SortBy)) // Kiểm tra xem điều kiện '!string.IsNullOrWhiteSpace(request.SortBy' có thỏa mãn hay không.
                    queryParams.Add($"SortBy={Uri.EscapeDataString(request.SortBy)}"); // Thực thi dòng lệnh nghiệp vụ.
                if (request.SortDescending) // Kiểm tra xem điều kiện 'request.SortDescending' có thỏa mãn hay không.
                    queryParams.Add("SortDescending=true"); // Thực thi dòng lệnh nghiệp vụ.

                var url = $"{BaseUrl}?{string.Join("&", queryParams)}"; // Thực hiện gán giá trị của biểu thức '$"{BaseUrl}?{string.Join("&", queryParams)}"' cho biến/thuộc tính 'url'.

                var result = await _httpClient.GetFromJsonAsync<ApiResult<PagedResult<CustomerDto>>>(url); // Gọi phương thức GET bất đồng bộ tới URL 'url' nhận kết quả kiểu ApiResult<PagedResult<CustomerDto>> và gán kết quả cho biến 'result'.
                return result ?? ApiResult<PagedResult<CustomerDto>>.Fail("Không thể tải danh sách khách hàng."); // Trả về giá trị của 'result' nếu khác null, ngược lại trả về 'ApiResult<PagedResult<CustomerDto>>.Fail("Không thể tải danh sách khách hàng.")'.
            }
            catch (Exception ex) // Khối bắt lỗi ngoại lệ Exception nếu xảy ra lỗi trong quá trình xử lý.
            {
                return ApiResult<PagedResult<CustomerDto>>.Fail($"Lỗi kết nối: {ex.Message}"); // Trả về giá trị của biểu thức 'ApiResult<PagedResult<CustomerDto>>.Fail($"Lỗi kết nối: {ex.Message}")'.
            }
        }

        public async Task<ApiResult<CustomerDetailDto>> GetByIdAsync(Guid id) // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'GetByIdAsync' nhận tham số (id) trả về kết quả kiểu Task<ApiResult<CustomerDetailDto>>.
        {
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var result = await _httpClient.GetFromJsonAsync<ApiResult<CustomerDetailDto>>($"{BaseUrl}/{id}"); // Gọi phương thức GET bất đồng bộ tới URL '{BaseUrl}/{id}' nhận kết quả kiểu ApiResult<CustomerDetailDto> và gán kết quả cho biến 'result'.
                return result ?? ApiResult<CustomerDetailDto>.Fail("Không tìm thấy khách hàng."); // Trả về giá trị của 'result' nếu khác null, ngược lại trả về 'ApiResult<CustomerDetailDto>.Fail("Không tìm thấy khách hàng.")'.
            }
            catch (Exception ex) // Khối bắt lỗi ngoại lệ Exception nếu xảy ra lỗi trong quá trình xử lý.
            {
                return ApiResult<CustomerDetailDto>.Fail($"Lỗi kết nối: {ex.Message}"); // Trả về giá trị của biểu thức 'ApiResult<CustomerDetailDto>.Fail($"Lỗi kết nối: {ex.Message}")'.
            }
        }

        public async Task<ApiResult<CustomerDto>> CreateAsync(CreateCustomerRequest request) // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'CreateAsync' nhận tham số (request) trả về kết quả kiểu Task<ApiResult<CustomerDto>>.
        {
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var response = await _httpClient.PostAsJsonAsync(BaseUrl, request); // Gọi phương thức POST (gửi dữ liệu JSON) bất đồng bộ tới URL 'BaseUrl' và gán kết quả cho biến 'response'.
                var result = await response.Content.ReadFromJsonAsync<ApiResult<CustomerDto>>(); // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'result'.
                return result ?? ApiResult<CustomerDto>.Fail("Không thể tạo khách hàng."); // Trả về giá trị của 'result' nếu khác null, ngược lại trả về 'ApiResult<CustomerDto>.Fail("Không thể tạo khách hàng.")'.
            }
            catch (Exception ex) // Khối bắt lỗi ngoại lệ Exception nếu xảy ra lỗi trong quá trình xử lý.
            {
                return ApiResult<CustomerDto>.Fail($"Lỗi kết nối: {ex.Message}"); // Trả về giá trị của biểu thức 'ApiResult<CustomerDto>.Fail($"Lỗi kết nối: {ex.Message}")'.
            }
        }

        public async Task<ApiResult<CustomerDto>> UpdateAsync(Guid id, UpdateCustomerRequest request) // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'UpdateAsync' nhận tham số (id, request) trả về kết quả kiểu Task<ApiResult<CustomerDto>>.
        {
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/{id}", request); // Gọi phương thức PUT (cập nhật dữ liệu JSON) bất đồng bộ tới URL '{BaseUrl}/{id}' và gán kết quả cho biến 'response'.
                var result = await response.Content.ReadFromJsonAsync<ApiResult<CustomerDto>>(); // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'result'.
                return result ?? ApiResult<CustomerDto>.Fail("Không thể cập nhật khách hàng."); // Trả về giá trị của 'result' nếu khác null, ngược lại trả về 'ApiResult<CustomerDto>.Fail("Không thể cập nhật khách hàng.")'.
            }
            catch (Exception ex) // Khối bắt lỗi ngoại lệ Exception nếu xảy ra lỗi trong quá trình xử lý.
            {
                return ApiResult<CustomerDto>.Fail($"Lỗi kết nối: {ex.Message}"); // Trả về giá trị của biểu thức 'ApiResult<CustomerDto>.Fail($"Lỗi kết nối: {ex.Message}")'.
            }
        }

        public async Task<ApiResult<bool>> DeactivateAsync(Guid id, string? lockReason = null) // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'DeactivateAsync' nhận tham số (id, null) trả về kết quả kiểu Task<ApiResult<bool>>.
        {
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var url = string.IsNullOrEmpty(lockReason) // Thực hiện gán giá trị của biểu thức 'string.IsNullOrEmpty(lockReason)' cho biến/thuộc tính 'url'.
                    ? $"{BaseUrl}/{id}" // Thực thi dòng lệnh nghiệp vụ.
                    : $"{BaseUrl}/{id}?lockReason={Uri.EscapeDataString(lockReason)}"; // Thực thi dòng lệnh nghiệp vụ.
                var response = await _httpClient.DeleteAsync(url); // Gọi phương thức DELETE (xóa tài nguyên) bất đồng bộ tới URL 'url' và gán kết quả cho biến 'response'.
                var result = await response.Content.ReadFromJsonAsync<ApiResult<bool>>(); // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'result'.
                return result ?? ApiResult<bool>.Fail("Không thể khóa tài khoản khách hàng."); // Trả về giá trị của 'result' nếu khác null, ngược lại trả về 'ApiResult<bool>.Fail("Không thể khóa tài khoản khách hàng.")'.
            }
            catch (Exception ex) // Khối bắt lỗi ngoại lệ Exception nếu xảy ra lỗi trong quá trình xử lý.
            {
                return ApiResult<bool>.Fail($"Lỗi kết nối: {ex.Message}"); // Trả về giá trị của biểu thức 'ApiResult<bool>.Fail($"Lỗi kết nối: {ex.Message}")'.
            }
        }

        public async Task<ApiResult<bool>> ReactivateAsync(Guid id) // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'ReactivateAsync' nhận tham số (id) trả về kết quả kiểu Task<ApiResult<bool>>.
        {
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/{id}/activate", (object?)null); // Gọi phương thức PUT (cập nhật dữ liệu JSON) bất đồng bộ tới URL '{BaseUrl}/{id}/activate' và gán kết quả cho biến 'response'.
                var result = await response.Content.ReadFromJsonAsync<ApiResult<bool>>(); // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'result'.
                return result ?? ApiResult<bool>.Fail("Không thể mở khóa tài khoản khách hàng."); // Trả về giá trị của 'result' nếu khác null, ngược lại trả về 'ApiResult<bool>.Fail("Không thể mở khóa tài khoản khách hàng.")'.
            }
            catch (Exception ex) // Khối bắt lỗi ngoại lệ Exception nếu xảy ra lỗi trong quá trình xử lý.
            {
                return ApiResult<bool>.Fail($"Lỗi kết nối: {ex.Message}"); // Trả về giá trị của biểu thức 'ApiResult<bool>.Fail($"Lỗi kết nối: {ex.Message}")'.
            }
        }

        public async Task<ApiResult<CustomerDto>> GetMyProfileAsync() // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'GetMyProfileAsync' không tham số trả về kết quả kiểu Task<ApiResult<CustomerDto>>.
        {
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var result = await _httpClient.GetFromJsonAsync<ApiResult<CustomerDto>>("api/storefront/profile/me"); // Gọi phương thức GET bất đồng bộ tới URL 'api/storefront/profile/me' nhận kết quả kiểu ApiResult<CustomerDto> và gán kết quả cho biến 'result'.
                return result ?? ApiResult<CustomerDto>.Fail("Không thể tải thông tin cá nhân."); // Trả về giá trị của 'result' nếu khác null, ngược lại trả về 'ApiResult<CustomerDto>.Fail("Không thể tải thông tin cá nhân.")'.
            }
            catch (Exception ex) // Khối bắt lỗi ngoại lệ Exception nếu xảy ra lỗi trong quá trình xử lý.
            {
                return ApiResult<CustomerDto>.Fail($"Lỗi kết nối: {ex.Message}"); // Trả về giá trị của biểu thức 'ApiResult<CustomerDto>.Fail($"Lỗi kết nối: {ex.Message}")'.
            }
        }

        public async Task<ApiResult<CustomerDto>> UpdateMyProfileAsync(UpdateCustomerRequest request) // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'UpdateMyProfileAsync' nhận tham số (request) trả về kết quả kiểu Task<ApiResult<CustomerDto>>.
        {
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var response = await _httpClient.PutAsJsonAsync("api/storefront/profile/me", request); // Gọi phương thức PUT (cập nhật dữ liệu JSON) bất đồng bộ tới URL 'api/storefront/profile/me' và gán kết quả cho biến 'response'.
                var result = await response.Content.ReadFromJsonAsync<ApiResult<CustomerDto>>(); // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'result'.
                return result ?? ApiResult<CustomerDto>.Fail("Không thể cập nhật thông tin cá nhân."); // Trả về giá trị của 'result' nếu khác null, ngược lại trả về 'ApiResult<CustomerDto>.Fail("Không thể cập nhật thông tin cá nhân.")'.
            }
            catch (Exception ex) // Khối bắt lỗi ngoại lệ Exception nếu xảy ra lỗi trong quá trình xử lý.
            {
                return ApiResult<CustomerDto>.Fail($"Lỗi kết nối: {ex.Message}"); // Trả về giá trị của biểu thức 'ApiResult<CustomerDto>.Fail($"Lỗi kết nối: {ex.Message}")'.
            }
        }
    }
}

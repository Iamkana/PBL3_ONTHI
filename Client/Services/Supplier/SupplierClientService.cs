using System.Net.Http.Json; // Sử dụng thư viện hỗ trợ gửi nhận dữ liệu JSON qua HTTP.
using PBL3.Shared.DTOs.Common; // Sử dụng các DTO của module Common thuộc tầng Shared.
using PBL3.Shared.DTOs.Products; // Sử dụng các DTO của module Products thuộc tầng Shared.
using PBL3.Shared.DTOs.Suppliers; // Sử dụng các DTO của module Suppliers thuộc tầng Shared.

namespace Client.Services.Supplier // Thiết lập namespace Client.Services.Supplier để tổ chức quản lý cấu trúc các lớp.
{
    public class SupplierClientService : ISupplierClientService // Định nghĩa lớp SupplierClientService triển khai các dịch vụ hoặc kế thừa từ ISupplierClientService.
    {
        private readonly HttpClient _httpClient; // Khai báo đối tượng HttpClient nội bộ dùng để thực hiện cuộc gọi API.
        private const string BaseUrl = "api/suppliers"; // Khai báo hằng số BaseUrl có giá trị là "api/suppliers".

        public SupplierClientService(HttpClient httpClient) // Hàm khởi tạo (Constructor) của lớp SupplierClientService tiêm các phụ thuộc: httpClient.
        {
            _httpClient = httpClient; // Thực hiện gán giá trị của biểu thức 'httpClient' cho biến/thuộc tính '_httpClient'.
        }

        public async Task<ApiResult<PagedResult<SupplierDto>>> GetListAsync(SupplierFilterRequest request) // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'GetListAsync' nhận tham số (request) trả về kết quả kiểu Task<ApiResult<PagedResult<SupplierDto>>>.
        {
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var queryParams = new List<string>(); // Thực hiện gán giá trị của biểu thức 'new List<string>()' cho biến/thuộc tính 'queryParams'.

                if (!string.IsNullOrWhiteSpace(request.Keyword)) // Kiểm tra xem điều kiện '!string.IsNullOrWhiteSpace(request.Keyword' có thỏa mãn hay không.
                    queryParams.Add($"Keyword={Uri.EscapeDataString(request.Keyword)}"); // Thực thi dòng lệnh nghiệp vụ.

                queryParams.Add($"PageNumber={request.PageNumber}"); // Thực thi dòng lệnh nghiệp vụ.
                queryParams.Add($"PageSize={request.PageSize}"); // Thực thi dòng lệnh nghiệp vụ.

                if (!string.IsNullOrWhiteSpace(request.SortBy)) // Kiểm tra xem điều kiện '!string.IsNullOrWhiteSpace(request.SortBy' có thỏa mãn hay không.
                    queryParams.Add($"SortBy={Uri.EscapeDataString(request.SortBy)}"); // Thực thi dòng lệnh nghiệp vụ.
                if (request.SortDescending) // Kiểm tra xem điều kiện 'request.SortDescending' có thỏa mãn hay không.
                    queryParams.Add("SortDescending=true"); // Thực thi dòng lệnh nghiệp vụ.

                var url = $"{BaseUrl}?{string.Join("&", queryParams)}"; // Thực hiện gán giá trị của biểu thức '$"{BaseUrl}?{string.Join("&", queryParams)}"' cho biến/thuộc tính 'url'.

                var result = await _httpClient // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'result'.
                    .GetFromJsonAsync<ApiResult<PagedResult<SupplierDto>>>(url); // Gọi phương thức GET bất đồng bộ tới URL 'url' nhận kết quả kiểu ApiResult<PagedResult<SupplierDto>>.
                return result ?? ApiResult<PagedResult<SupplierDto>>.Fail("Không thể tải danh sách nhà cung cấp."); // Trả về giá trị của 'result' nếu khác null, ngược lại trả về 'ApiResult<PagedResult<SupplierDto>>.Fail("Không thể tải danh sách nhà cung cấp.")'.
            }
            catch (Exception ex) // Khối bắt lỗi ngoại lệ Exception nếu xảy ra lỗi trong quá trình xử lý.
            {
                return ApiResult<PagedResult<SupplierDto>>.Fail($"Lỗi kết nối: {ex.Message}"); // Trả về giá trị của biểu thức 'ApiResult<PagedResult<SupplierDto>>.Fail($"Lỗi kết nối: {ex.Message}")'.
            }
        }

        public async Task<ApiResult<SupplierDto>> GetByIdAsync(int id) // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'GetByIdAsync' nhận tham số (id) trả về kết quả kiểu Task<ApiResult<SupplierDto>>.
        {
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var result = await _httpClient // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'result'.
                    .GetFromJsonAsync<ApiResult<SupplierDto>>($"{BaseUrl}/{id}"); // Gọi phương thức GET bất đồng bộ tới URL '{BaseUrl}/{id}' nhận kết quả kiểu ApiResult<SupplierDto>.
                return result ?? ApiResult<SupplierDto>.Fail("Không tìm thấy nhà cung cấp."); // Trả về giá trị của 'result' nếu khác null, ngược lại trả về 'ApiResult<SupplierDto>.Fail("Không tìm thấy nhà cung cấp.")'.
            }
            catch (Exception ex) // Khối bắt lỗi ngoại lệ Exception nếu xảy ra lỗi trong quá trình xử lý.
            {
                return ApiResult<SupplierDto>.Fail($"Lỗi kết nối: {ex.Message}"); // Trả về giá trị của biểu thức 'ApiResult<SupplierDto>.Fail($"Lỗi kết nối: {ex.Message}")'.
            }
        }

        public async Task<ApiResult<SupplierDto>> CreateAsync(CreateSupplierRequest request) // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'CreateAsync' nhận tham số (request) trả về kết quả kiểu Task<ApiResult<SupplierDto>>.
        {
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var response = await _httpClient.PostAsJsonAsync(BaseUrl, request); // Gọi phương thức POST (gửi dữ liệu JSON) bất đồng bộ tới URL 'BaseUrl' và gán kết quả cho biến 'response'.
                var result = await response.Content.ReadFromJsonAsync<ApiResult<SupplierDto>>(); // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'result'.
                return result ?? ApiResult<SupplierDto>.Fail("Không thể tạo nhà cung cấp."); // Trả về giá trị của 'result' nếu khác null, ngược lại trả về 'ApiResult<SupplierDto>.Fail("Không thể tạo nhà cung cấp.")'.
            }
            catch (Exception ex) // Khối bắt lỗi ngoại lệ Exception nếu xảy ra lỗi trong quá trình xử lý.
            {
                return ApiResult<SupplierDto>.Fail($"Lỗi kết nối: {ex.Message}"); // Trả về giá trị của biểu thức 'ApiResult<SupplierDto>.Fail($"Lỗi kết nối: {ex.Message}")'.
            }
        }

        public async Task<ApiResult<SupplierDto>> UpdateAsync(int id, UpdateSupplierRequest request) // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'UpdateAsync' nhận tham số (id, request) trả về kết quả kiểu Task<ApiResult<SupplierDto>>.
        {
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/{id}", request); // Gọi phương thức PUT (cập nhật dữ liệu JSON) bất đồng bộ tới URL '{BaseUrl}/{id}' và gán kết quả cho biến 'response'.
                var result = await response.Content.ReadFromJsonAsync<ApiResult<SupplierDto>>(); // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'result'.
                return result ?? ApiResult<SupplierDto>.Fail("Không thể cập nhật nhà cung cấp."); // Trả về giá trị của 'result' nếu khác null, ngược lại trả về 'ApiResult<SupplierDto>.Fail("Không thể cập nhật nhà cung cấp.")'.
            }
            catch (Exception ex) // Khối bắt lỗi ngoại lệ Exception nếu xảy ra lỗi trong quá trình xử lý.
            {
                return ApiResult<SupplierDto>.Fail($"Lỗi kết nối: {ex.Message}"); // Trả về giá trị của biểu thức 'ApiResult<SupplierDto>.Fail($"Lỗi kết nối: {ex.Message}")'.
            }
        }

        public async Task<ApiResult<bool>> DeleteAsync(int id) // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'DeleteAsync' nhận tham số (id) trả về kết quả kiểu Task<ApiResult<bool>>.
        {
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var response = await _httpClient.DeleteAsync($"{BaseUrl}/{id}"); // Gọi phương thức DELETE (xóa tài nguyên) bất đồng bộ tới URL '{BaseUrl}/{id}' và gán kết quả cho biến 'response'.
                var result = await response.Content.ReadFromJsonAsync<ApiResult<bool>>(); // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'result'.
                return result ?? ApiResult<bool>.Fail("Không thể xóa nhà cung cấp."); // Trả về giá trị của 'result' nếu khác null, ngược lại trả về 'ApiResult<bool>.Fail("Không thể xóa nhà cung cấp.")'.
            }
            catch (Exception ex) // Khối bắt lỗi ngoại lệ Exception nếu xảy ra lỗi trong quá trình xử lý.
            {
                return ApiResult<bool>.Fail($"Lỗi kết nối: {ex.Message}"); // Trả về giá trị của biểu thức 'ApiResult<bool>.Fail($"Lỗi kết nối: {ex.Message}")'.
            }
        }
    }
}

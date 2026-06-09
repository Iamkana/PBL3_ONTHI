using System.Net.Http.Json; // Sử dụng thư viện hỗ trợ gửi nhận dữ liệu JSON qua HTTP.
using PBL3.Shared.DTOs.Common; // Sử dụng các DTO của module Common thuộc tầng Shared.
using PBL3.Shared.DTOs.Employees; // Sử dụng các DTO của module Employees thuộc tầng Shared.
using System; // Nhập (import) namespace System để sử dụng các lớp bên trong.
using System.Collections.Generic; // Nhập (import) namespace System.Collections.Generic để sử dụng các lớp bên trong.
using System.Threading.Tasks; // Nhập (import) namespace System.Threading.Tasks để sử dụng các lớp bên trong.

namespace Client.Services.Employee // Thiết lập namespace Client.Services.Employee để tổ chức quản lý cấu trúc các lớp.
{
    public class EmployeeClientService : IEmployeeClientService // Định nghĩa lớp EmployeeClientService triển khai các dịch vụ hoặc kế thừa từ IEmployeeClientService.
    {
        private readonly HttpClient _httpClient; // Khai báo đối tượng HttpClient nội bộ dùng để thực hiện cuộc gọi API.
        private const string BaseUrl = "api/employees"; // Khai báo hằng số BaseUrl có giá trị là "api/employees".

        public EmployeeClientService(HttpClient httpClient) // Hàm khởi tạo (Constructor) của lớp EmployeeClientService tiêm các phụ thuộc: httpClient.
        {
            _httpClient = httpClient; // Thực hiện gán giá trị của biểu thức 'httpClient' cho biến/thuộc tính '_httpClient'.
        }

        public async Task<ApiResult<PagedResult<EmployeeListDto>>> GetListAsync(EmployeeFilterRequest request) // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'GetListAsync' nhận tham số (request) trả về kết quả kiểu Task<ApiResult<PagedResult<EmployeeListDto>>>.
        {
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var queryParams = new List<string>(); // Thực hiện gán giá trị của biểu thức 'new List<string>()' cho biến/thuộc tính 'queryParams'.

                if (!string.IsNullOrWhiteSpace(request.Keyword)) // Kiểm tra xem điều kiện '!string.IsNullOrWhiteSpace(request.Keyword' có thỏa mãn hay không.
                    queryParams.Add($"keyword={Uri.EscapeDataString(request.Keyword)}"); // Thực thi dòng lệnh nghiệp vụ.

                if (request.IsActive.HasValue) // Kiểm tra xem điều kiện 'request.IsActive.HasValue' có thỏa mãn hay không.
                    queryParams.Add($"isActive={request.IsActive.Value.ToString().ToLower()}"); // Thực thi dòng lệnh nghiệp vụ.

                if (request.Gender.HasValue) // Kiểm tra xem điều kiện 'request.Gender.HasValue' có thỏa mãn hay không.
                    queryParams.Add($"gender={request.Gender.Value}"); // Thực thi dòng lệnh nghiệp vụ.

                queryParams.Add($"pageNumber={request.PageNumber}"); // Thực thi dòng lệnh nghiệp vụ.
                queryParams.Add($"pageSize={request.PageSize}"); // Thực thi dòng lệnh nghiệp vụ.

                if (!string.IsNullOrWhiteSpace(request.SortBy)) // Kiểm tra xem điều kiện '!string.IsNullOrWhiteSpace(request.SortBy' có thỏa mãn hay không.
                    queryParams.Add($"sortBy={Uri.EscapeDataString(request.SortBy)}"); // Thực thi dòng lệnh nghiệp vụ.

                if (request.SortDescending) // Kiểm tra xem điều kiện 'request.SortDescending' có thỏa mãn hay không.
                    queryParams.Add("sortDescending=true"); // Thực thi dòng lệnh nghiệp vụ.

                var url = $"{BaseUrl}?{string.Join("&", queryParams)}"; // Thực hiện gán giá trị của biểu thức '$"{BaseUrl}?{string.Join("&", queryParams)}"' cho biến/thuộc tính 'url'.
                var result = await _httpClient.GetFromJsonAsync<ApiResult<PagedResult<EmployeeListDto>>>(url); // Gọi phương thức GET bất đồng bộ tới URL 'url' nhận kết quả kiểu ApiResult<PagedResult<EmployeeListDto>> và gán kết quả cho biến 'result'.
                return result ?? ApiResult<PagedResult<EmployeeListDto>>.Fail("Không thể tải danh sách nhân viên."); // Trả về giá trị của 'result' nếu khác null, ngược lại trả về 'ApiResult<PagedResult<EmployeeListDto>>.Fail("Không thể tải danh sách nhân viên.")'.
            }
            catch (Exception ex) // Khối bắt lỗi ngoại lệ Exception nếu xảy ra lỗi trong quá trình xử lý.
            {
                return ApiResult<PagedResult<EmployeeListDto>>.Fail($"Lỗi kết nối: {ex.Message}"); // Trả về giá trị của biểu thức 'ApiResult<PagedResult<EmployeeListDto>>.Fail($"Lỗi kết nối: {ex.Message}")'.
            }
        }

        public async Task<ApiResult<EmployeeListDto>> GetByIdAsync(Guid id) // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'GetByIdAsync' nhận tham số (id) trả về kết quả kiểu Task<ApiResult<EmployeeListDto>>.
        {
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var result = await _httpClient.GetFromJsonAsync<ApiResult<EmployeeListDto>>($"{BaseUrl}/{id}"); // Gọi phương thức GET bất đồng bộ tới URL '{BaseUrl}/{id}' nhận kết quả kiểu ApiResult<EmployeeListDto> và gán kết quả cho biến 'result'.
                return result ?? ApiResult<EmployeeListDto>.Fail("Không tìm thấy nhân viên."); // Trả về giá trị của 'result' nếu khác null, ngược lại trả về 'ApiResult<EmployeeListDto>.Fail("Không tìm thấy nhân viên.")'.
            }
            catch (Exception ex) // Khối bắt lỗi ngoại lệ Exception nếu xảy ra lỗi trong quá trình xử lý.
            {
                return ApiResult<EmployeeListDto>.Fail($"Lỗi kết nối: {ex.Message}"); // Trả về giá trị của biểu thức 'ApiResult<EmployeeListDto>.Fail($"Lỗi kết nối: {ex.Message}")'.
            }
        }

        public async Task<ApiResult<EmployeeListDto>> CreateAsync(CreateEmployeeRequest request) // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'CreateAsync' nhận tham số (request) trả về kết quả kiểu Task<ApiResult<EmployeeListDto>>.
        {
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var response = await _httpClient.PostAsJsonAsync(BaseUrl, request); // Gọi phương thức POST (gửi dữ liệu JSON) bất đồng bộ tới URL 'BaseUrl' và gán kết quả cho biến 'response'.
                var result = await response.Content.ReadFromJsonAsync<ApiResult<EmployeeListDto>>(); // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'result'.
                return result ?? ApiResult<EmployeeListDto>.Fail("Không thể tạo nhân viên."); // Trả về giá trị của 'result' nếu khác null, ngược lại trả về 'ApiResult<EmployeeListDto>.Fail("Không thể tạo nhân viên.")'.
            }
            catch (Exception ex) // Khối bắt lỗi ngoại lệ Exception nếu xảy ra lỗi trong quá trình xử lý.
            {
                return ApiResult<EmployeeListDto>.Fail($"Lỗi kết nối: {ex.Message}"); // Trả về giá trị của biểu thức 'ApiResult<EmployeeListDto>.Fail($"Lỗi kết nối: {ex.Message}")'.
            }
        }

        public async Task<ApiResult<EmployeeListDto>> UpdateAsync(Guid id, UpdateEmployeeRequest request) // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'UpdateAsync' nhận tham số (id, request) trả về kết quả kiểu Task<ApiResult<EmployeeListDto>>.
        {
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/{id}", request); // Gọi phương thức PUT (cập nhật dữ liệu JSON) bất đồng bộ tới URL '{BaseUrl}/{id}' và gán kết quả cho biến 'response'.
                var result = await response.Content.ReadFromJsonAsync<ApiResult<EmployeeListDto>>(); // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'result'.
                return result ?? ApiResult<EmployeeListDto>.Fail("Không thể cập nhật nhân viên."); // Trả về giá trị của 'result' nếu khác null, ngược lại trả về 'ApiResult<EmployeeListDto>.Fail("Không thể cập nhật nhân viên.")'.
            }
            catch (Exception ex) // Khối bắt lỗi ngoại lệ Exception nếu xảy ra lỗi trong quá trình xử lý.
            {
                return ApiResult<EmployeeListDto>.Fail($"Lỗi kết nối: {ex.Message}"); // Trả về giá trị của biểu thức 'ApiResult<EmployeeListDto>.Fail($"Lỗi kết nối: {ex.Message}")'.
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
                return result ?? ApiResult<bool>.Fail("Không thể khóa tài khoản nhân viên."); // Trả về giá trị của 'result' nếu khác null, ngược lại trả về 'ApiResult<bool>.Fail("Không thể khóa tài khoản nhân viên.")'.
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
                return result ?? ApiResult<bool>.Fail("Không thể mở khóa tài khoản nhân viên."); // Trả về giá trị của 'result' nếu khác null, ngược lại trả về 'ApiResult<bool>.Fail("Không thể mở khóa tài khoản nhân viên.")'.
            }
            catch (Exception ex) // Khối bắt lỗi ngoại lệ Exception nếu xảy ra lỗi trong quá trình xử lý.
            {
                return ApiResult<bool>.Fail($"Lỗi kết nối: {ex.Message}"); // Trả về giá trị của biểu thức 'ApiResult<bool>.Fail($"Lỗi kết nối: {ex.Message}")'.
            }
        }

        public async Task<ApiResult<List<EmployeeDto>>> GetTechniciansAsync() // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'GetTechniciansAsync' không tham số trả về kết quả kiểu Task<ApiResult<List<EmployeeDto>>>.
        {
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var result = await _httpClient.GetFromJsonAsync<ApiResult<List<EmployeeDto>>>($"{BaseUrl}/technicians"); // Gọi phương thức GET bất đồng bộ tới URL '{BaseUrl}/technicians' nhận kết quả kiểu ApiResult<List<EmployeeDto>> và gán kết quả cho biến 'result'.
                return result ?? ApiResult<List<EmployeeDto>>.Fail("Không thể tải danh sách kỹ thuật viên."); // Trả về giá trị của 'result' nếu khác null, ngược lại trả về 'ApiResult<List<EmployeeDto>>.Fail("Không thể tải danh sách kỹ thuật viên.")'.
            }
            catch (Exception ex) // Khối bắt lỗi ngoại lệ Exception nếu xảy ra lỗi trong quá trình xử lý.
            {
                return ApiResult<List<EmployeeDto>>.Fail($"Lỗi kết nối: {ex.Message}"); // Trả về giá trị của biểu thức 'ApiResult<List<EmployeeDto>>.Fail($"Lỗi kết nối: {ex.Message}")'.
            }
        }
    }
}

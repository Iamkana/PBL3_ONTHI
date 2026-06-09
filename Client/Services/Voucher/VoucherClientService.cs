using System.Net.Http.Json; // Sử dụng thư viện hỗ trợ gửi nhận dữ liệu JSON qua HTTP.
using PBL3.Shared.DTOs.Common; // Sử dụng các DTO của module Common thuộc tầng Shared.
using PBL3.Shared.DTOs.Vouchers; // Sử dụng các DTO của module Vouchers thuộc tầng Shared.

namespace Client.Services.Voucher // Thiết lập namespace Client.Services.Voucher để tổ chức quản lý cấu trúc các lớp.
{
    public class VoucherClientService : IVoucherClientService // Định nghĩa lớp VoucherClientService triển khai các dịch vụ hoặc kế thừa từ IVoucherClientService.
    {
        private readonly HttpClient _httpClient; // Khai báo đối tượng HttpClient nội bộ dùng để thực hiện cuộc gọi API.
        private const string BaseUrl = "api/vouchers"; // Khai báo hằng số BaseUrl có giá trị là "api/vouchers".

        public VoucherClientService(HttpClient httpClient) // Hàm khởi tạo (Constructor) của lớp VoucherClientService tiêm các phụ thuộc: httpClient.
        {
            _httpClient = httpClient; // Thực hiện gán giá trị của biểu thức 'httpClient' cho biến/thuộc tính '_httpClient'.
        }

        public async Task<ApiResult<PagedResult<VoucherDto>>> GetListAsync(VoucherFilterRequest request) // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'GetListAsync' nhận tham số (request) trả về kết quả kiểu Task<ApiResult<PagedResult<VoucherDto>>>.
        {
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var queryParams = new List<string>(); // Thực hiện gán giá trị của biểu thức 'new List<string>()' cho biến/thuộc tính 'queryParams'.

                if (!string.IsNullOrWhiteSpace(request.Keyword)) // Kiểm tra xem điều kiện '!string.IsNullOrWhiteSpace(request.Keyword' có thỏa mãn hay không.
                    queryParams.Add($"Keyword={Uri.EscapeDataString(request.Keyword)}"); // Thực thi dòng lệnh nghiệp vụ.
                if (!string.IsNullOrWhiteSpace(request.StatusFilter)) // Kiểm tra xem điều kiện '!string.IsNullOrWhiteSpace(request.StatusFilter' có thỏa mãn hay không.
                    queryParams.Add($"StatusFilter={Uri.EscapeDataString(request.StatusFilter)}"); // Thực thi dòng lệnh nghiệp vụ.
                if (request.FromDate.HasValue) // Kiểm tra xem điều kiện 'request.FromDate.HasValue' có thỏa mãn hay không.
                    queryParams.Add($"FromDate={Uri.EscapeDataString(request.FromDate.Value.ToString("o"))}"); // Thực thi dòng lệnh nghiệp vụ.
                if (request.ToDate.HasValue) // Kiểm tra xem điều kiện 'request.ToDate.HasValue' có thỏa mãn hay không.
                    queryParams.Add($"ToDate={Uri.EscapeDataString(request.ToDate.Value.ToString("o"))}"); // Thực thi dòng lệnh nghiệp vụ.

                queryParams.Add($"PageNumber={request.PageNumber}"); // Thực thi dòng lệnh nghiệp vụ.
                queryParams.Add($"PageSize={request.PageSize}"); // Thực thi dòng lệnh nghiệp vụ.

                if (!string.IsNullOrWhiteSpace(request.SortBy)) // Kiểm tra xem điều kiện '!string.IsNullOrWhiteSpace(request.SortBy' có thỏa mãn hay không.
                    queryParams.Add($"SortBy={Uri.EscapeDataString(request.SortBy)}"); // Thực thi dòng lệnh nghiệp vụ.
                if (request.SortDescending) // Kiểm tra xem điều kiện 'request.SortDescending' có thỏa mãn hay không.
                    queryParams.Add("SortDescending=true"); // Thực thi dòng lệnh nghiệp vụ.

                var url = $"{BaseUrl}?{string.Join("&", queryParams)}"; // Thực hiện gán giá trị của biểu thức '$"{BaseUrl}?{string.Join("&", queryParams)}"' cho biến/thuộc tính 'url'.
                var result = await _httpClient.GetFromJsonAsync<ApiResult<PagedResult<VoucherDto>>>(url); // Gọi phương thức GET bất đồng bộ tới URL 'url' nhận kết quả kiểu ApiResult<PagedResult<VoucherDto>> và gán kết quả cho biến 'result'.
                return result ?? ApiResult<PagedResult<VoucherDto>>.Fail("Không thể tải danh sách voucher."); // Trả về giá trị của 'result' nếu khác null, ngược lại trả về 'ApiResult<PagedResult<VoucherDto>>.Fail("Không thể tải danh sách voucher.")'.
            }
            catch (Exception ex) // Khối bắt lỗi ngoại lệ Exception nếu xảy ra lỗi trong quá trình xử lý.
            {
                return ApiResult<PagedResult<VoucherDto>>.Fail($"Lỗi kết nối: {ex.Message}"); // Trả về giá trị của biểu thức 'ApiResult<PagedResult<VoucherDto>>.Fail($"Lỗi kết nối: {ex.Message}")'.
            }
        }

        public async Task<ApiResult<VoucherDto>> GetByIdAsync(int id) // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'GetByIdAsync' nhận tham số (id) trả về kết quả kiểu Task<ApiResult<VoucherDto>>.
        {
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var result = await _httpClient.GetFromJsonAsync<ApiResult<VoucherDto>>($"{BaseUrl}/{id}"); // Gọi phương thức GET bất đồng bộ tới URL '{BaseUrl}/{id}' nhận kết quả kiểu ApiResult<VoucherDto> và gán kết quả cho biến 'result'.
                return result ?? ApiResult<VoucherDto>.Fail("Không tìm thấy voucher."); // Trả về giá trị của 'result' nếu khác null, ngược lại trả về 'ApiResult<VoucherDto>.Fail("Không tìm thấy voucher.")'.
            }
            catch (Exception ex) // Khối bắt lỗi ngoại lệ Exception nếu xảy ra lỗi trong quá trình xử lý.
            {
                return ApiResult<VoucherDto>.Fail($"Lỗi kết nối: {ex.Message}"); // Trả về giá trị của biểu thức 'ApiResult<VoucherDto>.Fail($"Lỗi kết nối: {ex.Message}")'.
            }
        }

        public async Task<ApiResult<VoucherDto>> CreateAsync(CreateVoucherRequest request) // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'CreateAsync' nhận tham số (request) trả về kết quả kiểu Task<ApiResult<VoucherDto>>.
        {
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var response = await _httpClient.PostAsJsonAsync(BaseUrl, request); // Gọi phương thức POST (gửi dữ liệu JSON) bất đồng bộ tới URL 'BaseUrl' và gán kết quả cho biến 'response'.
                var result = await response.Content.ReadFromJsonAsync<ApiResult<VoucherDto>>(); // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'result'.
                return result ?? ApiResult<VoucherDto>.Fail("Không thể tạo voucher."); // Trả về giá trị của 'result' nếu khác null, ngược lại trả về 'ApiResult<VoucherDto>.Fail("Không thể tạo voucher.")'.
            }
            catch (Exception ex) // Khối bắt lỗi ngoại lệ Exception nếu xảy ra lỗi trong quá trình xử lý.
            {
                return ApiResult<VoucherDto>.Fail($"Lỗi kết nối: {ex.Message}"); // Trả về giá trị của biểu thức 'ApiResult<VoucherDto>.Fail($"Lỗi kết nối: {ex.Message}")'.
            }
        }

        public async Task<ApiResult<VoucherDto>> UpdateAsync(int id, UpdateVoucherRequest request) // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'UpdateAsync' nhận tham số (id, request) trả về kết quả kiểu Task<ApiResult<VoucherDto>>.
        {
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/{id}", request); // Gọi phương thức PUT (cập nhật dữ liệu JSON) bất đồng bộ tới URL '{BaseUrl}/{id}' và gán kết quả cho biến 'response'.
                var result = await response.Content.ReadFromJsonAsync<ApiResult<VoucherDto>>(); // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'result'.
                return result ?? ApiResult<VoucherDto>.Fail("Không thể cập nhật voucher."); // Trả về giá trị của 'result' nếu khác null, ngược lại trả về 'ApiResult<VoucherDto>.Fail("Không thể cập nhật voucher.")'.
            }
            catch (Exception ex) // Khối bắt lỗi ngoại lệ Exception nếu xảy ra lỗi trong quá trình xử lý.
            {
                return ApiResult<VoucherDto>.Fail($"Lỗi kết nối: {ex.Message}"); // Trả về giá trị của biểu thức 'ApiResult<VoucherDto>.Fail($"Lỗi kết nối: {ex.Message}")'.
            }
        }

        public async Task<ApiResult<bool>> DeleteAsync(int id) // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'DeleteAsync' nhận tham số (id) trả về kết quả kiểu Task<ApiResult<bool>>.
        {
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var response = await _httpClient.DeleteAsync($"{BaseUrl}/{id}"); // Gọi phương thức DELETE (xóa tài nguyên) bất đồng bộ tới URL '{BaseUrl}/{id}' và gán kết quả cho biến 'response'.
                var result = await response.Content.ReadFromJsonAsync<ApiResult<bool>>(); // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'result'.
                return result ?? ApiResult<bool>.Fail("Không thể xóa voucher."); // Trả về giá trị của 'result' nếu khác null, ngược lại trả về 'ApiResult<bool>.Fail("Không thể xóa voucher.")'.
            }
            catch (Exception ex) // Khối bắt lỗi ngoại lệ Exception nếu xảy ra lỗi trong quá trình xử lý.
            {
                return ApiResult<bool>.Fail($"Lỗi kết nối: {ex.Message}"); // Trả về giá trị của biểu thức 'ApiResult<bool>.Fail($"Lỗi kết nối: {ex.Message}")'.
            }
        }

        public async Task<ApiResult<VoucherDto>> ToggleStatusAsync(int id) // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'ToggleStatusAsync' nhận tham số (id) trả về kết quả kiểu Task<ApiResult<VoucherDto>>.
        {
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var response = await _httpClient.PatchAsync($"{BaseUrl}/{id}/toggle-status", null); // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'response'.
                if (!response.IsSuccessStatusCode) // Kiểm tra xem điều kiện '!response.IsSuccessStatusCode' có thỏa mãn hay không.
                    return ApiResult<VoucherDto>.Fail($"Lỗi HTTP {(int)response.StatusCode}."); // Trả về giá trị của biểu thức 'ApiResult<VoucherDto>.Fail($"Lỗi HTTP {(int)response.StatusCode}.")'.
                var result = await response.Content.ReadFromJsonAsync<ApiResult<VoucherDto>>(); // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'result'.
                return result ?? ApiResult<VoucherDto>.Fail("Không thể thay đổi trạng thái voucher."); // Trả về giá trị của 'result' nếu khác null, ngược lại trả về 'ApiResult<VoucherDto>.Fail("Không thể thay đổi trạng thái voucher.")'.
            }
            catch (Exception ex) // Khối bắt lỗi ngoại lệ Exception nếu xảy ra lỗi trong quá trình xử lý.
            {
                return ApiResult<VoucherDto>.Fail($"Lỗi kết nối: {ex.Message}"); // Trả về giá trị của biểu thức 'ApiResult<VoucherDto>.Fail($"Lỗi kết nối: {ex.Message}")'.
            }
        }

        public async Task<ApiResult<List<VoucherAvailabilityDto>>> GetAvailableForOrderAsync( // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'GetAvailableForOrderAsync' không tham số trả về kết quả kiểu Task<ApiResult<List<VoucherAvailabilityDto>>>.
            GetAvailableVouchersRequest request) // Thực thi dòng lệnh nghiệp vụ.
        {
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/available-for-order", request); // Gọi phương thức POST (gửi dữ liệu JSON) bất đồng bộ tới URL '{BaseUrl}/available-for-order' và gán kết quả cho biến 'response'.
                var result = await response.Content.ReadFromJsonAsync<ApiResult<List<VoucherAvailabilityDto>>>(); // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'result'.
                return result ?? ApiResult<List<VoucherAvailabilityDto>>.Fail("Không thể tải danh sách voucher."); // Trả về giá trị của 'result' nếu khác null, ngược lại trả về 'ApiResult<List<VoucherAvailabilityDto>>.Fail("Không thể tải danh sách voucher.")'.
            }
            catch (Exception ex) // Khối bắt lỗi ngoại lệ Exception nếu xảy ra lỗi trong quá trình xử lý.
            {
                return ApiResult<List<VoucherAvailabilityDto>>.Fail($"Lỗi kết nối: {ex.Message}"); // Trả về giá trị của biểu thức 'ApiResult<List<VoucherAvailabilityDto>>.Fail($"Lỗi kết nối: {ex.Message}")'.
            }
        }
    }
}

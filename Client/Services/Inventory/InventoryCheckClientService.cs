using System.Net.Http.Json; // Sử dụng thư viện hỗ trợ gửi nhận dữ liệu JSON qua HTTP.
using PBL3.Shared.DTOs.Common; // Sử dụng các DTO của module Common thuộc tầng Shared.
using PBL3.Shared.DTOs.Inventory; // Sử dụng các DTO của module Inventory thuộc tầng Shared.

namespace Client.Services.Inventory // Thiết lập namespace Client.Services.Inventory để tổ chức quản lý cấu trúc các lớp.
{
    public class InventoryCheckClientService : IInventoryCheckClientService // Định nghĩa lớp InventoryCheckClientService triển khai các dịch vụ hoặc kế thừa từ IInventoryCheckClientService.
    {
        private readonly HttpClient _httpClient; // Khai báo đối tượng HttpClient nội bộ dùng để thực hiện cuộc gọi API.
        private const string BaseUrl = "api/inventory-checks"; // Khai báo hằng số BaseUrl có giá trị là "api/inventory-checks".

        public InventoryCheckClientService(HttpClient httpClient) // Hàm khởi tạo (Constructor) của lớp InventoryCheckClientService tiêm các phụ thuộc: httpClient.
        {
            _httpClient = httpClient; // Thực hiện gán giá trị của biểu thức 'httpClient' cho biến/thuộc tính '_httpClient'.
        }

        public async Task<ApiResult<InventoryCheckDto>> CreateAsync(CreateInventoryCheckRequest request) // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'CreateAsync' nhận tham số (request) trả về kết quả kiểu Task<ApiResult<InventoryCheckDto>>.
        {
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var response = await _httpClient.PostAsJsonAsync(BaseUrl, request); // Gọi phương thức POST (gửi dữ liệu JSON) bất đồng bộ tới URL 'BaseUrl' và gán kết quả cho biến 'response'.
                var result = await response.Content.ReadFromJsonAsync<ApiResult<InventoryCheckDto>>(); // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'result'.
                return result ?? ApiResult<InventoryCheckDto>.Fail("Không thể tạo phiếu kiểm kê."); // Trả về giá trị của 'result' nếu khác null, ngược lại trả về 'ApiResult<InventoryCheckDto>.Fail("Không thể tạo phiếu kiểm kê.")'.
            }
            catch (Exception ex) // Khối bắt lỗi ngoại lệ Exception nếu xảy ra lỗi trong quá trình xử lý.
            {
                return ApiResult<InventoryCheckDto>.Fail($"Lỗi kết nối: {ex.Message}"); // Trả về giá trị của biểu thức 'ApiResult<InventoryCheckDto>.Fail($"Lỗi kết nối: {ex.Message}")'.
            }
        }

        public async Task<ApiResult<PagedResult<InventoryCheckListItemDto>>> GetListAsync( // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'GetListAsync' không tham số trả về kết quả kiểu Task<ApiResult<PagedResult<InventoryCheckListItemDto>>>.
            InventoryCheckFilterRequest filter, CancellationToken cancellationToken = default) // Thực hiện gán giá trị của biểu thức 'default)' cho biến/thuộc tính 'InventoryCheckFilterRequest filter, CancellationToken cancellationToken'.
        {
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var q = new List<string>(); // Thực hiện gán giá trị của biểu thức 'new List<string>()' cho biến/thuộc tính 'q'.
                if (!string.IsNullOrWhiteSpace(filter.Keyword)) // Kiểm tra xem điều kiện '!string.IsNullOrWhiteSpace(filter.Keyword' có thỏa mãn hay không.
                    q.Add($"Keyword={Uri.EscapeDataString(filter.Keyword)}"); // Thực thi dòng lệnh nghiệp vụ.
                if (filter.Status.HasValue) // Kiểm tra xem điều kiện 'filter.Status.HasValue' có thỏa mãn hay không.
                    q.Add($"Status={filter.Status.Value}"); // Thực thi dòng lệnh nghiệp vụ.
                if (filter.FromDate.HasValue) // Kiểm tra xem điều kiện 'filter.FromDate.HasValue' có thỏa mãn hay không.
                    q.Add($"FromDate={filter.FromDate.Value:yyyy-MM-dd}"); // Thực thi dòng lệnh nghiệp vụ.
                if (filter.ToDate.HasValue) // Kiểm tra xem điều kiện 'filter.ToDate.HasValue' có thỏa mãn hay không.
                    q.Add($"ToDate={filter.ToDate.Value:yyyy-MM-dd}"); // Thực thi dòng lệnh nghiệp vụ.
                if (filter.EmployeeId.HasValue) // Kiểm tra xem điều kiện 'filter.EmployeeId.HasValue' có thỏa mãn hay không.
                    q.Add($"EmployeeId={filter.EmployeeId.Value}"); // Thực thi dòng lệnh nghiệp vụ.
                q.Add($"PageNumber={filter.PageNumber}"); // Thực thi dòng lệnh nghiệp vụ.
                q.Add($"PageSize={filter.PageSize}"); // Thực thi dòng lệnh nghiệp vụ.
                if (!string.IsNullOrWhiteSpace(filter.SortBy)) // Kiểm tra xem điều kiện '!string.IsNullOrWhiteSpace(filter.SortBy' có thỏa mãn hay không.
                    q.Add($"SortBy={Uri.EscapeDataString(filter.SortBy)}"); // Thực thi dòng lệnh nghiệp vụ.
                if (filter.SortDescending) // Kiểm tra xem điều kiện 'filter.SortDescending' có thỏa mãn hay không.
                    q.Add("SortDescending=true"); // Thực thi dòng lệnh nghiệp vụ.

                var url = $"{BaseUrl}?{string.Join("&", q)}"; // Thực hiện gán giá trị của biểu thức '$"{BaseUrl}?{string.Join("&", q)}"' cho biến/thuộc tính 'url'.
                var result = await _httpClient.GetFromJsonAsync<ApiResult<PagedResult<InventoryCheckListItemDto>>>(url, cancellationToken); // Gọi phương thức GET bất đồng bộ tới URL 'url' nhận kết quả kiểu ApiResult<PagedResult<InventoryCheckListItemDto>> và gán kết quả cho biến 'result'.
                return result ?? ApiResult<PagedResult<InventoryCheckListItemDto>>.Fail("Không thể tải danh sách phiếu kiểm kê."); // Trả về giá trị của 'result' nếu khác null, ngược lại trả về 'ApiResult<PagedResult<InventoryCheckListItemDto>>.Fail("Không thể tải danh sách phiếu kiểm kê.")'.
            }
            catch (OperationCanceledException) // Thực thi dòng lệnh nghiệp vụ.
            {
                return ApiResult<PagedResult<InventoryCheckListItemDto>>.Fail(string.Empty); // Trả về giá trị của biểu thức 'ApiResult<PagedResult<InventoryCheckListItemDto>>.Fail(string.Empty)'.
            }
            catch (Exception ex) // Khối bắt lỗi ngoại lệ Exception nếu xảy ra lỗi trong quá trình xử lý.
            {
                return ApiResult<PagedResult<InventoryCheckListItemDto>>.Fail($"Lỗi kết nối: {ex.Message}"); // Trả về giá trị của biểu thức 'ApiResult<PagedResult<InventoryCheckListItemDto>>.Fail($"Lỗi kết nối: {ex.Message}")'.
            }
        }

        public async Task<ApiResult<InventoryCheckDto>> GetByIdAsync(int id) // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'GetByIdAsync' nhận tham số (id) trả về kết quả kiểu Task<ApiResult<InventoryCheckDto>>.
        {
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var result = await _httpClient.GetFromJsonAsync<ApiResult<InventoryCheckDto>>($"{BaseUrl}/{id}"); // Gọi phương thức GET bất đồng bộ tới URL '{BaseUrl}/{id}' nhận kết quả kiểu ApiResult<InventoryCheckDto> và gán kết quả cho biến 'result'.
                return result ?? ApiResult<InventoryCheckDto>.Fail("Không tìm thấy phiếu kiểm kê."); // Trả về giá trị của 'result' nếu khác null, ngược lại trả về 'ApiResult<InventoryCheckDto>.Fail("Không tìm thấy phiếu kiểm kê.")'.
            }
            catch (Exception ex) // Khối bắt lỗi ngoại lệ Exception nếu xảy ra lỗi trong quá trình xử lý.
            {
                return ApiResult<InventoryCheckDto>.Fail($"Lỗi kết nối: {ex.Message}"); // Trả về giá trị của biểu thức 'ApiResult<InventoryCheckDto>.Fail($"Lỗi kết nối: {ex.Message}")'.
            }
        }

        public async Task<ApiResult<InventoryCheckDashboardDto>> GetDashboardAsync(int id) // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'GetDashboardAsync' nhận tham số (id) trả về kết quả kiểu Task<ApiResult<InventoryCheckDashboardDto>>.
        {
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var result = await _httpClient.GetFromJsonAsync<ApiResult<InventoryCheckDashboardDto>>($"{BaseUrl}/{id}/dashboard"); // Gọi phương thức GET bất đồng bộ tới URL '{BaseUrl}/{id}/dashboard' nhận kết quả kiểu ApiResult<InventoryCheckDashboardDto> và gán kết quả cho biến 'result'.
                return result ?? ApiResult<InventoryCheckDashboardDto>.Fail("Không tải được dashboard."); // Trả về giá trị của 'result' nếu khác null, ngược lại trả về 'ApiResult<InventoryCheckDashboardDto>.Fail("Không tải được dashboard.")'.
            }
            catch (Exception ex) // Khối bắt lỗi ngoại lệ Exception nếu xảy ra lỗi trong quá trình xử lý.
            {
                return ApiResult<InventoryCheckDashboardDto>.Fail($"Lỗi kết nối: {ex.Message}"); // Trả về giá trị của biểu thức 'ApiResult<InventoryCheckDashboardDto>.Fail($"Lỗi kết nối: {ex.Message}")'.
            }
        }

        public async Task<ApiResult<PagedResult<InventoryCheckSerialDto>>> GetSerialsAsync( // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'GetSerialsAsync' không tham số trả về kết quả kiểu Task<ApiResult<PagedResult<InventoryCheckSerialDto>>>.
            int checkId, InventoryCheckSerialFilterRequest filter, CancellationToken cancellationToken = default) // Thực hiện gán giá trị của biểu thức 'default)' cho biến/thuộc tính 'int checkId, InventoryCheckSerialFilterRequest filter, CancellationToken cancellationToken'.
        {
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var q = new List<string>(); // Thực hiện gán giá trị của biểu thức 'new List<string>()' cho biến/thuộc tính 'q'.
                if (filter.ScanStatus.HasValue) // Kiểm tra xem điều kiện 'filter.ScanStatus.HasValue' có thỏa mãn hay không.
                    q.Add($"ScanStatus={filter.ScanStatus.Value}"); // Thực thi dòng lệnh nghiệp vụ.
                if (filter.VariantId.HasValue) // Kiểm tra xem điều kiện 'filter.VariantId.HasValue' có thỏa mãn hay không.
                    q.Add($"VariantId={filter.VariantId.Value}"); // Thực thi dòng lệnh nghiệp vụ.
                q.Add($"PageNumber={filter.PageNumber}"); // Thực thi dòng lệnh nghiệp vụ.
                q.Add($"PageSize={filter.PageSize}"); // Thực thi dòng lệnh nghiệp vụ.

                var url = $"{BaseUrl}/{checkId}/serials?{string.Join("&", q)}"; // Thực hiện gán giá trị của biểu thức '$"{BaseUrl}/{checkId}/serials?{string.Join("&", q)}"' cho biến/thuộc tính 'url'.
                var result = await _httpClient.GetFromJsonAsync<ApiResult<PagedResult<InventoryCheckSerialDto>>>(url, cancellationToken); // Gọi phương thức GET bất đồng bộ tới URL 'url' nhận kết quả kiểu ApiResult<PagedResult<InventoryCheckSerialDto>> và gán kết quả cho biến 'result'.
                return result ?? ApiResult<PagedResult<InventoryCheckSerialDto>>.Fail("Không thể tải danh sách serial."); // Trả về giá trị của 'result' nếu khác null, ngược lại trả về 'ApiResult<PagedResult<InventoryCheckSerialDto>>.Fail("Không thể tải danh sách serial.")'.
            }
            catch (OperationCanceledException) // Thực thi dòng lệnh nghiệp vụ.
            {
                return ApiResult<PagedResult<InventoryCheckSerialDto>>.Fail(string.Empty); // Trả về giá trị của biểu thức 'ApiResult<PagedResult<InventoryCheckSerialDto>>.Fail(string.Empty)'.
            }
            catch (Exception ex) // Khối bắt lỗi ngoại lệ Exception nếu xảy ra lỗi trong quá trình xử lý.
            {
                return ApiResult<PagedResult<InventoryCheckSerialDto>>.Fail($"Lỗi kết nối: {ex.Message}"); // Trả về giá trị của biểu thức 'ApiResult<PagedResult<InventoryCheckSerialDto>>.Fail($"Lỗi kết nối: {ex.Message}")'.
            }
        }

        public async Task<ApiResult<ScanResultDto>> ScanSerialAsync(int checkId, ScanSerialRequest request) // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'ScanSerialAsync' nhận tham số (checkId, request) trả về kết quả kiểu Task<ApiResult<ScanResultDto>>.
        {
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/{checkId}/scan", request); // Gọi phương thức POST (gửi dữ liệu JSON) bất đồng bộ tới URL '{BaseUrl}/{checkId}/scan' và gán kết quả cho biến 'response'.
                var result = await response.Content.ReadFromJsonAsync<ApiResult<ScanResultDto>>(); // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'result'.
                return result ?? ApiResult<ScanResultDto>.Fail("Không nhận được kết quả quét."); // Trả về giá trị của 'result' nếu khác null, ngược lại trả về 'ApiResult<ScanResultDto>.Fail("Không nhận được kết quả quét.")'.
            }
            catch (Exception ex) // Khối bắt lỗi ngoại lệ Exception nếu xảy ra lỗi trong quá trình xử lý.
            {
                return ApiResult<ScanResultDto>.Fail($"Lỗi kết nối: {ex.Message}"); // Trả về giá trị của biểu thức 'ApiResult<ScanResultDto>.Fail($"Lỗi kết nối: {ex.Message}")'.
            }
        }

        public async Task<ApiResult<bool>> MarkDefectiveAsync(int checkId, int detailSerialId) // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'MarkDefectiveAsync' nhận tham số (checkId, detailSerialId) trả về kết quả kiểu Task<ApiResult<bool>>.
        {
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/{checkId}/serials/{detailSerialId}/mark-defective", new { }); // Gọi phương thức PUT (cập nhật dữ liệu JSON) bất đồng bộ tới URL '{BaseUrl}/{checkId}/serials/{detailSerialId}/mark-defective' và gán kết quả cho biến 'response'.
                var result = await response.Content.ReadFromJsonAsync<ApiResult<bool>>(); // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'result'.
                return result ?? ApiResult<bool>.Fail("Không thể đánh dấu hàng lỗi."); // Trả về giá trị của 'result' nếu khác null, ngược lại trả về 'ApiResult<bool>.Fail("Không thể đánh dấu hàng lỗi.")'.
            }
            catch (Exception ex) // Khối bắt lỗi ngoại lệ Exception nếu xảy ra lỗi trong quá trình xử lý.
            {
                return ApiResult<bool>.Fail($"Lỗi kết nối: {ex.Message}"); // Trả về giá trị của biểu thức 'ApiResult<bool>.Fail($"Lỗi kết nối: {ex.Message}")'.
            }
        }

        public async Task<ApiResult<bool>> UpdateReasonAsync(int checkId, int detailSerialId, UpdateScanReasonRequest request) // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'UpdateReasonAsync' nhận tham số (checkId, detailSerialId, request) trả về kết quả kiểu Task<ApiResult<bool>>.
        {
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/{checkId}/serials/{detailSerialId}/reason", request); // Gọi phương thức PUT (cập nhật dữ liệu JSON) bất đồng bộ tới URL '{BaseUrl}/{checkId}/serials/{detailSerialId}/reason' và gán kết quả cho biến 'response'.
                var result = await response.Content.ReadFromJsonAsync<ApiResult<bool>>(); // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'result'.
                return result ?? ApiResult<bool>.Fail("Không thể cập nhật lý do."); // Trả về giá trị của 'result' nếu khác null, ngược lại trả về 'ApiResult<bool>.Fail("Không thể cập nhật lý do.")'.
            }
            catch (Exception ex) // Khối bắt lỗi ngoại lệ Exception nếu xảy ra lỗi trong quá trình xử lý.
            {
                return ApiResult<bool>.Fail($"Lỗi kết nối: {ex.Message}"); // Trả về giá trị của biểu thức 'ApiResult<bool>.Fail($"Lỗi kết nối: {ex.Message}")'.
            }
        }

        public async Task<ApiResult<bool>> SubmitAsync(int checkId) // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'SubmitAsync' nhận tham số (checkId) trả về kết quả kiểu Task<ApiResult<bool>>.
        {
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/{checkId}/submit", new { }); // Gọi phương thức POST (gửi dữ liệu JSON) bất đồng bộ tới URL '{BaseUrl}/{checkId}/submit' và gán kết quả cho biến 'response'.
                var result = await response.Content.ReadFromJsonAsync<ApiResult<bool>>(); // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'result'.
                return result ?? ApiResult<bool>.Fail("Không thể gửi duyệt phiếu."); // Trả về giá trị của 'result' nếu khác null, ngược lại trả về 'ApiResult<bool>.Fail("Không thể gửi duyệt phiếu.")'.
            }
            catch (Exception ex) // Khối bắt lỗi ngoại lệ Exception nếu xảy ra lỗi trong quá trình xử lý.
            {
                return ApiResult<bool>.Fail($"Lỗi kết nối: {ex.Message}"); // Trả về giá trị của biểu thức 'ApiResult<bool>.Fail($"Lỗi kết nối: {ex.Message}")'.
            }
        }

        public async Task<ApiResult<bool>> ApproveAsync(int checkId) // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'ApproveAsync' nhận tham số (checkId) trả về kết quả kiểu Task<ApiResult<bool>>.
        {
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/{checkId}/approve", new { }); // Gọi phương thức POST (gửi dữ liệu JSON) bất đồng bộ tới URL '{BaseUrl}/{checkId}/approve' và gán kết quả cho biến 'response'.
                var result = await response.Content.ReadFromJsonAsync<ApiResult<bool>>(); // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'result'.
                return result ?? ApiResult<bool>.Fail("Không thể phê duyệt phiếu."); // Trả về giá trị của 'result' nếu khác null, ngược lại trả về 'ApiResult<bool>.Fail("Không thể phê duyệt phiếu.")'.
            }
            catch (Exception ex) // Khối bắt lỗi ngoại lệ Exception nếu xảy ra lỗi trong quá trình xử lý.
            {
                return ApiResult<bool>.Fail($"Lỗi kết nối: {ex.Message}"); // Trả về giá trị của biểu thức 'ApiResult<bool>.Fail($"Lỗi kết nối: {ex.Message}")'.
            }
        }

        public async Task<ApiResult<bool>> RejectAsync(int checkId, RejectInventoryCheckRequest request) // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'RejectAsync' nhận tham số (checkId, request) trả về kết quả kiểu Task<ApiResult<bool>>.
        {
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/{checkId}/reject", request); // Gọi phương thức POST (gửi dữ liệu JSON) bất đồng bộ tới URL '{BaseUrl}/{checkId}/reject' và gán kết quả cho biến 'response'.
                var result = await response.Content.ReadFromJsonAsync<ApiResult<bool>>(); // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'result'.
                return result ?? ApiResult<bool>.Fail("Không thể từ chối phiếu."); // Trả về giá trị của 'result' nếu khác null, ngược lại trả về 'ApiResult<bool>.Fail("Không thể từ chối phiếu.")'.
            }
            catch (Exception ex) // Khối bắt lỗi ngoại lệ Exception nếu xảy ra lỗi trong quá trình xử lý.
            {
                return ApiResult<bool>.Fail($"Lỗi kết nối: {ex.Message}"); // Trả về giá trị của biểu thức 'ApiResult<bool>.Fail($"Lỗi kết nối: {ex.Message}")'.
            }
        }

        public async Task<ApiResult<bool>> CancelAsync(int checkId) // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'CancelAsync' nhận tham số (checkId) trả về kết quả kiểu Task<ApiResult<bool>>.
        {
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/{checkId}/cancel", new { }); // Gọi phương thức POST (gửi dữ liệu JSON) bất đồng bộ tới URL '{BaseUrl}/{checkId}/cancel' và gán kết quả cho biến 'response'.
                var result = await response.Content.ReadFromJsonAsync<ApiResult<bool>>(); // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'result'.
                return result ?? ApiResult<bool>.Fail("Không thể hủy phiếu."); // Trả về giá trị của 'result' nếu khác null, ngược lại trả về 'ApiResult<bool>.Fail("Không thể hủy phiếu.")'.
            }
            catch (Exception ex) // Khối bắt lỗi ngoại lệ Exception nếu xảy ra lỗi trong quá trình xử lý.
            {
                return ApiResult<bool>.Fail($"Lỗi kết nối: {ex.Message}"); // Trả về giá trị của biểu thức 'ApiResult<bool>.Fail($"Lỗi kết nối: {ex.Message}")'.
            }
        }
    }
}

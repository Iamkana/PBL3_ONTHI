using System.Net.Http.Json; // Sử dụng thư viện hỗ trợ gửi nhận dữ liệu JSON qua HTTP.
using PBL3.Shared.DTOs.Common; // Sử dụng các DTO của module Common thuộc tầng Shared.
using PBL3.Shared.DTOs.Inventory; // Sử dụng các DTO của module Inventory thuộc tầng Shared.
using PBL3.Shared.DTOs.Products; // Sử dụng các DTO của module Products thuộc tầng Shared.

namespace Client.Services.Inventory // Thiết lập namespace Client.Services.Inventory để tổ chức quản lý cấu trúc các lớp.
{
    public class ImportReceiptClientService : IImportReceiptClientService // Định nghĩa lớp ImportReceiptClientService triển khai các dịch vụ hoặc kế thừa từ IImportReceiptClientService.
    {
        private readonly HttpClient _httpClient; // Khai báo đối tượng HttpClient nội bộ dùng để thực hiện cuộc gọi API.
        private const string BaseUrl = "api/import-receipts"; // Khai báo hằng số BaseUrl có giá trị là "api/import-receipts".

        public ImportReceiptClientService(HttpClient httpClient) // Hàm khởi tạo (Constructor) của lớp ImportReceiptClientService tiêm các phụ thuộc: httpClient.
        {
            _httpClient = httpClient; // Thực hiện gán giá trị của biểu thức 'httpClient' cho biến/thuộc tính '_httpClient'.
        }

        public async Task<ApiResult<ImportReceiptDto>> CreateAsync(CreateImportReceiptRequest request) // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'CreateAsync' nhận tham số (request) trả về kết quả kiểu Task<ApiResult<ImportReceiptDto>>.
        {
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var response = await _httpClient.PostAsJsonAsync(BaseUrl, request); // Gọi phương thức POST (gửi dữ liệu JSON) bất đồng bộ tới URL 'BaseUrl' và gán kết quả cho biến 'response'.
                var result = await response.Content.ReadFromJsonAsync<ApiResult<ImportReceiptDto>>(); // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'result'.
                return result ?? ApiResult<ImportReceiptDto>.Fail("Không thể tạo phiếu nhập kho."); // Trả về giá trị của 'result' nếu khác null, ngược lại trả về 'ApiResult<ImportReceiptDto>.Fail("Không thể tạo phiếu nhập kho.")'.
            }
            catch (Exception ex) // Khối bắt lỗi ngoại lệ Exception nếu xảy ra lỗi trong quá trình xử lý.
            {
                return ApiResult<ImportReceiptDto>.Fail($"Lỗi kết nối: {ex.Message}"); // Trả về giá trị của biểu thức 'ApiResult<ImportReceiptDto>.Fail($"Lỗi kết nối: {ex.Message}")'.
            }
        }

        public async Task<ApiResult<PagedResult<ImportReceiptDto>>> GetListAsync(ImportReceiptFilterRequest filter, CancellationToken cancellationToken = default) // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'GetListAsync' nhận tham số (filter, default) trả về kết quả kiểu Task<ApiResult<PagedResult<ImportReceiptDto>>>.
        {
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var queryParams = new List<string>(); // Thực hiện gán giá trị của biểu thức 'new List<string>()' cho biến/thuộc tính 'queryParams'.

                if (!string.IsNullOrWhiteSpace(filter.Keyword)) // Kiểm tra xem điều kiện '!string.IsNullOrWhiteSpace(filter.Keyword' có thỏa mãn hay không.
                    queryParams.Add($"Keyword={Uri.EscapeDataString(filter.Keyword)}"); // Thực thi dòng lệnh nghiệp vụ.

                queryParams.Add($"PageNumber={filter.PageNumber}"); // Thực thi dòng lệnh nghiệp vụ.
                queryParams.Add($"PageSize={filter.PageSize}"); // Thực thi dòng lệnh nghiệp vụ.

                if (filter.FromDate.HasValue) // Kiểm tra xem điều kiện 'filter.FromDate.HasValue' có thỏa mãn hay không.
                    queryParams.Add($"FromDate={filter.FromDate.Value:yyyy-MM-dd}"); // Thực thi dòng lệnh nghiệp vụ.
                if (filter.ToDate.HasValue) // Kiểm tra xem điều kiện 'filter.ToDate.HasValue' có thỏa mãn hay không.
                    queryParams.Add($"ToDate={filter.ToDate.Value:yyyy-MM-dd}"); // Thực thi dòng lệnh nghiệp vụ.
                if (filter.SupplierId.HasValue) // Kiểm tra xem điều kiện 'filter.SupplierId.HasValue' có thỏa mãn hay không.
                    queryParams.Add($"SupplierId={filter.SupplierId.Value}"); // Thực thi dòng lệnh nghiệp vụ.
                if (!string.IsNullOrWhiteSpace(filter.SortBy)) // Kiểm tra xem điều kiện '!string.IsNullOrWhiteSpace(filter.SortBy' có thỏa mãn hay không.
                    queryParams.Add($"SortBy={Uri.EscapeDataString(filter.SortBy)}"); // Thực thi dòng lệnh nghiệp vụ.
                if (filter.SortDescending) // Kiểm tra xem điều kiện 'filter.SortDescending' có thỏa mãn hay không.
                    queryParams.Add("SortDescending=true"); // Thực thi dòng lệnh nghiệp vụ.

                var url = $"{BaseUrl}?{string.Join("&", queryParams)}"; // Thực hiện gán giá trị của biểu thức '$"{BaseUrl}?{string.Join("&", queryParams)}"' cho biến/thuộc tính 'url'.
                var result = await _httpClient.GetFromJsonAsync<ApiResult<PagedResult<ImportReceiptDto>>>(url, cancellationToken); // Gọi phương thức GET bất đồng bộ tới URL 'url' nhận kết quả kiểu ApiResult<PagedResult<ImportReceiptDto>> và gán kết quả cho biến 'result'.
                return result ?? ApiResult<PagedResult<ImportReceiptDto>>.Fail("Không thể tải danh sách phiếu nhập."); // Trả về giá trị của 'result' nếu khác null, ngược lại trả về 'ApiResult<PagedResult<ImportReceiptDto>>.Fail("Không thể tải danh sách phiếu nhập.")'.
            }
            catch (OperationCanceledException) // Thực thi dòng lệnh nghiệp vụ.
            {
                return ApiResult<PagedResult<ImportReceiptDto>>.Fail(string.Empty); // Trả về giá trị của biểu thức 'ApiResult<PagedResult<ImportReceiptDto>>.Fail(string.Empty)'.
            }
            catch (Exception ex) // Khối bắt lỗi ngoại lệ Exception nếu xảy ra lỗi trong quá trình xử lý.
            {
                return ApiResult<PagedResult<ImportReceiptDto>>.Fail($"Lỗi kết nối: {ex.Message}"); // Trả về giá trị của biểu thức 'ApiResult<PagedResult<ImportReceiptDto>>.Fail($"Lỗi kết nối: {ex.Message}")'.
            }
        }

        public async Task<ApiResult<ImportReceiptDto>> GetByIdAsync(int id) // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'GetByIdAsync' nhận tham số (id) trả về kết quả kiểu Task<ApiResult<ImportReceiptDto>>.
        {
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var result = await _httpClient.GetFromJsonAsync<ApiResult<ImportReceiptDto>>($"{BaseUrl}/{id}"); // Gọi phương thức GET bất đồng bộ tới URL '{BaseUrl}/{id}' nhận kết quả kiểu ApiResult<ImportReceiptDto> và gán kết quả cho biến 'result'.
                return result ?? ApiResult<ImportReceiptDto>.Fail("Không tìm thấy phiếu nhập."); // Trả về giá trị của 'result' nếu khác null, ngược lại trả về 'ApiResult<ImportReceiptDto>.Fail("Không tìm thấy phiếu nhập.")'.
            }
            catch (Exception ex) // Khối bắt lỗi ngoại lệ Exception nếu xảy ra lỗi trong quá trình xử lý.
            {
                return ApiResult<ImportReceiptDto>.Fail($"Lỗi kết nối: {ex.Message}"); // Trả về giá trị của biểu thức 'ApiResult<ImportReceiptDto>.Fail($"Lỗi kết nối: {ex.Message}")'.
            }
        }
    }
}

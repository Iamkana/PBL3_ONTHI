using System.Net.Http; // Nhập (import) namespace System.Net.Http để sử dụng các lớp bên trong.
using System.Net.Http.Json; // Sử dụng thư viện hỗ trợ gửi nhận dữ liệu JSON qua HTTP.
using System.Threading.Tasks; // Nhập (import) namespace System.Threading.Tasks để sử dụng các lớp bên trong.
using PBL3.Shared.DTOs.Common; // Sử dụng các DTO của module Common thuộc tầng Shared.
using PBL3.Shared.DTOs.Inventory; // Sử dụng các DTO của module Inventory thuộc tầng Shared.

namespace Client.Services.Inventory // Thiết lập namespace Client.Services.Inventory để tổ chức quản lý cấu trúc các lớp.
{
    public class InventoryExportClientService : IInventoryExportClientService // Định nghĩa lớp InventoryExportClientService triển khai các dịch vụ hoặc kế thừa từ IInventoryExportClientService.
    {
        private readonly HttpClient _httpClient; // Khai báo đối tượng HttpClient nội bộ dùng để thực hiện cuộc gọi API.

        public InventoryExportClientService(HttpClient httpClient) // Hàm khởi tạo (Constructor) của lớp InventoryExportClientService tiêm các phụ thuộc: httpClient.
        {
            _httpClient = httpClient; // Thực hiện gán giá trị của biểu thức 'httpClient' cho biến/thuộc tính '_httpClient'.
        }

        public async Task<ApiResult<bool>> ExportOrderAsync(ExportOrderRequest request) // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'ExportOrderAsync' nhận tham số (request) trả về kết quả kiểu Task<ApiResult<bool>>.
        {
            var response = await _httpClient.PostAsJsonAsync("/api/inventory/export-order", request); // Gọi phương thức POST (gửi dữ liệu JSON) bất đồng bộ tới URL '/api/inventory/export-order' và gán kết quả cho biến 'response'.
            if (response.IsSuccessStatusCode) // Kiểm tra xem điều kiện 'response.IsSuccessStatusCode' có thỏa mãn hay không.
            {
                return await response.Content.ReadFromJsonAsync<ApiResult<bool>>() // Trả về giá trị của biểu thức 'await response.Content.ReadFromJsonAsync<ApiResult<bool>>()'.
                       ?? ApiResult<bool>.Fail("Không nhận được dữ liệu hợp lệ từ server."); // Thực thi dòng lệnh nghiệp vụ.
            }

            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var errorResult = await response.Content.ReadFromJsonAsync<ApiResult<bool>>(); // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'errorResult'.
                return errorResult ?? ApiResult<bool>.Fail($"Lỗi HTTP: {response.StatusCode}"); // Trả về giá trị của 'errorResult' nếu khác null, ngược lại trả về 'ApiResult<bool>.Fail($"Lỗi HTTP: {response.StatusCode}")'.
            }
            catch // Thực thi dòng lệnh nghiệp vụ.
            {
                return ApiResult<bool>.Fail($"Lỗi HTTP: {response.StatusCode}"); // Trả về giá trị của biểu thức 'ApiResult<bool>.Fail($"Lỗi HTTP: {response.StatusCode}")'.
            }
        }

        public async Task<ApiResult<bool>> ValidateSerialAsync(string serialNo, int variantId) // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'ValidateSerialAsync' nhận tham số (serialNo, variantId) trả về kết quả kiểu Task<ApiResult<bool>>.
        {
            var url = $"/api/inventory/serials/validate?serialNo={Uri.EscapeDataString(serialNo)}&variantId={variantId}"; // Thực hiện gán giá trị của biểu thức '$"/api/inventory/serials/validate?serialNo={Uri.EscapeDataString(serialNo)}&variantId={variantId}"' cho biến/thuộc tính 'url'.
            var response = await _httpClient.GetAsync(url); // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'response'.

            if (response.IsSuccessStatusCode) // Kiểm tra xem điều kiện 'response.IsSuccessStatusCode' có thỏa mãn hay không.
            {
                return await response.Content.ReadFromJsonAsync<ApiResult<bool>>() // Trả về giá trị của biểu thức 'await response.Content.ReadFromJsonAsync<ApiResult<bool>>()'.
                       ?? ApiResult<bool>.Fail("Không nhận được dữ liệu hợp lệ từ server."); // Thực thi dòng lệnh nghiệp vụ.
            }

            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var errorResult = await response.Content.ReadFromJsonAsync<ApiResult<bool>>(); // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'errorResult'.
                return errorResult ?? ApiResult<bool>.Fail($"Lỗi HTTP: {response.StatusCode}"); // Trả về giá trị của 'errorResult' nếu khác null, ngược lại trả về 'ApiResult<bool>.Fail($"Lỗi HTTP: {response.StatusCode}")'.
            }
            catch // Thực thi dòng lệnh nghiệp vụ.
            {
                return ApiResult<bool>.Fail($"Lỗi HTTP: {response.StatusCode}"); // Trả về giá trị của biểu thức 'ApiResult<bool>.Fail($"Lỗi HTTP: {response.StatusCode}")'.
            }
        }
    }
}

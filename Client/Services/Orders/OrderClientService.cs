using System.Net.Http; // Nhập (import) namespace System.Net.Http để sử dụng các lớp bên trong.
using System.Net.Http.Json; // Sử dụng thư viện hỗ trợ gửi nhận dữ liệu JSON qua HTTP.
using System.Threading.Tasks; // Nhập (import) namespace System.Threading.Tasks để sử dụng các lớp bên trong.
using PBL3.Shared.DTOs.Common; // Sử dụng các DTO của module Common thuộc tầng Shared.
using PBL3.Shared.DTOs.Sale; // Sử dụng các DTO của module Sale thuộc tầng Shared.

namespace Client.Services.Orders // Thiết lập namespace Client.Services.Orders để tổ chức quản lý cấu trúc các lớp.
{
    public class OrderClientService : IOrderClientService // Định nghĩa lớp OrderClientService triển khai các dịch vụ hoặc kế thừa từ IOrderClientService.
    {
        private readonly HttpClient _httpClient; // Khai báo đối tượng HttpClient nội bộ dùng để thực hiện cuộc gọi API.

        public OrderClientService(HttpClient httpClient) // Hàm khởi tạo (Constructor) của lớp OrderClientService tiêm các phụ thuộc: httpClient.
        {
            _httpClient = httpClient; // Thực hiện gán giá trị của biểu thức 'httpClient' cho biến/thuộc tính '_httpClient'.
        }

        public async Task<ApiResult<OrderDetailDto>> GetByIdAsync(int id) // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'GetByIdAsync' nhận tham số (id) trả về kết quả kiểu Task<ApiResult<OrderDetailDto>>.
        {
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var response = await _httpClient.GetAsync($"/api/orders/{id}"); // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'response'.
                var result = await response.Content.ReadFromJsonAsync<ApiResult<OrderDetailDto>>(); // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'result'.
                return result ?? ApiResult<OrderDetailDto>.Fail("Không nhận được phản hồi từ máy chủ."); // Trả về giá trị của 'result' nếu khác null, ngược lại trả về 'ApiResult<OrderDetailDto>.Fail("Không nhận được phản hồi từ máy chủ.")'.
            }
            catch (Exception ex) // Khối bắt lỗi ngoại lệ Exception nếu xảy ra lỗi trong quá trình xử lý.
            {
                return ApiResult<OrderDetailDto>.Fail($"Lỗi kết nối: {ex.Message}"); // Trả về giá trị của biểu thức 'ApiResult<OrderDetailDto>.Fail($"Lỗi kết nối: {ex.Message}")'.
            }
        }

        public async Task<PagedResult<OrderSummaryResponse>> GetPagedOrdersAsync(OrderFilterRequest request) // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'GetPagedOrdersAsync' nhận tham số (request) trả về kết quả kiểu Task<PagedResult<OrderSummaryResponse>>.
        {
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var queryString = $"?pageIndex={request.PageIndex}&pageSize={request.PageSize}"; // Thực hiện gán giá trị của biểu thức '$"?pageIndex={request.PageIndex}&pageSize={request.PageSize}"' cho biến/thuộc tính 'queryString'.
                if (!string.IsNullOrEmpty(request.Keyword)) // Kiểm tra xem điều kiện '!string.IsNullOrEmpty(request.Keyword' có thỏa mãn hay không.
                    queryString += $"&keyword={request.Keyword}"; // Thực thi dòng lệnh nghiệp vụ.
                if (request.Status.HasValue) // Kiểm tra xem điều kiện 'request.Status.HasValue' có thỏa mãn hay không.
                    queryString += $"&status={request.Status.Value}"; // Thực thi dòng lệnh nghiệp vụ.
                if (request.MinStatus.HasValue) // Kiểm tra xem điều kiện 'request.MinStatus.HasValue' có thỏa mãn hay không.
                    queryString += $"&minStatus={request.MinStatus.Value}"; // Thực thi dòng lệnh nghiệp vụ.
                if (request.MaxStatus.HasValue) // Kiểm tra xem điều kiện 'request.MaxStatus.HasValue' có thỏa mãn hay không.
                    queryString += $"&maxStatus={request.MaxStatus.Value}"; // Thực thi dòng lệnh nghiệp vụ.
                if (request.FromDate.HasValue) // Kiểm tra xem điều kiện 'request.FromDate.HasValue' có thỏa mãn hay không.
                    queryString += $"&fromDate={request.FromDate.Value:yyyy-MM-ddTHH:mm:ss}"; // Thực thi dòng lệnh nghiệp vụ.
                if (request.ToDate.HasValue) // Kiểm tra xem điều kiện 'request.ToDate.HasValue' có thỏa mãn hay không.
                    queryString += $"&toDate={request.ToDate.Value:yyyy-MM-ddTHH:mm:ss}"; // Thực thi dòng lệnh nghiệp vụ.

                var response = await _httpClient.GetAsync($"/api/orders{queryString}"); // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'response'.
                if (!response.IsSuccessStatusCode) // Kiểm tra xem điều kiện '!response.IsSuccessStatusCode' có thỏa mãn hay không.
                    return new PagedResult<OrderSummaryResponse>(); // Trả về giá trị của biểu thức 'new PagedResult<OrderSummaryResponse>()'.
                var result = await response.Content.ReadFromJsonAsync<ApiResult<PagedResult<OrderSummaryResponse>>>(); // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'result'.
                return result?.Data ?? new PagedResult<OrderSummaryResponse>(); // Trả về giá trị của 'result?.Data' nếu khác null, ngược lại trả về 'new PagedResult<OrderSummaryResponse>()'.
            }
            catch // Thực thi dòng lệnh nghiệp vụ.
            {
                return new PagedResult<OrderSummaryResponse>(); // Trả về giá trị của biểu thức 'new PagedResult<OrderSummaryResponse>()'.
            }
        }

        public async Task<ApiResult<PagedResult<OrderSummaryResponse>>> GetMyOrdersAsync(OrderFilterRequest request) // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'GetMyOrdersAsync' nhận tham số (request) trả về kết quả kiểu Task<ApiResult<PagedResult<OrderSummaryResponse>>>.
        {
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var queryString = $"?pageIndex={request.PageIndex}&pageSize={request.PageSize}"; // Thực hiện gán giá trị của biểu thức '$"?pageIndex={request.PageIndex}&pageSize={request.PageSize}"' cho biến/thuộc tính 'queryString'.
                if (request.Status.HasValue) // Kiểm tra xem điều kiện 'request.Status.HasValue' có thỏa mãn hay không.
                    queryString += $"&status={request.Status.Value}"; // Thực thi dòng lệnh nghiệp vụ.

                var response = await _httpClient.GetAsync($"/api/orders/my{queryString}"); // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'response'.
                if (!response.IsSuccessStatusCode) // Kiểm tra xem điều kiện '!response.IsSuccessStatusCode' có thỏa mãn hay không.
                    return ApiResult<PagedResult<OrderSummaryResponse>>.Fail($"Lỗi HTTP {(int)response.StatusCode}."); // Trả về giá trị của biểu thức 'ApiResult<PagedResult<OrderSummaryResponse>>.Fail($"Lỗi HTTP {(int)response.StatusCode}.")'.
                var result = await response.Content.ReadFromJsonAsync<ApiResult<PagedResult<OrderSummaryResponse>>>(); // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'result'.
                return result ?? ApiResult<PagedResult<OrderSummaryResponse>>.Fail("Không nhận được phản hồi từ máy chủ"); // Trả về giá trị của 'result' nếu khác null, ngược lại trả về 'ApiResult<PagedResult<OrderSummaryResponse>>.Fail("Không nhận được phản hồi từ máy chủ")'.
            }
            catch (Exception ex) // Khối bắt lỗi ngoại lệ Exception nếu xảy ra lỗi trong quá trình xử lý.
            {
                return ApiResult<PagedResult<OrderSummaryResponse>>.Fail($"Lỗi kết nối: {ex.Message}"); // Trả về giá trị của biểu thức 'ApiResult<PagedResult<OrderSummaryResponse>>.Fail($"Lỗi kết nối: {ex.Message}")'.
            }
        }

        public async Task<ApiResult<bool>> CancelOrderAsync(int id, string cancelReason) // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'CancelOrderAsync' nhận tham số (id, cancelReason) trả về kết quả kiểu Task<ApiResult<bool>>.
        {
            var request = new CancelOrderRequest { CancelReason = cancelReason }; // Thực hiện gán giá trị của biểu thức 'new CancelOrderRequest { CancelReason = cancelReason }' cho biến/thuộc tính 'request'.
            var response = await _httpClient.PutAsJsonAsync($"/api/orders/{id}/cancel", request); // Gọi phương thức PUT (cập nhật dữ liệu JSON) bất đồng bộ tới URL '/api/orders/{id}/cancel' và gán kết quả cho biến 'response'.
            
            if (response.IsSuccessStatusCode) // Kiểm tra xem điều kiện 'response.IsSuccessStatusCode' có thỏa mãn hay không.
            {
                var result = await response.Content.ReadFromJsonAsync<ApiResult<bool>>(); // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'result'.
                return result ?? ApiResult<bool>.Fail("Không nhận được phản hồi từ máy chủ"); // Trả về giá trị của 'result' nếu khác null, ngược lại trả về 'ApiResult<bool>.Fail("Không nhận được phản hồi từ máy chủ")'.
            }
            
            return ApiResult<bool>.Fail($"Lỗi HTTP: {response.StatusCode}"); // Trả về giá trị của biểu thức 'ApiResult<bool>.Fail($"Lỗi HTTP: {response.StatusCode}")'.
        }

        public async Task<ApiResult<CheckoutResponse>> CheckoutAsync(CheckoutRequest request) // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'CheckoutAsync' nhận tham số (request) trả về kết quả kiểu Task<ApiResult<CheckoutResponse>>.
        {
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var response = await _httpClient.PostAsJsonAsync("/api/orders/checkout", request); // Gọi phương thức POST (gửi dữ liệu JSON) bất đồng bộ tới URL '/api/orders/checkout' và gán kết quả cho biến 'response'.
                if (!response.IsSuccessStatusCode) // Kiểm tra xem điều kiện '!response.IsSuccessStatusCode' có thỏa mãn hay không.
                    return ApiResult<CheckoutResponse>.Fail($"Lỗi HTTP {(int)response.StatusCode}."); // Trả về giá trị của biểu thức 'ApiResult<CheckoutResponse>.Fail($"Lỗi HTTP {(int)response.StatusCode}.")'.
                var result = await response.Content.ReadFromJsonAsync<ApiResult<CheckoutResponse>>(); // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'result'.
                return result ?? ApiResult<CheckoutResponse>.Fail("Không nhận được phản hồi từ máy chủ"); // Trả về giá trị của 'result' nếu khác null, ngược lại trả về 'ApiResult<CheckoutResponse>.Fail("Không nhận được phản hồi từ máy chủ")'.
            }
            catch (Exception ex) // Khối bắt lỗi ngoại lệ Exception nếu xảy ra lỗi trong quá trình xử lý.
            {
                return ApiResult<CheckoutResponse>.Fail($"Lỗi kết nối: {ex.Message}"); // Trả về giá trị của biểu thức 'ApiResult<CheckoutResponse>.Fail($"Lỗi kết nối: {ex.Message}")'.
            }
        }

        public async Task<ApiResult<bool>> CompleteOrderAsync(int id) // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'CompleteOrderAsync' nhận tham số (id) trả về kết quả kiểu Task<ApiResult<bool>>.
        {
            var response = await _httpClient.PutAsJsonAsync($"/api/orders/{id}/complete", new { }); // Gọi phương thức PUT (cập nhật dữ liệu JSON) bất đồng bộ tới URL '/api/orders/{id}/complete' và gán kết quả cho biến 'response'.

            if (response.IsSuccessStatusCode) // Kiểm tra xem điều kiện 'response.IsSuccessStatusCode' có thỏa mãn hay không.
            {
                var result = await response.Content.ReadFromJsonAsync<ApiResult<bool>>(); // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'result'.
                return result ?? ApiResult<bool>.Fail("Không nhận được phản hồi từ máy chủ"); // Trả về giá trị của 'result' nếu khác null, ngược lại trả về 'ApiResult<bool>.Fail("Không nhận được phản hồi từ máy chủ")'.
            }

            return ApiResult<bool>.Fail($"Lỗi HTTP: {response.StatusCode}"); // Trả về giá trị của biểu thức 'ApiResult<bool>.Fail($"Lỗi HTTP: {response.StatusCode}")'.
        }

        public async Task<ApiResult<bool>> ConfirmOrderAsync(int id) // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'ConfirmOrderAsync' nhận tham số (id) trả về kết quả kiểu Task<ApiResult<bool>>.
        {
            var response = await _httpClient.PutAsJsonAsync($"/api/orders/{id}/confirm", new { }); // Gọi phương thức PUT (cập nhật dữ liệu JSON) bất đồng bộ tới URL '/api/orders/{id}/confirm' và gán kết quả cho biến 'response'.

            if (response.IsSuccessStatusCode) // Kiểm tra xem điều kiện 'response.IsSuccessStatusCode' có thỏa mãn hay không.
            {
                var result = await response.Content.ReadFromJsonAsync<ApiResult<bool>>(); // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'result'.
                return result ?? ApiResult<bool>.Fail("Không nhận được phản hồi từ máy chủ"); // Trả về giá trị của 'result' nếu khác null, ngược lại trả về 'ApiResult<bool>.Fail("Không nhận được phản hồi từ máy chủ")'.
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

        public async Task<ApiResult<bool>> CancelMyOrderAsync(int id, string cancelReason) // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'CancelMyOrderAsync' nhận tham số (id, cancelReason) trả về kết quả kiểu Task<ApiResult<bool>>.
        {
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var request = new CancelOrderRequest { CancelReason = cancelReason }; // Thực hiện gán giá trị của biểu thức 'new CancelOrderRequest { CancelReason = cancelReason }' cho biến/thuộc tính 'request'.
                var response = await _httpClient.PutAsJsonAsync($"/api/orders/my/{id}/cancel", request); // Gọi phương thức PUT (cập nhật dữ liệu JSON) bất đồng bộ tới URL '/api/orders/my/{id}/cancel' và gán kết quả cho biến 'response'.
                var result = await response.Content.ReadFromJsonAsync<ApiResult<bool>>(); // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'result'.
                return result ?? ApiResult<bool>.Fail("Không nhận được phản hồi từ máy chủ"); // Trả về giá trị của 'result' nếu khác null, ngược lại trả về 'ApiResult<bool>.Fail("Không nhận được phản hồi từ máy chủ")'.
            }
            catch (Exception ex) // Khối bắt lỗi ngoại lệ Exception nếu xảy ra lỗi trong quá trình xử lý.
            {
                return ApiResult<bool>.Fail($"Lỗi kết nối: {ex.Message}"); // Trả về giá trị của biểu thức 'ApiResult<bool>.Fail($"Lỗi kết nối: {ex.Message}")'.
            }
        }

        public async Task<ApiResult<bool>> ConfirmReceivedAsync(int id) // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'ConfirmReceivedAsync' nhận tham số (id) trả về kết quả kiểu Task<ApiResult<bool>>.
        {
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var response = await _httpClient.PutAsJsonAsync($"/api/orders/my/{id}/confirm-received", new { }); // Gọi phương thức PUT (cập nhật dữ liệu JSON) bất đồng bộ tới URL '/api/orders/my/{id}/confirm-received' và gán kết quả cho biến 'response'.
                var result = await response.Content.ReadFromJsonAsync<ApiResult<bool>>(); // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'result'.
                return result ?? ApiResult<bool>.Fail("Không nhận được phản hồi từ máy chủ"); // Trả về giá trị của 'result' nếu khác null, ngược lại trả về 'ApiResult<bool>.Fail("Không nhận được phản hồi từ máy chủ")'.
            }
            catch (Exception ex) // Khối bắt lỗi ngoại lệ Exception nếu xảy ra lỗi trong quá trình xử lý.
            {
                return ApiResult<bool>.Fail($"Lỗi kết nối: {ex.Message}"); // Trả về giá trị của biểu thức 'ApiResult<bool>.Fail($"Lỗi kết nối: {ex.Message}")'.
            }
        }
    }
}

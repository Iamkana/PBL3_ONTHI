using System; // Nhập (import) namespace System để sử dụng các lớp bên trong.
using System.Net.Http.Json; // Sử dụng thư viện hỗ trợ gửi nhận dữ liệu JSON qua HTTP.
using System.Threading.Tasks; // Nhập (import) namespace System.Threading.Tasks để sử dụng các lớp bên trong.
using PBL3.Shared.DTOs.Common; // Sử dụng các DTO của module Common thuộc tầng Shared.
using PBL3.Shared.DTOs.ServiceTickets; // Sử dụng các DTO của module ServiceTickets thuộc tầng Shared.

namespace Client.Services.ServiceTickets // Thiết lập namespace Client.Services.ServiceTickets để tổ chức quản lý cấu trúc các lớp.
{
    public class ServiceInvoiceClientService : IServiceInvoiceClientService // Định nghĩa lớp ServiceInvoiceClientService triển khai các dịch vụ hoặc kế thừa từ IServiceInvoiceClientService.
    {
        private readonly HttpClient _httpClient; // Khai báo đối tượng HttpClient nội bộ dùng để thực hiện cuộc gọi API.

        public ServiceInvoiceClientService(HttpClient httpClient) // Hàm khởi tạo (Constructor) của lớp ServiceInvoiceClientService tiêm các phụ thuộc: httpClient.
        {
            _httpClient = httpClient; // Thực hiện gán giá trị của biểu thức 'httpClient' cho biến/thuộc tính '_httpClient'.
        }

        public async Task<ApiResult<PagedResult<ServiceInvoiceListDto>>> GetPagedListAsync( // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'GetPagedListAsync' không tham số trả về kết quả kiểu Task<ApiResult<PagedResult<ServiceInvoiceListDto>>>.
            string? keyword, byte? paymentStatus, DateTime? fromDate, DateTime? toDate, // Thực thi dòng lệnh nghiệp vụ.
            int pageNumber, int pageSize, string? sortBy, bool sortDescending) // Thực thi dòng lệnh nghiệp vụ.
        {
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var url = $"api/service-invoices?pageNumber={pageNumber}&pageSize={pageSize}"; // Thực hiện gán giá trị của biểu thức '$"api/service-invoices?pageNumber={pageNumber}&pageSize={pageSize}"' cho biến/thuộc tính 'url'.
                if (!string.IsNullOrEmpty(keyword)) // Kiểm tra xem điều kiện '!string.IsNullOrEmpty(keyword' có thỏa mãn hay không.
                    url += $"&keyword={Uri.EscapeDataString(keyword)}"; // Thực thi dòng lệnh nghiệp vụ.
                if (paymentStatus.HasValue) // Kiểm tra xem điều kiện 'paymentStatus.HasValue' có thỏa mãn hay không.
                    url += $"&paymentStatus={paymentStatus.Value}"; // Thực thi dòng lệnh nghiệp vụ.
                if (fromDate.HasValue) // Kiểm tra xem điều kiện 'fromDate.HasValue' có thỏa mãn hay không.
                    url += $"&fromDate={fromDate.Value:yyyy-MM-dd}"; // Thực thi dòng lệnh nghiệp vụ.
                if (toDate.HasValue) // Kiểm tra xem điều kiện 'toDate.HasValue' có thỏa mãn hay không.
                    url += $"&toDate={toDate.Value:yyyy-MM-dd}"; // Thực thi dòng lệnh nghiệp vụ.
                if (!string.IsNullOrEmpty(sortBy)) // Kiểm tra xem điều kiện '!string.IsNullOrEmpty(sortBy' có thỏa mãn hay không.
                    url += $"&sortBy={Uri.EscapeDataString(sortBy)}&sortDescending={sortDescending}"; // Thực thi dòng lệnh nghiệp vụ.

                var response = await _httpClient.GetAsync(url); // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'response'.
                if (!response.IsSuccessStatusCode) // Kiểm tra xem điều kiện '!response.IsSuccessStatusCode' có thỏa mãn hay không.
                    return new ApiResult<PagedResult<ServiceInvoiceListDto>> { Message = $"Lỗi HTTP {(int)response.StatusCode}." }; // Trả về giá trị của biểu thức 'new ApiResult<PagedResult<ServiceInvoiceListDto>> { Message = $"Lỗi HTTP {(int)response.StatusCode}." }'.

                try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
                {
                    return await response.Content.ReadFromJsonAsync<ApiResult<PagedResult<ServiceInvoiceListDto>>>() // Trả về giá trị của biểu thức 'await response.Content.ReadFromJsonAsync<ApiResult<PagedResult<ServiceInvoiceListDto>>>()'.
                        ?? new ApiResult<PagedResult<ServiceInvoiceListDto>> { Message = "Lỗi lấy danh sách hóa đơn." }; // Thực hiện gán giá trị của biểu thức '"Lỗi lấy danh sách hóa đơn." }' cho biến/thuộc tính '?? new ApiResult<PagedResult<ServiceInvoiceListDto>> { Message'.
                }
                catch // Thực thi dòng lệnh nghiệp vụ.
                {
                    return new ApiResult<PagedResult<ServiceInvoiceListDto>> { Message = "Lỗi lấy danh sách hóa đơn." }; // Trả về giá trị của biểu thức 'new ApiResult<PagedResult<ServiceInvoiceListDto>> { Message = "Lỗi lấy danh sách hóa đơn." }'.
                }
            }
            catch (Exception ex) // Khối bắt lỗi ngoại lệ Exception nếu xảy ra lỗi trong quá trình xử lý.
            {
                return new ApiResult<PagedResult<ServiceInvoiceListDto>> { Message = $"Lỗi: {ex.Message}" }; // Trả về giá trị của biểu thức 'new ApiResult<PagedResult<ServiceInvoiceListDto>> { Message = $"Lỗi: {ex.Message}" }'.
            }
        }

        public async Task<ApiResult<ServiceInvoiceDetailDto>> GetByIdAsync(int id) // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'GetByIdAsync' nhận tham số (id) trả về kết quả kiểu Task<ApiResult<ServiceInvoiceDetailDto>>.
        {
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var response = await _httpClient.GetAsync($"api/service-invoices/{id}"); // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'response'.
                if (!response.IsSuccessStatusCode) // Kiểm tra xem điều kiện '!response.IsSuccessStatusCode' có thỏa mãn hay không.
                    return new ApiResult<ServiceInvoiceDetailDto> { Message = $"Lỗi HTTP {(int)response.StatusCode}." }; // Trả về giá trị của biểu thức 'new ApiResult<ServiceInvoiceDetailDto> { Message = $"Lỗi HTTP {(int)response.StatusCode}." }'.

                try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
                {
                    return await response.Content.ReadFromJsonAsync<ApiResult<ServiceInvoiceDetailDto>>() // Trả về giá trị của biểu thức 'await response.Content.ReadFromJsonAsync<ApiResult<ServiceInvoiceDetailDto>>()'.
                        ?? new ApiResult<ServiceInvoiceDetailDto> { Message = "Không tìm thấy hóa đơn." }; // Thực hiện gán giá trị của biểu thức '"Không tìm thấy hóa đơn." }' cho biến/thuộc tính '?? new ApiResult<ServiceInvoiceDetailDto> { Message'.
                }
                catch // Thực thi dòng lệnh nghiệp vụ.
                {
                    return new ApiResult<ServiceInvoiceDetailDto> { Message = "Không tìm thấy hóa đơn." }; // Trả về giá trị của biểu thức 'new ApiResult<ServiceInvoiceDetailDto> { Message = "Không tìm thấy hóa đơn." }'.
                }
            }
            catch (Exception ex) // Khối bắt lỗi ngoại lệ Exception nếu xảy ra lỗi trong quá trình xử lý.
            {
                return new ApiResult<ServiceInvoiceDetailDto> { Message = $"Lỗi: {ex.Message}" }; // Trả về giá trị của biểu thức 'new ApiResult<ServiceInvoiceDetailDto> { Message = $"Lỗi: {ex.Message}" }'.
            }
        }

        public async Task<ApiResult<bool>> MarkInvoicePaidAsync(int id) // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'MarkInvoicePaidAsync' nhận tham số (id) trả về kết quả kiểu Task<ApiResult<bool>>.
        {
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var response = await _httpClient.PatchAsync($"api/service-invoices/{id}/mark-paid", null); // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'response'.
                if (!response.IsSuccessStatusCode) // Kiểm tra xem điều kiện '!response.IsSuccessStatusCode' có thỏa mãn hay không.
                    return new ApiResult<bool> { Message = $"Lỗi HTTP {(int)response.StatusCode}." }; // Trả về giá trị của biểu thức 'new ApiResult<bool> { Message = $"Lỗi HTTP {(int)response.StatusCode}." }'.

                try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
                {
                    return await response.Content.ReadFromJsonAsync<ApiResult<bool>>() // Trả về giá trị của biểu thức 'await response.Content.ReadFromJsonAsync<ApiResult<bool>>()'.
                        ?? new ApiResult<bool> { Message = "Lỗi xác nhận thanh toán." }; // Thực hiện gán giá trị của biểu thức '"Lỗi xác nhận thanh toán." }' cho biến/thuộc tính '?? new ApiResult<bool> { Message'.
                }
                catch // Thực thi dòng lệnh nghiệp vụ.
                {
                    return new ApiResult<bool> { Message = "Lỗi xác nhận thanh toán." }; // Trả về giá trị của biểu thức 'new ApiResult<bool> { Message = "Lỗi xác nhận thanh toán." }'.
                }
            }
            catch (Exception ex) // Khối bắt lỗi ngoại lệ Exception nếu xảy ra lỗi trong quá trình xử lý.
            {
                return new ApiResult<bool> { Message = $"Lỗi: {ex.Message}" }; // Trả về giá trị của biểu thức 'new ApiResult<bool> { Message = $"Lỗi: {ex.Message}" }'.
            }
        }
    }
}

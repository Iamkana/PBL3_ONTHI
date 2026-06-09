using System; // Nhập (import) namespace System để sử dụng các lớp bên trong.
using System.Net.Http.Json; // Sử dụng thư viện hỗ trợ gửi nhận dữ liệu JSON qua HTTP.
using System.Threading.Tasks; // Nhập (import) namespace System.Threading.Tasks để sử dụng các lớp bên trong.
using PBL3.Shared.DTOs.Common; // Sử dụng các DTO của module Common thuộc tầng Shared.
using PBL3.Shared.DTOs.ServiceTickets; // Sử dụng các DTO của module ServiceTickets thuộc tầng Shared.

namespace Client.Services.ServiceTickets // Thiết lập namespace Client.Services.ServiceTickets để tổ chức quản lý cấu trúc các lớp.
{
    public class ServiceTicketClientService : IServiceTicketClientService // Định nghĩa lớp ServiceTicketClientService triển khai các dịch vụ hoặc kế thừa từ IServiceTicketClientService.
    {
        private readonly HttpClient _httpClient; // Khai báo đối tượng HttpClient nội bộ dùng để thực hiện cuộc gọi API.

        public ServiceTicketClientService(HttpClient httpClient) // Hàm khởi tạo (Constructor) của lớp ServiceTicketClientService tiêm các phụ thuộc: httpClient.
        {
            _httpClient = httpClient; // Thực hiện gán giá trị của biểu thức 'httpClient' cho biến/thuộc tính '_httpClient'.
        }

        public async Task<ApiResult<ServiceTicketIntakeEvaluationDto>> EvaluateIntakeAsync(string serialNumber) // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'EvaluateIntakeAsync' nhận tham số (serialNumber) trả về kết quả kiểu Task<ApiResult<ServiceTicketIntakeEvaluationDto>>.
        {
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var response = await _httpClient.PostAsJsonAsync("api/service-tickets/intake", serialNumber); // Gọi phương thức POST (gửi dữ liệu JSON) bất đồng bộ tới URL 'api/service-tickets/intake' và gán kết quả cho biến 'response'.
                return await response.Content.ReadFromJsonAsync<ApiResult<ServiceTicketIntakeEvaluationDto>>() // Trả về giá trị của biểu thức 'await response.Content.ReadFromJsonAsync<ApiResult<ServiceTicketIntakeEvaluationDto>>()'.
                    ?? new ApiResult<ServiceTicketIntakeEvaluationDto> { Message = "Lỗi đánh giá Serial." }; // Thực hiện gán giá trị của biểu thức '"Lỗi đánh giá Serial." }' cho biến/thuộc tính '?? new ApiResult<ServiceTicketIntakeEvaluationDto> { Message'.
            }
            catch (Exception ex) // Khối bắt lỗi ngoại lệ Exception nếu xảy ra lỗi trong quá trình xử lý.
            {
                return new ApiResult<ServiceTicketIntakeEvaluationDto> { Message = $"Lỗi: {ex.Message}" }; // Trả về giá trị của biểu thức 'new ApiResult<ServiceTicketIntakeEvaluationDto> { Message = $"Lỗi: {ex.Message}" }'.
            }
        }

        public async Task<ApiResult<ServiceTicketDetailDto>> CreateTicketAsync(CreateServiceTicketRequest request) // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'CreateTicketAsync' nhận tham số (request) trả về kết quả kiểu Task<ApiResult<ServiceTicketDetailDto>>.
        {
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var response = await _httpClient.PostAsJsonAsync("api/service-tickets", request); // Gọi phương thức POST (gửi dữ liệu JSON) bất đồng bộ tới URL 'api/service-tickets' và gán kết quả cho biến 'response'.
                return await response.Content.ReadFromJsonAsync<ApiResult<ServiceTicketDetailDto>>() // Trả về giá trị của biểu thức 'await response.Content.ReadFromJsonAsync<ApiResult<ServiceTicketDetailDto>>()'.
                    ?? new ApiResult<ServiceTicketDetailDto> { Message = "Lỗi tạo phiếu sửa chữa." }; // Thực hiện gán giá trị của biểu thức '"Lỗi tạo phiếu sửa chữa." }' cho biến/thuộc tính '?? new ApiResult<ServiceTicketDetailDto> { Message'.
            }
            catch (Exception ex) // Khối bắt lỗi ngoại lệ Exception nếu xảy ra lỗi trong quá trình xử lý.
            {
                return new ApiResult<ServiceTicketDetailDto> { Message = $"Lỗi: {ex.Message}" }; // Trả về giá trị của biểu thức 'new ApiResult<ServiceTicketDetailDto> { Message = $"Lỗi: {ex.Message}" }'.
            }
        }

        public async Task<ApiResult<PagedResult<ServiceTicketListDto>>> GetPagedListAsync( // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'GetPagedListAsync' không tham số trả về kết quả kiểu Task<ApiResult<PagedResult<ServiceTicketListDto>>>.
            string? keyword, byte? status, DateTime? fromDate, DateTime? toDate, // Thực thi dòng lệnh nghiệp vụ.
            int pageNumber, int pageSize, string? sortBy, bool sortDescending) // Thực thi dòng lệnh nghiệp vụ.
        {
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var url = $"api/service-tickets?pageNumber={pageNumber}&pageSize={pageSize}"; // Thực hiện gán giá trị của biểu thức '$"api/service-tickets?pageNumber={pageNumber}&pageSize={pageSize}"' cho biến/thuộc tính 'url'.
                if (!string.IsNullOrEmpty(keyword)) // Kiểm tra xem điều kiện '!string.IsNullOrEmpty(keyword' có thỏa mãn hay không.
                    url += $"&keyword={Uri.EscapeDataString(keyword)}"; // Thực thi dòng lệnh nghiệp vụ.
                if (status.HasValue) // Kiểm tra xem điều kiện 'status.HasValue' có thỏa mãn hay không.
                    url += $"&status={status.Value}"; // Thực thi dòng lệnh nghiệp vụ.
                if (fromDate.HasValue) // Kiểm tra xem điều kiện 'fromDate.HasValue' có thỏa mãn hay không.
                    url += $"&fromDate={fromDate.Value:yyyy-MM-dd}"; // Thực thi dòng lệnh nghiệp vụ.
                if (toDate.HasValue) // Kiểm tra xem điều kiện 'toDate.HasValue' có thỏa mãn hay không.
                    url += $"&toDate={toDate.Value:yyyy-MM-dd}"; // Thực thi dòng lệnh nghiệp vụ.
                if (!string.IsNullOrEmpty(sortBy)) // Kiểm tra xem điều kiện '!string.IsNullOrEmpty(sortBy' có thỏa mãn hay không.
                    url += $"&sortBy={Uri.EscapeDataString(sortBy)}&sortDescending={sortDescending}"; // Thực thi dòng lệnh nghiệp vụ.

                var response = await _httpClient.GetAsync(url); // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'response'.
                if (!response.IsSuccessStatusCode) // Kiểm tra xem điều kiện '!response.IsSuccessStatusCode' có thỏa mãn hay không.
                    return new ApiResult<PagedResult<ServiceTicketListDto>> { Message = "Lỗi tải danh sách phiếu." }; // Trả về giá trị của biểu thức 'new ApiResult<PagedResult<ServiceTicketListDto>> { Message = "Lỗi tải danh sách phiếu." }'.

                try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
                {
                    return await response.Content.ReadFromJsonAsync<ApiResult<PagedResult<ServiceTicketListDto>>>() // Trả về giá trị của biểu thức 'await response.Content.ReadFromJsonAsync<ApiResult<PagedResult<ServiceTicketListDto>>>()'.
                        ?? new ApiResult<PagedResult<ServiceTicketListDto>> { Message = "Lỗi lấy danh sách." }; // Thực hiện gán giá trị của biểu thức '"Lỗi lấy danh sách." }' cho biến/thuộc tính '?? new ApiResult<PagedResult<ServiceTicketListDto>> { Message'.
                }
                catch // Thực thi dòng lệnh nghiệp vụ.
                {
                    return new ApiResult<PagedResult<ServiceTicketListDto>> { Message = "Lỗi lấy danh sách." }; // Trả về giá trị của biểu thức 'new ApiResult<PagedResult<ServiceTicketListDto>> { Message = "Lỗi lấy danh sách." }'.
                }
            }
            catch (Exception ex) // Khối bắt lỗi ngoại lệ Exception nếu xảy ra lỗi trong quá trình xử lý.
            {
                return new ApiResult<PagedResult<ServiceTicketListDto>> { Message = $"Lỗi: {ex.Message}" }; // Trả về giá trị của biểu thức 'new ApiResult<PagedResult<ServiceTicketListDto>> { Message = $"Lỗi: {ex.Message}" }'.
            }
        }

        public async Task<ApiResult<PagedResult<ServiceTicketListDto>>> GetMyTicketsAsync( // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'GetMyTicketsAsync' không tham số trả về kết quả kiểu Task<ApiResult<PagedResult<ServiceTicketListDto>>>.
            string? keyword, byte? status, int pageNumber, int pageSize, string? sortBy, bool sortDescending) // Thực thi dòng lệnh nghiệp vụ.
        {
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var url = $"api/service-tickets/my?pageNumber={pageNumber}&pageSize={pageSize}"; // Thực hiện gán giá trị của biểu thức '$"api/service-tickets/my?pageNumber={pageNumber}&pageSize={pageSize}"' cho biến/thuộc tính 'url'.
                if (!string.IsNullOrEmpty(keyword)) // Kiểm tra xem điều kiện '!string.IsNullOrEmpty(keyword' có thỏa mãn hay không.
                    url += $"&keyword={Uri.EscapeDataString(keyword)}"; // Thực thi dòng lệnh nghiệp vụ.
                if (status.HasValue) // Kiểm tra xem điều kiện 'status.HasValue' có thỏa mãn hay không.
                    url += $"&status={status.Value}"; // Thực thi dòng lệnh nghiệp vụ.
                if (!string.IsNullOrEmpty(sortBy)) // Kiểm tra xem điều kiện '!string.IsNullOrEmpty(sortBy' có thỏa mãn hay không.
                    url += $"&sortBy={Uri.EscapeDataString(sortBy)}&sortDescending={sortDescending}"; // Thực thi dòng lệnh nghiệp vụ.

                var response = await _httpClient.GetAsync(url); // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'response'.
                if (!response.IsSuccessStatusCode) // Kiểm tra xem điều kiện '!response.IsSuccessStatusCode' có thỏa mãn hay không.
                    return new ApiResult<PagedResult<ServiceTicketListDto>> { Message = "Lỗi tải danh sách phiếu." }; // Trả về giá trị của biểu thức 'new ApiResult<PagedResult<ServiceTicketListDto>> { Message = "Lỗi tải danh sách phiếu." }'.

                try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
                {
                    return await response.Content.ReadFromJsonAsync<ApiResult<PagedResult<ServiceTicketListDto>>>() // Trả về giá trị của biểu thức 'await response.Content.ReadFromJsonAsync<ApiResult<PagedResult<ServiceTicketListDto>>>()'.
                        ?? new ApiResult<PagedResult<ServiceTicketListDto>> { Message = "Lỗi lấy danh sách." }; // Thực hiện gán giá trị của biểu thức '"Lỗi lấy danh sách." }' cho biến/thuộc tính '?? new ApiResult<PagedResult<ServiceTicketListDto>> { Message'.
                }
                catch // Thực thi dòng lệnh nghiệp vụ.
                {
                    return new ApiResult<PagedResult<ServiceTicketListDto>> { Message = "Lỗi lấy danh sách." }; // Trả về giá trị của biểu thức 'new ApiResult<PagedResult<ServiceTicketListDto>> { Message = "Lỗi lấy danh sách." }'.
                }
            }
            catch (Exception ex) // Khối bắt lỗi ngoại lệ Exception nếu xảy ra lỗi trong quá trình xử lý.
            {
                return new ApiResult<PagedResult<ServiceTicketListDto>> { Message = $"Lỗi: {ex.Message}" }; // Trả về giá trị của biểu thức 'new ApiResult<PagedResult<ServiceTicketListDto>> { Message = $"Lỗi: {ex.Message}" }'.
            }
        }

        public async Task<ApiResult<ServiceTicketDetailDto>> GetByIdAsync(int id) // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'GetByIdAsync' nhận tham số (id) trả về kết quả kiểu Task<ApiResult<ServiceTicketDetailDto>>.
        {
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var response = await _httpClient.GetAsync($"api/service-tickets/{id}"); // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'response'.
                if (!response.IsSuccessStatusCode) // Kiểm tra xem điều kiện '!response.IsSuccessStatusCode' có thỏa mãn hay không.
                    return new ApiResult<ServiceTicketDetailDto> { Message = "Không thể tải phiếu." }; // Trả về giá trị của biểu thức 'new ApiResult<ServiceTicketDetailDto> { Message = "Không thể tải phiếu." }'.

                try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
                {
                    return await response.Content.ReadFromJsonAsync<ApiResult<ServiceTicketDetailDto>>() // Trả về giá trị của biểu thức 'await response.Content.ReadFromJsonAsync<ApiResult<ServiceTicketDetailDto>>()'.
                        ?? new ApiResult<ServiceTicketDetailDto> { Message = "Không tìm thấy phiếu." }; // Thực hiện gán giá trị của biểu thức '"Không tìm thấy phiếu." }' cho biến/thuộc tính '?? new ApiResult<ServiceTicketDetailDto> { Message'.
                }
                catch // Thực thi dòng lệnh nghiệp vụ.
                {
                    return new ApiResult<ServiceTicketDetailDto> { Message = "Không tìm thấy phiếu." }; // Trả về giá trị của biểu thức 'new ApiResult<ServiceTicketDetailDto> { Message = "Không tìm thấy phiếu." }'.
                }
            }
            catch (Exception ex) // Khối bắt lỗi ngoại lệ Exception nếu xảy ra lỗi trong quá trình xử lý.
            {
                return new ApiResult<ServiceTicketDetailDto> { Message = $"Lỗi: {ex.Message}" }; // Trả về giá trị của biểu thức 'new ApiResult<ServiceTicketDetailDto> { Message = $"Lỗi: {ex.Message}" }'.
            }
        }

        public async Task<ApiResult<bool>> AssignTechnicianAsync(int ticketId, AssignTechnicianRequest request) // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'AssignTechnicianAsync' nhận tham số (ticketId, request) trả về kết quả kiểu Task<ApiResult<bool>>.
        {
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var response = await _httpClient.PutAsJsonAsync($"api/service-tickets/{ticketId}/assign", request); // Gọi phương thức PUT (cập nhật dữ liệu JSON) bất đồng bộ tới URL 'api/service-tickets/{ticketId}/assign' và gán kết quả cho biến 'response'.
                return await response.Content.ReadFromJsonAsync<ApiResult<bool>>() // Trả về giá trị của biểu thức 'await response.Content.ReadFromJsonAsync<ApiResult<bool>>()'.
                    ?? new ApiResult<bool> { Message = "Lỗi gán kỹ thuật viên." }; // Thực hiện gán giá trị của biểu thức '"Lỗi gán kỹ thuật viên." }' cho biến/thuộc tính '?? new ApiResult<bool> { Message'.
            }
            catch (Exception ex) // Khối bắt lỗi ngoại lệ Exception nếu xảy ra lỗi trong quá trình xử lý.
            {
                return new ApiResult<bool> { Message = $"Lỗi: {ex.Message}" }; // Trả về giá trị của biểu thức 'new ApiResult<bool> { Message = $"Lỗi: {ex.Message}" }'.
            }
        }

        public async Task<ApiResult<bool>> RecordDiagnosisAsync(int ticketId, RecordDiagnosisRequest request) // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'RecordDiagnosisAsync' nhận tham số (ticketId, request) trả về kết quả kiểu Task<ApiResult<bool>>.
        {
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var response = await _httpClient.PutAsJsonAsync($"api/service-tickets/{ticketId}/diagnosis", request); // Gọi phương thức PUT (cập nhật dữ liệu JSON) bất đồng bộ tới URL 'api/service-tickets/{ticketId}/diagnosis' và gán kết quả cho biến 'response'.
                return await response.Content.ReadFromJsonAsync<ApiResult<bool>>() // Trả về giá trị của biểu thức 'await response.Content.ReadFromJsonAsync<ApiResult<bool>>()'.
                    ?? new ApiResult<bool> { Message = "Lỗi ghi kết luận chẩn đoán." }; // Thực hiện gán giá trị của biểu thức '"Lỗi ghi kết luận chẩn đoán." }' cho biến/thuộc tính '?? new ApiResult<bool> { Message'.
            }
            catch (Exception ex) // Khối bắt lỗi ngoại lệ Exception nếu xảy ra lỗi trong quá trình xử lý.
            {
                return new ApiResult<bool> { Message = $"Lỗi: {ex.Message}" }; // Trả về giá trị của biểu thức 'new ApiResult<bool> { Message = $"Lỗi: {ex.Message}" }'.
            }
        }

        public async Task<ApiResult<bool>> ChooseBranchAsync(int ticketId, ChooseBranchRequest request) // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'ChooseBranchAsync' nhận tham số (ticketId, request) trả về kết quả kiểu Task<ApiResult<bool>>.
        {
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var response = await _httpClient.PutAsJsonAsync($"api/service-tickets/{ticketId}/branch", request); // Gọi phương thức PUT (cập nhật dữ liệu JSON) bất đồng bộ tới URL 'api/service-tickets/{ticketId}/branch' và gán kết quả cho biến 'response'.
                return await response.Content.ReadFromJsonAsync<ApiResult<bool>>() // Trả về giá trị của biểu thức 'await response.Content.ReadFromJsonAsync<ApiResult<bool>>()'.
                    ?? new ApiResult<bool> { Message = "Lỗi chọn loại sửa chữa." }; // Thực hiện gán giá trị của biểu thức '"Lỗi chọn loại sửa chữa." }' cho biến/thuộc tính '?? new ApiResult<bool> { Message'.
            }
            catch (Exception ex) // Khối bắt lỗi ngoại lệ Exception nếu xảy ra lỗi trong quá trình xử lý.
            {
                return new ApiResult<bool> { Message = $"Lỗi: {ex.Message}" }; // Trả về giá trị của biểu thức 'new ApiResult<bool> { Message = $"Lỗi: {ex.Message}" }'.
            }
        }

        public async Task<ApiResult<QuotationDto>> CreateQuotationAsync(int ticketId, CreateQuotationRequest request) // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'CreateQuotationAsync' nhận tham số (ticketId, request) trả về kết quả kiểu Task<ApiResult<QuotationDto>>.
        {
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var response = await _httpClient.PostAsJsonAsync($"api/service-tickets/{ticketId}/quotation", request); // Gọi phương thức POST (gửi dữ liệu JSON) bất đồng bộ tới URL 'api/service-tickets/{ticketId}/quotation' và gán kết quả cho biến 'response'.
                return await response.Content.ReadFromJsonAsync<ApiResult<QuotationDto>>() // Trả về giá trị của biểu thức 'await response.Content.ReadFromJsonAsync<ApiResult<QuotationDto>>()'.
                    ?? new ApiResult<QuotationDto> { Message = "Lỗi tạo báo giá." }; // Thực hiện gán giá trị của biểu thức '"Lỗi tạo báo giá." }' cho biến/thuộc tính '?? new ApiResult<QuotationDto> { Message'.
            }
            catch (Exception ex) // Khối bắt lỗi ngoại lệ Exception nếu xảy ra lỗi trong quá trình xử lý.
            {
                return new ApiResult<QuotationDto> { Message = $"Lỗi: {ex.Message}" }; // Trả về giá trị của biểu thức 'new ApiResult<QuotationDto> { Message = $"Lỗi: {ex.Message}" }'.
            }
        }

        public async Task<ApiResult<bool>> AcceptQuotationAsync(int ticketId, int quotationId, AcceptQuotationRequest request) // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'AcceptQuotationAsync' nhận tham số (ticketId, quotationId, request) trả về kết quả kiểu Task<ApiResult<bool>>.
        {
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var response = await _httpClient.PostAsJsonAsync($"api/service-tickets/{ticketId}/quotation/{quotationId}/accept", request); // Gọi phương thức POST (gửi dữ liệu JSON) bất đồng bộ tới URL 'api/service-tickets/{ticketId}/quotation/{quotationId}/accept' và gán kết quả cho biến 'response'.
                return await response.Content.ReadFromJsonAsync<ApiResult<bool>>() // Trả về giá trị của biểu thức 'await response.Content.ReadFromJsonAsync<ApiResult<bool>>()'.
                    ?? new ApiResult<bool> { Message = "Lỗi duyệt báo giá." }; // Thực hiện gán giá trị của biểu thức '"Lỗi duyệt báo giá." }' cho biến/thuộc tính '?? new ApiResult<bool> { Message'.
            }
            catch (Exception ex) // Khối bắt lỗi ngoại lệ Exception nếu xảy ra lỗi trong quá trình xử lý.
            {
                return new ApiResult<bool> { Message = $"Lỗi: {ex.Message}" }; // Trả về giá trị của biểu thức 'new ApiResult<bool> { Message = $"Lỗi: {ex.Message}" }'.
            }
        }

        public async Task<ApiResult<bool>> RejectQuotationAsync(int ticketId, int quotationId, RejectQuotationRequest request) // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'RejectQuotationAsync' nhận tham số (ticketId, quotationId, request) trả về kết quả kiểu Task<ApiResult<bool>>.
        {
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var response = await _httpClient.PostAsJsonAsync($"api/service-tickets/{ticketId}/quotation/{quotationId}/reject", request); // Gọi phương thức POST (gửi dữ liệu JSON) bất đồng bộ tới URL 'api/service-tickets/{ticketId}/quotation/{quotationId}/reject' và gán kết quả cho biến 'response'.
                return await response.Content.ReadFromJsonAsync<ApiResult<bool>>() // Trả về giá trị của biểu thức 'await response.Content.ReadFromJsonAsync<ApiResult<bool>>()'.
                    ?? new ApiResult<bool> { Message = "Lỗi từ chối báo giá." }; // Thực hiện gán giá trị của biểu thức '"Lỗi từ chối báo giá." }' cho biến/thuộc tính '?? new ApiResult<bool> { Message'.
            }
            catch (Exception ex) // Khối bắt lỗi ngoại lệ Exception nếu xảy ra lỗi trong quá trình xử lý.
            {
                return new ApiResult<bool> { Message = $"Lỗi: {ex.Message}" }; // Trả về giá trị của biểu thức 'new ApiResult<bool> { Message = $"Lỗi: {ex.Message}" }'.
            }
        }

        public async Task<ApiResult<RmaShipmentDto>> CreateRmaShipmentAsync(int ticketId, CreateRmaShipmentRequest request) // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'CreateRmaShipmentAsync' nhận tham số (ticketId, request) trả về kết quả kiểu Task<ApiResult<RmaShipmentDto>>.
        {
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var response = await _httpClient.PostAsJsonAsync($"api/service-tickets/{ticketId}/rma", request); // Gọi phương thức POST (gửi dữ liệu JSON) bất đồng bộ tới URL 'api/service-tickets/{ticketId}/rma' và gán kết quả cho biến 'response'.
                return await response.Content.ReadFromJsonAsync<ApiResult<RmaShipmentDto>>() // Trả về giá trị của biểu thức 'await response.Content.ReadFromJsonAsync<ApiResult<RmaShipmentDto>>()'.
                    ?? new ApiResult<RmaShipmentDto> { Message = "Lỗi tạo phiếu gửi hãng." }; // Thực hiện gán giá trị của biểu thức '"Lỗi tạo phiếu gửi hãng." }' cho biến/thuộc tính '?? new ApiResult<RmaShipmentDto> { Message'.
            }
            catch (Exception ex) // Khối bắt lỗi ngoại lệ Exception nếu xảy ra lỗi trong quá trình xử lý.
            {
                return new ApiResult<RmaShipmentDto> { Message = $"Lỗi: {ex.Message}" }; // Trả về giá trị của biểu thức 'new ApiResult<RmaShipmentDto> { Message = $"Lỗi: {ex.Message}" }'.
            }
        }

        public async Task<ApiResult<bool>> RecordRmaResolutionAsync(int ticketId, UpdateRmaResolutionRequest request) // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'RecordRmaResolutionAsync' nhận tham số (ticketId, request) trả về kết quả kiểu Task<ApiResult<bool>>.
        {
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var response = await _httpClient.PutAsJsonAsync($"api/service-tickets/{ticketId}/rma/resolution", request); // Gọi phương thức PUT (cập nhật dữ liệu JSON) bất đồng bộ tới URL 'api/service-tickets/{ticketId}/rma/resolution' và gán kết quả cho biến 'response'.
                return await response.Content.ReadFromJsonAsync<ApiResult<bool>>() // Trả về giá trị của biểu thức 'await response.Content.ReadFromJsonAsync<ApiResult<bool>>()'.
                    ?? new ApiResult<bool> { Message = "Lỗi cập nhật kết quả RMA." }; // Thực hiện gán giá trị của biểu thức '"Lỗi cập nhật kết quả RMA." }' cho biến/thuộc tính '?? new ApiResult<bool> { Message'.
            }
            catch (Exception ex) // Khối bắt lỗi ngoại lệ Exception nếu xảy ra lỗi trong quá trình xử lý.
            {
                return new ApiResult<bool> { Message = $"Lỗi: {ex.Message}" }; // Trả về giá trị của biểu thức 'new ApiResult<bool> { Message = $"Lỗi: {ex.Message}" }'.
            }
        }

        public async Task<ApiResult<bool>> Perform1For1SwapAsync(int ticketId, Perform1For1SwapRequest request) // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'Perform1For1SwapAsync' nhận tham số (ticketId, request) trả về kết quả kiểu Task<ApiResult<bool>>.
        {
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var response = await _httpClient.PostAsJsonAsync($"api/service-tickets/{ticketId}/swap", request); // Gọi phương thức POST (gửi dữ liệu JSON) bất đồng bộ tới URL 'api/service-tickets/{ticketId}/swap' và gán kết quả cho biến 'response'.
                return await response.Content.ReadFromJsonAsync<ApiResult<bool>>() // Trả về giá trị của biểu thức 'await response.Content.ReadFromJsonAsync<ApiResult<bool>>()'.
                    ?? new ApiResult<bool> { Message = "Lỗi thực hiện đổi 1-1." }; // Thực hiện gán giá trị của biểu thức '"Lỗi thực hiện đổi 1-1." }' cho biến/thuộc tính '?? new ApiResult<bool> { Message'.
            }
            catch (Exception ex) // Khối bắt lỗi ngoại lệ Exception nếu xảy ra lỗi trong quá trình xử lý.
            {
                return new ApiResult<bool> { Message = $"Lỗi: {ex.Message}" }; // Trả về giá trị của biểu thức 'new ApiResult<bool> { Message = $"Lỗi: {ex.Message}" }'.
            }
        }

        public async Task<ApiResult<bool>> StartRepairAsync(int ticketId) // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'StartRepairAsync' nhận tham số (ticketId) trả về kết quả kiểu Task<ApiResult<bool>>.
        {
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var response = await _httpClient.PostAsync($"api/service-tickets/{ticketId}/start-repair", null); // Gọi phương thức POST bất đồng bộ tới URL 'api/service-tickets/{ticketId}/start-repair' và gán kết quả cho biến 'response'.
                if (!response.IsSuccessStatusCode) // Kiểm tra xem điều kiện '!response.IsSuccessStatusCode' có thỏa mãn hay không.
                    return new ApiResult<bool> { Message = $"Lỗi HTTP {(int)response.StatusCode}." }; // Trả về giá trị của biểu thức 'new ApiResult<bool> { Message = $"Lỗi HTTP {(int)response.StatusCode}." }'.
                return await response.Content.ReadFromJsonAsync<ApiResult<bool>>() // Trả về giá trị của biểu thức 'await response.Content.ReadFromJsonAsync<ApiResult<bool>>()'.
                    ?? new ApiResult<bool> { Message = "Lỗi bắt đầu sửa chữa." }; // Thực hiện gán giá trị của biểu thức '"Lỗi bắt đầu sửa chữa." }' cho biến/thuộc tính '?? new ApiResult<bool> { Message'.
            }
            catch (Exception ex) // Khối bắt lỗi ngoại lệ Exception nếu xảy ra lỗi trong quá trình xử lý.
            {
                return new ApiResult<bool> { Message = $"Lỗi: {ex.Message}" }; // Trả về giá trị của biểu thức 'new ApiResult<bool> { Message = $"Lỗi: {ex.Message}" }'.
            }
        }

        public async Task<ApiResult<bool>> MarkWaitingPartsAsync(int ticketId) // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'MarkWaitingPartsAsync' nhận tham số (ticketId) trả về kết quả kiểu Task<ApiResult<bool>>.
        {
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var response = await _httpClient.PostAsync($"api/service-tickets/{ticketId}/waiting-parts", null); // Gọi phương thức POST bất đồng bộ tới URL 'api/service-tickets/{ticketId}/waiting-parts' và gán kết quả cho biến 'response'.
                if (!response.IsSuccessStatusCode) // Kiểm tra xem điều kiện '!response.IsSuccessStatusCode' có thỏa mãn hay không.
                    return new ApiResult<bool> { Message = $"Lỗi HTTP {(int)response.StatusCode}." }; // Trả về giá trị của biểu thức 'new ApiResult<bool> { Message = $"Lỗi HTTP {(int)response.StatusCode}." }'.
                return await response.Content.ReadFromJsonAsync<ApiResult<bool>>() // Trả về giá trị của biểu thức 'await response.Content.ReadFromJsonAsync<ApiResult<bool>>()'.
                    ?? new ApiResult<bool> { Message = "Lỗi ghi nhận chờ phụ tùng." }; // Thực hiện gán giá trị của biểu thức '"Lỗi ghi nhận chờ phụ tùng." }' cho biến/thuộc tính '?? new ApiResult<bool> { Message'.
            }
            catch (Exception ex) // Khối bắt lỗi ngoại lệ Exception nếu xảy ra lỗi trong quá trình xử lý.
            {
                return new ApiResult<bool> { Message = $"Lỗi: {ex.Message}" }; // Trả về giá trị của biểu thức 'new ApiResult<bool> { Message = $"Lỗi: {ex.Message}" }'.
            }
        }

        public async Task<ApiResult<bool>> ResumeRepairAsync(int ticketId) // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'ResumeRepairAsync' nhận tham số (ticketId) trả về kết quả kiểu Task<ApiResult<bool>>.
        {
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var response = await _httpClient.PostAsync($"api/service-tickets/{ticketId}/resume-repair", null); // Gọi phương thức POST bất đồng bộ tới URL 'api/service-tickets/{ticketId}/resume-repair' và gán kết quả cho biến 'response'.
                if (!response.IsSuccessStatusCode) // Kiểm tra xem điều kiện '!response.IsSuccessStatusCode' có thỏa mãn hay không.
                    return new ApiResult<bool> { Message = $"Lỗi HTTP {(int)response.StatusCode}." }; // Trả về giá trị của biểu thức 'new ApiResult<bool> { Message = $"Lỗi HTTP {(int)response.StatusCode}." }'.
                return await response.Content.ReadFromJsonAsync<ApiResult<bool>>() // Trả về giá trị của biểu thức 'await response.Content.ReadFromJsonAsync<ApiResult<bool>>()'.
                    ?? new ApiResult<bool> { Message = "Lỗi tiếp tục sửa chữa." }; // Thực hiện gán giá trị của biểu thức '"Lỗi tiếp tục sửa chữa." }' cho biến/thuộc tính '?? new ApiResult<bool> { Message'.
            }
            catch (Exception ex) // Khối bắt lỗi ngoại lệ Exception nếu xảy ra lỗi trong quá trình xử lý.
            {
                return new ApiResult<bool> { Message = $"Lỗi: {ex.Message}" }; // Trả về giá trị của biểu thức 'new ApiResult<bool> { Message = $"Lỗi: {ex.Message}" }'.
            }
        }

        public async Task<ApiResult<bool>> CompleteRepairAsync(int ticketId, CompleteRepairRequest request) // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'CompleteRepairAsync' nhận tham số (ticketId, request) trả về kết quả kiểu Task<ApiResult<bool>>.
        {
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var response = await _httpClient.PostAsJsonAsync($"api/service-tickets/{ticketId}/complete", request); // Gọi phương thức POST (gửi dữ liệu JSON) bất đồng bộ tới URL 'api/service-tickets/{ticketId}/complete' và gán kết quả cho biến 'response'.
                return await response.Content.ReadFromJsonAsync<ApiResult<bool>>() // Trả về giá trị của biểu thức 'await response.Content.ReadFromJsonAsync<ApiResult<bool>>()'.
                    ?? new ApiResult<bool> { Message = "Lỗi hoàn tất sửa chữa." }; // Thực hiện gán giá trị của biểu thức '"Lỗi hoàn tất sửa chữa." }' cho biến/thuộc tính '?? new ApiResult<bool> { Message'.
            }
            catch (Exception ex) // Khối bắt lỗi ngoại lệ Exception nếu xảy ra lỗi trong quá trình xử lý.
            {
                return new ApiResult<bool> { Message = $"Lỗi: {ex.Message}" }; // Trả về giá trị của biểu thức 'new ApiResult<bool> { Message = $"Lỗi: {ex.Message}" }'.
            }
        }

        public async Task<ApiResult<ServiceInvoiceDetailDto>> IssueServiceInvoiceAsync(int ticketId, IssueServiceInvoiceRequest request) // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'IssueServiceInvoiceAsync' nhận tham số (ticketId, request) trả về kết quả kiểu Task<ApiResult<ServiceInvoiceDetailDto>>.
        {
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var response = await _httpClient.PostAsJsonAsync($"api/service-tickets/{ticketId}/invoice", request); // Gọi phương thức POST (gửi dữ liệu JSON) bất đồng bộ tới URL 'api/service-tickets/{ticketId}/invoice' và gán kết quả cho biến 'response'.
                return await response.Content.ReadFromJsonAsync<ApiResult<ServiceInvoiceDetailDto>>() // Trả về giá trị của biểu thức 'await response.Content.ReadFromJsonAsync<ApiResult<ServiceInvoiceDetailDto>>()'.
                    ?? new ApiResult<ServiceInvoiceDetailDto> { Message = "Lỗi phát hành hóa đơn dịch vụ." }; // Thực hiện gán giá trị của biểu thức '"Lỗi phát hành hóa đơn dịch vụ." }' cho biến/thuộc tính '?? new ApiResult<ServiceInvoiceDetailDto> { Message'.
            }
            catch (Exception ex) // Khối bắt lỗi ngoại lệ Exception nếu xảy ra lỗi trong quá trình xử lý.
            {
                return new ApiResult<ServiceInvoiceDetailDto> { Message = $"Lỗi: {ex.Message}" }; // Trả về giá trị của biểu thức 'new ApiResult<ServiceInvoiceDetailDto> { Message = $"Lỗi: {ex.Message}" }'.
            }
        }

        public async Task<ApiResult<bool>> CancelTicketAsync(int ticketId, CancelTicketRequest request) // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'CancelTicketAsync' nhận tham số (ticketId, request) trả về kết quả kiểu Task<ApiResult<bool>>.
        {
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var response = await _httpClient.PostAsJsonAsync($"api/service-tickets/{ticketId}/cancel", request); // Gọi phương thức POST (gửi dữ liệu JSON) bất đồng bộ tới URL 'api/service-tickets/{ticketId}/cancel' và gán kết quả cho biến 'response'.
                return await response.Content.ReadFromJsonAsync<ApiResult<bool>>() // Trả về giá trị của biểu thức 'await response.Content.ReadFromJsonAsync<ApiResult<bool>>()'.
                    ?? new ApiResult<bool> { Message = "Lỗi hủy phiếu." }; // Thực hiện gán giá trị của biểu thức '"Lỗi hủy phiếu." }' cho biến/thuộc tính '?? new ApiResult<bool> { Message'.
            }
            catch (Exception ex) // Khối bắt lỗi ngoại lệ Exception nếu xảy ra lỗi trong quá trình xử lý.
            {
                return new ApiResult<bool> { Message = $"Lỗi: {ex.Message}" }; // Trả về giá trị của biểu thức 'new ApiResult<bool> { Message = $"Lỗi: {ex.Message}" }'.
            }
        }

        public async Task<ApiResult<List<ServiceTicketStatusHistoryDto>>> GetHistoryAsync(int ticketId) // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'GetHistoryAsync' nhận tham số (ticketId) trả về kết quả kiểu Task<ApiResult<List<ServiceTicketStatusHistoryDto>>>.
        {
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var response = await _httpClient.GetAsync($"api/service-tickets/{ticketId}/history"); // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'response'.
                return await response.Content.ReadFromJsonAsync<ApiResult<List<ServiceTicketStatusHistoryDto>>>() // Trả về giá trị của biểu thức 'await response.Content.ReadFromJsonAsync<ApiResult<List<ServiceTicketStatusHistoryDto>>>()'.
                    ?? new ApiResult<List<ServiceTicketStatusHistoryDto>> { Message = "Lỗi lấy lịch sử." }; // Thực hiện gán giá trị của biểu thức '"Lỗi lấy lịch sử." }' cho biến/thuộc tính '?? new ApiResult<List<ServiceTicketStatusHistoryDto>> { Message'.
            }
            catch (Exception ex) // Khối bắt lỗi ngoại lệ Exception nếu xảy ra lỗi trong quá trình xử lý.
            {
                return new ApiResult<List<ServiceTicketStatusHistoryDto>> { Message = $"Lỗi: {ex.Message}" }; // Trả về giá trị của biểu thức 'new ApiResult<List<ServiceTicketStatusHistoryDto>> { Message = $"Lỗi: {ex.Message}" }'.
            }
        }

        public async Task<ApiResult<List<SerialRepairLogDto>>> GetSerialRepairHistoryAsync(string serialNumber) // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'GetSerialRepairHistoryAsync' nhận tham số (serialNumber) trả về kết quả kiểu Task<ApiResult<List<SerialRepairLogDto>>>.
        {
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var response = await _httpClient.GetAsync($"api/service-tickets/serials/{Uri.EscapeDataString(serialNumber)}/repair-history"); // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'response'.
                return await response.Content.ReadFromJsonAsync<ApiResult<List<SerialRepairLogDto>>>() // Trả về giá trị của biểu thức 'await response.Content.ReadFromJsonAsync<ApiResult<List<SerialRepairLogDto>>>()'.
                    ?? new ApiResult<List<SerialRepairLogDto>> { Message = "Lỗi lấy lịch sử sửa chữa." }; // Thực hiện gán giá trị của biểu thức '"Lỗi lấy lịch sử sửa chữa." }' cho biến/thuộc tính '?? new ApiResult<List<SerialRepairLogDto>> { Message'.
            }
            catch (Exception ex) // Khối bắt lỗi ngoại lệ Exception nếu xảy ra lỗi trong quá trình xử lý.
            {
                return new ApiResult<List<SerialRepairLogDto>> { Message = $"Lỗi: {ex.Message}" }; // Trả về giá trị của biểu thức 'new ApiResult<List<SerialRepairLogDto>> { Message = $"Lỗi: {ex.Message}" }'.
            }
        }
    }
}

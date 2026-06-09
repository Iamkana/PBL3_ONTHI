using System.Net.Http; // Nhập (import) namespace System.Net.Http để sử dụng các lớp bên trong.
using System.Net.Http.Json; // Sử dụng thư viện hỗ trợ gửi nhận dữ liệu JSON qua HTTP.
using System.Threading.Tasks; // Nhập (import) namespace System.Threading.Tasks để sử dụng các lớp bên trong.
using System.Collections.Generic; // Nhập (import) namespace System.Collections.Generic để sử dụng các lớp bên trong.
using PBL3.Shared.DTOs.Pos; // Sử dụng các DTO của module Pos thuộc tầng Shared.
using PBL3.Shared.DTOs.Common; // Sử dụng các DTO của module Common thuộc tầng Shared.

namespace Client.Services.Pos // Thiết lập namespace Client.Services.Pos để tổ chức quản lý cấu trúc các lớp.
{
    public class PosClientService : IPosClientService // Định nghĩa lớp PosClientService triển khai các dịch vụ hoặc kế thừa từ IPosClientService.
    {
        private readonly HttpClient _httpClient; // Khai báo đối tượng HttpClient nội bộ dùng để thực hiện cuộc gọi API.

        public PosClientService(HttpClient httpClient) // Hàm khởi tạo (Constructor) của lớp PosClientService tiêm các phụ thuộc: httpClient.
        {
            _httpClient = httpClient; // Thực hiện gán giá trị của biểu thức 'httpClient' cho biến/thuộc tính '_httpClient'.
        }

        public async Task<ApiResult<PosScanResponse>> ScanBarcodeAsync(string serialNumber) // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'ScanBarcodeAsync' nhận tham số (serialNumber) trả về kết quả kiểu Task<ApiResult<PosScanResponse>>.
        {
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var request = new PosScanRequest { SerialNumber = serialNumber }; // Thực hiện gán giá trị của biểu thức 'new PosScanRequest { SerialNumber = serialNumber }' cho biến/thuộc tính 'request'.
                var response = await _httpClient.PostAsJsonAsync($"/api/pos/scan", request); // Gọi phương thức POST (gửi dữ liệu JSON) bất đồng bộ tới URL '/api/pos/scan' và gán kết quả cho biến 'response'.
                if (!response.IsSuccessStatusCode) // Kiểm tra xem điều kiện '!response.IsSuccessStatusCode' có thỏa mãn hay không.
                {
                    var err = await response.Content.ReadFromJsonAsync<ApiResult<PosScanResponse>>(); // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'err'.
                    return err ?? ApiResult<PosScanResponse>.Fail($"Lỗi HTTP {(int)response.StatusCode}."); // Trả về giá trị của 'err' nếu khác null, ngược lại trả về 'ApiResult<PosScanResponse>.Fail($"Lỗi HTTP {(int)response.StatusCode}.")'.
                }
                return await response.Content.ReadFromJsonAsync<ApiResult<PosScanResponse>>() // Trả về giá trị của biểu thức 'await response.Content.ReadFromJsonAsync<ApiResult<PosScanResponse>>()'.
                    ?? ApiResult<PosScanResponse>.Fail("Lỗi hệ thống khi quét mã."); // Thực thi dòng lệnh nghiệp vụ.
            }
            catch (Exception ex) // Khối bắt lỗi ngoại lệ Exception nếu xảy ra lỗi trong quá trình xử lý.
            {
                return ApiResult<PosScanResponse>.Fail($"Lỗi kết nối: {ex.Message}"); // Trả về giá trị của biểu thức 'ApiResult<PosScanResponse>.Fail($"Lỗi kết nối: {ex.Message}")'.
            }
        }

        public async Task<ApiResult<PosCustomerDto>> SearchCustomerByPhoneAsync(string phone) // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'SearchCustomerByPhoneAsync' nhận tham số (phone) trả về kết quả kiểu Task<ApiResult<PosCustomerDto>>.
        {
            var response = await _httpClient.GetAsync($"/api/pos/customer?phone={Uri.EscapeDataString(phone)}"); // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'response'.
            if (!response.IsSuccessStatusCode) return new ApiResult<PosCustomerDto> { Success = false, Message = "Không tìm thấy khách hàng." }; // Thực hiện gán giá trị của biểu thức 'false, Message = "Không tìm thấy khách hàng." }' cho biến/thuộc tính 'if (!response.IsSuccessStatusCode) return new ApiResult<PosCustomerDto> { Success'.
            return await response.Content.ReadFromJsonAsync<ApiResult<PosCustomerDto>>() // Trả về giá trị của biểu thức 'await response.Content.ReadFromJsonAsync<ApiResult<PosCustomerDto>>()'.
                ?? ApiResult<PosCustomerDto>.Fail("Lỗi hệ thống khi tìm kiếm khách hàng."); // Thực thi dòng lệnh nghiệp vụ.
        }

        public async Task<ApiResult<VoucherValidationDto>> ValidateVoucherAsync(string code, decimal subTotal) // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'ValidateVoucherAsync' nhận tham số (code, subTotal) trả về kết quả kiểu Task<ApiResult<VoucherValidationDto>>.
        {
            var response = await _httpClient.PostAsync( // Gọi phương thức POST bất đồng bộ tới URL '' và gán kết quả cho biến 'response'.
                $"/api/pos/voucher/validate?code={Uri.EscapeDataString(code)}&subTotal={subTotal}", null); // Thực thi dòng lệnh nghiệp vụ.
            if (!response.IsSuccessStatusCode) return new ApiResult<VoucherValidationDto> { Success = false, Message = "Lỗi khi kiểm tra voucher." }; // Thực hiện gán giá trị của biểu thức 'false, Message = "Lỗi khi kiểm tra voucher." }' cho biến/thuộc tính 'if (!response.IsSuccessStatusCode) return new ApiResult<VoucherValidationDto> { Success'.
            return await response.Content.ReadFromJsonAsync<ApiResult<VoucherValidationDto>>() // Trả về giá trị của biểu thức 'await response.Content.ReadFromJsonAsync<ApiResult<VoucherValidationDto>>()'.
                ?? ApiResult<VoucherValidationDto>.Fail("Lỗi hệ thống khi kiểm tra voucher."); // Thực thi dòng lệnh nghiệp vụ.
        }

        public async Task<ApiResult<PosOrderDto>> CheckoutAsync(PosCheckoutRequest request) // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'CheckoutAsync' nhận tham số (request) trả về kết quả kiểu Task<ApiResult<PosOrderDto>>.
        {
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var response = await _httpClient.PostAsJsonAsync("/api/pos/checkout", request); // Gọi phương thức POST (gửi dữ liệu JSON) bất đồng bộ tới URL '/api/pos/checkout' và gán kết quả cho biến 'response'.
                if (!response.IsSuccessStatusCode) // Kiểm tra xem điều kiện '!response.IsSuccessStatusCode' có thỏa mãn hay không.
                {
                    var err = await response.Content.ReadFromJsonAsync<ApiResult<PosOrderDto>>(); // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'err'.
                    return err ?? ApiResult<PosOrderDto>.Fail($"Lỗi HTTP {(int)response.StatusCode}."); // Trả về giá trị của 'err' nếu khác null, ngược lại trả về 'ApiResult<PosOrderDto>.Fail($"Lỗi HTTP {(int)response.StatusCode}.")'.
                }
                return await response.Content.ReadFromJsonAsync<ApiResult<PosOrderDto>>() // Trả về giá trị của biểu thức 'await response.Content.ReadFromJsonAsync<ApiResult<PosOrderDto>>()'.
                    ?? ApiResult<PosOrderDto>.Fail("Lỗi hệ thống khi thanh toán."); // Thực thi dòng lệnh nghiệp vụ.
            }
            catch (Exception ex) // Khối bắt lỗi ngoại lệ Exception nếu xảy ra lỗi trong quá trình xử lý.
            {
                return ApiResult<PosOrderDto>.Fail($"Lỗi kết nối: {ex.Message}"); // Trả về giá trị của biểu thức 'ApiResult<PosOrderDto>.Fail($"Lỗi kết nối: {ex.Message}")'.
            }
        }

        public async Task<ApiResult<int>> SaveDraftAsync(PosCheckoutRequest request) // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'SaveDraftAsync' nhận tham số (request) trả về kết quả kiểu Task<ApiResult<int>>.
        {
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var response = await _httpClient.PostAsJsonAsync("/api/pos/draft", request); // Gọi phương thức POST (gửi dữ liệu JSON) bất đồng bộ tới URL '/api/pos/draft' và gán kết quả cho biến 'response'.
                if (!response.IsSuccessStatusCode) // Kiểm tra xem điều kiện '!response.IsSuccessStatusCode' có thỏa mãn hay không.
                    return ApiResult<int>.Fail($"Lỗi HTTP {(int)response.StatusCode}."); // Trả về giá trị của biểu thức 'ApiResult<int>.Fail($"Lỗi HTTP {(int)response.StatusCode}.")'.
                return await response.Content.ReadFromJsonAsync<ApiResult<int>>() // Trả về giá trị của biểu thức 'await response.Content.ReadFromJsonAsync<ApiResult<int>>()'.
                    ?? ApiResult<int>.Fail("Lỗi hệ thống khi lưu tạm."); // Thực thi dòng lệnh nghiệp vụ.
            }
            catch (Exception ex) // Khối bắt lỗi ngoại lệ Exception nếu xảy ra lỗi trong quá trình xử lý.
            {
                return ApiResult<int>.Fail($"Lỗi kết nối: {ex.Message}"); // Trả về giá trị của biểu thức 'ApiResult<int>.Fail($"Lỗi kết nối: {ex.Message}")'.
            }
        }

        public async Task<ApiResult<List<PosDraftDto>>> GetDraftsAsync() // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'GetDraftsAsync' không tham số trả về kết quả kiểu Task<ApiResult<List<PosDraftDto>>>.
        {
            return await _httpClient.GetFromJsonAsync<ApiResult<List<PosDraftDto>>>("/api/pos/drafts") // Gọi phương thức GET bất đồng bộ tới URL '/api/pos/drafts' nhận kết quả kiểu ApiResult<List<PosDraftDto>>.
                ?? ApiResult<List<PosDraftDto>>.Fail("Lỗi hệ thống khi lấy danh sách đơn chờ."); // Thực thi dòng lệnh nghiệp vụ.
        }
    }
}

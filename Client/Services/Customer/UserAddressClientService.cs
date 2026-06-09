using System.Collections.Generic; // Nhập (import) namespace System.Collections.Generic để sử dụng các lớp bên trong.
using System.Net.Http; // Nhập (import) namespace System.Net.Http để sử dụng các lớp bên trong.
using System.Net.Http.Json; // Sử dụng thư viện hỗ trợ gửi nhận dữ liệu JSON qua HTTP.
using System.Threading.Tasks; // Nhập (import) namespace System.Threading.Tasks để sử dụng các lớp bên trong.
using PBL3.Shared.DTOs.Common; // Sử dụng các DTO của module Common thuộc tầng Shared.
using PBL3.Shared.DTOs.Customers; // Sử dụng các DTO của module Customers thuộc tầng Shared.

namespace Client.Services.Customer // Thiết lập namespace Client.Services.Customer để tổ chức quản lý cấu trúc các lớp.
{
    public class UserAddressClientService : IUserAddressClientService // Định nghĩa lớp UserAddressClientService triển khai các dịch vụ hoặc kế thừa từ IUserAddressClientService.
    {
        private readonly HttpClient _httpClient; // Khai báo đối tượng HttpClient nội bộ dùng để thực hiện cuộc gọi API.

        public UserAddressClientService(HttpClient httpClient) // Hàm khởi tạo (Constructor) của lớp UserAddressClientService tiêm các phụ thuộc: httpClient.
        {
            _httpClient = httpClient; // Thực hiện gán giá trị của biểu thức 'httpClient' cho biến/thuộc tính '_httpClient'.
        }

        public async Task<ApiResult<List<UserAddressDto>>> GetMyAddressesAsync() // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'GetMyAddressesAsync' không tham số trả về kết quả kiểu Task<ApiResult<List<UserAddressDto>>>.
        {
            var response = await _httpClient.GetAsync("api/storefront/user-addresses/my-addresses"); // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'response'.
            if (response.IsSuccessStatusCode) // Kiểm tra xem điều kiện 'response.IsSuccessStatusCode' có thỏa mãn hay không.
            {
                var result = await response.Content.ReadFromJsonAsync<ApiResult<List<UserAddressDto>>>(); // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'result'.
                return result ?? ApiResult<List<UserAddressDto>>.Fail("Không thể parse dữ liệu."); // Trả về giá trị của 'result' nếu khác null, ngược lại trả về 'ApiResult<List<UserAddressDto>>.Fail("Không thể parse dữ liệu.")'.
            }

            return ApiResult<List<UserAddressDto>>.Fail($"Lỗi gọi API: {response.ReasonPhrase}"); // Trả về giá trị của biểu thức 'ApiResult<List<UserAddressDto>>.Fail($"Lỗi gọi API: {response.ReasonPhrase}")'.
        }

        public async Task<ApiResult<int>> AddAddressAsync(UserAddressDto request) // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'AddAddressAsync' nhận tham số (request) trả về kết quả kiểu Task<ApiResult<int>>.
        {
            var response = await _httpClient.PostAsJsonAsync("api/storefront/user-addresses", request); // Gọi phương thức POST (gửi dữ liệu JSON) bất đồng bộ tới URL 'api/storefront/user-addresses' và gán kết quả cho biến 'response'.
            if (response.IsSuccessStatusCode) // Kiểm tra xem điều kiện 'response.IsSuccessStatusCode' có thỏa mãn hay không.
            {
                var result = await response.Content.ReadFromJsonAsync<ApiResult<int>>(); // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'result'.
                return result ?? ApiResult<int>.Fail("Không thể parse kết quả."); // Trả về giá trị của 'result' nếu khác null, ngược lại trả về 'ApiResult<int>.Fail("Không thể parse kết quả.")'.
            }
            return ApiResult<int>.Fail($"Lỗi: {response.ReasonPhrase}"); // Trả về giá trị của biểu thức 'ApiResult<int>.Fail($"Lỗi: {response.ReasonPhrase}")'.
        }
    }
}

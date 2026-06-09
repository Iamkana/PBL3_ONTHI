using System.Net.Http.Json; // Sử dụng thư viện hỗ trợ gửi nhận dữ liệu JSON qua HTTP.
using Blazored.LocalStorage; // Sử dụng dịch vụ lưu trữ dữ liệu Local Storage của trình duyệt.
using Client.Auth; // Sử dụng namespace chứa cấu hình xác thực/phân quyền.
using Microsoft.AspNetCore.Components; // Sử dụng các component cốt lõi của Blazor.
using Microsoft.AspNetCore.Components.Authorization; // Sử dụng thư viện quản lý trạng thái xác thực người dùng.
using PBL3.Shared.DTOs.Auth; // Sử dụng các DTO của module Auth thuộc tầng Shared.
using PBL3.Shared.DTOs.Common; // Sử dụng các DTO của module Common thuộc tầng Shared.
using PBL3.Shared.DTOs.Customers; // Sử dụng các DTO của module Customers thuộc tầng Shared.

namespace Client.Services.Auth // Thiết lập namespace Client.Services.Auth để tổ chức quản lý cấu trúc các lớp.
{
    public class AuthClientService : IAuthClientService // Định nghĩa lớp AuthClientService triển khai các dịch vụ hoặc kế thừa từ IAuthClientService.
    {
        private readonly HttpClient _httpClient; // Khai báo đối tượng HttpClient nội bộ dùng để thực hiện cuộc gọi API.
        private readonly ILocalStorageService _localStorage; // Khai báo biến dịch vụ LocalStorage để lưu trữ và đọc dữ liệu tại client.
        private readonly AuthenticationStateProvider _authStateProvider; // Thực thi dòng lệnh nghiệp vụ.
        private readonly NavigationManager _navigationManager; // Khai báo dịch vụ NavigationManager hỗ trợ điều hướng trang.
        private const string BaseUrl = "api/auth"; // Khai báo hằng số BaseUrl có giá trị là "api/auth".
        private const string TokenKey = "authToken"; // Khai báo hằng số TokenKey có giá trị là "authToken".
        private const string RefreshTokenKey = "refreshToken"; // Khai báo hằng số RefreshTokenKey có giá trị là "refreshToken".

        public AuthClientService( // Thực thi dòng lệnh nghiệp vụ.
            HttpClient httpClient, // Thực thi dòng lệnh nghiệp vụ.
            ILocalStorageService localStorage, // Thực thi dòng lệnh nghiệp vụ.
            AuthenticationStateProvider authStateProvider, // Thực thi dòng lệnh nghiệp vụ.
            NavigationManager navigationManager) // Thực thi dòng lệnh nghiệp vụ.
        {
            _httpClient = httpClient; // Thực hiện gán giá trị của biểu thức 'httpClient' cho biến/thuộc tính '_httpClient'.
            _localStorage = localStorage; // Thực hiện gán giá trị của biểu thức 'localStorage' cho biến/thuộc tính '_localStorage'.
            _authStateProvider = authStateProvider; // Thực hiện gán giá trị của biểu thức 'authStateProvider' cho biến/thuộc tính '_authStateProvider'.
            _navigationManager = navigationManager; // Thực hiện gán giá trị của biểu thức 'navigationManager' cho biến/thuộc tính '_navigationManager'.
        }

        public async Task<ApiResult<TokenResponse>> LoginAsync(LoginRequest request) // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'LoginAsync' nhận tham số (request) trả về kết quả kiểu Task<ApiResult<TokenResponse>>.
        {
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/login", request); // Gọi phương thức POST (gửi dữ liệu JSON) bất đồng bộ tới URL '{BaseUrl}/login' và gán kết quả cho biến 'response'.
                var result = await response.Content.ReadFromJsonAsync<ApiResult<TokenResponse>>(); // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'result'.

                if (result != null && result.Success && result.Data != null) // Kiểm tra xem điều kiện 'result != null && result.Success && result.Data != null' có thỏa mãn hay không.
                {
                    // Lưu token vào LocalStorage
                    await _localStorage.SetItemAsStringAsync(TokenKey, result.Data.AccessToken); // Thực thi dòng lệnh nghiệp vụ.
                    await _localStorage.SetItemAsStringAsync(RefreshTokenKey, result.Data.RefreshToken); // Thực thi dòng lệnh nghiệp vụ.

                    // Báo cho AuthStateProvider biết đã đăng nhập
                    ((JwtAuthenticationStateProvider)_authStateProvider).NotifyAuthStateChanged(); // Thực thi dòng lệnh nghiệp vụ.
                }

                return result ?? ApiResult<TokenResponse>.Fail("Đăng nhập thất bại."); // Trả về giá trị của 'result' nếu khác null, ngược lại trả về 'ApiResult<TokenResponse>.Fail("Đăng nhập thất bại.")'.
            }
            catch (Exception ex) // Khối bắt lỗi ngoại lệ Exception nếu xảy ra lỗi trong quá trình xử lý.
            {
                return ApiResult<TokenResponse>.Fail($"Lỗi kết nối: {ex.Message}"); // Trả về giá trị của biểu thức 'ApiResult<TokenResponse>.Fail($"Lỗi kết nối: {ex.Message}")'.
            }
        }

        public async Task LogoutAsync() // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'LogoutAsync' không tham số trả về kết quả kiểu Task.
        {
            // Xóa token khỏi LocalStorage
            await _localStorage.RemoveItemAsync(TokenKey); // Thực thi dòng lệnh nghiệp vụ.
            await _localStorage.RemoveItemAsync(RefreshTokenKey); // Thực thi dòng lệnh nghiệp vụ.

            // Báo cho AuthStateProvider biết đã đăng xuất
            ((JwtAuthenticationStateProvider)_authStateProvider).NotifyAuthStateChanged(); // Thực thi dòng lệnh nghiệp vụ.

            // Redirect về trang đăng nhập
            _navigationManager.NavigateTo("/login"); // Thực thi dòng lệnh nghiệp vụ.
        }

        public async Task<ApiResult<bool>> RegisterAsync(RegisterCustomerRequest request) // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'RegisterAsync' nhận tham số (request) trả về kết quả kiểu Task<ApiResult<bool>>.
        {
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/register", request); // Gọi phương thức POST (gửi dữ liệu JSON) bất đồng bộ tới URL '{BaseUrl}/register' và gán kết quả cho biến 'response'.
                var result = await response.Content.ReadFromJsonAsync<ApiResult<bool>>(); // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'result'.

                return result ?? ApiResult<bool>.Fail("Đăng ký thất bại."); // Trả về giá trị của 'result' nếu khác null, ngược lại trả về 'ApiResult<bool>.Fail("Đăng ký thất bại.")'.
            }
            catch (Exception ex) // Khối bắt lỗi ngoại lệ Exception nếu xảy ra lỗi trong quá trình xử lý.
            {
                return ApiResult<bool>.Fail($"Lỗi kết nối: {ex.Message}"); // Trả về giá trị của biểu thức 'ApiResult<bool>.Fail($"Lỗi kết nối: {ex.Message}")'.
            }
        }

        public async Task<ApiResult<bool>> ChangePasswordAsync(ChangePasswordRequest request) // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'ChangePasswordAsync' nhận tham số (request) trả về kết quả kiểu Task<ApiResult<bool>>.
        {
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/change-password", request); // Gọi phương thức PUT (cập nhật dữ liệu JSON) bất đồng bộ tới URL '{BaseUrl}/change-password' và gán kết quả cho biến 'response'.
                var result = await response.Content.ReadFromJsonAsync<ApiResult<bool>>(); // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'result'.

                return result ?? ApiResult<bool>.Fail("Đổi mật khẩu thất bại."); // Trả về giá trị của 'result' nếu khác null, ngược lại trả về 'ApiResult<bool>.Fail("Đổi mật khẩu thất bại.")'.
            }
            catch (Exception ex) // Khối bắt lỗi ngoại lệ Exception nếu xảy ra lỗi trong quá trình xử lý.
            {
                return ApiResult<bool>.Fail($"Lỗi kết nối: {ex.Message}"); // Trả về giá trị của biểu thức 'ApiResult<bool>.Fail($"Lỗi kết nối: {ex.Message}")'.
            }
        }

        public async Task RefreshSessionAsync() // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'RefreshSessionAsync' không tham số trả về kết quả kiểu Task.
        {
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var accessToken = (await _localStorage.GetItemAsStringAsync(TokenKey))?.Trim('"'); // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'accessToken'.
                var refreshToken = (await _localStorage.GetItemAsStringAsync(RefreshTokenKey))?.Trim('"'); // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'refreshToken'.
                if (string.IsNullOrWhiteSpace(accessToken) || string.IsNullOrWhiteSpace(refreshToken)) // Kiểm tra xem điều kiện 'string.IsNullOrWhiteSpace(accessToken' có thỏa mãn hay không.
                    return; // Thực thi dòng lệnh nghiệp vụ.

                var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/refresh-token", // Gọi phương thức POST (gửi dữ liệu JSON) bất đồng bộ tới URL '' và gán kết quả cho biến 'response'.
                    new RefreshTokenRequest { AccessToken = accessToken, RefreshToken = refreshToken }); // Thực hiện gán giá trị của biểu thức 'accessToken, RefreshToken = refreshToken })' cho biến/thuộc tính 'new RefreshTokenRequest { AccessToken'.

                if (!response.IsSuccessStatusCode) return; // Kiểm tra xem điều kiện '!response.IsSuccessStatusCode' có thỏa mãn hay không.

                var result = await response.Content.ReadFromJsonAsync<ApiResult<TokenResponse>>(); // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'result'.
                if (result?.Success != true || result.Data == null) return; // Kiểm tra xem điều kiện 'result?.Success != true || result.Data == null' có thỏa mãn hay không.

                await _localStorage.SetItemAsStringAsync(TokenKey, result.Data.AccessToken); // Thực thi dòng lệnh nghiệp vụ.
                await _localStorage.SetItemAsStringAsync(RefreshTokenKey, result.Data.RefreshToken); // Thực thi dòng lệnh nghiệp vụ.
                ((JwtAuthenticationStateProvider)_authStateProvider).NotifyAuthStateChanged(); // Thực thi dòng lệnh nghiệp vụ.
            }
            catch // Thực thi dòng lệnh nghiệp vụ.
            {
                // Refresh thất bại — không làm gián đoạn luồng chính
            }
        }
    }
}

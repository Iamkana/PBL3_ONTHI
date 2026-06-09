using System.Net; // Sử dụng thư viện mạng cơ bản của hệ thống.
using System.Net.Http.Headers; // Sử dụng thư viện cấu hình HTTP Headers.
using System.Net.Http.Json; // Sử dụng thư viện hỗ trợ gửi nhận dữ liệu JSON qua HTTP.
using Blazored.LocalStorage; // Sử dụng dịch vụ lưu trữ dữ liệu Local Storage của trình duyệt.
using Microsoft.AspNetCore.Components; // Sử dụng các component cốt lõi của Blazor.
using PBL3.Shared.DTOs.Auth; // Sử dụng các DTO của module Auth thuộc tầng Shared.
using PBL3.Shared.DTOs.Common; // Sử dụng các DTO của module Common thuộc tầng Shared.

namespace Client.Auth; // Thiết lập namespace Client.Auth để tổ chức quản lý cấu trúc các lớp.

public class AuthHeaderHandler : DelegatingHandler // Định nghĩa lớp AuthHeaderHandler triển khai các dịch vụ hoặc kế thừa từ DelegatingHandler.
{
    private readonly ILocalStorageService _localStorage; // Khai báo biến dịch vụ LocalStorage để lưu trữ và đọc dữ liệu tại client.
    private readonly JwtAuthenticationStateProvider _authStateProvider; // Khai báo biến nhà cung cấp trạng thái xác thực JwtAuthenticationStateProvider.
    private readonly NavigationManager _navigationManager; // Khai báo dịch vụ NavigationManager hỗ trợ điều hướng trang.
    private const string TokenKey = "authToken"; // Khai báo hằng số TokenKey có giá trị là "authToken".
    private const string RefreshTokenKey = "refreshToken"; // Khai báo hằng số RefreshTokenKey có giá trị là "refreshToken".

    public AuthHeaderHandler( // Thực thi dòng lệnh nghiệp vụ.
        ILocalStorageService localStorage, // Thực thi dòng lệnh nghiệp vụ.
        JwtAuthenticationStateProvider authStateProvider, // Thực thi dòng lệnh nghiệp vụ.
        NavigationManager navigationManager) // Thực thi dòng lệnh nghiệp vụ.
    {
        _localStorage = localStorage; // Thực hiện gán giá trị của biểu thức 'localStorage' cho biến/thuộc tính '_localStorage'.
        _authStateProvider = authStateProvider; // Thực hiện gán giá trị của biểu thức 'authStateProvider' cho biến/thuộc tính '_authStateProvider'.
        _navigationManager = navigationManager; // Thực hiện gán giá trị của biểu thức 'navigationManager' cho biến/thuộc tính '_navigationManager'.
    }

    protected override async Task<HttpResponseMessage> SendAsync( // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'SendAsync' không tham số trả về kết quả kiểu Task<HttpResponseMessage>.
        HttpRequestMessage request, CancellationToken cancellationToken) // Thực thi dòng lệnh nghiệp vụ.
    {
        var token = await _localStorage.GetItemAsStringAsync(TokenKey); // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'token'.
        if (!string.IsNullOrWhiteSpace(token)) // Kiểm tra xem điều kiện '!string.IsNullOrWhiteSpace(token' có thỏa mãn hay không.
        {
            token = token.Trim('"'); // Thực hiện gán giá trị của biểu thức 'token.Trim('"')' cho biến/thuộc tính 'token'.
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token); // Thực hiện gán giá trị của biểu thức 'new AuthenticationHeaderValue("Bearer", token)' cho biến/thuộc tính 'request.Headers.Authorization'.
        }

        var response = await base.SendAsync(request, cancellationToken); // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'response'.

        // Tài khoản bị khóa → 403 với header X-Account-Status: locked // Chú thích luồng xử lý khi tài khoản bị khóa trên backend.
        if (response.StatusCode == HttpStatusCode.Forbidden && // Kiểm tra xem điều kiện 'điều kiện' có thỏa mãn hay không.
            response.Headers.TryGetValues("X-Account-Status", out var statusValues) && // Thực thi dòng lệnh nghiệp vụ.
            statusValues.FirstOrDefault() == "locked") // Thực thi dòng lệnh nghiệp vụ.
        {
            var result = await response.Content.ReadFromJsonAsync<ApiResult<TokenResponse>>( // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'result'.
                cancellationToken: cancellationToken); // Thực thi dòng lệnh nghiệp vụ.
            await _localStorage.RemoveItemAsync(TokenKey); // Thực thi dòng lệnh nghiệp vụ.
            await _localStorage.RemoveItemAsync(RefreshTokenKey); // Thực thi dòng lệnh nghiệp vụ.
            _authStateProvider.NotifyAuthStateChanged(); // Thực thi dòng lệnh nghiệp vụ.
            var encoded = Uri.EscapeDataString(result?.Message ?? "Tài khoản của bạn đã bị khóa."); // Thực hiện gán giá trị của biểu thức 'Uri.EscapeDataString(result?.Message ?? "Tài khoản của bạn đã bị khóa.")' cho biến/thuộc tính 'encoded'.
            _navigationManager.NavigateTo($"/login?locked=true&reason={encoded}"); // Thực thi dòng lệnh nghiệp vụ.
            return response; // Trả về giá trị của biểu thức 'response'.
        }

        // Token hết hạn hoặc không hợp lệ → về trang đăng nhập // Chú thích xử lý lỗi hết hạn đăng nhập 401 Unauthorized.
        if (response.StatusCode == HttpStatusCode.Unauthorized && // Kiểm tra xem điều kiện 'điều kiện' có thỏa mãn hay không.
            !(request.RequestUri?.AbsolutePath.Contains("/api/auth/") ?? false) && // Thực thi dòng lệnh nghiệp vụ.
            !cancellationToken.IsCancellationRequested) // Thực thi dòng lệnh nghiệp vụ.
        {
            await _localStorage.RemoveItemAsync(TokenKey); // Thực thi dòng lệnh nghiệp vụ.
            await _localStorage.RemoveItemAsync(RefreshTokenKey); // Thực thi dòng lệnh nghiệp vụ.
            _authStateProvider.NotifyAuthStateChanged(); // Thực thi dòng lệnh nghiệp vụ.
            _navigationManager.NavigateTo("/login?expired=true"); // Thực thi dòng lệnh nghiệp vụ.
        }

        return response; // Trả về giá trị của biểu thức 'response'.
    }
}

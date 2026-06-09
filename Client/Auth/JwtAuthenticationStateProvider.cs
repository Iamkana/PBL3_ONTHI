using System.Security.Claims; // Sử dụng thư viện Claims của hệ thống để quản lý quyền hạn.
using System.Text.Json; // Sử dụng thư viện xử lý và tuần tự hóa dữ liệu JSON.
using Blazored.LocalStorage; // Sử dụng dịch vụ lưu trữ dữ liệu Local Storage của trình duyệt.
using Microsoft.AspNetCore.Components.Authorization; // Sử dụng thư viện quản lý trạng thái xác thực người dùng.

namespace Client.Auth; // Thiết lập namespace Client.Auth để tổ chức quản lý cấu trúc các lớp.

/// <summary>
/// Custom AuthenticationStateProvider cho Blazor WASM.
/// Đọc JWT từ LocalStorage, decode Base64 Payload để trích xuất Claims.
/// LƯU Ý: Class này chỉ phục vụ UX (ẩn/hiện menu), KHÔNG có tác dụng bảo mật.
/// Mọi bảo mật thực sự đều nằm ở API Backend.
/// </summary>
public class JwtAuthenticationStateProvider : AuthenticationStateProvider // Định nghĩa lớp JwtAuthenticationStateProvider triển khai các dịch vụ hoặc kế thừa từ AuthenticationStateProvider.
{
    private readonly ILocalStorageService _localStorage; // Khai báo biến dịch vụ LocalStorage để lưu trữ và đọc dữ liệu tại client.
    private const string TokenKey = "authToken"; // Khai báo hằng số TokenKey có giá trị là "authToken".

    public JwtAuthenticationStateProvider(ILocalStorageService localStorage) // Hàm khởi tạo (Constructor) của lớp JwtAuthenticationStateProvider tiêm các phụ thuộc: localStorage.
    {
        _localStorage = localStorage; // Thực hiện gán giá trị của biểu thức 'localStorage' cho biến/thuộc tính '_localStorage'.
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync() // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'GetAuthenticationStateAsync' không tham số trả về kết quả kiểu Task<AuthenticationState>.
    {
        var token = await _localStorage.GetItemAsStringAsync(TokenKey); // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'token'.

        // Nếu không có token hoặc token rỗng -> Trạng thái "Chưa đăng nhập" // Chú thích xử lý khi người dùng chưa đăng nhập.
        if (string.IsNullOrWhiteSpace(token)) // Kiểm tra xem điều kiện 'string.IsNullOrWhiteSpace(token' có thỏa mãn hay không.
        {
            return new AuthenticationState( // Trả về giá trị của biểu thức 'new AuthenticationState('.
                new ClaimsPrincipal(new ClaimsIdentity())); // Thực thi dòng lệnh nghiệp vụ.
        }

        // Loại bỏ dấu ngoặc kép nếu LocalStorage trả về chuỗi có bọc quotes // Chú thích định dạng chuỗi token.
        token = token.Trim('"'); // Thực hiện gán giá trị của biểu thức 'token.Trim('"')' cho biến/thuộc tính 'token'.

        // Parse claims từ JWT payload // Chú thích trích xuất thông tin người dùng.
        var claims = ParseClaimsFromJwt(token); // Thực hiện gán giá trị của biểu thức 'ParseClaimsFromJwt(token)' cho biến/thuộc tính 'claims'.

        // Tạo ClaimsIdentity với scheme "jwt" để Blazor biết user đã authenticated // Chú thích xác thực danh tính.
        var identity = new ClaimsIdentity(claims, "jwt"); // Thực hiện gán giá trị của biểu thức 'new ClaimsIdentity(claims, "jwt")' cho biến/thuộc tính 'identity'.
        var user = new ClaimsPrincipal(identity); // Thực hiện gán giá trị của biểu thức 'new ClaimsPrincipal(identity)' cho biến/thuộc tính 'user'.

        return new AuthenticationState(user); // Trả về giá trị của biểu thức 'new AuthenticationState(user)'.
    }

    /// <summary>
    /// Thông báo cho Blazor rằng trạng thái auth đã thay đổi (gọi sau Login/Logout).
    /// </summary>
    public void NotifyAuthStateChanged() // Thực hiện xử lý phương thức nghiệp vụ 'NotifyAuthStateChanged' không tham số trả về kết quả kiểu void.
    {
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync()); // Thực thi dòng lệnh nghiệp vụ.
    }

    /// <summary>
    /// Parse danh sách Claims từ JWT token string.
    /// JWT có 3 phần: Header.Payload.Signature, ta chỉ cần decode phần Payload (index 1).
    /// </summary>
    private static IEnumerable<Claim> ParseClaimsFromJwt(string jwt) // Thực thi dòng lệnh nghiệp vụ.
    {
        var claims = new List<Claim>(); // Thực hiện gán giá trị của biểu thức 'new List<Claim>()' cho biến/thuộc tính 'claims'.

        // JWT = header.payload.signature // Cấu trúc chuẩn của token JWT gồm 3 phần phân tách bằng dấu chấm.
        var parts = jwt.Split('.'); // Thực hiện gán giá trị của biểu thức 'jwt.Split('.')' cho biến/thuộc tính 'parts'.
        if (parts.Length != 3) // Kiểm tra xem điều kiện 'parts.Length != 3' có thỏa mãn hay không.
        {
            // Token không hợp lệ (không đúng format 3 phần) -> trả về rỗng // Chú thích xử lý token lỗi.
            return claims; // Trả về giá trị của biểu thức 'claims'.
        }

        var payload = parts[1]; // Thực hiện gán giá trị của biểu thức 'parts[1]' cho biến/thuộc tính 'payload'.

        // Decode Base64Url -> JSON bytes // Chú thích decode Base64 phần Payload.
        var jsonBytes = DecodeBase64Url(payload); // Thực hiện gán giá trị của biểu thức 'DecodeBase64Url(payload)' cho biến/thuộc tính 'jsonBytes'.
        if (jsonBytes == null || jsonBytes.Length == 0) // Kiểm tra xem điều kiện 'jsonBytes == null || jsonBytes.Length == 0' có thỏa mãn hay không.
        {
            return claims; // Trả về giá trị của biểu thức 'claims'.
        }

        // Parse JSON payload thành dictionary // Chú thích chuyển đổi JSON sang từ điển dữ liệu.
        var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(jsonBytes); // Thực hiện gán giá trị của biểu thức 'JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(jsonBytes)' cho biến/thuộc tính 'keyValuePairs'.
        if (keyValuePairs == null) // Kiểm tra xem điều kiện 'keyValuePairs == null' có thỏa mãn hay không.
        {
            return claims; // Trả về giá trị của biểu thức 'claims'.
        }

        // Xử lý claim "role" đặc biệt (có thể là string hoặc array) // Chú thích xử lý phân quyền người dùng.
        if (keyValuePairs.TryGetValue(ClaimTypes.Role, out var roles)) // Kiểm tra xem điều kiện 'keyValuePairs.TryGetValue(ClaimTypes.Role, out var roles' có thỏa mãn hay không.
        {
            if (roles.ValueKind == JsonValueKind.Array) // Kiểm tra xem điều kiện 'roles.ValueKind == JsonValueKind.Array' có thỏa mãn hay không.
            {
                foreach (var role in roles.EnumerateArray()) // Thực thi dòng lệnh nghiệp vụ.
                {
                    claims.Add(new Claim(ClaimTypes.Role, role.GetString()!)); // Thực thi dòng lệnh nghiệp vụ.
                }
            }
            else // Nhánh rẽ mặc định khi tất cả các điều kiện trước đều sai.
            {
                claims.Add(new Claim(ClaimTypes.Role, roles.GetString()!)); // Thực thi dòng lệnh nghiệp vụ.
            }

            keyValuePairs.Remove(ClaimTypes.Role); // Thực thi dòng lệnh nghiệp vụ.
        }

        // Map các claims còn lại // Chú thích đọc các thông tin cá nhân còn lại từ JWT Payload.
        foreach (var kvp in keyValuePairs) // Thực thi dòng lệnh nghiệp vụ.
        {
            // Bỏ qua các claim hệ thống JSON không cần thiết (exp, iat, nbf, iss, aud) // Chú thích lọc dữ liệu.
            // vẫn giữ lại để Blazor có thể dùng nếu cần // Giải thích thêm.
            claims.Add(new Claim(kvp.Key, kvp.Value.ToString())); // Thực thi dòng lệnh nghiệp vụ.
        }

        return claims; // Trả về giá trị của biểu thức 'claims'.
    }

    /// <summary>
    /// Decode chuỗi Base64Url thành byte array.
    /// 
    /// QUAN TRỌNG - Xử lý padding:
    /// Base64 chuẩn yêu cầu độ dài chuỗi chia hết cho 4. JWT dùng Base64Url 
    /// (thay '+' bằng '-', thay '/' bằng '_') và LOẠI BỎ padding '='.
    /// 
    /// Nếu không thêm lại padding, Convert.FromBase64String() sẽ ném FormatException.
    /// 
    /// Công thức: Thêm (4 - length % 4) % 4 ký tự '=' vào cuối.
    /// - length % 4 == 0 -> thêm 0 ký tự (đã đủ)
    /// - length % 4 == 1 -> KHÔNG HỢP LỆ trong Base64 (nhưng ta vẫn thêm 3 để tránh crash)
    /// - length % 4 == 2 -> thêm 2 ký tự '=='
    /// - length % 4 == 3 -> thêm 1 ký tự '='
    /// </summary>
    private static byte[]? DecodeBase64Url(string base64Url) // Thực thi dòng lệnh nghiệp vụ.
    {
        try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
        {
            // Bước 1: Thay thế ký tự Base64Url -> Base64 chuẩn // Chú thích thay đổi ký tự tương thích Base64.
            var base64 = base64Url // Thực hiện gán giá trị của biểu thức 'base64Url' cho biến/thuộc tính 'base64'.
                .Replace('-', '+') // Thực thi dòng lệnh nghiệp vụ.
                .Replace('_', '/'); // Thực thi dòng lệnh nghiệp vụ.

            // Bước 2: Thêm padding '=' cho đủ bội số của 4 // Chú thích bù ký tự đệm cho Base64.
            switch (base64.Length % 4) // Thực thi dòng lệnh nghiệp vụ.
            {
                case 2: // Thực thi dòng lệnh nghiệp vụ.
                    base64 += "=="; // Thực thi dòng lệnh nghiệp vụ.
                    break; // Thực thi dòng lệnh nghiệp vụ.
                case 3: // Thực thi dòng lệnh nghiệp vụ.
                    base64 += "="; // Thực thi dòng lệnh nghiệp vụ.
                    break; // Thực thi dòng lệnh nghiệp vụ.
                case 0: // Thực thi dòng lệnh nghiệp vụ.
                    // Đã đủ padding, không cần thêm // Bỏ qua không xử lý.
                    break; // Thực thi dòng lệnh nghiệp vụ.
                default: // Thực thi dòng lệnh nghiệp vụ.
                    // length % 4 == 1: Chuỗi Base64 không hợp lệ // Ghi nhận lỗi định dạng.
                    // Trả về null để caller xử lý gracefully thay vì ném exception // Hướng xử lý an toàn.
                    return null; // Trả về giá trị của biểu thức 'null'.
            }

            // Bước 3: Decode thành byte[] // Chú thích thực hiện giải mã chuỗi.
            return Convert.FromBase64String(base64); // Trả về giá trị của biểu thức 'Convert.FromBase64String(base64)'.
        }
        catch (FormatException) // Thực thi dòng lệnh nghiệp vụ.
        {
            // Nếu vẫn lỗi format (chuỗi chứa ký tự không hợp lệ) -> trả về null // Chú thích bắt lỗi định dạng.
            // Không ném exception ra ngoài để tránh crash app // Lý do bảo vệ ứng dụng.
            return null; // Trả về giá trị của biểu thức 'null'.
        }
    }
}

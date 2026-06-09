using System.Net.Http.Json; // Sử dụng thư viện hỗ trợ gửi nhận dữ liệu JSON qua HTTP.
using Microsoft.AspNetCore.Components.Forms; // Nhập (import) namespace Microsoft.AspNetCore.Components.Forms để sử dụng các lớp bên trong.
using PBL3.Shared.DTOs.Common; // Sử dụng các DTO của module Common thuộc tầng Shared.

namespace Client.Services.Image; // Thiết lập namespace Client.Services.Image để tổ chức quản lý cấu trúc các lớp.

public class ImageClientService : IImageClientService // Định nghĩa lớp ImageClientService triển khai các dịch vụ hoặc kế thừa từ IImageClientService.
{
    private readonly HttpClient _httpClient; // Khai báo đối tượng HttpClient nội bộ dùng để thực hiện cuộc gọi API.
    private const long MaxFileSizeBytes = 5 * 1024 * 1024; // Thực hiện gán giá trị của biểu thức '5 * 1024 * 1024' cho biến/thuộc tính 'private const long MaxFileSizeBytes'.

    public ImageClientService(HttpClient httpClient) // Hàm khởi tạo (Constructor) của lớp ImageClientService tiêm các phụ thuộc: httpClient.
    {
        _httpClient = httpClient; // Thực hiện gán giá trị của biểu thức 'httpClient' cho biến/thuộc tính '_httpClient'.
    }

    public async Task<string?> UploadAsync(IBrowserFile file, string folder) // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'UploadAsync' nhận tham số (file, folder) trả về kết quả kiểu Task<string?>.
    {
        try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
        {
            using var content = new MultipartFormDataContent(); // Khai báo sử dụng namespace hệ thống.
            using var stream = file.OpenReadStream(MaxFileSizeBytes); // Khai báo sử dụng namespace hệ thống.
            var streamContent = new StreamContent(stream); // Thực hiện gán giá trị của biểu thức 'new StreamContent(stream)' cho biến/thuộc tính 'streamContent'.
            streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType); // Thực hiện gán giá trị của biểu thức 'new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType)' cho biến/thuộc tính 'streamContent.Headers.ContentType'.
            content.Add(streamContent, "file", file.Name); // Thực thi dòng lệnh nghiệp vụ.

            var response = await _httpClient.PostAsync($"api/images/upload?folder={Uri.EscapeDataString(folder)}", content); // Gọi phương thức POST bất đồng bộ tới URL 'api/images/upload?folder={Uri.EscapeDataString(folder)}' và gán kết quả cho biến 'response'.
            if (!response.IsSuccessStatusCode) // Kiểm tra xem điều kiện '!response.IsSuccessStatusCode' có thỏa mãn hay không.
                return null; // Trả về giá trị của biểu thức 'null'.

            var result = await response.Content.ReadFromJsonAsync<ApiResult<UploadImageResponse>>(); // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'result'.
            return result?.Data?.Url; // Trả về giá trị của biểu thức 'result?.Data?.Url'.
        }
        catch // Thực thi dòng lệnh nghiệp vụ.
        {
            return null; // Trả về giá trị của biểu thức 'null'.
        }
    }
}

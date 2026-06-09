using Microsoft.JSInterop; // Nhập (import) namespace Microsoft.JSInterop để sử dụng các lớp bên trong.
using PBL3.Shared.DTOs.BuildPc; // Sử dụng các DTO của module BuildPc thuộc tầng Shared.
using System.Net.Http.Json; // Sử dụng thư viện hỗ trợ gửi nhận dữ liệu JSON qua HTTP.

namespace Client.Services.BuildPc // Thiết lập namespace Client.Services.BuildPc để tổ chức quản lý cấu trúc các lớp.
{
    public class BuildPcClientService : IBuildPcClientService // Định nghĩa lớp BuildPcClientService triển khai các dịch vụ hoặc kế thừa từ IBuildPcClientService.
    {
        private readonly HttpClient _httpClient; // Khai báo đối tượng HttpClient nội bộ dùng để thực hiện cuộc gọi API.
        private readonly IJSRuntime _jsRuntime; // Thực thi dòng lệnh nghiệp vụ.

        public BuildPcClientService(HttpClient httpClient, IJSRuntime jsRuntime) // Hàm khởi tạo (Constructor) của lớp BuildPcClientService tiêm các phụ thuộc: httpClient, jsRuntime.
        {
            _httpClient = httpClient; // Thực hiện gán giá trị của biểu thức 'httpClient' cho biến/thuộc tính '_httpClient'.
            _jsRuntime = jsRuntime; // Thực hiện gán giá trị của biểu thức 'jsRuntime' cho biến/thuộc tính '_jsRuntime'.
        }

        public async Task ExportBuildPcAsync(ExportBuildPcRequest request) // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'ExportBuildPcAsync' nhận tham số (request) trả về kết quả kiểu Task.
        {
            var response = await _httpClient.PostAsJsonAsync("/api/build-pc/export", request); // Gọi phương thức POST (gửi dữ liệu JSON) bất đồng bộ tới URL '/api/build-pc/export' và gán kết quả cho biến 'response'.
            if (!response.IsSuccessStatusCode) return; // Kiểm tra xem điều kiện '!response.IsSuccessStatusCode' có thỏa mãn hay không.

            var bytes = await response.Content.ReadAsByteArrayAsync(); // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'bytes'.
            var base64 = Convert.ToBase64String(bytes); // Thực hiện gán giá trị của biểu thức 'Convert.ToBase64String(bytes)' cho biến/thuộc tính 'base64'.
            await _jsRuntime.InvokeVoidAsync("downloadFile", // Thực thi dòng lệnh nghiệp vụ.
                "cau-hinh-pc.xlsx", // Thực thi dòng lệnh nghiệp vụ.
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", // Thực thi dòng lệnh nghiệp vụ.
                base64); // Thực thi dòng lệnh nghiệp vụ.
        }
    }
}

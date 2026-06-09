using Microsoft.AspNetCore.Components.Forms; // Nhập (import) namespace Microsoft.AspNetCore.Components.Forms để sử dụng các lớp bên trong.

namespace Client.Services.Image; // Thiết lập namespace Client.Services.Image để tổ chức quản lý cấu trúc các lớp.

public interface IImageClientService // Định nghĩa giao diện (interface) IImageClientService quy định các hàm tương tác của client.
{
    Task<string?> UploadAsync(IBrowserFile file, string folder); // Khai báo phương thức giao diện 'UploadAsync' với tham số (file, folder) có kết quả trả về kiểu Task<string?>.
}

using PBL3.Shared.DTOs.BuildPc; // Sử dụng các DTO của module BuildPc thuộc tầng Shared.

namespace Client.Services.BuildPc; // Thiết lập namespace Client.Services.BuildPc để tổ chức quản lý cấu trúc các lớp.

public interface IBuildPcClientService // Định nghĩa giao diện (interface) IBuildPcClientService quy định các hàm tương tác của client.
{
    Task ExportBuildPcAsync(ExportBuildPcRequest request); // Khai báo phương thức giao diện 'ExportBuildPcAsync' với tham số (request) có kết quả trả về kiểu Task.
}

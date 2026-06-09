using System.Threading.Tasks; // Nhập (import) namespace System.Threading.Tasks để sử dụng các lớp bên trong.
using PBL3.Shared.DTOs.Common; // Sử dụng các DTO của module Common thuộc tầng Shared.
using PBL3.Shared.DTOs.Inventory; // Sử dụng các DTO của module Inventory thuộc tầng Shared.

namespace Client.Services.Inventory; // Thiết lập namespace Client.Services.Inventory để tổ chức quản lý cấu trúc các lớp.

public interface IInventoryExportClientService // Định nghĩa giao diện (interface) IInventoryExportClientService quy định các hàm tương tác của client.
{
    Task<ApiResult<bool>> ExportOrderAsync(ExportOrderRequest request); // Khai báo phương thức giao diện 'ExportOrderAsync' với tham số (request) có kết quả trả về kiểu Task<ApiResult<bool>>.
    Task<ApiResult<bool>> ValidateSerialAsync(string serialNo, int variantId); // Khai báo phương thức giao diện 'ValidateSerialAsync' với tham số (serialNo, variantId) có kết quả trả về kiểu Task<ApiResult<bool>>.
}

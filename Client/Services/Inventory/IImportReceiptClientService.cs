using PBL3.Shared.DTOs.Common; // Sử dụng các DTO của module Common thuộc tầng Shared.
using PBL3.Shared.DTOs.Inventory; // Sử dụng các DTO của module Inventory thuộc tầng Shared.
using PBL3.Shared.DTOs.Products; // Sử dụng các DTO của module Products thuộc tầng Shared.

namespace Client.Services.Inventory; // Thiết lập namespace Client.Services.Inventory để tổ chức quản lý cấu trúc các lớp.

public interface IImportReceiptClientService // Định nghĩa giao diện (interface) IImportReceiptClientService quy định các hàm tương tác của client.
{
    Task<ApiResult<ImportReceiptDto>> CreateAsync(CreateImportReceiptRequest request); // Khai báo phương thức giao diện 'CreateAsync' với tham số (request) có kết quả trả về kiểu Task<ApiResult<ImportReceiptDto>>.
    Task<ApiResult<PagedResult<ImportReceiptDto>>> GetListAsync(ImportReceiptFilterRequest filter, CancellationToken cancellationToken = default); // Khai báo phương thức giao diện 'GetListAsync' với tham số (filter, default) có kết quả trả về kiểu Task<ApiResult<PagedResult<ImportReceiptDto>>>.
    Task<ApiResult<ImportReceiptDto>> GetByIdAsync(int id); // Khai báo phương thức giao diện 'GetByIdAsync' với tham số (id) có kết quả trả về kiểu Task<ApiResult<ImportReceiptDto>>.
}

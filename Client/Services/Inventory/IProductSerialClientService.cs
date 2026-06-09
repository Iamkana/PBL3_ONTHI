using PBL3.Shared.DTOs.Common; // Sử dụng các DTO của module Common thuộc tầng Shared.
using PBL3.Shared.DTOs.Inventory; // Sử dụng các DTO của module Inventory thuộc tầng Shared.

namespace Client.Services.Inventory; // Thiết lập namespace Client.Services.Inventory để tổ chức quản lý cấu trúc các lớp.

public interface IProductSerialClientService // Định nghĩa giao diện (interface) IProductSerialClientService quy định các hàm tương tác của client.
{
    Task<ApiResult<bool>> CheckExistAsync(string serialNumber, int variantId); // Khai báo phương thức giao diện 'CheckExistAsync' với tham số (serialNumber, variantId) có kết quả trả về kiểu Task<ApiResult<bool>>.
    Task<ApiResult<PagedResult<ProductSerialListDto>>> GetPagedListAsync(ProductSerialFilterRequest filter); // Khai báo phương thức giao diện 'GetPagedListAsync' với tham số (filter) có kết quả trả về kiểu Task<ApiResult<PagedResult<ProductSerialListDto>>>.
    Task<ApiResult<ProductSerialStatisticsDto>> GetStatisticsAsync(int? productId = null, int? variantId = null); // Khai báo phương thức giao diện 'GetStatisticsAsync' với tham số (null, null) có kết quả trả về kiểu Task<ApiResult<ProductSerialStatisticsDto>>.
    Task<ApiResult<ProductSerialDetailDto>> GetByIdAsync(int id); // Khai báo phương thức giao diện 'GetByIdAsync' với tham số (id) có kết quả trả về kiểu Task<ApiResult<ProductSerialDetailDto>>.
    Task<ApiResult<bool>> UpdateStatusAsync(int id, UpdateSerialStatusRequest request); // Khai báo phương thức giao diện 'UpdateStatusAsync' với tham số (id, request) có kết quả trả về kiểu Task<ApiResult<bool>>.
}

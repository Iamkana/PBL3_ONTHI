using PBL3.Shared.DTOs.Common; // Sử dụng các DTO của module Common thuộc tầng Shared.
using PBL3.Shared.DTOs.Manufacturers; // Sử dụng các DTO của module Manufacturers thuộc tầng Shared.

namespace Client.Services.Manufacturer; // Thiết lập namespace Client.Services.Manufacturer để tổ chức quản lý cấu trúc các lớp.

public interface IManufacturerClientService // Định nghĩa giao diện (interface) IManufacturerClientService quy định các hàm tương tác của client.
{
    Task<ApiResult<PagedResult<ManufacturerDto>>> GetListAsync(ManufacturerFilterRequest request); // Khai báo phương thức giao diện 'GetListAsync' với tham số (request) có kết quả trả về kiểu Task<ApiResult<PagedResult<ManufacturerDto>>>.
    Task<ApiResult<List<ManufacturerSummaryDto>>> GetDropdownAsync(); // Khai báo phương thức giao diện 'GetDropdownAsync' không tham số có kết quả trả về kiểu Task<ApiResult<List<ManufacturerSummaryDto>>>.
    Task<ApiResult<ManufacturerDto>> GetByIdAsync(int id); // Khai báo phương thức giao diện 'GetByIdAsync' với tham số (id) có kết quả trả về kiểu Task<ApiResult<ManufacturerDto>>.
    Task<ApiResult<ManufacturerDto>> CreateAsync(CreateManufacturerRequest request); // Khai báo phương thức giao diện 'CreateAsync' với tham số (request) có kết quả trả về kiểu Task<ApiResult<ManufacturerDto>>.
    Task<ApiResult<ManufacturerDto>> UpdateAsync(int id, UpdateManufacturerRequest request); // Khai báo phương thức giao diện 'UpdateAsync' với tham số (id, request) có kết quả trả về kiểu Task<ApiResult<ManufacturerDto>>.
    Task<ApiResult<bool>> DeleteAsync(int id); // Khai báo phương thức giao diện 'DeleteAsync' với tham số (id) có kết quả trả về kiểu Task<ApiResult<bool>>.
}

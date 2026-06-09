using PBL3.Shared.DTOs.Common; // Sử dụng các DTO của module Common thuộc tầng Shared.
using PBL3.Shared.DTOs.Products; // Sử dụng các DTO của module Products thuộc tầng Shared.
using PBL3.Shared.DTOs.Suppliers; // Sử dụng các DTO của module Suppliers thuộc tầng Shared.

namespace Client.Services.Supplier; // Thiết lập namespace Client.Services.Supplier để tổ chức quản lý cấu trúc các lớp.

public interface ISupplierClientService // Định nghĩa giao diện (interface) ISupplierClientService quy định các hàm tương tác của client.
{
    Task<ApiResult<PagedResult<SupplierDto>>> GetListAsync(SupplierFilterRequest request); // Khai báo phương thức giao diện 'GetListAsync' với tham số (request) có kết quả trả về kiểu Task<ApiResult<PagedResult<SupplierDto>>>.
    Task<ApiResult<SupplierDto>> GetByIdAsync(int id); // Khai báo phương thức giao diện 'GetByIdAsync' với tham số (id) có kết quả trả về kiểu Task<ApiResult<SupplierDto>>.
    Task<ApiResult<SupplierDto>> CreateAsync(CreateSupplierRequest request); // Khai báo phương thức giao diện 'CreateAsync' với tham số (request) có kết quả trả về kiểu Task<ApiResult<SupplierDto>>.
    Task<ApiResult<SupplierDto>> UpdateAsync(int id, UpdateSupplierRequest request); // Khai báo phương thức giao diện 'UpdateAsync' với tham số (id, request) có kết quả trả về kiểu Task<ApiResult<SupplierDto>>.
    Task<ApiResult<bool>> DeleteAsync(int id); // Khai báo phương thức giao diện 'DeleteAsync' với tham số (id) có kết quả trả về kiểu Task<ApiResult<bool>>.
}

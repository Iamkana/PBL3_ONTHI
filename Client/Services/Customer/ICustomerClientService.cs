using PBL3.Shared.DTOs.Common; // Sử dụng các DTO của module Common thuộc tầng Shared.
using PBL3.Shared.DTOs.Customers; // Sử dụng các DTO của module Customers thuộc tầng Shared.
using PBL3.Shared.DTOs.Products; // Sử dụng các DTO của module Products thuộc tầng Shared.

namespace Client.Services.Customer; // Thiết lập namespace Client.Services.Customer để tổ chức quản lý cấu trúc các lớp.

public interface ICustomerClientService // Định nghĩa giao diện (interface) ICustomerClientService quy định các hàm tương tác của client.
{
    Task<ApiResult<PagedResult<CustomerDto>>> GetListAsync(CustomerFilterRequest request); // Khai báo phương thức giao diện 'GetListAsync' với tham số (request) có kết quả trả về kiểu Task<ApiResult<PagedResult<CustomerDto>>>.
    Task<ApiResult<CustomerDetailDto>> GetByIdAsync(Guid id); // Khai báo phương thức giao diện 'GetByIdAsync' với tham số (id) có kết quả trả về kiểu Task<ApiResult<CustomerDetailDto>>.
    Task<ApiResult<CustomerDto>> CreateAsync(CreateCustomerRequest request); // Khai báo phương thức giao diện 'CreateAsync' với tham số (request) có kết quả trả về kiểu Task<ApiResult<CustomerDto>>.
    Task<ApiResult<CustomerDto>> UpdateAsync(Guid id, UpdateCustomerRequest request); // Khai báo phương thức giao diện 'UpdateAsync' với tham số (id, request) có kết quả trả về kiểu Task<ApiResult<CustomerDto>>.
    Task<ApiResult<bool>> DeactivateAsync(Guid id, string? lockReason = null); // Khai báo phương thức giao diện 'DeactivateAsync' với tham số (id, null) có kết quả trả về kiểu Task<ApiResult<bool>>.
    Task<ApiResult<bool>> ReactivateAsync(Guid id); // Khai báo phương thức giao diện 'ReactivateAsync' với tham số (id) có kết quả trả về kiểu Task<ApiResult<bool>>.
    Task<ApiResult<CustomerDto>> GetMyProfileAsync(); // Khai báo phương thức giao diện 'GetMyProfileAsync' không tham số có kết quả trả về kiểu Task<ApiResult<CustomerDto>>.
    Task<ApiResult<CustomerDto>> UpdateMyProfileAsync(UpdateCustomerRequest request); // Khai báo phương thức giao diện 'UpdateMyProfileAsync' với tham số (request) có kết quả trả về kiểu Task<ApiResult<CustomerDto>>.
}

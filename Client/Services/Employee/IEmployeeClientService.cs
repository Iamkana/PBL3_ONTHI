using PBL3.Shared.DTOs.Common; // Sử dụng các DTO của module Common thuộc tầng Shared.
using PBL3.Shared.DTOs.Employees; // Sử dụng các DTO của module Employees thuộc tầng Shared.
using System; // Nhập (import) namespace System để sử dụng các lớp bên trong.
using System.Collections.Generic; // Nhập (import) namespace System.Collections.Generic để sử dụng các lớp bên trong.
using System.Threading.Tasks; // Nhập (import) namespace System.Threading.Tasks để sử dụng các lớp bên trong.

namespace Client.Services.Employee; // Thiết lập namespace Client.Services.Employee để tổ chức quản lý cấu trúc các lớp.

public interface IEmployeeClientService // Định nghĩa giao diện (interface) IEmployeeClientService quy định các hàm tương tác của client.
{
    Task<ApiResult<PagedResult<EmployeeListDto>>> GetListAsync(EmployeeFilterRequest request); // Khai báo phương thức giao diện 'GetListAsync' với tham số (request) có kết quả trả về kiểu Task<ApiResult<PagedResult<EmployeeListDto>>>.
    Task<ApiResult<EmployeeListDto>> GetByIdAsync(Guid id); // Khai báo phương thức giao diện 'GetByIdAsync' với tham số (id) có kết quả trả về kiểu Task<ApiResult<EmployeeListDto>>.
    Task<ApiResult<EmployeeListDto>> CreateAsync(CreateEmployeeRequest request); // Khai báo phương thức giao diện 'CreateAsync' với tham số (request) có kết quả trả về kiểu Task<ApiResult<EmployeeListDto>>.
    Task<ApiResult<EmployeeListDto>> UpdateAsync(Guid id, UpdateEmployeeRequest request); // Khai báo phương thức giao diện 'UpdateAsync' với tham số (id, request) có kết quả trả về kiểu Task<ApiResult<EmployeeListDto>>.
    Task<ApiResult<bool>> DeactivateAsync(Guid id, string? lockReason = null); // Khai báo phương thức giao diện 'DeactivateAsync' với tham số (id, null) có kết quả trả về kiểu Task<ApiResult<bool>>.
    Task<ApiResult<bool>> ReactivateAsync(Guid id); // Khai báo phương thức giao diện 'ReactivateAsync' với tham số (id) có kết quả trả về kiểu Task<ApiResult<bool>>.
    Task<ApiResult<List<EmployeeDto>>> GetTechniciansAsync(); // Khai báo phương thức giao diện 'GetTechniciansAsync' không tham số có kết quả trả về kiểu Task<ApiResult<List<EmployeeDto>>>.
}

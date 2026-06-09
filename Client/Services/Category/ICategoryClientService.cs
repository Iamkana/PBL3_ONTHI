using PBL3.Shared.DTOs.Categories; // Sử dụng các DTO của module Categories thuộc tầng Shared.
using PBL3.Shared.DTOs.Common; // Sử dụng các DTO của module Common thuộc tầng Shared.

namespace Client.Services.Category; // Thiết lập namespace Client.Services.Category để tổ chức quản lý cấu trúc các lớp.

public interface ICategoryClientService // Định nghĩa giao diện (interface) ICategoryClientService quy định các hàm tương tác của client.
{
    Task<ApiResult<List<CategoryTreeDto>>> GetTreeAsync(); // Khai báo phương thức giao diện 'GetTreeAsync' không tham số có kết quả trả về kiểu Task<ApiResult<List<CategoryTreeDto>>>.
    Task<ApiResult<CategoryDto>> GetByIdAsync(int id); // Khai báo phương thức giao diện 'GetByIdAsync' với tham số (id) có kết quả trả về kiểu Task<ApiResult<CategoryDto>>.
    Task<ApiResult<CategoryDto>> CreateAsync(CreateCategoryRequest request); // Khai báo phương thức giao diện 'CreateAsync' với tham số (request) có kết quả trả về kiểu Task<ApiResult<CategoryDto>>.
    Task<ApiResult<CategoryDto>> UpdateAsync(int id, UpdateCategoryRequest request); // Khai báo phương thức giao diện 'UpdateAsync' với tham số (id, request) có kết quả trả về kiểu Task<ApiResult<CategoryDto>>.
    Task<ApiResult<bool>> DeleteAsync(int id); // Khai báo phương thức giao diện 'DeleteAsync' với tham số (id) có kết quả trả về kiểu Task<ApiResult<bool>>.
}

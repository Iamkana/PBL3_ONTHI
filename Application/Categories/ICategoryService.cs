using PBL3.Shared.DTOs.Categories; // Sử dụng các DTO liên quan đến Danh mục sản phẩm.
using PBL3.Shared.DTOs.Common; // Sử dụng DTO dùng chung ApiResult.

namespace PBL3.Application.Categories // Khai báo namespace cho tầng Application của module danh mục.
{
    public interface ICategoryService // Định nghĩa giao diện dịch vụ danh mục sản phẩm ICategoryService.
    {
        Task<ApiResult<List<CategoryTreeDto>>> GetTreeAsync(); // Khai báo phương thức lấy cấu trúc cây danh mục đệ quy.
        Task<ApiResult<CategoryDto>> GetByIdAsync(int id); // Khai báo phương thức lấy thông tin danh mục theo Id.
        Task<ApiResult<CategoryDto>> CreateAsync(CreateCategoryRequest request); // Khai báo phương thức tạo mới danh mục.
        Task<ApiResult<CategoryDto>> UpdateAsync(int id, UpdateCategoryRequest request); // Khai báo phương thức cập nhật danh mục và kiểm tra liên kết vòng.
        Task<ApiResult<bool>> DeleteAsync(int id); // Khai báo phương thức xóa mềm danh mục.
    }
}

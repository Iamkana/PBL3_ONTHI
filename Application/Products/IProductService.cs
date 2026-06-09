using PBL3.Shared.DTOs.Common; // Sử dụng DTO dùng chung ApiResult và PagedResult.
using PBL3.Shared.DTOs.Products; // Sử dụng DTO của module sản phẩm.

namespace PBL3.Application.Products // Khai báo namespace cho tầng Application của module quản lý sản phẩm.
{
    public interface IProductService // Định nghĩa giao diện dịch vụ sản phẩm IProductService.
    {
        Task<ApiResult<PagedResult<ProductListDto>>> GetListAsync(ProductFilterRequest request); // Khai báo phương thức lấy danh sách sản phẩm phân trang có bộ lọc.
        Task<ApiResult<ProductDetailDto>> GetByIdAsync(int id); // Khai báo phương thức lấy chi tiết sản phẩm theo Id.
        Task<ApiResult<ProductDetailDto>> CreateAsync(CreateProductRequest request); // Khai báo phương thức tạo mới sản phẩm (bao gồm cả các biến thể lồng nhau).
        Task<ApiResult<ProductDetailDto>> UpdateAsync(int id, UpdateProductRequest request); // Khai báo phương thức cập nhật thông tin chung của sản phẩm.
        Task<ApiResult<ProductVariantDto>> AddVariantAsync(int productId, SaveVariantRequest request); // Khai báo phương thức thêm mới một biến thể sản phẩm cho sản phẩm đã có.
        Task<ApiResult<bool>> UpdateImagesAsync(int productId, List<SaveImageRequest> images); // Khai báo phương thức cập nhật danh sách ảnh cho tất cả các biến thể của sản phẩm.
        Task<ApiResult<bool>> DeleteAsync(int id); // Khai báo phương thức xóa mềm sản phẩm theo Id.
    }
}

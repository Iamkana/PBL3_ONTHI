using PBL3.Shared.DTOs.Common; // Sử dụng các DTO của module Common thuộc tầng Shared.
using PBL3.Shared.DTOs.Products; // Sử dụng các DTO của module Products thuộc tầng Shared.

namespace Client.Services.Product; // Thiết lập namespace Client.Services.Product để tổ chức quản lý cấu trúc các lớp.

public interface IProductClientService // Định nghĩa giao diện (interface) IProductClientService quy định các hàm tương tác của client.
{
    Task<ApiResult<PagedResult<ProductListDto>>> GetListAsync(ProductFilterRequest request); // Khai báo phương thức giao diện 'GetListAsync' với tham số (request) có kết quả trả về kiểu Task<ApiResult<PagedResult<ProductListDto>>>.
    Task<ApiResult<ProductDetailDto>> GetByIdAsync(int id); // Khai báo phương thức giao diện 'GetByIdAsync' với tham số (id) có kết quả trả về kiểu Task<ApiResult<ProductDetailDto>>.
    Task<ApiResult<ProductDetailDto>> CreateAsync(CreateProductRequest request); // Khai báo phương thức giao diện 'CreateAsync' với tham số (request) có kết quả trả về kiểu Task<ApiResult<ProductDetailDto>>.
    Task<ApiResult<ProductDetailDto>> UpdateAsync(int id, UpdateProductRequest request); // Khai báo phương thức giao diện 'UpdateAsync' với tham số (id, request) có kết quả trả về kiểu Task<ApiResult<ProductDetailDto>>.
    Task<ApiResult<ProductVariantDto>> AddVariantAsync(int productId, SaveVariantRequest request); // Khai báo phương thức giao diện 'AddVariantAsync' với tham số (productId, request) có kết quả trả về kiểu Task<ApiResult<ProductVariantDto>>.
    Task<ApiResult<bool>> UpdateProductImagesAsync(int productId, List<SaveImageRequest> images); // Khai báo phương thức giao diện 'UpdateProductImagesAsync' với tham số (productId, images) có kết quả trả về kiểu Task<ApiResult<bool>>.
    Task<ApiResult<bool>> DeleteAsync(int id); // Khai báo phương thức giao diện 'DeleteAsync' với tham số (id) có kết quả trả về kiểu Task<ApiResult<bool>>.
}

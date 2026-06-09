using PBL3.Shared.DTOs.Storefront; // Sử dụng các DTO của module Storefront thuộc tầng Shared.
using PBL3.Shared.DTOs.Common; // Sử dụng các DTO của module Common thuộc tầng Shared.

namespace Client.Services.Storefront; // Thiết lập namespace Client.Services.Storefront để tổ chức quản lý cấu trúc các lớp.

public interface IStorefrontClientService // Định nghĩa giao diện (interface) IStorefrontClientService quy định các hàm tương tác của client.
{
    Task<ApiResult<List<CategoryMenuResponse>>> GetActiveCategoriesAsync(); // Khai báo phương thức giao diện 'GetActiveCategoriesAsync' không tham số có kết quả trả về kiểu Task<ApiResult<List<CategoryMenuResponse>>>.
    Task<ApiResult<List<ProductCardResponse>>> GetFeaturedProductsAsync(int? categoryId, int take = 10); // Khai báo phương thức giao diện 'GetFeaturedProductsAsync' với tham số (categoryId, 10) có kết quả trả về kiểu Task<ApiResult<List<ProductCardResponse>>>.
    Task<ApiResult<ProductDetailResponse>> GetProductDetailAsync(string slug); // Khai báo phương thức giao diện 'GetProductDetailAsync' với tham số (slug) có kết quả trả về kiểu Task<ApiResult<ProductDetailResponse>>.
    Task<ApiResult<List<ProductCardResponse>>> GetRelatedProductsAsync(string slug, int take = 5); // Khai báo phương thức giao diện 'GetRelatedProductsAsync' với tham số (slug, 5) có kết quả trả về kiểu Task<ApiResult<List<ProductCardResponse>>>.
    Task<ApiResult<CategoryDetailResponse>> GetCategoryBySlugAsync(string slug); // Khai báo phương thức giao diện 'GetCategoryBySlugAsync' với tham số (slug) có kết quả trả về kiểu Task<ApiResult<CategoryDetailResponse>>.
    Task<ApiResult<PagedResult<ProductCardResponse>>> GetProductsByCategoryAsync(string slug, int page = 1, int pageSize = 20); // Khai báo phương thức giao diện 'GetProductsByCategoryAsync' với tham số (slug, 1, 20) có kết quả trả về kiểu Task<ApiResult<PagedResult<ProductCardResponse>>>.
    Task<ApiResult<PagedResult<ProductCardResponse>>> SearchProductsAsync( // Thực hiện xử lý phương thức nghiệp vụ 'SearchProductsAsync' không tham số trả về kết quả kiểu Task<ApiResult<PagedResult<ProductCardResponse>>>.
        string? keyword, int? categoryId, decimal? priceMin, decimal? priceMax, // Thực thi dòng lệnh nghiệp vụ.
        int page = 1, int pageSize = 20); // Thực hiện gán giá trị của biểu thức '1, int pageSize = 20)' cho biến/thuộc tính 'int page'.
}

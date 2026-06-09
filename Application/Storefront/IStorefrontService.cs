using PBL3.Shared.DTOs.Common; // Sử dụng DTO dùng chung ApiResult và PagedResult.
using PBL3.Shared.DTOs.Storefront; // Sử dụng DTO liên quan đến giao diện Storefront cửa hàng.

namespace PBL3.Application.Storefront // Khai báo namespace cho tầng Application của module Storefront.
{
    public interface IStorefrontService // Định nghĩa giao diện dịch vụ khách hàng Storefront IStorefrontService.
    {
        Task<ApiResult<List<CategoryMenuResponse>>> GetActiveCategoriesAsync(); // Khai báo phương thức lấy cây danh mục sản phẩm đang hoạt động hiển thị menu.
        Task<ApiResult<List<ProductCardResponse>>> GetFeaturedProductsAsync(int? categoryId, int take = 5); // Khai báo phương thức lấy các sản phẩm nổi bật theo danh mục bất đồng bộ.
        Task<ApiResult<ProductDetailResponse>> GetProductDetailAsync(string slug); // Khai báo phương thức lấy thông tin chi tiết sản phẩm theo Slug (đường dẫn thân thiện).
        Task<ApiResult<List<ProductCardResponse>>> GetRelatedProductsAsync(string slug); // Khai báo phương thức lấy danh sách các sản phẩm liên quan theo Slug sản phẩm hiện tại.
        Task<ApiResult<CategoryDetailResponse>> GetCategoryBySlugAsync(string slug); // Khai báo phương thức lấy chi tiết danh mục theo Slug.
        Task<ApiResult<PagedResult<ProductCardResponse>>> GetProductsByCategoryAsync(string categorySlug, int page, int pageSize); // Khai báo phương thức lấy sản phẩm thuộc danh mục (và danh mục con) phân trang.
        Task<ApiResult<PagedResult<ProductCardResponse>>> SearchProductsAsync( // Khai báo phương thức tìm kiếm sản phẩm đa tiêu chí với phân trang.
            string? keyword, int? categoryId, decimal? priceMin, decimal? priceMax, // Từ khóa tìm kiếm, Id danh mục sản phẩm, giá tối thiểu, giá tối đa.
            int page, int pageSize); // Số trang và số phần tử trên trang.
    }
}

using Microsoft.AspNetCore.Authorization; // Sử dụng phân quyền truy cập.
using Microsoft.AspNetCore.Mvc; // Sử dụng các thành phần MVC cho Web API.
using PBL3.Application.Storefront; // Sử dụng tầng dịch vụ mặt tiền cửa hàng IStorefrontService.
using PBL3.Shared.DTOs.Common; // Sử dụng các DTO dùng chung như ApiResult, PagedResult.
using PBL3.Shared.DTOs.Storefront; // Sử dụng các DTO liên quan đến giao diện Storefront.
using PBL3.API.Extensions; // Sử dụng extension ToActionResult.

namespace PBL3.API.Controllers.Storefront // Khai báo namespace cho các Controller thuộc Storefront.
{
    [ApiController] // Khai báo đây là một Web API Controller hỗ trợ tự động validate model.
    [Route("api/storefront")] // Định nghĩa route truy cập: api/storefront.
    [Produces("application/json")] // Quy định định dạng trả về mặc định dạng JSON.
    [AllowAnonymous] // Cho phép truy cập nặc danh (không cần đăng nhập) vì đây là các API hiển thị thông tin sản phẩm ra trang chủ.
    public class StorefrontController : ControllerBase // Định nghĩa lớp StorefrontController kế thừa từ ControllerBase.
    {
        private readonly IStorefrontService _storefrontService; // Khai báo trường dịch vụ hiển thị Storefront.

        public StorefrontController(IStorefrontService storefrontService) // Constructor injection tiêm IStorefrontService.
        {
            _storefrontService = storefrontService; // Gán dịch vụ được tiêm.
        }

        /// <summary>
        /// Lấy danh sách danh mục đang hoạt động cho Menu.
        /// </summary>
        [HttpGet("categories")] // Định nghĩa HTTP GET Method lấy danh sách danh mục hiển thị trên menu (api/storefront/categories).
        [ProducesResponseType(typeof(ApiResult<List<CategoryMenuResponse>>), StatusCodes.Status200OK)] // Trả về danh sách danh mục thành công.
        public async Task<IActionResult> GetCategories() // Lấy cấu trúc cây danh mục sản phẩm đang hoạt động.
        {
            var result = await _storefrontService.GetActiveCategoriesAsync(); // Gọi service lấy danh sách các danh mục Active.
            return Ok(result); // Trả về HTTP 200 OK.
        }

        /// <summary>
        /// Lấy danh sách sản phẩm nổi bật.
        /// </summary>
        [HttpGet("products/featured")] // Định nghĩa HTTP GET Method lấy danh sách sản phẩm nổi bật (api/storefront/products/featured).
        [ProducesResponseType(typeof(ApiResult<List<ProductCardResponse>>), StatusCodes.Status200OK)] // Trả về danh sách sản phẩm nổi bật thành công.
        public async Task<IActionResult> GetFeaturedProducts([FromQuery] int? categoryId, [FromQuery] int take = 5) // Lấy sản phẩm nổi bật.
        {
            var result = await _storefrontService.GetFeaturedProductsAsync(categoryId, take); // Gọi service lấy sản phẩm nổi bật theo danh mục và số lượng.
            return Ok(result); // Trả về HTTP 200 OK.
        }

        /// <summary>
        /// Lấy thông tin chi tiết sản phẩm.
        /// </summary>
        [HttpGet("products/{slug}")] // Định nghĩa HTTP GET Method lấy chi tiết sản phẩm theo đường dẫn thân thiện - slug (api/storefront/products/{slug}).
        [ProducesResponseType(typeof(ApiResult<ProductDetailResponse>), StatusCodes.Status200OK)] // Tìm thấy chi tiết sản phẩm.
        public async Task<IActionResult> GetProductDetail(string slug) // Xem chi tiết thông tin, biến thể và hình ảnh của sản phẩm.
        {
            var result = await _storefrontService.GetProductDetailAsync(slug); // Gọi service lấy chi tiết sản phẩm theo slug.
            return result.ToActionResult(this); // Ánh xạ kết quả sang Action Result tương ứng.
        }

        /// <summary>
        /// Lấy danh sách sản phẩm liên quan.
        /// </summary>
        [HttpGet("products/{slug}/related")] // Định nghĩa HTTP GET Method lấy sản phẩm tương tự/liên quan (api/storefront/products/{slug}/related).
        [ProducesResponseType(typeof(ApiResult<List<ProductCardResponse>>), StatusCodes.Status200OK)] // Trả về danh sách liên quan thành công.
        public async Task<IActionResult> GetRelatedProducts(string slug) // Lấy các sản phẩm có cùng danh mục với sản phẩm hiện tại.
        {
            var result = await _storefrontService.GetRelatedProductsAsync(slug); // Gọi service lấy danh sách sản phẩm liên quan.
            return Ok(result); // Trả về HTTP 200 OK.
        }

        /// <summary>
        /// Lấy thông tin danh mục theo slug (dùng cho breadcrumb và tiêu đề trang).
        /// </summary>
        [HttpGet("categories/{slug}")] // Định nghĩa HTTP GET Method lấy chi tiết danh mục theo slug (api/storefront/categories/{slug}).
        [ProducesResponseType(typeof(ApiResult<CategoryDetailResponse>), StatusCodes.Status200OK)] // Thành công.
        public async Task<IActionResult> GetCategoryBySlug(string slug) // Lấy thông tin danh mục bao gồm cha/con để hiển thị Breadcrumb định hướng.
        {
            var result = await _storefrontService.GetCategoryBySlugAsync(slug); // Gọi service lấy chi tiết danh mục theo slug.
            return result.ToActionResult(this); // Trả về Action Result.
        }

        /// <summary>
        /// Tìm kiếm sản phẩm theo từ khóa, danh mục và khoảng giá với phân trang.
        /// </summary>
        [HttpGet("products/search")] // Định nghĩa HTTP GET Method tìm kiếm sản phẩm tổng hợp (api/storefront/products/search).
        [ProducesResponseType(typeof(ApiResult<PagedResult<ProductCardResponse>>), StatusCodes.Status200OK)] // Thành công.
        public async Task<IActionResult> SearchProducts( // Tìm kiếm sản phẩm đa tiêu chí.
            [FromQuery] string? keyword, // Nhận từ khóa tìm kiếm từ query.
            [FromQuery] int? categoryId, // Nhận mã danh mục từ query.
            [FromQuery] decimal? priceMin, // Nhận mức giá tối thiểu từ query.
            [FromQuery] decimal? priceMax, // Nhận mức giá tối đa từ query.
            [FromQuery] int page = 1, // Số trang hiện tại.
            [FromQuery] int pageSize = 20) // Kích thước trang.
        {
            var result = await _storefrontService.SearchProductsAsync(keyword, categoryId, priceMin, priceMax, page, pageSize); // Gọi service thực hiện tìm kiếm phân trang.
            return Ok(result); // Trả về HTTP 200 OK.
        }

        /// <summary>
        /// Lấy danh sách sản phẩm theo danh mục (bao gồm cả danh mục con) với phân trang.
        /// </summary>
        [HttpGet("categories/{slug}/products")] // Định nghĩa HTTP GET Method lấy sản phẩm của danh mục (api/storefront/categories/{slug}/products).
        [ProducesResponseType(typeof(ApiResult<PagedResult<ProductCardResponse>>), StatusCodes.Status200OK)] // Thành công.
        public async Task<IActionResult> GetProductsByCategory( // Lấy tất cả sản phẩm của danh mục cụ thể và danh mục cấp dưới của nó.
            string slug, // Slug danh mục.
            [FromQuery] int page = 1, // Trang hiện tại.
            [FromQuery] int pageSize = 20) // Kích thước trang.
        {
            var result = await _storefrontService.GetProductsByCategoryAsync(slug, page, pageSize); // Gọi service truy vấn sản phẩm theo danh mục.
            return result.ToActionResult(this); // Ánh xạ kết quả trả về.
        }
    }
}

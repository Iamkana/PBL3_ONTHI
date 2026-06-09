using Microsoft.AspNetCore.Authorization; // Sử dụng để cấu hình phân quyền truy cập.
using Microsoft.AspNetCore.Mvc; // Sử dụng các lớp cơ bản của ASP.NET Core MVC (ControllerBase, HttpGet...).
using PBL3.Application.Products; // Tham chiếu tầng dịch vụ sản phẩm IProductService.
using PBL3.Shared.DTOs.Common; // Sử dụng các DTO dùng chung như ApiResult, PagedResult.
using PBL3.Shared.DTOs.Products; // Sử dụng các DTO liên quan đến sản phẩm và phiên bản.
using PBL3.API.Extensions; // Sử dụng các phương thức mở rộng để map kết quả.

namespace PBL3.API.Controllers.Admin // Khai báo namespace cho các Controller quản lý sản phẩm.
{
    [ApiController] // Kích hoạt tính năng Web API Controller tự động validate dữ liệu đầu vào.
    [Route("api/[controller]")] // Định nghĩa route truy cập mặc định: api/products.
    [Produces("application/json")] // Quy định định dạng trả về mặc định dạng JSON.
    [Authorize(Roles = "Admin, Employee")] // Yêu cầu người dùng đăng nhập dưới quyền Admin hoặc Employee để truy cập các API (ngoại trừ được ghi đè).
    public class ProductsController : ControllerBase // Định nghĩa lớp ProductsController kế thừa từ ControllerBase.
    {
        private readonly IProductService _productService; // Khai báo trường dịch vụ sản phẩm.

        public ProductsController(IProductService productService) // Constructor injection tiêm IProductService từ DI container.
        {
            _productService = productService; // Gán dịch vụ được tiêm vào biến thành viên.
        }

        /// <summary>
        /// Lấy danh sách sản phẩm (phân trang, lọc theo Category/Price/Keyword).
        /// </summary>
        [AllowAnonymous] // Cho phép khách vãng lai gọi API để xem danh sách sản phẩm công khai.
        [HttpGet] // Định nghĩa HTTP GET Method lấy danh sách sản phẩm (api/products).
        [ProducesResponseType(typeof(ApiResult<PagedResult<ProductListDto>>), StatusCodes.Status200OK)] // Trả về HTTP 200 OK kèm danh sách phân trang.
        public async Task<IActionResult> GetList([FromQuery] ProductFilterRequest request) // Nhận tham số bộ lọc từ Query String.
        {
            var result = await _productService.GetListAsync(request); // Gọi service lấy danh sách sản phẩm.
            return Ok(result); // Trả về kết quả HTTP 200 OK.
        }

        /// <summary>
        /// Lấy chi tiết sản phẩm theo Id (bao gồm Variants, Images, Attributes).
        /// </summary>
        [AllowAnonymous] // Cho phép tất cả người dùng xem chi tiết sản phẩm.
        [HttpGet("{id:int}")] // Định nghĩa HTTP GET Method lấy chi tiết sản phẩm theo Id kiểu int (api/products/{id}).
        [ProducesResponseType(typeof(ApiResult<ProductDetailDto>), StatusCodes.Status200OK)] // Tìm thấy sản phẩm.
        [ProducesResponseType(typeof(ApiResult<ProductDetailDto>), StatusCodes.Status404NotFound)] // Không tìm thấy.
        public async Task<IActionResult> GetById(int id) // Lấy thông tin chi tiết sản phẩm theo Id.
        {
            var result = await _productService.GetByIdAsync(id); // Gọi service lấy chi tiết sản phẩm.
            return result.ToActionResult(this); // Ánh xạ kết quả sang Action Result tương ứng.
        }

        /// <summary>
        /// Tạo mới sản phẩm trọn gói (Product + Variants).
        /// </summary>
        [HttpPost] // Định nghĩa HTTP POST Method thêm mới sản phẩm (api/products).
        [ProducesResponseType(typeof(ApiResult<ProductDetailDto>), StatusCodes.Status201Created)] // Tạo thành công trả về 201 Created.
        [ProducesResponseType(typeof(ApiResult<ProductDetailDto>), StatusCodes.Status400BadRequest)] // Dữ liệu không hợp lệ.
        public async Task<IActionResult> Create([FromBody] CreateProductRequest request) // Nhận dữ liệu tạo mới sản phẩm từ Body.
        {
            var result = await _productService.CreateAsync(request); // Gọi service thực hiện tạo mới sản phẩm kèm các phiên bản.

            if (!result.Success) return result.ToActionResult(this); // Nếu có lỗi xảy ra thì trả về kết quả lỗi tương ứng.

            return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result); // Trả về HTTP 201 Created kèm route lấy chi tiết sản phẩm vừa tạo.
        }

        /// <summary>
        /// Cập nhật thông tin chung sản phẩm.
        /// </summary>
        [HttpPut("{id:int}")] // Định nghĩa HTTP PUT Method cập nhật sản phẩm theo Id (api/products/{id}).
        [ProducesResponseType(typeof(ApiResult<ProductDetailDto>), StatusCodes.Status200OK)] // Cập nhật thành công.
        [ProducesResponseType(typeof(ApiResult<ProductDetailDto>), StatusCodes.Status400BadRequest)] // Dữ liệu đầu vào lỗi.
        [ProducesResponseType(typeof(ApiResult<ProductDetailDto>), StatusCodes.Status404NotFound)] // Không tìm thấy.
        public async Task<IActionResult> Update(int id, [FromBody] UpdateProductRequest request) // Cập nhật thông tin sản phẩm theo Id.
        {
            var result = await _productService.UpdateAsync(id, request); // Gọi service thực hiện cập nhật sản phẩm.
            return result.ToActionResult(this); // Trả về kết quả tương ứng.
        }

        /// <summary>
        /// Thêm phiên bản (Variant) mới cho sản phẩm đã tồn tại.
        /// </summary>
        [HttpPost("{id:int}/variants")] // Định nghĩa HTTP POST Method thêm phiên bản cho sản phẩm (api/products/{id}/variants).
        [ProducesResponseType(typeof(ApiResult<ProductVariantDto>), StatusCodes.Status201Created)] // Tạo thành công.
        [ProducesResponseType(typeof(ApiResult<ProductVariantDto>), StatusCodes.Status400BadRequest)] // Lỗi nghiệp vụ/đầu vào.
        [ProducesResponseType(typeof(ApiResult<ProductVariantDto>), StatusCodes.Status404NotFound)] // Không tìm thấy sản phẩm.
        public async Task<IActionResult> AddVariant(int id, [FromBody] SaveVariantRequest request) // Thêm phiên bản mới.
        {
            var result = await _productService.AddVariantAsync(id, request); // Gọi service thực hiện thêm variant vào sản phẩm.

            if (!result.Success) return result.ToActionResult(this); // Nếu thất bại, trả về lỗi tương ứng.

            return CreatedAtAction(nameof(GetById), new { id }, result); // Trả về kết quả 201 Created.
        }

        /// <summary>
        /// Cập nhật danh sách ảnh cho sản phẩm (thay toàn bộ ảnh của tất cả variants).
        /// </summary>
        [HttpPut("{id:int}/images")] // Định nghĩa HTTP PUT Method cập nhật ảnh sản phẩm (api/products/{id}/images).
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status200OK)] // Thành công.
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status400BadRequest)] // Lỗi đầu vào.
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status404NotFound)] // Không tìm thấy sản phẩm.
        public async Task<IActionResult> UpdateImages(int id, [FromBody] List<SaveImageRequest> images) // Cập nhật danh sách ảnh.
        {
            var result = await _productService.UpdateImagesAsync(id, images); // Gọi service thực hiện cập nhật lại danh sách hình ảnh sản phẩm.
            return result.ToActionResult(this); // Trả về kết quả tương ứng.
        }

        /// <summary>
        /// Xóa mềm sản phẩm (Soft Delete).
        /// </summary>
        [HttpDelete("{id:int}")] // Định nghĩa HTTP DELETE Method xóa sản phẩm theo Id (api/products/{id}).
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status200OK)] // Xóa thành công.
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status400BadRequest)] // Có ràng buộc đơn hàng/hóa đơn không cho phép xóa.
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status404NotFound)] // Không tìm thấy sản phẩm.
        public async Task<IActionResult> Delete(int id) // Xóa mềm sản phẩm theo Id.
        {
            var result = await _productService.DeleteAsync(id); // Gọi service thực hiện đánh dấu xóa mềm sản phẩm.
            return result.ToActionResult(this); // Trả về kết quả tương ứng.
        }
    }
}

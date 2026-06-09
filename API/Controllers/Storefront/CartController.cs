using System; // Sử dụng các kiểu dữ liệu cơ bản của hệ thống.
using System.Security.Claims; // Sử dụng để xác thực claims và trích xuất thông tin người dùng.
using System.Threading.Tasks; // Sử dụng lập trình bất đồng bộ Task.
using Microsoft.AspNetCore.Authorization; // Sử dụng phân quyền truy cập.
using Microsoft.AspNetCore.Mvc; // Sử dụng các thành phần MVC cho Web API.
using PBL3.Application.Cart; // Sử dụng dịch vụ giỏ hàng ICartService.
using PBL3.Shared.DTOs.Cart; // Sử dụng các DTO liên quan đến giỏ hàng.
using PBL3.Shared.DTOs.Common; // Sử dụng các DTO dùng chung.
using PBL3.API.Extensions; // Sử dụng extension ToActionResult.

namespace PBL3.API.Controllers.Storefront // Khai báo namespace cho Controllers thuộc Storefront.
{
    [ApiController] // Khai báo đây là một API Controller có sẵn cơ chế validate model.
    [Route("api/cart")] // Định nghĩa route truy cập: api/cart.
    [Authorize] // Yêu cầu người dùng đăng nhập mới được truy cập các endpoint trong controller.
    public class CartController : ControllerBase // Định nghĩa lớp CartController kế thừa từ ControllerBase.
    {
        private readonly ICartService _cartService; // Khai báo trường dịch vụ giỏ hàng.

        public CartController(ICartService cartService) // Constructor injection tiêm ICartService.
        {
            _cartService = cartService; // Gán dịch vụ được tiêm.
        }

        private Guid GetUserId() // Hàm phụ trợ nội bộ lấy UserId từ JWT Claims.
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier); // Tìm Claim chứa định danh người dùng.
            if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId)) // Kiểm tra xem UserId có tồn tại hoặc hợp lệ không.
            {
                throw new UnauthorizedAccessException("Không thể xác định danh tính người dùng."); // Ném ra lỗi nếu chưa xác thực.
            }
            return userId; // Trả về UserId kiểu Guid hợp lệ.
        }

        [HttpGet] // Định nghĩa HTTP GET Method lấy giỏ hàng (api/cart).
        [ProducesResponseType(typeof(ApiResult<CartResponse>), 200)] // Trả về thông tin giỏ hàng thành công.
        public async Task<IActionResult> GetMyCart() // Lấy giỏ hàng của người dùng đang đăng nhập.
        {
            try // Bắt đầu khối bắt lỗi.
            {
                var userId = GetUserId(); // Lấy UserId người dùng.
                var result = await _cartService.GetMyCartAsync(userId); // Gọi service lấy chi tiết giỏ hàng kèm các sản phẩm bên trong.
                return Ok(result); // Trả về kết quả HTTP 200 OK.
            }
            catch (UnauthorizedAccessException ex) // Bắt lỗi chưa đăng nhập.
            {
                return Unauthorized(ApiResult<CartResponse>.Fail(ex.Message)); // Trả về lỗi 401 Unauthorized.
            }
            catch (Exception ex) // Bắt lỗi hệ thống.
            {
                return BadRequest(ApiResult<CartResponse>.Fail(ex.Message)); // Trả về lỗi 400 BadRequest.
            }
        }

        [HttpPost] // Định nghĩa HTTP POST Method thêm vào giỏ hàng (api/cart).
        [ProducesResponseType(typeof(ApiResult<CartResponse>), 200)] // Thêm thành công.
        public async Task<IActionResult> AddToCart([FromBody] AddToCartRequest request) // Thêm một sản phẩm vào giỏ hàng.
        {
            try // Bẫy lỗi.
            {
                var userId = GetUserId(); // Lấy UserId người dùng.
                var result = await _cartService.AddToCartAsync(userId, request); // Gọi service thực hiện thêm sản phẩm vào giỏ hàng.
                return result.ToActionResult(this); // Trả về kết quả qua extension ToActionResult.
            }
            catch (UnauthorizedAccessException ex) // Lỗi xác thực.
            {
                return Unauthorized(ApiResult<CartResponse>.Fail(ex.Message)); // Trả về lỗi 401.
            }
            catch (Exception ex) // Lỗi hệ thống.
            {
                return BadRequest(ApiResult<CartResponse>.Fail(ex.Message)); // Trả về lỗi 400.
            }
        }

        [HttpPut("items/{id}")] // Định nghĩa HTTP PUT Method cập nhật số lượng sản phẩm trong giỏ (api/cart/items/{id}).
        [ProducesResponseType(typeof(ApiResult<CartResponse>), 200)] // Cập nhật thành công.
        public async Task<IActionResult> UpdateQuantity(int id, [FromBody] UpdateCartItemRequest request) // Cập nhật số lượng của một mục sản phẩm trong giỏ hàng.
        {
            try // Bẫy lỗi.
            {
                var userId = GetUserId(); // Lấy UserId người dùng.
                var result = await _cartService.UpdateQuantityAsync(userId, id, request); // Gọi service cập nhật số lượng sản phẩm.
                return result.ToActionResult(this); // Trả về kết quả qua extension ToActionResult.
            }
            catch (UnauthorizedAccessException ex) // Lỗi xác thực.
            {
                return Unauthorized(ApiResult<CartResponse>.Fail(ex.Message)); // Trả về lỗi 401.
            }
            catch (Exception ex) // Lỗi hệ thống.
            {
                return BadRequest(ApiResult<CartResponse>.Fail(ex.Message)); // Trả về lỗi 400.
            }
        }

        [HttpDelete("items/{id}")] // Định nghĩa HTTP DELETE Method xóa sản phẩm khỏi giỏ (api/cart/items/{id}).
        [ProducesResponseType(typeof(ApiResult<CartResponse>), 200)] // Xóa thành công.
        public async Task<IActionResult> RemoveItem(int id) // Xóa bỏ hoàn toàn một mục sản phẩm khỏi giỏ hàng.
        {
            try // Bẫy lỗi.
            {
                var userId = GetUserId(); // Lấy UserId người dùng.
                var result = await _cartService.RemoveItemAsync(userId, id); // Gọi service xóa sản phẩm khỏi giỏ hàng.
                return result.ToActionResult(this); // Trả về kết quả qua extension ToActionResult.
            }
            catch (UnauthorizedAccessException ex) // Lỗi xác thực.
            {
                return Unauthorized(ApiResult<CartResponse>.Fail(ex.Message)); // Trả về lỗi 401.
            }
            catch (Exception ex) // Lỗi hệ thống.
            {
                return BadRequest(ApiResult<CartResponse>.Fail(ex.Message)); // Trả về lỗi 400.
            }
        }
    }
}

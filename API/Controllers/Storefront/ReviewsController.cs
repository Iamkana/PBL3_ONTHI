using System.Security.Claims; // Sử dụng để truy vấn thông tin người dùng từ JWT Claims.
using FluentValidation; // Sử dụng FluentValidation để kiểm tra dữ liệu đầu vào.
using Microsoft.AspNetCore.Authorization; // Sử dụng để phân quyền truy cập.
using Microsoft.AspNetCore.Mvc; // Sử dụng các thành phần MVC cho Web API.
using PBL3.Application.Reviews; // Sử dụng tầng nghiệp vụ đánh giá sản phẩm IProductReviewService.
using PBL3.Shared.DTOs.Common; // Sử dụng các DTO dùng chung.
using PBL3.Shared.DTOs.Reviews; // Sử dụng các DTO liên quan đến đánh giá sản phẩm.
using PBL3.API.Extensions; // Sử dụng extension ToActionResult.

namespace PBL3.API.Controllers.Storefront // Khai báo namespace cho Controllers thuộc Storefront.
{
    [ApiController] // Khai báo đây là một Web API Controller hỗ trợ tự động validate model.
    [Route("api/reviews")] // Định nghĩa route truy cập: api/reviews.
    [Produces("application/json")] // Quy định định dạng trả về mặc định dạng JSON.
    public class ReviewsController : ControllerBase // Định nghĩa lớp ReviewsController kế thừa từ ControllerBase.
    {
        private readonly IProductReviewService _reviewService; // Khai báo trường dịch vụ đánh giá sản phẩm.
        private readonly IValidator<CreateReviewRequest> _createValidator; // Khai báo bộ kiểm tra tính hợp lệ khi tạo đánh giá.

        public ReviewsController( // Constructor injection tiêm dịch vụ đánh giá và validator.
            IProductReviewService reviewService, // Tiêm review service.
            IValidator<CreateReviewRequest> createValidator) // Tiêm validator tạo đánh giá.
        {
            _reviewService = reviewService; // Gán dịch vụ đánh giá.
            _createValidator = createValidator; // Gán validator.
        }

        /// <summary>
        /// Lấy danh sách đánh giá của một sản phẩm (phân trang, công khai).
        /// </summary>
        [HttpGet] // Định nghĩa HTTP GET Method lấy danh sách đánh giá (api/reviews).
        [AllowAnonymous] // Cho phép tất cả mọi người (kể cả khách vãng lai) truy cập để xem bình luận/đánh giá.
        [ProducesResponseType(typeof(ApiResult<PagedResult<ReviewDto>>), StatusCodes.Status200OK)] // Trả về kết quả phân trang thành công.
        public async Task<IActionResult> GetReviews( // Lấy danh sách đánh giá của một sản phẩm cụ thể.
            [FromQuery] int productId, // Nhận productId từ query string.
            [FromQuery] int page = 1, // Trang mặc định là 1.
            [FromQuery] int pageSize = 10) // Số đánh giá mỗi trang mặc định là 10.
        {
            var result = await _reviewService.GetReviewsAsync(productId, page, pageSize); // Gọi service lấy danh sách đánh giá phân trang.
            return Ok(result); // Trả về HTTP 200 OK.
        }

        /// <summary>
        /// Tạo đánh giá mới. Yêu cầu đăng nhập, mỗi khách chỉ được đánh giá 1 lần / sản phẩm.
        /// </summary>
        [HttpPost] // Định nghĩa HTTP POST Method tạo mới đánh giá (api/reviews).
        [Authorize] // Yêu cầu khách hàng phải đăng nhập mới được viết đánh giá.
        [ProducesResponseType(typeof(ApiResult<ReviewDto>), StatusCodes.Status201Created)] // Tạo thành công trả về 201 Created.
        [ProducesResponseType(typeof(ApiResult<ReviewDto>), StatusCodes.Status400BadRequest)] // Dữ liệu không hợp lệ hoặc đã đánh giá sản phẩm này rồi.
        public async Task<IActionResult> CreateReview([FromBody] CreateReviewRequest request) // Viết đánh giá mới cho sản phẩm.
        {
            var validation = await _createValidator.ValidateAsync(request); // Thực hiện kiểm tra tính hợp lệ của dữ liệu đánh giá gửi lên.
            if (!validation.IsValid) // Nếu dữ liệu không hợp lệ.
            {
                var errors = string.Join("; ", validation.Errors.Select(e => e.ErrorMessage)); // Ghép các thông báo lỗi.
                return BadRequest(ApiResult<ReviewDto>.Fail(errors)); // Trả về lỗi 400 BadRequest kèm danh sách lỗi.
            }

            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier); // Lấy UserId từ JWT Claims.
            if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId)) // Kiểm tra xem người dùng đã được xác thực chưa.
                return Unauthorized(ApiResult<ReviewDto>.Fail("Người dùng chưa đăng nhập.")); // Trả về lỗi 401 Unauthorized.

            var result = await _reviewService.CreateReviewAsync(request, userId); // Gọi service tạo đánh giá cho sản phẩm.
            if (!result.Success) return result.ToActionResult(this); // Nếu có lỗi nghiệp vụ (chưa mua hàng, đã đánh giá...) trả về mã lỗi thích hợp.

            return CreatedAtAction(nameof(GetReviews), new { productId = request.ProductId }, result); // Trả về 201 Created kèm thông tin đánh giá mới tạo.
        }

        /// <summary>
        /// Xóa đánh giá. Chỉ chủ sở hữu mới được xóa.
        /// </summary>
        [HttpDelete("{reviewId:int}")] // Định nghĩa HTTP DELETE Method xóa đánh giá (api/reviews/{reviewId}).
        [Authorize] // Yêu cầu người dùng đăng nhập.
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status200OK)] // Xóa thành công.
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status400BadRequest)] // Xóa thất bại.
        public async Task<IActionResult> DeleteReview(int reviewId) // Xóa đánh giá sản phẩm theo reviewId.
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier); // Lấy UserId của người thực hiện yêu cầu xóa từ JWT Claims.
            if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId)) // Kiểm tra đăng nhập.
                return Unauthorized(ApiResult<bool>.Fail("Người dùng chưa đăng nhập.")); // Trả về lỗi 401.

            var result = await _reviewService.DeleteReviewAsync(reviewId, userId); // Gọi service xóa đánh giá (chỉ thành công nếu đúng chủ nhân hoặc Admin).
            return result.ToActionResult(this); // Trả về kết quả tương ứng.
        }
    }
}

using PBL3.Shared.DTOs.Common; // Sử dụng DTO dùng chung ApiResult và PagedResult.
using PBL3.Shared.DTOs.Reviews; // Sử dụng DTO của module đánh giá sản phẩm.

namespace PBL3.Application.Reviews // Khai báo namespace cho tầng Application của module đánh giá sản phẩm.
{
    public interface IProductReviewService // Định nghĩa giao diện dịch vụ đánh giá sản phẩm IProductReviewService.
    {
        Task<ApiResult<PagedResult<ReviewDto>>> GetReviewsAsync(int productId, int page, int pageSize); // Khai báo phương thức lấy danh sách đánh giá phân trang của một sản phẩm.
        Task<ApiResult<ReviewDto>> CreateReviewAsync(CreateReviewRequest request, Guid userId); // Khai báo phương thức tạo mới đánh giá sản phẩm bất đồng bộ.
        Task<ApiResult<bool>> DeleteReviewAsync(int reviewId, Guid userId); // Khai báo phương thức xóa đánh giá sản phẩm của người dùng.
    }
}

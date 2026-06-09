using Microsoft.EntityFrameworkCore; // Sử dụng Entity Framework Core.
using PBL3.Core.Entities; // Sử dụng thực thể ProductReview.
using PBL3.Core.Interfaces; // Sử dụng giao diện repository.
using PBL3.Infrastructure.Data; // Sử dụng DBContext.
using PBL3.Shared.DTOs.Common; // Sử dụng DTO dùng chung ApiResult.
using PBL3.Shared.DTOs.Reviews; // Sử dụng DTO của module đánh giá.

namespace PBL3.Application.Reviews // Khai báo namespace cho tầng Application của module đánh giá sản phẩm.
{
    public class ProductReviewService( // Định nghĩa lớp ProductReviewService sử dụng Primary Constructor.
        IProductReviewRepository reviewRepo, // Tiêm repository đánh giá sản phẩm.
        HushStoreDbContext context) : IProductReviewService // Tiêm DbContext và triển khai IProductReviewService.
    {
        private readonly IProductReviewRepository _reviewRepo = // Gán repository đánh giá vào trường thành viên.
            reviewRepo ?? throw new ArgumentNullException(nameof(reviewRepo)); // Kiểm tra null cho reviewRepo.
        private readonly HushStoreDbContext _context = // Gán DbContext vào trường thành viên.
            context ?? throw new ArgumentNullException(nameof(context)); // Kiểm tra null cho context.

        public async Task<ApiResult<PagedResult<ReviewDto>>> GetReviewsAsync( // Định nghĩa phương thức lấy danh sách đánh giá phân trang của một sản phẩm.
            int productId, int page, int pageSize) // Nhận mã sản phẩm, số trang và kích thước trang.
        {
            var (items, totalCount) = await _reviewRepo.GetPagedByProductIdAsync(productId, page, pageSize); // Lấy danh sách thực thể đánh giá phân trang và tổng số từ repo.
            return ApiResult<PagedResult<ReviewDto>>.Ok(new PagedResult<ReviewDto> // Trả về kết quả phân trang thành công chứa danh sách DTO.
            {
                Items = items.Select(MapToDto).ToList(), // Ánh xạ danh sách thực thể sang DTO.
                TotalCount = totalCount, // Gán tổng số lượng đánh giá.
                PageNumber = page, // Gán trang hiện tại.
                PageSize = pageSize // Gán kích thước trang.
            });
        }

        public async Task<ApiResult<ReviewDto>> CreateReviewAsync( // Định nghĩa phương thức tạo mới đánh giá sản phẩm.
            CreateReviewRequest request, Guid userId) // Nhận DTO yêu cầu và Id người dùng đánh giá.
        {
            var productExists = await _context.Products // Kiểm tra sản phẩm có tồn tại và đang hoạt động không.
                .AsNoTracking() // Truy vấn không theo dõi để tối ưu hiệu năng.
                .AnyAsync(p => p.Id == request.ProductId && p.Status == 1 && !p.IsDeleted); // Điều kiện sản phẩm hợp lệ.

            if (!productExists) // Nếu sản phẩm không tồn tại hoặc bị khóa/xóa.
                return ApiResult<ReviewDto>.Fail("Không tìm thấy sản phẩm yêu cầu.", ApiErrorCode.NotFound); // Báo lỗi NotFound.

            if (await _reviewRepo.ExistsAsync(request.ProductId, userId)) // Kiểm tra xem người dùng này đã từng đánh giá sản phẩm này chưa.
                return ApiResult<ReviewDto>.Fail("Bạn đã đánh giá sản phẩm này rồi."); // Báo lỗi nếu đã đánh giá rồi để tránh spam.

            var review = new ProductReview // Khởi tạo thực thể đánh giá sản phẩm mới.
            {
                ProductId   = request.ProductId, // Gán mã sản phẩm.
                UserId      = userId, // Gán mã người dùng.
                Rating      = request.Rating, // Gán số sao đánh giá.
                Title       = request.Title?.Trim(), // Gán tiêu đề đánh giá đã cắt khoảng trắng.
                Content     = request.Content?.Trim(), // Gán nội dung chi tiết đánh giá đã cắt khoảng trắng.
                CreatedDate = DateTime.UtcNow // Gán thời điểm tạo theo UTC hiện tại.
            };

            await _reviewRepo.AddAsync(review); // Thêm đánh giá vào cơ sở dữ liệu.
            await _reviewRepo.SaveChangesAsync(); // Lưu thay đổi.

            var profile = await _context.UserProfiles // Lấy thông tin hồ sơ của người dùng đánh giá.
                .AsNoTracking() // Không theo dõi để tăng tốc.
                .FirstOrDefaultAsync(p => p.UserId == userId); // Tìm hồ sơ theo UserId.

            return ApiResult<ReviewDto>.Ok(new ReviewDto // Trả về DTO đánh giá vừa tạo thành công.
            {
                Id            = review.Id, // Gán Id đánh giá.
                UserId        = userId, // Gán mã người dùng.
                UserFullName  = profile?.FullName ?? "Khách hàng", // Gán tên đầy đủ hoặc mặc định là Khách hàng.
                UserAvatarUrl = profile?.AvatarUrl, // Gán đường dẫn ảnh đại diện.
                Rating        = review.Rating, // Gán số sao.
                Title         = review.Title, // Gán tiêu đề.
                Content       = review.Content, // Gán nội dung.
                CreatedDate   = review.CreatedDate // Gán ngày tạo.
            }, "Đánh giá của bạn đã được ghi nhận."); // Thông điệp thành công.
        }

        public async Task<ApiResult<bool>> DeleteReviewAsync(int reviewId, Guid userId) // Định nghĩa phương thức xóa đánh giá của người dùng.
        {
            var review = await _reviewRepo.GetByIdWithTrackingAsync(reviewId); // Lấy thực thể đánh giá theo Id kèm chế độ tracking.

            if (review == null) // Nếu không tìm thấy đánh giá.
                return ApiResult<bool>.Fail("Không tìm thấy đánh giá yêu cầu.", ApiErrorCode.NotFound); // Báo lỗi NotFound.

            if (review.UserId != userId) // Kiểm tra quyền sở hữu: chỉ có chính người dùng đó mới được xóa đánh giá của mình.
                return ApiResult<bool>.Fail("Bạn không có quyền xóa đánh giá này.", ApiErrorCode.Forbidden); // Báo lỗi Forbidden 403.

            review.IsDeleted   = true; // Thực hiện xóa mềm bằng cách đánh dấu IsDeleted.
            review.DeletedDate = DateTime.UtcNow; // Ghi nhận thời gian xóa.

            await _reviewRepo.SaveChangesAsync(); // Lưu thay đổi xuống cơ sở dữ liệu.
            return ApiResult<bool>.Ok(true, "Đánh giá đã được xóa."); // Trả về kết quả xóa thành công.
        }

        private static ReviewDto MapToDto(ProductReview r) => new() // Định nghĩa hàm hỗ trợ ánh xạ thực thể sang DTO.
        {
            Id            = r.Id, // Ánh xạ Id.
            UserId        = r.UserId, // Ánh xạ UserId.
            UserFullName  = r.User?.Profile?.FullName ?? r.User?.UserName ?? "Khách hàng", // Ánh xạ tên đầy đủ của người đánh giá.
            UserAvatarUrl = r.User?.Profile?.AvatarUrl, // Ánh xạ ảnh đại diện.
            Rating        = r.Rating, // Ánh xạ số sao đánh giá.
            Title         = r.Title, // Ánh xạ tiêu đề.
            Content       = r.Content, // Ánh xạ nội dung.
            CreatedDate   = r.CreatedDate // Ánh xạ ngày đánh giá.
        };
    }
}

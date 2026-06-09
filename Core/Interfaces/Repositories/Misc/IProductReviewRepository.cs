using PBL3.Core.Entities; // Nhập namespace chứa các thực thể nghiệp vụ cốt lõi của hệ thống.

namespace PBL3.Core.Interfaces // Định nghĩa namespace PBL3.Core.Interfaces để quản lý thực thể/giao diện.
{
    /// <summary>
    /// Repository interface cho ProductReview (Đánh giá sản phẩm).
    /// </summary>
    public interface IProductReviewRepository // Định nghĩa giao diện (interface) IProductReviewRepository quy định hợp đồng cho tầng dữ liệu.
    {
        /// <summary>
        /// Lấy danh sách đánh giá theo productId có phân trang, sắp xếp mới nhất trước.
        /// </summary>
        Task<(List<ProductReview> Items, int TotalCount)> GetPagedByProductIdAsync( // Khai báo thành phần cấu trúc nghiệp vụ.
            int productId, int pageNumber, int pageSize); // Khai báo thành phần cấu trúc nghiệp vụ.

        /// <summary>
        /// Kiểm tra khách hàng đã đánh giá sản phẩm này chưa.
        /// </summary>
        Task<bool> ExistsAsync(int productId, Guid userId); // Định nghĩa phương thức bất đồng bộ 'ExistsAsync' với tham số (productId, userId) trả về kiểu Task<bool>.

        Task<ProductReview?> GetByIdAsync(int id); // Định nghĩa phương thức bất đồng bộ 'GetByIdAsync' với tham số (id) trả về kiểu Task<ProductReview?>.

        /// <summary>
        /// Lấy đánh giá theo Id (WITH TRACKING để soft-delete).
        /// </summary>
        Task<ProductReview?> GetByIdWithTrackingAsync(int id); // Định nghĩa phương thức bất đồng bộ 'GetByIdWithTrackingAsync' với tham số (id) trả về kiểu Task<ProductReview?>.

        Task AddAsync(ProductReview review); // Định nghĩa phương thức bất đồng bộ 'AddAsync' với tham số (review) trả về kiểu Task.
        Task SaveChangesAsync(); // Định nghĩa phương thức bất đồng bộ 'SaveChangesAsync' không tham số trả về kiểu Task.
    }
}

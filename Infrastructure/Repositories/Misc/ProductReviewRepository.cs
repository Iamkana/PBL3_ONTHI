using Microsoft.EntityFrameworkCore; // Nhập thư viện Microsoft.EntityFrameworkCore để làm việc với CSDL qua ORM.
using PBL3.Core.Entities; // Nhập namespace chứa các thực thể nghiệp vụ cốt lưu trữ của hệ thống.
using PBL3.Core.Interfaces; // Nhập namespace chứa các giao diện (interface) dùng chung của hệ thống.
using PBL3.Infrastructure.Data; // Nhập namespace chứa DbContext và các cấu hình dữ liệu của Infrastructure.

namespace PBL3.Infrastructure.Repositories // Định nghĩa namespace PBL3.Infrastructure.Repositories quản lý cấu trúc code.
{
    public class ProductReviewRepository(HushStoreDbContext context) : IProductReviewRepository // Định nghĩa lớp ProductReviewRepository sử dụng Primary Constructor nhận (HushStoreDbContext context) triển khai/kế thừa IProductReviewRepository.
    {
        private readonly HushStoreDbContext _context = // Khai báo trường dữ liệu private readonly và bắt đầu gán giá trị.
            context ?? throw new ArgumentNullException(nameof(context)); // Kiểm tra null và ném ra ngoại lệ ArgumentNullException nếu đối tượng context bị null.

        public async Task<(List<ProductReview> Items, int TotalCount)> GetPagedByProductIdAsync( // Thực hiện xử lý bất đồng bộ phương thức 'Task<' nhận tham số (Items, TotalCount) trả về kiểu .
            int productId, int pageNumber, int pageSize) // Thực thi dòng lệnh nghiệp vụ.
        {
            var query = _context.ProductReviews // Thực hiện gán giá trị của biểu thức '_context.ProductReviews' cho biến 'query'.
                .AsNoTracking() // Tắt tính năng theo dõi trạng thái thực thể (AsNoTracking) để tối ưu hiệu năng đọc dữ liệu.
                .Where(r => r.ProductId == productId) // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(r => r.ProductId == productId).
                .Include(r => r.User).ThenInclude(u => u.Profile); // Nạp kèm (Eager load) dữ liệu quan hệ liên quan: .Include(r => r.User).ThenInclude(u => u.Profile);.

            var totalCount = await query.CountAsync(); // Tính toán tổng số lượng bản ghi thỏa mãn điều kiện bất đồng bộ.
            var items = await query // Chờ tác vụ bất đồng bộ hoàn thành và gán kết quả cho biến 'items'.
                .OrderByDescending(r => r.CreatedDate) // Sắp xếp danh sách kết quả theo thứ tự giảm dần.
                .Skip((pageNumber - 1) * pageSize) // Thực thi dòng lệnh nghiệp vụ.
                .Take(pageSize) // Thực thi dòng lệnh nghiệp vụ.
                .ToListAsync(); // Truy vấn bất đồng bộ và chuyển đổi kết quả trả về dạng danh sách List.

            return (items, totalCount); // Trả về giá trị của biểu thức '(items, totalCount)'.
        }

        public Task<bool> ExistsAsync(int productId, Guid userId) => // Thực hiện xử lý phương thức 'ExistsAsync' nhận tham số (productId, userId) trả về kiểu Task<bool>.
            _context.ProductReviews.AnyAsync(r => r.ProductId == productId && r.UserId == userId); // Kiểm tra xem có bất kỳ bản ghi nào khớp với điều kiện hay không.

        public Task<ProductReview?> GetByIdAsync(int id) => // Thực hiện xử lý phương thức 'GetByIdAsync' nhận tham số (id) trả về kiểu Task<ProductReview?>.
            _context.ProductReviews.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id); // Tắt tính năng theo dõi trạng thái thực thể (AsNoTracking) để tối ưu hiệu năng đọc dữ liệu.

        public Task<ProductReview?> GetByIdWithTrackingAsync(int id) => // Thực hiện xử lý phương thức 'GetByIdWithTrackingAsync' nhận tham số (id) trả về kiểu Task<ProductReview?>.
            _context.ProductReviews.FirstOrDefaultAsync(r => r.Id == id); // Lấy phần tử đầu tiên thỏa mãn điều kiện hoặc trả về null nếu không tìm thấy.

        public async Task AddAsync(ProductReview review) => // Thực hiện xử lý bất đồng bộ phương thức 'AddAsync' nhận tham số (review) trả về kiểu Task.
            await _context.ProductReviews.AddAsync(review); // Thêm thực thể mới vào CSDL thông qua DbSet tương ứng.

        public Task SaveChangesAsync() => _context.SaveChangesAsync(); // Thực thi dòng lệnh nghiệp vụ.
    }
}

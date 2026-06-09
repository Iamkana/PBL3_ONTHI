using Microsoft.EntityFrameworkCore; // Nhập thư viện Microsoft.EntityFrameworkCore để làm việc với CSDL qua ORM.
using PBL3.Core.Entities; // Nhập namespace chứa các thực thể nghiệp vụ cốt lưu trữ của hệ thống.
using PBL3.Core.Interfaces; // Nhập namespace chứa các giao diện (interface) dùng chung của hệ thống.
using PBL3.Infrastructure.Data; // Nhập namespace chứa DbContext và các cấu hình dữ liệu của Infrastructure.
using System; // Nhập thư viện hệ thống cơ bản.
using System.Collections.Generic; // Nhập thư viện làm việc với các kiểu danh sách, bộ sưu tập.
using System.Linq; // Nhập thư viện LINQ hỗ trợ truy vấn dữ liệu nhanh chóng.
using System.Threading.Tasks; // Nhập thư viện hỗ trợ lập trình bất đồng bộ.

namespace PBL3.Infrastructure.Repositories // Định nghĩa namespace PBL3.Infrastructure.Repositories quản lý cấu trúc code.
{
    public class CartRepository(HushStoreDbContext context) : ICartRepository // Định nghĩa lớp CartRepository sử dụng Primary Constructor nhận (HushStoreDbContext context) triển khai/kế thừa ICartRepository.
    {
        private readonly HushStoreDbContext _context = // Khai báo trường dữ liệu private readonly và bắt đầu gán giá trị.
            context ?? throw new ArgumentNullException(nameof(context)); // Kiểm tra null và ném ra ngoại lệ ArgumentNullException nếu đối tượng context bị null.

        public async Task<List<Cart>> GetCartItemsByUserAsync(Guid userId) // Thực hiện xử lý bất đồng bộ phương thức 'GetCartItemsByUserAsync' nhận tham số (userId) trả về kiểu Task<List<Cart>>.
        {
            return await _context.Carts // Chờ và trả về giá trị của tác vụ bất đồng bộ '_context.Carts'.
                .Include(c => c.Variant) // Nạp kèm (Eager load) dữ liệu quan hệ liên quan: .Include(c => c.Variant).
                    .ThenInclude(v => v.Product) // Thực thi dòng lệnh nghiệp vụ.
                .Include(c => c.Variant) // Nạp kèm (Eager load) dữ liệu quan hệ liên quan: .Include(c => c.Variant).
                    .ThenInclude(v => v.Images) // Thực thi dòng lệnh nghiệp vụ.
                .Where(c => c.UserId == userId) // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(c => c.UserId == userId).
                .AsNoTracking() // Tắt tính năng theo dõi trạng thái thực thể (AsNoTracking) để tối ưu hiệu năng đọc dữ liệu.
                .OrderByDescending(c => c.CreatedDate) // Sắp xếp danh sách kết quả theo thứ tự giảm dần.
                .ToListAsync(); // Truy vấn bất đồng bộ và chuyển đổi kết quả trả về dạng danh sách List.
        }

        public async Task<List<Cart>> GetCartItemsWithTrackingAsync(Guid userId) // Thực hiện xử lý bất đồng bộ phương thức 'GetCartItemsWithTrackingAsync' nhận tham số (userId) trả về kiểu Task<List<Cart>>.
        {
            return await _context.Carts // Chờ và trả về giá trị của tác vụ bất đồng bộ '_context.Carts'.
                .Include(c => c.Variant) // Nạp kèm (Eager load) dữ liệu quan hệ liên quan: .Include(c => c.Variant).
                    .ThenInclude(v => v.Product) // Thực thi dòng lệnh nghiệp vụ.
                .Where(c => c.UserId == userId) // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(c => c.UserId == userId).
                .ToListAsync(); // Truy vấn bất đồng bộ và chuyển đổi kết quả trả về dạng danh sách List.
        }

        public async Task<Cart?> GetCartItemAsync(int cartItemId, Guid userId) // Thực hiện xử lý bất đồng bộ phương thức 'GetCartItemAsync' nhận tham số (cartItemId, userId) trả về kiểu Task<Cart?>.
        {
            return await _context.Carts // Chờ và trả về giá trị của tác vụ bất đồng bộ '_context.Carts'.
                .FirstOrDefaultAsync(c => c.Id == cartItemId && c.UserId == userId); // Lấy phần tử đầu tiên thỏa mãn điều kiện hoặc trả về null nếu không tìm thấy.
        }

        public async Task<Cart?> FindByUserAndVariantAsync(Guid userId, int variantId) // Thực hiện xử lý bất đồng bộ phương thức 'FindByUserAndVariantAsync' nhận tham số (userId, variantId) trả về kiểu Task<Cart?>.
        {
            return await _context.Carts // Chờ và trả về giá trị của tác vụ bất đồng bộ '_context.Carts'.
                .FirstOrDefaultAsync(c => c.UserId == userId && c.VariantId == variantId); // Lấy phần tử đầu tiên thỏa mãn điều kiện hoặc trả về null nếu không tìm thấy.
        }

        public async Task AddAsync(Cart cart) // Thực hiện xử lý bất đồng bộ phương thức 'AddAsync' nhận tham số (cart) trả về kiểu Task.
        {
            await _context.Carts.AddAsync(cart); // Thêm thực thể mới vào CSDL thông qua DbSet tương ứng.
        }

        public void Remove(Cart cart) // Thực hiện xử lý phương thức 'Remove' nhận tham số (cart) trả về kiểu void.
        {
            _context.Carts.Remove(cart); // Xóa thực thể khỏi CSDL thông qua DbSet tương ứng.
        }

        public void RemoveRange(IEnumerable<Cart> carts) // Thực hiện xử lý phương thức 'RemoveRange' nhận tham số (carts) trả về kiểu void.
        {
            _context.Carts.RemoveRange(carts); // Thực thi dòng lệnh nghiệp vụ.
        }

        public async Task SaveChangesAsync() // Thực hiện xử lý bất đồng bộ phương thức 'SaveChangesAsync' không tham số trả về kiểu Task.
        {
            await _context.SaveChangesAsync(); // Thực thi dòng lệnh nghiệp vụ.
        }
    }
}

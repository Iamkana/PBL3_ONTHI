using System; // Nhập thư viện hệ thống cơ bản.
using System.Linq; // Nhập thư viện LINQ hỗ trợ truy vấn dữ liệu nhanh chóng.
using System.Threading.Tasks; // Nhập thư viện hỗ trợ lập trình bất đồng bộ.
using Microsoft.EntityFrameworkCore; // Nhập thư viện Microsoft.EntityFrameworkCore để làm việc với CSDL qua ORM.
using PBL3.Core.Entities; // Nhập namespace chứa các thực thể nghiệp vụ cốt lưu trữ của hệ thống.
using PBL3.Core.Interfaces; // Nhập namespace chứa các giao diện (interface) dùng chung của hệ thống.
using PBL3.Infrastructure.Data; // Nhập namespace chứa DbContext và các cấu hình dữ liệu của Infrastructure.

namespace PBL3.Infrastructure.Repositories // Định nghĩa namespace PBL3.Infrastructure.Repositories quản lý cấu trúc code.
{
    public class OrderRepository(HushStoreDbContext dbContext) : IOrderRepository // Định nghĩa lớp OrderRepository sử dụng Primary Constructor nhận (HushStoreDbContext dbContext) triển khai/kế thừa IOrderRepository.
    {
        private readonly HushStoreDbContext _dbContext = // Khai báo trường dữ liệu private readonly và bắt đầu gán giá trị.
            dbContext ?? throw new ArgumentNullException(nameof(dbContext)); // Kiểm tra null và ném ra ngoại lệ ArgumentNullException nếu đối tượng context bị null.

        public IQueryable<Order> GetQueryable() // Thực thi dòng lệnh nghiệp vụ.
        {
            return _dbContext.Orders.AsQueryable(); // Chuyển cấu trúc dữ liệu thành IQueryable để hoãn việc thực thi truy vấn.
        }

        public async Task<Order?> GetByIdAsync(int id) // Thực hiện xử lý bất đồng bộ phương thức 'GetByIdAsync' nhận tham số (id) trả về kiểu Task<Order?>.
        {
            return await _dbContext.Orders.FindAsync(id); // Chờ và trả về giá trị của tác vụ bất đồng bộ '_dbContext.Orders.FindAsync(id)'.
        }

        public async Task<Order?> GetByIdWithDetailsAsync(int id) // Thực hiện xử lý bất đồng bộ phương thức 'GetByIdWithDetailsAsync' nhận tham số (id) trả về kiểu Task<Order?>.
        {
            return await _dbContext.Orders.AsNoTracking() // Tắt tính năng theo dõi trạng thái thực thể (AsNoTracking) để tối ưu hiệu năng đọc dữ liệu.
                .Include(o => o.OrderDetails) // Nạp kèm (Eager load) dữ liệu quan hệ liên quan: .Include(o => o.OrderDetails).
                    .ThenInclude(od => od.Variant) // Thực thi dòng lệnh nghiệp vụ.
                        .ThenInclude(v => v.Images) // Thực thi dòng lệnh nghiệp vụ.
                .Include(o => o.OrderDetails) // Nạp kèm (Eager load) dữ liệu quan hệ liên quan: .Include(o => o.OrderDetails).
                    .ThenInclude(od => od.OrderSerials) // Thực thi dòng lệnh nghiệp vụ.
                        .ThenInclude(os => os.Serial) // Thực thi dòng lệnh nghiệp vụ.
                .Include(o => o.VoucherUsages) // Nạp kèm (Eager load) dữ liệu quan hệ liên quan: .Include(o => o.VoucherUsages).
                    .ThenInclude(vu => vu.Voucher) // Thực thi dòng lệnh nghiệp vụ.
                .Include(o => o.User) // Nạp kèm (Eager load) dữ liệu quan hệ liên quan: .Include(o => o.User).
                .FirstOrDefaultAsync(o => o.Id == id); // Lấy phần tử đầu tiên thỏa mãn điều kiện hoặc trả về null nếu không tìm thấy.
        }

        /// <summary>
        /// Load Order kèm Details/Serials WITH TRACKING — dùng cho các thao tác ghi (xuất kho).
        /// </summary>
        public async Task<Order?> GetByIdWithDetailsTrackedAsync(int id) // Thực hiện xử lý bất đồng bộ phương thức 'GetByIdWithDetailsTrackedAsync' nhận tham số (id) trả về kiểu Task<Order?>.
        {
            return await _dbContext.Orders // Chờ và trả về giá trị của tác vụ bất đồng bộ '_dbContext.Orders'.
                .Include(o => o.OrderDetails) // Nạp kèm (Eager load) dữ liệu quan hệ liên quan: .Include(o => o.OrderDetails).
                    .ThenInclude(od => od.Variant) // Thực thi dòng lệnh nghiệp vụ.
                .Include(o => o.OrderDetails) // Nạp kèm (Eager load) dữ liệu quan hệ liên quan: .Include(o => o.OrderDetails).
                    .ThenInclude(od => od.OrderSerials) // Thực thi dòng lệnh nghiệp vụ.
                        .ThenInclude(os => os.Serial) // Thực thi dòng lệnh nghiệp vụ.
                .Include(o => o.VoucherUsages) // Nạp kèm (Eager load) dữ liệu quan hệ liên quan: .Include(o => o.VoucherUsages).
                    .ThenInclude(vu => vu.Voucher) // Thực thi dòng lệnh nghiệp vụ.
                .Include(o => o.User) // Nạp kèm (Eager load) dữ liệu quan hệ liên quan: .Include(o => o.User).
                .FirstOrDefaultAsync(o => o.Id == id); // Lấy phần tử đầu tiên thỏa mãn điều kiện hoặc trả về null nếu không tìm thấy.
        }

        public async Task<string?> GetLastOrderCodeByDateAsync(string datePrefix) // Thực hiện xử lý bất đồng bộ phương thức 'GetLastOrderCodeByDateAsync' nhận tham số (datePrefix) trả về kiểu Task<string?>.
        {
            return await _dbContext.Orders // Chờ và trả về giá trị của tác vụ bất đồng bộ '_dbContext.Orders'.
                .Where(o => o.OrderCode.StartsWith(datePrefix)) // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(o => o.OrderCode.StartsWith(datePrefix)).
                .OrderByDescending(o => o.OrderCode) // Sắp xếp danh sách kết quả theo thứ tự giảm dần.
                .Select(o => o.OrderCode) // Thực thi dòng lệnh nghiệp vụ.
                .FirstOrDefaultAsync(); // Lấy phần tử đầu tiên thỏa mãn điều kiện hoặc trả về null nếu không tìm thấy.
        }

        public async Task AddAsync(Order order) // Thực hiện xử lý bất đồng bộ phương thức 'AddAsync' nhận tham số (order) trả về kiểu Task.
        {
            await _dbContext.Orders.AddAsync(order); // Thêm thực thể mới vào CSDL thông qua DbSet tương ứng.
        }

        public async Task<List<Order>> GetDraftsByEmployeeAsync(Guid employeeId) // Thực hiện xử lý bất đồng bộ phương thức 'GetDraftsByEmployeeAsync' nhận tham số (employeeId) trả về kiểu Task<List<Order>>.
        {
            return await _dbContext.Orders // Chờ và trả về giá trị của tác vụ bất đồng bộ '_dbContext.Orders'.
                .Where(o => o.Status == 6 && o.EmployeeId == employeeId) // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(o => o.Status == 6 && o.EmployeeId == employeeId).
                .OrderByDescending(o => o.OrderDate) // Sắp xếp danh sách kết quả theo thứ tự giảm dần.
                .ToListAsync(); // Truy vấn bất đồng bộ và chuyển đổi kết quả trả về dạng danh sách List.
        }

        public async Task<Dictionary<int, int>> GetActiveOrderQuantitiesByVariantIdsAsync(List<int> variantIds) // Thực hiện xử lý bất đồng bộ phương thức 'GetActiveOrderQuantitiesByVariantIdsAsync' nhận tham số (variantIds) trả về kiểu Task<Dictionary<int, int>>.
        {
            // Single query: JOIN Orders + OrderDetails
            // WHERE Order.Status IN (0, 1) AND OrderDetail.VariantId IN (@variantIds)
            // GROUP BY VariantId -> SUM(Quantity)
            return await _dbContext.OrderDetails // Chờ và trả về giá trị của tác vụ bất đồng bộ '_dbContext.OrderDetails'.
                .Where(od => (od.Order.Status == 0 || od.Order.Status == 1) // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(od => (od.Order.Status == 0 || od.Order.Status == 1).
                             && variantIds.Contains(od.VariantId)) // Thực thi dòng lệnh nghiệp vụ.
                .GroupBy(od => od.VariantId) // Thực thi dòng lệnh nghiệp vụ.
                .ToDictionaryAsync(g => g.Key, g => g.Sum(od => od.Quantity)); // Thực thi dòng lệnh nghiệp vụ.
        }

        public async Task SaveChangesAsync() // Thực hiện xử lý bất đồng bộ phương thức 'SaveChangesAsync' không tham số trả về kiểu Task.
        {
            await _dbContext.SaveChangesAsync(); // Thực thi dòng lệnh nghiệp vụ.
        }
    }
}

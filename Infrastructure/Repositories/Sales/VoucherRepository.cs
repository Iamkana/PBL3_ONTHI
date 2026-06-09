using System; // Nhập thư viện hệ thống cơ bản.
using System.Collections.Generic; // Nhập thư viện làm việc với các kiểu danh sách, bộ sưu tập.
using System.Linq; // Nhập thư viện LINQ hỗ trợ truy vấn dữ liệu nhanh chóng.
using System.Threading.Tasks; // Nhập thư viện hỗ trợ lập trình bất đồng bộ.
using Microsoft.EntityFrameworkCore; // Nhập thư viện Microsoft.EntityFrameworkCore để làm việc với CSDL qua ORM.
using PBL3.Core.Entities; // Nhập namespace chứa các thực thể nghiệp vụ cốt lưu trữ của hệ thống.
using PBL3.Core.Interfaces; // Nhập namespace chứa các giao diện (interface) dùng chung của hệ thống.
using PBL3.Infrastructure.Data; // Nhập namespace chứa DbContext và các cấu hình dữ liệu của Infrastructure.

namespace PBL3.Infrastructure.Repositories // Định nghĩa namespace PBL3.Infrastructure.Repositories quản lý cấu trúc code.
{
    public class VoucherRepository(HushStoreDbContext dbContext) : IVoucherRepository // Định nghĩa lớp VoucherRepository sử dụng Primary Constructor nhận (HushStoreDbContext dbContext) triển khai/kế thừa IVoucherRepository.
    {
        private readonly HushStoreDbContext _dbContext = // Khai báo trường dữ liệu private readonly và bắt đầu gán giá trị.
            dbContext ?? throw new ArgumentNullException(nameof(dbContext)); // Kiểm tra null và ném ra ngoại lệ ArgumentNullException nếu đối tượng context bị null.

        // ========================================================
        // MANAGEMENT CRUD
        // ========================================================

        public async Task<(List<Voucher> Items, int TotalCount)> GetPagedListAsync( // Thực hiện xử lý bất đồng bộ phương thức 'Task<' nhận tham số (Items, TotalCount) trả về kiểu .
            string? keyword, // Thực thi dòng lệnh nghiệp vụ.
            string? statusFilter, // Thực thi dòng lệnh nghiệp vụ.
            DateTime? fromDate, // Thực thi dòng lệnh nghiệp vụ.
            DateTime? toDate, // Thực thi dòng lệnh nghiệp vụ.
            int pageNumber, // Thực thi dòng lệnh nghiệp vụ.
            int pageSize, // Thực thi dòng lệnh nghiệp vụ.
            string? sortBy, // Thực thi dòng lệnh nghiệp vụ.
            bool sortDescending) // Thực thi dòng lệnh nghiệp vụ.
        {
            var query = _dbContext.Vouchers.AsNoTracking().AsQueryable(); // Tắt tính năng theo dõi trạng thái thực thể (AsNoTracking) để tối ưu hiệu năng đọc dữ liệu.

            if (!string.IsNullOrWhiteSpace(keyword)) // Kiểm tra điều kiện: '!string.IsNullOrWhiteSpace(keyword'.
            {
                var kw = keyword.Trim().ToLower(); // Thực hiện gán giá trị của biểu thức 'keyword.Trim().ToLower()' cho biến 'kw'.
                query = query.Where(v => // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(v =>.
                    v.Code.ToLower().Contains(kw) || // Thực thi dòng lệnh nghiệp vụ.
                    v.Name.ToLower().Contains(kw)); // Thực thi dòng lệnh nghiệp vụ.
            }

            if (!string.IsNullOrWhiteSpace(statusFilter)) // Kiểm tra điều kiện: '!string.IsNullOrWhiteSpace(statusFilter'.
            {
                var now = DateTime.UtcNow; // Thực hiện gán giá trị của biểu thức 'DateTime.UtcNow' cho biến 'now'.
                query = statusFilter.ToLower() switch // Thực hiện gán giá trị của biểu thức 'statusFilter.ToLower() switch' cho biến 'query'.
                {
                    "active"    => query.Where(v => v.IsActive && v.StartDate <= now && v.EndDate >= now // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(v => v.IsActive && v.StartDate <= now && v.EndDate >= now.
                                       && (v.Quantity == null || v.UsedCount < v.Quantity)), // Thực thi dòng lệnh nghiệp vụ.
                    "upcoming"  => query.Where(v => v.IsActive && v.StartDate > now), // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(v => v.IsActive && v.StartDate > now),.
                    "expired"   => query.Where(v => v.EndDate < now), // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(v => v.EndDate < now),.
                    "exhausted" => query.Where(v => v.Quantity.HasValue && v.UsedCount >= v.Quantity), // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(v => v.Quantity.HasValue && v.UsedCount >= v.Quantity),.
                    "paused"    => query.Where(v => !v.IsActive), // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(v => !v.IsActive),.
                    _           => query // Thực hiện gán giá trị của biểu thức '> query' cho biến '_'.
                };
            }

            if (fromDate.HasValue) // Kiểm tra điều kiện: 'fromDate.HasValue'.
                query = query.Where(v => v.EndDate >= fromDate.Value); // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(v => v.EndDate >= fromDate.Value);.

            if (toDate.HasValue) // Kiểm tra điều kiện: 'toDate.HasValue'.
                query = query.Where(v => v.StartDate <= toDate.Value); // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(v => v.StartDate <= toDate.Value);.

            var totalCount = await query.CountAsync(); // Tính toán tổng số lượng bản ghi thỏa mãn điều kiện bất đồng bộ.

            query = sortBy?.ToLower() switch // Thực hiện gán giá trị của biểu thức 'sortBy?.ToLower() switch' cho biến 'query'.
            {
                "code"      => sortDescending ? query.OrderByDescending(v => v.Code) : query.OrderBy(v => v.Code), // Sắp xếp danh sách kết quả theo thứ tự giảm dần.
                "name"      => sortDescending ? query.OrderByDescending(v => v.Name) : query.OrderBy(v => v.Name), // Sắp xếp danh sách kết quả theo thứ tự giảm dần.
                "startdate" => sortDescending ? query.OrderByDescending(v => v.StartDate) : query.OrderBy(v => v.StartDate), // Sắp xếp danh sách kết quả theo thứ tự giảm dần.
                "enddate"   => sortDescending ? query.OrderByDescending(v => v.EndDate) : query.OrderBy(v => v.EndDate), // Sắp xếp danh sách kết quả theo thứ tự giảm dần.
                "usedcount" => sortDescending ? query.OrderByDescending(v => v.UsedCount) : query.OrderBy(v => v.UsedCount), // Sắp xếp danh sách kết quả theo thứ tự giảm dần.
                _           => query.OrderByDescending(v => v.CreatedDate) // Sắp xếp danh sách kết quả theo thứ tự giảm dần.
            };

            var items = await query // Chờ tác vụ bất đồng bộ hoàn thành và gán kết quả cho biến 'items'.
                .Skip((pageNumber - 1) * pageSize) // Thực thi dòng lệnh nghiệp vụ.
                .Take(pageSize) // Thực thi dòng lệnh nghiệp vụ.
                .Include(v => v.VoucherCategories) // Nạp kèm (Eager load) dữ liệu quan hệ liên quan: .Include(v => v.VoucherCategories).
                .ToListAsync(); // Truy vấn bất đồng bộ và chuyển đổi kết quả trả về dạng danh sách List.

            return (items, totalCount); // Trả về giá trị của biểu thức '(items, totalCount)'.
        }

        public async Task<Voucher?> GetByIdWithCategoriesAsync(int id) // Thực hiện xử lý bất đồng bộ phương thức 'GetByIdWithCategoriesAsync' nhận tham số (id) trả về kiểu Task<Voucher?>.
        {
            return await _dbContext.Vouchers // Chờ và trả về giá trị của tác vụ bất đồng bộ '_dbContext.Vouchers'.
                .Include(v => v.VoucherCategories) // Nạp kèm (Eager load) dữ liệu quan hệ liên quan: .Include(v => v.VoucherCategories).
                .FirstOrDefaultAsync(v => v.Id == id); // Lấy phần tử đầu tiên thỏa mãn điều kiện hoặc trả về null nếu không tìm thấy.
        }

        public async Task<Voucher?> GetByIdNoTrackingAsync(int id) // Thực hiện xử lý bất đồng bộ phương thức 'GetByIdNoTrackingAsync' nhận tham số (id) trả về kiểu Task<Voucher?>.
        {
            return await _dbContext.Vouchers // Chờ và trả về giá trị của tác vụ bất đồng bộ '_dbContext.Vouchers'.
                .AsNoTracking() // Tắt tính năng theo dõi trạng thái thực thể (AsNoTracking) để tối ưu hiệu năng đọc dữ liệu.
                .FirstOrDefaultAsync(v => v.Id == id); // Lấy phần tử đầu tiên thỏa mãn điều kiện hoặc trả về null nếu không tìm thấy.
        }

        public async Task<bool> IsDuplicateCodeAsync(string code, int? excludeId = null) // Thực hiện xử lý bất đồng bộ phương thức 'IsDuplicateCodeAsync' nhận tham số (code, null) trả về kiểu Task<bool>.
        {
            var normalized = code.Trim().ToUpper(); // Thực hiện gán giá trị của biểu thức 'code.Trim().ToUpper()' cho biến 'normalized'.
            return await _dbContext.Vouchers // Chờ và trả về giá trị của tác vụ bất đồng bộ '_dbContext.Vouchers'.
                .AnyAsync(v => // Kiểm tra xem có bất kỳ bản ghi nào khớp với điều kiện hay không.
                    v.Code.ToUpper() == normalized && // Thực thi dòng lệnh nghiệp vụ.
                    (excludeId == null || v.Id != excludeId)); // Thực thi dòng lệnh nghiệp vụ.
        }

        public async Task AddAsync(Voucher voucher) // Thực hiện xử lý bất đồng bộ phương thức 'AddAsync' nhận tham số (voucher) trả về kiểu Task.
        {
            await _dbContext.Vouchers.AddAsync(voucher); // Thêm thực thể mới vào CSDL thông qua DbSet tương ứng.
        }

        // ========================================================
        // CHECKOUT USAGE
        // ========================================================

        public async Task<List<Voucher>> GetByCodesWithCategoriesAsync(List<string> codes) // Thực hiện xử lý bất đồng bộ phương thức 'GetByCodesWithCategoriesAsync' nhận tham số (codes) trả về kiểu Task<List<Voucher>>.
        {
            return await _dbContext.Vouchers // Chờ và trả về giá trị của tác vụ bất đồng bộ '_dbContext.Vouchers'.
                .Include(v => v.VoucherCategories) // Nạp kèm (Eager load) dữ liệu quan hệ liên quan: .Include(v => v.VoucherCategories).
                .Where(v => codes.Contains(v.Code)) // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(v => codes.Contains(v.Code)).
                .ToListAsync(); // Truy vấn bất đồng bộ và chuyển đổi kết quả trả về dạng danh sách List.
        }

        public async Task<List<Voucher>> GetByCodesAsync(List<string> codes) // Thực hiện xử lý bất đồng bộ phương thức 'GetByCodesAsync' nhận tham số (codes) trả về kiểu Task<List<Voucher>>.
        {
            return await _dbContext.Vouchers // Chờ và trả về giá trị của tác vụ bất đồng bộ '_dbContext.Vouchers'.
                .Where(v => codes.Contains(v.Code)) // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(v => codes.Contains(v.Code)).
                .ToListAsync(); // Truy vấn bất đồng bộ và chuyển đổi kết quả trả về dạng danh sách List.
        }

        public async Task<Dictionary<int, int>> GetUserVoucherUsageCountsAsync(Guid userId, List<int> voucherIds) // Thực hiện xử lý bất đồng bộ phương thức 'GetUserVoucherUsageCountsAsync' nhận tham số (userId, voucherIds) trả về kiểu Task<Dictionary<int, int>>.
        {
            return await _dbContext.VoucherUsages // Chờ và trả về giá trị của tác vụ bất đồng bộ '_dbContext.VoucherUsages'.
                .Where(vu => vu.UserId == userId && voucherIds.Contains(vu.VoucherId)) // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(vu => vu.UserId == userId && voucherIds.Contains(vu.VoucherId)).
                .GroupBy(vu => vu.VoucherId) // Thực thi dòng lệnh nghiệp vụ.
                .Select(g => new { VoucherId = g.Key, Count = g.Count() }) // Thực thi dòng lệnh nghiệp vụ.
                .ToDictionaryAsync(x => x.VoucherId, x => x.Count); // Thực thi dòng lệnh nghiệp vụ.
        }

        public async Task<List<int>> GetUsedVoucherIdsByUserAsync(Guid userId, List<int> voucherIds) // Thực hiện xử lý bất đồng bộ phương thức 'GetUsedVoucherIdsByUserAsync' nhận tham số (userId, voucherIds) trả về kiểu Task<List<int>>.
        {
            return await _dbContext.VoucherUsages // Chờ và trả về giá trị của tác vụ bất đồng bộ '_dbContext.VoucherUsages'.
                .Where(vu => vu.UserId == userId && voucherIds.Contains(vu.VoucherId)) // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(vu => vu.UserId == userId && voucherIds.Contains(vu.VoucherId)).
                .Select(vu => vu.VoucherId) // Thực thi dòng lệnh nghiệp vụ.
                .Distinct() // Thực thi dòng lệnh nghiệp vụ.
                .ToListAsync(); // Truy vấn bất đồng bộ và chuyển đổi kết quả trả về dạng danh sách List.
        }

        public async Task<List<Voucher>> GetActiveVouchersForCustomerAsync() // Thực hiện xử lý bất đồng bộ phương thức 'GetActiveVouchersForCustomerAsync' không tham số trả về kiểu Task<List<Voucher>>.
        {
            var now = DateTime.UtcNow; // Thực hiện gán giá trị của biểu thức 'DateTime.UtcNow' cho biến 'now'.
            return await _dbContext.Vouchers // Chờ và trả về giá trị của tác vụ bất đồng bộ '_dbContext.Vouchers'.
                .AsNoTracking() // Tắt tính năng theo dõi trạng thái thực thể (AsNoTracking) để tối ưu hiệu năng đọc dữ liệu.
                .Include(v => v.VoucherCategories) // Nạp kèm (Eager load) dữ liệu quan hệ liên quan: .Include(v => v.VoucherCategories).
                .Where(v => v.IsActive // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(v => v.IsActive.
                    && v.StartDate <= now && v.EndDate >= now // Thực thi dòng lệnh nghiệp vụ.
                    && (v.Quantity == null || v.UsedCount < v.Quantity)) // Thực thi dòng lệnh nghiệp vụ.
                .ToListAsync(); // Truy vấn bất đồng bộ và chuyển đổi kết quả trả về dạng danh sách List.
        }

        public async Task AddUsagesAsync(IEnumerable<VoucherUsage> usages) // Thực hiện xử lý bất đồng bộ phương thức 'AddUsagesAsync' nhận tham số (usages) trả về kiểu Task.
        {
            await _dbContext.VoucherUsages.AddRangeAsync(usages); // Thực thi dòng lệnh nghiệp vụ.
        }

        public async Task SaveChangesAsync() // Thực hiện xử lý bất đồng bộ phương thức 'SaveChangesAsync' không tham số trả về kiểu Task.
        {
            await _dbContext.SaveChangesAsync(); // Thực thi dòng lệnh nghiệp vụ.
        }
    }
}

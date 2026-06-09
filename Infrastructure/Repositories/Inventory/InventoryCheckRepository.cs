using Microsoft.EntityFrameworkCore; // Nhập thư viện Microsoft.EntityFrameworkCore để làm việc với CSDL qua ORM.
using PBL3.Core.Entities; // Nhập namespace chứa các thực thể nghiệp vụ cốt lưu trữ của hệ thống.
using PBL3.Core.Interfaces; // Nhập namespace chứa các giao diện (interface) dùng chung của hệ thống.
using PBL3.Infrastructure.Data; // Nhập namespace chứa DbContext và các cấu hình dữ liệu của Infrastructure.

namespace PBL3.Infrastructure.Repositories // Định nghĩa namespace PBL3.Infrastructure.Repositories quản lý cấu trúc code.
{
    public class InventoryCheckRepository(HushStoreDbContext context) : IInventoryCheckRepository // Định nghĩa lớp InventoryCheckRepository sử dụng Primary Constructor nhận (HushStoreDbContext context) triển khai/kế thừa IInventoryCheckRepository.
    {
        private readonly HushStoreDbContext _context = // Khai báo trường dữ liệu private readonly và bắt đầu gán giá trị.
            context ?? throw new ArgumentNullException(nameof(context)); // Kiểm tra null và ném ra ngoại lệ ArgumentNullException nếu đối tượng context bị null.

        public async Task<InventoryCheck?> GetByIdAsync(int id) // Thực hiện xử lý bất đồng bộ phương thức 'GetByIdAsync' nhận tham số (id) trả về kiểu Task<InventoryCheck?>.
        {
            return await _context.InventoryChecks // Chờ và trả về giá trị của tác vụ bất đồng bộ '_context.InventoryChecks'.
                .Include(c => c.Details) // Nạp kèm (Eager load) dữ liệu quan hệ liên quan: .Include(c => c.Details).
                .FirstOrDefaultAsync(c => c.Id == id); // Lấy phần tử đầu tiên thỏa mãn điều kiện hoặc trả về null nếu không tìm thấy.
        }

        public async Task<InventoryCheck?> GetByIdWithDetailsAsync(int id) // Thực hiện xử lý bất đồng bộ phương thức 'GetByIdWithDetailsAsync' nhận tham số (id) trả về kiểu Task<InventoryCheck?>.
        {
            return await _context.InventoryChecks // Chờ và trả về giá trị của tác vụ bất đồng bộ '_context.InventoryChecks'.
                .AsNoTracking() // Tắt tính năng theo dõi trạng thái thực thể (AsNoTracking) để tối ưu hiệu năng đọc dữ liệu.
                .Include(c => c.ScopeCategory) // Nạp kèm (Eager load) dữ liệu quan hệ liên quan: .Include(c => c.ScopeCategory).
                .Include(c => c.Details) // Nạp kèm (Eager load) dữ liệu quan hệ liên quan: .Include(c => c.Details).
                    .ThenInclude(d => d.Variant) // Thực thi dòng lệnh nghiệp vụ.
                .FirstOrDefaultAsync(c => c.Id == id); // Lấy phần tử đầu tiên thỏa mãn điều kiện hoặc trả về null nếu không tìm thấy.
        }

        public async Task<(List<InventoryCheck> Items, int TotalCount)> GetPagedListAsync( // Thực hiện xử lý bất đồng bộ phương thức 'Task<' nhận tham số (Items, TotalCount) trả về kiểu .
            string? keyword, // Thực thi dòng lệnh nghiệp vụ.
            byte? status, // Thực thi dòng lệnh nghiệp vụ.
            DateTime? fromDate, // Thực thi dòng lệnh nghiệp vụ.
            DateTime? toDate, // Thực thi dòng lệnh nghiệp vụ.
            Guid? employeeId, // Thực thi dòng lệnh nghiệp vụ.
            int pageNumber, // Thực thi dòng lệnh nghiệp vụ.
            int pageSize, // Thực thi dòng lệnh nghiệp vụ.
            string? sortBy, // Thực thi dòng lệnh nghiệp vụ.
            bool sortDescending) // Thực thi dòng lệnh nghiệp vụ.
        {
            var query = _context.InventoryChecks // Thực hiện gán giá trị của biểu thức '_context.InventoryChecks' cho biến 'query'.
                .AsNoTracking() // Tắt tính năng theo dõi trạng thái thực thể (AsNoTracking) để tối ưu hiệu năng đọc dữ liệu.
                .Include(c => c.ScopeCategory) // Nạp kèm (Eager load) dữ liệu quan hệ liên quan: .Include(c => c.ScopeCategory).
                .Include(c => c.Details) // Nạp kèm (Eager load) dữ liệu quan hệ liên quan: .Include(c => c.Details).
                .AsQueryable(); // Chuyển cấu trúc dữ liệu thành IQueryable để hoãn việc thực thi truy vấn.

            if (!string.IsNullOrWhiteSpace(keyword)) // Kiểm tra điều kiện: '!string.IsNullOrWhiteSpace(keyword'.
            {
                var kw = keyword.Trim().ToLower(); // Thực hiện gán giá trị của biểu thức 'keyword.Trim().ToLower()' cho biến 'kw'.
                query = query.Where(c => c.CheckCode.ToLower().Contains(kw) || // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(c => c.CheckCode.ToLower().Contains(kw) ||.
                                        (c.Note != null && c.Note.ToLower().Contains(kw))); // Thực thi dòng lệnh nghiệp vụ.
            }

            if (status.HasValue) // Kiểm tra điều kiện: 'status.HasValue'.
                query = query.Where(c => c.Status == status.Value); // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(c => c.Status == status.Value);.

            if (fromDate.HasValue) // Kiểm tra điều kiện: 'fromDate.HasValue'.
                query = query.Where(c => c.CheckDate >= fromDate.Value.ToUniversalTime()); // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(c => c.CheckDate >= fromDate.Value.ToUniversalTime());.

            if (toDate.HasValue) // Kiểm tra điều kiện: 'toDate.HasValue'.
                query = query.Where(c => c.CheckDate < toDate.Value.AddDays(1).ToUniversalTime()); // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(c => c.CheckDate < toDate.Value.AddDays(1).ToUniversalTime());.

            if (employeeId.HasValue) // Kiểm tra điều kiện: 'employeeId.HasValue'.
                query = query.Where(c => c.EmployeeId == employeeId.Value); // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(c => c.EmployeeId == employeeId.Value);.

            var totalCount = await query.CountAsync(); // Tính toán tổng số lượng bản ghi thỏa mãn điều kiện bất đồng bộ.

            query = sortBy?.ToLower() switch // Thực hiện gán giá trị của biểu thức 'sortBy?.ToLower() switch' cho biến 'query'.
            {
                "code" => sortDescending ? query.OrderByDescending(c => c.CheckCode) : query.OrderBy(c => c.CheckCode), // Sắp xếp danh sách kết quả theo thứ tự giảm dần.
                "status" => sortDescending ? query.OrderByDescending(c => c.Status) : query.OrderBy(c => c.Status), // Sắp xếp danh sách kết quả theo thứ tự giảm dần.
                _ => sortDescending ? query.OrderByDescending(c => c.CheckDate) : query.OrderBy(c => c.CheckDate) // Sắp xếp danh sách kết quả theo thứ tự giảm dần.
            };

            var items = await query // Chờ tác vụ bất đồng bộ hoàn thành và gán kết quả cho biến 'items'.
                .Skip((pageNumber - 1) * pageSize) // Thực thi dòng lệnh nghiệp vụ.
                .Take(pageSize) // Thực thi dòng lệnh nghiệp vụ.
                .ToListAsync(); // Truy vấn bất đồng bộ và chuyển đổi kết quả trả về dạng danh sách List.

            return (items, totalCount); // Trả về giá trị của biểu thức '(items, totalCount)'.
        }

        public async Task<string?> GetLastCheckCodeByDateAsync(string datePrefix) // Thực hiện xử lý bất đồng bộ phương thức 'GetLastCheckCodeByDateAsync' nhận tham số (datePrefix) trả về kiểu Task<string?>.
        {
            return await _context.InventoryChecks // Chờ và trả về giá trị của tác vụ bất đồng bộ '_context.InventoryChecks'.
                .AsNoTracking() // Tắt tính năng theo dõi trạng thái thực thể (AsNoTracking) để tối ưu hiệu năng đọc dữ liệu.
                .IgnoreQueryFilters() // Thực thi dòng lệnh nghiệp vụ.
                .Where(c => c.CheckCode.StartsWith(datePrefix)) // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(c => c.CheckCode.StartsWith(datePrefix)).
                .OrderByDescending(c => c.CheckCode) // Sắp xếp danh sách kết quả theo thứ tự giảm dần.
                .Select(c => c.CheckCode) // Thực thi dòng lệnh nghiệp vụ.
                .FirstOrDefaultAsync(); // Lấy phần tử đầu tiên thỏa mãn điều kiện hoặc trả về null nếu không tìm thấy.
        }

        public async Task<InventoryCheckDetailSerial?> GetDetailSerialAsync(int detailSerialId, bool withTracking = false) // Thực hiện xử lý bất đồng bộ phương thức 'GetDetailSerialAsync' nhận tham số (detailSerialId, false) trả về kiểu Task<InventoryCheckDetailSerial?>.
        {
            var query = _context.InventoryCheckDetailSerials.AsQueryable(); // Chuyển cấu trúc dữ liệu thành IQueryable để hoãn việc thực thi truy vấn.
            if (!withTracking) // Kiểm tra điều kiện: '!withTracking'.
                query = query.AsNoTracking(); // Tắt tính năng theo dõi trạng thái thực thể (AsNoTracking) để tối ưu hiệu năng đọc dữ liệu.

            return await query // Chờ và trả về giá trị của tác vụ bất đồng bộ 'query'.
                .Include(s => s.Variant) // Nạp kèm (Eager load) dữ liệu quan hệ liên quan: .Include(s => s.Variant).
                .FirstOrDefaultAsync(s => s.Id == detailSerialId); // Lấy phần tử đầu tiên thỏa mãn điều kiện hoặc trả về null nếu không tìm thấy.
        }

        public async Task<InventoryCheckDetailSerial?> GetPendingDetailSerialBySerialIdAsync(int checkId, int serialId) // Thực hiện xử lý bất đồng bộ phương thức 'GetPendingDetailSerialBySerialIdAsync' nhận tham số (checkId, serialId) trả về kiểu Task<InventoryCheckDetailSerial?>.
        {
            return await _context.InventoryCheckDetailSerials // Chờ và trả về giá trị của tác vụ bất đồng bộ '_context.InventoryCheckDetailSerials'.
                .FirstOrDefaultAsync(s => s.CheckId == checkId && // Lấy phần tử đầu tiên thỏa mãn điều kiện hoặc trả về null nếu không tìm thấy.
                                          s.SerialId == serialId && // Thực thi dòng lệnh nghiệp vụ.
                                          s.ScanStatus == 0); // Thực thi dòng lệnh nghiệp vụ.
        }

        public async Task<bool> IsSerialAlreadyScannedAsync(int checkId, string serialNumberRaw) // Thực hiện xử lý bất đồng bộ phương thức 'IsSerialAlreadyScannedAsync' nhận tham số (checkId, serialNumberRaw) trả về kiểu Task<bool>.
        {
            return await _context.InventoryCheckDetailSerials // Chờ và trả về giá trị của tác vụ bất đồng bộ '_context.InventoryCheckDetailSerials'.
                .AnyAsync(s => s.CheckId == checkId && // Kiểm tra xem có bất kỳ bản ghi nào khớp với điều kiện hay không.
                               s.SerialNumberRaw == serialNumberRaw && // Thực thi dòng lệnh nghiệp vụ.
                               s.ScanStatus != 0); // Thực thi dòng lệnh nghiệp vụ.
        }

        public async Task<List<InventoryCheckDetailSerial>> GetPendingDetailSerialsAsync(int checkId) // Thực hiện xử lý bất đồng bộ phương thức 'GetPendingDetailSerialsAsync' nhận tham số (checkId) trả về kiểu Task<List<InventoryCheckDetailSerial>>.
        {
            return await _context.InventoryCheckDetailSerials // Chờ và trả về giá trị của tác vụ bất đồng bộ '_context.InventoryCheckDetailSerials'.
                .Where(s => s.CheckId == checkId && s.ScanStatus == 0) // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(s => s.CheckId == checkId && s.ScanStatus == 0).
                .Include(s => s.Detail) // Nạp kèm (Eager load) dữ liệu quan hệ liên quan: .Include(s => s.Detail).
                .ToListAsync(); // Truy vấn bất đồng bộ và chuyển đổi kết quả trả về dạng danh sách List.
        }

        public async Task<List<InventoryCheckDetailSerial>> GetSurplusDetailSerialsAsync(int checkId) // Thực hiện xử lý bất đồng bộ phương thức 'GetSurplusDetailSerialsAsync' nhận tham số (checkId) trả về kiểu Task<List<InventoryCheckDetailSerial>>.
        {
            return await _context.InventoryCheckDetailSerials // Chờ và trả về giá trị của tác vụ bất đồng bộ '_context.InventoryCheckDetailSerials'.
                .Where(s => s.CheckId == checkId && // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(s => s.CheckId == checkId &&.
                            (s.ScanStatus == 3 || s.ScanStatus == 4)) // Thực thi dòng lệnh nghiệp vụ.
                .ToListAsync(); // Truy vấn bất đồng bộ và chuyển đổi kết quả trả về dạng danh sách List.
        }

        public async Task<(List<InventoryCheckDetailSerial> Items, int TotalCount)> GetDetailSerialsPagedAsync( // Thực hiện xử lý bất đồng bộ phương thức 'Task<' nhận tham số (Items, TotalCount) trả về kiểu .
            int checkId, // Thực thi dòng lệnh nghiệp vụ.
            byte? scanStatus, // Thực thi dòng lệnh nghiệp vụ.
            int? variantId, // Thực thi dòng lệnh nghiệp vụ.
            int pageNumber, // Thực thi dòng lệnh nghiệp vụ.
            int pageSize) // Thực thi dòng lệnh nghiệp vụ.
        {
            var query = _context.InventoryCheckDetailSerials // Thực hiện gán giá trị của biểu thức '_context.InventoryCheckDetailSerials' cho biến 'query'.
                .AsNoTracking() // Tắt tính năng theo dõi trạng thái thực thể (AsNoTracking) để tối ưu hiệu năng đọc dữ liệu.
                .Where(s => s.CheckId == checkId) // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(s => s.CheckId == checkId).
                .Include(s => s.Variant) // Nạp kèm (Eager load) dữ liệu quan hệ liên quan: .Include(s => s.Variant).
                .AsQueryable(); // Chuyển cấu trúc dữ liệu thành IQueryable để hoãn việc thực thi truy vấn.

            if (scanStatus.HasValue) // Kiểm tra điều kiện: 'scanStatus.HasValue'.
                query = query.Where(s => s.ScanStatus == scanStatus.Value); // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(s => s.ScanStatus == scanStatus.Value);.

            if (variantId.HasValue) // Kiểm tra điều kiện: 'variantId.HasValue'.
                query = query.Where(s => s.VariantId == variantId.Value); // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(s => s.VariantId == variantId.Value);.

            var totalCount = await query.CountAsync(); // Tính toán tổng số lượng bản ghi thỏa mãn điều kiện bất đồng bộ.

            var items = await query // Chờ tác vụ bất đồng bộ hoàn thành và gán kết quả cho biến 'items'.
                .OrderBy(s => s.ScanStatus) // Sắp xếp danh sách kết quả theo thứ tự tăng dần.
                .ThenBy(s => s.SerialNumberRaw) // Thực thi dòng lệnh nghiệp vụ.
                .Skip((pageNumber - 1) * pageSize) // Thực thi dòng lệnh nghiệp vụ.
                .Take(pageSize) // Thực thi dòng lệnh nghiệp vụ.
                .ToListAsync(); // Truy vấn bất đồng bộ và chuyển đổi kết quả trả về dạng danh sách List.

            return (items, totalCount); // Trả về giá trị của biểu thức '(items, totalCount)'.
        }

        public async Task<Dictionary<byte, int>> GetGroupedCountsByCheckAsync(int checkId) // Thực hiện xử lý bất đồng bộ phương thức 'GetGroupedCountsByCheckAsync' nhận tham số (checkId) trả về kiểu Task<Dictionary<byte, int>>.
        {
            return await _context.InventoryCheckDetailSerials // Chờ và trả về giá trị của tác vụ bất đồng bộ '_context.InventoryCheckDetailSerials'.
                .AsNoTracking() // Tắt tính năng theo dõi trạng thái thực thể (AsNoTracking) để tối ưu hiệu năng đọc dữ liệu.
                .Where(s => s.CheckId == checkId) // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(s => s.CheckId == checkId).
                .GroupBy(s => s.ScanStatus) // Thực thi dòng lệnh nghiệp vụ.
                .Select(g => new { Status = g.Key, Count = g.Count() }) // Thực thi dòng lệnh nghiệp vụ.
                .ToDictionaryAsync(x => x.Status, x => x.Count); // Thực thi dòng lệnh nghiệp vụ.
        }

        public async Task<InventoryCheckDetail?> GetDetailByCheckAndVariantAsync(int checkId, int variantId, bool withTracking = false) // Thực hiện xử lý bất đồng bộ phương thức 'GetDetailByCheckAndVariantAsync' nhận tham số (checkId, variantId, false) trả về kiểu Task<InventoryCheckDetail?>.
        {
            var query = _context.InventoryCheckDetails.AsQueryable(); // Chuyển cấu trúc dữ liệu thành IQueryable để hoãn việc thực thi truy vấn.
            if (!withTracking) // Kiểm tra điều kiện: '!withTracking'.
                query = query.AsNoTracking(); // Tắt tính năng theo dõi trạng thái thực thể (AsNoTracking) để tối ưu hiệu năng đọc dữ liệu.

            return await query.FirstOrDefaultAsync(d => d.CheckId == checkId && d.VariantId == variantId); // Lấy phần tử đầu tiên thỏa mãn điều kiện hoặc trả về null nếu không tìm thấy.
        }

        public async Task<List<InventoryCheckDetailSerial>> GetMissingDetailSerialsWithSerialAsync(int checkId) // Thực hiện xử lý bất đồng bộ phương thức 'GetMissingDetailSerialsWithSerialAsync' nhận tham số (checkId) trả về kiểu Task<List<InventoryCheckDetailSerial>>.
        {
            return await _context.InventoryCheckDetailSerials // Chờ và trả về giá trị của tác vụ bất đồng bộ '_context.InventoryCheckDetailSerials'.
                .Where(s => s.CheckId == checkId && s.ScanStatus == 2) // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(s => s.CheckId == checkId && s.ScanStatus == 2).
                .Include(s => s.Serial) // Nạp kèm (Eager load) dữ liệu quan hệ liên quan: .Include(s => s.Serial).
                    .ThenInclude(ps => ps!.ImportReceipt) // Thực thi dòng lệnh nghiệp vụ.
                        .ThenInclude(r => r.Details) // Thực thi dòng lệnh nghiệp vụ.
                .ToListAsync(); // Truy vấn bất đồng bộ và chuyển đổi kết quả trả về dạng danh sách List.
        }

        public async Task<List<InventoryCheckDetailSerial>> GetDefectiveDetailSerialsWithSerialAsync(int checkId) // Thực hiện xử lý bất đồng bộ phương thức 'GetDefectiveDetailSerialsWithSerialAsync' nhận tham số (checkId) trả về kiểu Task<List<InventoryCheckDetailSerial>>.
        {
            return await _context.InventoryCheckDetailSerials // Chờ và trả về giá trị của tác vụ bất đồng bộ '_context.InventoryCheckDetailSerials'.
                .Where(s => s.CheckId == checkId && s.ScanStatus == 5) // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(s => s.CheckId == checkId && s.ScanStatus == 5).
                .Include(s => s.Serial) // Nạp kèm (Eager load) dữ liệu quan hệ liên quan: .Include(s => s.Serial).
                    .ThenInclude(ps => ps!.ImportReceipt) // Thực thi dòng lệnh nghiệp vụ.
                        .ThenInclude(r => r.Details) // Thực thi dòng lệnh nghiệp vụ.
                .ToListAsync(); // Truy vấn bất đồng bộ và chuyển đổi kết quả trả về dạng danh sách List.
        }

        public async Task AddAsync(InventoryCheck check) // Thực hiện xử lý bất đồng bộ phương thức 'AddAsync' nhận tham số (check) trả về kiểu Task.
        {
            _context.InventoryChecks.Add(check); // Thêm thực thể mới vào CSDL thông qua DbSet tương ứng.
            await Task.CompletedTask; // Trả về trạng thái tác vụ đã hoàn thành.
        }

        public async Task AddDetailAsync(InventoryCheckDetail detail) // Thực hiện xử lý bất đồng bộ phương thức 'AddDetailAsync' nhận tham số (detail) trả về kiểu Task.
        {
            _context.InventoryCheckDetails.Add(detail); // Thêm thực thể mới vào CSDL thông qua DbSet tương ứng.
            await Task.CompletedTask; // Trả về trạng thái tác vụ đã hoàn thành.
        }

        public async Task AddDetailSerialAsync(InventoryCheckDetailSerial detailSerial) // Thực hiện xử lý bất đồng bộ phương thức 'AddDetailSerialAsync' nhận tham số (detailSerial) trả về kiểu Task.
        {
            _context.InventoryCheckDetailSerials.Add(detailSerial); // Thêm thực thể mới vào CSDL thông qua DbSet tương ứng.
            await Task.CompletedTask; // Trả về trạng thái tác vụ đã hoàn thành.
        }

        public async Task AddDetailSerialsAsync(IEnumerable<InventoryCheckDetailSerial> detailSerials) // Thực hiện xử lý bất đồng bộ phương thức 'AddDetailSerialsAsync' nhận tham số (detailSerials) trả về kiểu Task.
        {
            await _context.InventoryCheckDetailSerials.AddRangeAsync(detailSerials); // Thực thi dòng lệnh nghiệp vụ.
        }

        public async Task AddAdjustmentLogsAsync(IEnumerable<InventoryAdjustmentLog> logs) // Thực hiện xử lý bất đồng bộ phương thức 'AddAdjustmentLogsAsync' nhận tham số (logs) trả về kiểu Task.
        {
            await _context.InventoryAdjustmentLogs.AddRangeAsync(logs); // Thực thi dòng lệnh nghiệp vụ.
        }

        public async Task RemoveDetailSerialsAsync(IEnumerable<InventoryCheckDetailSerial> serials) // Thực hiện xử lý bất đồng bộ phương thức 'RemoveDetailSerialsAsync' nhận tham số (serials) trả về kiểu Task.
        {
            _context.InventoryCheckDetailSerials.RemoveRange(serials); // Thực thi dòng lệnh nghiệp vụ.
            await Task.CompletedTask; // Trả về trạng thái tác vụ đã hoàn thành.
        }

        public async Task SaveChangesAsync() // Thực hiện xử lý bất đồng bộ phương thức 'SaveChangesAsync' không tham số trả về kiểu Task.
        {
            await _context.SaveChangesAsync(); // Thực thi dòng lệnh nghiệp vụ.
        }
    }
}

using Microsoft.EntityFrameworkCore; // Nhập thư viện Microsoft.EntityFrameworkCore để làm việc với CSDL qua ORM.
using PBL3.Core.Entities; // Nhập namespace chứa các thực thể nghiệp vụ cốt lưu trữ của hệ thống.
using PBL3.Core.Interfaces; // Nhập namespace chứa các giao diện (interface) dùng chung của hệ thống.
using PBL3.Infrastructure.Data; // Nhập namespace chứa DbContext và các cấu hình dữ liệu của Infrastructure.

namespace PBL3.Infrastructure.Repositories // Định nghĩa namespace PBL3.Infrastructure.Repositories quản lý cấu trúc code.
{
    public class ImportReceiptRepository(HushStoreDbContext context) : IImportReceiptRepository // Định nghĩa lớp ImportReceiptRepository sử dụng Primary Constructor nhận (HushStoreDbContext context) triển khai/kế thừa IImportReceiptRepository.
    {
        private readonly HushStoreDbContext _context = // Khai báo trường dữ liệu private readonly và bắt đầu gán giá trị.
            context ?? throw new ArgumentNullException(nameof(context)); // Kiểm tra null và ném ra ngoại lệ ArgumentNullException nếu đối tượng context bị null.

        public async Task<(List<ImportReceipt> Items, int TotalCount)> GetPagedListAsync( // Thực hiện xử lý bất đồng bộ phương thức 'Task<' nhận tham số (Items, TotalCount) trả về kiểu .
            string? keyword, // Thực thi dòng lệnh nghiệp vụ.
            DateTime? fromDate, // Thực thi dòng lệnh nghiệp vụ.
            DateTime? toDate, // Thực thi dòng lệnh nghiệp vụ.
            int? supplierId, // Thực thi dòng lệnh nghiệp vụ.
            int pageNumber, // Thực thi dòng lệnh nghiệp vụ.
            int pageSize, // Thực thi dòng lệnh nghiệp vụ.
            string? sortBy, // Thực thi dòng lệnh nghiệp vụ.
            bool sortDescending) // Thực thi dòng lệnh nghiệp vụ.
        {
            var query = _context.ImportReceipts // Thực hiện gán giá trị của biểu thức '_context.ImportReceipts' cho biến 'query'.
                .AsNoTracking() // Tắt tính năng theo dõi trạng thái thực thể (AsNoTracking) để tối ưu hiệu năng đọc dữ liệu.
                .Include(r => r.Supplier) // Nạp kèm (Eager load) dữ liệu quan hệ liên quan: .Include(r => r.Supplier).
                .AsQueryable(); // Chuyển cấu trúc dữ liệu thành IQueryable để hoãn việc thực thi truy vấn.

            if (!string.IsNullOrWhiteSpace(keyword)) // Kiểm tra điều kiện: '!string.IsNullOrWhiteSpace(keyword'.
            {
                var kw = keyword.Trim().ToLower(); // Thực hiện gán giá trị của biểu thức 'keyword.Trim().ToLower()' cho biến 'kw'.
                query = query.Where(r => // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(r =>.
                    r.ReceiptCode.ToLower().Contains(kw) || // Thực thi dòng lệnh nghiệp vụ.
                    r.Supplier.Name.ToLower().Contains(kw)); // Thực thi dòng lệnh nghiệp vụ.
            }

            if (fromDate.HasValue) // Kiểm tra điều kiện: 'fromDate.HasValue'.
                query = query.Where(r => r.ImportDate >= fromDate.Value.ToUniversalTime()); // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(r => r.ImportDate >= fromDate.Value.ToUniversalTime());.

            if (toDate.HasValue) // Kiểm tra điều kiện: 'toDate.HasValue'.
                query = query.Where(r => r.ImportDate < toDate.Value.AddDays(1).ToUniversalTime()); // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(r => r.ImportDate < toDate.Value.AddDays(1).ToUniversalTime());.

            if (supplierId.HasValue) // Kiểm tra điều kiện: 'supplierId.HasValue'.
                query = query.Where(r => r.SupplierId == supplierId.Value); // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(r => r.SupplierId == supplierId.Value);.

            var totalCount = await query.CountAsync(); // Tính toán tổng số lượng bản ghi thỏa mãn điều kiện bất đồng bộ.

            // Sắp xếp
            query = sortBy?.ToLower() switch // Thực hiện gán giá trị của biểu thức 'sortBy?.ToLower() switch' cho biến 'query'.
            {
                "code" => sortDescending ? query.OrderByDescending(r => r.ReceiptCode) : query.OrderBy(r => r.ReceiptCode), // Sắp xếp danh sách kết quả theo thứ tự giảm dần.
                "supplier" => sortDescending ? query.OrderByDescending(r => r.Supplier.Name) : query.OrderBy(r => r.Supplier.Name), // Sắp xếp danh sách kết quả theo thứ tự giảm dần.
                "total" => sortDescending ? query.OrderByDescending(r => r.TotalAmount) : query.OrderBy(r => r.TotalAmount), // Sắp xếp danh sách kết quả theo thứ tự giảm dần.
                _ => sortDescending ? query.OrderByDescending(r => r.ImportDate) : query.OrderBy(r => r.ImportDate) // Sắp xếp danh sách kết quả theo thứ tự giảm dần.
            };

            var items = await query // Chờ tác vụ bất đồng bộ hoàn thành và gán kết quả cho biến 'items'.
                .Skip((pageNumber - 1) * pageSize) // Thực thi dòng lệnh nghiệp vụ.
                .Take(pageSize) // Thực thi dòng lệnh nghiệp vụ.
                .ToListAsync(); // Truy vấn bất đồng bộ và chuyển đổi kết quả trả về dạng danh sách List.

            return (items, totalCount); // Trả về giá trị của biểu thức '(items, totalCount)'.
        }

        public async Task<ImportReceipt?> GetByIdWithDetailsAsync(int id) // Thực hiện xử lý bất đồng bộ phương thức 'GetByIdWithDetailsAsync' nhận tham số (id) trả về kiểu Task<ImportReceipt?>.
        {
            return await _context.ImportReceipts // Chờ và trả về giá trị của tác vụ bất đồng bộ '_context.ImportReceipts'.
                .AsNoTracking() // Tắt tính năng theo dõi trạng thái thực thể (AsNoTracking) để tối ưu hiệu năng đọc dữ liệu.
                .Include(r => r.Supplier) // Nạp kèm (Eager load) dữ liệu quan hệ liên quan: .Include(r => r.Supplier).
                .Include(r => r.Details) // Nạp kèm (Eager load) dữ liệu quan hệ liên quan: .Include(r => r.Details).
                    .ThenInclude(d => d.Variant) // Thực thi dòng lệnh nghiệp vụ.
                .FirstOrDefaultAsync(r => r.Id == id); // Lấy phần tử đầu tiên thỏa mãn điều kiện hoặc trả về null nếu không tìm thấy.
        }

        public async Task<string?> GetLastReceiptCodeByDateAsync(string datePrefix) // Thực hiện xử lý bất đồng bộ phương thức 'GetLastReceiptCodeByDateAsync' nhận tham số (datePrefix) trả về kiểu Task<string?>.
        {
            return await _context.ImportReceipts // Chờ và trả về giá trị của tác vụ bất đồng bộ '_context.ImportReceipts'.
                .AsNoTracking() // Tắt tính năng theo dõi trạng thái thực thể (AsNoTracking) để tối ưu hiệu năng đọc dữ liệu.
                .Where(r => r.ReceiptCode.StartsWith(datePrefix)) // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(r => r.ReceiptCode.StartsWith(datePrefix)).
                .OrderByDescending(r => r.ReceiptCode) // Sắp xếp danh sách kết quả theo thứ tự giảm dần.
                .Select(r => r.ReceiptCode) // Thực thi dòng lệnh nghiệp vụ.
                .FirstOrDefaultAsync(); // Lấy phần tử đầu tiên thỏa mãn điều kiện hoặc trả về null nếu không tìm thấy.
        }

        public async Task AddAsync(ImportReceipt receipt) // Thực hiện xử lý bất đồng bộ phương thức 'AddAsync' nhận tham số (receipt) trả về kiểu Task.
        {
            _context.ImportReceipts.Add(receipt); // Thêm thực thể mới vào CSDL thông qua DbSet tương ứng.
            await Task.CompletedTask; // Trả về trạng thái tác vụ đã hoàn thành.
        }

        public async Task AddDetailAsync(ImportReceiptDetail detail) // Thực hiện xử lý bất đồng bộ phương thức 'AddDetailAsync' nhận tham số (detail) trả về kiểu Task.
        {
            _context.ImportReceiptDetails.Add(detail); // Thêm thực thể mới vào CSDL thông qua DbSet tương ứng.
            await Task.CompletedTask; // Trả về trạng thái tác vụ đã hoàn thành.
        }

        public async Task SaveChangesAsync() // Thực hiện xử lý bất đồng bộ phương thức 'SaveChangesAsync' không tham số trả về kiểu Task.
        {
            await _context.SaveChangesAsync(); // Thực thi dòng lệnh nghiệp vụ.
        }
    }
}

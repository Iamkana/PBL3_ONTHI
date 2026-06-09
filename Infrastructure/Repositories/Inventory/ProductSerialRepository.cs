using Microsoft.EntityFrameworkCore; // Nhập thư viện Microsoft.EntityFrameworkCore để làm việc với CSDL qua ORM.
using PBL3.Core.Entities; // Nhập namespace chứa các thực thể nghiệp vụ cốt lưu trữ của hệ thống.
using PBL3.Core.Interfaces; // Nhập namespace chứa các giao diện (interface) dùng chung của hệ thống.
using PBL3.Infrastructure.Data; // Nhập namespace chứa DbContext và các cấu hình dữ liệu của Infrastructure.

namespace PBL3.Infrastructure.Repositories // Định nghĩa namespace PBL3.Infrastructure.Repositories quản lý cấu trúc code.
{
    public class ProductSerialRepository(HushStoreDbContext context) : IProductSerialRepository // Định nghĩa lớp ProductSerialRepository sử dụng Primary Constructor nhận (HushStoreDbContext context) triển khai/kế thừa IProductSerialRepository.
    {
        private readonly HushStoreDbContext _context = // Khai báo trường dữ liệu private readonly và bắt đầu gán giá trị.
            context ?? throw new ArgumentNullException(nameof(context)); // Kiểm tra null và ném ra ngoại lệ ArgumentNullException nếu đối tượng context bị null.

        public async Task<bool> ExistsAsync(string serialNumber, int variantId) // Thực hiện xử lý bất đồng bộ phương thức 'ExistsAsync' nhận tham số (serialNumber, variantId) trả về kiểu Task<bool>.
        {
            return await _context.ProductSerials // Chờ và trả về giá trị của tác vụ bất đồng bộ '_context.ProductSerials'.
                .AsNoTracking() // Tắt tính năng theo dõi trạng thái thực thể (AsNoTracking) để tối ưu hiệu năng đọc dữ liệu.
                .AnyAsync(x => x.SerialNumber == serialNumber && x.VariantId == variantId); // Kiểm tra xem có bất kỳ bản ghi nào khớp với điều kiện hay không.
        }

        public async Task<List<string>> GetExistingSerialsAsync(List<string> serialNumbers) // Thực hiện xử lý bất đồng bộ phương thức 'GetExistingSerialsAsync' nhận tham số (serialNumbers) trả về kiểu Task<List<string>>.
        {
            // Tìm các Serial đã tồn tại trong DB (so sánh case-insensitive)
            return await _context.ProductSerials // Chờ và trả về giá trị của tác vụ bất đồng bộ '_context.ProductSerials'.
                .AsNoTracking() // Tắt tính năng theo dõi trạng thái thực thể (AsNoTracking) để tối ưu hiệu năng đọc dữ liệu.
                .Where(s => serialNumbers.Contains(s.SerialNumber)) // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(s => serialNumbers.Contains(s.SerialNumber)).
                .Select(s => s.SerialNumber) // Thực thi dòng lệnh nghiệp vụ.
                .ToListAsync(); // Truy vấn bất đồng bộ và chuyển đổi kết quả trả về dạng danh sách List.
        }

        public async Task AddRangeAsync(IEnumerable<ProductSerial> serials) // Thực hiện xử lý bất đồng bộ phương thức 'AddRangeAsync' nhận tham số (serials) trả về kiểu Task.
        {
            _context.ProductSerials.AddRange(serials); // Thực thi dòng lệnh nghiệp vụ.
            await Task.CompletedTask; // Trả về trạng thái tác vụ đã hoàn thành.
        }

        public async Task<List<string>> GetSerialsByReceiptAndVariantAsync(int receiptId, int variantId) // Thực hiện xử lý bất đồng bộ phương thức 'GetSerialsByReceiptAndVariantAsync' nhận tham số (receiptId, variantId) trả về kiểu Task<List<string>>.
        {
            return await _context.ProductSerials // Chờ và trả về giá trị của tác vụ bất đồng bộ '_context.ProductSerials'.
                .AsNoTracking() // Tắt tính năng theo dõi trạng thái thực thể (AsNoTracking) để tối ưu hiệu năng đọc dữ liệu.
                .Where(s => s.ImportReceiptId == receiptId && s.VariantId == variantId) // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(s => s.ImportReceiptId == receiptId && s.VariantId == variantId).
                .Select(s => s.SerialNumber) // Thực thi dòng lệnh nghiệp vụ.
                .ToListAsync(); // Truy vấn bất đồng bộ và chuyển đổi kết quả trả về dạng danh sách List.
        }

        public async Task<ProductSerial?> GetBySerialNumberAsync(string serialNumber) // Thực hiện xử lý bất đồng bộ phương thức 'GetBySerialNumberAsync' nhận tham số (serialNumber) trả về kiểu Task<ProductSerial?>.
        {
            return await _context.ProductSerials // Chờ và trả về giá trị của tác vụ bất đồng bộ '_context.ProductSerials'.
                .Include(s => s.Variant) // Nạp kèm (Eager load) dữ liệu quan hệ liên quan: .Include(s => s.Variant).
                    .ThenInclude(v => v.Product) // Thực thi dòng lệnh nghiệp vụ.
                .AsNoTracking() // Tắt tính năng theo dõi trạng thái thực thể (AsNoTracking) để tối ưu hiệu năng đọc dữ liệu.
                .FirstOrDefaultAsync(s => s.SerialNumber == serialNumber); // Lấy phần tử đầu tiên thỏa mãn điều kiện hoặc trả về null nếu không tìm thấy.
        }

        public async Task<List<ProductSerial>> GetAvailableSerialsByVariantAsync(int variantId, int count) // Thực hiện xử lý bất đồng bộ phương thức 'GetAvailableSerialsByVariantAsync' nhận tham số (variantId, count) trả về kiểu Task<List<ProductSerial>>.
        {
            return await _context.ProductSerials // Chờ và trả về giá trị của tác vụ bất đồng bộ '_context.ProductSerials'.
                .Where(s => s.VariantId == variantId && s.Status == 0) // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(s => s.VariantId == variantId && s.Status == 0).
                .Take(count) // Thực thi dòng lệnh nghiệp vụ.
                .ToListAsync(); // Truy vấn bất đồng bộ và chuyển đổi kết quả trả về dạng danh sách List.
        }

        public async Task<List<ProductSerial>> GetSerialsWithTrackingAsync(List<string> serialNumbers) // Thực hiện xử lý bất đồng bộ phương thức 'GetSerialsWithTrackingAsync' nhận tham số (serialNumbers) trả về kiểu Task<List<ProductSerial>>.
        {
            return await _context.ProductSerials // Chờ và trả về giá trị của tác vụ bất đồng bộ '_context.ProductSerials'.
                .Include(s => s.Variant) // Nạp kèm (Eager load) dữ liệu quan hệ liên quan: .Include(s => s.Variant).
                .Where(s => serialNumbers.Contains(s.SerialNumber)) // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(s => serialNumbers.Contains(s.SerialNumber)).
                .ToListAsync(); // Truy vấn bất đồng bộ và chuyển đổi kết quả trả về dạng danh sách List.
        }

        public async Task<ProductSerial?> GetByIdWithTrackingAsync(int id) // Thực hiện xử lý bất đồng bộ phương thức 'GetByIdWithTrackingAsync' nhận tham số (id) trả về kiểu Task<ProductSerial?>.
        {
            return await _context.ProductSerials // Chờ và trả về giá trị của tác vụ bất đồng bộ '_context.ProductSerials'.
                .Include(s => s.Variant) // Nạp kèm (Eager load) dữ liệu quan hệ liên quan: .Include(s => s.Variant).
                .FirstOrDefaultAsync(s => s.Id == id); // Lấy phần tử đầu tiên thỏa mãn điều kiện hoặc trả về null nếu không tìm thấy.
        }

        public async Task<Dictionary<int, int>> CountAvailableByVariantIdsAsync(List<int> variantIds) // Thực hiện xử lý bất đồng bộ phương thức 'CountAvailableByVariantIdsAsync' nhận tham số (variantIds) trả về kiểu Task<Dictionary<int, int>>.
        {
            return await _context.ProductSerials // Chờ và trả về giá trị của tác vụ bất đồng bộ '_context.ProductSerials'.
                .AsNoTracking() // Tắt tính năng theo dõi trạng thái thực thể (AsNoTracking) để tối ưu hiệu năng đọc dữ liệu.
                .Where(s => s.Status == 0 && variantIds.Contains(s.VariantId)) // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(s => s.Status == 0 && variantIds.Contains(s.VariantId)).
                .GroupBy(s => s.VariantId) // Thực thi dòng lệnh nghiệp vụ.
                .ToDictionaryAsync(g => g.Key, g => g.Count()); // Thực thi dòng lệnh nghiệp vụ.
        }

        public async Task SaveChangesAsync() // Thực hiện xử lý bất đồng bộ phương thức 'SaveChangesAsync' không tham số trả về kiểu Task.
        {
            await _context.SaveChangesAsync(); // Thực thi dòng lệnh nghiệp vụ.
        }

        public async Task<(List<ProductSerial> Items, int TotalCount)> GetPagedListAsync( // Thực hiện xử lý bất đồng bộ phương thức 'Task<' nhận tham số (Items, TotalCount) trả về kiểu .
            string? keyword, int? productId, int? variantId, // Thực thi dòng lệnh nghiệp vụ.
            byte? status, DateTime? fromDate, DateTime? toDate, // Thực thi dòng lệnh nghiệp vụ.
            int pageNumber, int pageSize, string? sortBy, bool sortDescending) // Thực thi dòng lệnh nghiệp vụ.
        {
            var query = _context.ProductSerials // Thực hiện gán giá trị của biểu thức '_context.ProductSerials' cho biến 'query'.
                .AsNoTracking() // Tắt tính năng theo dõi trạng thái thực thể (AsNoTracking) để tối ưu hiệu năng đọc dữ liệu.
                .Include(s => s.Variant).ThenInclude(v => v.Product) // Nạp kèm (Eager load) dữ liệu quan hệ liên quan: .Include(s => s.Variant).ThenInclude(v => v.Product).
                .Include(s => s.ImportReceipt) // Nạp kèm (Eager load) dữ liệu quan hệ liên quan: .Include(s => s.ImportReceipt).
                .AsQueryable(); // Chuyển cấu trúc dữ liệu thành IQueryable để hoãn việc thực thi truy vấn.

            if (!string.IsNullOrWhiteSpace(keyword)) // Kiểm tra điều kiện: '!string.IsNullOrWhiteSpace(keyword'.
                query = query.Where(s => s.SerialNumber.Contains(keyword.Trim())); // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(s => s.SerialNumber.Contains(keyword.Trim()));.
            if (productId.HasValue) // Kiểm tra điều kiện: 'productId.HasValue'.
                query = query.Where(s => s.Variant.ProductId == productId.Value); // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(s => s.Variant.ProductId == productId.Value);.
            if (variantId.HasValue) // Kiểm tra điều kiện: 'variantId.HasValue'.
                query = query.Where(s => s.VariantId == variantId.Value); // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(s => s.VariantId == variantId.Value);.
            if (status.HasValue) // Kiểm tra điều kiện: 'status.HasValue'.
                query = query.Where(s => s.Status == status.Value); // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(s => s.Status == status.Value);.
            if (fromDate.HasValue) // Kiểm tra điều kiện: 'fromDate.HasValue'.
                query = query.Where(s => s.CreatedDate >= fromDate.Value); // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(s => s.CreatedDate >= fromDate.Value);.
            if (toDate.HasValue) // Kiểm tra điều kiện: 'toDate.HasValue'.
                query = query.Where(s => s.CreatedDate <= toDate.Value); // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(s => s.CreatedDate <= toDate.Value);.

            var totalCount = await query.CountAsync(); // Tính toán tổng số lượng bản ghi thỏa mãn điều kiện bất đồng bộ.

            query = sortBy?.ToLower() switch // Thực hiện gán giá trị của biểu thức 'sortBy?.ToLower() switch' cho biến 'query'.
            {
                "serialnumber" => sortDescending ? query.OrderByDescending(s => s.SerialNumber) : query.OrderBy(s => s.SerialNumber), // Sắp xếp danh sách kết quả theo thứ tự giảm dần.
                "status"       => sortDescending ? query.OrderByDescending(s => s.Status)       : query.OrderBy(s => s.Status), // Sắp xếp danh sách kết quả theo thứ tự giảm dần.
                _              => sortDescending ? query.OrderByDescending(s => s.CreatedDate)  : query.OrderBy(s => s.CreatedDate) // Sắp xếp danh sách kết quả theo thứ tự giảm dần.
            };

            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(); // Truy vấn bất đồng bộ và chuyển đổi kết quả trả về dạng danh sách List.
            return (items, totalCount); // Trả về giá trị của biểu thức '(items, totalCount)'.
        }

        public async Task<Dictionary<byte, int>> GetStatusCountsAsync(int? productId, int? variantId) // Thực hiện xử lý bất đồng bộ phương thức 'GetStatusCountsAsync' nhận tham số (productId, variantId) trả về kiểu Task<Dictionary<byte, int>>.
        {
            var query = _context.ProductSerials.AsNoTracking().AsQueryable(); // Tắt tính năng theo dõi trạng thái thực thể (AsNoTracking) để tối ưu hiệu năng đọc dữ liệu.
            if (productId.HasValue) // Kiểm tra điều kiện: 'productId.HasValue'.
                query = query.Where(s => s.Variant.ProductId == productId.Value); // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(s => s.Variant.ProductId == productId.Value);.
            if (variantId.HasValue) // Kiểm tra điều kiện: 'variantId.HasValue'.
                query = query.Where(s => s.VariantId == variantId.Value); // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(s => s.VariantId == variantId.Value);.

            return await query // Chờ và trả về giá trị của tác vụ bất đồng bộ 'query'.
                .GroupBy(s => s.Status) // Thực thi dòng lệnh nghiệp vụ.
                .Select(g => new { Status = g.Key, Count = g.Count() }) // Thực thi dòng lệnh nghiệp vụ.
                .ToDictionaryAsync(x => x.Status, x => x.Count); // Thực thi dòng lệnh nghiệp vụ.
        }

        public async Task<ProductSerial?> GetByIdWithDetailsAsync(int id) // Thực hiện xử lý bất đồng bộ phương thức 'GetByIdWithDetailsAsync' nhận tham số (id) trả về kiểu Task<ProductSerial?>.
        {
            return await _context.ProductSerials // Chờ và trả về giá trị của tác vụ bất đồng bộ '_context.ProductSerials'.
                .AsNoTracking() // Tắt tính năng theo dõi trạng thái thực thể (AsNoTracking) để tối ưu hiệu năng đọc dữ liệu.
                .Include(s => s.Variant).ThenInclude(v => v.Product) // Nạp kèm (Eager load) dữ liệu quan hệ liên quan: .Include(s => s.Variant).ThenInclude(v => v.Product).
                .Include(s => s.ImportReceipt).ThenInclude(r => r.Supplier) // Nạp kèm (Eager load) dữ liệu quan hệ liên quan: .Include(s => s.ImportReceipt).ThenInclude(r => r.Supplier).
                .FirstOrDefaultAsync(s => s.Id == id); // Lấy phần tử đầu tiên thỏa mãn điều kiện hoặc trả về null nếu không tìm thấy.
        }

        public async Task<List<(int SerialId, int VariantId, string SerialNumber)>> GetAvailableSerialsBatchAsync(List<int> variantIds) // Thực hiện xử lý bất đồng bộ phương thức 'Task<List<' nhận tham số (SerialId, VariantId, SerialNumber) trả về kiểu .
        {
            return await _context.ProductSerials // Chờ và trả về giá trị của tác vụ bất đồng bộ '_context.ProductSerials'.
                .AsNoTracking() // Tắt tính năng theo dõi trạng thái thực thể (AsNoTracking) để tối ưu hiệu năng đọc dữ liệu.
                .Where(s => variantIds.Contains(s.VariantId) && s.Status == 0) // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(s => variantIds.Contains(s.VariantId) && s.Status == 0).
                .Select(s => new { s.Id, s.VariantId, s.SerialNumber }) // Thực thi dòng lệnh nghiệp vụ.
                .ToListAsync() // Truy vấn bất đồng bộ và chuyển đổi kết quả trả về dạng danh sách List.
                .ContinueWith(t => t.Result // Thực thi dòng lệnh nghiệp vụ.
                    .Select(x => (x.Id, x.VariantId, x.SerialNumber)) // Thực thi dòng lệnh nghiệp vụ.
                    .ToList()); // Thực thi dòng lệnh nghiệp vụ.
        }
    }
}

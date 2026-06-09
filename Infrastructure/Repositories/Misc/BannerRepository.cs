using Microsoft.EntityFrameworkCore; // Nhập thư viện Microsoft.EntityFrameworkCore để làm việc với CSDL qua ORM.
using PBL3.Core.Entities; // Nhập namespace chứa các thực thể nghiệp vụ cốt lưu trữ của hệ thống.
using PBL3.Core.Interfaces; // Nhập namespace chứa các giao diện (interface) dùng chung của hệ thống.
using PBL3.Infrastructure.Data; // Nhập namespace chứa DbContext và các cấu hình dữ liệu của Infrastructure.

namespace PBL3.Infrastructure.Repositories // Định nghĩa namespace PBL3.Infrastructure.Repositories quản lý cấu trúc code.
{
    public class BannerRepository(HushStoreDbContext context) : IBannerRepository // Định nghĩa lớp BannerRepository sử dụng Primary Constructor nhận (HushStoreDbContext context) triển khai/kế thừa IBannerRepository.
    {
        private readonly HushStoreDbContext _context = // Khai báo trường dữ liệu private readonly và bắt đầu gán giá trị.
            context ?? throw new ArgumentNullException(nameof(context)); // Kiểm tra null và ném ra ngoại lệ ArgumentNullException nếu đối tượng context bị null.

        public async Task<(List<Banner> Items, int TotalCount)> GetPagedListAsync( // Thực hiện xử lý bất đồng bộ phương thức 'Task<' nhận tham số (Items, TotalCount) trả về kiểu .
            string? keyword, // Thực thi dòng lệnh nghiệp vụ.
            int pageNumber, // Thực thi dòng lệnh nghiệp vụ.
            int pageSize, // Thực thi dòng lệnh nghiệp vụ.
            string? sortBy, // Thực thi dòng lệnh nghiệp vụ.
            bool sortDescending) // Thực thi dòng lệnh nghiệp vụ.
        {
            var query = _context.Banners.AsNoTracking().AsQueryable(); // Tắt tính năng theo dõi trạng thái thực thể (AsNoTracking) để tối ưu hiệu năng đọc dữ liệu.

            if (!string.IsNullOrWhiteSpace(keyword)) // Kiểm tra điều kiện: '!string.IsNullOrWhiteSpace(keyword'.
            {
                var kw = keyword.Trim().ToLower(); // Thực hiện gán giá trị của biểu thức 'keyword.Trim().ToLower()' cho biến 'kw'.
                query = query.Where(b => b.Title.ToLower().Contains(kw)); // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(b => b.Title.ToLower().Contains(kw));.
            }

            var totalCount = await query.CountAsync(); // Tính toán tổng số lượng bản ghi thỏa mãn điều kiện bất đồng bộ.

            query = sortBy?.ToLower() switch // Thực hiện gán giá trị của biểu thức 'sortBy?.ToLower() switch' cho biến 'query'.
            {
                "title"       => sortDescending ? query.OrderByDescending(b => b.Title)       : query.OrderBy(b => b.Title), // Sắp xếp danh sách kết quả theo thứ tự giảm dần.
                "sortorder"   => sortDescending ? query.OrderByDescending(b => b.SortOrder)   : query.OrderBy(b => b.SortOrder), // Sắp xếp danh sách kết quả theo thứ tự giảm dần.
                "createddate" => sortDescending ? query.OrderByDescending(b => b.CreatedDate) : query.OrderBy(b => b.CreatedDate), // Sắp xếp danh sách kết quả theo thứ tự giảm dần.
                _             => query.OrderBy(b => b.SortOrder).ThenBy(b => b.Id) // Sắp xếp danh sách kết quả theo thứ tự tăng dần.
            };

            var items = await query // Chờ tác vụ bất đồng bộ hoàn thành và gán kết quả cho biến 'items'.
                .Skip((pageNumber - 1) * pageSize) // Thực thi dòng lệnh nghiệp vụ.
                .Take(pageSize) // Thực thi dòng lệnh nghiệp vụ.
                .ToListAsync(); // Truy vấn bất đồng bộ và chuyển đổi kết quả trả về dạng danh sách List.

            return (items, totalCount); // Trả về giá trị của biểu thức '(items, totalCount)'.
        }

        public async Task<Banner?> GetByIdAsync(int id) // Thực hiện xử lý bất đồng bộ phương thức 'GetByIdAsync' nhận tham số (id) trả về kiểu Task<Banner?>.
        {
            return await _context.Banners // Chờ và trả về giá trị của tác vụ bất đồng bộ '_context.Banners'.
                .AsNoTracking() // Tắt tính năng theo dõi trạng thái thực thể (AsNoTracking) để tối ưu hiệu năng đọc dữ liệu.
                .FirstOrDefaultAsync(b => b.Id == id); // Lấy phần tử đầu tiên thỏa mãn điều kiện hoặc trả về null nếu không tìm thấy.
        }

        public async Task<Banner?> GetByIdWithTrackingAsync(int id) // Thực hiện xử lý bất đồng bộ phương thức 'GetByIdWithTrackingAsync' nhận tham số (id) trả về kiểu Task<Banner?>.
        {
            return await _context.Banners // Chờ và trả về giá trị của tác vụ bất đồng bộ '_context.Banners'.
                .FirstOrDefaultAsync(b => b.Id == id); // Lấy phần tử đầu tiên thỏa mãn điều kiện hoặc trả về null nếu không tìm thấy.
        }

        public async Task<List<Banner>> GetActiveAsync(DateTime nowUtc) // Thực hiện xử lý bất đồng bộ phương thức 'GetActiveAsync' nhận tham số (nowUtc) trả về kiểu Task<List<Banner>>.
        {
            return await _context.Banners // Chờ và trả về giá trị của tác vụ bất đồng bộ '_context.Banners'.
                .AsNoTracking() // Tắt tính năng theo dõi trạng thái thực thể (AsNoTracking) để tối ưu hiệu năng đọc dữ liệu.
                .Where(b => b.IsActive // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(b => b.IsActive.
                            && (b.StartDate == null || b.StartDate <= nowUtc) // Thực thi dòng lệnh nghiệp vụ.
                            && (b.EndDate == null || b.EndDate >= nowUtc)) // Thực thi dòng lệnh nghiệp vụ.
                .OrderBy(b => b.SortOrder) // Sắp xếp danh sách kết quả theo thứ tự tăng dần.
                .ThenBy(b => b.Id) // Thực thi dòng lệnh nghiệp vụ.
                .ToListAsync(); // Truy vấn bất đồng bộ và chuyển đổi kết quả trả về dạng danh sách List.
        }

        public async Task AddAsync(Banner banner) // Thực hiện xử lý bất đồng bộ phương thức 'AddAsync' nhận tham số (banner) trả về kiểu Task.
        {
            _context.Banners.Add(banner); // Thêm thực thể mới vào CSDL thông qua DbSet tương ứng.
            await Task.CompletedTask; // Trả về trạng thái tác vụ đã hoàn thành.
        }

        public async Task SaveChangesAsync() // Thực hiện xử lý bất đồng bộ phương thức 'SaveChangesAsync' không tham số trả về kiểu Task.
        {
            await _context.SaveChangesAsync(); // Thực thi dòng lệnh nghiệp vụ.
        }
    }
}

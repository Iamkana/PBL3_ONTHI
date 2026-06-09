using Microsoft.EntityFrameworkCore; // Nhập thư viện Microsoft.EntityFrameworkCore để làm việc với CSDL qua ORM.
using PBL3.Core.Entities; // Nhập namespace chứa các thực thể nghiệp vụ cốt lưu trữ của hệ thống.
using PBL3.Core.Interfaces; // Nhập namespace chứa các giao diện (interface) dùng chung của hệ thống.
using PBL3.Infrastructure.Data; // Nhập namespace chứa DbContext và các cấu hình dữ liệu của Infrastructure.

namespace PBL3.Infrastructure.Repositories // Định nghĩa namespace PBL3.Infrastructure.Repositories quản lý cấu trúc code.
{
    public class ManufacturerRepository(HushStoreDbContext context) : IManufacturerRepository // Định nghĩa lớp ManufacturerRepository sử dụng Primary Constructor nhận (HushStoreDbContext context) triển khai/kế thừa IManufacturerRepository.
    {
        private readonly HushStoreDbContext _context = // Khai báo trường dữ liệu private readonly và bắt đầu gán giá trị.
            context ?? throw new ArgumentNullException(nameof(context)); // Kiểm tra null và ném ra ngoại lệ ArgumentNullException nếu đối tượng context bị null.

        // ========================================================
        // GET PAGED LIST — Phân trang + tìm kiếm
        // ========================================================
        public async Task<(List<Manufacturer> Items, int TotalCount)> GetPagedListAsync( // Thực hiện xử lý bất đồng bộ phương thức 'Task<' nhận tham số (Items, TotalCount) trả về kiểu .
            string? keyword, // Thực thi dòng lệnh nghiệp vụ.
            int pageNumber, // Thực thi dòng lệnh nghiệp vụ.
            int pageSize, // Thực thi dòng lệnh nghiệp vụ.
            string? sortBy, // Thực thi dòng lệnh nghiệp vụ.
            bool sortDescending) // Thực thi dòng lệnh nghiệp vụ.
        {
            // Global Query Filter tự động lọc IsDeleted == true
            var query = _context.Manufacturers.AsNoTracking().AsQueryable(); // Tắt tính năng theo dõi trạng thái thực thể (AsNoTracking) để tối ưu hiệu năng đọc dữ liệu.

            // Tìm kiếm theo Tên hoặc Website
            if (!string.IsNullOrWhiteSpace(keyword)) // Kiểm tra điều kiện: '!string.IsNullOrWhiteSpace(keyword'.
            {
                var kw = keyword.Trim().ToLower(); // Thực hiện gán giá trị của biểu thức 'keyword.Trim().ToLower()' cho biến 'kw'.
                query = query.Where(m => // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(m =>.
                    m.Name.ToLower().Contains(kw) || // Thực thi dòng lệnh nghiệp vụ.
                    (m.Website != null && m.Website.ToLower().Contains(kw))); // Thực thi dòng lệnh nghiệp vụ.
            }

            // Đếm tổng trước khi phân trang
            var totalCount = await query.CountAsync(); // Tính toán tổng số lượng bản ghi thỏa mãn điều kiện bất đồng bộ.

            // Sắp xếp
            query = sortBy?.ToLower() switch // Thực hiện gán giá trị của biểu thức 'sortBy?.ToLower() switch' cho biến 'query'.
            {
                "name"        => sortDescending ? query.OrderByDescending(m => m.Name) : query.OrderBy(m => m.Name), // Sắp xếp danh sách kết quả theo thứ tự giảm dần.
                "createddate" => sortDescending ? query.OrderByDescending(m => m.CreatedDate) : query.OrderBy(m => m.CreatedDate), // Sắp xếp danh sách kết quả theo thứ tự giảm dần.
                _             => query.OrderBy(m => m.Name) // Sắp xếp danh sách kết quả theo thứ tự tăng dần.
            };

            // Phân trang
            var items = await query // Chờ tác vụ bất đồng bộ hoàn thành và gán kết quả cho biến 'items'.
                .Skip((pageNumber - 1) * pageSize) // Thực thi dòng lệnh nghiệp vụ.
                .Take(pageSize) // Thực thi dòng lệnh nghiệp vụ.
                .ToListAsync(); // Truy vấn bất đồng bộ và chuyển đổi kết quả trả về dạng danh sách List.

            return (items, totalCount); // Trả về giá trị của biểu thức '(items, totalCount)'.
        }

        // ========================================================
        // GET BY ID
        // ========================================================
        public async Task<Manufacturer?> GetByIdAsync(int id) // Thực hiện xử lý bất đồng bộ phương thức 'GetByIdAsync' nhận tham số (id) trả về kiểu Task<Manufacturer?>.
        {
            // Global Query Filter tự động loại IsDeleted
            return await _context.Manufacturers // Chờ và trả về giá trị của tác vụ bất đồng bộ '_context.Manufacturers'.
                .FirstOrDefaultAsync(m => m.Id == id); // Lấy phần tử đầu tiên thỏa mãn điều kiện hoặc trả về null nếu không tìm thấy.
        }

        // ========================================================
        // GET ALL ACTIVE — Dùng cho Dropdown
        // ========================================================
        public async Task<List<Manufacturer>> GetAllActiveAsync() // Thực hiện xử lý bất đồng bộ phương thức 'GetAllActiveAsync' không tham số trả về kiểu Task<List<Manufacturer>>.
        {
            return await _context.Manufacturers // Chờ và trả về giá trị của tác vụ bất đồng bộ '_context.Manufacturers'.
                .AsNoTracking() // Tắt tính năng theo dõi trạng thái thực thể (AsNoTracking) để tối ưu hiệu năng đọc dữ liệu.
                .OrderBy(m => m.Name) // Sắp xếp danh sách kết quả theo thứ tự tăng dần.
                .ToListAsync(); // Truy vấn bất đồng bộ và chuyển đổi kết quả trả về dạng danh sách List.
        }

        // ========================================================
        // HAS PRODUCTS — Kiểm tra ràng buộc xóa
        // ========================================================
        public async Task<bool> HasProductsAsync(int manufacturerId) // Thực hiện xử lý bất đồng bộ phương thức 'HasProductsAsync' nhận tham số (manufacturerId) trả về kiểu Task<bool>.
        {
            return await _context.Products // Chờ và trả về giá trị của tác vụ bất đồng bộ '_context.Products'.
                .AnyAsync(p => p.ManufacturerId == manufacturerId && !p.IsDeleted); // Kiểm tra xem có bất kỳ bản ghi nào khớp với điều kiện hay không.
        }

        // ========================================================
        // IS DUPLICATE NAME — Kiểm tra trùng tên
        // ========================================================
        public async Task<bool> IsDuplicateNameAsync(string name, int? excludeId = null) // Thực hiện xử lý bất đồng bộ phương thức 'IsDuplicateNameAsync' nhận tham số (name, null) trả về kiểu Task<bool>.
        {
            var normalizedName = name.Trim().ToLower(); // Thực hiện gán giá trị của biểu thức 'name.Trim().ToLower()' cho biến 'normalizedName'.
            return await _context.Manufacturers // Chờ và trả về giá trị của tác vụ bất đồng bộ '_context.Manufacturers'.
                .AnyAsync(m => // Kiểm tra xem có bất kỳ bản ghi nào khớp với điều kiện hay không.
                    m.Name.ToLower() == normalizedName && // Thực thi dòng lệnh nghiệp vụ.
                    (excludeId == null || m.Id != excludeId)); // Thực thi dòng lệnh nghiệp vụ.
        }

        // ========================================================
        // ADD + SAVE
        // ========================================================
        public async Task AddAsync(Manufacturer manufacturer) // Thực hiện xử lý bất đồng bộ phương thức 'AddAsync' nhận tham số (manufacturer) trả về kiểu Task.
        {
            _context.Manufacturers.Add(manufacturer); // Thêm thực thể mới vào CSDL thông qua DbSet tương ứng.
            await Task.CompletedTask; // Trả về trạng thái tác vụ đã hoàn thành.
        }

        public async Task SaveChangesAsync() // Thực hiện xử lý bất đồng bộ phương thức 'SaveChangesAsync' không tham số trả về kiểu Task.
        {
            await _context.SaveChangesAsync(); // Thực thi dòng lệnh nghiệp vụ.
        }
    }
}

using Microsoft.EntityFrameworkCore; // Nhập thư viện Microsoft.EntityFrameworkCore để làm việc với CSDL qua ORM.
using PBL3.Core.Entities; // Nhập namespace chứa các thực thể nghiệp vụ cốt lưu trữ của hệ thống.
using PBL3.Core.Interfaces; // Nhập namespace chứa các giao diện (interface) dùng chung của hệ thống.
using PBL3.Infrastructure.Data; // Nhập namespace chứa DbContext và các cấu hình dữ liệu của Infrastructure.

namespace PBL3.Infrastructure.Repositories // Định nghĩa namespace PBL3.Infrastructure.Repositories quản lý cấu trúc code.
{
    public class CategoryRepository(HushStoreDbContext context) : ICategoryRepository // Định nghĩa lớp CategoryRepository sử dụng Primary Constructor nhận (HushStoreDbContext context) triển khai/kế thừa ICategoryRepository.
    {
        private readonly HushStoreDbContext _context = // Khai báo trường dữ liệu private readonly và bắt đầu gán giá trị.
            context ?? throw new ArgumentNullException(nameof(context)); // Kiểm tra null và ném ra ngoại lệ ArgumentNullException nếu đối tượng context bị null.

        public async Task<List<Category>> GetAllActiveAsync() // Thực hiện xử lý bất đồng bộ phương thức 'GetAllActiveAsync' không tham số trả về kiểu Task<List<Category>>.
        {
            return await _context.Categories // Chờ và trả về giá trị của tác vụ bất đồng bộ '_context.Categories'.
                .AsNoTracking() // Tắt tính năng theo dõi trạng thái thực thể (AsNoTracking) để tối ưu hiệu năng đọc dữ liệu.
                .Where(c => !c.IsDeleted) // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(c => !c.IsDeleted).
                .OrderBy(c => c.SortOrder) // Sắp xếp danh sách kết quả theo thứ tự tăng dần.
                .ThenBy(c => c.Name) // Thực thi dòng lệnh nghiệp vụ.
                .ToListAsync(); // Truy vấn bất đồng bộ và chuyển đổi kết quả trả về dạng danh sách List.
        }

        public async Task<Category?> GetByIdAsync(int id, bool includeParent = false) // Thực hiện xử lý bất đồng bộ phương thức 'GetByIdAsync' nhận tham số (id, false) trả về kiểu Task<Category?>.
        {
            var query = _context.Categories.AsQueryable(); // Chuyển cấu trúc dữ liệu thành IQueryable để hoãn việc thực thi truy vấn.

            if (includeParent) // Kiểm tra điều kiện: 'includeParent'.
                query = query.Include(c => c.Parent); // Nạp kèm (Eager load) dữ liệu quan hệ liên quan: .Include(c => c.Parent);.

            return await query.FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted); // Lấy phần tử đầu tiên thỏa mãn điều kiện hoặc trả về null nếu không tìm thấy.
        }

        public async Task<bool> IsDuplicateNameAsync(int? parentId, string name, int? excludeId = null) // Thực hiện xử lý bất đồng bộ phương thức 'IsDuplicateNameAsync' nhận tham số (parentId, name, null) trả về kiểu Task<bool>.
        {
            var query = _context.Categories // Thực hiện gán giá trị của biểu thức '_context.Categories' cho biến 'query'.
                .Where(c => c.ParentId == parentId && c.Name == name && !c.IsDeleted); // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(c => c.ParentId == parentId && c.Name == name && !c.IsDeleted);.

            if (excludeId.HasValue) // Kiểm tra điều kiện: 'excludeId.HasValue'.
                query = query.Where(c => c.Id != excludeId.Value); // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(c => c.Id != excludeId.Value);.

            return await query.AnyAsync(); // Kiểm tra xem có bất kỳ bản ghi nào khớp với điều kiện hay không.
        }

        public async Task<bool> IsDuplicateSlugAsync(string slug, int? excludeId = null) // Thực hiện xử lý bất đồng bộ phương thức 'IsDuplicateSlugAsync' nhận tham số (slug, null) trả về kiểu Task<bool>.
        {
            var query = _context.Categories // Thực hiện gán giá trị của biểu thức '_context.Categories' cho biến 'query'.
                .Where(c => c.Slug == slug && !c.IsDeleted); // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(c => c.Slug == slug && !c.IsDeleted);.

            if (excludeId.HasValue) // Kiểm tra điều kiện: 'excludeId.HasValue'.
                query = query.Where(c => c.Id != excludeId.Value); // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(c => c.Id != excludeId.Value);.

            return await query.AnyAsync(); // Kiểm tra xem có bất kỳ bản ghi nào khớp với điều kiện hay không.
        }

        public async Task<bool> HasActiveChildrenAsync(int id) // Thực hiện xử lý bất đồng bộ phương thức 'HasActiveChildrenAsync' nhận tham số (id) trả về kiểu Task<bool>.
        {
            return await _context.Categories // Chờ và trả về giá trị của tác vụ bất đồng bộ '_context.Categories'.
                .AnyAsync(c => c.ParentId == id && !c.IsDeleted); // Kiểm tra xem có bất kỳ bản ghi nào khớp với điều kiện hay không.
        }

        public async Task<bool> HasProductsAsync(int id) // Thực hiện xử lý bất đồng bộ phương thức 'HasProductsAsync' nhận tham số (id) trả về kiểu Task<bool>.
        {
            return await _context.Products // Chờ và trả về giá trị của tác vụ bất đồng bộ '_context.Products'.
                .AnyAsync(p => p.CategoryId == id && !p.IsDeleted); // Kiểm tra xem có bất kỳ bản ghi nào khớp với điều kiện hay không.
        }

        public async Task<Dictionary<int, int?>> GetAllCategoryParentMapAsync() // Thực hiện xử lý bất đồng bộ phương thức 'GetAllCategoryParentMapAsync' không tham số trả về kiểu Task<Dictionary<int, int?>>.
        {
            return await _context.Categories // Chờ và trả về giá trị của tác vụ bất đồng bộ '_context.Categories'.
                .AsNoTracking() // Tắt tính năng theo dõi trạng thái thực thể (AsNoTracking) để tối ưu hiệu năng đọc dữ liệu.
                .Where(c => !c.IsDeleted) // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(c => !c.IsDeleted).
                .Select(c => new { c.Id, c.ParentId }) // Thực thi dòng lệnh nghiệp vụ.
                .ToDictionaryAsync(c => c.Id, c => c.ParentId); // Thực thi dòng lệnh nghiệp vụ.
        }

        public async Task<List<Category>> GetChildrenAsync(int parentId) // Thực hiện xử lý bất đồng bộ phương thức 'GetChildrenAsync' nhận tham số (parentId) trả về kiểu Task<List<Category>>.
        {
            return await _context.Categories // Chờ và trả về giá trị của tác vụ bất đồng bộ '_context.Categories'.
                .Where(c => c.ParentId == parentId && !c.IsDeleted) // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(c => c.ParentId == parentId && !c.IsDeleted).
                .ToListAsync(); // Truy vấn bất đồng bộ và chuyển đổi kết quả trả về dạng danh sách List.
        }

        public async Task AddAsync(Category category) // Thực hiện xử lý bất đồng bộ phương thức 'AddAsync' nhận tham số (category) trả về kiểu Task.
        {
            _context.Categories.Add(category); // Thêm thực thể mới vào CSDL thông qua DbSet tương ứng.
            await Task.CompletedTask; // Trả về trạng thái tác vụ đã hoàn thành.
        }

        public async Task SaveChangesAsync() // Thực hiện xử lý bất đồng bộ phương thức 'SaveChangesAsync' không tham số trả về kiểu Task.
        {
            await _context.SaveChangesAsync(); // Thực thi dòng lệnh nghiệp vụ.
        }
    }
}

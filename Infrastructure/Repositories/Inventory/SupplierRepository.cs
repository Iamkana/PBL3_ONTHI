using Microsoft.EntityFrameworkCore; // Nhập thư viện Microsoft.EntityFrameworkCore để làm việc với CSDL qua ORM.
using PBL3.Core.Entities; // Nhập namespace chứa các thực thể nghiệp vụ cốt lưu trữ của hệ thống.
using PBL3.Core.Interfaces; // Nhập namespace chứa các giao diện (interface) dùng chung của hệ thống.
using PBL3.Infrastructure.Data; // Nhập namespace chứa DbContext và các cấu hình dữ liệu của Infrastructure.

namespace PBL3.Infrastructure.Repositories // Định nghĩa namespace PBL3.Infrastructure.Repositories quản lý cấu trúc code.
{
    public class SupplierRepository(HushStoreDbContext context) : ISupplierRepository // Định nghĩa lớp SupplierRepository sử dụng Primary Constructor nhận (HushStoreDbContext context) triển khai/kế thừa ISupplierRepository.
    {
        private readonly HushStoreDbContext _context = // Khai báo trường dữ liệu private readonly và bắt đầu gán giá trị.
            context ?? throw new ArgumentNullException(nameof(context)); // Kiểm tra null và ném ra ngoại lệ ArgumentNullException nếu đối tượng context bị null.

        public async Task<(List<Supplier> Items, int TotalCount)> GetPagedListAsync( // Thực hiện xử lý bất đồng bộ phương thức 'Task<' nhận tham số (Items, TotalCount) trả về kiểu .
            string? keyword, // Thực thi dòng lệnh nghiệp vụ.
            int pageNumber, // Thực thi dòng lệnh nghiệp vụ.
            int pageSize, // Thực thi dòng lệnh nghiệp vụ.
            string? sortBy, // Thực thi dòng lệnh nghiệp vụ.
            bool sortDescending) // Thực thi dòng lệnh nghiệp vụ.
        {
            // Global Query Filter đã tự động lọc IsDeleted == true
            var query = _context.Suppliers.AsNoTracking().AsQueryable(); // Tắt tính năng theo dõi trạng thái thực thể (AsNoTracking) để tối ưu hiệu năng đọc dữ liệu.

            // Tìm kiếm theo Tên hoặc Số điện thoại
            if (!string.IsNullOrWhiteSpace(keyword)) // Kiểm tra điều kiện: '!string.IsNullOrWhiteSpace(keyword'.
            {
                var kw = keyword.Trim().ToLower(); // Thực hiện gán giá trị của biểu thức 'keyword.Trim().ToLower()' cho biến 'kw'.
                query = query.Where(s => // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(s =>.
                    s.Name.ToLower().Contains(kw) || // Thực thi dòng lệnh nghiệp vụ.
                    s.PhoneNumber.Contains(kw)); // Thực thi dòng lệnh nghiệp vụ.
            }

            // Đếm tổng trước khi phân trang
            var totalCount = await query.CountAsync(); // Tính toán tổng số lượng bản ghi thỏa mãn điều kiện bất đồng bộ.

            // Sắp xếp
            query = sortBy?.ToLower() switch // Thực hiện gán giá trị của biểu thức 'sortBy?.ToLower() switch' cho biến 'query'.
            {
                "name" => sortDescending ? query.OrderByDescending(s => s.Name) : query.OrderBy(s => s.Name), // Sắp xếp danh sách kết quả theo thứ tự giảm dần.
                "phone" => sortDescending ? query.OrderByDescending(s => s.PhoneNumber) : query.OrderBy(s => s.PhoneNumber), // Sắp xếp danh sách kết quả theo thứ tự giảm dần.
                "createddate" => sortDescending ? query.OrderByDescending(s => s.CreatedDate) : query.OrderBy(s => s.CreatedDate), // Sắp xếp danh sách kết quả theo thứ tự giảm dần.
                _ => query.OrderByDescending(s => s.CreatedDate) // Sắp xếp danh sách kết quả theo thứ tự giảm dần.
            };

            // Phân trang
            var items = await query // Chờ tác vụ bất đồng bộ hoàn thành và gán kết quả cho biến 'items'.
                .Skip((pageNumber - 1) * pageSize) // Thực thi dòng lệnh nghiệp vụ.
                .Take(pageSize) // Thực thi dòng lệnh nghiệp vụ.
                .ToListAsync(); // Truy vấn bất đồng bộ và chuyển đổi kết quả trả về dạng danh sách List.

            return (items, totalCount); // Trả về giá trị của biểu thức '(items, totalCount)'.
        }

        public async Task<Supplier?> GetByIdAsync(int id) // Thực hiện xử lý bất đồng bộ phương thức 'GetByIdAsync' nhận tham số (id) trả về kiểu Task<Supplier?>.
        {
            // Global Query Filter tự động loại IsDeleted
            return await _context.Suppliers // Chờ và trả về giá trị của tác vụ bất đồng bộ '_context.Suppliers'.
                .FirstOrDefaultAsync(s => s.Id == id); // Lấy phần tử đầu tiên thỏa mãn điều kiện hoặc trả về null nếu không tìm thấy.
        }

        public async Task<bool> HasImportReceiptsAsync(int supplierId) // Thực hiện xử lý bất đồng bộ phương thức 'HasImportReceiptsAsync' nhận tham số (supplierId) trả về kiểu Task<bool>.
        {
            return await _context.ImportReceipts // Chờ và trả về giá trị của tác vụ bất đồng bộ '_context.ImportReceipts'.
                .AnyAsync(r => r.SupplierId == supplierId); // Kiểm tra xem có bất kỳ bản ghi nào khớp với điều kiện hay không.
        }

        public async Task AddAsync(Supplier supplier) // Thực hiện xử lý bất đồng bộ phương thức 'AddAsync' nhận tham số (supplier) trả về kiểu Task.
        {
            _context.Suppliers.Add(supplier); // Thêm thực thể mới vào CSDL thông qua DbSet tương ứng.
            await Task.CompletedTask; // Trả về trạng thái tác vụ đã hoàn thành.
        }

        public async Task SaveChangesAsync() // Thực hiện xử lý bất đồng bộ phương thức 'SaveChangesAsync' không tham số trả về kiểu Task.
        {
            await _context.SaveChangesAsync(); // Thực thi dòng lệnh nghiệp vụ.
        }
    }
}

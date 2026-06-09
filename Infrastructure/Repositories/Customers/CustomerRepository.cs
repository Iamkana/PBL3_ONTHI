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
    public class CustomerRepository(HushStoreDbContext context) : ICustomerRepository // Định nghĩa lớp CustomerRepository sử dụng Primary Constructor nhận (HushStoreDbContext context) triển khai/kế thừa ICustomerRepository.
    {
        private readonly HushStoreDbContext _context = // Khai báo trường dữ liệu private readonly và bắt đầu gán giá trị.
            context ?? throw new ArgumentNullException(nameof(context)); // Kiểm tra null và ném ra ngoại lệ ArgumentNullException nếu đối tượng context bị null.

        public async Task<(List<AppUser> Items, int TotalCount)> GetPagedListAsync( // Thực hiện xử lý bất đồng bộ phương thức 'Task<' nhận tham số (Items, TotalCount) trả về kiểu .
            string? keyword, // Thực thi dòng lệnh nghiệp vụ.
            bool? isActive, // Thực thi dòng lệnh nghiệp vụ.
            byte? gender, // Thực thi dòng lệnh nghiệp vụ.
            int pageNumber, // Thực thi dòng lệnh nghiệp vụ.
            int pageSize, // Thực thi dòng lệnh nghiệp vụ.
            string? sortBy, // Thực thi dòng lệnh nghiệp vụ.
            bool sortDescending) // Thực thi dòng lệnh nghiệp vụ.
        {
            // Join AppUser and UserProfile to filter and sort efficiently
            // Only customers (Type = 2)
            var query = _context.Users // Thực hiện gán giá trị của biểu thức '_context.Users' cho biến 'query'.
                .Include(u => u.Profile) // Nạp kèm (Eager load) dữ liệu quan hệ liên quan: .Include(u => u.Profile).
                .Where(u => u.Type == 2) // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(u => u.Type == 2).
                .AsNoTracking() // Tắt tính năng theo dõi trạng thái thực thể (AsNoTracking) để tối ưu hiệu năng đọc dữ liệu.
                .AsQueryable(); // Chuyển cấu trúc dữ liệu thành IQueryable để hoãn việc thực thi truy vấn.

            // Lọc theo IsActive nếu có
            if (isActive.HasValue) // Kiểm tra điều kiện: 'isActive.HasValue'.
                query = query.Where(u => u.IsActive == isActive.Value); // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(u => u.IsActive == isActive.Value);.

            // Lọc theo giới tính nếu có
            if (gender.HasValue) // Kiểm tra điều kiện: 'gender.HasValue'.
                query = query.Where(u => u.Profile != null && u.Profile.Gender == gender.Value); // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(u => u.Profile != null && u.Profile.Gender == gender.Value);.

            // Lọc theo keyword (tìm trên Email, PhoneNumber, FullName)
            if (!string.IsNullOrWhiteSpace(keyword)) // Kiểm tra điều kiện: '!string.IsNullOrWhiteSpace(keyword'.
            {
                var lowerKeyword = keyword.ToLower(); // Thực hiện gán giá trị của biểu thức 'keyword.ToLower()' cho biến 'lowerKeyword'.
                query = query.Where(u => // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(u =>.
                    (u.Email != null && u.Email.ToLower().Contains(lowerKeyword)) || // Thực thi dòng lệnh nghiệp vụ.
                    (u.PhoneNumber != null && u.PhoneNumber.Contains(lowerKeyword)) || // Thực thi dòng lệnh nghiệp vụ.
                    (u.Profile != null && u.Profile.FullName.ToLower().Contains(lowerKeyword))); // Thực thi dòng lệnh nghiệp vụ.
            }

            // Sắp xếp
            if (!string.IsNullOrWhiteSpace(sortBy)) // Kiểm tra điều kiện: '!string.IsNullOrWhiteSpace(sortBy'.
            {
                switch (sortBy.ToLower()) // Cấu trúc rẽ nhánh switch để xử lý các trường hợp tương ứng của biểu thức điều kiện.
                {
                    case "fullname": // Trường hợp biểu thức khớp với giá trị: "fullname".
                        query = sortDescending // Thực hiện gán giá trị của biểu thức 'sortDescending' cho biến 'query'.
                            ? query.OrderByDescending(u => u.Profile!.FullName) // Sắp xếp danh sách kết quả theo thứ tự giảm dần.
                            : query.OrderBy(u => u.Profile!.FullName); // Sắp xếp danh sách kết quả theo thứ tự tăng dần.
                        break; // Thoát khỏi cấu trúc rẽ nhánh hiện tại.
                    case "email": // Trường hợp biểu thức khớp với giá trị: "email".
                        query = sortDescending // Thực hiện gán giá trị của biểu thức 'sortDescending' cho biến 'query'.
                            ? query.OrderByDescending(u => u.Email) // Sắp xếp danh sách kết quả theo thứ tự giảm dần.
                            : query.OrderBy(u => u.Email); // Sắp xếp danh sách kết quả theo thứ tự tăng dần.
                        break; // Thoát khỏi cấu trúc rẽ nhánh hiện tại.
                    case "phonenumber": // Trường hợp biểu thức khớp với giá trị: "phonenumber".
                        query = sortDescending // Thực hiện gán giá trị của biểu thức 'sortDescending' cho biến 'query'.
                            ? query.OrderByDescending(u => u.PhoneNumber) // Sắp xếp danh sách kết quả theo thứ tự giảm dần.
                            : query.OrderBy(u => u.PhoneNumber); // Sắp xếp danh sách kết quả theo thứ tự tăng dần.
                        break; // Thoát khỏi cấu trúc rẽ nhánh hiện tại.
                    case "createddate": // Trường hợp biểu thức khớp với giá trị: "createddate".
                    default: // Trường hợp mặc định nếu không khớp với bất kỳ case nào phía trên.
                        query = sortDescending // Thực hiện gán giá trị của biểu thức 'sortDescending' cho biến 'query'.
                            ? query.OrderByDescending(u => u.CreatedDate) // Sắp xếp danh sách kết quả theo thứ tự giảm dần.
                            : query.OrderBy(u => u.CreatedDate); // Sắp xếp danh sách kết quả theo thứ tự tăng dần.
                        break; // Thoát khỏi cấu trúc rẽ nhánh hiện tại.
                }
            }
            else // Nhánh xử lý mặc định khi các điều kiện trên đều sai.
            {
                query = query.OrderByDescending(u => u.CreatedDate); // Sắp xếp danh sách kết quả theo thứ tự giảm dần.
            }

            var totalCount = await query.CountAsync(); // Tính toán tổng số lượng bản ghi thỏa mãn điều kiện bất đồng bộ.

            var items = await query // Chờ tác vụ bất đồng bộ hoàn thành và gán kết quả cho biến 'items'.
                .Skip((pageNumber - 1) * pageSize) // Thực thi dòng lệnh nghiệp vụ.
                .Take(pageSize) // Thực thi dòng lệnh nghiệp vụ.
                .ToListAsync(); // Truy vấn bất đồng bộ và chuyển đổi kết quả trả về dạng danh sách List.

            return (items, totalCount); // Trả về giá trị của biểu thức '(items, totalCount)'.
        }

        public async Task<AppUser?> GetByIdWithProfileAsync(Guid id) // Thực hiện xử lý bất đồng bộ phương thức 'GetByIdWithProfileAsync' nhận tham số (id) trả về kiểu Task<AppUser?>.
        {
            return await _context.Users // Chờ và trả về giá trị của tác vụ bất đồng bộ '_context.Users'.
                .Include(u => u.Profile) // Nạp kèm (Eager load) dữ liệu quan hệ liên quan: .Include(u => u.Profile).
                .Where(u => u.Type == 2) // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(u => u.Type == 2).
                .FirstOrDefaultAsync(u => u.Id == id); // Lấy phần tử đầu tiên thỏa mãn điều kiện hoặc trả về null nếu không tìm thấy.
        }

        public async Task<List<Order>> GetRecentOrdersAsync(Guid userId, int count) // Thực hiện xử lý bất đồng bộ phương thức 'GetRecentOrdersAsync' nhận tham số (userId, count) trả về kiểu Task<List<Order>>.
        {
            return await _context.Orders // Chờ và trả về giá trị của tác vụ bất đồng bộ '_context.Orders'.
                .Where(o => o.UserId == userId) // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(o => o.UserId == userId).
                .OrderByDescending(o => o.OrderDate) // Sắp xếp danh sách kết quả theo thứ tự giảm dần.
                .Take(count) // Thực thi dòng lệnh nghiệp vụ.
                .AsNoTracking() // Tắt tính năng theo dõi trạng thái thực thể (AsNoTracking) để tối ưu hiệu năng đọc dữ liệu.
                .ToListAsync(); // Truy vấn bất đồng bộ và chuyển đổi kết quả trả về dạng danh sách List.
        }

        public async Task<bool> HasPendingOrdersAsync(Guid userId) // Thực hiện xử lý bất đồng bộ phương thức 'HasPendingOrdersAsync' nhận tham số (userId) trả về kiểu Task<bool>.
        {
            // Statuses: 0 = Pending, 1 = Confirmed, 2 = Shipping
            return await _context.Orders // Chờ và trả về giá trị của tác vụ bất đồng bộ '_context.Orders'.
                .AnyAsync(o => o.UserId == userId && (o.Status == 0 || o.Status == 1 || o.Status == 2)); // Kiểm tra xem có bất kỳ bản ghi nào khớp với điều kiện hay không.
        }
    }
}

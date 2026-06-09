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
    public class EmployeeRepository(HushStoreDbContext context) : IEmployeeRepository // Định nghĩa lớp EmployeeRepository sử dụng Primary Constructor nhận (HushStoreDbContext context) triển khai/kế thừa IEmployeeRepository.
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
            var query = _context.Users // Thực hiện gán giá trị của biểu thức '_context.Users' cho biến 'query'.
                .Include(u => u.Profile) // Nạp kèm (Eager load) dữ liệu quan hệ liên quan: .Include(u => u.Profile).
                .Where(u => u.Type == 1) // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(u => u.Type == 1).
                .AsNoTracking() // Tắt tính năng theo dõi trạng thái thực thể (AsNoTracking) để tối ưu hiệu năng đọc dữ liệu.
                .AsQueryable(); // Chuyển cấu trúc dữ liệu thành IQueryable để hoãn việc thực thi truy vấn.

            if (isActive.HasValue) // Kiểm tra điều kiện: 'isActive.HasValue'.
                query = query.Where(u => u.IsActive == isActive.Value); // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(u => u.IsActive == isActive.Value);.

            if (gender.HasValue) // Kiểm tra điều kiện: 'gender.HasValue'.
                query = query.Where(u => u.Profile != null && u.Profile.Gender == gender.Value); // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(u => u.Profile != null && u.Profile.Gender == gender.Value);.

            if (!string.IsNullOrWhiteSpace(keyword)) // Kiểm tra điều kiện: '!string.IsNullOrWhiteSpace(keyword'.
            {
                var lower = keyword.ToLower(); // Thực hiện gán giá trị của biểu thức 'keyword.ToLower()' cho biến 'lower'.
                query = query.Where(u => // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(u =>.
                    (u.Email != null && u.Email.ToLower().Contains(lower)) || // Thực thi dòng lệnh nghiệp vụ.
                    (u.PhoneNumber != null && u.PhoneNumber.Contains(lower)) || // Thực thi dòng lệnh nghiệp vụ.
                    (u.Profile != null && u.Profile.FullName.ToLower().Contains(lower))); // Thực thi dòng lệnh nghiệp vụ.
            }

            if (!string.IsNullOrWhiteSpace(sortBy)) // Kiểm tra điều kiện: '!string.IsNullOrWhiteSpace(sortBy'.
            {
                query = sortBy.ToLower() switch // Thực hiện gán giá trị của biểu thức 'sortBy.ToLower() switch' cho biến 'query'.
                {
                    "fullname" => sortDescending // Thực thi dòng lệnh nghiệp vụ.
                        ? query.OrderByDescending(u => u.Profile!.FullName) // Sắp xếp danh sách kết quả theo thứ tự giảm dần.
                        : query.OrderBy(u => u.Profile!.FullName), // Sắp xếp danh sách kết quả theo thứ tự tăng dần.
                    "email" => sortDescending // Thực thi dòng lệnh nghiệp vụ.
                        ? query.OrderByDescending(u => u.Email) // Sắp xếp danh sách kết quả theo thứ tự giảm dần.
                        : query.OrderBy(u => u.Email), // Sắp xếp danh sách kết quả theo thứ tự tăng dần.
                    "phonenumber" => sortDescending // Thực thi dòng lệnh nghiệp vụ.
                        ? query.OrderByDescending(u => u.PhoneNumber) // Sắp xếp danh sách kết quả theo thứ tự giảm dần.
                        : query.OrderBy(u => u.PhoneNumber), // Sắp xếp danh sách kết quả theo thứ tự tăng dần.
                    _ => sortDescending // Thực hiện gán giá trị của biểu thức '> sortDescending' cho biến '_'.
                        ? query.OrderByDescending(u => u.CreatedDate) // Sắp xếp danh sách kết quả theo thứ tự giảm dần.
                        : query.OrderBy(u => u.CreatedDate) // Sắp xếp danh sách kết quả theo thứ tự tăng dần.
                };
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
                .Where(u => u.Type == 1) // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(u => u.Type == 1).
                .FirstOrDefaultAsync(u => u.Id == id); // Lấy phần tử đầu tiên thỏa mãn điều kiện hoặc trả về null nếu không tìm thấy.
        }
    }
}

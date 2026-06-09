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
    public class UserAddressRepository(HushStoreDbContext context) : IUserAddressRepository // Định nghĩa lớp UserAddressRepository sử dụng Primary Constructor nhận (HushStoreDbContext context) triển khai/kế thừa IUserAddressRepository.
    {
        private readonly HushStoreDbContext _context = // Khai báo trường dữ liệu private readonly và bắt đầu gán giá trị.
            context ?? throw new ArgumentNullException(nameof(context)); // Kiểm tra null và ném ra ngoại lệ ArgumentNullException nếu đối tượng context bị null.

        public async Task<UserAddress?> GetByIdAsync(int id) // Thực hiện xử lý bất đồng bộ phương thức 'GetByIdAsync' nhận tham số (id) trả về kiểu Task<UserAddress?>.
        {
            return await _context.UserAddresses // Chờ và trả về giá trị của tác vụ bất đồng bộ '_context.UserAddresses'.
                .FirstOrDefaultAsync(ua => ua.Id == id); // Lấy phần tử đầu tiên thỏa mãn điều kiện hoặc trả về null nếu không tìm thấy.
        }

        public async Task<List<UserAddress>> GetByUserIdAsync(Guid userId) // Thực hiện xử lý bất đồng bộ phương thức 'GetByUserIdAsync' nhận tham số (userId) trả về kiểu Task<List<UserAddress>>.
        {
            return await _context.UserAddresses // Chờ và trả về giá trị của tác vụ bất đồng bộ '_context.UserAddresses'.
                .Where(ua => ua.UserId == userId) // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(ua => ua.UserId == userId).
                .AsNoTracking() // Tắt tính năng theo dõi trạng thái thực thể (AsNoTracking) để tối ưu hiệu năng đọc dữ liệu.
                .ToListAsync(); // Truy vấn bất đồng bộ và chuyển đổi kết quả trả về dạng danh sách List.
        }

        public async Task AddAsync(UserAddress address) // Thực hiện xử lý bất đồng bộ phương thức 'AddAsync' nhận tham số (address) trả về kiểu Task.
        {
            await _context.UserAddresses.AddAsync(address); // Thêm thực thể mới vào CSDL thông qua DbSet tương ứng.
        }

        public async Task ClearUserDefaultsAsync(Guid userId) // Thực hiện xử lý bất đồng bộ phương thức 'ClearUserDefaultsAsync' nhận tham số (userId) trả về kiểu Task.
        {
            await _context.UserAddresses // Thực thi dòng lệnh nghiệp vụ.
                .Where(ua => ua.UserId == userId && ua.IsDefault) // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(ua => ua.UserId == userId && ua.IsDefault).
                .ExecuteUpdateAsync(s => s.SetProperty(ua => ua.IsDefault, false)); // Thực thi dòng lệnh nghiệp vụ.
        }

        public async Task SaveChangesAsync() // Thực hiện xử lý bất đồng bộ phương thức 'SaveChangesAsync' không tham số trả về kiểu Task.
        {
            await _context.SaveChangesAsync(); // Thực thi dòng lệnh nghiệp vụ.
        }
    }
}

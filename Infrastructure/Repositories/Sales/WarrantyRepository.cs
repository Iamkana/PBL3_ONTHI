using System.Collections.Generic; // Nhập thư viện làm việc với các kiểu danh sách, bộ sưu tập.
using System.Threading.Tasks; // Nhập thư viện hỗ trợ lập trình bất đồng bộ.
using Microsoft.EntityFrameworkCore; // Nhập thư viện Microsoft.EntityFrameworkCore để làm việc với CSDL qua ORM.
using PBL3.Core.Entities; // Nhập namespace chứa các thực thể nghiệp vụ cốt lưu trữ của hệ thống.
using PBL3.Core.Interfaces; // Nhập namespace chứa các giao diện (interface) dùng chung của hệ thống.
using PBL3.Infrastructure.Data; // Nhập namespace chứa DbContext và các cấu hình dữ liệu của Infrastructure.

namespace PBL3.Infrastructure.Repositories // Định nghĩa namespace PBL3.Infrastructure.Repositories quản lý cấu trúc code.
{
    public class WarrantyRepository(HushStoreDbContext dbContext) : IWarrantyRepository // Định nghĩa lớp WarrantyRepository sử dụng Primary Constructor nhận (HushStoreDbContext dbContext) triển khai/kế thừa IWarrantyRepository.
    {
        private readonly HushStoreDbContext _dbContext = // Khai báo trường dữ liệu private readonly và bắt đầu gán giá trị.
            dbContext ?? throw new ArgumentNullException(nameof(dbContext)); // Kiểm tra null và ném ra ngoại lệ ArgumentNullException nếu đối tượng context bị null.

        public async Task<List<Warranty>> GetActiveBySerialIdAsync(int serialId) // Thực hiện xử lý bất đồng bộ phương thức 'GetActiveBySerialIdAsync' nhận tham số (serialId) trả về kiểu Task<List<Warranty>>.
        {
            return await _dbContext.Warranties // Chờ và trả về giá trị của tác vụ bất đồng bộ '_dbContext.Warranties'.
                .Where(w => w.SerialId == serialId && w.Status != 2) // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(w => w.SerialId == serialId && w.Status != 2).
                .OrderByDescending(w => w.EndDate) // Sắp xếp danh sách kết quả theo thứ tự giảm dần.
                .AsNoTracking() // Tắt tính năng theo dõi trạng thái thực thể (AsNoTracking) để tối ưu hiệu năng đọc dữ liệu.
                .ToListAsync(); // Truy vấn bất đồng bộ và chuyển đổi kết quả trả về dạng danh sách List.
        }

        public async Task<Warranty?> GetByIdWithTrackingAsync(int id) // Thực hiện xử lý bất đồng bộ phương thức 'GetByIdWithTrackingAsync' nhận tham số (id) trả về kiểu Task<Warranty?>.
        {
            return await _dbContext.Warranties // Chờ và trả về giá trị của tác vụ bất đồng bộ '_dbContext.Warranties'.
                .FirstOrDefaultAsync(w => w.Id == id); // Lấy phần tử đầu tiên thỏa mãn điều kiện hoặc trả về null nếu không tìm thấy.
        }

        public async Task AddAsync(Warranty warranty) // Thực hiện xử lý bất đồng bộ phương thức 'AddAsync' nhận tham số (warranty) trả về kiểu Task.
        {
            await _dbContext.Warranties.AddAsync(warranty); // Thêm thực thể mới vào CSDL thông qua DbSet tương ứng.
        }

        public async Task AddRangeAsync(IEnumerable<Warranty> warranties) // Thực hiện xử lý bất đồng bộ phương thức 'AddRangeAsync' nhận tham số (warranties) trả về kiểu Task.
        {
            await _dbContext.Warranties.AddRangeAsync(warranties); // Thực thi dòng lệnh nghiệp vụ.
        }

        public async Task SaveChangesAsync() // Thực hiện xử lý bất đồng bộ phương thức 'SaveChangesAsync' không tham số trả về kiểu Task.
        {
            await _dbContext.SaveChangesAsync(); // Thực thi dòng lệnh nghiệp vụ.
        }
    }
}

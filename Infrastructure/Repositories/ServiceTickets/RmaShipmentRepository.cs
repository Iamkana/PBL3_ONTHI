using Microsoft.EntityFrameworkCore; // Nhập thư viện Microsoft.EntityFrameworkCore để làm việc với CSDL qua ORM.
using PBL3.Core.Entities; // Nhập namespace chứa các thực thể nghiệp vụ cốt lưu trữ của hệ thống.
using PBL3.Core.Interfaces; // Nhập namespace chứa các giao diện (interface) dùng chung của hệ thống.
using PBL3.Infrastructure.Data; // Nhập namespace chứa DbContext và các cấu hình dữ liệu của Infrastructure.

namespace PBL3.Infrastructure.Repositories // Định nghĩa namespace PBL3.Infrastructure.Repositories quản lý cấu trúc code.
{
    public class RmaShipmentRepository(HushStoreDbContext dbContext) : IRmaShipmentRepository // Định nghĩa lớp RmaShipmentRepository sử dụng Primary Constructor nhận (HushStoreDbContext dbContext) triển khai/kế thừa IRmaShipmentRepository.
    {
        private readonly HushStoreDbContext _dbContext = // Khai báo trường dữ liệu private readonly và bắt đầu gán giá trị.
            dbContext ?? throw new ArgumentNullException(nameof(dbContext)); // Kiểm tra null và ném ra ngoại lệ ArgumentNullException nếu đối tượng context bị null.

        public async Task<RmaShipment?> GetByTicketIdAsync(int ticketId) // Thực hiện xử lý bất đồng bộ phương thức 'GetByTicketIdAsync' nhận tham số (ticketId) trả về kiểu Task<RmaShipment?>.
        {
            return await _dbContext.RmaShipments // Chờ và trả về giá trị của tác vụ bất đồng bộ '_dbContext.RmaShipments'.
                .AsNoTracking() // Tắt tính năng theo dõi trạng thái thực thể (AsNoTracking) để tối ưu hiệu năng đọc dữ liệu.
                .FirstOrDefaultAsync(r => r.TicketId == ticketId); // Lấy phần tử đầu tiên thỏa mãn điều kiện hoặc trả về null nếu không tìm thấy.
        }

        public async Task<RmaShipment?> GetByIdWithTrackingAsync(int id) // Thực hiện xử lý bất đồng bộ phương thức 'GetByIdWithTrackingAsync' nhận tham số (id) trả về kiểu Task<RmaShipment?>.
        {
            return await _dbContext.RmaShipments // Chờ và trả về giá trị của tác vụ bất đồng bộ '_dbContext.RmaShipments'.
                .FirstOrDefaultAsync(r => r.Id == id); // Lấy phần tử đầu tiên thỏa mãn điều kiện hoặc trả về null nếu không tìm thấy.
        }

        public async Task AddAsync(RmaShipment shipment) // Thực hiện xử lý bất đồng bộ phương thức 'AddAsync' nhận tham số (shipment) trả về kiểu Task.
        {
            await _dbContext.RmaShipments.AddAsync(shipment); // Thêm thực thể mới vào CSDL thông qua DbSet tương ứng.
        }

        public async Task SaveChangesAsync() // Thực hiện xử lý bất đồng bộ phương thức 'SaveChangesAsync' không tham số trả về kiểu Task.
        {
            await _dbContext.SaveChangesAsync(); // Thực thi dòng lệnh nghiệp vụ.
        }
    }
}

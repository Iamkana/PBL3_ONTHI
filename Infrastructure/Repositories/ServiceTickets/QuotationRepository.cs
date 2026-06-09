using Microsoft.EntityFrameworkCore; // Nhập thư viện Microsoft.EntityFrameworkCore để làm việc với CSDL qua ORM.
using PBL3.Core.Entities; // Nhập namespace chứa các thực thể nghiệp vụ cốt lưu trữ của hệ thống.
using PBL3.Core.Interfaces; // Nhập namespace chứa các giao diện (interface) dùng chung của hệ thống.
using PBL3.Infrastructure.Data; // Nhập namespace chứa DbContext và các cấu hình dữ liệu của Infrastructure.

namespace PBL3.Infrastructure.Repositories // Định nghĩa namespace PBL3.Infrastructure.Repositories quản lý cấu trúc code.
{
    public class QuotationRepository(HushStoreDbContext dbContext) : IQuotationRepository // Định nghĩa lớp QuotationRepository sử dụng Primary Constructor nhận (HushStoreDbContext dbContext) triển khai/kế thừa IQuotationRepository.
    {
        private readonly HushStoreDbContext _dbContext = // Khai báo trường dữ liệu private readonly và bắt đầu gán giá trị.
            dbContext ?? throw new ArgumentNullException(nameof(dbContext)); // Kiểm tra null và ném ra ngoại lệ ArgumentNullException nếu đối tượng context bị null.

        public async Task<Quotation?> GetByIdWithItemsAsync(int id) // Thực hiện xử lý bất đồng bộ phương thức 'GetByIdWithItemsAsync' nhận tham số (id) trả về kiểu Task<Quotation?>.
        {
            return await _dbContext.Quotations // Chờ và trả về giá trị của tác vụ bất đồng bộ '_dbContext.Quotations'.
                .Include(q => q.Items) // Nạp kèm (Eager load) dữ liệu quan hệ liên quan: .Include(q => q.Items).
                .AsNoTracking() // Tắt tính năng theo dõi trạng thái thực thể (AsNoTracking) để tối ưu hiệu năng đọc dữ liệu.
                .FirstOrDefaultAsync(q => q.Id == id); // Lấy phần tử đầu tiên thỏa mãn điều kiện hoặc trả về null nếu không tìm thấy.
        }

        public async Task<Quotation?> GetByIdWithTrackingAsync(int id) // Thực hiện xử lý bất đồng bộ phương thức 'GetByIdWithTrackingAsync' nhận tham số (id) trả về kiểu Task<Quotation?>.
        {
            return await _dbContext.Quotations // Chờ và trả về giá trị của tác vụ bất đồng bộ '_dbContext.Quotations'.
                .Include(q => q.Items) // Nạp kèm (Eager load) dữ liệu quan hệ liên quan: .Include(q => q.Items).
                .FirstOrDefaultAsync(q => q.Id == id); // Lấy phần tử đầu tiên thỏa mãn điều kiện hoặc trả về null nếu không tìm thấy.
        }

        public async Task<List<Quotation>> GetByTicketIdAsync(int ticketId) // Thực hiện xử lý bất đồng bộ phương thức 'GetByTicketIdAsync' nhận tham số (ticketId) trả về kiểu Task<List<Quotation>>.
        {
            return await _dbContext.Quotations // Chờ và trả về giá trị của tác vụ bất đồng bộ '_dbContext.Quotations'.
                .Where(q => q.TicketId == ticketId) // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(q => q.TicketId == ticketId).
                .Include(q => q.Items) // Nạp kèm (Eager load) dữ liệu quan hệ liên quan: .Include(q => q.Items).
                .AsNoTracking() // Tắt tính năng theo dõi trạng thái thực thể (AsNoTracking) để tối ưu hiệu năng đọc dữ liệu.
                .ToListAsync(); // Truy vấn bất đồng bộ và chuyển đổi kết quả trả về dạng danh sách List.
        }

        public async Task<bool> HasAcceptedQuotationAsync(int ticketId) // Thực hiện xử lý bất đồng bộ phương thức 'HasAcceptedQuotationAsync' nhận tham số (ticketId) trả về kiểu Task<bool>.
        {
            return await _dbContext.Quotations // Chờ và trả về giá trị của tác vụ bất đồng bộ '_dbContext.Quotations'.
                .Where(q => q.TicketId == ticketId && q.Status == 1) // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(q => q.TicketId == ticketId && q.Status == 1).
                .AnyAsync(); // Kiểm tra xem có bất kỳ bản ghi nào khớp với điều kiện hay không.
        }

        public async Task AddAsync(Quotation quotation) // Thực hiện xử lý bất đồng bộ phương thức 'AddAsync' nhận tham số (quotation) trả về kiểu Task.
        {
            await _dbContext.Quotations.AddAsync(quotation); // Thêm thực thể mới vào CSDL thông qua DbSet tương ứng.
        }

        public async Task SaveChangesAsync() // Thực hiện xử lý bất đồng bộ phương thức 'SaveChangesAsync' không tham số trả về kiểu Task.
        {
            await _dbContext.SaveChangesAsync(); // Thực thi dòng lệnh nghiệp vụ.
        }
    }
}

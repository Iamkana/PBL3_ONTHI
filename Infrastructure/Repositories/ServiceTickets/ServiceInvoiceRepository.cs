using Microsoft.EntityFrameworkCore; // Nhập thư viện Microsoft.EntityFrameworkCore để làm việc với CSDL qua ORM.
using PBL3.Core.Entities; // Nhập namespace chứa các thực thể nghiệp vụ cốt lưu trữ của hệ thống.
using PBL3.Core.Interfaces; // Nhập namespace chứa các giao diện (interface) dùng chung của hệ thống.
using PBL3.Infrastructure.Data; // Nhập namespace chứa DbContext và các cấu hình dữ liệu của Infrastructure.

namespace PBL3.Infrastructure.Repositories // Định nghĩa namespace PBL3.Infrastructure.Repositories quản lý cấu trúc code.
{
    public class ServiceInvoiceRepository(HushStoreDbContext dbContext) : IServiceInvoiceRepository // Định nghĩa lớp ServiceInvoiceRepository sử dụng Primary Constructor nhận (HushStoreDbContext dbContext) triển khai/kế thừa IServiceInvoiceRepository.
    {
        private readonly HushStoreDbContext _dbContext = // Khai báo trường dữ liệu private readonly và bắt đầu gán giá trị.
            dbContext ?? throw new ArgumentNullException(nameof(dbContext)); // Kiểm tra null và ném ra ngoại lệ ArgumentNullException nếu đối tượng context bị null.

        public async Task<(List<ServiceInvoice> Items, int TotalCount)> GetPagedListAsync( // Thực hiện xử lý bất đồng bộ phương thức 'Task<' nhận tham số (Items, TotalCount) trả về kiểu .
            string? keyword, // Thực thi dòng lệnh nghiệp vụ.
            byte? paymentStatus, // Thực thi dòng lệnh nghiệp vụ.
            DateTime? fromDate, // Thực thi dòng lệnh nghiệp vụ.
            DateTime? toDate, // Thực thi dòng lệnh nghiệp vụ.
            int pageNumber, // Thực thi dòng lệnh nghiệp vụ.
            int pageSize, // Thực thi dòng lệnh nghiệp vụ.
            string? sortBy, // Thực thi dòng lệnh nghiệp vụ.
            bool sortDescending) // Thực thi dòng lệnh nghiệp vụ.
        {
            var query = _dbContext.ServiceInvoices.AsNoTracking(); // Tắt tính năng theo dõi trạng thái thực thể (AsNoTracking) để tối ưu hiệu năng đọc dữ liệu.

            if (!string.IsNullOrWhiteSpace(keyword)) // Kiểm tra điều kiện: '!string.IsNullOrWhiteSpace(keyword'.
                query = query.Where(i => i.InvoiceCode.Contains(keyword)); // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(i => i.InvoiceCode.Contains(keyword));.

            if (paymentStatus.HasValue) // Kiểm tra điều kiện: 'paymentStatus.HasValue'.
                query = query.Where(i => i.PaymentStatus == paymentStatus.Value); // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(i => i.PaymentStatus == paymentStatus.Value);.

            if (fromDate.HasValue) // Kiểm tra điều kiện: 'fromDate.HasValue'.
                query = query.Where(i => i.IssuedDate >= fromDate.Value.Date); // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(i => i.IssuedDate >= fromDate.Value.Date);.

            if (toDate.HasValue) // Kiểm tra điều kiện: 'toDate.HasValue'.
                query = query.Where(i => i.IssuedDate < toDate.Value.Date.AddDays(1)); // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(i => i.IssuedDate < toDate.Value.Date.AddDays(1));.

            var totalCount = await query.CountAsync(); // Tính toán tổng số lượng bản ghi thỏa mãn điều kiện bất đồng bộ.

            // Sorting
            query = sortBy switch // Thực hiện gán giá trị của biểu thức 'sortBy switch' cho biến 'query'.
            {
                "code" => sortDescending ? query.OrderByDescending(i => i.InvoiceCode) : query.OrderBy(i => i.InvoiceCode), // Sắp xếp danh sách kết quả theo thứ tự giảm dần.
                "issued" => sortDescending ? query.OrderByDescending(i => i.IssuedDate) : query.OrderBy(i => i.IssuedDate), // Sắp xếp danh sách kết quả theo thứ tự giảm dần.
                _ => query.OrderByDescending(i => i.IssuedDate) // Sắp xếp danh sách kết quả theo thứ tự giảm dần.
            };

            var items = await query // Chờ tác vụ bất đồng bộ hoàn thành và gán kết quả cho biến 'items'.
                .Skip((pageNumber - 1) * pageSize) // Thực thi dòng lệnh nghiệp vụ.
                .Take(pageSize) // Thực thi dòng lệnh nghiệp vụ.
                .ToListAsync(); // Truy vấn bất đồng bộ và chuyển đổi kết quả trả về dạng danh sách List.

            return (items, totalCount); // Trả về giá trị của biểu thức '(items, totalCount)'.
        }

        public async Task<ServiceInvoice?> GetByIdWithDetailsAsync(int id) // Thực hiện xử lý bất đồng bộ phương thức 'GetByIdWithDetailsAsync' nhận tham số (id) trả về kiểu Task<ServiceInvoice?>.
        {
            return await _dbContext.ServiceInvoices // Chờ và trả về giá trị của tác vụ bất đồng bộ '_dbContext.ServiceInvoices'.
                .Include(i => i.Ticket) // Nạp kèm (Eager load) dữ liệu quan hệ liên quan: .Include(i => i.Ticket).
                .Include(i => i.Quotation) // Nạp kèm (Eager load) dữ liệu quan hệ liên quan: .Include(i => i.Quotation).
                .Include(i => i.Items) // Nạp kèm (Eager load) dữ liệu quan hệ liên quan: .Include(i => i.Items).
                .AsNoTracking() // Tắt tính năng theo dõi trạng thái thực thể (AsNoTracking) để tối ưu hiệu năng đọc dữ liệu.
                .FirstOrDefaultAsync(i => i.Id == id); // Lấy phần tử đầu tiên thỏa mãn điều kiện hoặc trả về null nếu không tìm thấy.
        }

        public async Task<ServiceInvoice?> GetByIdWithTrackingAsync(int id) // Thực hiện xử lý bất đồng bộ phương thức 'GetByIdWithTrackingAsync' nhận tham số (id) trả về kiểu Task<ServiceInvoice?>.
        {
            return await _dbContext.ServiceInvoices // Chờ và trả về giá trị của tác vụ bất đồng bộ '_dbContext.ServiceInvoices'.
                .FirstOrDefaultAsync(i => i.Id == id); // Lấy phần tử đầu tiên thỏa mãn điều kiện hoặc trả về null nếu không tìm thấy.
        }

        public async Task<ServiceInvoice?> GetByTicketIdAsync(int ticketId) // Thực hiện xử lý bất đồng bộ phương thức 'GetByTicketIdAsync' nhận tham số (ticketId) trả về kiểu Task<ServiceInvoice?>.
        {
            return await _dbContext.ServiceInvoices // Chờ và trả về giá trị của tác vụ bất đồng bộ '_dbContext.ServiceInvoices'.
                .AsNoTracking() // Tắt tính năng theo dõi trạng thái thực thể (AsNoTracking) để tối ưu hiệu năng đọc dữ liệu.
                .FirstOrDefaultAsync(i => i.TicketId == ticketId); // Lấy phần tử đầu tiên thỏa mãn điều kiện hoặc trả về null nếu không tìm thấy.
        }

        public async Task<string?> GetLastInvoiceCodeByDateAsync(string datePrefix) // Thực hiện xử lý bất đồng bộ phương thức 'GetLastInvoiceCodeByDateAsync' nhận tham số (datePrefix) trả về kiểu Task<string?>.
        {
            return await _dbContext.ServiceInvoices // Chờ và trả về giá trị của tác vụ bất đồng bộ '_dbContext.ServiceInvoices'.
                .Where(i => i.InvoiceCode.StartsWith(datePrefix)) // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(i => i.InvoiceCode.StartsWith(datePrefix)).
                .OrderByDescending(i => i.InvoiceCode) // Sắp xếp danh sách kết quả theo thứ tự giảm dần.
                .Select(i => i.InvoiceCode) // Thực thi dòng lệnh nghiệp vụ.
                .FirstOrDefaultAsync(); // Lấy phần tử đầu tiên thỏa mãn điều kiện hoặc trả về null nếu không tìm thấy.
        }

        public async Task<bool> InvoiceExistsForTicketAsync(int ticketId) // Thực hiện xử lý bất đồng bộ phương thức 'InvoiceExistsForTicketAsync' nhận tham số (ticketId) trả về kiểu Task<bool>.
        {
            return await _dbContext.ServiceInvoices // Chờ và trả về giá trị của tác vụ bất đồng bộ '_dbContext.ServiceInvoices'.
                .AnyAsync(i => i.TicketId == ticketId); // Kiểm tra xem có bất kỳ bản ghi nào khớp với điều kiện hay không.
        }

        public async Task AddAsync(ServiceInvoice invoice) // Thực hiện xử lý bất đồng bộ phương thức 'AddAsync' nhận tham số (invoice) trả về kiểu Task.
        {
            await _dbContext.ServiceInvoices.AddAsync(invoice); // Thêm thực thể mới vào CSDL thông qua DbSet tương ứng.
        }

        public async Task SaveChangesAsync() // Thực hiện xử lý bất đồng bộ phương thức 'SaveChangesAsync' không tham số trả về kiểu Task.
        {
            await _dbContext.SaveChangesAsync(); // Thực thi dòng lệnh nghiệp vụ.
        }
    }
}

using Microsoft.EntityFrameworkCore; // Nhập thư viện Microsoft.EntityFrameworkCore để làm việc với CSDL qua ORM.
using PBL3.Core.Entities; // Nhập namespace chứa các thực thể nghiệp vụ cốt lưu trữ của hệ thống.
using PBL3.Core.Interfaces; // Nhập namespace chứa các giao diện (interface) dùng chung của hệ thống.
using PBL3.Infrastructure.Data; // Nhập namespace chứa DbContext và các cấu hình dữ liệu của Infrastructure.

namespace PBL3.Infrastructure.Repositories // Định nghĩa namespace PBL3.Infrastructure.Repositories quản lý cấu trúc code.
{
    public class ServiceTicketRepository(HushStoreDbContext dbContext) : IServiceTicketRepository // Định nghĩa lớp ServiceTicketRepository sử dụng Primary Constructor nhận (HushStoreDbContext dbContext) triển khai/kế thừa IServiceTicketRepository.
    {
        private readonly HushStoreDbContext _dbContext = // Khai báo trường dữ liệu private readonly và bắt đầu gán giá trị.
            dbContext ?? throw new ArgumentNullException(nameof(dbContext)); // Kiểm tra null và ném ra ngoại lệ ArgumentNullException nếu đối tượng context bị null.

        public async Task<(List<ServiceTicket> Items, int TotalCount)> GetPagedListAsync( // Thực hiện xử lý bất đồng bộ phương thức 'Task<' nhận tham số (Items, TotalCount) trả về kiểu .
            string? keyword, // Thực thi dòng lệnh nghiệp vụ.
            byte? status, // Thực thi dòng lệnh nghiệp vụ.
            byte? resolutionType, // Thực thi dòng lệnh nghiệp vụ.
            Guid? assignedEmployeeId, // Thực thi dòng lệnh nghiệp vụ.
            Guid? customerId, // Thực thi dòng lệnh nghiệp vụ.
            DateTime? fromDate, // Thực thi dòng lệnh nghiệp vụ.
            DateTime? toDate, // Thực thi dòng lệnh nghiệp vụ.
            int pageNumber, // Thực thi dòng lệnh nghiệp vụ.
            int pageSize, // Thực thi dòng lệnh nghiệp vụ.
            string? sortBy, // Thực thi dòng lệnh nghiệp vụ.
            bool sortDescending) // Thực thi dòng lệnh nghiệp vụ.
        {
            var query = _dbContext.ServiceTickets.AsNoTracking(); // Tắt tính năng theo dõi trạng thái thực thể (AsNoTracking) để tối ưu hiệu năng đọc dữ liệu.

            if (!string.IsNullOrWhiteSpace(keyword)) // Kiểm tra điều kiện: '!string.IsNullOrWhiteSpace(keyword'.
            {
                query = query.Where(t => t.TicketCode.Contains(keyword)); // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(t => t.TicketCode.Contains(keyword));.
            }

            if (status.HasValue) // Kiểm tra điều kiện: 'status.HasValue'.
            {
                query = query.Where(t => t.Status == status.Value); // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(t => t.Status == status.Value);.
            }

            if (resolutionType.HasValue) // Kiểm tra điều kiện: 'resolutionType.HasValue'.
            {
                query = query.Where(t => t.ResolutionType == resolutionType.Value); // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(t => t.ResolutionType == resolutionType.Value);.
            }

            if (assignedEmployeeId.HasValue) // Kiểm tra điều kiện: 'assignedEmployeeId.HasValue'.
            {
                query = query.Where(t => t.AssignedEmployeeId == assignedEmployeeId.Value); // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(t => t.AssignedEmployeeId == assignedEmployeeId.Value);.
            }

            if (customerId.HasValue) // Kiểm tra điều kiện: 'customerId.HasValue'.
            {
                query = query.Where(t => t.CustomerId == customerId.Value); // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(t => t.CustomerId == customerId.Value);.
            }

            if (fromDate.HasValue) // Kiểm tra điều kiện: 'fromDate.HasValue'.
            {
                query = query.Where(t => t.IntakeDate >= fromDate.Value); // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(t => t.IntakeDate >= fromDate.Value);.
            }

            if (toDate.HasValue) // Kiểm tra điều kiện: 'toDate.HasValue'.
            {
                query = query.Where(t => t.IntakeDate <= toDate.Value); // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(t => t.IntakeDate <= toDate.Value);.
            }

            var totalCount = await query.CountAsync(); // Tính toán tổng số lượng bản ghi thỏa mãn điều kiện bất đồng bộ.

            // Sorting
            query = sortBy switch // Thực hiện gán giá trị của biểu thức 'sortBy switch' cho biến 'query'.
            {
                "code" => sortDescending ? query.OrderByDescending(t => t.TicketCode) : query.OrderBy(t => t.TicketCode), // Sắp xếp danh sách kết quả theo thứ tự giảm dần.
                "status" => sortDescending ? query.OrderByDescending(t => t.Status) : query.OrderBy(t => t.Status), // Sắp xếp danh sách kết quả theo thứ tự giảm dần.
                "intakeDate" => sortDescending ? query.OrderByDescending(t => t.IntakeDate) : query.OrderBy(t => t.IntakeDate), // Sắp xếp danh sách kết quả theo thứ tự giảm dần.
                _ => query.OrderByDescending(t => t.CreatedDate) // Sắp xếp danh sách kết quả theo thứ tự giảm dần.
            };

            var items = await query // Chờ tác vụ bất đồng bộ hoàn thành và gán kết quả cho biến 'items'.
                .Include(t => t.Serial) // Nạp kèm (Eager load) dữ liệu quan hệ liên quan: .Include(t => t.Serial).
                    .ThenInclude(s => s.Variant) // Thực thi dòng lệnh nghiệp vụ.
                        .ThenInclude(v => v.Product) // Thực thi dòng lệnh nghiệp vụ.
                .Skip((pageNumber - 1) * pageSize) // Thực thi dòng lệnh nghiệp vụ.
                .Take(pageSize) // Thực thi dòng lệnh nghiệp vụ.
                .ToListAsync(); // Truy vấn bất đồng bộ và chuyển đổi kết quả trả về dạng danh sách List.

            return (items, totalCount); // Trả về giá trị của biểu thức '(items, totalCount)'.
        }

        public async Task<ServiceTicket?> GetByIdAsync(int id) // Thực hiện xử lý bất đồng bộ phương thức 'GetByIdAsync' nhận tham số (id) trả về kiểu Task<ServiceTicket?>.
        {
            return await _dbContext.ServiceTickets // Chờ và trả về giá trị của tác vụ bất đồng bộ '_dbContext.ServiceTickets'.
                .AsNoTracking() // Tắt tính năng theo dõi trạng thái thực thể (AsNoTracking) để tối ưu hiệu năng đọc dữ liệu.
                .FirstOrDefaultAsync(t => t.Id == id); // Lấy phần tử đầu tiên thỏa mãn điều kiện hoặc trả về null nếu không tìm thấy.
        }

        public async Task<ServiceTicket?> GetByIdWithDetailsAsync(int id) // Thực hiện xử lý bất đồng bộ phương thức 'GetByIdWithDetailsAsync' nhận tham số (id) trả về kiểu Task<ServiceTicket?>.
        {
            return await _dbContext.ServiceTickets // Chờ và trả về giá trị của tác vụ bất đồng bộ '_dbContext.ServiceTickets'.
                .Include(t => t.Serial) // Nạp kèm (Eager load) dữ liệu quan hệ liên quan: .Include(t => t.Serial).
                    .ThenInclude(s => s.Variant) // Thực thi dòng lệnh nghiệp vụ.
                        .ThenInclude(v => v.Product) // Thực thi dòng lệnh nghiệp vụ.
                .Include(t => t.OriginalOrder) // Nạp kèm (Eager load) dữ liệu quan hệ liên quan: .Include(t => t.OriginalOrder).
                .Include(t => t.Customer) // Nạp kèm (Eager load) dữ liệu quan hệ liên quan: .Include(t => t.Customer).
                    .ThenInclude(c => c!.Profile) // Thực thi dòng lệnh nghiệp vụ.
                .Include(t => t.StatusHistory.OrderByDescending(h => h.ChangedAt)) // Nạp kèm (Eager load) dữ liệu quan hệ liên quan: .Include(t => t.StatusHistory.OrderByDescending(h => h.ChangedAt)).
                .Include(t => t.Quotations) // Nạp kèm (Eager load) dữ liệu quan hệ liên quan: .Include(t => t.Quotations).
                    .ThenInclude(q => q.Items) // Thực thi dòng lệnh nghiệp vụ.
                .Include(t => t.RmaShipment) // Nạp kèm (Eager load) dữ liệu quan hệ liên quan: .Include(t => t.RmaShipment).
                .Include(t => t.Invoice) // Nạp kèm (Eager load) dữ liệu quan hệ liên quan: .Include(t => t.Invoice).
                    .ThenInclude(i => i.Items) // Thực thi dòng lệnh nghiệp vụ.
                .Include(t => t.ReplacementSerial) // Nạp kèm (Eager load) dữ liệu quan hệ liên quan: .Include(t => t.ReplacementSerial).
                .AsNoTracking() // Tắt tính năng theo dõi trạng thái thực thể (AsNoTracking) để tối ưu hiệu năng đọc dữ liệu.
                .FirstOrDefaultAsync(t => t.Id == id); // Lấy phần tử đầu tiên thỏa mãn điều kiện hoặc trả về null nếu không tìm thấy.
        }

        public async Task<ServiceTicket?> GetByIdWithTrackingAsync(int id) // Thực hiện xử lý bất đồng bộ phương thức 'GetByIdWithTrackingAsync' nhận tham số (id) trả về kiểu Task<ServiceTicket?>.
        {
            return await _dbContext.ServiceTickets // Chờ và trả về giá trị của tác vụ bất đồng bộ '_dbContext.ServiceTickets'.
                .Include(t => t.Serial) // Nạp kèm (Eager load) dữ liệu quan hệ liên quan: .Include(t => t.Serial).
                    .ThenInclude(s => s.Variant) // Thực thi dòng lệnh nghiệp vụ.
                .Include(t => t.ReplacementSerial) // Nạp kèm (Eager load) dữ liệu quan hệ liên quan: .Include(t => t.ReplacementSerial).
                .Include(t => t.RmaShipment) // Nạp kèm (Eager load) dữ liệu quan hệ liên quan: .Include(t => t.RmaShipment).
                .Include(t => t.Quotations) // Nạp kèm (Eager load) dữ liệu quan hệ liên quan: .Include(t => t.Quotations).
                .FirstOrDefaultAsync(t => t.Id == id); // Lấy phần tử đầu tiên thỏa mãn điều kiện hoặc trả về null nếu không tìm thấy.
        }

        public async Task<string?> GetLastTicketCodeByDateAsync(string datePrefix) // Thực hiện xử lý bất đồng bộ phương thức 'GetLastTicketCodeByDateAsync' nhận tham số (datePrefix) trả về kiểu Task<string?>.
        {
            return await _dbContext.ServiceTickets // Chờ và trả về giá trị của tác vụ bất đồng bộ '_dbContext.ServiceTickets'.
                .Where(t => t.TicketCode.StartsWith(datePrefix)) // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(t => t.TicketCode.StartsWith(datePrefix)).
                .OrderByDescending(t => t.TicketCode) // Sắp xếp danh sách kết quả theo thứ tự giảm dần.
                .Select(t => t.TicketCode) // Thực thi dòng lệnh nghiệp vụ.
                .FirstOrDefaultAsync(); // Lấy phần tử đầu tiên thỏa mãn điều kiện hoặc trả về null nếu không tìm thấy.
        }

        public async Task<bool> HasOpenTicketForSerialAsync(int serialId) // Thực hiện xử lý bất đồng bộ phương thức 'HasOpenTicketForSerialAsync' nhận tham số (serialId) trả về kiểu Task<bool>.
        {
            // Terminal states: 3 = QuoteRejected, 8 = Swapped, 9 = Completed, 10 = Cancelled
            var terminalStates = new[] { (byte)3, (byte)8, (byte)9, (byte)10 }; // Thực hiện gán giá trị của biểu thức 'new[] { (byte)3, (byte)8, (byte)9, (byte)10 }' cho biến 'terminalStates'.
            return await _dbContext.ServiceTickets // Chờ và trả về giá trị của tác vụ bất đồng bộ '_dbContext.ServiceTickets'.
                .Where(t => t.SerialId == serialId && !terminalStates.Contains(t.Status)) // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(t => t.SerialId == serialId && !terminalStates.Contains(t.Status)).
                .AnyAsync(); // Kiểm tra xem có bất kỳ bản ghi nào khớp với điều kiện hay không.
        }

        public async Task<(List<ServiceTicket> Items, int TotalCount)> GetTicketsByOrderUserIdAsync( // Thực hiện xử lý bất đồng bộ phương thức 'Task<' nhận tham số (Items, TotalCount) trả về kiểu .
            Guid userId, // Thực thi dòng lệnh nghiệp vụ.
            string? keyword, // Thực thi dòng lệnh nghiệp vụ.
            byte? status, // Thực thi dòng lệnh nghiệp vụ.
            int pageNumber, // Thực thi dòng lệnh nghiệp vụ.
            int pageSize, // Thực thi dòng lệnh nghiệp vụ.
            string? sortBy, // Thực thi dòng lệnh nghiệp vụ.
            bool sortDescending) // Thực thi dòng lệnh nghiệp vụ.
        {
            var query = _dbContext.ServiceTickets // Thực hiện gán giá trị của biểu thức '_dbContext.ServiceTickets' cho biến 'query'.
                .Where(t => t.OriginalOrder.UserId == userId) // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(t => t.OriginalOrder.UserId == userId).
                .AsNoTracking(); // Tắt tính năng theo dõi trạng thái thực thể (AsNoTracking) để tối ưu hiệu năng đọc dữ liệu.

            if (!string.IsNullOrWhiteSpace(keyword)) // Kiểm tra điều kiện: '!string.IsNullOrWhiteSpace(keyword'.
            {
                query = query.Where(t => t.TicketCode.Contains(keyword)); // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(t => t.TicketCode.Contains(keyword));.
            }

            if (status.HasValue) // Kiểm tra điều kiện: 'status.HasValue'.
            {
                query = query.Where(t => t.Status == status.Value); // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(t => t.Status == status.Value);.
            }

            var totalCount = await query.CountAsync(); // Tính toán tổng số lượng bản ghi thỏa mãn điều kiện bất đồng bộ.

            // Sorting
            query = sortBy switch // Thực hiện gán giá trị của biểu thức 'sortBy switch' cho biến 'query'.
            {
                "code" => sortDescending ? query.OrderByDescending(t => t.TicketCode) : query.OrderBy(t => t.TicketCode), // Sắp xếp danh sách kết quả theo thứ tự giảm dần.
                "status" => sortDescending ? query.OrderByDescending(t => t.Status) : query.OrderBy(t => t.Status), // Sắp xếp danh sách kết quả theo thứ tự giảm dần.
                _ => query.OrderByDescending(t => t.CreatedDate) // Sắp xếp danh sách kết quả theo thứ tự giảm dần.
            };

            var items = await query // Chờ tác vụ bất đồng bộ hoàn thành và gán kết quả cho biến 'items'.
                .Include(t => t.Serial) // Nạp kèm (Eager load) dữ liệu quan hệ liên quan: .Include(t => t.Serial).
                    .ThenInclude(s => s.Variant) // Thực thi dòng lệnh nghiệp vụ.
                        .ThenInclude(v => v.Product) // Thực thi dòng lệnh nghiệp vụ.
                .Skip((pageNumber - 1) * pageSize) // Thực thi dòng lệnh nghiệp vụ.
                .Take(pageSize) // Thực thi dòng lệnh nghiệp vụ.
                .ToListAsync(); // Truy vấn bất đồng bộ và chuyển đổi kết quả trả về dạng danh sách List.

            return (items, totalCount); // Trả về giá trị của biểu thức '(items, totalCount)'.
        }

        public async Task AddAsync(ServiceTicket ticket) // Thực hiện xử lý bất đồng bộ phương thức 'AddAsync' nhận tham số (ticket) trả về kiểu Task.
        {
            await _dbContext.ServiceTickets.AddAsync(ticket); // Thêm thực thể mới vào CSDL thông qua DbSet tương ứng.
        }

        public async Task AddStatusHistoryAsync(ServiceTicketStatusHistory history) // Thực hiện xử lý bất đồng bộ phương thức 'AddStatusHistoryAsync' nhận tham số (history) trả về kiểu Task.
        {
            await _dbContext.ServiceTicketStatusHistories.AddAsync(history); // Thêm thực thể mới vào CSDL thông qua DbSet tương ứng.
        }

        public async Task SaveChangesAsync() // Thực hiện xử lý bất đồng bộ phương thức 'SaveChangesAsync' không tham số trả về kiểu Task.
        {
            await _dbContext.SaveChangesAsync(); // Thực thi dòng lệnh nghiệp vụ.
        }
    }
}

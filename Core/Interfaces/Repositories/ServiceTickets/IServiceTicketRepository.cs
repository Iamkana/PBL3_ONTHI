using PBL3.Core.Entities; // Nhập namespace chứa các thực thể nghiệp vụ cốt lõi của hệ thống.

namespace PBL3.Core.Interfaces // Định nghĩa namespace PBL3.Core.Interfaces để quản lý thực thể/giao diện.
{
    /// <summary>
    /// Repository interface cho ServiceTicket.
    /// </summary>
    public interface IServiceTicketRepository // Định nghĩa giao diện (interface) IServiceTicketRepository quy định hợp đồng cho tầng dữ liệu.
    {
        /// <summary>
        /// Lấy danh sách phiếu sửa chữa có phân trang, bộ lọc đa tiêu chí.
        /// </summary>
        Task<(List<ServiceTicket> Items, int TotalCount)> GetPagedListAsync( // Khai báo thành phần cấu trúc nghiệp vụ.
            string? keyword, // Khai báo thành phần cấu trúc nghiệp vụ.
            byte? status, // Khai báo thành phần cấu trúc nghiệp vụ.
            byte? resolutionType, // Khai báo thành phần cấu trúc nghiệp vụ.
            Guid? assignedEmployeeId, // Khai báo thành phần cấu trúc nghiệp vụ.
            Guid? customerId, // Khai báo thành phần cấu trúc nghiệp vụ.
            DateTime? fromDate, // Khai báo thành phần cấu trúc nghiệp vụ.
            DateTime? toDate, // Khai báo thành phần cấu trúc nghiệp vụ.
            int pageNumber, // Khai báo thành phần cấu trúc nghiệp vụ.
            int pageSize, // Khai báo thành phần cấu trúc nghiệp vụ.
            string? sortBy, // Khai báo thành phần cấu trúc nghiệp vụ.
            bool sortDescending); // Khai báo thành phần cấu trúc nghiệp vụ.

        /// <summary>
        /// Lấy chi tiết phiếu theo Id, không tracking.
        /// </summary>
        Task<ServiceTicket?> GetByIdAsync(int id); // Định nghĩa phương thức bất đồng bộ 'GetByIdAsync' với tham số (id) trả về kiểu Task<ServiceTicket?>.

        /// <summary>
        /// Lấy chi tiết phiếu theo Id, bao gồm các related entities (Serial, Order, Customer, StatusHistory, Quotations, RmaShipment, Invoice, ReplacementSerial).
        /// Không tracking (read-only).
        /// </summary>
        Task<ServiceTicket?> GetByIdWithDetailsAsync(int id); // Định nghĩa phương thức bất đồng bộ 'GetByIdWithDetailsAsync' với tham số (id) trả về kiểu Task<ServiceTicket?>.

        /// <summary>
        /// Lấy phiếu theo Id với tracking, bao gồm ReplacementSerial, RmaShipment, Quotations.
        /// Dùng cho update operations.
        /// </summary>
        Task<ServiceTicket?> GetByIdWithTrackingAsync(int id); // Định nghĩa phương thức bất đồng bộ 'GetByIdWithTrackingAsync' với tham số (id) trả về kiểu Task<ServiceTicket?>.

        /// <summary>
        /// Lấy mã phiếu cuối cùng của một ngày (để sinh mã tự động ST-yyyyMMdd-NNN).
        /// </summary>
        Task<string?> GetLastTicketCodeByDateAsync(string datePrefix); // Định nghĩa phương thức bất đồng bộ 'GetLastTicketCodeByDateAsync' với tham số (datePrefix) trả về kiểu Task<string?>.

        /// <summary>
        /// Kiểm tra xem một serial đã có phiếu mở không (status != terminal).
        /// </summary>
        Task<bool> HasOpenTicketForSerialAsync(int serialId); // Định nghĩa phương thức bất đồng bộ 'HasOpenTicketForSerialAsync' với tham số (serialId) trả về kiểu Task<bool>.

        /// <summary>
        /// Lấy danh sách phiếu của một customer (theo Order.UserId), với bộ lọc tùy chọn.
        /// </summary>
        Task<(List<ServiceTicket> Items, int TotalCount)> GetTicketsByOrderUserIdAsync( // Khai báo thành phần cấu trúc nghiệp vụ.
            Guid userId, // Khai báo thành phần cấu trúc nghiệp vụ.
            string? keyword, // Khai báo thành phần cấu trúc nghiệp vụ.
            byte? status, // Khai báo thành phần cấu trúc nghiệp vụ.
            int pageNumber, // Khai báo thành phần cấu trúc nghiệp vụ.
            int pageSize, // Khai báo thành phần cấu trúc nghiệp vụ.
            string? sortBy, // Khai báo thành phần cấu trúc nghiệp vụ.
            bool sortDescending); // Khai báo thành phần cấu trúc nghiệp vụ.

        Task AddAsync(ServiceTicket ticket); // Định nghĩa phương thức bất đồng bộ 'AddAsync' với tham số (ticket) trả về kiểu Task.
        Task AddStatusHistoryAsync(ServiceTicketStatusHistory history); // Định nghĩa phương thức bất đồng bộ 'AddStatusHistoryAsync' với tham số (history) trả về kiểu Task.
        Task SaveChangesAsync(); // Định nghĩa phương thức bất đồng bộ 'SaveChangesAsync' không tham số trả về kiểu Task.
    }
}

using PBL3.Core.Entities; // Nhập namespace chứa các thực thể nghiệp vụ cốt lõi của hệ thống.

namespace PBL3.Core.Interfaces // Định nghĩa namespace PBL3.Core.Interfaces để quản lý thực thể/giao diện.
{
    /// <summary>
    /// Repository interface cho ServiceInvoice.
    /// </summary>
    public interface IServiceInvoiceRepository // Định nghĩa giao diện (interface) IServiceInvoiceRepository quy định hợp đồng cho tầng dữ liệu.
    {
        /// <summary>
        /// Lấy danh sách hóa đơn dịch vụ có phân trang.
        /// </summary>
        Task<(List<ServiceInvoice> Items, int TotalCount)> GetPagedListAsync( // Khai báo thành phần cấu trúc nghiệp vụ.
            string? keyword, // Khai báo thành phần cấu trúc nghiệp vụ.
            byte? paymentStatus, // Khai báo thành phần cấu trúc nghiệp vụ.
            DateTime? fromDate, // Khai báo thành phần cấu trúc nghiệp vụ.
            DateTime? toDate, // Khai báo thành phần cấu trúc nghiệp vụ.
            int pageNumber, // Khai báo thành phần cấu trúc nghiệp vụ.
            int pageSize, // Khai báo thành phần cấu trúc nghiệp vụ.
            string? sortBy, // Khai báo thành phần cấu trúc nghiệp vụ.
            bool sortDescending); // Khai báo thành phần cấu trúc nghiệp vụ.

        /// <summary>
        /// Lấy hóa đơn theo Id, bao gồm Ticket, Quotation, Items, không tracking.
        /// </summary>
        Task<ServiceInvoice?> GetByIdWithDetailsAsync(int id); // Định nghĩa phương thức bất đồng bộ 'GetByIdWithDetailsAsync' với tham số (id) trả về kiểu Task<ServiceInvoice?>.

        /// <summary>
        /// Lấy hóa đơn của một phiếu sửa chữa (1:1), không tracking.
        /// </summary>
        Task<ServiceInvoice?> GetByTicketIdAsync(int ticketId); // Định nghĩa phương thức bất đồng bộ 'GetByTicketIdAsync' với tham số (ticketId) trả về kiểu Task<ServiceInvoice?>.

        /// <summary>
        /// Lấy mã hóa đơn cuối cùng của một ngày (để sinh mã tự động SRV-yyyyMMdd-NNN).
        /// </summary>
        Task<string?> GetLastInvoiceCodeByDateAsync(string datePrefix); // Định nghĩa phương thức bất đồng bộ 'GetLastInvoiceCodeByDateAsync' với tham số (datePrefix) trả về kiểu Task<string?>.

        /// <summary>
        /// Lấy hóa đơn theo Id, với tracking để update.
        /// </summary>
        Task<ServiceInvoice?> GetByIdWithTrackingAsync(int id); // Định nghĩa phương thức bất đồng bộ 'GetByIdWithTrackingAsync' với tham số (id) trả về kiểu Task<ServiceInvoice?>.

        /// <summary>
        /// Kiểm tra phiếu đã có hóa đơn chưa.
        /// </summary>
        Task<bool> InvoiceExistsForTicketAsync(int ticketId); // Định nghĩa phương thức bất đồng bộ 'InvoiceExistsForTicketAsync' với tham số (ticketId) trả về kiểu Task<bool>.

        Task AddAsync(ServiceInvoice invoice); // Định nghĩa phương thức bất đồng bộ 'AddAsync' với tham số (invoice) trả về kiểu Task.
        Task SaveChangesAsync(); // Định nghĩa phương thức bất đồng bộ 'SaveChangesAsync' không tham số trả về kiểu Task.
    }
}

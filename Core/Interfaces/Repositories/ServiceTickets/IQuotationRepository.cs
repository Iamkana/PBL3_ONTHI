using PBL3.Core.Entities; // Nhập namespace chứa các thực thể nghiệp vụ cốt lõi của hệ thống.

namespace PBL3.Core.Interfaces // Định nghĩa namespace PBL3.Core.Interfaces để quản lý thực thể/giao diện.
{
    /// <summary>
    /// Repository interface cho Quotation.
    /// </summary>
    public interface IQuotationRepository // Định nghĩa giao diện (interface) IQuotationRepository quy định hợp đồng cho tầng dữ liệu.
    {
        /// <summary>
        /// Lấy báo giá theo Id, bao gồm Items, không tracking.
        /// </summary>
        Task<Quotation?> GetByIdWithItemsAsync(int id); // Định nghĩa phương thức bất đồng bộ 'GetByIdWithItemsAsync' với tham số (id) trả về kiểu Task<Quotation?>.

        /// <summary>
        /// Lấy báo giá theo Id, với tracking để update.
        /// </summary>
        Task<Quotation?> GetByIdWithTrackingAsync(int id); // Định nghĩa phương thức bất đồng bộ 'GetByIdWithTrackingAsync' với tham số (id) trả về kiểu Task<Quotation?>.

        /// <summary>
        /// Lấy danh sách báo giá của một phiếu (allow multiple revisions).
        /// </summary>
        Task<List<Quotation>> GetByTicketIdAsync(int ticketId); // Định nghĩa phương thức bất đồng bộ 'GetByTicketIdAsync' với tham số (ticketId) trả về kiểu Task<List<Quotation>>.

        /// <summary>
        /// Kiểm tra phiếu đã có báo giá được chấp nhận (status=Accepted) hay không.
        /// </summary>
        Task<bool> HasAcceptedQuotationAsync(int ticketId); // Định nghĩa phương thức bất đồng bộ 'HasAcceptedQuotationAsync' với tham số (ticketId) trả về kiểu Task<bool>.

        Task AddAsync(Quotation quotation); // Định nghĩa phương thức bất đồng bộ 'AddAsync' với tham số (quotation) trả về kiểu Task.
        Task SaveChangesAsync(); // Định nghĩa phương thức bất đồng bộ 'SaveChangesAsync' không tham số trả về kiểu Task.
    }
}

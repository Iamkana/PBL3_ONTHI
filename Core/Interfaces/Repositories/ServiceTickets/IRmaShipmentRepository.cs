using PBL3.Core.Entities; // Nhập namespace chứa các thực thể nghiệp vụ cốt lõi của hệ thống.

namespace PBL3.Core.Interfaces // Định nghĩa namespace PBL3.Core.Interfaces để quản lý thực thể/giao diện.
{
    /// <summary>
    /// Repository interface cho RmaShipment.
    /// </summary>
    public interface IRmaShipmentRepository // Định nghĩa giao diện (interface) IRmaShipmentRepository quy định hợp đồng cho tầng dữ liệu.
    {
        /// <summary>
        /// Lấy phiếu RMA của một phiếu sửa chữa (1:1), không tracking.
        /// </summary>
        Task<RmaShipment?> GetByTicketIdAsync(int ticketId); // Định nghĩa phương thức bất đồng bộ 'GetByTicketIdAsync' với tham số (ticketId) trả về kiểu Task<RmaShipment?>.

        /// <summary>
        /// Lấy RMA theo Id, với tracking để update.
        /// </summary>
        Task<RmaShipment?> GetByIdWithTrackingAsync(int id); // Định nghĩa phương thức bất đồng bộ 'GetByIdWithTrackingAsync' với tham số (id) trả về kiểu Task<RmaShipment?>.

        Task AddAsync(RmaShipment shipment); // Định nghĩa phương thức bất đồng bộ 'AddAsync' với tham số (shipment) trả về kiểu Task.
        Task SaveChangesAsync(); // Định nghĩa phương thức bất đồng bộ 'SaveChangesAsync' không tham số trả về kiểu Task.
    }
}

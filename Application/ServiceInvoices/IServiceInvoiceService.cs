using PBL3.Shared.DTOs.ServiceTickets; // Sử dụng DTO của module hóa đơn/phiếu dịch vụ kỹ thuật.

namespace PBL3.Application.ServiceInvoices // Khai báo namespace cho tầng Application của module hóa đơn dịch vụ.
{
    public interface IServiceInvoiceService // Định nghĩa giao diện dịch vụ hóa đơn dịch vụ IServiceInvoiceService.
    {
        Task<ServiceInvoiceDetailDto?> GetByIdAsync(int id); // Khai báo phương thức lấy chi tiết hóa đơn dịch vụ kỹ thuật theo Id.
        Task<ServiceInvoiceDetailDto?> GetByTicketIdAsync(int ticketId); // Khai báo phương thức lấy hóa đơn dịch vụ tương ứng của một phiếu sửa chữa (ticketId).
        Task<(List<ServiceInvoiceListDto> Items, int TotalCount)> GetPagedListAsync( // Khai báo phương thức lấy danh sách hóa đơn dịch vụ phân trang.
            string? keyword, byte? paymentStatus, DateTime? fromDate, DateTime? toDate, // Lọc theo từ khóa, trạng thái thanh toán, khoảng thời gian.
            int pageNumber, int pageSize, string? sortBy, bool sortDescending); // Các tham số cấu hình phân trang và sắp xếp.
        Task MarkInvoicePaidAsync(int id); // Khai báo phương thức đánh dấu hóa đơn dịch vụ đã được khách thanh toán tiền thành công.
    }
}

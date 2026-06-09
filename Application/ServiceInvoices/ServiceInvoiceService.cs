using System; // Sử dụng các kiểu dữ liệu cơ bản của hệ thống.
using System.Collections.Generic; // Sử dụng các cấu trúc tập hợp danh sách.
using System.Linq; // Sử dụng các phương thức mở rộng truy vấn Linq.
using System.Threading.Tasks; // Sử dụng các tác vụ lập trình bất đồng bộ.
using PBL3.Core.Interfaces; // Sử dụng giao diện Repository từ tầng Core.
using PBL3.Shared.DTOs.ServiceTickets; // Sử dụng DTO của module hóa đơn sửa chữa.

namespace PBL3.Application.ServiceInvoices // Khai báo namespace cho tầng Application của module hóa đơn dịch vụ.
{
    public class ServiceInvoiceService( // Định nghĩa lớp ServiceInvoiceService sử dụng Primary Constructor.
        IServiceInvoiceRepository repository) : IServiceInvoiceService // Tiêm repository hóa đơn dịch vụ và triển khai IServiceInvoiceService.
    {
        private readonly IServiceInvoiceRepository _repository = // Gán repository hóa đơn dịch vụ vào trường thành viên.
            repository ?? throw new ArgumentNullException(nameof(repository)); // Kiểm tra null cho repository.

        public async Task<ServiceInvoiceDetailDto?> GetByIdAsync(int id) // Định nghĩa phương thức lấy chi tiết hóa đơn theo Id.
        {
            var invoice = await _repository.GetByIdWithDetailsAsync(id); // Gọi repository lấy thông tin thực thể hóa đơn dịch vụ đầy đủ chi tiết các mục.
            return invoice == null ? null : MapToDetailDto(invoice); // Trả về null nếu không tìm thấy hóa đơn, ngược lại trả về DTO đã được ánh xạ.
        }

        public async Task<ServiceInvoiceDetailDto?> GetByTicketIdAsync(int ticketId) // Định nghĩa phương thức lấy hóa đơn dịch vụ theo Id phiếu sửa chữa.
        {
            var invoice = await _repository.GetByTicketIdAsync(ticketId); // Gọi repository tìm hóa đơn dịch vụ được liên kết với phiếu dịch vụ sửa chữa (ticketId).
            return invoice == null ? null : MapToDetailDto(invoice); // Trả về null nếu chưa xuất hóa đơn cho phiếu này, ngược lại trả về DTO chi tiết.
        }

        public async Task<(List<ServiceInvoiceListDto> Items, int TotalCount)> GetPagedListAsync( // Định nghĩa phương thức lấy danh sách hóa đơn dịch vụ phân trang.
            string? keyword, byte? paymentStatus, DateTime? fromDate, DateTime? toDate, // Lọc theo từ khóa, trạng thái thanh toán, khoảng thời gian hóa đơn phát hành.
            int pageNumber, int pageSize, string? sortBy, bool sortDescending) // Các thuộc tính cấu hình phân trang và sắp xếp.
        {
            var (items, totalCount) = await _repository.GetPagedListAsync( // Gọi repository lấy danh sách hóa đơn dịch vụ và đếm tổng số phần tử thỏa mãn.
                keyword, paymentStatus, fromDate, toDate, pageNumber, pageSize, sortBy, sortDescending); // Truyền bộ lọc và thông số phân trang.
            var dtos = items.Select(MapToListDto).ToList(); // Ánh xạ danh sách thực thể động sang danh sách DTO rút gọn.
            return (dtos, totalCount); // Trả về bộ kết quả gồm danh sách DTO và tổng số lượng hóa đơn.
        }

        public async Task MarkInvoicePaidAsync(int id) // Định nghĩa phương thức đánh dấu hóa đơn dịch vụ đã thanh toán.
        {
            var invoice = await _repository.GetByIdWithTrackingAsync(id); // Gọi repository lấy thực thể hóa đơn theo Id ở chế độ theo dõi thay đổi (tracking).
            if (invoice == null) // Nếu không tìm thấy hóa đơn trong hệ thống.
                throw new InvalidOperationException("Không tìm thấy hóa đơn dịch vụ."); // Bắn ra ngoại lệ InvalidOperationException.
            if (invoice.PaymentStatus == 1) // Nếu trạng thái hóa đơn vốn đã được thanh toán rồi (PaymentStatus = 1).
                throw new InvalidOperationException("Hóa đơn đã được thanh toán trước đó."); // Bắn ra ngoại lệ thông báo không được thanh toán lại.

            invoice.PaymentStatus = 1; // Thực hiện thay đổi trạng thái thanh toán của hóa đơn thành Đã thanh toán (1).
            await _repository.SaveChangesAsync(); // Lưu thay đổi cập nhật thực thể xuống cơ sở dữ liệu.
        }

        private ServiceInvoiceDetailDto MapToDetailDto(dynamic invoice) // Định nghĩa hàm phụ trợ ánh xạ từ thực thể dynamic sang DTO chi tiết hóa đơn dịch vụ.
        {
            var items = new List<ServiceInvoiceItemDto>(); // Khởi tạo danh sách các DTO chi tiết khoản mục hóa đơn dịch vụ.
            if (invoice.Items != null) // Nếu danh sách chi tiết các khoản mục hóa đơn không bị null.
            {
                foreach (var i in invoice.Items) // Duyệt qua từng khoản mục thực thể (ví dụ: linh kiện thay thế, dịch vụ sửa chữa).
                {
                    items.Add(new ServiceInvoiceItemDto // Ánh xạ sang DTO khoản mục hóa đơn dịch vụ.
                    {
                        Id = i.Id, // Ánh xạ Id khoản mục.
                        Description = i.Description, // Ánh xạ mô tả khoản mục (ví dụ: Thay màn hình, Vệ sinh máy).
                        Quantity = i.Quantity, // Ánh xạ số lượng.
                        UnitPrice = i.UnitPrice, // Ánh xạ đơn giá.
                        LineTotal = i.LineTotal // Ánh xạ tổng tiền khoản mục này.
                    }); // Kết thúc thêm khoản mục.
                } // Kết thúc vòng lặp.
            } // Kết thúc khối điều kiện.

            return new ServiceInvoiceDetailDto // Khởi tạo DTO chi tiết hóa đơn dịch vụ hoàn chỉnh để trả về.
            {
                Id = invoice.Id, // Ánh xạ Id hóa đơn.
                InvoiceCode = invoice.InvoiceCode, // Ánh xạ mã số hóa đơn tự sinh.
                TicketId = invoice.TicketId, // Ánh xạ Id phiếu dịch vụ liên kết.
                QuotationId = invoice.QuotationId, // Ánh xạ Id bảng báo giá dịch vụ (nếu có).
                IssuedDate = invoice.IssuedDate, // Ánh xạ ngày xuất hóa đơn.
                IssuedByEmployeeId = invoice.IssuedByEmployeeId, // Ánh xạ Id nhân viên thực hiện xuất hóa đơn.
                LaborCost = invoice.LaborCost, // Ánh xạ tiền công sửa chữa.
                PartsTotal = invoice.PartsTotal, // Ánh xạ tổng chi phí linh kiện.
                GrandTotal = invoice.GrandTotal, // Ánh xạ tổng giá trị hóa đơn (Tiền công + Linh kiện).
                PaymentMethod = invoice.PaymentMethod, // Ánh xạ phương thức thanh toán sử dụng (Ví dụ: Tiền mặt, Chuyển khoản).
                PaymentStatus = invoice.PaymentStatus, // Ánh xạ trạng thái thanh toán hiện tại.
                Note = invoice.Note, // Ánh xạ ghi chú bổ sung của hóa đơn.
                Items = items // Gán danh sách DTO khoản mục chi tiết vừa ánh xạ.
            }; // Kết thúc khởi tạo DTO.
        }

        private ServiceInvoiceListDto MapToListDto(dynamic invoice) // Định nghĩa hàm phụ trợ ánh xạ thực thể sang DTO rút gọn hiển thị danh sách.
        {
            return new ServiceInvoiceListDto // Khởi tạo DTO rút gọn danh sách hóa đơn dịch vụ.
            {
                Id = invoice.Id, // Ánh xạ Id hóa đơn.
                InvoiceCode = invoice.InvoiceCode, // Ánh xạ mã hóa đơn.
                TicketId = invoice.TicketId, // Ánh xạ Id phiếu dịch vụ liên kết.
                IssuedDate = invoice.IssuedDate, // Ánh xạ ngày xuất hóa đơn.
                GrandTotal = invoice.GrandTotal, // Ánh xạ tổng tiền hóa đơn.
                PaymentStatus = invoice.PaymentStatus // Ánh xạ trạng thái thanh toán.
            }; // Kết thúc DTO.
        }
    }
}

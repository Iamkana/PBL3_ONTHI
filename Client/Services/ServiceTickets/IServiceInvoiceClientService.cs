using PBL3.Shared.DTOs.Common; // Sử dụng các DTO của module Common thuộc tầng Shared.
using PBL3.Shared.DTOs.ServiceTickets; // Sử dụng các DTO của module ServiceTickets thuộc tầng Shared.

namespace Client.Services.ServiceTickets; // Thiết lập namespace Client.Services.ServiceTickets để tổ chức quản lý cấu trúc các lớp.

public interface IServiceInvoiceClientService // Định nghĩa giao diện (interface) IServiceInvoiceClientService quy định các hàm tương tác của client.
{
    Task<ApiResult<PagedResult<ServiceInvoiceListDto>>> GetPagedListAsync( // Thực hiện xử lý phương thức nghiệp vụ 'GetPagedListAsync' không tham số trả về kết quả kiểu Task<ApiResult<PagedResult<ServiceInvoiceListDto>>>.
        string? keyword, byte? paymentStatus, DateTime? fromDate, DateTime? toDate, // Thực thi dòng lệnh nghiệp vụ.
        int pageNumber, int pageSize, string? sortBy, bool sortDescending); // Thực thi dòng lệnh nghiệp vụ.
    Task<ApiResult<ServiceInvoiceDetailDto>> GetByIdAsync(int id); // Khai báo phương thức giao diện 'GetByIdAsync' với tham số (id) có kết quả trả về kiểu Task<ApiResult<ServiceInvoiceDetailDto>>.
    Task<ApiResult<bool>> MarkInvoicePaidAsync(int id); // Khai báo phương thức giao diện 'MarkInvoicePaidAsync' với tham số (id) có kết quả trả về kiểu Task<ApiResult<bool>>.
}

using PBL3.Shared.DTOs.Common; // Sử dụng các DTO của module Common thuộc tầng Shared.
using PBL3.Shared.DTOs.ServiceTickets; // Sử dụng các DTO của module ServiceTickets thuộc tầng Shared.

namespace Client.Services.ServiceTickets; // Thiết lập namespace Client.Services.ServiceTickets để tổ chức quản lý cấu trúc các lớp.

public interface IServiceTicketClientService // Định nghĩa giao diện (interface) IServiceTicketClientService quy định các hàm tương tác của client.
{
    Task<ApiResult<ServiceTicketIntakeEvaluationDto>> EvaluateIntakeAsync(string serialNumber); // Khai báo phương thức giao diện 'EvaluateIntakeAsync' với tham số (serialNumber) có kết quả trả về kiểu Task<ApiResult<ServiceTicketIntakeEvaluationDto>>.
    Task<ApiResult<ServiceTicketDetailDto>> CreateTicketAsync(CreateServiceTicketRequest request); // Khai báo phương thức giao diện 'CreateTicketAsync' với tham số (request) có kết quả trả về kiểu Task<ApiResult<ServiceTicketDetailDto>>.
    Task<ApiResult<PagedResult<ServiceTicketListDto>>> GetPagedListAsync( // Thực hiện xử lý phương thức nghiệp vụ 'GetPagedListAsync' không tham số trả về kết quả kiểu Task<ApiResult<PagedResult<ServiceTicketListDto>>>.
        string? keyword, byte? status, DateTime? fromDate, DateTime? toDate, // Thực thi dòng lệnh nghiệp vụ.
        int pageNumber, int pageSize, string? sortBy, bool sortDescending); // Thực thi dòng lệnh nghiệp vụ.
    Task<ApiResult<PagedResult<ServiceTicketListDto>>> GetMyTicketsAsync( // Thực hiện xử lý phương thức nghiệp vụ 'GetMyTicketsAsync' không tham số trả về kết quả kiểu Task<ApiResult<PagedResult<ServiceTicketListDto>>>.
        string? keyword, byte? status, int pageNumber, int pageSize, string? sortBy, bool sortDescending); // Thực thi dòng lệnh nghiệp vụ.
    Task<ApiResult<ServiceTicketDetailDto>> GetByIdAsync(int id); // Khai báo phương thức giao diện 'GetByIdAsync' với tham số (id) có kết quả trả về kiểu Task<ApiResult<ServiceTicketDetailDto>>.
    Task<ApiResult<bool>> AssignTechnicianAsync(int ticketId, AssignTechnicianRequest request); // Khai báo phương thức giao diện 'AssignTechnicianAsync' với tham số (ticketId, request) có kết quả trả về kiểu Task<ApiResult<bool>>.
    Task<ApiResult<bool>> RecordDiagnosisAsync(int ticketId, RecordDiagnosisRequest request); // Khai báo phương thức giao diện 'RecordDiagnosisAsync' với tham số (ticketId, request) có kết quả trả về kiểu Task<ApiResult<bool>>.
    Task<ApiResult<bool>> ChooseBranchAsync(int ticketId, ChooseBranchRequest request); // Khai báo phương thức giao diện 'ChooseBranchAsync' với tham số (ticketId, request) có kết quả trả về kiểu Task<ApiResult<bool>>.
    Task<ApiResult<QuotationDto>> CreateQuotationAsync(int ticketId, CreateQuotationRequest request); // Khai báo phương thức giao diện 'CreateQuotationAsync' với tham số (ticketId, request) có kết quả trả về kiểu Task<ApiResult<QuotationDto>>.
    Task<ApiResult<bool>> AcceptQuotationAsync(int ticketId, int quotationId, AcceptQuotationRequest request); // Khai báo phương thức giao diện 'AcceptQuotationAsync' với tham số (ticketId, quotationId, request) có kết quả trả về kiểu Task<ApiResult<bool>>.
    Task<ApiResult<bool>> RejectQuotationAsync(int ticketId, int quotationId, RejectQuotationRequest request); // Khai báo phương thức giao diện 'RejectQuotationAsync' với tham số (ticketId, quotationId, request) có kết quả trả về kiểu Task<ApiResult<bool>>.
    Task<ApiResult<RmaShipmentDto>> CreateRmaShipmentAsync(int ticketId, CreateRmaShipmentRequest request); // Khai báo phương thức giao diện 'CreateRmaShipmentAsync' với tham số (ticketId, request) có kết quả trả về kiểu Task<ApiResult<RmaShipmentDto>>.
    Task<ApiResult<bool>> RecordRmaResolutionAsync(int ticketId, UpdateRmaResolutionRequest request); // Khai báo phương thức giao diện 'RecordRmaResolutionAsync' với tham số (ticketId, request) có kết quả trả về kiểu Task<ApiResult<bool>>.
    Task<ApiResult<bool>> Perform1For1SwapAsync(int ticketId, Perform1For1SwapRequest request); // Khai báo phương thức giao diện 'Perform1For1SwapAsync' với tham số (ticketId, request) có kết quả trả về kiểu Task<ApiResult<bool>>.
    Task<ApiResult<bool>> StartRepairAsync(int ticketId); // Khai báo phương thức giao diện 'StartRepairAsync' với tham số (ticketId) có kết quả trả về kiểu Task<ApiResult<bool>>.
    Task<ApiResult<bool>> MarkWaitingPartsAsync(int ticketId); // Khai báo phương thức giao diện 'MarkWaitingPartsAsync' với tham số (ticketId) có kết quả trả về kiểu Task<ApiResult<bool>>.
    Task<ApiResult<bool>> ResumeRepairAsync(int ticketId); // Khai báo phương thức giao diện 'ResumeRepairAsync' với tham số (ticketId) có kết quả trả về kiểu Task<ApiResult<bool>>.
    Task<ApiResult<bool>> CompleteRepairAsync(int ticketId, CompleteRepairRequest request); // Khai báo phương thức giao diện 'CompleteRepairAsync' với tham số (ticketId, request) có kết quả trả về kiểu Task<ApiResult<bool>>.
    Task<ApiResult<ServiceInvoiceDetailDto>> IssueServiceInvoiceAsync(int ticketId, IssueServiceInvoiceRequest request); // Khai báo phương thức giao diện 'IssueServiceInvoiceAsync' với tham số (ticketId, request) có kết quả trả về kiểu Task<ApiResult<ServiceInvoiceDetailDto>>.
    Task<ApiResult<bool>> CancelTicketAsync(int ticketId, CancelTicketRequest request); // Khai báo phương thức giao diện 'CancelTicketAsync' với tham số (ticketId, request) có kết quả trả về kiểu Task<ApiResult<bool>>.
    Task<ApiResult<List<ServiceTicketStatusHistoryDto>>> GetHistoryAsync(int ticketId); // Khai báo phương thức giao diện 'GetHistoryAsync' với tham số (ticketId) có kết quả trả về kiểu Task<ApiResult<List<ServiceTicketStatusHistoryDto>>>.
    Task<ApiResult<List<SerialRepairLogDto>>> GetSerialRepairHistoryAsync(string serialNumber); // Khai báo phương thức giao diện 'GetSerialRepairHistoryAsync' với tham số (serialNumber) có kết quả trả về kiểu Task<ApiResult<List<SerialRepairLogDto>>>.
}

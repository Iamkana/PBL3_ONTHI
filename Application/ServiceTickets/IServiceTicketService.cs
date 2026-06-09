using PBL3.Shared.DTOs.ServiceTickets; // Sử dụng DTO của module quản lý dịch vụ kỹ thuật và phiếu bảo hành.

namespace PBL3.Application.ServiceTickets // Khai báo namespace cho tầng Application của module phiếu dịch vụ.
{
    public interface IServiceTicketService // Định nghĩa giao diện dịch vụ quản lý phiếu sửa chữa/bảo hành IServiceTicketService.
    {
        Task<ServiceTicketIntakeEvaluationDto> GetWarrantyEvaluationAsync(string serialNumber); // Khai báo phương thức kiểm định nhanh thời hạn bảo hành của thiết bị qua số Serial.
        Task<ServiceTicketDetailDto?> CreateTicketFromSerialScanAsync(ServiceTicketIntakeRequestDto request, Guid userId); // Khai báo phương thức khởi tạo phiếu dịch vụ mới từ việc quét số Serial.
        Task<bool> AssignTechnicianAsync(int ticketId, Guid employeeId, Guid userId); // Khai báo phương thức phân công kỹ thuật viên đảm nhận sửa chữa cho phiếu dịch vụ.
        Task<bool> RecordDiagnosisAsync(int ticketId, ServiceTicketDiagnosisDto request, Guid userId, bool isAdmin = false); // Khai báo phương thức ghi nhận kết quả chẩn đoán lỗi thiết bị.
        Task<bool> ChooseBranchAsync(int ticketId, ServiceTicketBranchDto request, Guid userId, bool isAdmin = false); // Khai báo phương thức định hướng luồng xử lý: Sửa chữa tại cửa hàng hay gửi lên Hãng (RMA).
        Task<QuotationDetailDto?> CreateQuotationAsync(int ticketId, QuotationCreateDto request, Guid userId, bool isAdmin = false); // Khai báo phương thức lập bảng báo giá chi tiết linh kiện và tiền công sửa chữa.
        Task<bool> AcceptQuotationAsync(int ticketId, int quotationId, QuotationAcceptDto nextStatus, Guid userId, bool isAdmin = false); // Khai báo phương thức khách hàng/quản trị viên đồng ý bảng báo giá để bắt đầu sửa.
        Task<bool> RejectQuotationAsync(int ticketId, int quotationId, QuotationRejectDto request, Guid userId, bool isAdmin = false); // Khai báo phương thức khách hàng/quản trị từ chối bảng báo giá dịch vụ.
        Task<RmaShipmentDetailDto?> CreateRmaShipmentAsync(int ticketId, RmaShipmentCreateDto request, Guid userId, bool isAdmin = false); // Khai báo phương thức tạo phiếu gửi hàng bảo hành lên Hãng sản xuất (RMA Shipment).
        Task<bool> RecordRmaResolutionAsync(int ticketId, RmaResolutionUpdateDto request, Guid userId, bool isAdmin = false); // Khai báo phương thức ghi nhận kết quả phản hồi bảo hành từ phía Hãng (RMA Resolution).
        Task<bool> Perform1For1SwapAsync(int ticketId, int newSerialId, Guid userId, bool isAdmin = false); // Khai báo phương thức thực hiện đổi mới thiết bị 1-đổi-1 (cập nhật serial mới, hoàn trả serial cũ).
        Task<bool> MarkInternalRepairCompletedAsync(int ticketId, ServiceTicketCompleteDto request, Guid userId, bool isAdmin = false); // Khai báo phương thức xác nhận hoàn thành sửa chữa nội bộ tại cửa hàng.
        Task<bool> StartRepairAsync(int ticketId, Guid userId, bool isAdmin = false); // Khai báo phương thức chuyển trạng thái bắt đầu tiến hành sửa chữa thiết bị.
        Task<bool> MarkWaitingPartsAsync(int ticketId, Guid userId, bool isAdmin = false); // Khai báo phương thức đánh dấu thiết bị đang đợi linh kiện thay thế gửi về.
        Task<bool> ResumeRepairAsync(int ticketId, Guid userId, bool isAdmin = false); // Khai báo phương thức tiếp tục tiến trình sửa chữa sau khi đã có linh kiện.
        Task<ServiceInvoiceDetailDto?> IssueServiceInvoiceAsync(int ticketId, ServiceInvoiceCreateDto request, Guid userId, bool isAdmin = false); // Khai báo phương thức xuất hóa đơn tài chính cho phiếu dịch vụ kỹ thuật.
        Task<bool> CancelTicketAsync(int ticketId, string reason, Guid userId); // Khai báo phương thức hủy bỏ phiếu dịch vụ kỹ thuật kèm lý do cụ thể.
        Task<ServiceTicketDetailDto?> GetTicketByIdAsync(int id, Guid? currentUserId = null, bool isCustomer = false); // Khai báo phương thức lấy thông tin chi tiết của một phiếu dịch vụ theo Id.
        Task<(List<ServiceTicketListDto> Items, int TotalCount)> GetPagedTicketsAsync( // Khai báo phương thức lấy danh sách tất cả phiếu dịch vụ phân trang có bộ lọc đa năng.
            string? keyword, byte? status, byte? resolutionType, Guid? assignedEmployeeId, Guid? customerId, // Các bộ lọc từ khóa, trạng thái phiếu, loại hướng giải quyết, nhân viên đảm nhận, khách hàng.
            DateTime? fromDate, DateTime? toDate, int pageNumber, int pageSize, string? sortBy, bool sortDescending); // Khoảng thời gian, trang, số phần tử trên trang, sắp xếp.
        Task<(List<ServiceTicketListDto> Items, int TotalCount)> GetMyTicketsAsync( // Khai báo phương thức lấy danh sách các phiếu dịch vụ thuộc về cá nhân (kỹ thuật viên phụ trách hoặc khách hàng).
            Guid userId, string? keyword, byte? status, int pageNumber, int pageSize, string? sortBy, bool sortDescending); // Id người dùng, từ khóa, trạng thái, phân trang và cấu hình sắp xếp.
        Task<List<ServiceTicketStatusHistoryDto>> GetTicketHistoryAsync(int ticketId); // Khai báo phương thức lấy lịch sử nhật ký chuyển đổi trạng thái của phiếu sửa chữa.
        Task<List<SerialRepairHistoryDto>> GetSerialRepairHistoryAsync(string serialNumber); // Khai báo phương thức tra cứu lịch sử sửa chữa/bảo hành trong quá khứ của một thiết bị qua số Serial.
    }
}

using PBL3.Shared.DTOs.Common; // Sử dụng DTO dùng chung ApiResult và PagedResult.
using PBL3.Shared.DTOs.Inventory; // Sử dụng các DTO quản lý kiểm kê kho.

namespace PBL3.Application.Inventory // Khai báo namespace cho tầng Application của module kiểm kê kho.
{
    public interface IInventoryCheckService // Định nghĩa giao diện dịch vụ kiểm kê kho IInventoryCheckService.
    {
        Task<ApiResult<InventoryCheckDto>> CreateAsync(CreateInventoryCheckRequest request, Guid employeeId); // Khai báo phương thức tạo phiếu kiểm kê mới và chốt snapshot tồn kho sổ sách.
        Task<ApiResult<PagedResult<InventoryCheckListItemDto>>> GetPagedListAsync(InventoryCheckFilterRequest filter); // Khai báo phương thức lấy danh sách phiếu kiểm kê phân trang bất đồng bộ.
        Task<ApiResult<InventoryCheckDto>> GetByIdAsync(int id); // Khai báo phương thức lấy thông tin chi tiết phiếu kiểm kê theo Id.
        Task<ApiResult<InventoryCheckDashboardDto>> GetDashboardAsync(int id); // Khai báo phương thức lấy dữ liệu dashboard đối chiếu số liệu kiểm kê thời gian thực.
        Task<ApiResult<PagedResult<InventoryCheckSerialDto>>> GetSerialsAsync(int checkId, InventoryCheckSerialFilterRequest filter); // Khai báo phương thức lấy danh sách serial phân trang theo bộ lọc ScanStatus.
        Task<ApiResult<ScanResultDto>> ScanSerialAsync(int checkId, ScanSerialRequest request, Guid employeeId); // Khai báo phương thức quét một mã Serial vào phiếu kiểm kê.
        Task<ApiResult<bool>> MarkDefectiveAsync(int checkId, int detailSerialId, Guid employeeId); // Khai báo phương thức đánh dấu Serial sản phẩm bị hỏng/lỗi vật lý.
        Task<ApiResult<bool>> UpdateReasonAsync(int checkId, int detailSerialId, UpdateScanReasonRequest request, Guid employeeId); // Khai báo phương thức cập nhật nguyên nhân chênh lệch và hướng xử lý đề xuất cho Serial.
        Task<ApiResult<bool>> SubmitAsync(int checkId, Guid employeeId); // Khai báo phương thức gửi duyệt phiếu kiểm kê (chuyển trạng thái sang Chờ phê duyệt).
        Task<ApiResult<bool>> ApproveAsync(int checkId, Guid adminId); // Khai báo phương thức phê duyệt và tự động cân bằng tồn kho hệ thống (chỉ dành cho Admin).
        Task<ApiResult<bool>> RejectAsync(int checkId, RejectInventoryCheckRequest request, Guid adminId); // Khai báo phương thức từ chối duyệt phiếu kiểm kê (chỉ dành cho Admin).
        Task<ApiResult<bool>> CancelAsync(int checkId, Guid employeeId, bool isAdmin); // Khai báo phương thức hủy bỏ phiếu kiểm kê đang ở trạng thái nháp.
    }
}

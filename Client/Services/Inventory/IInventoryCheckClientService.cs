using PBL3.Shared.DTOs.Common; // Sử dụng các DTO của module Common thuộc tầng Shared.
using PBL3.Shared.DTOs.Inventory; // Sử dụng các DTO của module Inventory thuộc tầng Shared.

namespace Client.Services.Inventory; // Thiết lập namespace Client.Services.Inventory để tổ chức quản lý cấu trúc các lớp.

public interface IInventoryCheckClientService // Định nghĩa giao diện (interface) IInventoryCheckClientService quy định các hàm tương tác của client.
{
    Task<ApiResult<InventoryCheckDto>> CreateAsync(CreateInventoryCheckRequest request); // Khai báo phương thức giao diện 'CreateAsync' với tham số (request) có kết quả trả về kiểu Task<ApiResult<InventoryCheckDto>>.
    Task<ApiResult<PagedResult<InventoryCheckListItemDto>>> GetListAsync(InventoryCheckFilterRequest filter, CancellationToken cancellationToken = default); // Khai báo phương thức giao diện 'GetListAsync' với tham số (filter, default) có kết quả trả về kiểu Task<ApiResult<PagedResult<InventoryCheckListItemDto>>>.
    Task<ApiResult<InventoryCheckDto>> GetByIdAsync(int id); // Khai báo phương thức giao diện 'GetByIdAsync' với tham số (id) có kết quả trả về kiểu Task<ApiResult<InventoryCheckDto>>.
    Task<ApiResult<InventoryCheckDashboardDto>> GetDashboardAsync(int id); // Khai báo phương thức giao diện 'GetDashboardAsync' với tham số (id) có kết quả trả về kiểu Task<ApiResult<InventoryCheckDashboardDto>>.
    Task<ApiResult<PagedResult<InventoryCheckSerialDto>>> GetSerialsAsync(int checkId, InventoryCheckSerialFilterRequest filter, CancellationToken cancellationToken = default); // Khai báo phương thức giao diện 'GetSerialsAsync' với tham số (checkId, filter, default) có kết quả trả về kiểu Task<ApiResult<PagedResult<InventoryCheckSerialDto>>>.
    Task<ApiResult<ScanResultDto>> ScanSerialAsync(int checkId, ScanSerialRequest request); // Khai báo phương thức giao diện 'ScanSerialAsync' với tham số (checkId, request) có kết quả trả về kiểu Task<ApiResult<ScanResultDto>>.
    Task<ApiResult<bool>> MarkDefectiveAsync(int checkId, int detailSerialId); // Khai báo phương thức giao diện 'MarkDefectiveAsync' với tham số (checkId, detailSerialId) có kết quả trả về kiểu Task<ApiResult<bool>>.
    Task<ApiResult<bool>> UpdateReasonAsync(int checkId, int detailSerialId, UpdateScanReasonRequest request); // Khai báo phương thức giao diện 'UpdateReasonAsync' với tham số (checkId, detailSerialId, request) có kết quả trả về kiểu Task<ApiResult<bool>>.
    Task<ApiResult<bool>> SubmitAsync(int checkId); // Khai báo phương thức giao diện 'SubmitAsync' với tham số (checkId) có kết quả trả về kiểu Task<ApiResult<bool>>.
    Task<ApiResult<bool>> ApproveAsync(int checkId); // Khai báo phương thức giao diện 'ApproveAsync' với tham số (checkId) có kết quả trả về kiểu Task<ApiResult<bool>>.
    Task<ApiResult<bool>> RejectAsync(int checkId, RejectInventoryCheckRequest request); // Khai báo phương thức giao diện 'RejectAsync' với tham số (checkId, request) có kết quả trả về kiểu Task<ApiResult<bool>>.
    Task<ApiResult<bool>> CancelAsync(int checkId); // Khai báo phương thức giao diện 'CancelAsync' với tham số (checkId) có kết quả trả về kiểu Task<ApiResult<bool>>.
}

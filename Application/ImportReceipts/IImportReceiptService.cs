using PBL3.Shared.DTOs.Common; // Sử dụng DTO dùng chung ApiResult và PagedResult.
using PBL3.Shared.DTOs.Inventory; // Sử dụng các DTO quản lý kho hàng và phiếu nhập.
using PBL3.Shared.DTOs.Products; // Sử dụng các DTO sản phẩm.

namespace PBL3.Application.ImportReceipts // Khai báo namespace cho tầng Application của module quản lý phiếu nhập kho.
{
    public interface IImportReceiptService // Định nghĩa giao diện dịch vụ phiếu nhập kho IImportReceiptService.
    {
        Task<ApiResult<ImportReceiptDto>> CreateAsync(CreateImportReceiptRequest request); // Khai báo phương thức tạo phiếu nhập kho mới (sử dụng Transaction).
        Task<ApiResult<PagedResult<ImportReceiptDto>>> GetPagedListAsync(ImportReceiptFilterRequest filter); // Khai báo phương thức lấy danh sách phiếu nhập kho phân trang bất đồng bộ.
        Task<ApiResult<ImportReceiptDto>> GetByIdAsync(int id); // Khai báo phương thức lấy chi tiết phiếu nhập kho theo Id (kèm danh sách Serial).
    }
}

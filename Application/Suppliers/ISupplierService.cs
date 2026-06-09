using PBL3.Shared.DTOs.Common; // Sử dụng DTO dùng chung ApiResult và PagedResult.
using PBL3.Shared.DTOs.Products; // Sử dụng DTO sản phẩm.
using PBL3.Shared.DTOs.Suppliers; // Sử dụng DTO nhà cung cấp.

namespace PBL3.Application.Suppliers // Khai báo namespace cho tầng Application của module nhà cung cấp.
{
    public interface ISupplierService // Định nghĩa giao diện dịch vụ nhà cung cấp ISupplierService.
    {
        Task<ApiResult<PagedResult<SupplierDto>>> GetPagedListAsync(SupplierFilterRequest filter); // Khai báo phương thức lấy danh sách nhà cung cấp phân trang bất đồng bộ.
        Task<ApiResult<SupplierDto>> GetByIdAsync(int id); // Khai báo phương thức lấy chi tiết nhà cung cấp theo Id.
        Task<ApiResult<SupplierDto>> CreateAsync(CreateSupplierRequest request); // Khai báo phương thức tạo mới nhà cung cấp bất đồng bộ.
        Task<ApiResult<SupplierDto>> UpdateAsync(int id, UpdateSupplierRequest request); // Khai báo phương thức cập nhật nhà cung cấp theo Id.
        Task<ApiResult<bool>> DeleteAsync(int id); // Khai báo phương thức xóa mềm nhà cung cấp theo Id (Soft Delete).
    }
}

using PBL3.Shared.DTOs.Common; // Sử dụng DTO dùng chung ApiResult và PagedResult.
using PBL3.Shared.DTOs.Manufacturers; // Sử dụng các DTO liên quan đến hãng sản xuất.

namespace PBL3.Application.Manufacturers // Khai báo namespace cho tầng Application của module hãng sản xuất.
{
    public interface IManufacturerService // Định nghĩa giao diện dịch vụ hãng sản xuất IManufacturerService.
    {
        Task<ApiResult<PagedResult<ManufacturerDto>>> GetPagedListAsync(ManufacturerFilterRequest filter); // Khai báo phương thức lấy danh sách hãng sản xuất phân trang bất đồng bộ.
        Task<ApiResult<List<ManufacturerSummaryDto>>> GetAllForDropdownAsync(); // Khai báo phương thức lấy tất cả danh sách hãng sản xuất rút gọn phục vụ Dropdown.
        Task<ApiResult<ManufacturerDto>> GetByIdAsync(int id); // Khai báo phương thức lấy thông tin chi tiết hãng sản xuất theo Id.
        Task<ApiResult<ManufacturerDto>> CreateAsync(CreateManufacturerRequest request); // Khai báo phương thức tạo mới hãng sản xuất bất đồng bộ.
        Task<ApiResult<ManufacturerDto>> UpdateAsync(int id, UpdateManufacturerRequest request); // Khai báo phương thức cập nhật hãng sản xuất theo Id.
        Task<ApiResult<bool>> DeleteAsync(int id); // Khai báo phương thức xóa mềm hãng sản xuất theo Id (có kiểm tra ràng buộc sản phẩm).
    }
}

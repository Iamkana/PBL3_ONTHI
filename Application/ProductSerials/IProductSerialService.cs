using PBL3.Shared.DTOs.Common; // Sử dụng DTO dùng chung ApiResult và PagedResult.
using PBL3.Shared.DTOs.Inventory; // Sử dụng DTO của module kho hàng.

namespace PBL3.Application.ProductSerials // Khai báo namespace cho tầng Application của module quản lý số Serial.
{
    public interface IProductSerialService // Định nghĩa giao diện dịch vụ quản lý số Serial sản phẩm.
    {
        Task<ApiResult<bool>> CheckExistAsync(string serialNumber, int variantId); // Khai báo phương thức kiểm tra xem số Serial đã tồn tại trong DB chưa.
        Task<ApiResult<PagedResult<ProductSerialListDto>>> GetPagedListAsync(ProductSerialFilterRequest filter); // Khai báo phương thức lấy danh sách Serial phân trang có bộ lọc.
        Task<ApiResult<ProductSerialDetailDto>> GetByIdAsync(int id); // Khai báo phương thức lấy chi tiết một số Serial kèm lịch sử đơn hàng.
        Task<ApiResult<ProductSerialStatisticsDto>> GetStatisticsAsync(int? productId, int? variantId); // Khai báo phương thức lấy thống kê số lượng Serial theo các trạng thái.
        Task<ApiResult<bool>> UpdateStatusAsync(int id, UpdateSerialStatusRequest request); // Khai báo phương thức cập nhật trạng thái của số Serial (lỗi, trả hàng, phục hồi).
    }
}

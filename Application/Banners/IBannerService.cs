using PBL3.Shared.DTOs.Banners; // Sử dụng DTO của module banner quảng cáo.
using PBL3.Shared.DTOs.Common; // Sử dụng DTO dùng chung ApiResult.

namespace PBL3.Application.Banners // Khai báo namespace cho tầng Application của module banner.
{
    public interface IBannerService // Định nghĩa giao diện dịch vụ quản lý banner IBannerService.
    {
        Task<ApiResult<PagedResult<BannerDto>>> GetPagedListAsync(BannerFilterRequest filter); // Khai báo phương thức lấy danh sách banner phân trang có bộ lọc.
        Task<ApiResult<BannerDto>> GetByIdAsync(int id); // Khai báo phương thức lấy chi tiết banner theo Id.
        Task<ApiResult<List<BannerPublicDto>>> GetActiveAsync(); // Khai báo phương thức lấy danh sách banner đang hoạt động và có hiệu lực cho trang chủ.
        Task<ApiResult<BannerDto>> CreateAsync(CreateBannerRequest request); // Khai báo phương thức tạo mới banner quảng cáo.
        Task<ApiResult<BannerDto>> UpdateAsync(int id, UpdateBannerRequest request); // Khai báo phương thức cập nhật thông tin banner theo Id.
        Task<ApiResult<bool>> DeleteAsync(int id); // Khai báo phương thức xóa banner theo Id.
    }
}

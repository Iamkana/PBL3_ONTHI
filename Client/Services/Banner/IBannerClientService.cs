using PBL3.Shared.DTOs.Banners; // Sử dụng các DTO của module Banners thuộc tầng Shared.
using PBL3.Shared.DTOs.Common; // Sử dụng các DTO của module Common thuộc tầng Shared.

namespace Client.Services.Banner; // Thiết lập namespace Client.Services.Banner để tổ chức quản lý cấu trúc các lớp.

public interface IBannerClientService // Định nghĩa giao diện (interface) IBannerClientService quy định các hàm tương tác của client.
{
    Task<ApiResult<PagedResult<BannerDto>>> GetListAsync(BannerFilterRequest request); // Khai báo phương thức giao diện 'GetListAsync' với tham số (request) có kết quả trả về kiểu Task<ApiResult<PagedResult<BannerDto>>>.
    Task<ApiResult<BannerDto>> GetByIdAsync(int id); // Khai báo phương thức giao diện 'GetByIdAsync' với tham số (id) có kết quả trả về kiểu Task<ApiResult<BannerDto>>.
    Task<ApiResult<BannerDto>> CreateAsync(CreateBannerRequest request); // Khai báo phương thức giao diện 'CreateAsync' với tham số (request) có kết quả trả về kiểu Task<ApiResult<BannerDto>>.
    Task<ApiResult<BannerDto>> UpdateAsync(int id, UpdateBannerRequest request); // Khai báo phương thức giao diện 'UpdateAsync' với tham số (id, request) có kết quả trả về kiểu Task<ApiResult<BannerDto>>.
    Task<ApiResult<bool>> DeleteAsync(int id); // Khai báo phương thức giao diện 'DeleteAsync' với tham số (id) có kết quả trả về kiểu Task<ApiResult<bool>>.
    Task<ApiResult<List<BannerPublicDto>>> GetActiveAsync(); // Khai báo phương thức giao diện 'GetActiveAsync' không tham số có kết quả trả về kiểu Task<ApiResult<List<BannerPublicDto>>>.
}

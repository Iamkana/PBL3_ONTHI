using PBL3.Shared.DTOs.Common; // Sử dụng các DTO của module Common thuộc tầng Shared.
using PBL3.Shared.DTOs.Vouchers; // Sử dụng các DTO của module Vouchers thuộc tầng Shared.

namespace Client.Services.Voucher; // Thiết lập namespace Client.Services.Voucher để tổ chức quản lý cấu trúc các lớp.

public interface IVoucherClientService // Định nghĩa giao diện (interface) IVoucherClientService quy định các hàm tương tác của client.
{
    Task<ApiResult<PagedResult<VoucherDto>>> GetListAsync(VoucherFilterRequest request); // Khai báo phương thức giao diện 'GetListAsync' với tham số (request) có kết quả trả về kiểu Task<ApiResult<PagedResult<VoucherDto>>>.
    Task<ApiResult<VoucherDto>> GetByIdAsync(int id); // Khai báo phương thức giao diện 'GetByIdAsync' với tham số (id) có kết quả trả về kiểu Task<ApiResult<VoucherDto>>.
    Task<ApiResult<VoucherDto>> CreateAsync(CreateVoucherRequest request); // Khai báo phương thức giao diện 'CreateAsync' với tham số (request) có kết quả trả về kiểu Task<ApiResult<VoucherDto>>.
    Task<ApiResult<VoucherDto>> UpdateAsync(int id, UpdateVoucherRequest request); // Khai báo phương thức giao diện 'UpdateAsync' với tham số (id, request) có kết quả trả về kiểu Task<ApiResult<VoucherDto>>.
    Task<ApiResult<bool>> DeleteAsync(int id); // Khai báo phương thức giao diện 'DeleteAsync' với tham số (id) có kết quả trả về kiểu Task<ApiResult<bool>>.
    Task<ApiResult<VoucherDto>> ToggleStatusAsync(int id); // Khai báo phương thức giao diện 'ToggleStatusAsync' với tham số (id) có kết quả trả về kiểu Task<ApiResult<VoucherDto>>.
    Task<ApiResult<List<VoucherAvailabilityDto>>> GetAvailableForOrderAsync(GetAvailableVouchersRequest request); // Khai báo phương thức giao diện 'GetAvailableForOrderAsync' với tham số (request) có kết quả trả về kiểu Task<ApiResult<List<VoucherAvailabilityDto>>>.
}

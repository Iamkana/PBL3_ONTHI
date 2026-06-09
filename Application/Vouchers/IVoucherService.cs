using PBL3.Shared.DTOs.Common; // Sử dụng DTO dùng chung ApiResult và PagedResult.
using PBL3.Shared.DTOs.Vouchers; // Sử dụng các DTO liên quan đến voucher.

namespace PBL3.Application.Vouchers // Khai báo namespace cho tầng Application của module voucher.
{
    public interface IVoucherService // Định nghĩa giao diện dịch vụ voucher IVoucherService.
    {
        Task<ApiResult<PagedResult<VoucherDto>>> GetPagedListAsync(VoucherFilterRequest filter); // Khai báo phương thức lấy danh sách voucher phân trang bất đồng bộ.
        Task<ApiResult<VoucherDto>> GetByIdAsync(int id); // Khai báo phương thức lấy chi tiết voucher theo Id.
        Task<ApiResult<VoucherDto>> CreateAsync(CreateVoucherRequest request); // Khai báo phương thức tạo mới voucher bất đồng bộ.
        Task<ApiResult<VoucherDto>> UpdateAsync(int id, UpdateVoucherRequest request); // Khai báo phương thức cập nhật voucher theo Id bất đồng bộ.
        Task<ApiResult<bool>> DeleteAsync(int id); // Khai báo phương thức xóa mềm voucher theo Id.
        Task<ApiResult<VoucherDto>> ToggleStatusAsync(int id); // Khai báo phương thức bật/tắt trạng thái hoạt động (IsActive) của voucher.
        Task<ApiResult<ValidateVoucherResponse>> ValidateVoucherCodeAsync(ValidateVoucherRequest request, Guid? userId); // Khai báo phương thức kiểm tra mã voucher và tính toán giá trị giảm giá preview.
        Task<ApiResult<List<VoucherAvailabilityDto>>> GetAvailableForOrderAsync(GetAvailableVouchersRequest request, Guid? userId); // Khai báo phương thức lấy danh sách các voucher khả dụng cho đơn hàng hiện tại.
    }
}

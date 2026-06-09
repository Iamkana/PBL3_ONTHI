using PBL3.Shared.DTOs.Common; // Sử dụng các DTO dùng chung ApiResult.
using PBL3.Shared.DTOs.Pos; // Sử dụng DTO dùng cho nghiệp vụ bán hàng tại quầy POS.
using System; // Sử dụng các kiểu dữ liệu cơ bản của .NET.
using System.Collections.Generic; // Sử dụng cấu trúc danh sách tập hợp List.
using System.Threading.Tasks; // Sử dụng các tác vụ bất đồng bộ Task.

namespace PBL3.Application.Pos // Khai báo namespace cho tầng Application của module POS.
{
    public interface IPosService // Định nghĩa giao diện dịch vụ bán hàng tại quầy IPosService.
    {
        Task<ApiResult<PosScanResponse>> ScanSerialAsync(string serialNumber); // Khai báo phương thức quét mã số serial của thiết bị để đưa vào hóa đơn.
        Task<ApiResult<PosCustomerDto>> LookupCustomerAsync(string phone); // Khai báo phương thức tra cứu thông tin khách hàng dựa trên số điện thoại.
        Task<ApiResult<VoucherValidationDto>> ValidateVoucherAsync(string code, decimal subTotal); // Khai báo phương thức kiểm định tính hợp lệ của mã giảm giá khi áp dụng tại quầy.
        Task<ApiResult<PosOrderDto>> CheckoutAsync(PosCheckoutRequest request, Guid employeeId); // Khai báo phương thức thanh toán xuất hóa đơn chính thức tại quầy.
        Task<ApiResult<PosDraftDto>> SaveDraftAsync(PosCheckoutRequest request, Guid employeeId); // Khai báo phương thức lưu trữ đơn hàng nháp tại quầy.
        Task<ApiResult<List<PosDraftDto>>> GetDraftsAsync(Guid employeeId); // Khai báo phương thức lấy danh sách các đơn hàng nháp hiện có của nhân viên.
        Task<ApiResult<PosDraftDto>> GetDraftByIdAsync(int orderId, Guid employeeId); // Khai báo phương thức lấy thông tin chi tiết của đơn nháp theo Id.
        Task<ApiResult<bool>> DeleteDraftAsync(int orderId, Guid employeeId); // Khai báo phương thức xóa bỏ đơn hàng nháp.
    }
}

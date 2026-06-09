using PBL3.Shared.DTOs.Pos; // Sử dụng các DTO của module Pos thuộc tầng Shared.
using PBL3.Shared.DTOs.Common; // Sử dụng các DTO của module Common thuộc tầng Shared.

namespace Client.Services.Pos; // Thiết lập namespace Client.Services.Pos để tổ chức quản lý cấu trúc các lớp.

public interface IPosClientService // Định nghĩa giao diện (interface) IPosClientService quy định các hàm tương tác của client.
{
    Task<ApiResult<PosScanResponse>> ScanBarcodeAsync(string serialNumber); // Khai báo phương thức giao diện 'ScanBarcodeAsync' với tham số (serialNumber) có kết quả trả về kiểu Task<ApiResult<PosScanResponse>>.
    Task<ApiResult<PosCustomerDto>> SearchCustomerByPhoneAsync(string phone); // Khai báo phương thức giao diện 'SearchCustomerByPhoneAsync' với tham số (phone) có kết quả trả về kiểu Task<ApiResult<PosCustomerDto>>.
    Task<ApiResult<VoucherValidationDto>> ValidateVoucherAsync(string code, decimal subTotal); // Khai báo phương thức giao diện 'ValidateVoucherAsync' với tham số (code, subTotal) có kết quả trả về kiểu Task<ApiResult<VoucherValidationDto>>.
    Task<ApiResult<PosOrderDto>> CheckoutAsync(PosCheckoutRequest request); // Khai báo phương thức giao diện 'CheckoutAsync' với tham số (request) có kết quả trả về kiểu Task<ApiResult<PosOrderDto>>.
    Task<ApiResult<int>> SaveDraftAsync(PosCheckoutRequest request); // Khai báo phương thức giao diện 'SaveDraftAsync' với tham số (request) có kết quả trả về kiểu Task<ApiResult<int>>.
    Task<ApiResult<List<PosDraftDto>>> GetDraftsAsync(); // Khai báo phương thức giao diện 'GetDraftsAsync' không tham số có kết quả trả về kiểu Task<ApiResult<List<PosDraftDto>>>.
}

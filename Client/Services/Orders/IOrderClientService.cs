using System.Threading.Tasks; // Nhập (import) namespace System.Threading.Tasks để sử dụng các lớp bên trong.
using PBL3.Shared.DTOs.Common; // Sử dụng các DTO của module Common thuộc tầng Shared.
using PBL3.Shared.DTOs.Sale; // Sử dụng các DTO của module Sale thuộc tầng Shared.

namespace Client.Services.Orders; // Thiết lập namespace Client.Services.Orders để tổ chức quản lý cấu trúc các lớp.

public interface IOrderClientService // Định nghĩa giao diện (interface) IOrderClientService quy định các hàm tương tác của client.
{
    Task<ApiResult<OrderDetailDto>> GetByIdAsync(int id); // Khai báo phương thức giao diện 'GetByIdAsync' với tham số (id) có kết quả trả về kiểu Task<ApiResult<OrderDetailDto>>.
    Task<PagedResult<OrderSummaryResponse>> GetPagedOrdersAsync(OrderFilterRequest request); // Khai báo phương thức giao diện 'GetPagedOrdersAsync' với tham số (request) có kết quả trả về kiểu Task<PagedResult<OrderSummaryResponse>>.
    Task<ApiResult<PagedResult<OrderSummaryResponse>>> GetMyOrdersAsync(OrderFilterRequest request); // Khai báo phương thức giao diện 'GetMyOrdersAsync' với tham số (request) có kết quả trả về kiểu Task<ApiResult<PagedResult<OrderSummaryResponse>>>.
    Task<ApiResult<bool>> CancelOrderAsync(int id, string cancelReason); // Khai báo phương thức giao diện 'CancelOrderAsync' với tham số (id, cancelReason) có kết quả trả về kiểu Task<ApiResult<bool>>.
    Task<ApiResult<CheckoutResponse>> CheckoutAsync(CheckoutRequest request); // Khai báo phương thức giao diện 'CheckoutAsync' với tham số (request) có kết quả trả về kiểu Task<ApiResult<CheckoutResponse>>.
    Task<ApiResult<bool>> CompleteOrderAsync(int id); // Khai báo phương thức giao diện 'CompleteOrderAsync' với tham số (id) có kết quả trả về kiểu Task<ApiResult<bool>>.
    Task<ApiResult<bool>> ConfirmOrderAsync(int id); // Khai báo phương thức giao diện 'ConfirmOrderAsync' với tham số (id) có kết quả trả về kiểu Task<ApiResult<bool>>.
    Task<ApiResult<bool>> CancelMyOrderAsync(int id, string cancelReason); // Khai báo phương thức giao diện 'CancelMyOrderAsync' với tham số (id, cancelReason) có kết quả trả về kiểu Task<ApiResult<bool>>.
    Task<ApiResult<bool>> ConfirmReceivedAsync(int id); // Khai báo phương thức giao diện 'ConfirmReceivedAsync' với tham số (id) có kết quả trả về kiểu Task<ApiResult<bool>>.
}

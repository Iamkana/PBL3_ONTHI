using PBL3.Shared.DTOs.Cart; // Sử dụng các DTO của module Cart thuộc tầng Shared.
using PBL3.Shared.DTOs.Common; // Sử dụng các DTO của module Common thuộc tầng Shared.

namespace Client.Services.Cart; // Thiết lập namespace Client.Services.Cart để tổ chức quản lý cấu trúc các lớp.

public interface ICartClientService // Định nghĩa giao diện (interface) ICartClientService quy định các hàm tương tác của client.
{
    Task<ApiResult<bool>> AddToCartAsync(int variantId, int quantity); // Khai báo phương thức giao diện 'AddToCartAsync' với tham số (variantId, quantity) có kết quả trả về kiểu Task<ApiResult<bool>>.
    Task<ApiResult<CartResponse>> GetMyCartAsync(); // Khai báo phương thức giao diện 'GetMyCartAsync' không tham số có kết quả trả về kiểu Task<ApiResult<CartResponse>>.
    Task<ApiResult<CartResponse>> UpdateQuantityAsync(int cartItemId, int quantity); // Khai báo phương thức giao diện 'UpdateQuantityAsync' với tham số (cartItemId, quantity) có kết quả trả về kiểu Task<ApiResult<CartResponse>>.
    Task<ApiResult<CartResponse>> RemoveItemAsync(int cartItemId); // Khai báo phương thức giao diện 'RemoveItemAsync' với tham số (cartItemId) có kết quả trả về kiểu Task<ApiResult<CartResponse>>.
}

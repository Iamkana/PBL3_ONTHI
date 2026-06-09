using System; // Sử dụng các kiểu dữ liệu cơ bản của hệ thống.
using System.Threading.Tasks; // Sử dụng lập trình bất đồng bộ Task.
using PBL3.Shared.DTOs.Cart; // Sử dụng các DTO liên quan đến giỏ hàng.
using PBL3.Shared.DTOs.Common; // Sử dụng DTO dùng chung ApiResult.

namespace PBL3.Application.Cart // Khai báo namespace cho tầng Application của module giỏ hàng.
{
    public interface ICartService // Định nghĩa giao diện dịch vụ giỏ hàng ICartService.
    {
        Task<ApiResult<CartResponse>> GetMyCartAsync(Guid userId); // Khai báo phương thức lấy thông tin giỏ hàng của người dùng.
        Task<ApiResult<CartResponse>> AddToCartAsync(Guid userId, AddToCartRequest request); // Khai báo phương thức thêm sản phẩm vào giỏ hàng.
        Task<ApiResult<CartResponse>> UpdateQuantityAsync(Guid userId, int cartItemId, UpdateCartItemRequest request); // Khai báo phương thức cập nhật số lượng của sản phẩm trong giỏ hàng.
        Task<ApiResult<CartResponse>> RemoveItemAsync(Guid userId, int cartItemId); // Khai báo phương thức xóa một mục sản phẩm khỏi giỏ hàng.
        Task<ApiResult<bool>> ClearCartAsync(Guid userId); // Khai báo phương thức xóa sạch giỏ hàng (sau khi đặt hàng thành công).
    }
}

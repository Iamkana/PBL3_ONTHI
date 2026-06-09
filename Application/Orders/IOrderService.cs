using System; // Sử dụng các kiểu dữ liệu cơ bản của hệ thống .NET.
using System.Threading.Tasks; // Sử dụng các tác vụ bất đồng bộ Task.
using PBL3.Shared.DTOs.Sale; // Sử dụng DTO thuộc module bán hàng (Checkout, Order).
using PBL3.Shared.DTOs.Common; // Sử dụng DTO dùng chung ApiResult.
using PBL3.Shared.DTOs.Products; // Sử dụng DTO thuộc module sản phẩm.

namespace PBL3.Application.Orders // Khai báo namespace cho tầng Application của module đơn hàng.
{
    public interface IOrderService // Định nghĩa giao diện dịch vụ quản lý đơn hàng IOrderService.
    {
        Task<ApiResult<CheckoutResponse>> CheckoutAsync(CheckoutRequest request, Guid userId); // Khai báo phương thức tính toán đơn hàng (Checkout) trước khi thanh toán.
        Task<ApiResult<OrderDetailDto>> PlaceOrderAsync(CreateOrderRequest request, Guid userId); // Khai báo phương thức khởi tạo đặt hàng mới.
        Task<ApiResult<OrderDetailDto>> GetByIdAsync(int id); // Khai báo phương thức lấy thông tin chi tiết đơn hàng theo Id (Admin).
        Task<ApiResult<PagedResult<OrderSummaryResponse>>> GetPagedOrdersAsync(OrderFilterRequest request); // Khai báo phương thức lấy danh sách đơn hàng phân trang kèm bộ lọc (Admin).
        Task<ApiResult<PagedResult<OrderSummaryResponse>>> GetMyOrdersAsync(Guid userId, OrderFilterRequest request); // Khai báo phương thức lấy danh sách đơn hàng của khách hàng hiện tại.
        Task<ApiResult<bool>> CancelOrderAsync(int id, CancelOrderRequest request); // Khai báo phương thức hủy đơn hàng (Admin/Nhân viên).
        Task<ApiResult<bool>> CompleteOrderAsync(int id); // Khai báo phương thức hoàn tất giao dịch đơn hàng (Nhân viên).
        Task<ApiResult<bool>> ConfirmOrderAsync(int id); // Khai báo phương thức phê duyệt xác nhận đơn hàng (Nhân viên).

        // Customer self-service (enforce ownership)
        Task<ApiResult<OrderDetailDto>> GetMyOrderByIdAsync(int id, Guid userId); // Khai báo phương thức khách tự xem chi tiết đơn hàng của mình (Bảo mật ownership).
        Task<ApiResult<bool>> CancelMyOrderAsync(int id, Guid userId, string cancelReason); // Khai báo phương thức khách tự yêu cầu hủy đơn hàng của mình.
        Task<ApiResult<bool>> ConfirmReceivedByCustomerAsync(int id, Guid userId); // Khai báo phương thức khách hàng tự xác nhận đã nhận được hàng.
    }
}

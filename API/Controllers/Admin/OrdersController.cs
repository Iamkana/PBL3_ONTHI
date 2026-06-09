using System; // Sử dụng các kiểu dữ liệu cơ bản của hệ thống.
using System.Security.Claims; // Sử dụng Claims để trích xuất thông tin người dùng từ JWT.
using System.Threading.Tasks; // Sử dụng lập trình bất đồng bộ Task.
using Microsoft.AspNetCore.Authorization; // Sử dụng để phân quyền truy cập.
using Microsoft.AspNetCore.Mvc; // Sử dụng các thành phần Web API.
using PBL3.Application.Orders; // Sử dụng tầng dịch vụ quản lý đơn hàng IOrderService.
using PBL3.Shared.DTOs.Sale; // Sử dụng các DTO liên quan đến bán hàng/đơn hàng.
using PBL3.Shared.DTOs.Common; // Sử dụng các DTO dùng chung như ApiResult, PagedResult.
using PBL3.API.Extensions; // Sử dụng các phương thức mở rộng.

namespace PBL3.API.Controllers.Admin // Khai báo namespace cho Controllers quản lý đơn hàng.
{
    [Route("api/[controller]")] // Định nghĩa route truy cập: api/orders.
    [ApiController] // Khai báo lớp là một API Controller có cơ chế tự động kiểm tra model.
    [Authorize] // Yêu cầu người dùng phải xác thực đăng nhập để truy cập mọi endpoint trong controller này.
    public class OrdersController : ControllerBase // Định nghĩa lớp OrdersController kế thừa từ ControllerBase.
    {
        private readonly IOrderService _orderService; // Khai báo service quản lý đơn hàng.

        public OrdersController(IOrderService orderService) // Constructor injection tiêm IOrderService từ DI container.
        {
            _orderService = orderService; // Gán service được tiêm vào trường nội bộ.
        }

        [HttpPost("checkout")] // Định nghĩa HTTP POST Method thanh toán/đặt hàng (api/orders/checkout).
        [Authorize(Roles = "Customer")] // Chỉ cho phép người dùng có vai trò Customer thực hiện đặt hàng.
        public async Task<IActionResult> Checkout([FromBody] CheckoutRequest request) // Nhận thông tin đơn đặt hàng từ Request Body.
        {
            var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value; // Trích xuất UserId dạng chuỗi từ Claim NameIdentifier của JWT token.
            if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId)) // Kiểm tra xem UserId có hợp lệ không.
            {
                return Unauthorized(ApiResult<CheckoutResponse>.Fail("Không thể xác thực thông tin người dùng.")); // Trả về lỗi 401 Unauthorized nếu không hợp lệ.
            }

            try // Khối try bắt các lỗi runtime phát sinh khi checkout.
            {
                var result = await _orderService.CheckoutAsync(request, userId); // Gọi service thực hiện tạo đơn hàng atomic transaction.
                return result.ToActionResult(this); // Ánh xạ ApiResult sang HTTP Response tương ứng.
            }
            catch (Exception ex) // Bắt các lỗi ngoại lệ phát sinh.
            {
                return BadRequest(ApiResult<CheckoutResponse>.Fail(ex.Message)); // Trả về HTTP 400 BadRequest kèm thông báo lỗi.
            }
        }

        [HttpGet("my")] // Định nghĩa HTTP GET Method lấy đơn hàng cá nhân (api/orders/my).
        [Authorize(Roles = "Customer")] // Chỉ khách hàng mới được phép xem danh sách đơn của chính họ.
        public async Task<IActionResult> GetMyOrders([FromQuery] OrderFilterRequest request) // Lọc và xem danh sách đơn hàng cá nhân.
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; // Lấy UserId từ Claims.
            if (!Guid.TryParse(userIdStr, out var userId)) // Kiểm tra và ép kiểu sang Guid.
                return Unauthorized(ApiResult<PagedResult<OrderSummaryResponse>>.Fail("Không thể xác thực thông tin người dùng.")); // Trả về lỗi 401 Unauthorized nếu thất bại.
            var result = await _orderService.GetMyOrdersAsync(userId, request); // Gọi service lấy danh sách đơn hàng của khách hàng hiện tại.
            return result.ToActionResult(this); // Trả về kết quả qua extension ToActionResult.
        }

        [HttpGet("{id}")] // Định nghĩa HTTP GET Method lấy thông tin đơn hàng theo Id (api/orders/{id}).
        public async Task<IActionResult> GetById(int id) // Lấy thông tin đơn hàng theo mã đơn hàng.
        {
            if (User.IsInRole("Customer")) // Nếu người dùng đăng nhập mang vai trò Customer (Khách hàng).
            {
                var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; // Lấy UserId từ Claims của khách hàng.
                if (!Guid.TryParse(userIdStr, out var userId)) // Kiểm tra định dạng Guid.
                    return Unauthorized(ApiResult<OrderDetailDto>.Fail("Không thể xác thực thông tin người dùng.")); // Trả về lỗi 401 Unauthorized.

                var myResult = await _orderService.GetMyOrderByIdAsync(id, userId); // Gọi service lấy chi tiết đơn hàng của đúng khách hàng đó (bảo mật dữ liệu chéo).
                return myResult.ToActionResult(this); // Trả về kết quả qua extension ToActionResult.
            }

            var result = await _orderService.GetByIdAsync(id); // Nếu người dùng là Admin/Employee, gọi service lấy bất kỳ đơn hàng nào mà không cần lọc theo UserId.
            return result.ToActionResult(this); // Trả về kết quả qua extension ToActionResult.
        }

        [HttpPut("my/{id}/cancel")] // Định nghĩa HTTP PUT Method khách hàng tự hủy đơn (api/orders/my/{id}/cancel).
        [Authorize(Roles = "Customer")] // Chỉ khách hàng mới được tự hủy đơn hàng của họ.
        public async Task<IActionResult> CancelMyOrder(int id, [FromBody] CancelOrderRequest request) // Nhận lý do hủy từ body.
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; // Lấy UserId từ Claims.
            if (!Guid.TryParse(userIdStr, out var userId)) // Kiểm tra Guid.
                return Unauthorized(ApiResult<bool>.Fail("Không thể xác thực thông tin người dùng.")); // Trả về lỗi 401 Unauthorized.

            try // Bắt các lỗi nghiệp vụ khi hủy đơn.
            {
                var result = await _orderService.CancelMyOrderAsync(id, userId, request?.CancelReason ?? string.Empty); // Gọi service hủy đơn hàng của khách.
                return result.ToActionResult(this); // Trả về kết quả qua extension ToActionResult.
            }
            catch (Exception ex) // Bắt ngoại lệ.
            {
                return BadRequest(ApiResult<bool>.Fail(ex.Message)); // Trả về 400 BadRequest.
            }
        }

        [HttpPut("my/{id}/confirm-received")] // Định nghĩa HTTP PUT Method xác nhận đã nhận hàng (api/orders/my/{id}/confirm-received).
        [Authorize(Roles = "Customer")] // Chỉ khách hàng mới được gọi hành động này.
        public async Task<IActionResult> ConfirmReceived(int id) // Xác nhận nhận hàng thành công.
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; // Lấy UserId từ Claims.
            if (!Guid.TryParse(userIdStr, out var userId)) // Kiểm tra Guid.
                return Unauthorized(ApiResult<bool>.Fail("Không thể xác thực thông tin người dùng.")); // Trả về lỗi 401.

            try // Bắt lỗi nghiệp vụ.
            {
                var result = await _orderService.ConfirmReceivedByCustomerAsync(id, userId); // Gọi service xác nhận đã nhận được hàng.
                return result.ToActionResult(this); // Trả về kết quả qua extension ToActionResult.
            }
            catch (Exception ex) // Bắt lỗi ngoại lệ.
            {
                return BadRequest(ApiResult<bool>.Fail(ex.Message)); // Trả về 400 BadRequest.
            }
        }

        [HttpGet] // Định nghĩa HTTP GET Method lấy danh sách đơn hàng cho quản lý (api/orders).
        [Authorize(Roles = "Admin, Employee")] // Chỉ Admin hoặc nhân viên mới có quyền truy cập endpoint này.
        public async Task<IActionResult> GetPagedOrders([FromQuery] OrderFilterRequest request) // Lấy danh sách tất cả các đơn hàng phân trang theo bộ lọc.
        {
            var result = await _orderService.GetPagedOrdersAsync(request); // Gọi service lấy danh sách đơn hàng hệ thống phân trang.
            return result.ToActionResult(this); // Trả về kết quả qua extension ToActionResult.
        }

        [HttpPut("{id}/cancel")] // Định nghĩa HTTP PUT Method Admin/Nhân viên hủy đơn hàng (api/orders/{id}/cancel).
        [Authorize(Roles = "Admin, Employee")] // Yêu cầu vai trò Admin hoặc Employee.
        public async Task<IActionResult> CancelOrder(int id, [FromBody] CancelOrderRequest request) // Hủy đơn hàng từ phía quản lý.
        {
            try // Bắt lỗi nghiệp vụ.
            {
                var result = await _orderService.CancelOrderAsync(id, request); // Gọi service quản lý hủy đơn hàng (hoàn lại tồn kho).
                return result.ToActionResult(this); // Trả về kết quả.
            }
            catch (Exception ex) // Bắt ngoại lệ.
            {
                return BadRequest(ApiResult<bool>.Fail(ex.Message)); // Trả về 400 BadRequest.
            }
        }

        [HttpPut("{id}/complete")] // Định nghĩa HTTP PUT Method hoàn thành đơn hàng (api/orders/{id}/complete).
        [Authorize(Roles = "Admin, Employee")] // Yêu cầu quyền quản lý.
        public async Task<IActionResult> CompleteOrder(int id) // Xác nhận hoàn thành đơn hàng.
        {
            try // Bắt lỗi nghiệp vụ.
            {
                var result = await _orderService.CompleteOrderAsync(id); // Gọi service chuyển trạng thái đơn sang Đã hoàn thành (Đã thanh toán & Đã nhận).
                return result.ToActionResult(this); // Trả về kết quả.
            }
            catch (Exception ex) // Bắt ngoại lệ.
            {
                return BadRequest(ApiResult<bool>.Fail(ex.Message)); // Trả về 400 BadRequest.
            }
        }

        [HttpPut("{id}/confirm")] // Định nghĩa HTTP PUT Method xác nhận đơn hàng (api/orders/{id}/confirm).
        [Authorize(Roles = "Admin, Employee")] // Yêu cầu quyền quản lý.
        public async Task<IActionResult> ConfirmOrder(int id) // Xác nhận đơn hàng hợp lệ để chuẩn bị vận chuyển.
        {
            try // Bắt lỗi nghiệp vụ.
            {
                var result = await _orderService.ConfirmOrderAsync(id); // Gọi service xác nhận đơn hàng, cập nhật trạng thái đơn sang Đang xử lý.
                return result.ToActionResult(this); // Trả về kết quả.
            }
            catch (Exception ex) // Bắt ngoại lệ.
            {
                return BadRequest(ApiResult<bool>.Fail(ex.Message)); // Trả về 400 BadRequest.
            }
        }
    }
}

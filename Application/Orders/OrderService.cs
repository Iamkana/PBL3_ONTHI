using System; // Sử dụng các kiểu dữ liệu và lớp cơ bản của hệ thống .NET.
using System.Collections.Generic; // Sử dụng các kiểu cấu trúc dữ liệu danh sách như List, Dictionary.
using System.Linq; // Sử dụng các thư viện truy vấn dữ liệu LINQ.
using System.Threading.Tasks; // Sử dụng thư viện lập trình bất đồng bộ Task.
using Microsoft.EntityFrameworkCore; // Sử dụng thư viện Entity Framework Core.
using PBL3.Core.Entities; // Sử dụng các thực thể nghiệp vụ cốt lõi của tầng Core.
using PBL3.Core.Interfaces; // Sử dụng các interface đại diện cho repository và dịch vụ.
using PBL3.Shared.DTOs.Common; // Sử dụng các DTO dùng chung của hệ thống.
using PBL3.Shared.DTOs.Sale; // Sử dụng các DTO liên quan đến bán hàng và đơn hàng.
using PBL3.Shared.DTOs.Products; // Sử dụng các DTO liên quan đến sản phẩm.
using PBL3.Shared.Enums; // Sử dụng các enum định nghĩa trạng thái của hệ thống.

namespace PBL3.Application.Orders // Định nghĩa namespace cho lớp dịch vụ OrderService.
{
    public class OrderService( // Khai báo lớp OrderService sử dụng Primary Constructor.
        IUnitOfWork unitOfWork, // Tiêm UnitOfWork để quản lý Transaction dữ liệu.
        IOrderRepository orderRepo, // Tiêm repository quản lý đơn hàng.
        IVoucherRepository voucherRepo, // Tiêm repository quản lý mã giảm giá.
        IProductRepository productRepo, // Tiêm repository quản lý sản phẩm.
        ICartRepository cartRepo, // Tiêm repository quản lý giỏ hàng.
        IUserAddressRepository userAddressRepo, // Tiêm repository quản lý địa chỉ người dùng.
        IProductSerialRepository productSerialRepo) : IOrderService // Tiêm repository quản lý số serial và kế thừa IOrderService.
    {
        private readonly IUnitOfWork _unitOfWork = // Khai báo trường readonly để lưu trữ UnitOfWork.
            unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork)); // Gán giá trị và kiểm tra null.
        private readonly IOrderRepository _orderRepo = // Khai báo trường readonly để lưu trữ repo đơn hàng.
            orderRepo ?? throw new ArgumentNullException(nameof(orderRepo)); // Gán giá trị và kiểm tra null.
        private readonly IVoucherRepository _voucherRepo = // Khai báo trường readonly để lưu trữ repo voucher.
            voucherRepo ?? throw new ArgumentNullException(nameof(voucherRepo)); // Gán giá trị và kiểm tra null.
        private readonly IProductRepository _productRepo = // Khai báo trường readonly để lưu trữ repo sản phẩm.
            productRepo ?? throw new ArgumentNullException(nameof(productRepo)); // Gán giá trị và kiểm tra null.
        private readonly ICartRepository _cartRepo = // Khai báo trường readonly để lưu trữ repo giỏ hàng.
            cartRepo ?? throw new ArgumentNullException(nameof(cartRepo)); // Gán giá trị và kiểm tra null.
        private readonly IUserAddressRepository _userAddressRepo = // Khai báo trường readonly để lưu trữ repo địa chỉ.
            userAddressRepo ?? throw new ArgumentNullException(nameof(userAddressRepo)); // Gán giá trị và kiểm tra null.
        private readonly IProductSerialRepository _productSerialRepo = // Khai báo trường readonly để lưu trữ repo serial.
            productSerialRepo ?? throw new ArgumentNullException(nameof(productSerialRepo)); // Gán giá trị và kiểm tra null.

        public async Task<ApiResult<CheckoutResponse>> CheckoutAsync(CheckoutRequest request, Guid userId) // Phương thức thanh toán đơn hàng online.
        {
            var checkoutItems = new List<(int VariantId, int Quantity, decimal Price)>(); // Khởi tạo danh sách chứa các sản phẩm checkout (Biến thể, Số lượng, Giá).
            List<PBL3.Core.Entities.Cart>? cartsToRemove = null; // Khởi tạo danh sách các bản ghi giỏ hàng cần xóa sau khi đặt hàng thành công.

            if (request.IsBuyNow) // Nếu người dùng chọn "Mua ngay" trực tiếp từ trang chi tiết sản phẩm.
            {
                if (!request.BuyNowVariantId.HasValue || !request.BuyNowQuantity.HasValue || request.BuyNowQuantity.Value <= 0) // Kiểm tra tính hợp lệ của tham số mua ngay.
                {
                    return ApiResult<CheckoutResponse>.Fail("Thông tin mua ngay không hợp lệ."); // Trả về kết quả thất bại nếu thông tin trống hoặc số lượng <= 0.
                }

                var variant = await _productRepo.GetVariantByIdAsync(request.BuyNowVariantId.Value); // Lấy thông tin chi tiết biến thể sản phẩm từ repo.
                if (variant == null) // Nếu không tìm thấy biến thể sản phẩm.
                {
                    return ApiResult<CheckoutResponse>.Fail("Sản phẩm không tồn tại."); // Trả về lỗi không tồn tại sản phẩm.
                }

                checkoutItems.Add((variant.Id, request.BuyNowQuantity.Value, variant.Price)); // Thêm sản phẩm mua ngay vào danh sách thanh toán.
            }
            else // Nếu thanh toán từ giỏ hàng hiện có của người dùng.
            {
                var carts = await _cartRepo.GetCartItemsWithTrackingAsync(userId); // Lấy toàn bộ sản phẩm trong giỏ hàng kèm tracking trạng thái.
                if (!carts.Any()) // Nếu giỏ hàng của người dùng trống.
                {
                    return ApiResult<CheckoutResponse>.Fail("Giỏ hàng của bạn đang trống."); // Trả về lỗi thông báo giỏ hàng trống.
                }

                checkoutItems = carts.Select(c => (c.VariantId, c.Quantity, c.Variant.Price)).ToList(); // Ánh xạ danh sách giỏ hàng sang danh sách checkout.
                cartsToRemove = carts; // Gán danh sách giỏ hàng cần dọn dẹp sau khi lưu đơn thành công.
            }

            var address = await _userAddressRepo.GetByIdAsync(request.UserAddressId); // Lấy thông tin địa chỉ giao nhận của người dùng từ repo.
            if (address == null || address.UserId != userId) // Nếu địa chỉ không tồn tại hoặc không thuộc về người dùng hiện tại.
            {
                return ApiResult<CheckoutResponse>.Fail("Địa chỉ giao hàng không hợp lệ."); // Trả về lỗi địa chỉ không hợp lệ.
            }

            var variantIds = checkoutItems.Select(x => x.VariantId).Distinct().ToList(); // Lấy danh sách các Id biến thể sản phẩm không trùng lặp.
            
            var availableSerialsMap = await _productSerialRepo.CountAvailableByVariantIdsAsync(variantIds); // Lấy tổng số lượng số serial đang có sẵn trong kho của các biến thể.
            
            var activeOrderQuantitiesMap = await _orderRepo.GetActiveOrderQuantitiesByVariantIdsAsync(variantIds); // Lấy số lượng biến thể đang bị giữ chỗ trong các đơn hàng online chưa hoàn thành.

            foreach (var item in checkoutItems) // Lặp qua từng sản phẩm trong danh sách thanh toán để kiểm tra tồn kho ảo.
            {
                var availableSerials = availableSerialsMap.ContainsKey(item.VariantId) ? availableSerialsMap[item.VariantId] : 0; // Lấy số serial sẵn có trong kho của biến thể này.
                var reservedInOrders = activeOrderQuantitiesMap.ContainsKey(item.VariantId) ? activeOrderQuantitiesMap[item.VariantId] : 0; // Lấy số lượng bị giữ chỗ của biến thể này.
                var realAvailableStock = availableSerials - reservedInOrders; // Tính tồn kho ảo khả dụng = Sẵn có - Giữ chỗ.

                if (realAvailableStock < item.Quantity) // Nếu tồn kho ảo khả dụng nhỏ hơn số lượng khách đặt mua.
                {
                    return ApiResult<CheckoutResponse>.Fail($"Sản phẩm có mã {item.VariantId} hiện đã hết hàng hoặc không đủ số lượng (Khả dụng: {Math.Max(0, realAvailableStock)})."); // Báo lỗi hết hàng hoặc không đủ số lượng bán.
                }
            }

            decimal subTotal = checkoutItems.Sum(x => x.Price * x.Quantity); // Tính tổng tiền trước khi giảm giá của toàn bộ đơn hàng.

            var itemCategoryIds = (request.VoucherCodes != null && request.VoucherCodes.Any()) // Kiểm tra nếu có mã giảm giá thì mới truy vấn danh mục.
                ? await _productRepo.GetCategoryIdsByVariantIdsAsync(variantIds) // Lấy danh sách Id danh mục của các sản phẩm để kiểm tra điều kiện áp voucher.
                : null; // Ngược lại gán null.

            var (usages, totalDiscount) = await ApplyVouchersAsync( // Gọi hàm áp dụng và tính toán tiền giảm giá từ danh sách voucher.
                subTotal, request.VoucherCodes, userId, isOnlineOrder: true, itemCategoryIds); // Truyền tổng tiền, danh sách code, Id user, phân biệt online, danh mục.
            
            totalDiscount = Math.Min(totalDiscount, subTotal); // Đảm bảo tổng tiền giảm giá không vượt quá số tiền của sản phẩm.

            decimal totalAmount = subTotal + request.ShippingFee - totalDiscount; // Tính tổng số tiền cuối cùng của đơn hàng = Tiền hàng + Phí ship - Giảm giá.

            await _unitOfWork.BeginTransactionAsync(); // Mở một Transaction cơ sở dữ liệu mới thông qua Unit of Work để đảm bảo tính toàn vẹn dữ liệu.
            try // Khối lệnh try để bắt lỗi và rollback transaction nếu có sự cố.
            {
                string datePrefix = "ORD-" + DateTime.Now.ToString("yyyyMMdd"); // Tạo tiền tố mã đơn hàng theo ngày hiện tại ORD-YYYYMMDD.
                string? lastCode = await _orderRepo.GetLastOrderCodeByDateAsync(datePrefix); // Lấy mã đơn hàng cuối cùng trong ngày để làm căn cứ tăng số thứ tự.
                int nextIndex = 1; // Khởi tạo số thứ tự đơn hàng tiếp theo mặc định là 1.
                if (!string.IsNullOrEmpty(lastCode)) // Nếu trong ngày đã tồn tại đơn hàng trước đó.
                {
                    string suffix = lastCode.Substring(lastCode.LastIndexOf('-') + 1); // Cắt lấy phần số thứ tự ở đuôi mã đơn hàng.
                    if (int.TryParse(suffix, out int lastIndex)) // Chuyển đổi phần số thứ tự sang kiểu số nguyên.
                    {
                        nextIndex = lastIndex + 1; // Tăng số thứ tự đơn hàng tiếp theo lên 1 đơn vị.
                    }
                }
                string newOrderCode = $"{datePrefix}-{nextIndex:D3}"; // Tạo mã đơn hàng hoàn chỉnh với số thứ tự định dạng 3 chữ số (ví dụ: ORD-20260521-001).

                byte orderStatus = (byte)OrderStatus.Pending; // Gán trạng thái mặc định của đơn hàng online mới tạo là Pending (Chờ duyệt).

                var order = new Order // Khởi tạo thực thể Order mới.
                {
                    OrderCode = newOrderCode, // Gán mã đơn hàng mới.
                    UserId = userId, // Gán Id người dùng đặt hàng.
                    OrderDate = DateTime.UtcNow, // Gán ngày đặt hàng theo giờ UTC hiện tại.
                    Status = orderStatus, // Gán trạng thái đơn hàng.
                    SubTotal = subTotal, // Gán tổng tiền hàng.
                    ShippingFee = request.ShippingFee, // Gán phí vận chuyển.
                    DiscountAmount = totalDiscount, // Gán số tiền giảm giá.
                    TotalAmount = totalAmount, // Gán tổng tiền phải trả.
                    ShipName = address.ReceiverName, // Gán tên người nhận hàng.
                    ShipPhone = address.PhoneNumber, // Gán số điện thoại người nhận.
                    ShipAddress = address.AddressLine, // Gán địa chỉ giao nhận chi tiết.
                    ShipCity = address.City, // Gán thành phố giao nhận.
                    PaymentMethod = request.PaymentMethod, // Gán phương thức thanh toán (COD hoặc Online).
                    PaymentStatus = (byte)(request.PaymentMethod == 0 ? 0 : 1), // Gán trạng thái thanh toán (0: Chưa trả đối với COD, 1: Đã trả đối với Online).
                    OrderType = 0, // Gán loại đơn hàng (0: Đơn đặt hàng Online).
                    Note = request.Note // Gán ghi chú đơn hàng của khách.
                };

                await _orderRepo.AddAsync(order); // Thêm bản ghi đơn hàng mới vào DB context.
                await _unitOfWork.SaveChangesAsync(); // Lưu thay đổi tạm thời để database tự động phát sinh Id khóa chính cho đơn hàng.

                foreach (var item in checkoutItems) // Lặp qua danh sách sản phẩm để thêm vào bảng chi tiết đơn hàng (OrderDetail).
                {
                    order.OrderDetails.Add(new OrderDetail // Khởi tạo và thêm mới thực thể OrderDetail vào đơn hàng.
                    {
                        OrderId = order.Id, // Gán Id đơn hàng vừa sinh ra.
                        VariantId = item.VariantId, // Gán Id biến thể sản phẩm.
                        Quantity = item.Quantity, // Gán số lượng mua.
                        UnitPrice = item.Price // Gán đơn giá tại thời điểm mua.
                    });
                }

                foreach (var usage in usages) // Lặp qua danh sách lịch sử sử dụng voucher để liên kết với đơn hàng mới tạo.
                {
                    usage.OrderId = order.Id; // Gán Id đơn hàng vừa tạo cho bản ghi lịch sử dùng voucher.
                }
                if (usages.Any()) // Nếu đơn hàng này có sử dụng mã giảm giá.
                {
                    await _voucherRepo.AddUsagesAsync(usages); // Thêm danh sách lịch sử sử dụng voucher vào cơ sở dữ liệu.
                    
                    if (request.VoucherCodes != null && request.VoucherCodes.Any()) // Nếu danh sách mã voucher yêu cầu không trống.
                    {
                        var vouchersToUpdate = await _voucherRepo.GetByCodesAsync(request.VoucherCodes); // Lấy thông tin các thực thể voucher tương ứng từ database.
                        foreach (var voucher in vouchersToUpdate) // Lặp qua từng voucher để cập nhật số lượt sử dụng.
                        {
                            voucher.UsedCount += 1; // Tăng số lượt đã sử dụng của voucher lên 1 đơn vị.
                        }
                    }
                }

                if (!request.IsBuyNow && cartsToRemove != null) // Nếu đặt hàng từ giỏ hàng và danh sách giỏ hàng cần xóa có dữ liệu.
                {
                    _cartRepo.RemoveRange(cartsToRemove); // Xóa các sản phẩm tương ứng trong giỏ hàng của người dùng khỏi DB.
                }

                await _unitOfWork.SaveChangesAsync(); // Lưu toàn bộ các thay đổi (Chi tiết đơn hàng, lịch sử voucher, giỏ hàng) xuống DB.
                await _unitOfWork.CommitAsync(); // Xác nhận (Commit) Transaction hoàn thành giao dịch an toàn.

                var response = new CheckoutResponse // Khởi tạo đối tượng phản hồi kết quả thanh toán.
                {
                    OrderId = order.Id, // Gán Id đơn hàng mới.
                    OrderCode = order.OrderCode, // Gán mã đơn hàng.
                    TotalAmount = order.TotalAmount, // Gán tổng tiền thanh toán.
                    Status = order.Status, // Gán trạng thái đơn.
                    PaymentMethod = order.PaymentMethod, // Gán phương thức thanh toán.
                    PaymentUrl = null // Link thanh toán VNPay/Momo sẽ được tích hợp sau.
                };

                return ApiResult<CheckoutResponse>.Ok(response, "Đặt hàng thành công!"); // Trả về kết quả thành công kèm dữ liệu phản hồi.
            }
            catch (Exception ex) // Bắt bất kỳ lỗi ngoại lệ nào xảy ra trong quá trình xử lý.
            {
                await _unitOfWork.RollbackAsync(); // Hủy bỏ (Rollback) toàn bộ các thao tác ghi dữ liệu trước đó trong transaction để tránh dữ liệu rác.
                throw new Exception("Lỗi hệ thống khi đặt hàng: " + ex.Message, ex); // Ném ra ngoại lệ thông báo lỗi hệ thống kèm chi tiết.
            }
        }

        public async Task<ApiResult<OrderDetailDto>> PlaceOrderAsync(CreateOrderRequest request, Guid userId) // Phương thức đặt hàng thay thế (dùng cho luồng khác/Admin).
        {
            decimal subTotal = 0; // Khởi tạo tổng tiền hàng bằng 0.
            var variantPrices = new Dictionary<int, decimal>(); // Khởi tạo từ điển lưu trữ giá của các biến thể sản phẩm.

            foreach (var item in request.Items) // Lặp qua danh sách sản phẩm yêu cầu để tính tiền và lấy đơn giá.
            {
                var variant = await _productRepo.GetVariantByIdAsync(item.VariantId); // Truy vấn biến thể sản phẩm theo Id.
                if (variant == null) // Nếu không tìm thấy sản phẩm.
                {
                    return ApiResult<OrderDetailDto>.Fail($"Không tìm thấy sản phẩm có mã {item.VariantId}", ApiErrorCode.NotFound); // Trả về lỗi NotFound.
                }
                
                variantPrices[item.VariantId] = variant.Price; // Lưu đơn giá biến thể sản phẩm vào từ điển.
                subTotal += variant.Price * item.Quantity; // Cộng dồn thành tiền = đơn giá * số lượng vào tổng tiền hàng.
            }

            decimal shippingFee = 30000; // Gán phí vận chuyển mặc định cố định là 30,000 VNĐ.

            var placeOrderVariantIds = request.Items.Select(i => i.VariantId).Distinct().ToList(); // Lấy danh sách các Id biến thể sản phẩm không trùng lặp.
            var placeOrderCategoryIds = (request.VoucherCodes != null && request.VoucherCodes.Any()) // Nếu có áp dụng mã voucher.
                ? await _productRepo.GetCategoryIdsByVariantIdsAsync(placeOrderVariantIds) // Lấy danh sách Id danh mục của các sản phẩm tương ứng.
                : null; // Ngược lại gán null.

            var (usages, totalDiscount) = await ApplyVouchersAsync( // Gọi hàm áp dụng voucher và tính số tiền chiết khấu.
                subTotal, request.VoucherCodes, userId, isOnlineOrder: true, placeOrderCategoryIds); // Truyền các tham số tương ứng.

            totalDiscount = Math.Min(totalDiscount, subTotal); // Giới hạn số tiền giảm giá tối đa bằng tổng tiền hàng.

            await _unitOfWork.BeginTransactionAsync(); // Mở transaction mới qua Unit of Work để ghi nhận đơn hàng.
            try // Bắt đầu khối kiểm soát lỗi.
            {
                string datePrefix = "ORD-" + DateTime.Now.ToString("yyyyMMdd"); // Định dạng tiền tố mã đơn hàng theo ngày.
                string? lastCode = await _orderRepo.GetLastOrderCodeByDateAsync(datePrefix); // Lấy mã đơn hàng cuối cùng trong ngày từ repo.
                int nextIndex = 1; // Khởi tạo số thứ tự mặc định là 1.
                if (!string.IsNullOrEmpty(lastCode)) // Nếu đã có đơn đặt trước đó trong ngày.
                {
                    string suffix = lastCode.Substring(lastCode.LastIndexOf('-') + 1); // Cắt lấy số thứ tự ở cuối mã đơn.
                    if (int.TryParse(suffix, out int lastIndex)) // Chuyển đổi số thứ tự sang số nguyên.
                    {
                        nextIndex = lastIndex + 1; // Tăng số thứ tự tiếp theo lên 1.
                    }
                }
                string newOrderCode = $"{datePrefix}-{nextIndex:D3}"; // Tạo mã đơn hàng mới dạng ORD-YYYYMMDD-XXX.

                var order = new Order // Khởi tạo đối tượng thực thể Order.
                {
                    OrderCode = newOrderCode, // Gán mã đơn hàng.
                    UserId = userId, // Gán Id người dùng.
                    OrderDate = DateTime.UtcNow, // Gán thời gian tạo theo giờ UTC.
                    Status = 0, // Đặt trạng thái mặc định là 0 (Pending - Chờ duyệt).
                    SubTotal = subTotal, // Gán tổng tiền hàng trước giảm.
                    ShippingFee = shippingFee, // Gán phí vận chuyển.
                    DiscountAmount = totalDiscount, // Gán tiền được giảm.
                    TotalAmount = subTotal + shippingFee - totalDiscount, // Tính tổng số tiền cuối cùng cần trả.
                    ShipName = request.ShipName, // Gán tên người nhận.
                    ShipPhone = request.ShipPhone, // Gán số điện thoại nhận.
                    ShipAddress = request.ShipAddress, // Gán địa chỉ nhận.
                    ShipCity = request.ShipCity, // Gán thành phố nhận.
                    PaymentMethod = request.PaymentMethod, // Gán phương thức thanh toán.
                    PaymentStatus = (byte)(request.PaymentMethod == 0 ? 0 : 1), // Gán trạng thái thanh toán tương ứng.
                    Note = request.Note // Gán ghi chú đơn hàng.
                };
                
                await _orderRepo.AddAsync(order); // Thêm bản ghi đơn hàng mới vào repo.
                await _unitOfWork.SaveChangesAsync(); // Lưu tạm để database phát sinh Id khóa chính cho đơn hàng.

                foreach (var item in request.Items) // Lặp qua danh sách sản phẩm để lưu chi tiết đơn hàng.
                {
                    order.OrderDetails.Add(new OrderDetail // Khởi tạo và thêm mới thực thể OrderDetail.
                    {
                        OrderId = order.Id, // Gán Id đơn hàng vừa tạo.
                        VariantId = item.VariantId, // Gán Id biến thể sản phẩm.
                        Quantity = item.Quantity, // Gán số lượng đặt mua.
                        UnitPrice = variantPrices[item.VariantId] // Lấy và gán đơn giá đã lưu từ trước.
                    });
                }

                foreach (var usage in usages) // Lặp qua lịch sử sử dụng voucher.
                {
                    usage.OrderId = order.Id; // Liên kết lịch sử sử dụng voucher với Id đơn hàng vừa sinh.
                }
                await _voucherRepo.AddUsagesAsync(usages); // Thêm các bản ghi sử dụng voucher vào DB.

                if (request.VoucherCodes != null && request.VoucherCodes.Any()) // Nếu đơn hàng có sử dụng mã giảm giá.
                {
                    var vouchers = await _voucherRepo.GetByCodesAsync(request.VoucherCodes); // Lấy các đối tượng voucher từ cơ sở dữ liệu.
                    foreach (var voucher in vouchers) // Lặp qua từng voucher để cập nhật số lần sử dụng.
                    {
                        voucher.UsedCount += 1; // Tăng số lần đã sử dụng của voucher lên 1 đơn vị.
                    }
                }

                await _unitOfWork.SaveChangesAsync(); // Lưu các thay đổi xuống cơ sở dữ liệu.
                await _unitOfWork.CommitAsync(); // Xác nhận (Commit) transaction thành công.

                var savedOrderInfo = await _orderRepo.GetByIdWithDetailsAsync(order.Id); // Tải lại thông tin đơn hàng đầy đủ kèm các quan hệ chi tiết từ DB.
                var dto = MapToOrderDetailDto(savedOrderInfo); // Ánh xạ thông tin đơn hàng sang OrderDetailDto để hiển thị cho client.

                return ApiResult<OrderDetailDto>.Ok(dto, "Đặt hàng thành công!"); // Trả về kết quả thành công kèm DTO chi tiết đơn hàng.
            }
            catch (Exception ex) // Bắt lỗi ngoại lệ nếu có sự cố xảy ra.
            {
                await _unitOfWork.RollbackAsync(); // Thu hồi (Rollback) các thay đổi dữ liệu trong transaction để bảo toàn dữ liệu DB.
                throw new Exception("Lỗi khi tạo đơn hàng: " + ex.Message, ex); // Ném ra ngoại lệ lỗi hệ thống kèm chi tiết.
            }
        }

        private async Task<(List<VoucherUsage> Usages, decimal TotalDiscount)> ApplyVouchersAsync( // Định nghĩa phương thức phụ trợ kiểm tra và áp dụng voucher cho đơn hàng.
            decimal subTotal, // Tham số nhận tổng tiền hàng trước giảm giá.
            List<string>? voucherCodes, // Tham số nhận danh sách mã giảm giá do người dùng nhập.
            Guid userId, // Tham số nhận Id người dùng đang thực hiện giao dịch.
            bool isOnlineOrder, // Tham số kiểu bool phân biệt đơn hàng Online hay tại quầy POS.
            List<int>? orderItemCategoryIds = null) // Danh sách Id danh mục các sản phẩm trong đơn (mặc định null).
        {
            var usages = new List<VoucherUsage>(); // Khởi tạo danh sách lưu trữ lịch sử sử dụng voucher.
            if (voucherCodes == null || !voucherCodes.Any()) // Nếu không có mã giảm giá nào được nhập.
                return (usages, 0); // Trả về danh sách trống và số tiền giảm giá bằng 0.

            var vouchers = await _voucherRepo.GetByCodesWithCategoriesAsync(voucherCodes); // Truy vấn thông tin chi tiết các voucher kèm danh mục áp dụng tương ứng từ database.

            var foundCodes = vouchers.Select(v => v.Code).ToHashSet(StringComparer.OrdinalIgnoreCase); // Tạo tập hợp hash các mã voucher hợp lệ tìm được trong DB (không phân biệt chữ hoa/thường).
            var invalidCodes = voucherCodes.Where(c => !foundCodes.Contains(c)).ToList(); // Lọc ra các mã voucher người dùng nhập nhưng không tìm thấy trong database.
            if (invalidCodes.Any()) // Nếu tồn tại mã giảm giá không hợp lệ.
                throw new Exception($"Mã giảm giá không tồn tại: {string.Join(", ", invalidCodes)}"); // Ném ra ngoại lệ báo lỗi mã giảm giá không tồn tại.

            if (vouchers.Count > 1 && vouchers.Any(v => !v.IsStackable)) // Nếu người dùng nhập nhiều hơn 1 mã voucher nhưng có mã không cho phép cộng dồn chéo.
                throw new Exception("Một hoặc nhiều mã giảm giá không thể được sử dụng cùng lúc với mã khác."); // Ném ra lỗi ngăn chặn hành vi cộng dồn voucher không hợp lệ.

            var now = DateTime.UtcNow; // Lấy thời gian hiện tại theo giờ UTC để kiểm tra hạn sử dụng.
            foreach (var voucher in vouchers) // Lặp qua từng voucher để thực hiện 6 bước kiểm tra điều kiện áp dụng.
            {
                if (!voucher.IsActive) // 1. Kiểm tra trạng thái hoạt động của Voucher.
                    throw new Exception($"Mã '{voucher.Code}' đã bị vô hiệu hóa."); // Ném ra lỗi nếu voucher đã bị tắt kích hoạt.

                if (now < voucher.StartDate || now > voucher.EndDate) // 2. Kiểm tra thời hạn hiệu lực của Voucher.
                    throw new Exception($"Mã '{voucher.Code}' đã hết hạn hoặc chưa đến thời gian sử dụng."); // Ném ra lỗi nếu nằm ngoài thời gian cho phép.

                if (voucher.Quantity.HasValue && voucher.UsedCount >= voucher.Quantity.Value) // 3. Kiểm tra giới hạn số lượng phát hành của hệ thống.
                    throw new Exception($"Mã '{voucher.Code}' đã hết lượt sử dụng."); // Ném ra lỗi nếu voucher đã dùng hết số lượt phát hành.

                if (subTotal < voucher.MinOrderValue) // 4. Kiểm tra giá trị đơn hàng tối thiểu để được áp dụng mã.
                    throw new Exception( // Ném ra lỗi kèm chi tiết số tiền tối thiểu cần thiết để áp dụng.
                        $"Mã '{voucher.Code}' yêu cầu đơn hàng tối thiểu {voucher.MinOrderValue:#,0}đ " + // Thông báo yêu cầu tối thiểu.
                        $"(đơn hiện tại: {subTotal:#,0}đ)."); // Thông báo số tiền đơn hàng hiện tại.

                if (isOnlineOrder && voucher.ApplyFor == 2) // 5. Kiểm tra kênh áp dụng: Nếu đơn online nhưng voucher chỉ dùng tại POS (ApplyFor = 2).
                    throw new Exception($"Mã '{voucher.Code}' chỉ áp dụng tại quầy, không áp dụng cho đơn online."); // Ném ra lỗi không đúng kênh áp dụng.
                if (!isOnlineOrder && voucher.ApplyFor == 1) // Nếu đơn tại POS nhưng voucher chỉ dùng cho đơn Online (ApplyFor = 1).
                    throw new Exception($"Mã '{voucher.Code}' chỉ áp dụng cho đơn online, không áp dụng tại quầy."); // Ném ra lỗi không đúng kênh áp dụng.

                if (voucher.VoucherCategories.Any() && orderItemCategoryIds != null && orderItemCategoryIds.Any()) // 6. Kiểm tra danh mục sản phẩm được áp dụng.
                {
                    var voucherCategoryIds = voucher.VoucherCategories.Select(vc => vc.CategoryId).ToHashSet(); // Lấy tập hợp Id danh mục được áp dụng của voucher này.
                    if (!orderItemCategoryIds.Any(catId => voucherCategoryIds.Contains(catId))) // Nếu không có bất kỳ sản phẩm nào trong đơn thuộc danh mục áp dụng của voucher.
                        throw new Exception($"Mã '{voucher.Code}' không áp dụng cho danh mục sản phẩm trong đơn hàng này."); // Ném ra lỗi không thỏa điều kiện danh mục.
                }
            }

            var voucherIds = vouchers.Select(v => v.Id).ToList(); // Lấy danh sách Id của các voucher đang xét.
            var usageCounts = await _voucherRepo.GetUserVoucherUsageCountsAsync(userId, voucherIds); // Lấy số lần người dùng hiện tại đã sử dụng các voucher này trong quá khứ.
            foreach (var voucher in vouchers) // Lặp qua để thực hiện kiểm tra giới hạn sử dụng của khách hàng.
            {
                var currentCount = usageCounts.GetValueOrDefault(voucher.Id, 0); // Lấy số lần đã dùng của user đối với voucher này, mặc định là 0.
                if (voucher.MaxUsesPerUser.HasValue && currentCount >= voucher.MaxUsesPerUser.Value) // 7. Kiểm tra giới hạn số lần sử dụng tối đa của cá nhân khách hàng.
                    throw new Exception( // Ném ra lỗi nếu vượt quá giới hạn cho phép sử dụng của cá nhân.
                        $"Bạn đã sử dụng mã '{voucher.Code}' {currentCount} lần " + // Thông báo số lần đã dùng.
                        $"(tối đa {voucher.MaxUsesPerUser} lần/khách)."); // Thông báo số lần tối đa.

                if (!voucher.MaxUsesPerUser.HasValue && !voucher.IsStackable && currentCount >= 1) // Hỗ trợ tương thích ngược: nếu không cho cộng dồn và không cấu hình số lần dùng tối đa thì mặc định dùng tối đa 1 lần.
                    throw new Exception( // Ném ra lỗi.
                        $"Bạn đã sử dụng mã giảm giá '{voucher.Code}'. Mỗi mã chỉ được sử dụng 1 lần."); // Thông báo mỗi mã dùng tối đa 1 lần.
            }

            decimal totalDiscount = 0; // Khởi tạo tổng tiền giảm giá của đơn hàng bằng 0.

            foreach (var voucher in vouchers) // 8. TÍNH TOÁN SỐ TIỀN GIẢM GIÁ CHO TỪNG VOUCHER.
            {
                decimal discountApplied; // Khai báo biến lưu số tiền giảm giá của voucher hiện tại.

                if (voucher.DiscountType == 0) // Loại 0: Giảm tiền cố định (Ví dụ: Giảm trực tiếp 50,000đ).
                {
                    discountApplied = voucher.DiscountValue; // Số tiền giảm bằng chính giá trị định nghĩa của voucher.
                }
                else // Loại 1: Giảm theo tỷ lệ phần trăm (Ví dụ: Giảm 10%, tối đa 200,000đ).
                {
                    discountApplied = subTotal * voucher.DiscountValue / 100; // Tính tiền giảm = tổng tiền hàng * phần trăm giảm.
                    if (voucher.MaxDiscountAmount.HasValue && discountApplied > voucher.MaxDiscountAmount.Value) // Nếu tiền giảm vượt quá mức trần tối đa cấu hình cho phép.
                        discountApplied = voucher.MaxDiscountAmount.Value; // Giới hạn tiền giảm bằng mức trần tối đa.
                }

                totalDiscount += discountApplied; // Cộng dồn tiền giảm vào tổng tiền giảm giá chung của đơn hàng.

                usages.Add(new VoucherUsage // Khởi tạo bản ghi lịch sử sử dụng voucher và thêm vào danh sách usages.
                {
                    VoucherId       = voucher.Id, // Gán Id voucher.
                    UserId          = userId, // Gán Id người dùng.
                    DiscountApplied = discountApplied, // Gán số tiền được chiết khấu từ voucher này.
                    UsedDate        = DateTime.UtcNow // Gán thời gian sử dụng là giờ UTC hiện tại.
                });
            }

            return (usages, totalDiscount); // Trả về bộ kết quả gồm danh sách sử dụng voucher và tổng tiền giảm giá.
        }

        public async Task<ApiResult<OrderDetailDto>> GetByIdAsync(int id) // Định nghĩa phương thức lấy thông tin đơn hàng theo Id cho Admin.
        {
            var order = await _orderRepo.GetByIdWithDetailsAsync(id); // Truy vấn thông tin đơn hàng kèm chi tiết sản phẩm và voucher từ repo.
            if (order == null) // Nếu không tìm thấy đơn hàng.
                return ApiResult<OrderDetailDto>.Fail("Không tìm thấy đơn hàng.", ApiErrorCode.NotFound); // Trả về lỗi NotFound.

            var dto = MapToOrderDetailDto(order); // Ánh xạ thực thể đơn hàng sang DTO chi tiết.
            return ApiResult<OrderDetailDto>.Ok(dto); // Trả về kết quả thành công chứa DTO đơn hàng.
        }

        public async Task<ApiResult<PagedResult<OrderSummaryResponse>>> GetPagedOrdersAsync(OrderFilterRequest request) // Phương thức lấy danh sách đơn hàng phân trang cho Admin/Nhân viên.
        {
            var query = _orderRepo.GetQueryable().AsNoTracking(); // Lấy truy vấn truy xuất danh sách đơn hàng không theo dõi trạng thái (AsNoTracking) để tăng hiệu năng.

            if (!string.IsNullOrEmpty(request.Keyword)) // Lọc theo từ khóa (Mã đơn hàng, tên hoặc SĐT khách nhận).
            {
                var lowerKeyword = request.Keyword.ToLower(); // Chuyển từ khóa sang chữ thường để so sánh không phân biệt hoa thường.
                query = query.Where(o => o.OrderCode.ToLower().Contains(lowerKeyword) || // Lọc theo mã đơn hàng.
                                         o.ShipName.ToLower().Contains(lowerKeyword) || // Lọc theo tên người nhận.
                                         o.ShipPhone.Contains(request.Keyword)); // Lọc theo số điện thoại nhận hàng.
            }

            if (request.Status.HasValue) // Lọc theo trạng thái đơn hàng cụ thể (nếu có).
            {
                query = query.Where(o => o.Status == request.Status.Value); // Thêm điều kiện so khớp trạng thái đơn hàng.
            }

            if (request.MinStatus.HasValue) // Lọc theo giới hạn trạng thái tối thiểu (nếu có).
                query = query.Where(o => o.Status >= request.MinStatus.Value); // Thêm điều kiện trạng thái >= giá trị tối thiểu.
            if (request.MaxStatus.HasValue) // Lọc theo giới hạn trạng thái tối đa (nếu có).
                query = query.Where(o => o.Status <= request.MaxStatus.Value); // Thêm điều kiện trạng thái <= giá trị tối đa.

            if (request.FromDate.HasValue) // Lọc theo mốc thời gian bắt đầu (nếu có).
            {
                query = query.Where(o => o.OrderDate >= request.FromDate.Value); // Thêm điều kiện ngày đặt >= ngày bắt đầu.
            }

            if (request.ToDate.HasValue) // Lọc theo mốc thời gian kết thúc (nếu có).
            {
                query = query.Where(o => o.OrderDate <= request.ToDate.Value); // Thêm điều kiện ngày đặt <= ngày kết thúc.
            }

            int totalCount = await query.CountAsync(); // Thực thi đếm tổng số lượng bản ghi thỏa mãn điều kiện lọc dưới database bất đồng bộ.

            var items = await query // Lấy danh sách kết quả sau khi phân trang.
                .OrderByDescending(o => o.OrderDate) // Sắp xếp danh sách đơn hàng mới nhất lên đầu.
                .Skip((request.PageIndex - 1) * request.PageSize) // Bỏ qua số lượng bản ghi của các trang trước đó.
                .Take(request.PageSize) // Lấy số lượng bản ghi tương ứng với kích thước trang.
                .Select(o => new OrderSummaryResponse // Ánh xạ sang DTO gọn để hiển thị danh sách tổng hợp.
                {
                    Id = o.Id, // Gán Id đơn hàng.
                    OrderCode = o.OrderCode, // Gán mã đơn hàng.
                    CustomerName = o.ShipName, // Gán tên người nhận.
                    CustomerPhone = o.ShipPhone, // Gán SĐT nhận.
                    CustomerAvatarUrl = o.User != null ? o.User.Profile!.AvatarUrl : null, // Gán ảnh đại diện khách đặt hàng nếu có.
                    TotalAmount = o.TotalAmount, // Gán tổng tiền thanh toán đơn hàng.
                    CreatedDate = o.OrderDate, // Gán ngày tạo đơn.
                    Status = o.Status, // Gán trạng thái đơn hàng.
                    PaymentStatus = o.PaymentStatus // Gán trạng thái thanh toán.
                })
                .ToListAsync(); // Thực thi truy vấn và chuyển kết quả thành List bất đồng bộ.

            var result = new PagedResult<OrderSummaryResponse> // Khởi tạo DTO phân trang kết quả.
            {
                Items = items, // Gán danh sách đơn hàng.
                TotalCount = totalCount, // Gán tổng số lượng.
                PageSize = request.PageSize, // Gán kích thước trang.
                PageNumber = request.PageIndex // Gán chỉ số trang hiện tại.
            };

            return ApiResult<PagedResult<OrderSummaryResponse>>.Ok(result); // Trả về kết quả thành công kèm dữ liệu phân trang.
        }

        public async Task<ApiResult<PagedResult<OrderSummaryResponse>>> GetMyOrdersAsync(Guid userId, OrderFilterRequest request) // Phương thức lấy lịch sử đơn hàng của cá nhân khách hàng.
        {
            var query = _orderRepo.GetQueryable().AsNoTracking() // Truy vấn danh sách đơn hàng không theo dõi trạng thái (AsNoTracking).
                .Where(o => o.UserId == userId); // Thêm điều kiện lọc bắt buộc chỉ lấy đơn hàng thuộc về UserId hiện tại.

            if (request.Status.HasValue) // Lọc theo trạng thái đơn hàng (nếu có).
                query = query.Where(o => o.Status == request.Status.Value); // Thêm điều kiện so khớp trạng thái.

            int totalCount = await query.CountAsync(); // Đếm tổng số đơn hàng của khách hàng thỏa điều kiện lọc.

            var items = await query // Lấy danh sách kết quả sau khi phân trang.
                .OrderByDescending(o => o.OrderDate) // Sắp xếp đơn hàng mới nhất lên đầu.
                .Skip((request.PageIndex - 1) * request.PageSize) // Bỏ qua bản ghi của các trang trước.
                .Take(request.PageSize) // Lấy số lượng bản ghi của trang hiện tại.
                .Select(o => new OrderSummaryResponse // Ánh xạ sang DTO tổng quan đơn hàng.
                {
                    Id = o.Id, // Gán Id đơn hàng.
                    OrderCode = o.OrderCode, // Gán mã đơn hàng.
                    CustomerName = o.ShipName, // Gán tên người nhận.
                    CustomerPhone = o.ShipPhone, // Gán SĐT nhận.
                    CustomerAvatarUrl = o.User != null ? o.User.Profile!.AvatarUrl : null, // Gán link ảnh đại diện.
                    TotalAmount = o.TotalAmount, // Gán tổng số tiền.
                    CreatedDate = o.OrderDate, // Gán ngày đặt hàng.
                    Status = o.Status, // Gán trạng thái đơn.
                    PaymentStatus = o.PaymentStatus // Gán trạng thái thanh toán.
                })
                .ToListAsync(); // Chuyển kết quả sang List bất đồng bộ.

            var result = new PagedResult<OrderSummaryResponse> // Khởi tạo đối tượng phân trang kết quả.
            {
                Items = items, // Gán danh sách đơn hàng.
                TotalCount = totalCount, // Gán tổng số đơn.
                PageSize = request.PageSize, // Gán kích thước trang.
                PageNumber = request.PageIndex // Gán số trang hiện tại.
            };

            return ApiResult<PagedResult<OrderSummaryResponse>>.Ok(result); // Trả về kết quả thành công kèm DTO phân trang.
        }

        public async Task<ApiResult<bool>> CancelOrderAsync(int id, CancelOrderRequest request) // Phương thức hủy đơn hàng (dành cho Admin/Nhân viên).
        {
            var order = await _orderRepo.GetByIdAsync(id); // Lấy thông tin đơn hàng theo Id.
            if (order == null) // Nếu không tìm thấy đơn hàng.
            {
                return ApiResult<bool>.Fail("Không tìm thấy đơn hàng.", ApiErrorCode.NotFound); // Trả về lỗi NotFound.
            }

            if (order.Status == 2) // Kiểm tra nếu đơn hàng đang trong trạng thái "Đang giao" (Shipping / Status = 2).
            {
                throw new Exception("Đơn hàng đang giao (Shipping). Tuyệt đối cấm hủy."); // Ném ra ngoại lệ ngăn chặn hành vi hủy đơn hàng đang vận chuyển.
            }

            if (order.Status != 0 && order.Status != 1) // Chỉ cho phép hủy đơn khi đơn hàng ở trạng thái Chờ duyệt (0) hoặc Đã xác nhận (1).
            {
                return ApiResult<bool>.Fail($"Không thể hủy đơn hàng ở trạng thái hiện tại."); // Trả về lỗi nếu trạng thái đơn không hợp lệ để hủy.
            }

            order.Status = 4; // Cập nhật trạng thái đơn hàng thành 4 (Cancelled - Đã hủy).
            order.CancelReason = request.CancelReason; // Ghi nhận lý do hủy đơn hàng từ yêu cầu.

            await _unitOfWork.SaveChangesAsync(); // Lưu thay đổi trạng thái và lý do hủy vào cơ sở dữ liệu.
            return ApiResult<bool>.Ok(true, "Hủy đơn hàng thành công."); // Trả về kết quả thành công.
        }

        public async Task<ApiResult<bool>> CompleteOrderAsync(int id) // Phương thức xác nhận hoàn thành đơn hàng (Admin/Nhân viên).
        {
            var order = await _orderRepo.GetByIdAsync(id); // Lấy thông tin đơn hàng theo Id.
            if (order == null) // Nếu không tìm thấy đơn hàng.
            {
                return ApiResult<bool>.Fail("Không tìm thấy đơn hàng.", ApiErrorCode.NotFound); // Trả về lỗi không tìm thấy đơn.
            }

            if (order.Status != 2) // Ràng buộc: chỉ cho phép hoàn thành khi đơn hàng đang ở trạng thái "Đang giao" (Shipping / Status = 2).
            {
                return ApiResult<bool>.Fail("Chỉ có thể xác nhận giao cho đơn hàng đang trong trạng thái 'Đang giao'."); // Trả về lỗi nếu đơn hàng không ở trạng thái đang vận chuyển.
            }

            order.Status = 3; // Cập nhật trạng thái đơn hàng thành 3 (Success - Thành công).
            await _unitOfWork.SaveChangesAsync(); // Lưu thay đổi trạng thái hoàn thành vào database.
            return ApiResult<bool>.Ok(true, "Đơn hàng đã được đánh dấu hoàn thành."); // Trả về kết quả thành công.
        }

        public async Task<ApiResult<bool>> ConfirmOrderAsync(int id) // Phương thức duyệt/xác nhận đơn hàng (Admin/Nhân viên).
        {
            var order = await _orderRepo.GetByIdAsync(id); // Lấy thông tin đơn hàng theo Id.
            if (order == null) // Nếu không tìm thấy đơn hàng.
                return ApiResult<bool>.Fail("Không tìm thấy đơn hàng.", ApiErrorCode.NotFound); // Trả về lỗi không tìm thấy.

            if (order.Status != 0) // Ràng buộc: chỉ cho phép duyệt đơn hàng đang ở trạng thái "Chờ duyệt" (Pending / Status = 0).
                return ApiResult<bool>.Fail("Chỉ có thể duyệt đơn hàng đang ở trạng thái 'Chờ duyệt'."); // Trả về lỗi nếu trạng thái đơn hàng không phải chờ duyệt.

            order.Status = 1; // Cập nhật trạng thái đơn hàng thành 1 (Confirmed - Đã xác nhận).
            await _unitOfWork.SaveChangesAsync(); // Lưu thay đổi trạng thái xác nhận xuống cơ sở dữ liệu.
            return ApiResult<bool>.Ok(true, "Đã duyệt đơn hàng thành công."); // Trả về kết quả thành công.
        }

        public async Task<ApiResult<OrderDetailDto>> GetMyOrderByIdAsync(int id, Guid userId) // Phương thức lấy chi tiết đơn hàng của cá nhân khách hàng.
        {
            var order = await _orderRepo.GetByIdWithDetailsAsync(id); // Truy vấn thông tin đơn hàng kèm chi tiết.
            if (order == null || order.UserId != userId) // Nếu không tìm thấy đơn hàng hoặc đơn hàng không thuộc về người dùng đang đăng nhập.
                return ApiResult<OrderDetailDto>.Fail("Không tìm thấy đơn hàng.", ApiErrorCode.NotFound); // Trả về lỗi NotFound để bảo mật, tránh rò rỉ thông tin của user khác.

            var dto = MapToOrderDetailDto(order); // Ánh xạ thực thể đơn hàng sang DTO chi tiết.
            return ApiResult<OrderDetailDto>.Ok(dto); // Trả về kết quả thành công kèm DTO.
        }

        public async Task<ApiResult<bool>> CancelMyOrderAsync(int id, Guid userId, string cancelReason) // Phương thức khách hàng tự hủy đơn hàng của chính mình.
        {
            if (string.IsNullOrWhiteSpace(cancelReason)) // Ràng buộc: lý do hủy không được để trống.
                return ApiResult<bool>.Fail("Vui lòng cung cấp lý do hủy đơn."); // Trả về lỗi yêu cầu lý do hủy.

            var order = await _orderRepo.GetByIdAsync(id); // Lấy thông tin đơn hàng theo Id.
            if (order == null || order.UserId != userId) // Nếu không tìm thấy đơn hoặc đơn không thuộc về người dùng hiện tại.
                return ApiResult<bool>.Fail("Không tìm thấy đơn hàng.", ApiErrorCode.NotFound); // Trả về lỗi NotFound.

            if (order.Status != (byte)OrderStatus.Pending) // Khách hàng chỉ được phép tự hủy đơn khi đơn hàng đang ở trạng thái Chờ duyệt (Pending).
                return ApiResult<bool>.Fail("Chỉ có thể hủy đơn hàng đang ở trạng thái 'Chờ duyệt'."); // Trả về lỗi không thể hủy đơn do đơn đã được duyệt hoặc đang xử lý.

            order.Status = (byte)OrderStatus.Cancelled; // Cập nhật trạng thái đơn thành Cancelled (Đã hủy).
            order.CancelReason = cancelReason; // Ghi nhận lý do hủy của khách hàng.

            await _unitOfWork.SaveChangesAsync(); // Lưu thay đổi trạng thái hủy đơn xuống DB.
            return ApiResult<bool>.Ok(true, "Hủy đơn hàng thành công."); // Trả về kết quả thành công.
        }

        public async Task<ApiResult<bool>> ConfirmReceivedByCustomerAsync(int id, Guid userId) // Phương thức khách hàng tự xác nhận đã nhận được hàng.
        {
            var order = await _orderRepo.GetByIdAsync(id); // Lấy thông tin đơn hàng theo Id.
            if (order == null || order.UserId != userId) // Nếu không tìm thấy đơn hàng hoặc đơn hàng không thuộc về người dùng hiện tại.
                return ApiResult<bool>.Fail("Không tìm thấy đơn hàng.", ApiErrorCode.NotFound); // Trả về lỗi không tìm thấy đơn.

            if (order.Status != (byte)OrderStatus.Exported) // Khách chỉ được phép xác nhận nhận hàng khi đơn hàng đang ở trạng thái Đang giao (Exported/Shipping).
                return ApiResult<bool>.Fail("Chỉ có thể xác nhận khi đơn hàng đang ở trạng thái 'Đang giao'."); // Trả về lỗi nếu trạng thái đơn hàng hiện tại không khớp điều kiện xác nhận.

            order.Status = (byte)OrderStatus.Success; // Cập nhật trạng thái đơn thành Success (Giao hàng thành công).
            await _unitOfWork.SaveChangesAsync(); // Lưu thay đổi trạng thái đơn hàng xuống DB.
            return ApiResult<bool>.Ok(true, "Đã xác nhận nhận hàng thành công."); // Trả về kết quả thành công.
        }

        private OrderDetailDto MapToOrderDetailDto(Order order) // Hàm phụ trợ ánh xạ từ thực thể Order sang OrderDetailDto chi tiết.
        {
            return new OrderDetailDto // Khởi tạo đối tượng DTO chi tiết đơn hàng mới.
            {
                Id = order.Id, // Ánh xạ Id đơn hàng.
                OrderCode = order.OrderCode, // Ánh xạ mã đơn hàng.
                OrderDate = order.OrderDate, // Ánh xạ ngày đặt hàng.
                Status = order.Status, // Ánh xạ trạng thái đơn hàng.
                ShipName = order.ShipName, // Ánh xạ tên người nhận.
                ShipPhone = order.ShipPhone, // Ánh xạ số điện thoại người nhận.
                ShipAddress = order.ShipAddress, // Ánh xạ địa chỉ nhận.
                ShipCity = order.ShipCity, // Ánh xạ thành phố nhận.
                PaymentMethod = order.PaymentMethod, // Ánh xạ phương thức thanh toán.
                PaymentStatus = order.PaymentStatus, // Ánh xạ trạng thái thanh toán.
                OrderType = order.OrderType, // Ánh xạ loại đơn hàng (Online/POS).
                Note = order.Note, // Ánh xạ ghi chú đơn hàng.
                CancelReason = order.CancelReason, // Ánh xạ lý do hủy đơn hàng.
                SubTotal = order.SubTotal, // Ánh xạ tổng tiền trước giảm.
                ShippingFee = order.ShippingFee, // Ánh xạ phí vận chuyển.
                DiscountAmount = order.DiscountAmount, // Ánh xạ tiền được chiết khấu.
                TotalAmount = order.TotalAmount, // Ánh xạ tổng số tiền phải trả cuối cùng.
                Items = order.OrderDetails.Select(d => new OrderDetailLineDto // Ánh xạ danh sách chi tiết sản phẩm đơn hàng.
                {
                    Id = d.Id, // Gán Id bản ghi chi tiết.
                    VariantId = d.VariantId, // Gán Id biến thể sản phẩm.
                    VariantName = d.Variant.VariantName, // Gán tên biến thể sản phẩm.
                    SKU = d.Variant.SKU, // Gán mã SKU sản phẩm.
                    Quantity = d.Quantity, // Gán số lượng đặt mua.
                    UnitPrice = d.UnitPrice, // Gán đơn giá lúc mua.
                    TotalLine = d.Quantity * d.UnitPrice, // Tính tổng tiền dòng sản phẩm = Số lượng * Đơn giá.
                    MainImageUrl = d.Variant.Images != null && d.Variant.Images.Any() // Nếu sản phẩm có ảnh đi kèm.
                        ? (d.Variant.Images.FirstOrDefault(i => i.IsMain)?.ImageUrl // Lấy ảnh đại diện chính của sản phẩm.
                           ?? d.Variant.Images.OrderBy(i => i.SortOrder).First().ImageUrl) // Nếu không có ảnh chính, lấy ảnh đầu tiên theo thứ tự sắp xếp.
                        : null, // Ngược lại gán null.
                    Serials = d.OrderSerials?.Select(os => os.Serial.SerialNumber).ToList() ?? new List<string>() // Lấy danh sách số serial xuất kho cho dòng sản phẩm này nếu có, ngược lại khởi tạo list trống.
                }).ToList(), // Chuyển đổi Select sang danh sách List.
                AppliedVouchers = order.VoucherUsages?.Select(v => new VoucherUsageDto // Ánh xạ danh sách các voucher đã được áp dụng cho đơn hàng.
                {
                    VoucherCode = v.Voucher.Code, // Gán mã code voucher.
                    VoucherName = v.Voucher.Name, // Gán tên hiển thị voucher.
                    DiscountType = v.Voucher.DiscountType, // Gán loại chiết khấu (Tiền cố định hoặc phần trăm).
                    DiscountValue = v.Voucher.DiscountValue, // Gán giá trị chiết khấu gốc.
                    DiscountApplied = v.DiscountApplied // Gán số tiền thực tế được giảm từ voucher này.
                }).ToList() ?? new List<VoucherUsageDto>() // Chuyển đổi sang danh sách List, mặc định list trống nếu null.
            };
        }
    }
}

using System; // Sử dụng các kiểu dữ liệu cơ bản của hệ thống.
using System.Linq; // Sử dụng LINQ cho các thao tác trên danh sách.
using System.Threading.Tasks; // Sử dụng Task cho lập trình bất đồng bộ.
using PBL3.Core.Entities; // Sử dụng các thực thể nghiệp vụ (Cart, Product, Variant).
using PBL3.Core.Interfaces; // Sử dụng các giao diện repository và unit of work.
using PBL3.Shared.DTOs.Cart; // Sử dụng các DTO liên quan đến giỏ hàng.
using PBL3.Shared.DTOs.Common; // Sử dụng DTO dùng chung ApiResult.

namespace PBL3.Application.Cart // Khai báo namespace cho lớp dịch vụ CartService thuộc tầng Application.
{
    public class CartService( // Khởi tạo lớp CartService bằng Primary Constructor.
        ICartRepository cartRepo, // Tiêm ICartRepository để làm việc với dữ liệu giỏ hàng.
        IProductRepository productRepo, // Tiêm IProductRepository để truy xuất thông tin sản phẩm và biến thể.
        IUnitOfWork unitOfWork) : ICartService // Tiêm IUnitOfWork quản lý lưu trữ dữ liệu và triển khai ICartService.
    {
        private readonly ICartRepository _cartRepo = // Gán trường ICartRepository.
            cartRepo ?? throw new ArgumentNullException(nameof(cartRepo)); // Kiểm tra null cho cartRepo.
        private readonly IProductRepository _productRepo = // Gán trường IProductRepository.
            productRepo ?? throw new ArgumentNullException(nameof(productRepo)); // Kiểm tra null cho productRepo.
        private readonly IUnitOfWork _unitOfWork = // Gán trường IUnitOfWork.
            unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork)); // Kiểm tra null cho unitOfWork.

        public async Task<ApiResult<CartResponse>> GetMyCartAsync(Guid userId) // Phương thức lấy thông tin giỏ hàng của người dùng bất đồng bộ.
        {
            var carts = await _cartRepo.GetCartItemsByUserAsync(userId); // Gọi repository lấy danh sách các mặt hàng trong giỏ từ database.

            var response = new CartResponse(); // Khởi tạo đối tượng DTO phản hồi giỏ hàng.
            foreach (var cart in carts) // Duyệt qua từng bản ghi giỏ hàng của người dùng.
            {
                var image = cart.Variant.Images.FirstOrDefault(i => i.IsMain) // Tìm hình ảnh chính (IsMain) của phiên bản sản phẩm.
                            ?? cart.Variant.Images.OrderBy(i => i.SortOrder).FirstOrDefault(); // Nếu không có, chọn hình ảnh có thứ tự sắp xếp nhỏ nhất.

                response.Items.Add(new CartItemResponse // Thêm mặt hàng giỏ hàng vào danh sách DTO.
                {
                    Id = cart.Id, // Gán Id mục giỏ hàng.
                    VariantId = cart.VariantId, // Gán mã phiên bản sản phẩm.
                    ProductName = cart.Variant.Product.Name, // Gán tên sản phẩm.
                    VariantName = cart.Variant.VariantName, // Gán tên phiên bản sản phẩm.
                    ImageUrl = image?.ImageUrl, // Gán đường dẫn hình ảnh sản phẩm.
                    UnitPrice = cart.Variant.Price, // Gán đơn giá của phiên bản sản phẩm.
                    Quantity = cart.Quantity, // Gán số lượng người dùng muốn mua.
                    SubTotal = cart.Variant.Price * cart.Quantity, // Tính thành tiền cho dòng sản phẩm hiện tại.
                    StockQuantity = cart.Variant.StockQuantity, // Gán số lượng tồn kho còn lại trong hệ thống.
                    ProductSlug = cart.Variant.Product.Slug // Gán đường dẫn thân thiện (slug) của sản phẩm.
                });
            }

            response.TotalAmount = response.Items.Sum(i => i.SubTotal); // Tính tổng số tiền của toàn bộ giỏ hàng.

            return ApiResult<CartResponse>.Ok(response); // Trả về kết quả thành công chứa thông tin chi tiết giỏ hàng.
        }

        public async Task<ApiResult<CartResponse>> AddToCartAsync(Guid userId, AddToCartRequest request) // Phương thức thêm sản phẩm vào giỏ hàng bất đồng bộ.
        {
            var variant = await _productRepo.GetVariantByIdAsync(request.VariantId); // Gọi repository lấy thông tin phiên bản sản phẩm theo Id.
            if (variant == null) // Nếu không tìm thấy phiên bản sản phẩm.
            {
                return ApiResult<CartResponse>.Fail("Sản phẩm không tồn tại hoặc đã ngừng kinh doanh.", ApiErrorCode.NotFound); // Trả về thông báo lỗi 404.
            }

            var product = await _productRepo.GetByIdAsync(variant.ProductId); // Lấy thông tin sản phẩm gốc của phiên bản đó.
            if (product == null || product.Status != 1 || product.IsDeleted) // Nếu sản phẩm không tồn tại, ngừng hoạt động hoặc đã bị xóa mềm.
            {
                return ApiResult<CartResponse>.Fail("Sản phẩm không tồn tại hoặc đã ngừng kinh doanh.", ApiErrorCode.NotFound); // Trả về thông báo lỗi 404.
            }

            if (request.Quantity <= 0) // Kiểm tra số lượng thêm vào giỏ có hợp lệ không (phải lớn hơn 0).
            {
                return ApiResult<CartResponse>.Fail("Số lượng không hợp lệ."); // Báo lỗi số lượng không hợp lệ.
            }

            var existingCart = await _cartRepo.FindByUserAndVariantAsync(userId, request.VariantId); // Tìm xem phiên bản sản phẩm này đã có sẵn trong giỏ hàng của người dùng chưa.

            var currentQty = existingCart?.Quantity ?? 0; // Lấy số lượng hiện tại của sản phẩm trong giỏ (mặc định 0 nếu chưa có).
            var newTotal = currentQty + request.Quantity; // Tính tổng số lượng mới của sản phẩm nếu được thêm vào.

            if (newTotal > variant.StockQuantity) // Kiểm tra xem tổng số lượng yêu cầu có vượt quá số lượng hàng tồn kho không.
            {
                var remaining = variant.StockQuantity - currentQty; // Tính số lượng sản phẩm còn lại tối đa có thể thêm vào giỏ.
                return ApiResult<CartResponse>.Fail(remaining <= 0 // Báo lỗi dựa trên số sản phẩm có thể thêm còn lại.
                    ? "Sản phẩm đã đạt giới hạn tồn kho trong giỏ hàng." // Nếu còn 0, báo đạt giới hạn tồn kho.
                    : $"Chỉ có thể thêm tối đa {remaining} sản phẩm nữa."); // Ngược lại báo số lượng cụ thể còn lại.
            }

            if (existingCart != null) // Nếu sản phẩm đã tồn tại sẵn trong giỏ hàng.
            {
                existingCart.Quantity = newTotal; // Cộng dồn số lượng mới cho bản ghi giỏ hàng hiện tại.
            }
            else // Nếu sản phẩm chưa có trong giỏ hàng.
            {
                var newCart = new PBL3.Core.Entities.Cart // Khởi tạo bản ghi giỏ hàng mới.
                {
                    UserId = userId, // Gán UserId của người mua.
                    VariantId = request.VariantId, // Gán VariantId của phiên bản sản phẩm.
                    Quantity = request.Quantity, // Gán số lượng cần thêm.
                    CreatedDate = DateTime.UtcNow // Gán thời gian tạo.
                };
                await _cartRepo.AddAsync(newCart); // Thêm bản ghi giỏ hàng mới vào Database context thông qua repository.
            }

            await _unitOfWork.SaveChangesAsync(); // Lưu toàn bộ thay đổi xuống Database.

            return await GetMyCartAsync(userId); // Gọi lại phương thức lấy giỏ hàng mới nhất để phản hồi cho người dùng.
        }

        public async Task<ApiResult<CartResponse>> UpdateQuantityAsync(Guid userId, int cartItemId, UpdateCartItemRequest request) // Phương thức cập nhật số lượng mặt hàng trong giỏ.
        {
            var cart = await _cartRepo.GetCartItemAsync(cartItemId, userId); // Lấy bản ghi giỏ hàng tương ứng theo Id và UserId sở hữu.
            if (cart == null) // Nếu không tìm thấy bản ghi.
            {
                return ApiResult<CartResponse>.Fail("Không tìm thấy sản phẩm trong giỏ hàng.", ApiErrorCode.NotFound); // Trả về thông báo lỗi 404.
            }

            if (request.Quantity <= 0) // Nếu số lượng mới cập nhật nhỏ hơn hoặc bằng 0.
            {
                _cartRepo.Remove(cart); // Tiến hành xóa hoàn toàn mặt hàng này khỏi giỏ hàng.
            }
            else // Nếu số lượng mới hợp lệ (> 0).
            {
                var variant = await _productRepo.GetVariantByIdAsync(cart.VariantId); // Lấy thông tin tồn kho của phiên bản sản phẩm đó.
                var stockLimit = variant?.StockQuantity ?? 99; // Lấy số lượng tồn kho thực tế của phiên bản (mặc định 99 nếu trống).

                if (request.Quantity > stockLimit) // Kiểm tra số lượng cập nhật có vượt quá tồn kho không.
                {
                    return ApiResult<CartResponse>.Fail($"Số lượng vượt tồn kho. Chỉ còn {stockLimit} sản phẩm."); // Báo lỗi số lượng vượt tồn kho.
                }

                cart.Quantity = request.Quantity; // Cập nhật số lượng mới cho bản ghi giỏ hàng.
            }

            await _unitOfWork.SaveChangesAsync(); // Lưu thay đổi giỏ hàng vào Database.

            return await GetMyCartAsync(userId); // Trả về thông tin giỏ hàng mới cập nhật.
        }

        public async Task<ApiResult<CartResponse>> RemoveItemAsync(Guid userId, int cartItemId) // Phương thức xóa mặt hàng khỏi giỏ.
        {
            var cart = await _cartRepo.GetCartItemAsync(cartItemId, userId); // Lấy thông tin mặt hàng trong giỏ theo Id và UserId.
            if (cart == null) // Nếu không tìm thấy.
            {
                return ApiResult<CartResponse>.Fail("Không tìm thấy sản phẩm trong giỏ hàng.", ApiErrorCode.NotFound); // Báo lỗi không tìm thấy.
            }

            _cartRepo.Remove(cart); // Gọi repository thực hiện xóa mặt hàng này khỏi giỏ.
            await _unitOfWork.SaveChangesAsync(); // Lưu thay đổi xóa vào Database.

            return await GetMyCartAsync(userId); // Trả về thông tin giỏ hàng mới sau khi đã xóa.
        }

        public async Task<ApiResult<bool>> ClearCartAsync(Guid userId) // Phương thức dọn dẹp sạch toàn bộ giỏ hàng của người dùng.
        {
            var carts = await _cartRepo.GetCartItemsWithTrackingAsync(userId); // Lấy toàn bộ các bản ghi giỏ hàng của người dùng dưới chế độ tracking.
            if (carts.Any()) // Nếu giỏ hàng hiện tại không rỗng.
            {
                _cartRepo.RemoveRange(carts); // Gọi repository xóa hàng loạt toàn bộ các mặt hàng trong giỏ.
                await _unitOfWork.SaveChangesAsync(); // Lưu thay đổi xóa hàng loạt vào Database.
            }

            return ApiResult<bool>.Ok(true, "Đã xóa toàn bộ giỏ hàng."); // Trả về kết quả thành công kèm thông báo.
        }
    }
}

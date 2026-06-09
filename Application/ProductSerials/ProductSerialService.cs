using Microsoft.Extensions.Logging; // Sử dụng thư viện ghi log hệ thống.
using PBL3.Core.Interfaces; // Sử dụng các giao diện repository.
using PBL3.Application.Inventory; // Sử dụng dịch vụ đồng bộ kho hàng.
using PBL3.Shared.DTOs.Common; // Sử dụng DTO dùng chung ApiResult.
using PBL3.Shared.DTOs.Inventory; // Sử dụng DTO của module kho.
using PBL3.Shared.Enums; // Sử dụng Enum SerialStatus.

namespace PBL3.Application.ProductSerials // Khai báo namespace cho tầng Application của module quản lý số Serial.
{
    public class ProductSerialService( // Định nghĩa lớp ProductSerialService sử dụng Primary Constructor.
        IProductSerialRepository productSerialRepository, // Tiêm repository số Serial sản phẩm.
        IOrderRepository orderRepository, // Tiêm repository đơn hàng.
        IInventorySyncService inventorySyncService, // Tiêm dịch vụ đồng bộ tồn kho.
        ILogger<ProductSerialService> logger) : IProductSerialService // Tiêm logger hệ thống và triển khai giao diện IProductSerialService.
    {
        private readonly IProductSerialRepository _productSerialRepository = // Gán repository số Serial vào trường thành viên.
            productSerialRepository ?? throw new ArgumentNullException(nameof(productSerialRepository)); // Kiểm tra null cho productSerialRepository.
        private readonly IOrderRepository _orderRepository = // Gán repository đơn hàng vào trường thành viên.
            orderRepository ?? throw new ArgumentNullException(nameof(orderRepository)); // Kiểm tra null cho orderRepository.
        private readonly IInventorySyncService _inventorySyncService = // Gán dịch vụ đồng bộ kho vào trường thành viên.
            inventorySyncService ?? throw new ArgumentNullException(nameof(inventorySyncService)); // Kiểm tra null cho inventorySyncService.
        private readonly ILogger<ProductSerialService> _logger = // Gán logger vào trường thành viên.
            logger ?? throw new ArgumentNullException(nameof(logger)); // Kiểm tra null cho logger.

        public async Task<ApiResult<bool>> CheckExistAsync(string serialNumber, int variantId) // Định nghĩa phương thức kiểm tra xem số Serial đã tồn tại trong DB chưa.
        {
            var exists = await _productSerialRepository.ExistsAsync(serialNumber, variantId); // Gọi repository kiểm tra xem số Serial có tồn tại theo VariantId không.
            return new ApiResult<bool> // Khởi tạo kết quả API Result.
            {
                Success = true, // Thiết lập thành công.
                Message = exists // Gán thông báo chi tiết tùy theo trạng thái tồn tại.
                    ? "Mã Serial đã tồn tại trong hệ thống." // Thông báo nếu mã đã tồn tại.
                    : "Mã Serial hợp lệ, chưa có trong hệ thống.", // Thông báo nếu mã hợp lệ chưa tồn tại.
                Data = exists // Gán dữ liệu trả về dạng bool chỉ ra trạng thái tồn tại.
            };
        }

        public async Task<ApiResult<PagedResult<ProductSerialListDto>>> GetPagedListAsync(ProductSerialFilterRequest filter) // Định nghĩa phương thức lấy danh sách Serial phân trang có bộ lọc.
        {
            var (items, totalCount) = await _productSerialRepository.GetPagedListAsync( // Lấy danh sách Serial phân trang và tổng số bản ghi từ repository.
                filter.Keyword, filter.ProductId, filter.VariantId, // Lọc theo từ khóa, Id sản phẩm, Id biến thể sản phẩm.
                filter.Status, filter.FromDate, filter.ToDate, // Lọc theo trạng thái Serial, khoảng thời gian tạo.
                filter.PageNumber, filter.PageSize, filter.SortBy, filter.SortDescending); // Phân trang và cấu hình cột sắp xếp.

            var dtos = items.Select(s => new ProductSerialListDto // Ánh xạ danh sách thực thể sang danh sách DTO hiển thị.
            {
                Id = s.Id, // Ánh xạ Id.
                SerialNumber = s.SerialNumber, // Ánh xạ số Serial.
                VariantId = s.VariantId, // Ánh xạ Id biến thể sản phẩm.
                VariantName = s.Variant.VariantName, // Ánh xạ tên biến thể.
                SKU = s.Variant.SKU, // Ánh xạ mã SKU.
                ProductId = s.Variant.ProductId, // Ánh xạ Id sản phẩm.
                ProductName = s.Variant.Product.Name, // Ánh xạ tên sản phẩm.
                ImportReceiptId = s.ImportReceiptId, // Ánh xạ Id phiếu nhập kho.
                ReceiptCode = s.ImportReceipt.ReceiptCode, // Ánh xạ mã phiếu nhập kho.
                Status = s.Status, // Ánh xạ trạng thái số Serial dưới dạng byte.
                StatusLabel = GetStatusLabel(s.Status), // Ánh xạ tên nhãn trạng thái (ví dụ: Trong kho, Đã bán,...).
                OrderId = s.OrderId, // Ánh xạ Id đơn hàng liên quan nếu đã bán hoặc đặt hàng.
                CreatedDate = s.CreatedDate, // Ánh xạ ngày tạo/nhập kho.
                SoldDate = s.SoldDate // Ánh xạ ngày bán (nếu có).
            }).ToList(); // Chuyển kết quả sang danh sách List.

            var result = new PagedResult<ProductSerialListDto> // Khởi tạo kết quả phân trang DTO.
            {
                Items = dtos, // Gán danh sách DTO.
                TotalCount = totalCount, // Gán tổng số lượng bản ghi khớp bộ lọc.
                PageNumber = filter.PageNumber, // Gán số trang hiện tại.
                PageSize = filter.PageSize // Gán số lượng phần tử trên trang.
            };

            return ApiResult<PagedResult<ProductSerialListDto>>.Ok(result); // Trả về kết quả phân trang thành công.
        }

        public async Task<ApiResult<ProductSerialDetailDto>> GetByIdAsync(int id) // Định nghĩa phương thức lấy chi tiết một số Serial theo Id.
        {
            var serial = await _productSerialRepository.GetByIdWithDetailsAsync(id); // Lấy thực thể Serial kèm theo các chi tiết liên kết từ repository.
            if (serial == null) // Nếu không tìm thấy thực thể Serial nào khớp với Id.
                return ApiResult<ProductSerialDetailDto>.Fail("Không tìm thấy Serial yêu cầu.", ApiErrorCode.NotFound); // Trả về kết quả lỗi NotFound.

            var dto = new ProductSerialDetailDto // Khởi tạo DTO thông tin chi tiết số Serial.
            {
                Id = serial.Id, // Gán Id.
                SerialNumber = serial.SerialNumber, // Gán số Serial.
                Status = serial.Status, // Gán trạng thái số Serial dạng byte.
                StatusLabel = GetStatusLabel(serial.Status), // Gán tên nhãn trạng thái.
                CreatedDate = serial.CreatedDate, // Gán ngày tạo.
                SoldDate = serial.SoldDate, // Gán ngày bán.
                VariantId = serial.VariantId, // Gán Id biến thể sản phẩm.
                VariantName = serial.Variant.VariantName, // Gán tên biến thể.
                SKU = serial.Variant.SKU, // Gán mã SKU.
                WarrantyMonth = serial.Variant.WarrantyMonth, // Gán số tháng bảo hành của sản phẩm.
                Price = serial.Variant.Price, // Gán đơn giá của sản phẩm.
                ProductId = serial.Variant.ProductId, // Gán Id sản phẩm.
                ProductName = serial.Variant.Product.Name, // Gán tên sản phẩm.
                ImportReceiptId = serial.ImportReceiptId, // Gán Id phiếu nhập kho.
                ReceiptCode = serial.ImportReceipt.ReceiptCode, // Gán mã phiếu nhập kho.
                ImportDate = serial.ImportReceipt.ImportDate, // Gán ngày nhập kho.
                SupplierName = serial.ImportReceipt.Supplier.Name, // Gán tên nhà cung cấp.
                OrderId = serial.OrderId // Gán Id đơn hàng liên kết (nếu có).
            };

            if (serial.OrderId.HasValue) // Nếu số Serial này đã được bán hoặc liên kết với một đơn hàng cụ thể.
            {
                var order = await _orderRepository.GetByIdAsync(serial.OrderId.Value); // Lấy thông tin thực thể đơn hàng tương ứng từ repository.
                if (order != null) // Nếu tìm thấy thông tin đơn hàng trong cơ sở dữ liệu.
                {
                    dto.OrderCode = order.OrderCode; // Gán mã đơn hàng vào DTO chi tiết Serial.
                    dto.OrderDate = order.OrderDate; // Gán ngày tạo đơn hàng.
                    dto.OrderStatus = order.Status; // Gán trạng thái hiện tại của đơn hàng.
                }
            }

            return ApiResult<ProductSerialDetailDto>.Ok(dto); // Trả về kết quả chi tiết số Serial thành công.
        }

        public async Task<ApiResult<ProductSerialStatisticsDto>> GetStatisticsAsync(int? productId, int? variantId) // Định nghĩa phương thức thống kê số lượng Serial theo trạng thái.
        {
            var counts = await _productSerialRepository.GetStatusCountsAsync(productId, variantId); // Gọi repository lấy số lượng thống kê theo trạng thái của sản phẩm hoặc biến thể.

            var dto = new ProductSerialStatisticsDto // Khởi tạo DTO chứa dữ liệu thống kê.
            {
                AvailableCount = counts.GetValueOrDefault((byte)SerialStatus.Available, 0), // Gán số lượng Serial có sẵn (trong kho).
                ReservedCount = counts.GetValueOrDefault((byte)SerialStatus.Reserved, 0), // Gán số lượng Serial đã được đặt hàng (giữ chỗ).
                SoldCount = counts.GetValueOrDefault((byte)SerialStatus.Sold, 0), // Gán số lượng Serial đã bán ra.
                DefectiveCount = counts.GetValueOrDefault((byte)SerialStatus.Defective, 0), // Gán số lượng Serial bị lỗi vật lý.
                ReturnedCount = counts.GetValueOrDefault((byte)SerialStatus.Returned, 0), // Gán số lượng Serial đã được trả lại.
                LostCount = counts.GetValueOrDefault((byte)SerialStatus.Lost, 0), // Gán số lượng Serial bị thất thoát.
                ProductId = productId, // Gán Id lọc theo sản phẩm.
                VariantId = variantId // Gán Id lọc theo biến thể.
            };
            dto.TotalCount = counts.Values.Sum(); // Tính tổng số lượng Serial bằng cách cộng tất cả các giá trị trạng thái.

            return ApiResult<ProductSerialStatisticsDto>.Ok(dto); // Trả về DTO kết quả thống kê thành công.
        }

        public async Task<ApiResult<bool>> UpdateStatusAsync(int id, UpdateSerialStatusRequest request) // Định nghĩa phương thức thay đổi trạng thái Serial của quản trị viên.
        {
            var serial = await _productSerialRepository.GetByIdWithTrackingAsync(id); // Lấy thực thể Serial theo Id ở chế độ theo dõi thay đổi.
            if (serial == null) // Nếu không tìm thấy thực thể Serial.
                return ApiResult<bool>.Fail("Không tìm thấy Serial yêu cầu.", ApiErrorCode.NotFound); // Trả về kết quả lỗi NotFound.

            var currentStatus = (SerialStatus)serial.Status; // Lấy trạng thái hiện tại của thực thể Serial.
            var newStatus = (SerialStatus)request.NewStatus; // Lấy trạng thái mới cần cập nhật.

            var validationError = ValidateTransition(currentStatus, newStatus); // Kiểm tra xem việc chuyển dịch trạng thái này có hợp lệ về mặt nghiệp vụ không.
            if (validationError != null) // Nếu phát sinh lỗi chuyển dịch trạng thái không hợp lệ.
                return ApiResult<bool>.Fail(validationError); // Trả về kết quả thất bại kèm mô tả lỗi nghiệp vụ.

            var variantId = serial.VariantId; // Lưu lại Id biến thể của số Serial hiện tại.

            try // Bắt đầu khối xử lý ngoại lệ khi cập nhật và đồng bộ tồn kho.
            {
                serial.Status = request.NewStatus; // Cập nhật giá trị trạng thái mới cho thực thể Serial.
                await _productSerialRepository.SaveChangesAsync(); // Lưu thay đổi cập nhật thực thể vào cơ sở dữ liệu.

                if (!string.IsNullOrWhiteSpace(request.Note)) // Nếu có ghi chú lý do thay đổi trạng thái.
                    _logger.LogInformation("Serial {SerialNumber} chuyển trạng thái {From} → {To}. Lý do: {Note}", // Ghi log thông báo chuyển đổi trạng thái thành công.
                        serial.SerialNumber, currentStatus, newStatus, request.Note); // Các tham số thông tin chi tiết.

                await _inventorySyncService.SyncStockAsync(variantId); // Gọi dịch vụ đồng bộ tồn kho vật lý để tính toán lại số lượng tồn khả dụng thực tế của biến thể này.

                return ApiResult<bool>.Ok(true, "Cập nhật trạng thái Serial thành công."); // Trả về kết quả cập nhật trạng thái thành công.
            }
            catch (Exception ex) // Bắt lỗi nếu có ngoại lệ phát sinh trong quá trình lưu dữ liệu hoặc đồng bộ kho.
            {
                _logger.LogError(ex, "Lỗi khi cập nhật trạng thái Serial {Id}", id); // Ghi log lỗi hệ thống kèm exception.
                return ApiResult<bool>.Fail("Đã xảy ra lỗi khi cập nhật trạng thái Serial."); // Trả về kết quả lỗi thất bại.
            }
        }

        private static string? ValidateTransition(SerialStatus current, SerialStatus next) // Định nghĩa hàm kiểm tra hợp lệ của việc chuyển dịch trạng thái.
        {
            if (current == SerialStatus.Sold) // Chốt chặn: Nếu Serial đã bán thì tuyệt đối không được phép thay đổi trạng thái nữa.
                return "Không thể thay đổi trạng thái Serial đã bán."; // Trả về thông báo lỗi.

            var allowed = (current, next) switch // Sử dụng Pattern Matching kiểm tra cặp chuyển dịch trạng thái (hiện tại -> kế tiếp).
            {
                (SerialStatus.Available, SerialStatus.Defective) => true, // Cho phép chuyển từ Có sẵn sang Bị lỗi.
                (SerialStatus.Reserved,  SerialStatus.Defective) => true, // Cho phép chuyển từ Giữ chỗ sang Bị lỗi.
                (SerialStatus.Defective, SerialStatus.Returned)  => true, // Cho phép chuyển từ Bị lỗi sang Đã trả lại.
                (SerialStatus.Defective, SerialStatus.Available) => true, // Cho phép chuyển từ Bị lỗi sang Có sẵn (sau khi sửa/sửa xong).
                (SerialStatus.Returned,  SerialStatus.Available) => true, // Cho phép chuyển từ Đã trả lại sang Có sẵn (tái nhập kho).
                (SerialStatus.Lost,      SerialStatus.Returned)  => true, // Cho phép chuyển từ Thất thoát sang Đã trả lại.
                _ => false // Tất cả các trường hợp chuyển dịch trạng thái khác đều bị cấm.
            };

            return allowed // Trả về kết quả kiểm tra.
                ? null // Trả về null nếu hợp lệ.
                : $"Không thể chuyển trạng thái từ '{GetStatusLabel((byte)current)}' sang '{GetStatusLabel((byte)next)}'."; // Trả về chuỗi lỗi mô tả chi tiết nếu không hợp lệ.
        }

        private static string GetStatusLabel(byte status) => status switch // Định nghĩa hàm hỗ trợ lấy nhãn tiếng Việt tương ứng cho từng mã trạng thái Serial.
        {
            0 => "Trong kho", // Trạng thái Available.
            1 => "Đã đặt hàng", // Trạng thái Reserved.
            2 => "Đã bán", // Trạng thái Sold.
            3 => "Hàng lỗi", // Trạng thái Defective.
            4 => "Đã trả lại", // Trạng thái Returned.
            5 => "Thất thoát", // Trạng thái Lost.
            _ => "Không xác định" // Trạng thái không hợp lệ hoặc chưa định nghĩa.
        };
    }
}

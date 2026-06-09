using System; // Sử dụng các kiểu dữ liệu cơ bản của hệ thống.
using System.Collections.Generic; // Sử dụng lớp danh sách generic.
using System.Linq; // Sử dụng LINQ cho các thao tác trên tập hợp.
using System.Threading.Tasks; // Sử dụng Task lập trình bất đồng bộ.
using Microsoft.Extensions.Logging; // Sử dụng ghi log hệ thống.
using PBL3.Core.Entities; // Sử dụng các thực thể từ tầng Core (Order, OrderDetail, ProductSerial, OrderSerial).
using PBL3.Core.Interfaces; // Sử dụng các giao diện repository từ tầng Core.
using PBL3.Shared.DTOs.Common; // Sử dụng DTO dùng chung ApiResult.
using PBL3.Shared.DTOs.Inventory; // Sử dụng các DTO của module kho hàng.
using PBL3.Shared.Enums; // Sử dụng các enum nghiệp vụ (OrderStatus, SerialStatus).

namespace PBL3.Application.Inventory // Khai báo namespace cho lớp dịch vụ xuất kho thuộc tầng Application.
{
    public class InventoryExportService( // Định nghĩa lớp InventoryExportService sử dụng Primary Constructor.
        IUnitOfWork unitOfWork, // Tiêm IUnitOfWork để quản lý transaction.
        IOrderRepository orderRepo, // Tiêm repository quản lý đơn hàng.
        IProductSerialRepository serialRepo, // Tiêm repository quản lý serial sản phẩm.
        IInventorySyncService inventorySyncService, // Tiêm dịch vụ đồng bộ số lượng tồn kho.
        ILogger<InventoryExportService> logger) : IInventoryExportService // Tiêm Logger ghi log và kế thừa IInventoryExportService.
    {
        private readonly IUnitOfWork _unitOfWork = // Gán UnitOfWork vào trường thành viên.
            unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork)); // Kiểm tra null cho unitOfWork.
        private readonly IOrderRepository _orderRepo = // Gán repository đơn hàng vào trường thành viên.
            orderRepo ?? throw new ArgumentNullException(nameof(orderRepo)); // Kiểm tra null cho orderRepo.
        private readonly IProductSerialRepository _serialRepo = // Gán repository serial vào trường thành viên.
            serialRepo ?? throw new ArgumentNullException(nameof(serialRepo)); // Kiểm tra null cho serialRepo.
        private readonly IInventorySyncService _inventorySyncService = // Gán dịch vụ đồng bộ tồn kho vào trường thành viên.
            inventorySyncService ?? throw new ArgumentNullException(nameof(inventorySyncService)); // Kiểm tra null cho inventorySyncService.
        private readonly ILogger<InventoryExportService> _logger = // Gán Logger vào trường thành viên.
            logger ?? throw new ArgumentNullException(nameof(logger)); // Kiểm tra null cho logger.

        public async Task<ApiResult<bool>> ExportOrderAsync(ExportOrderRequest request) // Định nghĩa phương thức xuất kho cho đơn hàng bất đồng bộ.
        {
            var order = await _orderRepo.GetByIdWithDetailsTrackedAsync(request.OrderId); // Lấy thông tin đơn hàng kèm chi tiết đang được tracking từ repo.
            if (order == null) // Nếu đơn hàng không tồn tại.
            {
                return ApiResult<bool>.Fail("Đơn hàng không tồn tại.", ApiErrorCode.NotFound); // Trả về thông báo lỗi NotFound 404.
            }

            if (order.Status != (byte)OrderStatus.Confirmed) // Nếu đơn hàng không ở trạng thái Confirmed (Đã xác nhận).
            {
                return ApiResult<bool>.Fail("Đơn hàng không ở trạng thái Đã xác nhận (Confirmed). Không thể xuất kho."); // Báo lỗi không thể xuất kho.
            }

            var allSerialNumbers = request.Details.SelectMany(d => d.SerialNumbers).Distinct().ToList(); // Gộp toàn bộ mã serial được quét từ yêu cầu và lọc các mã trùng lặp.
            if (!allSerialNumbers.Any()) // Nếu không có mã serial nào được gửi lên.
            {
                return ApiResult<bool>.Fail("Không có mã Serial nào được quét."); // Báo lỗi không quét serial.
            }

            await _unitOfWork.BeginTransactionAsync(); // Khởi động transaction cơ sở dữ liệu để bảo vệ tính toàn vẹn dữ liệu.

            try // Bắt đầu khối bẫy lỗi.
            {
                var dbSerials = await _serialRepo.GetSerialsWithTrackingAsync(allSerialNumbers); // Truy vấn các serial tương ứng từ DB dưới chế độ tracking.
                var dbSerialsMap = dbSerials.ToDictionary(s => s.SerialNumber, s => s, StringComparer.OrdinalIgnoreCase); // Tạo Dictionary ánh xạ serial để tìm kiếm nhanh không phân biệt hoa thường.
                var variantIdsToSync = new HashSet<int>(); // Khởi tạo tập hợp chứa các Id biến thể sản phẩm cần đồng bộ tồn kho.

                foreach (var detailReq in request.Details) // Duyệt qua từng chi tiết mặt hàng trong yêu cầu xuất kho.
                {
                    var orderDetail = order.OrderDetails.FirstOrDefault(od => od.Id == detailReq.OrderDetailId); // Tìm dòng chi tiết đơn hàng tương ứng trong cơ sở dữ liệu.
                    if (orderDetail == null) // Nếu dòng chi tiết đơn hàng không thuộc về đơn hàng này.
                    {
                        throw new Exception($"Chi tiết đơn hàng {detailReq.OrderDetailId} không thuộc về đơn hàng này."); // Ném lỗi ngoại lệ để kích hoạt rollback.
                    }

                    if (detailReq.SerialNumbers.Count != orderDetail.Quantity) // Kiểm tra số lượng serial quét được có khớp với số lượng mua trong hóa đơn không.
                    {
                        throw new Exception($"Chưa quét đúng số lượng cho sản phẩm {orderDetail.Variant?.VariantName ?? orderDetail.VariantId.ToString()}. " + // Ném lỗi chênh lệch số lượng.
                                            $"Yêu cầu: {orderDetail.Quantity}, Đã quét: {detailReq.SerialNumbers.Count}"); // Thông báo rõ số lượng yêu cầu và số lượng quét thực tế.
                    }

                    foreach (var serialNo in detailReq.SerialNumbers) // Duyệt qua từng mã serial được quét cho dòng mặt hàng hiện tại.
                    {
                        if (!dbSerialsMap.TryGetValue(serialNo, out var productSerial)) // Kiểm tra xem mã serial quét có tồn tại trong Dictionary DB không.
                        {
                            throw new Exception($"Mã Serial '{serialNo}' không tồn tại trong hệ thống."); // Ném lỗi không tồn tại serial.
                        }

                        if (productSerial.Status != (byte)SerialStatus.Available) // Đảm bảo serial này đang ở trạng thái có sẵn trong kho (Available).
                        {
                            throw new Exception($"Mã Serial '{serialNo}' không ở trạng thái trong kho (Available). Trạng thái hiện tại: {productSerial.Status}."); // Báo lỗi trạng thái không hợp lệ.
                        }

                        if (productSerial.VariantId != orderDetail.VariantId) // Kiểm tra xem mã serial có thuộc đúng VariantId được đặt mua trong hóa đơn hay không.
                        {
                            throw new Exception($"Mã Serial '{serialNo}' (thuộc sản phẩm {productSerial.Variant?.VariantName ?? productSerial.VariantId.ToString()}) " + // Báo lỗi giao nhầm sản phẩm.
                                                $"KHÔNG KHỚP với sản phẩm yêu cầu trong đơn hàng ({orderDetail.Variant?.VariantName ?? orderDetail.VariantId.ToString()})."); // Chi tiết sản phẩm bị lệch.
                        }

                        productSerial.Status = (byte)SerialStatus.Sold; // Chuyển đổi trạng thái serial sang Đã bán (Sold).
                        productSerial.SoldDate = DateTime.UtcNow; // Ghi nhận thời gian bán là thời gian hiện tại.
                        productSerial.OrderId = order.Id; // Liên kết serial với mã đơn hàng.

                        orderDetail.OrderSerials.Add(new OrderSerial // Thêm bản ghi liên kết OrderSerial để truy vết lịch sử bảo hành.
                        {
                            OrderDetailId = orderDetail.Id, // Mã dòng chi tiết đơn hàng.
                            SerialId = productSerial.Id // Mã thực thể serial sản phẩm.
                        });

                        variantIdsToSync.Add(productSerial.VariantId); // Đưa VariantId vào danh sách cần đồng bộ tồn kho.
                    }
                }

                order.Status = (byte)OrderStatus.Exported; // Chuyển trạng thái đơn hàng sang Đã xuất kho (Exported).

                await _unitOfWork.SaveChangesAsync(); // Lưu các thay đổi của đơn hàng, serial và liên kết xuống database.
                await _unitOfWork.CommitAsync(); // Hoàn tất và ghi nhận toàn bộ giao dịch vào cơ sở dữ liệu (Commit Transaction).

                if (variantIdsToSync.Any()) // Nếu có danh sách các biến thể sản phẩm cần đồng bộ.
                {
                    await _inventorySyncService.SyncStockBatchAsync(variantIdsToSync); // Đồng bộ tồn kho thực tế của các biến thể sản phẩm này.
                }

                _logger.LogInformation("Xuất kho thành công cho đơn hàng {OrderId} ({OrderCode})", order.Id, order.OrderCode); // Ghi log thông báo xuất kho thành công.

                return ApiResult<bool>.Ok(true, "Xuất kho thành công. Đơn hàng chuyển sang trạng thái Đã xuất kho."); // Trả về kết quả xuất kho thành công.
            }
            catch (Exception ex) // Bắt lỗi xảy ra trong quá trình xuất kho.
            {
                await _unitOfWork.RollbackAsync(); // Hủy bỏ toàn bộ giao dịch để tránh làm sai lệch dữ liệu (Rollback Transaction).
                _logger.LogError(ex, "Lỗi khi xuất kho cho đơn hàng {OrderId}", order.Id); // Ghi log chi tiết lỗi hệ thống.
                return ApiResult<bool>.Fail($"Lỗi khi xuất kho: {ex.Message}"); // Trả về thông báo lỗi cho người dùng.
            }
        }

        public async Task<ApiResult<bool>> ValidateSerialAsync(string serialNo, int variantId) // Định nghĩa phương thức kiểm tra tính hợp lệ của serial.
        {
            var serial = await _serialRepo.GetBySerialNumberAsync(serialNo); // Lấy thông tin serial từ repo theo mã số serial.

            if (serial == null) // Nếu mã serial không tồn tại trong hệ thống.
                return ApiResult<bool>.Fail($"Mã Serial '{serialNo}' không tồn tại trong hệ thống."); // Báo lỗi không tồn tại.

            if (serial.VariantId != variantId) // Nếu mã serial thuộc biến thể sản phẩm khác biến thể được yêu cầu.
                return ApiResult<bool>.Fail($"Mã Serial '{serialNo}' không thuộc sản phẩm yêu cầu."); // Báo lỗi lệch biến thể sản phẩm.

            if (serial.Status != (byte)SerialStatus.Available) // Nếu serial không ở trạng thái sẵn sàng để bán.
                return ApiResult<bool>.Fail($"Mã Serial '{serialNo}' không ở trạng thái Available (có thể đã bán hoặc hỏng)."); // Báo lỗi không khả dụng.

            return ApiResult<bool>.Ok(true); // Trả về kết quả kiểm tra thành công.
        }
    }
}

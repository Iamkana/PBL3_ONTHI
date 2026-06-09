using Microsoft.Extensions.Logging; // Sử dụng ghi log hệ thống.
using PBL3.Core.Entities; // Sử dụng các thực thể ImportReceipt, ImportReceiptDetail, ProductSerial.
using PBL3.Core.Interfaces; // Sử dụng các giao diện repository.
using PBL3.Shared.DTOs.Common; // Sử dụng DTO dùng chung ApiResult.
using PBL3.Shared.DTOs.Inventory; // Sử dụng các DTO của module kho hàng.
using PBL3.Shared.DTOs.Products; // Sử dụng các DTO sản phẩm.

namespace PBL3.Application.ImportReceipts // Khai báo namespace cho tầng Application của module nhập kho.
{
    public class ImportReceiptService( // Khai báo lớp ImportReceiptService sử dụng Primary Constructor.
        IImportReceiptRepository receiptRepo, // Tiêm repository phiếu nhập kho.
        IProductSerialRepository serialRepo, // Tiêm repository số serial sản phẩm.
        ISupplierRepository supplierRepo, // Tiêm repository nhà cung cấp.
        IProductRepository productRepo, // Tiêm repository sản phẩm.
        IUnitOfWork unitOfWork, // Tiêm UnitOfWork quản lý transaction.
        IInventorySyncService inventorySyncService, // Tiêm dịch vụ đồng bộ hóa tồn kho.
        ILogger<ImportReceiptService> logger) : IImportReceiptService // Tiêm Logger và kế thừa giao diện IImportReceiptService.
    {
        private readonly IImportReceiptRepository _receiptRepo = // Gán repository phiếu nhập vào trường thành viên.
            receiptRepo ?? throw new ArgumentNullException(nameof(receiptRepo)); // Kiểm tra null cho receiptRepo.
        private readonly IProductSerialRepository _serialRepo = // Gán repository số serial vào trường thành viên.
            serialRepo ?? throw new ArgumentNullException(nameof(serialRepo)); // Kiểm tra null cho serialRepo.
        private readonly ISupplierRepository _supplierRepo = // Gán repository nhà cung cấp vào trường thành viên.
            supplierRepo ?? throw new ArgumentNullException(nameof(supplierRepo)); // Kiểm tra null cho supplierRepo.
        private readonly IProductRepository _productRepo = // Gán repository sản phẩm vào trường thành viên.
            productRepo ?? throw new ArgumentNullException(nameof(productRepo)); // Kiểm tra null cho productRepo.
        private readonly IUnitOfWork _unitOfWork = // Gán UnitOfWork vào trường thành viên.
            unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork)); // Kiểm tra null cho unitOfWork.
        private readonly IInventorySyncService _inventorySyncService = // Gán dịch vụ đồng bộ tồn kho vào trường thành viên.
            inventorySyncService ?? throw new ArgumentNullException(nameof(inventorySyncService)); // Kiểm tra null cho inventorySyncService.
        private readonly ILogger<ImportReceiptService> _logger = // Gán Logger ghi log vào trường thành viên.
            logger ?? throw new ArgumentNullException(nameof(logger)); // Kiểm tra null cho logger.

        public async Task<ApiResult<ImportReceiptDto>> CreateAsync(CreateImportReceiptRequest request) // Phương thức tạo mới phiếu nhập kho bất đồng bộ.
        {
            var supplier = await _supplierRepo.GetByIdAsync(request.SupplierId); // Kiểm tra nhà cung cấp có tồn tại hay không.
            if (supplier == null) // Nếu nhà cung cấp không tồn tại.
                return ApiResult<ImportReceiptDto>.Fail("Nhà cung cấp không tồn tại hoặc đã bị xoá."); // Trả về thông báo lỗi.

            var allSerials = request.Details // Lấy toàn bộ danh sách mã serial được gửi lên trong yêu cầu nhập kho.
                .SelectMany(d => d.SerialNumbers) // Gộp tất cả danh sách serial từ các chi tiết mặt hàng.
                .Select(s => s.Trim()) // Cắt bỏ khoảng trắng thừa của từng mã serial.
                .ToList(); // Chuyển đổi sang danh sách List.

            var duplicateInternal = allSerials // Tìm kiếm các mã serial bị trùng lặp ngay trong chính yêu cầu gửi lên.
                .GroupBy(s => s, StringComparer.OrdinalIgnoreCase) // Nhóm các mã serial không phân biệt chữ hoa chữ thường.
                .Where(g => g.Count() > 1) // Lọc các nhóm có số lượng xuất hiện lớn hơn 1.
                .Select(g => g.Key) // Lấy ra mã serial bị trùng.
                .ToList(); // Chuyển đổi thành danh sách List.

            if (duplicateInternal.Any()) // Nếu phát hiện có serial trùng lặp nội bộ trong phiếu.
                return ApiResult<ImportReceiptDto>.Fail( // Trả về thông báo lỗi trùng lặp.
                    $"Mã Serial bị trùng lặp trong phiếu nhập: {string.Join(", ", duplicateInternal)}"); // Thông báo rõ các mã serial bị trùng.

            var existingSerials = await _serialRepo.GetExistingSerialsAsync(allSerials); // Kiểm tra các mã serial này đã tồn tại trong cơ sở dữ liệu chưa.
            if (existingSerials.Any()) // Nếu có mã serial đã tồn tại từ trước trên hệ thống.
                return ApiResult<ImportReceiptDto>.Fail( // Báo lỗi trùng lặp serial hệ thống.
                    $"Mã Serial đã tồn tại trong hệ thống: {string.Join(", ", existingSerials)}"); // Thông báo chi tiết các mã serial bị trùng.

            var variantIds = request.Details.Select(d => d.VariantId).Distinct().ToList(); // Lấy danh sách duy nhất các mã biến thể sản phẩm (VariantId) trong phiếu nhập.
            var existingVariants = await _productRepo.GetExistingVariantIdsAsync(variantIds); // Lấy danh sách các VariantId thực tế tồn tại trong cơ sở dữ liệu.

            var missingVariants = variantIds.Except(existingVariants).ToList(); // Tìm các VariantId được yêu cầu nhập nhưng không tồn tại trong DB.
            if (missingVariants.Any()) // Nếu phát hiện có biến thể không tồn tại.
                return ApiResult<ImportReceiptDto>.Fail( // Báo lỗi không tồn tại biến thể sản phẩm.
                    $"Biến thể sản phẩm không tồn tại: {string.Join(", ", missingVariants)}"); // Liệt kê cụ thể các mã biến thể thiếu.

            await _unitOfWork.BeginTransactionAsync(); // Bắt đầu một giao dịch (Transaction) cơ sở dữ liệu thông qua UnitOfWork.

            try // Khối bẫy lỗi.
            {
                var receiptCode = await GenerateReceiptCodeAsync(); // Sinh mã phiếu nhập kho tự động.

                var totalAmount = request.Details // Tính tổng số tiền của phiếu nhập kho.
                    .Sum(d => d.Quantity * d.ImportPrice); // Cộng tổng tiền của từng mặt hàng (Số lượng x Giá nhập).

                var receipt = new ImportReceipt // Khởi tạo thực thể phiếu nhập kho mới.
                {
                    ReceiptCode = receiptCode, // Gán mã phiếu nhập kho.
                    SupplierId = request.SupplierId, // Gán mã nhà cung cấp.
                    EmployeeId = Guid.Empty, // Tạm thời gán Guid rỗng do chưa có thông tin nhân viên đăng nhập.
                    ImportDate = DateTime.UtcNow, // Gán ngày nhập là thời gian UTC hiện tại.
                    TotalAmount = totalAmount, // Gán tổng tiền phiếu nhập.
                    Note = request.Note?.Trim(), // Gán ghi chú đã cắt khoảng trắng thừa.
                    IsDeleted = false // Đánh dấu chưa bị xóa.
                };

                await _receiptRepo.AddAsync(receipt); // Thêm thực thể phiếu nhập mới vào DB Context.
                await _unitOfWork.SaveChangesAsync(); // Lưu thay đổi tạm thời xuống DB để lấy mã Id tự sinh của phiếu nhập.

                var allNewSerials = new List<ProductSerial>(); // Khởi tạo danh sách chứa tất cả các thực thể serial vật lý mới sẽ tạo.

                foreach (var detailReq in request.Details) // Duyệt qua từng chi tiết dòng hàng trong yêu cầu nhập.
                {
                    var detail = new ImportReceiptDetail // Khởi tạo thực thể chi tiết phiếu nhập kho mới.
                    {
                        ReceiptId = receipt.Id, // Gán mã phiếu nhập vừa được sinh Id.
                        VariantId = detailReq.VariantId, // Gán mã biến thể sản phẩm.
                        Quantity = detailReq.Quantity, // Gán số lượng nhập.
                        ImportPrice = detailReq.ImportPrice // Gán giá nhập của biến thể.
                    };

                    await _receiptRepo.AddDetailAsync(detail); // Thêm chi tiết phiếu nhập vào DB Context thông qua repository.

                    foreach (var serialNumber in detailReq.SerialNumbers) // Duyệt qua danh sách số serial của dòng mặt hàng hiện tại.
                    {
                        allNewSerials.Add(new ProductSerial // Tạo mới thực thể ProductSerial vật lý.
                        {
                            SerialNumber = serialNumber.Trim(), // Gán mã serial.
                            VariantId = detailReq.VariantId, // Gán mã biến thể sản phẩm.
                            ImportReceiptId = receipt.Id, // Gán mã phiếu nhập kho.
                            Status = 0, // Thiết lập trạng thái 0 (Available - Sẵn sàng bán).
                            CreatedDate = DateTime.UtcNow // Gán thời điểm tạo.
                        });
                    }
                }

                await _serialRepo.AddRangeAsync(allNewSerials); // Thêm hàng loạt (Bulk Add) danh sách serial vật lý mới vào DB Context.

                await _unitOfWork.SaveChangesAsync(); // Lưu tất cả thay đổi chi tiết phiếu nhập và danh sách serial xuống DB.

                var importedVariantIds = request.Details.Select(d => d.VariantId).Distinct().ToList(); // Lấy danh sách VariantId vừa được nhập hàng.
                await _inventorySyncService.SyncStockBatchAsync(importedVariantIds); // Kích hoạt dịch vụ đồng bộ số lượng tồn kho của các biến thể này dựa trên serial thực tế.

                await _unitOfWork.CommitAsync(); // Xác nhận hoàn tất và áp dụng toàn bộ giao dịch (Commit Transaction).

                _logger.LogInformation( // Ghi log thông báo tạo phiếu nhập kho thành công.
                    "Tạo phiếu nhập kho thành công: {ReceiptCode}, NCC: {SupplierName}, Tổng: {TotalAmount}", // Chuỗi định dạng log.
                    receiptCode, supplier.Name, totalAmount); // Các tham số truyền vào log.

                var resultDto = new ImportReceiptDto // Khởi tạo DTO kết quả trả về.
                {
                    Id = receipt.Id, // Gán Id.
                    ReceiptCode = receipt.ReceiptCode, // Gán mã phiếu nhập.
                    SupplierId = receipt.SupplierId, // Gán mã nhà cung cấp.
                    SupplierName = supplier.Name, // Gán tên nhà cung cấp.
                    EmployeeName = "Hệ thống", // Mặc định hiển thị Hệ thống.
                    ImportDate = receipt.ImportDate, // Gán ngày nhập.
                    TotalAmount = receipt.TotalAmount, // Gán tổng số tiền.
                    Note = receipt.Note // Gán ghi chú.
                };

                return ApiResult<ImportReceiptDto>.Ok(resultDto, "Tạo phiếu nhập kho thành công."); // Trả về kết quả thành công kèm DTO.
            }
            catch (Exception ex) // Bắt bất kỳ ngoại lệ nào xảy ra trong quá trình thực thi giao dịch.
            {
                await _unitOfWork.RollbackAsync(); // Hủy bỏ toàn bộ các thay đổi trong giao dịch để bảo toàn tính nhất quán (Rollback Transaction).

                _logger.LogError(ex, "Lỗi khi tạo phiếu nhập kho."); // Ghi log chi tiết lỗi hệ thống.

                return ApiResult<ImportReceiptDto>.Fail("Đã xảy ra lỗi khi tạo phiếu nhập kho. Vui lòng thử lại."); // Trả về thông báo lỗi cho người dùng.
            }
        }

        public async Task<ApiResult<PagedResult<ImportReceiptDto>>> GetPagedListAsync(ImportReceiptFilterRequest filter) // Phương thức lấy danh sách phiếu nhập phân trang bất đồng bộ.
        {
            var (items, totalCount) = await _receiptRepo.GetPagedListAsync( // Gọi repo lấy danh sách phiếu nhập và tổng số lượng khớp bộ lọc.
                filter.Keyword, // Lọc theo từ khóa.
                filter.FromDate, // Lọc theo ngày bắt đầu.
                filter.ToDate, // Lọc theo ngày kết thúc.
                filter.SupplierId, // Lọc theo mã nhà cung cấp.
                filter.PageNumber, // Trang hiện tại.
                filter.PageSize, // Kích thước trang.
                filter.SortBy, // Cột sắp xếp.
                filter.SortDescending); // Sắp xếp giảm dần hay không.

            var dtos = items.Select(r => new ImportReceiptDto // Ánh xạ danh sách thực thể sang danh sách DTO.
            {
                Id = r.Id, // Ánh xạ Id.
                ReceiptCode = r.ReceiptCode, // Ánh xạ mã phiếu nhập.
                SupplierId = r.SupplierId, // Ánh xạ mã nhà cung cấp.
                SupplierName = r.Supplier?.Name ?? string.Empty, // Ánh xạ tên nhà cung cấp.
                EmployeeName = "Hệ thống", // Mặc định tên nhân viên là Hệ thống.
                ImportDate = r.ImportDate, // Ánh xạ ngày nhập.
                TotalAmount = r.TotalAmount, // Ánh xạ tổng tiền.
                Note = r.Note // Ánh xạ ghi chú.
            }).ToList(); // Chuyển sang List.

            var result = new PagedResult<ImportReceiptDto> // Khởi tạo kết quả phân trang DTO.
            {
                Items = dtos, // Gán danh sách DTO.
                TotalCount = totalCount, // Gán tổng số bản ghi.
                PageNumber = filter.PageNumber, // Gán trang hiện tại.
                PageSize = filter.PageSize // Gán số lượng phần tử trên trang.
            };

            return ApiResult<PagedResult<ImportReceiptDto>>.Ok(result); // Trả về kết quả phân trang thành công.
        }

        public async Task<ApiResult<ImportReceiptDto>> GetByIdAsync(int id) // Phương thức lấy chi tiết phiếu nhập kho theo Id.
        {
            var receipt = await _receiptRepo.GetByIdWithDetailsAsync(id); // Lấy thông tin phiếu nhập kho kèm theo các dòng chi tiết từ repo.

            if (receipt == null) // Nếu không tìm thấy phiếu nhập.
                return ApiResult<ImportReceiptDto>.Fail("Không tìm thấy phiếu nhập kho yêu cầu.", ApiErrorCode.NotFound); // Trả về lỗi NotFound.

            var detailDtos = new List<ImportReceiptDetailDto>(); // Khởi tạo danh sách chứa các DTO chi tiết phiếu nhập.

            foreach (var detail in receipt.Details) // Duyệt qua từng chi tiết mặt hàng trong phiếu nhập.
            {
                var serials = await _serialRepo.GetSerialsByReceiptAndVariantAsync(receipt.Id, detail.VariantId); // Lấy danh sách các số serial thuộc phiếu nhập và biến thể sản phẩm hiện tại.

                detailDtos.Add(new ImportReceiptDetailDto // Thêm chi tiết dòng hàng vào danh sách DTO.
                {
                    Id = detail.Id, // Ánh xạ Id chi tiết.
                    VariantId = detail.VariantId, // Ánh xạ mã biến thể.
                    VariantName = detail.Variant?.VariantName ?? string.Empty, // Ánh xạ tên biến thể.
                    SKU = detail.Variant?.SKU ?? string.Empty, // Ánh xạ SKU.
                    Quantity = detail.Quantity, // Ánh xạ số lượng.
                    ImportPrice = detail.ImportPrice, // Ánh xạ giá nhập.
                    SubTotal = detail.Quantity * detail.ImportPrice, // Tính thành tiền.
                    SerialNumbers = serials // Gán danh sách số serial tương ứng.
                });
            }

            var dto = new ImportReceiptDto // Khởi tạo DTO thông tin phiếu nhập kho đầy đủ.
            {
                Id = receipt.Id, // Gán Id.
                ReceiptCode = receipt.ReceiptCode, // Gán mã phiếu.
                SupplierId = receipt.SupplierId, // Gán mã nhà cung cấp.
                SupplierName = receipt.Supplier?.Name ?? string.Empty, // Gán tên nhà cung cấp.
                EmployeeName = "Hệ thống", // Gán tên nhân viên là Hệ thống.
                ImportDate = receipt.ImportDate, // Gán ngày nhập.
                TotalAmount = receipt.TotalAmount, // Gán tổng tiền.
                Note = receipt.Note, // Gán ghi chú.
                Details = detailDtos // Gán danh sách chi tiết kèm serial.
            };

            return ApiResult<ImportReceiptDto>.Ok(dto); // Trả về kết quả chi tiết thành công.
        }

        private async Task<string> GenerateReceiptCodeAsync() // Hàm phụ trợ sinh mã phiếu nhập kho tự động bất đồng bộ.
        {
            var dateStr = DateTime.UtcNow.ToString("yyyyMMdd"); // Lấy chuỗi ngày tháng năm hiện tại định dạng yyyyMMdd.
            var prefix = $"PN-{dateStr}-"; // Tạo tiền tố mã phiếu nhập (Ví dụ: PN-20260531-).

            var lastCode = await _receiptRepo.GetLastReceiptCodeByDateAsync(prefix); // Gọi repo tìm mã phiếu nhập cuối cùng trong ngày có tiền tố này.

            int nextNumber = 1; // Khởi tạo số thứ tự phiếu nhập tiếp theo mặc định là 1.
            if (!string.IsNullOrEmpty(lastCode)) // Nếu tìm thấy mã phiếu nhập đã tồn tại trong ngày.
            {
                var lastPart = lastCode.Substring(prefix.Length); // Cắt lấy phần số thứ tự phía sau tiền tố.
                if (int.TryParse(lastPart, out int lastNumber)) // Chuyển đổi chuỗi số thứ tự sang kiểu số nguyên.
                {
                    nextNumber = lastNumber + 1; // Tăng số thứ tự lên 1 để cho phiếu tiếp theo.
                }
            }

            return $"{prefix}{nextNumber:D3}"; // Định dạng chuỗi mã phiếu mới với số thứ tự có 3 chữ số (Ví dụ: PN-20260531-001).
        }
    }
}

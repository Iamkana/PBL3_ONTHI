using Microsoft.EntityFrameworkCore; // Sử dụng các phương thức mở rộng Async của EF Core.
using Microsoft.Extensions.Logging; // Sử dụng để ghi log lịch sử hệ thống và lỗi.
using PBL3.Core.Entities; // Sử dụng các thực thể nghiệp vụ cốt lõi như InventoryCheck, InventoryCheckDetail.
using PBL3.Core.Interfaces; // Sử dụng các interface định nghĩa Repository và Service của Core.
using PBL3.Infrastructure.Data; // Sử dụng DbContext HushStoreDbContext để truy vấn dữ liệu trực tiếp.
using PBL3.Shared.DTOs.Common; // Sử dụng DTO dùng chung ApiResult và PagedResult.
using PBL3.Shared.DTOs.Inventory; // Sử dụng các DTO quản lý kho hàng và kiểm kê.
using PBL3.Shared.Enums; // Sử dụng các Enum trạng thái và kiểu phạm vi kiểm kê.

namespace PBL3.Application.Inventory // Khai báo namespace cho lớp dịch vụ kiểm kê kho.
{
    public class InventoryCheckService( // Khởi tạo lớp InventoryCheckService sử dụng Primary Constructor.
        IInventoryCheckRepository checkRepo, // Tiêm repository quản lý phiếu kiểm kê.
        IProductSerialRepository serialRepo, // Tiêm repository quản lý serial sản phẩm.
        IProductRepository productRepo, // Tiêm repository quản lý sản phẩm.
        IInventorySyncService inventorySyncService, // Tiêm dịch vụ đồng bộ số lượng tồn kho khả dụng.
        IUnitOfWork unitOfWork, // Tiêm UnitOfWork để quản lý các giao dịch cơ sở dữ liệu.
        HushStoreDbContext context, // Tiêm DbContext để thực hiện các thao tác truy vấn trực tiếp.
        ILogger<InventoryCheckService> logger) : IInventoryCheckService // Tiêm Logger và kế thừa giao diện IInventoryCheckService.
    {
        private readonly IInventoryCheckRepository _checkRepo = // Khai báo trường lưu trữ repository kiểm kê.
            checkRepo ?? throw new ArgumentNullException(nameof(checkRepo)); // Gán giá trị và kiểm tra null.
        private readonly IProductSerialRepository _serialRepo = // Khai báo trường lưu trữ repository serial.
            serialRepo ?? throw new ArgumentNullException(nameof(serialRepo)); // Gán giá trị và kiểm tra null.
        private readonly IProductRepository _productRepo = // Khai báo trường lưu trữ repository sản phẩm.
            productRepo ?? throw new ArgumentNullException(nameof(productRepo)); // Gán giá trị và kiểm tra null.
        private readonly IInventorySyncService _inventorySyncService = // Khai báo trường lưu trữ dịch vụ đồng bộ.
            inventorySyncService ?? throw new ArgumentNullException(nameof(inventorySyncService)); // Gán giá trị và kiểm tra null.
        private readonly IUnitOfWork _unitOfWork = // Khai báo trường lưu trữ UnitOfWork.
            unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork)); // Gán giá trị và kiểm tra null.
        private readonly HushStoreDbContext _context = // Khai báo trường lưu trữ DbContext.
            context ?? throw new ArgumentNullException(nameof(context)); // Gán giá trị và kiểm tra null.
        private readonly ILogger<InventoryCheckService> _logger = // Khai báo trường lưu trữ Logger.
            logger ?? throw new ArgumentNullException(nameof(logger)); // Gán giá trị và kiểm tra null.

        public async Task<ApiResult<InventoryCheckDto>> CreateAsync(CreateInventoryCheckRequest request, Guid employeeId) // Tạo mới phiếu kiểm kê và chốt snapshot tồn kho sổ sách.
        {
            if (request.ScopeType == (byte)InventoryCheckScopeType.Category) // Kiểm tra nếu phạm vi kiểm kê là theo danh mục.
            {
                if (!request.ScopeCategoryId.HasValue) // Nếu không truyền mã danh mục.
                    return ApiResult<InventoryCheckDto>.Fail("Phải chọn danh mục khi phạm vi kiểm kê là theo danh mục."); // Trả về lỗi yêu cầu tham số.

                var categoryExists = await _context.Categories // Truy vấn kiểm tra danh mục có tồn tại trong cơ sở dữ liệu hay không.
                    .AnyAsync(c => c.Id == request.ScopeCategoryId.Value); // Sử dụng AnyAsync bất đồng bộ.
                if (!categoryExists) // Nếu danh mục không tồn tại.
                    return ApiResult<InventoryCheckDto>.Fail("Danh mục kiểm kê không tồn tại."); // Trả về lỗi danh mục không tồn tại.
            }

            await _unitOfWork.BeginTransactionAsync(); // Bắt đầu một giao dịch cơ sở dữ liệu thông qua UnitOfWork để bảo toàn tính nhất quán.
            try // Khối try bắt đầu xử lý nghiệp vụ chính.
            {
                var now = DateTime.UtcNow; // Lấy thời gian UTC hiện tại.
                var checkCode = await GenerateCheckCodeAsync(); // Tự động sinh mã phiếu kiểm kê mới.

                var check = new InventoryCheck // Khởi tạo thực thể phiếu kiểm kê mới.
                {
                    CheckCode = checkCode, // Gán mã phiếu kiểm kê.
                    EmployeeId = employeeId, // Gán Id nhân viên thực hiện tạo phiếu.
                    CheckDate = now, // Gán ngày kiểm kê.
                    SnapshotAt = now, // Gán mốc thời gian chốt số lượng sổ sách.
                    Status = (byte)InventoryCheckStatus.Draft, // Đặt trạng thái mặc định là Nháp.
                    ScopeType = request.ScopeType, // Gán phạm vi kiểm kê (Toàn bộ / Theo danh mục).
                    ScopeCategoryId = request.ScopeCategoryId, // Gán mã danh mục (nếu có).
                    Note = request.Note?.Trim(), // Gán ghi chú đã cắt khoảng trắng.
                    IsDeleted = false // Đánh dấu chưa bị xóa.
                };

                await _checkRepo.AddAsync(check); // Thêm phiếu kiểm kê vào DB Context.
                await _unitOfWork.SaveChangesAsync(); // Lưu thay đổi tạm thời xuống DB để lấy mã Id tự sinh của phiếu kiểm kê.

                var variantIds = await GetVariantIdsInScopeAsync(request.ScopeType, request.ScopeCategoryId); // Lấy danh sách Id biến thể sản phẩm nằm trong phạm vi kiểm kê.
                if (!variantIds.Any()) // Nếu không tìm thấy biến thể nào trong phạm vi được chọn.
                {
                    await _unitOfWork.RollbackAsync(); // Hủy bỏ giao dịch.
                    return ApiResult<InventoryCheckDto>.Fail("Không có sản phẩm nào trong phạm vi kiểm kê."); // Trả về lỗi phạm vi rỗng.
                }

                var availableSerials = await _serialRepo.GetAvailableSerialsBatchAsync(variantIds); // Lấy toàn bộ danh sách serial khả dụng (Available) làm snapshot đối chiếu.

                var detailMap = new Dictionary<int, InventoryCheckDetail>(); // Khởi tạo Dictionary map để lưu trữ nhanh các dòng chi tiết theo VariantId.
                foreach (var variantId in variantIds) // Duyệt qua danh sách Id các biến thể trong phạm vi.
                {
                    var sysQty = availableSerials.Count(s => s.VariantId == variantId); // Đếm số lượng serial sổ sách khả dụng cho biến thể hiện tại.
                    var detail = new InventoryCheckDetail // Khởi tạo dòng chi tiết kiểm kê cho biến thể.
                    {
                        CheckId = check.Id, // Liên kết với mã phiếu kiểm kê.
                        VariantId = variantId, // Mã biến thể sản phẩm.
                        SystemQuantity = sysQty, // Số lượng sổ sách chốt tại thời điểm này.
                        ActualQuantity = 0, // Số lượng thực tế quét ban đầu mặc định là 0.
                        MatchedQuantity = 0, // Số lượng khớp thực tế quét ban đầu mặc định là 0.
                        MissingQuantity = 0, // Số lượng thiếu ban đầu mặc định là 0.
                        SurplusQuantity = 0, // Số lượng thừa ban đầu mặc định là 0.
                        DefectiveQuantity = 0 // Số lượng hàng lỗi vật lý quét được mặc định là 0.
                    };
                    await _checkRepo.AddDetailAsync(detail); // Lưu chi tiết kiểm kê vào DB Context.
                    detailMap[variantId] = detail; // Lưu vào map để tra cứu nhanh ở bước sau.
                }

                await _unitOfWork.SaveChangesAsync(); // Lưu thay đổi xuống cơ sở dữ liệu để lấy Id của các bản ghi chi tiết vừa tạo.

                var snapshotRows = availableSerials.Select(s => new InventoryCheckDetailSerial // Ánh xạ danh sách serial sổ sách sang danh sách serial chi tiết kiểm kê.
                {
                    CheckId = check.Id, // Mã phiếu kiểm kê.
                    DetailId = detailMap.TryGetValue(s.VariantId, out var d) ? d.Id : null, // Gán Id chi tiết tương ứng từ map.
                    VariantId = s.VariantId, // Gán mã biến thể.
                    SerialId = s.SerialId, // Gán mã thực thể serial.
                    SerialNumberRaw = s.SerialNumber, // Gán chuỗi serial.
                    OriginalStatus = (byte)SerialStatus.Available, // Trạng thái gốc trên sổ sách là Khả dụng.
                    ScanStatus = (byte)InventoryScanStatus.Pending, // Đặt trạng thái quét ban đầu là Chờ quét (Pending).
                    ScannedAt = null // Thời gian quét ban đầu là null.
                }).ToList(); // Chuyển kết quả Select sang danh sách List.

                await _checkRepo.AddDetailSerialsAsync(snapshotRows); // Thêm hàng loạt các dòng chi tiết serial vào cơ sở dữ liệu.
                await _unitOfWork.SaveChangesAsync(); // Lưu thay đổi chi tiết serial xuống DB.
                await _unitOfWork.CommitAsync(); // Hoàn tất giao dịch (Commit Transaction).

                _logger.LogInformation( // Ghi log thông báo tạo phiếu kiểm kê thành công.
                    "Tạo phiếu kiểm kê: {CheckCode}, Phạm vi: {ScopeType}, Snapshot: {Total} serials", // Chuỗi định dạng log.
                    checkCode, request.ScopeType, availableSerials.Count); // Tham số log.

                var dto = await BuildCheckDtoAsync(check.Id); // Xây dựng DTO phản hồi chi tiết của phiếu kiểm kê vừa tạo.
                return ApiResult<InventoryCheckDto>.Ok(dto!, "Tạo phiếu kiểm kê thành công."); // Trả về kết quả thành công chứa DTO.
            }
            catch (Exception ex) // Bắt lỗi xảy ra trong quá trình xử lý.
            {
                await _unitOfWork.RollbackAsync(); // Hủy bỏ giao dịch để khôi phục trạng thái DB.
                _logger.LogError(ex, "Lỗi khi tạo phiếu kiểm kê."); // Ghi log chi tiết lỗi.
                return ApiResult<InventoryCheckDto>.Fail("Đã xảy ra lỗi khi tạo phiếu kiểm kê. Vui lòng thử lại."); // Trả về thông báo lỗi cho người dùng.
            }
        }

        public async Task<ApiResult<PagedResult<InventoryCheckListItemDto>>> GetPagedListAsync(InventoryCheckFilterRequest filter) // Phương thức lấy danh sách phiếu kiểm kê phân trang.
        {
            var (items, totalCount) = await _checkRepo.GetPagedListAsync( // Gọi repository lấy danh sách phiếu và tổng số lượng khớp bộ lọc.
                filter.Keyword, filter.Status, filter.FromDate, filter.ToDate, // Truyền từ khóa, trạng thái, và khoảng thời gian lọc.
                filter.EmployeeId, filter.PageNumber, filter.PageSize, // Truyền mã nhân viên, trang hiện tại, kích thước trang.
                filter.SortBy, filter.SortDescending); // Truyền thông tin sắp xếp.

            var employeeIds = items.Select(c => c.EmployeeId).Distinct().ToList(); // Lấy danh sách Id nhân viên duy nhất từ các phiếu kiểm kê.
            var employeeNames = await GetEmployeeNamesAsync(employeeIds); // Lấy họ tên nhân viên tương ứng từ các Id trên.

            var dtos = items.Select(c => new InventoryCheckListItemDto // Ánh xạ danh sách thực thể sang DTO hiển thị danh sách.
            {
                Id = c.Id, // Mã Id phiếu.
                CheckCode = c.CheckCode, // Mã phiếu kiểm kê.
                EmployeeName = employeeNames.GetValueOrDefault(c.EmployeeId, c.EmployeeId.ToString()), // Lấy tên nhân viên hoặc dùng Id nếu không tìm thấy.
                CheckDate = c.CheckDate, // Ngày lập phiếu.
                SnapshotAt = c.SnapshotAt, // Ngày giờ chốt số lượng sổ sách.
                Status = c.Status, // Mã trạng thái phiếu.
                StatusName = GetStatusName(c.Status), // Tên trạng thái tương ứng.
                ScopeType = c.ScopeType, // Kiểu phạm vi kiểm kê.
                ScopeCategoryName = c.ScopeCategory?.Name, // Tên danh mục áp dụng phạm vi (nếu có).
                TotalVariants = c.Details?.Count ?? 0, // Số lượng dòng biến thể sản phẩm kiểm kê.
                Note = c.Note // Ghi chú phiếu.
            }).ToList(); // Chuyển kết quả sang List.

            return ApiResult<PagedResult<InventoryCheckListItemDto>>.Ok(new PagedResult<InventoryCheckListItemDto> // Trả về kết quả phân trang thành công.
            {
                Items = dtos, // Gán danh sách DTO.
                TotalCount = totalCount, // Gán tổng số lượng bản ghi.
                PageNumber = filter.PageNumber, // Gán trang hiện tại.
                PageSize = filter.PageSize // Gán kích thước trang.
            });
        }

        public async Task<ApiResult<InventoryCheckDto>> GetByIdAsync(int id) // Phương thức lấy chi tiết phiếu kiểm kê theo Id.
        {
            var dto = await BuildCheckDtoAsync(id); // Gọi hàm dựng DTO chi tiết phiếu kiểm kê.
            if (dto == null) // Nếu không tìm thấy thông tin phiếu.
                return ApiResult<InventoryCheckDto>.Fail("Không tìm thấy phiếu kiểm kê yêu cầu.", ApiErrorCode.NotFound); // Trả về mã lỗi NotFound.
            return ApiResult<InventoryCheckDto>.Ok(dto); // Trả về kết quả thành công kèm DTO chi tiết.
        }

        public async Task<ApiResult<InventoryCheckDashboardDto>> GetDashboardAsync(int id) // Phương thức lấy dữ liệu đối chiếu số liệu kiểm kê thời gian thực.
        {
            var check = await _checkRepo.GetByIdWithDetailsAsync(id); // Lấy thông tin phiếu kiểm kê kèm chi tiết từ repository.
            if (check == null) // Nếu không tìm thấy phiếu kiểm kê.
                return ApiResult<InventoryCheckDashboardDto>.Fail("Không tìm thấy phiếu kiểm kê yêu cầu.", ApiErrorCode.NotFound); // Trả về lỗi không tìm thấy.

            var counts = await _checkRepo.GetGroupedCountsByCheckAsync(id); // Lấy số lượng serial theo nhóm trạng thái quét (matched, missing, surplus...).

            var matched = counts.GetValueOrDefault((byte)InventoryScanStatus.Matched, 0); // Lấy số lượng khớp quét, mặc định là 0.
            var missing = counts.GetValueOrDefault((byte)InventoryScanStatus.Missing, 0); // Lấy số lượng thiếu quét, mặc định là 0.
            var surplus = counts.GetValueOrDefault((byte)InventoryScanStatus.Surplus, 0); // Lấy số lượng thừa quét, mặc định là 0.
            var unknown = counts.GetValueOrDefault((byte)InventoryScanStatus.UnknownSurplus, 0); // Lấy số lượng thừa mã lạ, mặc định là 0.
            var defective = counts.GetValueOrDefault((byte)InventoryScanStatus.Defective, 0); // Lấy số lượng hàng lỗi vật lý, mặc định là 0.
            var pending = counts.GetValueOrDefault((byte)InventoryScanStatus.Pending, 0); // Lấy số lượng chờ quét, mặc định là 0.

            int totalSystem = check.Details?.Sum(d => d.SystemQuantity) ?? 0; // Tính tổng số lượng sổ sách của các dòng chi tiết kiểm kê.
            int totalScanned = matched + surplus + unknown + defective; // Tính tổng số lượng đã quét thực tế (khớp + thừa + lỗi).

            var dashboard = new InventoryCheckDashboardDto // Khởi tạo DTO dashboard đối chiếu số liệu.
            {
                CheckId = check.Id, // Id phiếu kiểm kê.
                CheckCode = check.CheckCode, // Mã phiếu kiểm kê.
                Status = check.Status, // Trạng thái phiếu.
                StatusName = GetStatusName(check.Status), // Tên trạng thái phiếu.
                SnapshotAt = check.SnapshotAt, // Mốc thời gian chốt số liệu sổ sách.
                TotalSystem = totalSystem, // Tổng số lượng sổ sách chốt.
                TotalScanned = totalScanned, // Tổng số lượng thực tế đã quét.
                MatchedCount = matched, // Số lượng khớp.
                MissingCount = missing + pending, // Số lượng thiếu thực tế (thiếu đã chốt + chờ quét chưa thấy).
                SurplusCount = surplus, // Số lượng thừa.
                UnknownSurplusCount = unknown, // Số lượng thừa mã lạ.
                DefectiveCount = defective // Số lượng hàng lỗi vật lý.
            };

            return ApiResult<InventoryCheckDashboardDto>.Ok(dashboard); // Trả về kết quả thành công kèm DTO dashboard.
        }

        public async Task<ApiResult<PagedResult<InventoryCheckSerialDto>>> GetSerialsAsync(int checkId, InventoryCheckSerialFilterRequest filter) // Phương thức lấy danh sách serial phân trang của phiếu kiểm kê.
        {
            var exists = await _context.InventoryChecks.AnyAsync(c => c.Id == checkId); // Kiểm tra xem phiếu kiểm kê có tồn tại trong cơ sở dữ liệu hay không.
            if (!exists) // Nếu không tồn tại phiếu.
                return ApiResult<PagedResult<InventoryCheckSerialDto>>.Fail("Không tìm thấy phiếu kiểm kê yêu cầu.", ApiErrorCode.NotFound); // Trả về lỗi NotFound.

            var (items, totalCount) = await _checkRepo.GetDetailSerialsPagedAsync( // Lấy danh sách serial phân trang theo bộ lọc từ repository.
                checkId, filter.ScanStatus, filter.VariantId, filter.PageNumber, filter.PageSize); // Truyền trạng thái quét, mã biến thể, trang và kích thước trang.

            var dtos = items.Select(MapSerialToDto).ToList(); // Ánh xạ danh sách thực thể serial sang DTO.

            return ApiResult<PagedResult<InventoryCheckSerialDto>>.Ok(new PagedResult<InventoryCheckSerialDto> // Trả về kết quả phân trang thành công.
            {
                Items = dtos, // Gán danh sách DTO.
                TotalCount = totalCount, // Gán tổng số lượng.
                PageNumber = filter.PageNumber, // Gán trang hiện tại.
                PageSize = filter.PageSize // Gán kích thước trang.
            });
        }

        public async Task<ApiResult<ScanResultDto>> ScanSerialAsync(int checkId, ScanSerialRequest request, Guid employeeId) // Quét một mã Serial vào phiếu kiểm kê.
        {
            var check = await _checkRepo.GetByIdAsync(checkId); // Lấy thông tin phiếu kiểm kê từ repository.
            if (check == null) // Nếu phiếu không tồn tại.
                return ApiResult<ScanResultDto>.Fail("Không tìm thấy phiếu kiểm kê yêu cầu.", ApiErrorCode.NotFound); // Trả về lỗi không tìm thấy.

            if (check.Status != (byte)InventoryCheckStatus.Draft) // Nếu phiếu kiểm kê không còn ở trạng thái Nháp (Draft).
                return ApiResult<ScanResultDto>.Fail("Chỉ có thể quét khi phiếu ở trạng thái Nháp."); // Ngăn chặn hành động quét thêm serial.

            var serialNumber = request.SerialNumber.Trim(); // Cắt bỏ khoảng trắng thừa của mã serial được gửi lên.

            var alreadyScanned = await _checkRepo.IsSerialAlreadyScannedAsync(checkId, serialNumber); // Kiểm tra xem mã serial đã được quét trước đó trong phiếu kiểm kê này chưa.
            if (alreadyScanned) // Nếu mã serial bị quét trùng lắp.
            {
                return ApiResult<ScanResultDto>.Ok(new ScanResultDto // Phản hồi thông tin quét trùng mà không báo lỗi hệ thống.
                {
                    Success = false, // Đánh dấu quét không thành công (trùng lặp).
                    IsDuplicateScan = true, // Gán cờ quét trùng là true.
                    SerialNumberRaw = serialNumber, // Gán mã serial gốc.
                    Message = $"Serial [{serialNumber}] đã được quét trong phiếu này.", // Gán chuỗi thông báo lỗi nghiệp vụ.
                    ScanStatus = 0, // Thiết lập trạng thái mặc định.
                    ScanStatusName = string.Empty, // Thiết lập chuỗi trống.
                    MiniDashboard = await BuildMiniDashboardAsync(checkId, check) // Cập nhật và gửi kèm dashboard thông số mini.
                });
            }

            var now = DateTime.UtcNow; // Lấy thời gian UTC hiện tại.
            var dbSerial = await _serialRepo.GetBySerialNumberAsync(serialNumber); // Tìm kiếm thông tin thực thể serial trong cơ sở dữ liệu theo mã số quét được.

            if (dbSerial == null) // Trường hợp serial không tồn tại trong hệ thống (thừa mã lạ).
            {
                if (request.VariantIdForUnknown.HasValue) // Nếu thủ kho đề xuất mã biến thể liên kết cho serial lạ này.
                {
                    var variantExists = await _context.ProductVariants // Kiểm tra xem biến thể đề xuất có tồn tại không.
                        .AnyAsync(v => v.Id == request.VariantIdForUnknown.Value); // Sử dụng AnyAsync bất đồng bộ.
                    if (!variantExists) // Nếu biến thể không tồn tại.
                        return ApiResult<ScanResultDto>.Fail("Biến thể sản phẩm đề xuất không tồn tại."); // Trả về lỗi biến thể.
                }

                var unknownRow = new InventoryCheckDetailSerial // Khởi tạo thực thể ghi nhận serial thừa mã lạ.
                {
                    CheckId = checkId, // Liên kết mã phiếu.
                    DetailId = null, // DetailId sẽ cập nhật sau nếu xác định được biến thể sản phẩm.
                    VariantId = request.VariantIdForUnknown, // Gán mã biến thể liên kết đề xuất (nếu có).
                    SerialId = null, // Không có Id serial trong DB.
                    SerialNumberRaw = serialNumber, // Gán mã serial quét thực tế.
                    OriginalStatus = null, // Không có trạng thái gốc.
                    ScanStatus = (byte)InventoryScanStatus.UnknownSurplus, // Đặt trạng thái quét là Thừa (Mã lạ).
                    ScannedAt = now, // Ghi nhận thời điểm quét.
                    ScannedByEmployeeId = employeeId, // Ghi nhận nhân viên thực hiện quét.
                    Note = "Serial không tồn tại trong hệ thống." // Ghi chú chi tiết lý do.
                };

                await _checkRepo.AddDetailSerialAsync(unknownRow); // Lưu thực thể serial lạ vào DB Context.

                if (request.VariantIdForUnknown.HasValue) // Nếu thủ kho đã chỉ định biến thể sản phẩm cho mã lạ này.
                {
                    var detail = await _checkRepo.GetDetailByCheckAndVariantAsync(checkId, request.VariantIdForUnknown.Value, withTracking: true); // Lấy dòng chi tiết tổng hợp của biến thể.
                    if (detail != null) // Nếu dòng chi tiết tổng hợp tồn tại.
                    {
                        detail.SurplusQuantity++; // Tăng số lượng thừa (SurplusQuantity) của biến thể lên 1.
                        unknownRow.DetailId = detail.Id; // Liên kết dòng serial lạ với Id chi tiết tổng hợp.
                    }
                }

                await _checkRepo.SaveChangesAsync(); // Lưu các thay đổi xuống cơ sở dữ liệu.

                return ApiResult<ScanResultDto>.Ok(new ScanResultDto // Trả về kết quả quét thành công thừa mã lạ.
                {
                    Success = true, // Quét thành công ghi nhận dữ liệu lạ.
                    IsDuplicateScan = false, // Cờ quét trùng là false.
                    RequiresVariantInput = request.VariantIdForUnknown == null, // Yêu cầu Client hiển thị UI chọn Variant nếu chưa có VariantId.
                    SerialNumberRaw = serialNumber, // Mã serial quét được.
                    ScanStatus = (byte)InventoryScanStatus.UnknownSurplus, // Trạng thái là Thừa (Mã lạ).
                    ScanStatusName = "Thừa (Mã lạ)", // Tên trạng thái hiển thị.
                    Message = "Serial không tồn tại trong hệ thống. Đã ghi nhận là Thừa kiểm kê (Mã lạ).", // Thông báo phản hồi.
                    MiniDashboard = await BuildMiniDashboardAsync(checkId, check) // Dashboard mini đi kèm.
                });
            }

            if (dbSerial.Status == (byte)SerialStatus.Available) // Trường hợp serial tồn tại và có trạng thái gốc là Khả dụng (trong kho).
            {
                var pendingRow = await _checkRepo.GetPendingDetailSerialBySerialIdAsync(checkId, dbSerial.Id); // Tìm dòng snapshot chốt ban đầu của serial này ở trạng thái Pending.

                if (pendingRow != null) // Nếu serial nằm trong danh sách chốt ban đầu (quét khớp).
                {
                    pendingRow.ScanStatus = (byte)InventoryScanStatus.Matched; // Cập nhật trạng thái quét thành Khớp (Matched).
                    pendingRow.ScannedAt = now; // Ghi nhận thời gian quét.
                    pendingRow.ScannedByEmployeeId = employeeId; // Ghi nhận người quét.

                    if (pendingRow.DetailId.HasValue) // Nếu dòng serial liên kết với Id chi tiết tổng hợp.
                    {
                        var detail = await _checkRepo.GetDetailByCheckAndVariantAsync(checkId, dbSerial.VariantId, withTracking: true); // Lấy chi tiết tổng hợp với tracking để cập nhật số lượng.
                        if (detail != null) // Nếu chi tiết tổng hợp tồn tại.
                        {
                            detail.ActualQuantity++; // Tăng số lượng đếm thực tế (ActualQuantity) lên 1.
                            detail.MatchedQuantity++; // Tăng số lượng khớp (MatchedQuantity) lên 1.
                        }
                    }

                    await _checkRepo.SaveChangesAsync(); // Lưu các thay đổi của dòng quét khớp xuống DB.

                    return ApiResult<ScanResultDto>.Ok(new ScanResultDto // Phản hồi kết quả quét khớp thành công.
                    {
                        Success = true, // Thành công.
                        SerialNumberRaw = serialNumber, // Mã serial.
                        ScanStatus = (byte)InventoryScanStatus.Matched, // Trạng thái Khớp.
                        ScanStatusName = "Khớp", // Tên trạng thái.
                        Message = "Khớp.", // Thông báo khớp.
                        FoundSerialNumber = dbSerial.SerialNumber, // Số serial thực tế trong DB.
                        FoundVariantName = dbSerial.Variant?.VariantName, // Tên biến thể sản phẩm.
                        FoundOriginalStatus = (byte)SerialStatus.Available, // Trạng thái gốc trong kho.
                        FoundOriginalStatusName = "Trong kho", // Tên trạng thái gốc.
                        MiniDashboard = await BuildMiniDashboardAsync(checkId, check) // Dashboard mini cập nhật.
                    });
                }
                else // Trường hợp serial khả dụng trong DB nhưng nằm ngoài danh sách chốt ban đầu (ví dụ: chuyển kho nhầm/kiểm kê lệch danh mục).
                {
                    var surplusRow = new InventoryCheckDetailSerial // Khởi tạo dòng ghi nhận serial quét thừa.
                    {
                        CheckId = checkId, // Mã phiếu.
                        VariantId = dbSerial.VariantId, // Mã biến thể.
                        SerialId = dbSerial.Id, // Mã thực thể serial.
                        SerialNumberRaw = serialNumber, // Mã số serial quét.
                        OriginalStatus = (byte)SerialStatus.Available, // Trạng thái gốc là khả dụng.
                        ScanStatus = (byte)InventoryScanStatus.Surplus, // Trạng thái quét là Thừa (Surplus).
                        ScannedAt = now, // Thời điểm quét.
                        ScannedByEmployeeId = employeeId, // Người quét.
                        Note = "Serial sẵn bán nhưng nằm ngoài phạm vi kiểm kê." // Lưu chú thích lý do thừa.
                    };
                    await _checkRepo.AddDetailSerialAsync(surplusRow); // Lưu dòng serial thừa vào DB Context.
                    await _checkRepo.SaveChangesAsync(); // Lưu thay đổi xuống DB.

                    return ApiResult<ScanResultDto>.Ok(new ScanResultDto // Phản hồi kết quả quét thừa thành công.
                    {
                        Success = true, // Thành công.
                        SerialNumberRaw = serialNumber, // Mã serial.
                        ScanStatus = (byte)InventoryScanStatus.Surplus, // Trạng thái Thừa.
                        ScanStatusName = "Thừa", // Tên trạng thái.
                        Message = "Serial nằm ngoài phạm vi kiểm kê.", // Thông báo thừa.
                        FoundSerialNumber = dbSerial.SerialNumber, // Số serial trong DB.
                        MiniDashboard = await BuildMiniDashboardAsync(checkId, check) // Dashboard mini.
                    });
                }
            }

            var surplusNote = dbSerial.Status switch // Trường hợp serial tồn tại nhưng có trạng thái khác Available (Đã bán, Lỗi, Trả lại...).
            {
                (byte)SerialStatus.Sold => $"Đã bán tại Đơn hàng #{dbSerial.OrderId} ngày {dbSerial.SoldDate?.ToString("dd/MM/yyyy") ?? "N/A"}.", // Chú thích nếu đã bán.
                (byte)SerialStatus.Reserved => $"Đang giữ chỗ cho Đơn hàng #{dbSerial.OrderId}.", // Chú thích nếu đang giữ chỗ.
                (byte)SerialStatus.Defective => "Đang nằm trong kho lỗi.", // Chú thích nếu đã ghi nhận lỗi trước đó.
                (byte)SerialStatus.Returned => "Đã được ghi nhận trả lại.", // Chú thích nếu là hàng hoàn trả.
                (byte)SerialStatus.Lost => "Đã được ghi nhận thất thoát.", // Chú thích nếu đã báo mất.
                _ => $"Trạng thái hiện tại: {dbSerial.Status}." // Mặc định hiển thị mã trạng thái gốc khác.
            };

            var existingDetail = await _checkRepo.GetDetailByCheckAndVariantAsync(checkId, dbSerial.VariantId, withTracking: true); // Lấy dòng chi tiết tổng hợp tương ứng để cập nhật.
            var surplusDetailId = existingDetail?.Id; // Lưu Id chi tiết (nếu có).

            var surplusEntry = new InventoryCheckDetailSerial // Khởi tạo dòng serial thừa kèm lý do lỗi trạng thái.
            {
                CheckId = checkId, // Mã phiếu.
                DetailId = surplusDetailId, // Id chi tiết tổng hợp.
                VariantId = dbSerial.VariantId, // Mã biến thể.
                SerialId = dbSerial.Id, // Mã thực thể serial.
                SerialNumberRaw = serialNumber, // Mã số serial quét.
                OriginalStatus = dbSerial.Status, // Trạng thái gốc khác Available.
                ScanStatus = (byte)InventoryScanStatus.Surplus, // Ghi nhận quét Thừa.
                ScannedAt = now, // Thời điểm quét.
                ScannedByEmployeeId = employeeId, // Người quét.
                Note = surplusNote // Lưu lý do chênh lệch trạng thái để làm cơ sở đối soát.
            };

            if (existingDetail != null) // Nếu dòng chi tiết tổng hợp tồn tại.
                existingDetail.SurplusQuantity++; // Tăng chỉ số lượng thừa của biến thể lên 1.

            await _checkRepo.AddDetailSerialAsync(surplusEntry); // Lưu dòng serial thừa trạng thái vào DB Context.
            await _checkRepo.SaveChangesAsync(); // Lưu thay đổi xuống cơ sở dữ liệu.

            var originalStatusName = ((SerialStatus)dbSerial.Status).ToString(); // Đọc tên trạng thái gốc của serial từ Enum.
            return ApiResult<ScanResultDto>.Ok(new ScanResultDto // Phản hồi kết quả quét thừa kèm lý do trạng thái lệch.
            {
                Success = true, // Thành công.
                SerialNumberRaw = serialNumber, // Số serial quét.
                ScanStatus = (byte)InventoryScanStatus.Surplus, // Trạng thái Thừa.
                ScanStatusName = "Thừa", // Tên trạng thái.
                Message = $"Serial tìm thấy nhưng đang ở trạng thái {originalStatusName}. {surplusNote}", // Thông báo chi tiết lý do.
                FoundSerialNumber = dbSerial.SerialNumber, // Số serial trong hệ thống.
                FoundVariantName = dbSerial.Variant?.VariantName, // Tên biến thể sản phẩm.
                FoundOriginalStatus = dbSerial.Status, // Mã trạng thái gốc.
                FoundOriginalStatusName = originalStatusName, // Tên trạng thái gốc.
                SurplusNote = surplusNote, // Lý do chênh lệch trạng thái.
                MiniDashboard = await BuildMiniDashboardAsync(checkId, check) // Dashboard mini.
            });
        }

        public async Task<ApiResult<bool>> MarkDefectiveAsync(int checkId, int detailSerialId, Guid employeeId) // Đánh dấu serial sản phẩm bị lỗi vật lý khi quét thực tế.
        {
            var check = await _checkRepo.GetByIdAsync(checkId); // Lấy thông tin phiếu kiểm kê từ repository.
            if (check == null) // Nếu không tìm thấy phiếu kiểm kê.
                return ApiResult<bool>.Fail("Không tìm thấy phiếu kiểm kê yêu cầu.", ApiErrorCode.NotFound); // Trả về lỗi NotFound.

            if (check.Status != (byte)InventoryCheckStatus.Draft) // Chỉ cho phép cập nhật thông tin khi phiếu đang ở trạng thái Nháp (Draft).
                return ApiResult<bool>.Fail("Chỉ có thể đánh dấu lỗi khi phiếu ở trạng thái Nháp."); // Trả về lỗi nếu phiếu đã gửi duyệt hoặc hoàn tất.

            var row = await _checkRepo.GetDetailSerialAsync(detailSerialId, withTracking: true); // Lấy dòng serial cần cập nhật kèm theo dõi trạng thái (Tracking).
            if (row == null || row.CheckId != checkId) // Nếu không tìm thấy dòng serial hoặc dòng đó không thuộc về phiếu kiểm kê này.
                return ApiResult<bool>.Fail("Không tìm thấy dòng Serial trong phiếu kiểm kê.", ApiErrorCode.NotFound); // Trả về lỗi không tìm thấy.

            if (row.ScanStatus != (byte)InventoryScanStatus.Matched) // Nghiệp vụ: Chỉ cho phép báo lỗi đối với các serial thực tế quét khớp (đang có sẵn bán).
                return ApiResult<bool>.Fail("Chỉ có thể đánh dấu lỗi cho Serial đang ở trạng thái Khớp."); // Báo lỗi nếu serial ở trạng thái khác.

            row.ScanStatus = (byte)InventoryScanStatus.Defective; // Cập nhật trạng thái quét thành Lỗi vật lý (Defective).

            if (row.DetailId.HasValue && row.VariantId.HasValue) // Nếu dòng serial có liên kết với chi tiết tổng hợp và biến thể.
            {
                var detail = await _checkRepo.GetDetailByCheckAndVariantAsync(checkId, row.VariantId.Value, withTracking: true); // Lấy dòng chi tiết tổng hợp của biến thể.
                if (detail != null) // Nếu dòng chi tiết tồn tại.
                {
                    detail.MatchedQuantity--; // Giảm số lượng khớp đi 1 (vì đã chuyển sang lỗi).
                    detail.DefectiveQuantity++; // Tăng số lượng hàng lỗi vật lý quét được lên 1.
                }
            }

            await _checkRepo.SaveChangesAsync(); // Lưu các thay đổi số lượng và trạng thái của serial xuống DB.
            return ApiResult<bool>.Ok(true, "Đã đánh dấu Serial là hàng lỗi vật lý."); // Trả về kết quả thành công.
        }

        public async Task<ApiResult<bool>> UpdateReasonAsync(int checkId, int detailSerialId, UpdateScanReasonRequest request, Guid employeeId) // Cập nhật nguyên nhân chênh lệch và hướng xử lý đề xuất.
        {
            var check = await _checkRepo.GetByIdAsync(checkId); // Lấy thông tin phiếu kiểm kê.
            if (check == null) // Nếu không tìm thấy phiếu kiểm kê.
                return ApiResult<bool>.Fail("Không tìm thấy phiếu kiểm kê yêu cầu.", ApiErrorCode.NotFound); // Trả về lỗi NotFound.

            if (check.Status != (byte)InventoryCheckStatus.Draft) // Chỉ cho phép cập nhật khi phiếu kiểm kê ở trạng thái Nháp.
                return ApiResult<bool>.Fail("Chỉ có thể cập nhật lý do khi phiếu ở trạng thái Nháp."); // Trả về thông báo lỗi nghiệp vụ.

            var row = await _checkRepo.GetDetailSerialAsync(detailSerialId, withTracking: true); // Lấy dòng serial cần cập nhật lý do kèm tracking.
            if (row == null || row.CheckId != checkId) // Nếu không tìm thấy dòng serial trong phiếu.
                return ApiResult<bool>.Fail("Không tìm thấy dòng Serial trong phiếu kiểm kê.", ApiErrorCode.NotFound); // Trả về lỗi không tìm thấy.

            row.Note = request.Reason.Trim(); // Lưu nguyên nhân chênh lệch đã cắt khoảng trắng thừa.
            row.ProposedActionNote = request.ProposedActionNote?.Trim(); // Lưu hướng xử lý đề xuất.

            await _checkRepo.SaveChangesAsync(); // Lưu thay đổi xuống cơ sở dữ liệu.
            return ApiResult<bool>.Ok(true, "Cập nhật lý do thành công."); // Trả về kết quả thành công.
        }

        public async Task<ApiResult<bool>> SubmitAsync(int checkId, Guid employeeId) // Nghiệp vụ gửi duyệt phiếu kiểm kê (chuyển trạng thái sang Chờ phê duyệt).
        {
            var check = await _checkRepo.GetByIdAsync(checkId); // Lấy thông tin phiếu kiểm kê từ repository.
            if (check == null) // Nếu không tìm thấy phiếu.
                return ApiResult<bool>.Fail("Không tìm thấy phiếu kiểm kê yêu cầu.", ApiErrorCode.NotFound); // Trả về lỗi NotFound.

            if (check.Status != (byte)InventoryCheckStatus.Draft) // Chỉ cho phép gửi duyệt khi phiếu đang ở trạng thái Nháp.
                return ApiResult<bool>.Fail("Chỉ có thể gửi duyệt khi phiếu ở trạng thái Nháp."); // Trả về thông báo lỗi.

            if (check.EmployeeId != employeeId) // Ràng buộc: Chỉ nhân viên tạo phiếu mới được quyền gửi duyệt phiếu kiểm kê đó.
                return ApiResult<bool>.Fail("Bạn không có quyền gửi duyệt phiếu này.", ApiErrorCode.Forbidden); // Trả về lỗi cấm truy cập Forbidden.

            await _unitOfWork.BeginTransactionAsync(); // Bắt đầu giao dịch để đảm bảo việc chốt số lượng thiếu tự động diễn ra đồng bộ, an toàn.
            try // Khối bẫy lỗi.
            {
                var pendingRows = await _checkRepo.GetPendingDetailSerialsAsync(checkId); // Lấy tất cả các dòng serial chốt ban đầu mà chưa được quét thực tế (trạng thái Pending).
                var detailMissingCounts = new Dictionary<int, int>(); // Khởi tạo Dictionary đếm số lượng thiếu theo từng chi tiết tổng hợp.

                foreach (var row in pendingRows) // Duyệt qua từng dòng serial chờ quét nhưng không thấy ở kệ kho.
                {
                    row.ScanStatus = (byte)InventoryScanStatus.Missing; // Tự động cập nhật trạng thái quét thành Thiếu (Missing - Thất thoát).
                    if (row.DetailId.HasValue) // Nếu dòng serial có liên kết với chi tiết tổng hợp.
                    {
                        detailMissingCounts.TryAdd(row.DetailId.Value, 0); // Khởi tạo giá trị trong Dictionary nếu chưa tồn tại khóa.
                        detailMissingCounts[row.DetailId.Value]++; // Tăng số lượng thiếu của chi tiết tổng hợp lên 1.
                    }
                }

                foreach (var (detailId, missingCount) in detailMissingCounts) // Duyệt qua danh sách chi tiết tổng hợp và số lượng thiếu đếm được.
                {
                    var detail = await _context.InventoryCheckDetails // Lấy dòng chi tiết tổng hợp tương ứng từ cơ sở dữ liệu.
                        .FirstOrDefaultAsync(d => d.Id == detailId); // Tìm dòng đầu tiên khớp Id.
                    if (detail != null) // Nếu dòng chi tiết tồn tại.
                        detail.MissingQuantity += missingCount; // Cộng dồn số lượng thiếu vào cột MissingQuantity trên dòng tổng hợp.
                }

                check.Status = (byte)InventoryCheckStatus.AwaitingApproval; // Cập nhật trạng thái phiếu kiểm kê sang Chờ phê duyệt (AwaitingApproval).
                await _unitOfWork.SaveChangesAsync(); // Lưu thay đổi trạng thái và số lượng xuống database.
                await _unitOfWork.CommitAsync(); // Xác nhận hoàn tất giao dịch.

                _logger.LogInformation( // Ghi log thông tin gửi duyệt phiếu thành công.
                    "Gửi duyệt phiếu kiểm kê {CheckCode}: {MissingCount} serials thiếu", // Chuỗi định dạng log.
                    check.CheckCode, pendingRows.Count); // Mã phiếu và tổng số serial bị thiếu.

                return ApiResult<bool>.Ok(true, "Đã gửi phiếu kiểm kê để phê duyệt thành công."); // Trả về kết quả thành công.
            }
            catch (Exception ex) // Bắt lỗi xảy ra trong quá trình xử lý giao dịch.
            {
                await _unitOfWork.RollbackAsync(); // Rollback giao dịch để phục hồi dữ liệu ban đầu.
                _logger.LogError(ex, "Lỗi khi gửi duyệt phiếu kiểm kê {CheckId}.", checkId); // Ghi log chi tiết lỗi.
                return ApiResult<bool>.Fail("Đã xảy ra lỗi khi gửi duyệt. Vui lòng thử lại."); // Trả về thông báo lỗi hệ thống.
            }
        }

        public async Task<ApiResult<bool>> ApproveAsync(int checkId, Guid adminId) // Nghiệp vụ phê duyệt phiếu kiểm kê và tự động cân bằng tồn kho hệ thống (chỉ dành cho Admin).
        {
            var check = await _checkRepo.GetByIdAsync(checkId); // Lấy thông tin phiếu kiểm kê từ repository.
            if (check == null) // Nếu không tìm thấy phiếu kiểm kê.
                return ApiResult<bool>.Fail("Không tìm thấy phiếu kiểm kê yêu cầu.", ApiErrorCode.NotFound); // Trả về lỗi NotFound.

            if (check.Status != (byte)InventoryCheckStatus.AwaitingApproval) // Chỉ phê duyệt khi phiếu đang ở trạng thái Chờ duyệt (AwaitingApproval).
                return ApiResult<bool>.Fail("Chỉ có thể phê duyệt phiếu ở trạng thái Chờ duyệt."); // Trả về lỗi nghiệp vụ.

            await _unitOfWork.BeginTransactionAsync(); // Bắt đầu giao dịch để thực hiện cân bằng kho đồng bộ, tránh tranh chấp dữ liệu.
            try // Khối bẫy lỗi.
            {
                var adjustmentLogs = new List<InventoryAdjustmentLog>(); // Khởi tạo danh sách lưu log điều chỉnh kho phục vụ kiểm toán.
                var affectedVariantIds = new HashSet<int>(); // Tập hợp chứa các Id biến thể sản phẩm bị ảnh hưởng để đồng bộ số lượng tồn kho.

                var missingRows = await _checkRepo.GetMissingDetailSerialsWithSerialAsync(checkId); // Lấy toàn bộ các dòng serial bị thiếu trong phiếu kiểm kê kèm thực thể serial tương ứng.
                foreach (var row in missingRows) // Duyệt qua từng dòng serial bị thiếu.
                {
                    if (row.Serial == null) continue; // Nếu không có thông tin serial đi kèm, bỏ qua.

                    var currentStatus = row.Serial.Status; // Đọc trạng thái hiện tại của serial trong cơ sở dữ liệu.

                    if (currentStatus == (byte)SerialStatus.Available) // Nếu tại thời điểm duyệt, serial đó vẫn đang có trạng thái khả dụng trên sổ sách.
                    {
                        var costImpact = await GetSerialCostAsync(row.Serial); // Lấy giá vốn nhập kho gần nhất của serial này để tính giá trị tổn thất tài chính.

                        row.Serial.Status = (byte)SerialStatus.Lost; // Chuyển trạng thái phần mềm của serial sang Mất (Lost - Thất thoát).
                        affectedVariantIds.Add(row.Serial.VariantId); // Thêm Id biến thể sản phẩm vào tập hợp cần đồng bộ tồn kho.

                        adjustmentLogs.Add(new InventoryAdjustmentLog // Khởi tạo dòng log điều chỉnh kho chi tiết.
                        {
                            AuditCheckId = checkId, // Mã phiếu kiểm kê.
                            SerialId = row.Serial.Id, // Mã số serial.
                            VariantId = row.Serial.VariantId, // Mã biến thể.
                            OldStatus = currentStatus, // Trạng thái cũ.
                            NewStatus = (byte)SerialStatus.Lost, // Trạng thái mới.
                            AdjustmentType = (byte)InventoryAdjustmentType.Lost, // Kiểu điều chỉnh là báo mất.
                            CostImpact = costImpact, // Giá vốn chi phí hao tổn.
                            Reason = row.Note ?? "Không tìm thấy khi kiểm kê.", // Nguyên nhân.
                            AdjustedDate = DateTime.UtcNow, // Thời gian thực hiện điều chỉnh.
                            AdjustedByEmployeeId = adminId // Người thực hiện phê duyệt điều chỉnh.
                        });
                    }
                    else // Trường hợp "Cửa sổ kiểm kê": Serial bị thiếu lúc quét nhưng đã được bán hoặc giữ chỗ bởi đơn hàng mới phát sinh khi chờ duyệt.
                    {
                        row.ResolvedDuringApproval = true; // Đánh dấu đã tự động giải quyết bỏ qua trong quá trình duyệt.
                        var resolveNote = currentStatus switch // Sinh chuỗi lý do tự động giải quyết theo trạng thái hiện tại của serial.
                        {
                            (byte)SerialStatus.Sold => "Đã bán trong cửa sổ kiểm kê — không ghi lỗ.", // Nếu đã bán.
                            (byte)SerialStatus.Reserved => "Đã giữ chỗ trong cửa sổ kiểm kê — không ghi lỗ.", // Nếu đang giữ chỗ đơn mới.
                            _ => $"Trạng thái thay đổi trong cửa sổ kiểm kê ({currentStatus}) — không ghi lỗ." // Trường hợp khác.
                        };
                        row.Note = string.IsNullOrEmpty(row.Note) // Cập nhật ghi chú dòng serial kiểm kê.
                            ? resolveNote // Nếu ghi chú trống, gán lý do.
                            : $"{row.Note} | {resolveNote}"; // Nếu đã có ghi chú, nối thêm lý do cửa sổ kiểm kê.
                    }
                }

                var defectiveRows = await _checkRepo.GetDefectiveDetailSerialsWithSerialAsync(checkId); // Lấy tất cả các dòng serial được báo lỗi vật lý trong phiếu.
                foreach (var row in defectiveRows) // Duyệt qua từng dòng serial lỗi.
                {
                    if (row.Serial == null) continue; // Bỏ qua nếu thiếu thông tin thực thể serial.

                    var currentStatus = row.Serial.Status; // Lấy trạng thái hiện tại của serial.

                    if (currentStatus == (byte)SerialStatus.Available) // Chỉ chuyển sang trạng thái lỗi hỏng nếu serial vẫn đang khả dụng trên sổ sách.
                    {
                        var costImpact = await GetSerialCostAsync(row.Serial); // Lấy giá vốn nhập kho của serial lỗi.

                        row.Serial.Status = (byte)SerialStatus.Defective; // Chuyển trạng thái phần mềm sang Lỗi hỏng (Defective) để loại bỏ khỏi danh mục bán hàng.
                        affectedVariantIds.Add(row.Serial.VariantId); // Đăng ký Id biến thể cần đồng bộ số lượng tồn kho khả dụng.

                        adjustmentLogs.Add(new InventoryAdjustmentLog // Ghi nhận log điều chỉnh kho cho hàng lỗi hỏng vật lý.
                        {
                            AuditCheckId = checkId, // Mã phiếu.
                            SerialId = row.Serial.Id, // Mã serial.
                            VariantId = row.Serial.VariantId, // Mã biến thể.
                            OldStatus = currentStatus, // Trạng thái cũ.
                            NewStatus = (byte)SerialStatus.Defective, // Trạng thái mới.
                            AdjustmentType = (byte)InventoryAdjustmentType.Defective, // Loại điều chỉnh là lỗi hỏng.
                            CostImpact = costImpact, // Giá vốn hao tổn.
                            Reason = row.Note ?? "Hàng lỗi vật lý phát hiện khi kiểm kê.", // Lý do ghi nhận.
                            AdjustedDate = DateTime.UtcNow, // Thời gian điều chỉnh.
                            AdjustedByEmployeeId = adminId // Người phê duyệt.
                        });
                    }
                }

                if (adjustmentLogs.Any()) // Nếu có danh sách log điều chỉnh kho phát sinh.
                    await _checkRepo.AddAdjustmentLogsAsync(adjustmentLogs); // Thêm hàng loạt các dòng log điều chỉnh vào DB Context.

                check.Status = (byte)InventoryCheckStatus.Completed; // Chuyển trạng thái phiếu kiểm kê sang Đã hoàn tất (Completed).
                check.ApprovedByEmployeeId = adminId; // Ghi nhận mã quản trị viên phê duyệt.
                check.ApprovedAt = DateTime.UtcNow; // Ghi nhận ngày giờ phê duyệt là giờ UTC hiện tại.

                await _unitOfWork.SaveChangesAsync(); // Lưu toàn bộ thay đổi trạng thái phiếu, serial và log điều chỉnh xuống DB.

                if (affectedVariantIds.Any()) // Nếu có các biến thể bị thay đổi số lượng tồn kho khả dụng.
                    await _inventorySyncService.SyncStockBatchAsync(affectedVariantIds); // Đồng bộ lại số lượng tồn kho thực tế của các biến thể để cập nhật lên website.

                await _unitOfWork.CommitAsync(); // Xác nhận hoàn tất và áp dụng toàn bộ các thay đổi trong giao dịch.

                _logger.LogInformation( // Ghi log thông báo phê duyệt thành công.
                    "Phê duyệt phiếu kiểm kê {CheckCode}: {Lost} lost, {Defective} defective. Admin: {AdminId}", // Định dạng log.
                    check.CheckCode, // Mã phiếu.
                    adjustmentLogs.Count(l => l.AdjustmentType == (byte)InventoryAdjustmentType.Lost), // Số lượng serial thất thoát.
                    adjustmentLogs.Count(l => l.AdjustmentType == (byte)InventoryAdjustmentType.Defective), // Số lượng serial lỗi hỏng.
                    adminId); // Mã admin phê duyệt.

                return ApiResult<bool>.Ok(true, "Phê duyệt và cân bằng kho thành công."); // Trả về kết quả thành công.
            }
            catch (Exception ex) // Bắt lỗi ngoại lệ.
            {
                await _unitOfWork.RollbackAsync(); // Hủy bỏ toàn bộ giao dịch để khôi phục trạng thái an toàn cho DB.
                _logger.LogError(ex, "Lỗi khi phê duyệt phiếu kiểm kê {CheckId}.", checkId); // Ghi log chi tiết lỗi hệ thống.
                return ApiResult<bool>.Fail("Đã xảy ra lỗi khi phê duyệt. Vui lòng thử lại."); // Trả về thông báo lỗi.
            }
        }

        public async Task<ApiResult<bool>> RejectAsync(int checkId, RejectInventoryCheckRequest request, Guid adminId) // Từ chối phê duyệt phiếu kiểm kê và xử lý theo các phương án (trả về nháp hoặc hủy bỏ).
        {
            var check = await _checkRepo.GetByIdAsync(checkId); // Lấy thông tin phiếu kiểm kê từ repository theo Id.
            if (check == null) // Nếu không tìm thấy phiếu.
                return ApiResult<bool>.Fail("Không tìm thấy phiếu kiểm kê yêu cầu.", ApiErrorCode.NotFound); // Trả về lỗi NotFound.

            if (check.Status != (byte)InventoryCheckStatus.AwaitingApproval) // Yêu cầu: Phiếu kiểm kê phải đang ở trạng thái Chờ duyệt mới có thể từ chối.
                return ApiResult<bool>.Fail("Chỉ có thể từ chối phiếu ở trạng thái Chờ duyệt."); // Trả về lỗi nghiệp vụ.

            await _unitOfWork.BeginTransactionAsync(); // Bắt đầu một giao dịch để đảm bảo quá trình khôi phục/hủy phiếu diễn ra nhất quán.
            try // Khối bẫy lỗi.
            {
                check.RejectReason = request.Reason.Trim(); // Lưu lý do từ chối phê duyệt từ quản trị viên.

                if (request.ReturnToDraft) // Phương án 1: Trả về trạng thái Nháp (Draft) để nhân viên thực hiện kiểm kho/quét lại.
                {
                    var actualMissingRows = await _context.InventoryCheckDetailSerials // Lấy tất cả các dòng serial kiểm kê đang ghi nhận trạng thái Thiếu (Missing).
                        .Where(s => s.CheckId == checkId && s.ScanStatus == (byte)InventoryScanStatus.Missing) // Lọc theo CheckId và trạng thái Missing.
                        .Include(s => s.Detail) // Tải kèm thông tin dòng chi tiết tổng hợp.
                        .ToListAsync(); // Chuyển đổi thành danh sách bất đồng bộ.

                    foreach (var row in actualMissingRows) // Duyệt qua danh sách serial bị thiếu để khôi phục.
                    {
                        row.ScanStatus = (byte)InventoryScanStatus.Pending; // Khôi phục trạng thái quét trở lại là Chờ quét (Pending).
                        if (row.Detail != null) // Nếu có dòng chi tiết tổng hợp liên kết.
                            row.Detail.MissingQuantity = 0; // Đặt lại số lượng thiếu của dòng tổng hợp về 0.
                    }

                    var surplusRows = await _checkRepo.GetSurplusDetailSerialsAsync(checkId); // Lấy toàn bộ các dòng serial quét thừa (thừa / thừa mã lạ) phát sinh trong lần quét trước.
                    await _checkRepo.RemoveDetailSerialsAsync(surplusRows); // Xóa sạch các dòng quét thừa này khỏi cơ sở dữ liệu.

                    var details = await _context.InventoryCheckDetails // Lấy danh sách các dòng chi tiết tổng hợp của phiếu kiểm kê.
                        .Where(d => d.CheckId == checkId) // Lọc theo mã phiếu kiểm kê.
                        .ToListAsync(); // Chuyển thành danh sách.
                    foreach (var d in details) // Duyệt qua từng dòng chi tiết để reset lại toàn bộ số lượng thực tế kiểm đếm.
                    {
                        d.ActualQuantity = 0; // Đặt lại số lượng thực tế đếm được về 0.
                        d.MatchedQuantity = 0; // Đặt lại số lượng khớp về 0.
                        d.MissingQuantity = 0; // Đặt lại số lượng thiếu về 0.
                        d.SurplusQuantity = 0; // Đặt lại số lượng thừa về 0.
                        d.DefectiveQuantity = 0; // Đặt lại số lượng lỗi vật lý về 0.
                    }

                    var matchedRows = await _context.InventoryCheckDetailSerials // Lấy danh sách các dòng serial thực tế đã quét Khớp (Matched).
                        .Where(s => s.CheckId == checkId && s.ScanStatus == (byte)InventoryScanStatus.Matched) // Lọc theo phiếu và trạng thái Khớp.
                        .Include(s => s.Detail) // Tải kèm chi tiết tổng hợp.
                        .ToListAsync(); // Chuyển thành danh sách.
                    foreach (var row in matchedRows) // Điền lại số lượng đếm thực tế của các serial quét khớp vẫn giữ nguyên kết quả.
                    {
                        if (row.Detail != null) // Nếu dòng chi tiết tổng hợp tồn tại.
                        {
                            row.Detail.ActualQuantity++; // Tăng lại số lượng thực tế đếm được thêm 1.
                            row.Detail.MatchedQuantity++; // Tăng lại số lượng khớp thêm 1.
                        }
                    }

                    var defectiveRows = await _context.InventoryCheckDetailSerials // Lấy danh sách các dòng serial đã quét báo Lỗi vật lý (Defective).
                        .Where(s => s.CheckId == checkId && s.ScanStatus == (byte)InventoryScanStatus.Defective) // Lọc theo trạng thái Lỗi.
                        .Include(s => s.Detail) // Tải kèm chi tiết tổng hợp.
                        .ToListAsync(); // Chuyển thành danh sách.
                    foreach (var row in defectiveRows) // Điền lại số lượng đếm thực tế cho hàng lỗi vật lý.
                    {
                        if (row.Detail != null) // Nếu dòng chi tiết tổng hợp tồn tại.
                        {
                            row.Detail.ActualQuantity++; // Tăng số lượng thực tế đếm thêm 1.
                            row.Detail.DefectiveQuantity++; // Tăng số lượng lỗi hỏng thêm 1.
                        }
                    }

                    check.Status = (byte)InventoryCheckStatus.Draft; // Chuyển trạng thái phiếu kiểm kê trở lại trạng thái Nháp (Draft).
                }
                else // Phương án 2: Hủy bỏ hoàn toàn (Cancelled) phiếu kiểm kê.
                {
                    check.Status = (byte)InventoryCheckStatus.Cancelled; // Đóng vĩnh viễn phiếu và giữ nguyên hiện trạng số liệu để lưu vết lịch sử lỗi.
                }

                await _unitOfWork.SaveChangesAsync(); // Lưu các thay đổi khôi phục hoặc hủy phiếu xuống database.
                await _unitOfWork.CommitAsync(); // Xác nhận hoàn tất giao dịch.

                var action = request.ReturnToDraft ? "trả về Nháp" : "hủy"; // Xác định chuỗi hành động để ghi log.
                _logger.LogInformation( // Ghi log thông báo từ chối duyệt phiếu kiểm kê thành công.
                    "Từ chối phiếu kiểm kê {CheckCode} ({Action}). Admin: {AdminId}. Lý do: {Reason}", // Định dạng log.
                    check.CheckCode, action, adminId, request.Reason); // Tham số truyền vào log.

                return ApiResult<bool>.Ok(true, $"Đã từ chối và {action} phiếu kiểm kê."); // Trả về kết quả thành công kèm thông báo.
            }
            catch (Exception ex) // Bắt lỗi ngoại lệ.
            {
                await _unitOfWork.RollbackAsync(); // Rollback giao dịch để bảo toàn dữ liệu.
                _logger.LogError(ex, "Lỗi khi từ chối phiếu kiểm kê {CheckId}.", checkId); // Ghi log chi tiết lỗi.
                return ApiResult<bool>.Fail("Đã xảy ra lỗi khi từ chối. Vui lòng thử lại."); // Trả về thông báo lỗi.
            }
        }

        public async Task<ApiResult<bool>> CancelAsync(int checkId, Guid employeeId, bool isAdmin) // Hủy bỏ phiếu kiểm kê đang ở trạng thái Nháp (Draft).
        {
            var check = await _checkRepo.GetByIdAsync(checkId); // Lấy thông tin phiếu kiểm kê từ repository.
            if (check == null) // Nếu phiếu không tồn tại.
                return ApiResult<bool>.Fail("Không tìm thấy phiếu kiểm kê yêu cầu.", ApiErrorCode.NotFound); // Trả về lỗi NotFound.

            if (check.Status != (byte)InventoryCheckStatus.Draft) // Ràng buộc: Chỉ cho phép hủy phiếu kiểm kê khi phiếu đang ở trạng thái Nháp.
                return ApiResult<bool>.Fail("Chỉ có thể hủy phiếu ở trạng thái Nháp."); // Trả về lỗi nếu trạng thái phiếu không hợp lệ.

            if (!isAdmin && check.EmployeeId != employeeId) // Ràng buộc quyền hạn: Chỉ người tạo phiếu (hoặc Admin) mới có quyền hủy phiếu.
                return ApiResult<bool>.Fail("Bạn không có quyền hủy phiếu này.", ApiErrorCode.Forbidden); // Trả về lỗi cấm truy cập Forbidden.

            check.Status = (byte)InventoryCheckStatus.Cancelled; // Cập nhật trạng thái phiếu kiểm kê sang Đã hủy (Cancelled).
            await _checkRepo.SaveChangesAsync(); // Lưu thay đổi trạng thái xuống DB.

            return ApiResult<bool>.Ok(true, "Đã hủy phiếu kiểm kê."); // Trả về kết quả thành công.
        }

        private async Task<string> GenerateCheckCodeAsync() // Hàm phụ trợ sinh mã phiếu kiểm kê tự động có dạng KK-yyyyMMdd-NNN.
        {
            var dateStr = DateTime.UtcNow.ToString("yyyyMMdd"); // Lấy chuỗi ngày tháng năm hiện tại dạng yyyyMMdd.
            var prefix = $"KK-{dateStr}-"; // Khởi tạo tiền tố mã phiếu kiểm kê.
            var lastCode = await _checkRepo.GetLastCheckCodeByDateAsync(prefix); // Gọi repo lấy mã phiếu kiểm kê cuối cùng được lập trong ngày có tiền tố này.

            int nextNumber = 1; // Số thứ tự tiếp theo mặc định ban đầu là 1.
            if (!string.IsNullOrEmpty(lastCode)) // Nếu tìm thấy mã phiếu kiểm kê trước đó trong ngày.
            {
                var lastPart = lastCode.Substring(prefix.Length); // Cắt lấy phần số thứ tự phía sau tiền tố.
                if (int.TryParse(lastPart, out int lastNumber)) // Chuyển đổi phần số sang kiểu số nguyên.
                    nextNumber = lastNumber + 1; // Tăng số thứ tự thêm 1 cho phiếu mới.
            }
            return $"{prefix}{nextNumber:D3}"; // Định dạng chuỗi mã phiếu với số thứ tự có 3 chữ số (ví dụ: KK-20260531-001).
        }

        private async Task<List<int>> GetVariantIdsInScopeAsync(byte scopeType, int? scopeCategoryId) // Hàm phụ trợ lấy danh sách mã biến thể sản phẩm nằm trong phạm vi kiểm kê.
        {
            if (scopeType == (byte)InventoryCheckScopeType.AllStore) // Nếu phạm vi kiểm kê là toàn bộ cửa hàng.
            {
                return await _context.ProductVariants // Lấy toàn bộ danh sách biến thể chưa bị xóa khỏi cơ sở dữ liệu.
                    .AsNoTracking() // Tối ưu hóa truy vấn không theo dõi trạng thái thực thể (Read-only).
                    .Where(v => !v.IsDeleted) // Lọc các biến thể chưa bị xóa mềm.
                    .Select(v => v.Id) // Chỉ lấy ra Id biến thể.
                    .ToListAsync(); // Chuyển đổi thành danh sách bất đồng bộ.
            }
            else // Nếu phạm vi kiểm kê giới hạn theo một Danh mục cụ thể (ScopeType = Category).
            {
                var categoryIds = await _productRepo.GetCategoryChildIdsAsync(scopeCategoryId!.Value); // Sử dụng thuật toán lấy toàn bộ danh sách Id các danh mục con cháu phẳng (flat tree).
                categoryIds.Add(scopeCategoryId.Value); // Thêm chính danh mục cha vào tập hợp lọc.

                return await _context.ProductVariants // Lấy danh sách biến thể thuộc các danh mục nằm trong tập hợp trên.
                    .AsNoTracking() // Truy vấn tối ưu AsNoTracking.
                    .Where(v => !v.IsDeleted && categoryIds.Contains(v.Product.CategoryId)) // Lọc theo trạng thái chưa xóa và thuộc danh mục kiểm duyệt.
                    .Select(v => v.Id) // Lấy Id biến thể.
                    .ToListAsync(); // Chuyển sang danh sách List.
            }
        }

        private async Task<decimal> GetSerialCostAsync(ProductSerial serial) // Hàm phụ trợ lấy giá vốn nhập kho của một Serial sản phẩm.
        {
            var cost = await _context.ImportReceiptDetails // Tìm kiếm dòng chi tiết của hóa đơn nhập kho chứa serial này.
                .AsNoTracking() // Truy vấn không tracking để tối ưu hiệu năng.
                .Where(d => d.ReceiptId == serial.ImportReceiptId && d.VariantId == serial.VariantId) // Khớp mã hóa đơn nhập và mã biến thể sản phẩm.
                .Select(d => (decimal?)d.ImportPrice) // Trích xuất giá nhập kiểu decimal nullable.
                .FirstOrDefaultAsync(); // Lấy giá trị đầu tiên được tìm thấy.
            return cost ?? 0m; // Trả về giá nhập kho tìm thấy, mặc định là 0 nếu không tìm thấy dữ liệu.
        }

        private async Task<InventoryCheckDto?> BuildCheckDtoAsync(int checkId) // Hàm phụ trợ xây dựng DTO chi tiết phiếu kiểm kê đầy đủ thông tin hiển thị.
        {
            var check = await _checkRepo.GetByIdWithDetailsAsync(checkId); // Lấy thông tin phiếu kiểm kê kèm chi tiết từ repository.
            if (check == null) return null; // Trả về null nếu không tìm thấy phiếu.

            var employeeNames = await GetEmployeeNamesAsync(new List<Guid> { check.EmployeeId }); // Lấy tên nhân viên lập phiếu.
            var approverNames = check.ApprovedByEmployeeId.HasValue // Lấy tên quản trị viên phê duyệt phiếu (nếu có).
                ? await GetEmployeeNamesAsync(new List<Guid> { check.ApprovedByEmployeeId.Value }) // Truy vấn tên theo Id.
                : new Dictionary<Guid, string>(); // Trả về Dictionary rỗng nếu chưa phê duyệt.

            return new InventoryCheckDto // Khởi tạo DTO chi tiết phiếu kiểm kê và gán giá trị.
            {
                Id = check.Id, // Mã Id phiếu.
                CheckCode = check.CheckCode, // Mã phiếu.
                EmployeeId = check.EmployeeId, // Mã nhân viên tạo.
                EmployeeName = employeeNames.GetValueOrDefault(check.EmployeeId, check.EmployeeId.ToString()), // Tên nhân viên tạo.
                CheckDate = check.CheckDate, // Ngày kiểm kê.
                SnapshotAt = check.SnapshotAt, // Ngày giờ chốt số liệu sổ sách.
                Status = check.Status, // Mã trạng thái.
                StatusName = GetStatusName(check.Status), // Tên trạng thái.
                ScopeType = check.ScopeType, // Mã phạm vi.
                ScopeTypeName = check.ScopeType == 0 ? "Toàn bộ kho" : "Theo danh mục", // Tên phạm vi.
                ScopeCategoryId = check.ScopeCategoryId, // Mã danh mục.
                ScopeCategoryName = check.ScopeCategory?.Name, // Tên danh mục.
                Note = check.Note, // Ghi chú.
                RejectReason = check.RejectReason, // Lý do từ chối.
                ApprovedByEmployeeId = check.ApprovedByEmployeeId, // Mã quản trị viên phê duyệt.
                ApprovedByEmployeeName = check.ApprovedByEmployeeId.HasValue // Tên quản trị viên phê duyệt.
                    ? approverNames.GetValueOrDefault(check.ApprovedByEmployeeId.Value, check.ApprovedByEmployeeId.Value.ToString()) // Lấy tên từ Dictionary.
                    : null, // Mặc định null nếu chưa duyệt.
                ApprovedAt = check.ApprovedAt, // Thời gian phê duyệt.
                Details = check.Details.Select(d => new InventoryCheckDetailLineDto // Ánh xạ danh sách dòng chi tiết tổng hợp của biến thể.
                {
                    Id = d.Id, // Mã dòng chi tiết.
                    VariantId = d.VariantId, // Mã biến thể sản phẩm.
                    VariantName = d.Variant?.VariantName ?? string.Empty, // Tên biến thể.
                    SKU = d.Variant?.SKU ?? string.Empty, // Mã SKU.
                    SystemQuantity = d.SystemQuantity, // Số lượng sổ sách chốt.
                    ActualQuantity = d.ActualQuantity, // Số lượng thực tế quét.
                    Difference = d.Difference, // Chênh lệch số lượng (Actual - System).
                    MatchedQuantity = d.MatchedQuantity, // Số lượng khớp.
                    MissingQuantity = d.MissingQuantity, // Số lượng thiếu.
                    SurplusQuantity = d.SurplusQuantity, // Số lượng thừa.
                    DefectiveQuantity = d.DefectiveQuantity, // Số lượng lỗi vật lý.
                    Reason = d.Reason // Nguyên nhân chênh lệch chung.
                }).ToList() // Chuyển kết quả Select sang danh sách List DTO.
            };
        }

        private async Task<InventoryCheckDashboardDto> BuildMiniDashboardAsync(int checkId, InventoryCheck check) // Hàm phụ trợ xây dựng dashboard thông số mini trong quá trình quét serial.
        {
            var counts = await _checkRepo.GetGroupedCountsByCheckAsync(checkId); // Lấy số lượng serial theo nhóm trạng thái quét.
            var matched = counts.GetValueOrDefault((byte)InventoryScanStatus.Matched, 0); // Lấy số lượng khớp.
            var missing = counts.GetValueOrDefault((byte)InventoryScanStatus.Missing, 0); // Lấy số lượng thiếu.
            var surplus = counts.GetValueOrDefault((byte)InventoryScanStatus.Surplus, 0); // Lấy số lượng thừa.
            var unknown = counts.GetValueOrDefault((byte)InventoryScanStatus.UnknownSurplus, 0); // Lấy số lượng thừa mã lạ.
            var defective = counts.GetValueOrDefault((byte)InventoryScanStatus.Defective, 0); // Lấy số lượng lỗi vật lý.
            var pending = counts.GetValueOrDefault((byte)InventoryScanStatus.Pending, 0); // Lấy số lượng chờ quét.

            var totalSystem = await _context.InventoryCheckDetails // Đếm tổng số lượng sổ sách khả dụng đã chốt cho phiếu này.
                .AsNoTracking() // Tối ưu hóa hiệu năng đọc dữ liệu.
                .Where(d => d.CheckId == checkId) // Lọc theo mã phiếu kiểm kê.
                .SumAsync(d => d.SystemQuantity); // Tính tổng số lượng sổ sách.

            return new InventoryCheckDashboardDto // Khởi tạo DTO dashboard mini.
            {
                CheckId = checkId, // Mã phiếu.
                CheckCode = check.CheckCode, // Mã phiếu.
                Status = check.Status, // Mã trạng thái.
                StatusName = GetStatusName(check.Status), // Tên trạng thái.
                SnapshotAt = check.SnapshotAt, // Mốc chốt số liệu.
                TotalSystem = totalSystem, // Tổng số lượng sổ sách.
                TotalScanned = matched + surplus + unknown + defective, // Tổng số lượng thực tế đã quét.
                MatchedCount = matched, // Số lượng khớp.
                MissingCount = missing + pending, // Số lượng thiếu (Missing + Pending).
                SurplusCount = surplus, // Số lượng thừa.
                UnknownSurplusCount = unknown, // Số lượng thừa mã lạ.
                DefectiveCount = defective // Số lượng lỗi vật lý.
            };
        }

        private static InventoryCheckSerialDto MapSerialToDto(InventoryCheckDetailSerial s) // Hàm phụ trợ ánh xạ từ thực thể InventoryCheckDetailSerial sang InventoryCheckSerialDto.
        {
            return new InventoryCheckSerialDto // Khởi tạo DTO và gán dữ liệu tương ứng.
            {
                Id = s.Id, // Mã Id.
                SerialId = s.SerialId, // Mã Id serial.
                SerialNumberRaw = s.SerialNumberRaw, // Chuỗi serial quét.
                VariantId = s.VariantId, // Mã biến thể.
                VariantName = s.Variant?.VariantName, // Tên biến thể.
                SKU = s.Variant?.SKU, // SKU biến thể.
                OriginalStatus = s.OriginalStatus, // Mã trạng thái gốc.
                OriginalStatusName = s.OriginalStatus.HasValue // Tên trạng thái gốc.
                    ? ((SerialStatus)s.OriginalStatus.Value).ToString() // Chuyển đổi mã sang chuỗi Enum.
                    : null, // Trả về null nếu không có trạng thái gốc.
                ScanStatus = s.ScanStatus, // Mã trạng thái quét.
                ScanStatusName = GetScanStatusName(s.ScanStatus), // Tên trạng thái quét.
                ScannedAt = s.ScannedAt, // Thời điểm quét.
                Note = s.Note, // Chú thích/lý do chênh lệch.
                ProposedActionNote = s.ProposedActionNote, // Hướng xử lý đề xuất.
                ResolvedDuringApproval = s.ResolvedDuringApproval // Đánh dấu giải quyết trong cửa sổ kiểm duyệt.
            };
        }

        private async Task<Dictionary<Guid, string>> GetEmployeeNamesAsync(List<Guid> employeeIds) // Hàm phụ trợ truy vấn hàng loạt họ tên nhân viên dựa trên danh sách Id.
        {
            if (!employeeIds.Any()) return new Dictionary<Guid, string>(); // Trả về Dictionary rỗng nếu danh sách đầu vào rỗng.
            return await _context.Users // Kết nối bảng Users và UserProfiles để lấy họ tên đầy đủ.
                .AsNoTracking() // Tối ưu hiệu năng đọc.
                .Where(u => employeeIds.Contains(u.Id)) // Lọc theo danh sách Id.
                .Join(_context.UserProfiles, // Thực hiện phép Join với bảng UserProfiles qua khóa UserId.
                    u => u.Id, // Khóa ngoại của bảng Users.
                    p => p.UserId, // Khóa ngoại của bảng UserProfiles.
                    (u, p) => new { u.Id, FullName = p.FullName ?? u.UserName ?? u.Id.ToString() }) // Trích xuất Id và họ tên (hoặc dùng username/Id nếu null).
                .ToDictionaryAsync(x => x.Id, x => x.FullName); // Chuyển đổi bất đồng bộ kết quả thành một Dictionary tra cứu.
        }

        private static string GetStatusName(byte status) => status switch // Hàm phụ trợ chuyển mã trạng thái phiếu kiểm kê sang tên hiển thị tiếng Việt.
        {
            0 => "Nháp", // 0: Draft.
            1 => "Chờ duyệt", // 1: AwaitingApproval.
            2 => "Đã hoàn tất", // 2: Completed.
            3 => "Đã hủy", // 3: Cancelled.
            _ => "Không xác định" // Các giá trị khác.
        };

        private static string GetScanStatusName(byte status) => status switch // Hàm phụ trợ chuyển mã trạng thái quét của serial sang tên hiển thị tiếng Việt.
        {
            0 => "Chờ quét", // 0: Pending.
            1 => "Khớp", // 1: Matched.
            2 => "Thiếu", // 2: Missing.
            3 => "Thừa", // 3: Surplus.
            4 => "Thừa (Mã lạ)", // 4: UnknownSurplus.
            5 => "Lỗi vật lý", // 5: Defective.
            _ => "Không xác định" // Các giá trị khác.
        };
    }
}

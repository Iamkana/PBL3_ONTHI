using Microsoft.Extensions.Logging; // Sử dụng thư viện ghi log hệ thống.
using PBL3.Core.Entities; // Sử dụng các thực thể Voucher, VoucherCategory.
using PBL3.Core.Interfaces; // Sử dụng các giao diện repository từ tầng Core.
using PBL3.Shared.DTOs.Common; // Sử dụng DTO dùng chung ApiResult.
using PBL3.Shared.DTOs.Vouchers; // Sử dụng các DTO của module voucher.

namespace PBL3.Application.Vouchers // Khai báo namespace cho lớp dịch vụ voucher thuộc tầng Application.
{
    public class VoucherService( // Định nghĩa lớp VoucherService sử dụng Primary Constructor.
        IVoucherRepository voucherRepo, // Tiêm repository voucher.
        ILogger<VoucherService> logger) : IVoucherService // Tiêm logger hệ thống và triển khai giao diện IVoucherService.
    {
        private readonly IVoucherRepository _voucherRepo = // Gán repository voucher vào trường thành viên.
            voucherRepo ?? throw new ArgumentNullException(nameof(voucherRepo)); // Kiểm tra null cho voucherRepo.
        private readonly ILogger<VoucherService> _logger = // Gán logger vào trường thành viên.
            logger ?? throw new ArgumentNullException(nameof(logger)); // Kiểm tra null cho logger.

        public async Task<ApiResult<PagedResult<VoucherDto>>> GetPagedListAsync(VoucherFilterRequest filter) // Định nghĩa phương thức lấy danh sách voucher phân trang bất đồng bộ.
        {
            var (items, totalCount) = await _voucherRepo.GetPagedListAsync( // Gọi repository lấy danh sách voucher phân trang và tổng số lượng khớp bộ lọc.
                filter.Keyword, // Lọc theo từ khóa tìm kiếm.
                filter.StatusFilter, // Lọc theo bộ lọc trạng thái voucher.
                filter.FromDate, // Lọc theo ngày bắt đầu.
                filter.ToDate, // Lọc theo ngày kết thúc.
                filter.PageNumber, // Số trang hiện tại.
                filter.PageSize, // Kích thước trang cần lấy.
                filter.SortBy, // Cột cần sắp xếp.
                filter.SortDescending); // Sắp xếp giảm dần hay không.

            var result = new PagedResult<VoucherDto> // Khởi tạo kết quả phân trang DTO.
            {
                Items      = items.Select(MapToDto).ToList(), // Ánh xạ danh sách thực thể sang DTO và gán vào Items.
                TotalCount = totalCount, // Gán tổng số lượng bản ghi.
                PageNumber = filter.PageNumber, // Gán trang hiện tại.
                PageSize   = filter.PageSize // Gán số lượng phần tử trên trang.
            };

            return ApiResult<PagedResult<VoucherDto>>.Ok(result); // Trả về kết quả phân trang thành công kèm DTO.
        }

        public async Task<ApiResult<VoucherDto>> GetByIdAsync(int id) // Định nghĩa phương thức lấy chi tiết voucher theo Id.
        {
            var voucher = await _voucherRepo.GetByIdWithCategoriesAsync(id); // Lấy thực thể voucher kèm danh mục liên kết theo Id từ repo.

            if (voucher == null) // Nếu không tìm thấy thực thể voucher nào khớp với Id.
                return ApiResult<VoucherDto>.Fail("Không tìm thấy voucher yêu cầu.", ApiErrorCode.NotFound); // Báo lỗi NotFound 404.

            return ApiResult<VoucherDto>.Ok(MapToDto(voucher)); // Ánh xạ thực thể sang DTO và trả về kết quả thành công.
        }

        public async Task<ApiResult<VoucherDto>> CreateAsync(CreateVoucherRequest request) // Định nghĩa phương thức tạo mới voucher.
        {
            var normalizedCode = request.Code.Trim().ToUpper(); // Chuẩn hóa mã voucher (loại bỏ khoảng trắng và chuyển sang chữ hoa).

            if (await _voucherRepo.IsDuplicateCodeAsync(normalizedCode)) // Kiểm tra xem mã voucher này đã tồn tại trong hệ thống chưa.
                return ApiResult<VoucherDto>.Fail($"Mã voucher \"{normalizedCode}\" đã tồn tại trong hệ thống.", ApiErrorCode.Conflict); // Báo lỗi Conflict nếu bị trùng mã.

            var voucher = new Voucher // Khởi tạo thực thể voucher mới.
            {
                Code             = normalizedCode, // Gán mã voucher đã chuẩn hóa.
                Name             = request.Name.Trim(), // Gán tên chương trình voucher (cắt khoảng trắng).
                DiscountType     = request.DiscountType, // Gán loại giảm giá (0 = cố định, 1 = phần trăm).
                DiscountValue    = request.DiscountValue, // Gán giá trị giảm giá.
                MinOrderValue    = request.MinOrderValue, // Gán giá trị đơn hàng tối thiểu cần thiết để áp dụng.
                MaxDiscountAmount = request.MaxDiscountAmount, // Gán số tiền giảm tối đa (cho loại chiết khấu phần trăm).
                StartDate        = request.StartDate, // Gán ngày bắt đầu có hiệu lực.
                EndDate          = request.EndDate, // Gán ngày kết thúc hiệu lực.
                Quantity         = request.Quantity, // Gán tổng số lượt sử dụng tối đa (null nếu không giới hạn).
                MaxUsesPerUser   = request.MaxUsesPerUser, // Gán giới hạn lượt dùng tối đa của mỗi khách hàng.
                ApplyFor         = request.ApplyFor, // Gán kênh áp dụng (0 = cả hai, 1 = Online, 2 = Offline).
                IsStackable      = request.IsStackable, // Gán thuộc tính cho phép cộng dồn với voucher khác hay không.
                IsActive         = request.IsActive, // Gán trạng thái hoạt động.
                Description      = request.Description?.Trim(), // Gán mô tả chi tiết của voucher.
                CreatedDate      = DateTime.UtcNow, // Gán ngày tạo theo giờ UTC hiện tại.
                IsDeleted        = false, // Thiết lập trạng thái chưa bị xóa.
                UsedCount        = 0 // Khởi tạo số lượng đã dùng ban đầu bằng 0.
            };

            if (request.CategoryIds != null && request.CategoryIds.Count > 0) // Nếu có danh sách Id danh mục sản phẩm được áp dụng voucher.
            {
                foreach (var categoryId in request.CategoryIds.Distinct()) // Duyệt qua từng Id danh mục duy nhất trong danh sách.
                {
                    voucher.VoucherCategories.Add(new VoucherCategory // Thêm liên kết danh mục được áp dụng vào voucher.
                    {
                        CategoryId = categoryId // Gán mã danh mục.
                    });
                }
            }

            await _voucherRepo.AddAsync(voucher); // Thêm thực thể voucher mới vào cơ sở dữ liệu.
            await _voucherRepo.SaveChangesAsync(); // Lưu thay đổi xuống cơ sở dữ liệu.

            _logger.LogInformation("Tạo voucher mới: {Code} — {Name} (Id: {Id})", // Ghi log thông báo tạo mới voucher thành công.
                voucher.Code, voucher.Name, voucher.Id); // Ghi nhận mã, tên và Id của voucher mới tạo.

            return ApiResult<VoucherDto>.Ok(MapToDto(voucher), "Tạo voucher thành công."); // Trả về DTO kết quả tạo thành công.
        }

        public async Task<ApiResult<VoucherDto>> UpdateAsync(int id, UpdateVoucherRequest request) // Định nghĩa phương thức cập nhật voucher theo Id.
        {
            var voucher = await _voucherRepo.GetByIdWithCategoriesAsync(id); // Lấy thông tin thực thể voucher hiện tại kèm danh mục từ repository.

            if (voucher == null) // Nếu không tìm thấy thực thể voucher cần cập nhật.
                return ApiResult<VoucherDto>.Fail("Không tìm thấy voucher yêu cầu.", ApiErrorCode.NotFound); // Báo lỗi NotFound.

            if (request.Quantity.HasValue && request.Quantity.Value < voucher.UsedCount) // Kiểm tra số lượng phát hành mới có nhỏ hơn số lượt đã thực tế sử dụng không.
                return ApiResult<VoucherDto>.Fail( // Trả về lỗi nghiệp vụ nếu số lượng phát hành mới không hợp lệ.
                    $"Số lượng phát hành ({request.Quantity}) không được nhỏ hơn số lần đã sử dụng ({voucher.UsedCount})."); // Chi tiết thông báo lỗi.

            voucher.Name             = request.Name.Trim(); // Cập nhật tên voucher.
            voucher.DiscountType     = request.DiscountType; // Cập nhật loại giảm giá.
            voucher.DiscountValue    = request.DiscountValue; // Cập nhật giá trị giảm giá.
            voucher.MinOrderValue    = request.MinOrderValue; // Cập nhật giá trị đơn hàng tối thiểu.
            voucher.MaxDiscountAmount = request.MaxDiscountAmount; // Cập nhật số tiền giảm giá tối đa.
            voucher.StartDate        = request.StartDate; // Cập nhật ngày bắt đầu hiệu lực.
            voucher.EndDate          = request.EndDate; // Cập nhật ngày hết hạn hiệu lực.
            voucher.Quantity         = request.Quantity; // Cập nhật tổng số lượng phát hành.
            voucher.MaxUsesPerUser   = request.MaxUsesPerUser; // Cập nhật số lần dùng tối đa của mỗi khách hàng.
            voucher.ApplyFor         = request.ApplyFor; // Cập nhật kênh áp dụng.
            voucher.IsStackable      = request.IsStackable; // Cập nhật thuộc tính cộng dồn.
            voucher.IsActive         = request.IsActive; // Cập nhật trạng thái hoạt động.
            voucher.Description      = request.Description?.Trim(); // Cập nhật mô tả.

            voucher.VoucherCategories.Clear(); // Xóa sạch danh sách liên kết danh mục áp dụng cũ.
            if (request.CategoryIds != null && request.CategoryIds.Count > 0) // Nếu có danh sách danh mục áp dụng mới.
            {
                foreach (var categoryId in request.CategoryIds.Distinct()) // Duyệt qua từng Id danh mục duy nhất trong danh sách.
                {
                    voucher.VoucherCategories.Add(new VoucherCategory // Thêm liên kết danh mục mới được áp dụng.
                    {
                        VoucherId  = voucher.Id, // Gán Id của voucher hiện tại.
                        CategoryId = categoryId // Gán Id của danh mục.
                    });
                }
            }

            await _voucherRepo.SaveChangesAsync(); // Lưu tất cả thay đổi cập nhật xuống cơ sở dữ liệu.

            _logger.LogInformation("Cập nhật voucher: {Code} (Id: {Id})", voucher.Code, voucher.Id); // Ghi log thông báo cập nhật voucher thành công.

            return ApiResult<VoucherDto>.Ok(MapToDto(voucher), "Cập nhật voucher thành công."); // Trả về kết quả cập nhật kèm DTO thành công.
        }

        public async Task<ApiResult<bool>> DeleteAsync(int id) // Định nghĩa phương thức xóa mềm voucher theo Id.
        {
            var voucher = await _voucherRepo.GetByIdWithCategoriesAsync(id); // Lấy thông tin thực thể voucher từ repo theo Id.

            if (voucher == null) // Nếu không tìm thấy thực thể voucher phù hợp.
                return ApiResult<bool>.Fail("Không tìm thấy voucher yêu cầu.", ApiErrorCode.NotFound); // Báo lỗi NotFound.

            voucher.IsDeleted   = true; // Thực hiện đánh dấu trạng thái xóa mềm.
            voucher.DeletedDate = DateTime.UtcNow; // Ghi nhận thời gian xóa mềm theo chuẩn UTC.
            voucher.IsActive    = false; // Vô hiệu hóa hoạt động của voucher ngay lập tức.

            await _voucherRepo.SaveChangesAsync(); // Lưu thay đổi xuống cơ sở dữ liệu.

            _logger.LogInformation("Xóa voucher: {Code} (Id: {Id})", voucher.Code, voucher.Id); // Ghi log thông báo xóa mềm voucher thành công.

            return ApiResult<bool>.Ok(true, "Xóa voucher thành công."); // Trả về kết quả thành công dạng boolean.
        }

        public async Task<ApiResult<VoucherDto>> ToggleStatusAsync(int id) // Định nghĩa phương thức bật/tắt trạng thái hoạt động của voucher.
        {
            var voucher = await _voucherRepo.GetByIdWithCategoriesAsync(id); // Lấy thực thể voucher từ repository theo Id.

            if (voucher == null) // Nếu không tìm thấy voucher phù hợp.
                return ApiResult<VoucherDto>.Fail("Không tìm thấy voucher yêu cầu.", ApiErrorCode.NotFound); // Báo lỗi NotFound.

            voucher.IsActive = !voucher.IsActive; // Đảo ngược trạng thái hoạt động của voucher.

            await _voucherRepo.SaveChangesAsync(); // Lưu thay đổi xuống cơ sở dữ liệu.

            var status = voucher.IsActive ? "kích hoạt" : "vô hiệu hóa"; // Chuẩn bị từ mô tả trạng thái mới.
            _logger.LogInformation("{Status} voucher: {Code} (Id: {Id})", status, voucher.Code, voucher.Id); // Ghi log thông báo thay đổi trạng thái thành công.

            return ApiResult<VoucherDto>.Ok(MapToDto(voucher), $"Đã {status} voucher thành công."); // Trả về DTO kèm thông điệp thay đổi trạng thái thành công.
        }

        public async Task<ApiResult<ValidateVoucherResponse>> ValidateVoucherCodeAsync( // Định nghĩa phương thức kiểm tra và xác thực mã voucher.
            ValidateVoucherRequest request, // Tham số yêu cầu xác thực.
            Guid? userId) // Tham số Id của người dùng hiện tại (nếu có).
        {
            var vouchers = await _voucherRepo.GetByCodesWithCategoriesAsync( // Tìm thực thể voucher trong cơ sở dữ liệu theo mã code.
                new List<string> { request.Code.Trim().ToUpper() }); // Truyền danh sách mã code cần tìm đã chuẩn hóa chữ hoa.

            var voucher = vouchers.FirstOrDefault(); // Lấy thực thể voucher đầu tiên tìm được.

            if (voucher == null) // Nếu không tìm thấy thực thể voucher nào khớp với mã code.
            {
                return ApiResult<ValidateVoucherResponse>.Ok(new ValidateVoucherResponse // Trả về kết quả xem như voucher không hợp lệ.
                {
                    IsValid = false, // Trạng thái không hợp lệ.
                    ErrorMessage = "Mã voucher không tồn tại.", // Chi tiết thông báo lỗi.
                    Code = request.Code // Gán mã voucher tương ứng.
                });
            }

            var now = DateTime.UtcNow; // Lấy thời gian hiện tại theo UTC.

            if (!voucher.IsActive) // Kiểm tra xem voucher có đang ở trạng thái kích hoạt hoạt động không.
                return OkInvalid(request.Code, "Mã voucher đã bị vô hiệu hóa."); // Báo lỗi voucher bị vô hiệu hóa.

            if (now < voucher.StartDate) // Kiểm tra xem thời gian hiện tại đã đạt tới ngày bắt đầu áp dụng của voucher chưa.
                return OkInvalid(request.Code, $"Mã voucher chưa đến thời gian sử dụng (từ {voucher.StartDate:dd/MM/yyyy})."); // Báo lỗi thời gian áp dụng chưa bắt đầu.

            if (now > voucher.EndDate) // Kiểm tra xem voucher đã hết hạn sử dụng chưa.
                return OkInvalid(request.Code, $"Mã voucher đã hết hạn vào ngày {voucher.EndDate:dd/MM/yyyy}."); // Báo lỗi voucher hết hạn.

            if (voucher.Quantity.HasValue && voucher.UsedCount >= voucher.Quantity.Value) // Kiểm tra xem số lượt đã dùng có vượt quá số lượt phát hành tối đa không.
                return OkInvalid(request.Code, "Mã voucher đã hết lượt sử dụng."); // Báo lỗi hết lượt dùng của hệ thống.

            if (request.SubTotal < voucher.MinOrderValue) // Kiểm tra xem giá trị tạm tính của đơn hàng có đạt giá trị đơn hàng tối thiểu quy định không.
                return OkInvalid(request.Code, // Báo lỗi chưa đạt giá trị đơn tối thiểu.
                    $"Đơn hàng chưa đạt giá trị tối thiểu {voucher.MinOrderValue:#,0}đ để sử dụng mã này."); // Chi tiết số tiền tối thiểu cần đạt.

            if (request.IsOnlineOrder && voucher.ApplyFor == 2) // Nếu là đơn hàng Online nhưng voucher chỉ áp dụng cho offline tại quầy.
                return OkInvalid(request.Code, "Mã voucher này chỉ áp dụng tại quầy."); // Báo lỗi kênh offline.

            if (!request.IsOnlineOrder && voucher.ApplyFor == 1) // Nếu là đơn hàng offline tại quầy nhưng voucher chỉ áp dụng cho mua online.
                return OkInvalid(request.Code, "Mã voucher này chỉ áp dụng cho đơn hàng online."); // Báo lỗi kênh online.

            if (voucher.VoucherCategories.Any() && request.OrderItemCategoryIds != null && request.OrderItemCategoryIds.Any()) // Nếu voucher giới hạn theo danh mục và đơn hàng có danh mục sản phẩm.
            {
                var voucherCategoryIds = voucher.VoucherCategories.Select(vc => vc.CategoryId).ToHashSet(); // Chuyển đổi danh sách Id danh mục áp dụng của voucher thành HashSet để kiểm tra nhanh.
                if (!request.OrderItemCategoryIds.Any(catId => voucherCategoryIds.Contains(catId))) // Kiểm tra xem đơn hàng có chứa sản phẩm nào thuộc danh mục áp dụng không.
                    return OkInvalid(request.Code, "Mã voucher không áp dụng cho các sản phẩm trong đơn hàng."); // Báo lỗi không có sản phẩm phù hợp danh mục.
            }

            if (userId.HasValue && voucher.MaxUsesPerUser.HasValue) // Nếu người dùng đã đăng nhập và voucher giới hạn lượt dùng của mỗi khách.
            {
                var usageCounts = await _voucherRepo.GetUserVoucherUsageCountsAsync( // Lấy số lần khách đã dùng voucher này từ repository.
                    userId.Value, new List<int> { voucher.Id }); // Truyền Id người dùng và danh sách Id voucher.
                var currentCount = usageCounts.GetValueOrDefault(voucher.Id, 0); // Lấy số lần đã dùng thực tế, mặc định bằng 0.
                if (currentCount >= voucher.MaxUsesPerUser.Value) // Nếu số lượt đã dùng đạt giới hạn tối đa cho phép của khách hàng.
                    return OkInvalid(request.Code, // Báo lỗi vượt quá giới hạn dùng cá nhân.
                        $"Bạn đã sử dụng mã này {currentCount} lần (tối đa {voucher.MaxUsesPerUser} lần/khách)."); // Chi tiết lượt dùng tối đa.
            }

            decimal discountAmount; // Khai báo biến lưu số tiền được giảm giá.
            if (voucher.DiscountType == 0) // Nếu loại giảm giá là tiền cố định.
            {
                discountAmount = voucher.DiscountValue; // Số tiền giảm bằng chính giá trị giảm cố định của voucher.
            }
            else // Nếu loại giảm giá là theo tỷ lệ phần trăm.
            {
                discountAmount = request.SubTotal * voucher.DiscountValue / 100; // Tính số tiền giảm theo tỷ lệ phần trăm của tổng đơn.
                if (voucher.MaxDiscountAmount.HasValue && discountAmount > voucher.MaxDiscountAmount.Value) // Nếu có cấu hình số tiền giảm tối đa và số tiền tính được lớn hơn mức khống chế này.
                    discountAmount = voucher.MaxDiscountAmount.Value; // Giới hạn số tiền giảm bằng số tiền giảm tối đa cho phép.
            }

            return ApiResult<ValidateVoucherResponse>.Ok(new ValidateVoucherResponse // Trả về kết quả xác thực hợp lệ kèm số tiền giảm tính toán được.
            {
                IsValid        = true, // Đánh dấu voucher hợp lệ.
                DiscountAmount = discountAmount, // Gán số tiền được giảm.
                VoucherName    = voucher.Name, // Gán tên chương trình voucher.
                Code           = voucher.Code // Gán mã voucher.
            });
        }

        public async Task<ApiResult<List<VoucherAvailabilityDto>>> GetAvailableForOrderAsync( // Định nghĩa phương thức lấy danh sách các voucher khả dụng cho một giỏ hàng cụ thể.
            GetAvailableVouchersRequest request, Guid? userId) // Tham số chi tiết đơn hàng và Id người dùng (nếu có).
        {
            var vouchers = await _voucherRepo.GetActiveVouchersForCustomerAsync(); // Lấy danh sách các voucher đang hoạt động và trong thời hạn áp dụng từ repository.

            Dictionary<int, int>? usageCounts = null; // Khai báo Dictionary lưu lượt dùng voucher của khách hàng.
            if (userId.HasValue && vouchers.Any()) // Nếu khách hàng đã đăng nhập và hệ thống có voucher hoạt động.
            {
                var ids = vouchers.Select(v => v.Id).ToList(); // Lấy danh sách Id của tất cả các voucher đang hoạt động.
                usageCounts = await _voucherRepo.GetUserVoucherUsageCountsAsync(userId.Value, ids); // Lấy số lần khách đã dùng từng voucher tương ứng.
            }

            var result = vouchers.Select(v => // Duyệt qua danh sách voucher và kiểm tra tính khả dụng của từng mã.
            {
                string? reason = null; // Khởi tạo lý do không khả dụng mặc định là null.

                if (request.SubTotal < v.MinOrderValue) // 1. Kiểm tra điều kiện giá trị đơn hàng tối thiểu.
                    reason = $"Đơn từ {v.MinOrderValue:#,0}đ (bạn: {request.SubTotal:#,0}đ)"; // Thiết lập lý do chưa đạt giá trị đơn tối thiểu.
                else if (request.IsOnlineOrder && v.ApplyFor == 2) // 2. Kiểm tra điều kiện kênh bán hàng (nếu là đơn Online nhưng voucher chỉ dùng tại quầy).
                    reason = "Chỉ áp dụng tại quầy"; // Thiết lập lý do sai kênh bán hàng.
                else if (!request.IsOnlineOrder && v.ApplyFor == 1) // 3. Kiểm tra kênh bán hàng (nếu là đơn tại quầy nhưng voucher chỉ dùng online).
                    reason = "Chỉ áp dụng online"; // Thiết lập lý do sai kênh.
                else if (userId.HasValue && v.MaxUsesPerUser.HasValue) // 4. Kiểm tra giới hạn lượt dùng tối đa của riêng khách hàng này.
                {
                    var used = usageCounts?.GetValueOrDefault(v.Id, 0) ?? 0; // Lấy số lần khách hàng đã sử dụng voucher này.
                    if (used >= v.MaxUsesPerUser.Value) // Nếu lượt dùng đạt hoặc vượt giới hạn cá nhân cho phép.
                        reason = $"Bạn đã dùng {used}/{v.MaxUsesPerUser} lần"; // Thiết lập lý do hết lượt dùng cá nhân.
                }

                decimal discount = 0; // Khởi tạo giá trị giảm dự kiến mặc định là 0.
                if (reason == null) // Nếu voucher khả dụng (không có lý do từ chối nào).
                {
                    discount = v.DiscountType == 0 // Kiểm tra loại giảm giá.
                        ? v.DiscountValue // Giảm giá cố định.
                        : Math.Min( // Giảm giá theo phần trăm và khống chế mức giảm tối đa.
                            request.SubTotal * v.DiscountValue / 100, // Số tiền giảm theo tỷ lệ phần trăm tính toán được.
                            v.MaxDiscountAmount ?? decimal.MaxValue); // Khống chế tối đa (mặc định lấy MaxValue nếu không giới hạn).
                }

                return new VoucherAvailabilityDto // Trả về DTO thông tin tính khả dụng của voucher.
                {
                    Id = v.Id, Code = v.Code, Name = v.Name, Description = v.Description, // Ánh xạ các thông tin cơ bản.
                    DiscountType = v.DiscountType, DiscountValue = v.DiscountValue, // Ánh xạ cấu hình chiết khấu.
                    MaxDiscountAmount = v.MaxDiscountAmount, MinOrderValue = v.MinOrderValue, // Ánh xạ giới hạn chiết khấu và đơn hàng.
                    StartDate = v.StartDate, EndDate = v.EndDate, IsStackable = v.IsStackable, // Ánh xạ hiệu lực và tính cộng dồn.
                    IsApplicable = reason == null, // Gán trạng thái có khả dụng áp dụng được hay không.
                    EstimatedDiscount = discount, // Gán số tiền giảm giá dự kiến.
                    NotApplicableReason = reason // Gán lý do không khả dụng nếu có.
                };
            })
            .OrderByDescending(v => v.IsApplicable) // Sắp xếp ưu tiên các voucher dùng được lên trước.
            .ThenByDescending(v => v.EstimatedDiscount) // Tiếp theo sắp xếp giảm dần theo số tiền giảm dự kiến được nhiều nhất.
            .ToList(); // Chuyển kết quả sang danh sách List.

            return ApiResult<List<VoucherAvailabilityDto>>.Ok(result); // Trả về danh sách voucher khả dụng thành công.
        }

        private static ApiResult<ValidateVoucherResponse> OkInvalid(string code, string error) => // Định nghĩa hàm hỗ trợ đóng gói nhanh kết quả xác thực voucher không hợp lệ.
            ApiResult<ValidateVoucherResponse>.Ok(new ValidateVoucherResponse // Trả về API Result thành công chứa thông tin voucher không hợp lệ.
            {
                IsValid      = false, // Đánh dấu không hợp lệ.
                ErrorMessage = error, // Gán thông báo lỗi chi tiết.
                Code         = code // Gán mã voucher tương ứng.
            });

        private static VoucherDto MapToDto(Voucher e) => new() // Định nghĩa hàm hỗ trợ ánh xạ thực thể Voucher sang VoucherDto.
        {
            Id               = e.Id, // Ánh xạ Id.
            Code             = e.Code, // Ánh xạ Code.
            Name             = e.Name, // Ánh xạ Name.
            DiscountType     = e.DiscountType, // Ánh xạ loại chiết khấu.
            DiscountValue    = e.DiscountValue, // Ánh xạ giá trị giảm.
            MinOrderValue    = e.MinOrderValue, // Ánh xạ đơn tối thiểu.
            MaxDiscountAmount = e.MaxDiscountAmount, // Ánh xạ số tiền giảm tối đa.
            StartDate        = e.StartDate, // Ánh xạ ngày bắt đầu hiệu lực.
            EndDate          = e.EndDate, // Ánh xạ ngày kết thúc hiệu lực.
            Quantity         = e.Quantity, // Ánh xạ tổng số lượt phát hành.
            UsedCount        = e.UsedCount, // Ánh xạ số lượt đã sử dụng.
            MaxUsesPerUser   = e.MaxUsesPerUser, // Ánh xạ lượt dùng tối đa của mỗi khách.
            ApplyFor         = e.ApplyFor, // Ánh xạ kênh bán hàng áp dụng.
            IsStackable      = e.IsStackable, // Ánh xạ thuộc tính cộng dồn.
            IsActive         = e.IsActive, // Ánh xạ trạng thái hoạt động.
            Description      = e.Description, // Ánh xạ mô tả voucher.
            CreatedDate      = e.CreatedDate, // Ánh xạ ngày khởi tạo.
            CategoryIds      = e.VoucherCategories.Select(vc => vc.CategoryId).ToList() // Ánh xạ danh sách các danh mục sản phẩm được áp dụng.
        };
    }
}

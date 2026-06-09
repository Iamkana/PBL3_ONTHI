using FluentValidation; // Sử dụng thư viện FluentValidation để kiểm tra tính hợp lệ của dữ liệu đầu vào.
using Microsoft.Extensions.Logging; // Sử dụng thư viện Logging để ghi nhật ký hệ thống (log).
using PBL3.Core.Entities; // Tham chiếu các thực thể cốt lõi (Domain Entities) như Banner.
using PBL3.Core.Interfaces; // Tham chiếu các giao diện (Interfaces) của tầng Core.
using PBL3.Shared.DTOs.Banners; // Sử dụng các DTO (Data Transfer Objects) liên quan đến Banner.
using PBL3.Shared.DTOs.Common; // Sử dụng các DTO dùng chung của ứng dụng.

namespace PBL3.Application.Banners // Khai báo namespace cho lớp dịch vụ BannerService thuộc tầng Application.
{
    public class BannerService( // Khai báo lớp BannerService và sử dụng Primary Constructor (C# 12) để tiêm phụ thuộc.
        IBannerRepository bannerRepo, // Tiêm repository của Banner để truy xuất dữ liệu.
        IValidator<CreateBannerRequest> createValidator, // Tiêm validator cho yêu cầu tạo Banner.
        IValidator<UpdateBannerRequest> updateValidator, // Tiêm validator cho yêu cầu cập nhật Banner.
        ILogger<BannerService> logger) : IBannerService // Tiêm Logger và kế thừa giao diện IBannerService.
    {
        private readonly IBannerRepository _bannerRepo = // Khai báo trường readonly để lưu trữ repository Banner.
            bannerRepo ?? throw new ArgumentNullException(nameof(bannerRepo)); // Kiểm tra null và gán giá trị cho repository Banner.
        private readonly IValidator<CreateBannerRequest> _createValidator = // Khai báo trường readonly để lưu validator tạo Banner.
            createValidator ?? throw new ArgumentNullException(nameof(createValidator)); // Kiểm tra null và gán giá trị cho validator tạo Banner.
        private readonly IValidator<UpdateBannerRequest> _updateValidator = // Khai báo trường readonly để lưu validator cập nhật Banner.
            updateValidator ?? throw new ArgumentNullException(nameof(updateValidator)); // Kiểm tra null và gán giá trị cho validator cập nhật Banner.
        private readonly ILogger<BannerService> _logger = // Khai báo trường readonly để lưu Logger ghi log.
            logger ?? throw new ArgumentNullException(nameof(logger)); // Kiểm tra null và gán giá trị cho Logger.

        public async Task<ApiResult<PagedResult<BannerDto>>> GetPagedListAsync(BannerFilterRequest filter) // Định nghĩa phương thức lấy danh sách Banner phân trang bất đồng bộ.
        {
            var (items, totalCount) = await _bannerRepo.GetPagedListAsync( // Gọi repository để lấy danh sách Banner và tổng số lượng dựa trên bộ lọc.
                filter.Keyword, filter.PageNumber, filter.PageSize, filter.SortBy, filter.SortDescending); // Truyền các tham số lọc, phân trang và sắp xếp.

            var dtos = items.Select(MapToDto).ToList(); // Chuyển đổi danh sách thực thể Banner sang danh sách DTO để trả về.

            var result = new PagedResult<BannerDto> // Khởi tạo đối tượng kết quả phân trang PagedResult.
            {
                Items      = dtos, // Gán danh sách DTO.
                TotalCount = totalCount, // Gán tổng số lượng bản ghi tìm thấy.
                PageNumber = filter.PageNumber, // Gán chỉ số trang hiện tại.
                PageSize   = filter.PageSize // Gán số lượng phần tử trên mỗi trang.
            };

            return ApiResult<PagedResult<BannerDto>>.Ok(result); // Trả về kết quả thành công chứa dữ liệu phân trang.
        }

        public async Task<ApiResult<BannerDto>> GetByIdAsync(int id) // Định nghĩa phương thức lấy thông tin chi tiết Banner theo Id.
        {
            var banner = await _bannerRepo.GetByIdAsync(id); // Gọi repository lấy Banner theo Id bất đồng bộ.
            if (banner == null) // Nếu không tìm thấy Banner trong cơ sở dữ liệu.
                return ApiResult<BannerDto>.Fail("Không tìm thấy banner yêu cầu.", ApiErrorCode.NotFound); // Trả về kết quả thất bại với mã lỗi NotFound.

            return ApiResult<BannerDto>.Ok(MapToDto(banner)); // Trả về kết quả thành công chứa DTO của Banner.
        }

        public async Task<ApiResult<List<BannerPublicDto>>> GetActiveAsync() // Định nghĩa phương thức lấy danh sách Banner đang hoạt động công khai.
        {
            var banners = await _bannerRepo.GetActiveAsync(DateTime.UtcNow); // Gọi repository lấy các Banner đang hoạt động tính đến thời điểm hiện tại.

            var dtos = banners.Select(b => new BannerPublicDto // Chuyển đổi danh sách Banner thực thể sang BannerPublicDto để hiển thị cho Client công khai.
            {
                Id       = b.Id, // Gán Id của Banner.
                Title    = b.Title, // Gán tiêu đề Banner.
                ImageUrl = b.ImageUrl, // Gán đường dẫn ảnh Banner.
                LinkUrl  = b.LinkUrl // Gán đường dẫn liên kết khi nhấn vào Banner.
            }).ToList(); // Chuyển đổi kết quả Select sang List.

            return ApiResult<List<BannerPublicDto>>.Ok(dtos); // Trả về kết quả thành công chứa danh sách DTO công khai.
        }

        public async Task<ApiResult<BannerDto>> CreateAsync(CreateBannerRequest request) // Định nghĩa phương thức tạo mới Banner.
        {
            var validation = await _createValidator.ValidateAsync(request); // Thực hiện validate dữ liệu yêu cầu tạo mới bất đồng bộ.
            if (!validation.IsValid) // Nếu dữ liệu không hợp lệ.
                return ApiResult<BannerDto>.Fail(validation.Errors.First().ErrorMessage, ApiErrorCode.Validation); // Trả về kết quả lỗi với thông báo validate đầu tiên.

            var banner = new Banner // Khởi tạo đối tượng thực thể Banner mới.
            {
                Title       = request.Title.Trim(), // Gán tiêu đề đã cắt bỏ khoảng trắng thừa.
                ImageUrl    = request.ImageUrl.Trim(), // Gán đường dẫn ảnh đã cắt bỏ khoảng trắng thừa.
                LinkUrl     = string.IsNullOrWhiteSpace(request.LinkUrl) ? null : request.LinkUrl.Trim(), // Gán liên kết (null nếu trống) đã cắt khoảng trắng.
                SortOrder   = request.SortOrder, // Gán thứ tự sắp xếp của Banner.
                IsActive    = request.IsActive, // Gán trạng thái kích hoạt của Banner.
                StartDate   = request.StartDate, // Gán ngày bắt đầu hiển thị Banner.
                EndDate     = request.EndDate, // Gán ngày kết thúc hiển thị Banner.
                CreatedDate = DateTime.UtcNow, // Gán ngày tạo là thời gian UTC hiện tại.
                IsDeleted   = false // Gán mặc định trạng thái chưa xóa cho Banner mới.
            };

            await _bannerRepo.AddAsync(banner); // Thêm thực thể Banner mới vào DbContext thông qua repository.
            await _bannerRepo.SaveChangesAsync(); // Lưu các thay đổi vào cơ sở dữ liệu.

            _logger.LogInformation("Tạo banner mới: {Title} (Id: {Id})", banner.Title, banner.Id); // Ghi log thông báo tạo Banner thành công.

            return ApiResult<BannerDto>.Ok(MapToDto(banner), "Tạo banner thành công."); // Trả về kết quả thành công kèm DTO và thông báo.
        }

        public async Task<ApiResult<BannerDto>> UpdateAsync(int id, UpdateBannerRequest request) // Định nghĩa phương thức cập nhật thông tin Banner.
        {
            var validation = await _updateValidator.ValidateAsync(request); // Thực hiện validate dữ liệu yêu cầu cập nhật.
            if (!validation.IsValid) // Nếu dữ liệu không hợp lệ.
                return ApiResult<BannerDto>.Fail(validation.Errors.First().ErrorMessage, ApiErrorCode.Validation); // Trả về kết quả lỗi kèm thông báo validate đầu tiên.

            var banner = await _bannerRepo.GetByIdWithTrackingAsync(id); // Lấy Banner theo Id kèm theo dõi trạng thái (Tracking) từ repository.
            if (banner == null) // Nếu không tìm thấy Banner.
                return ApiResult<BannerDto>.Fail("Không tìm thấy banner yêu cầu.", ApiErrorCode.NotFound); // Trả về kết quả thất bại với mã lỗi NotFound.

            banner.Title        = request.Title.Trim(); // Cập nhật tiêu đề Banner.
            banner.ImageUrl     = request.ImageUrl.Trim(); // Cập nhật đường dẫn ảnh Banner.
            banner.LinkUrl      = string.IsNullOrWhiteSpace(request.LinkUrl) ? null : request.LinkUrl.Trim(); // Cập nhật đường dẫn liên kết (null nếu trống).
            banner.SortOrder    = request.SortOrder; // Cập nhật thứ tự hiển thị.
            banner.IsActive     = request.IsActive; // Cập nhật trạng thái kích hoạt.
            banner.StartDate    = request.StartDate; // Cập nhật ngày bắt đầu hiển thị.
            banner.EndDate      = request.EndDate; // Cập nhật ngày kết thúc hiển thị.
            banner.ModifiedDate = DateTime.UtcNow; // Cập nhật ngày chỉnh sửa là thời gian UTC hiện tại.

            await _bannerRepo.SaveChangesAsync(); // Lưu các thay đổi đã cập nhật vào cơ sở dữ liệu.

            _logger.LogInformation("Cập nhật banner: {Title} (Id: {Id})", banner.Title, banner.Id); // Ghi log thông báo cập nhật thành công.

            return ApiResult<BannerDto>.Ok(MapToDto(banner), "Cập nhật banner thành công."); // Trả về kết quả thành công kèm DTO và thông báo.
        }

        public async Task<ApiResult<bool>> DeleteAsync(int id) // Định nghĩa phương thức xóa Banner.
        {
            var banner = await _bannerRepo.GetByIdWithTrackingAsync(id); // Lấy Banner cần xóa kèm theo dõi trạng thái (Tracking) từ repository.
            if (banner == null) // Nếu không tìm thấy Banner.
                return ApiResult<bool>.Fail("Không tìm thấy banner yêu cầu.", ApiErrorCode.NotFound); // Trả về kết quả thất bại với mã lỗi NotFound.

            banner.IsDeleted   = true; // Đánh dấu xóa mềm thực thể Banner.
            banner.DeletedDate = DateTime.UtcNow; // Ghi nhận thời gian xóa mềm Banner là thời gian UTC hiện tại.

            await _bannerRepo.SaveChangesAsync(); // Lưu các thay đổi xóa mềm vào cơ sở dữ liệu.

            _logger.LogInformation("Xóa mềm banner: {Title} (Id: {Id})", banner.Title, banner.Id); // Ghi log thông báo xóa mềm Banner thành công.

            return ApiResult<bool>.Ok(true, "Xóa banner thành công."); // Trả về kết quả thành công kiểu bool kèm thông báo.
        }

        private static BannerDto MapToDto(Banner entity) => new() // Hàm phụ trợ chuyển đổi thực thể Banner sang BannerDto.
        {
            Id          = entity.Id, // Ánh xạ Id.
            Title       = entity.Title, // Ánh xạ tiêu đề.
            ImageUrl    = entity.ImageUrl, // Ánh xạ đường dẫn ảnh.
            LinkUrl     = entity.LinkUrl, // Ánh xạ liên kết.
            SortOrder   = entity.SortOrder, // Ánh xạ thứ tự sắp xếp.
            IsActive    = entity.IsActive, // Ánh xạ trạng thái kích hoạt.
            StartDate   = entity.StartDate, // Ánh xạ ngày bắt đầu hiển thị.
            EndDate     = entity.EndDate, // Ánh xạ ngày kết thúc hiển thị.
            CreatedDate = entity.CreatedDate // Ánh xạ ngày tạo.
        };
    }
}

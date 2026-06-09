using Microsoft.Extensions.Logging; // Sử dụng thư viện ghi log hệ thống.
using PBL3.Core.Entities; // Sử dụng các thực thể Manufacturer.
using PBL3.Core.Interfaces; // Sử dụng các giao diện repository.
using PBL3.Shared.DTOs.Common; // Sử dụng DTO dùng chung ApiResult.
using PBL3.Shared.DTOs.Manufacturers; // Sử dụng các DTO của module hãng sản xuất.

namespace PBL3.Application.Manufacturers // Khai báo namespace cho tầng Application của module hãng sản xuất.
{
    public class ManufacturerService( // Định nghĩa lớp ManufacturerService sử dụng Primary Constructor.
        IManufacturerRepository manufacturerRepo, // Tiêm repository hãng sản xuất.
        ILogger<ManufacturerService> logger) : IManufacturerService // Tiêm logger hệ thống và triển khai giao diện IManufacturerService.
    {
        private readonly IManufacturerRepository _manufacturerRepo = // Gán repository vào trường thành viên.
            manufacturerRepo ?? throw new ArgumentNullException(nameof(manufacturerRepo)); // Kiểm tra null cho manufacturerRepo.
        private readonly ILogger<ManufacturerService> _logger = // Gán logger vào trường thành viên.
            logger ?? throw new ArgumentNullException(nameof(logger)); // Kiểm tra null cho logger.

        public async Task<ApiResult<PagedResult<ManufacturerDto>>> GetPagedListAsync(ManufacturerFilterRequest filter) // Định nghĩa phương thức lấy danh sách hãng sản xuất phân trang bất đồng bộ.
        {
            var (items, totalCount) = await _manufacturerRepo.GetPagedListAsync( // Gọi repository lấy danh sách hãng sản xuất phân trang và tổng số lượng.
                filter.Keyword, // Tìm kiếm theo từ khóa.
                filter.PageNumber, // Trang hiện tại.
                filter.PageSize, // Số lượng bản ghi trên một trang.
                filter.SortBy, // Cột cần sắp xếp.
                filter.SortDescending); // Sắp xếp giảm dần hay không.

            var dtos = items.Select(MapToDto).ToList(); // Ánh xạ danh sách thực thể hãng sản xuất sang danh sách DTO.

            var result = new PagedResult<ManufacturerDto> // Khởi tạo kết quả phân trang DTO.
            {
                Items      = dtos, // Gán danh sách DTO.
                TotalCount = totalCount, // Gán tổng số lượng bản ghi.
                PageNumber = filter.PageNumber, // Gán số trang hiện tại.
                PageSize   = filter.PageSize // Gán số lượng phần tử trên mỗi trang.
            };

            return ApiResult<PagedResult<ManufacturerDto>>.Ok(result); // Trả về kết quả phân trang thành công.
        }

        public async Task<ApiResult<List<ManufacturerSummaryDto>>> GetAllForDropdownAsync() // Định nghĩa phương thức lấy danh sách hãng sản xuất rút gọn cho Dropdown.
        {
            var items = await _manufacturerRepo.GetAllActiveAsync(); // Lấy tất cả hãng sản xuất đang hoạt động (chưa bị xóa) từ repository.

            var dtos = items.Select(m => new ManufacturerSummaryDto // Ánh xạ danh sách thực thể sang DTO rút gọn.
            {
                Id      = m.Id, // Ánh xạ Id.
                Name    = m.Name, // Ánh xạ tên hãng sản xuất.
                LogoUrl = m.LogoUrl // Ánh xạ URL ảnh logo của hãng.
            }).ToList(); // Chuyển đổi kết quả sang danh sách List.

            return ApiResult<List<ManufacturerSummaryDto>>.Ok(dtos); // Trả về danh sách DTO rút gọn thành công.
        }

        public async Task<ApiResult<ManufacturerDto>> GetByIdAsync(int id) // Định nghĩa phương thức lấy chi tiết hãng sản xuất theo Id.
        {
            var manufacturer = await _manufacturerRepo.GetByIdAsync(id); // Lấy thực thể hãng sản xuất theo Id từ repository.

            if (manufacturer == null) // Nếu không tìm thấy hãng sản xuất phù hợp với Id.
                return ApiResult<ManufacturerDto>.Fail("Không tìm thấy hãng sản xuất yêu cầu.", ApiErrorCode.NotFound); // Trả về kết quả thất bại lỗi NotFound.

            return ApiResult<ManufacturerDto>.Ok(MapToDto(manufacturer)); // Ánh xạ sang DTO và trả về kết quả thành công.
        }

        public async Task<ApiResult<ManufacturerDto>> CreateAsync(CreateManufacturerRequest request) // Định nghĩa phương thức tạo mới hãng sản xuất.
        {
            if (await _manufacturerRepo.IsDuplicateNameAsync(request.Name)) // Kiểm tra tên hãng sản xuất mới có bị trùng lặp trong hệ thống không.
                return ApiResult<ManufacturerDto>.Fail($"Hãng sản xuất với tên \"{request.Name}\" đã tồn tại trong hệ thống.", ApiErrorCode.Conflict); // Báo lỗi Conflict nếu bị trùng tên.

            var manufacturer = new Manufacturer // Khởi tạo thực thể hãng sản xuất mới.
            {
                Name         = request.Name.Trim(), // Gán tên hãng sản xuất (cắt bỏ khoảng trắng).
                LogoUrl      = request.LogoUrl?.Trim(), // Gán đường dẫn logo.
                Website      = request.Website?.Trim(), // Gán địa chỉ website hãng.
                SupportEmail = request.SupportEmail?.Trim(), // Gán email hỗ trợ của hãng.
                CreatedDate  = DateTime.UtcNow, // Gán thời điểm tạo là UTC hiện tại.
                IsDeleted    = false // Đánh dấu chưa bị xóa.
            };

            await _manufacturerRepo.AddAsync(manufacturer); // Thêm hãng sản xuất mới vào DB Context.
            await _manufacturerRepo.SaveChangesAsync(); // Lưu thay đổi xuống cơ sở dữ liệu.

            _logger.LogInformation("Tạo hãng sản xuất mới: {ManufacturerName} (Id: {ManufacturerId})", // Ghi log thông tin tạo mới thành công.
                manufacturer.Name, manufacturer.Id); // Ghi nhận tên và mã của hãng sản xuất vừa tạo.

            return ApiResult<ManufacturerDto>.Ok(MapToDto(manufacturer), "Tạo hãng sản xuất thành công."); // Trả về DTO kết quả tạo thành công.
        }

        public async Task<ApiResult<ManufacturerDto>> UpdateAsync(int id, UpdateManufacturerRequest request) // Định nghĩa phương thức cập nhật hãng sản xuất.
        {
            var manufacturer = await _manufacturerRepo.GetByIdAsync(id); // Lấy thực thể hãng sản xuất hiện tại theo Id từ repository.

            if (manufacturer == null) // Nếu không tìm thấy hãng sản xuất cần cập nhật.
                return ApiResult<ManufacturerDto>.Fail("Không tìm thấy hãng sản xuất yêu cầu.", ApiErrorCode.NotFound); // Trả về thông báo lỗi NotFound.

            if (await _manufacturerRepo.IsDuplicateNameAsync(request.Name, excludeId: id)) // Kiểm tra xem tên hãng cập nhật mới có trùng với tên hãng khác đã tồn tại không.
                return ApiResult<ManufacturerDto>.Fail($"Hãng sản xuất với tên \"{request.Name}\" đã tồn tại trong hệ thống.", ApiErrorCode.Conflict); // Báo lỗi Conflict nếu trùng tên.

            manufacturer.Name         = request.Name.Trim(); // Cập nhật tên hãng sản xuất.
            manufacturer.LogoUrl      = request.LogoUrl?.Trim(); // Cập nhật đường dẫn logo.
            manufacturer.Website      = request.Website?.Trim(); // Cập nhật địa chỉ website.
            manufacturer.SupportEmail = request.SupportEmail?.Trim(); // Cập nhật email hỗ trợ.
            manufacturer.ModifiedDate = DateTime.UtcNow; // Ghi nhận thời điểm cập nhật mới nhất.

            await _manufacturerRepo.SaveChangesAsync(); // Lưu thay đổi cập nhật thực thể xuống cơ sở dữ liệu.

            _logger.LogInformation("Cập nhật hãng sản xuất: {ManufacturerName} (Id: {ManufacturerId})", // Ghi log thông báo cập nhật thành công.
                manufacturer.Name, manufacturer.Id); // Ghi nhận thông tin hãng sản xuất vừa được cập nhật.

            return ApiResult<ManufacturerDto>.Ok(MapToDto(manufacturer), "Cập nhật hãng sản xuất thành công."); // Trả về kết quả cập nhật thành công kèm DTO.
        }

        public async Task<ApiResult<bool>> DeleteAsync(int id) // Định nghĩa phương thức xóa mềm hãng sản xuất.
        {
            var manufacturer = await _manufacturerRepo.GetByIdAsync(id); // Lấy thực thể hãng sản xuất theo Id từ repository.

            if (manufacturer == null) // Nếu không tìm thấy hãng sản xuất cần xóa.
                return ApiResult<bool>.Fail("Không tìm thấy hãng sản xuất yêu cầu.", ApiErrorCode.NotFound); // Trả về kết quả lỗi NotFound.

            if (await _manufacturerRepo.HasProductsAsync(id)) // Kiểm tra xem có sản phẩm nào thuộc hãng sản xuất này không.
                return ApiResult<bool>.Fail( // Trả về thông báo lỗi ràng buộc dữ liệu nếu còn sản phẩm.
                    "Không thể xóa hãng sản xuất này vì vẫn còn sản phẩm đang thuộc hãng. " + // Chi tiết thông báo lỗi.
                    "Vui lòng xóa hoặc chuyển hãng cho tất cả sản phẩm trước."); // Hướng dẫn xử lý lỗi.

            manufacturer.IsDeleted   = true; // Thiết lập trạng thái đã bị xóa mềm.
            manufacturer.DeletedDate = DateTime.UtcNow; // Ghi nhận thời gian thực hiện xóa mềm.

            await _manufacturerRepo.SaveChangesAsync(); // Lưu thay đổi xóa mềm xuống cơ sở dữ liệu.

            _logger.LogInformation("Xóa mềm hãng sản xuất: {ManufacturerName} (Id: {ManufacturerId})", // Ghi log thông báo xóa mềm thành công.
                manufacturer.Name, manufacturer.Id); // Ghi nhận hãng sản xuất bị xóa.

            return ApiResult<bool>.Ok(true, "Xóa hãng sản xuất thành công."); // Trả về kết quả xóa thành công dạng Boolean.
        }

        private static ManufacturerDto MapToDto(Manufacturer entity) // Định nghĩa hàm hỗ trợ ánh xạ thực thể sang DTO.
        {
            return new ManufacturerDto // Khởi tạo DTO mới và gán giá trị từ thực thể.
            {
                Id           = entity.Id, // Ánh xạ trường Id.
                Name         = entity.Name, // Ánh xạ trường Name.
                LogoUrl      = entity.LogoUrl, // Ánh xạ trường LogoUrl.
                Website      = entity.Website, // Ánh xạ trường Website.
                SupportEmail = entity.SupportEmail, // Ánh xạ trường SupportEmail.
                CreatedDate  = entity.CreatedDate // Ánh xạ trường CreatedDate.
            };
        }
    }
}

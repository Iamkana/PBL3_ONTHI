using Microsoft.Extensions.Logging; // Sử dụng thư viện ghi log hệ thống.
using PBL3.Core.Entities; // Sử dụng thực thể Supplier.
using PBL3.Core.Interfaces; // Sử dụng các giao diện repository.
using PBL3.Shared.DTOs.Common; // Sử dụng DTO dùng chung ApiResult.
using PBL3.Shared.DTOs.Products; // Sử dụng DTO sản phẩm.
using PBL3.Shared.DTOs.Suppliers; // Sử dụng DTO nhà cung cấp.

namespace PBL3.Application.Suppliers // Khai báo namespace cho tầng Application của module nhà cung cấp.
{
    public class SupplierService( // Định nghĩa lớp SupplierService bằng Primary Constructor.
        ISupplierRepository supplierRepo, // Tiêm repository nhà cung cấp.
        ILogger<SupplierService> logger) : ISupplierService // Tiêm logger hệ thống và triển khai giao diện ISupplierService.
    {
        private readonly ISupplierRepository _supplierRepo = // Gán repository vào trường thành viên.
            supplierRepo ?? throw new ArgumentNullException(nameof(supplierRepo)); // Kiểm tra null cho supplierRepo.
        private readonly ILogger<SupplierService> _logger = // Gán logger vào trường thành viên.
            logger ?? throw new ArgumentNullException(nameof(logger)); // Kiểm tra null cho logger.

        public async Task<ApiResult<PagedResult<SupplierDto>>> GetPagedListAsync(SupplierFilterRequest filter) // Định nghĩa phương thức lấy danh sách nhà cung cấp phân trang bất đồng bộ.
        {
            var (items, totalCount) = await _supplierRepo.GetPagedListAsync( // Gọi repository lấy danh sách nhà cung cấp phân trang và tổng số lượng.
                filter.Keyword, // Lọc theo từ khóa tìm kiếm.
                filter.PageNumber, // Trang hiện tại.
                filter.PageSize, // Số lượng bản ghi trên một trang.
                filter.SortBy, // Cột sắp xếp.
                filter.SortDescending); // Sắp xếp giảm dần hay không.

            var dtos = items.Select(MapToDto).ToList(); // Ánh xạ danh sách thực thể nhà cung cấp sang danh sách DTO.

            var result = new PagedResult<SupplierDto> // Khởi tạo kết quả phân trang DTO.
            {
                Items = dtos, // Gán danh sách DTO.
                TotalCount = totalCount, // Gán tổng số bản ghi.
                PageNumber = filter.PageNumber, // Gán số trang hiện tại.
                PageSize = filter.PageSize // Gán số lượng phần tử trên trang.
            };

            return ApiResult<PagedResult<SupplierDto>>.Ok(result); // Trả về kết quả phân trang thành công.
        }

        public async Task<ApiResult<SupplierDto>> GetByIdAsync(int id) // Định nghĩa phương thức lấy thông tin nhà cung cấp theo Id.
        {
            var supplier = await _supplierRepo.GetByIdAsync(id); // Lấy thực thể nhà cung cấp theo Id từ repository.

            if (supplier == null) // Nếu không tìm thấy nhà cung cấp.
                return ApiResult<SupplierDto>.Fail("Không tìm thấy nhà cung cấp yêu cầu.", ApiErrorCode.NotFound); // Trả về thông báo lỗi NotFound.

            return ApiResult<SupplierDto>.Ok(MapToDto(supplier)); // Ánh xạ thực thể sang DTO và trả về kết quả thành công.
        }

        public async Task<ApiResult<SupplierDto>> CreateAsync(CreateSupplierRequest request) // Định nghĩa phương thức tạo mới nhà cung cấp.
        {
            var supplier = new Supplier // Khởi tạo thực thể nhà cung cấp mới.
            {
                Name = request.Name.Trim(), // Cắt khoảng trắng và gán tên nhà cung cấp.
                ContactPerson = request.ContactPerson?.Trim(), // Gán tên người liên hệ.
                PhoneNumber = request.PhoneNumber.Trim(), // Gán số điện thoại.
                Email = request.Email?.Trim(), // Gán email.
                Address = request.Address?.Trim(), // Gán địa chỉ.
                TaxCode = request.TaxCode?.Trim(), // Gán mã số thuế.
                CreatedDate = DateTime.UtcNow, // Gán thời gian tạo hiện tại theo chuẩn UTC.
                IsDeleted = false // Mặc định trạng thái chưa xóa.
            };

            await _supplierRepo.AddAsync(supplier); // Thêm nhà cung cấp mới vào DB Context.
            await _supplierRepo.SaveChangesAsync(); // Lưu các thay đổi xuống cơ sở dữ liệu.

            _logger.LogInformation("Tạo nhà cung cấp mới: {SupplierName} (Id: {SupplierId})", supplier.Name, supplier.Id); // Ghi log thông báo tạo mới nhà cung cấp thành công.

            return ApiResult<SupplierDto>.Ok(MapToDto(supplier), "Tạo nhà cung cấp thành công."); // Trả về DTO kết quả tạo thành công.
        }

        public async Task<ApiResult<SupplierDto>> UpdateAsync(int id, UpdateSupplierRequest request) // Định nghĩa phương thức cập nhật nhà cung cấp theo Id.
        {
            var supplier = await _supplierRepo.GetByIdAsync(id); // Lấy thực thể nhà cung cấp từ repository theo Id.

            if (supplier == null) // Nếu không tìm thấy nhà cung cấp phù hợp để cập nhật.
                return ApiResult<SupplierDto>.Fail("Không tìm thấy nhà cung cấp yêu cầu.", ApiErrorCode.NotFound); // Báo lỗi NotFound.

            supplier.Name = request.Name.Trim(); // Cập nhật tên nhà cung cấp.
            supplier.ContactPerson = request.ContactPerson?.Trim(); // Cập nhật người liên hệ.
            supplier.PhoneNumber = request.PhoneNumber.Trim(); // Cập nhật số điện thoại.
            supplier.Email = request.Email?.Trim(); // Cập nhật email.
            supplier.Address = request.Address?.Trim(); // Cập nhật địa chỉ.
            supplier.TaxCode = request.TaxCode?.Trim(); // Cập nhật mã số thuế.

            await _supplierRepo.SaveChangesAsync(); // Lưu thay đổi cập nhật xuống cơ sở dữ liệu.

            _logger.LogInformation("Cập nhật nhà cung cấp: {SupplierName} (Id: {SupplierId})", supplier.Name, supplier.Id); // Ghi log thông báo cập nhật thành công.

            return ApiResult<SupplierDto>.Ok(MapToDto(supplier), "Cập nhật nhà cung cấp thành công."); // Trả về kết quả cập nhật kèm DTO.
        }

        public async Task<ApiResult<bool>> DeleteAsync(int id) // Định nghĩa phương thức xóa mềm nhà cung cấp.
        {
            var supplier = await _supplierRepo.GetByIdAsync(id); // Lấy thực thể nhà cung cấp từ repository theo Id.

            if (supplier == null) // Nếu không tìm thấy nhà cung cấp.
                return ApiResult<bool>.Fail("Không tìm thấy nhà cung cấp yêu cầu.", ApiErrorCode.NotFound); // Báo lỗi NotFound.

            supplier.IsDeleted = true; // Thực hiện xóa mềm bằng cách đánh dấu IsDeleted = true.

            await _supplierRepo.SaveChangesAsync(); // Lưu thay đổi xuống cơ sở dữ liệu.

            _logger.LogInformation("Xóa mềm nhà cung cấp: {SupplierName} (Id: {SupplierId})", supplier.Name, supplier.Id); // Ghi log thông báo xóa mềm thành công.

            return ApiResult<bool>.Ok(true, "Xóa nhà cung cấp thành công."); // Trả về kết quả thành công dạng boolean.
        }

        private static SupplierDto MapToDto(Supplier entity) // Định nghĩa hàm hỗ trợ ánh xạ thực thể sang DTO.
        {
            return new SupplierDto // Khởi tạo DTO mới và gán giá trị tương ứng từ thực thể.
            {
                Id = entity.Id, // Ánh xạ Id.
                Name = entity.Name, // Ánh xạ tên nhà cung cấp.
                ContactPerson = entity.ContactPerson, // Ánh xạ người liên hệ.
                PhoneNumber = entity.PhoneNumber, // Ánh xạ số điện thoại.
                Email = entity.Email, // Ánh xạ email.
                Address = entity.Address, // Ánh xạ địa chỉ.
                TaxCode = entity.TaxCode, // Ánh xạ mã số thuế.
                CreatedDate = entity.CreatedDate // Ánh xạ thời điểm tạo bản ghi.
            };
        }
    }
}

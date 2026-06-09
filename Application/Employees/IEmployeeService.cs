using PBL3.Shared.DTOs.Common; // Sử dụng DTO dùng chung ApiResult và PagedResult.
using PBL3.Shared.DTOs.Employees; // Sử dụng các DTO quản lý nhân viên.
using System; // Sử dụng các kiểu dữ liệu cơ bản của hệ thống.
using System.Collections.Generic; // Sử dụng lớp danh sách generic.
using System.Threading.Tasks; // Sử dụng Task lập trình bất đồng bộ.

namespace PBL3.Application.Employees // Khai báo namespace cho tầng Application của module nhân viên.
{
    public interface IEmployeeService // Định nghĩa giao diện dịch vụ nhân viên IEmployeeService.
    {
        Task<ApiResult<PagedResult<EmployeeListDto>>> GetPagedListAsync(EmployeeFilterRequest filter); // Khai báo phương thức lấy danh sách nhân viên phân trang bất đồng bộ.
        Task<ApiResult<EmployeeListDto>> GetByIdAsync(Guid id); // Khai báo phương thức lấy chi tiết thông tin nhân viên theo Id.
        Task<ApiResult<EmployeeListDto>> CreateAsync(CreateEmployeeRequest request); // Khai báo phương thức tạo mới thông tin nhân viên.
        Task<ApiResult<EmployeeListDto>> UpdateAsync(Guid id, UpdateEmployeeRequest request); // Khai báo phương thức cập nhật thông tin nhân viên.
        Task<ApiResult<bool>> DeactivateAsync(Guid id, string? lockReason); // Khai báo phương thức khóa tài khoản nhân viên kèm lý do.
        Task<ApiResult<bool>> ReactivateAsync(Guid id); // Khai báo phương thức kích hoạt lại tài khoản nhân viên bị khóa.
        Task<ApiResult<List<EmployeeDto>>> GetTechniciansSimpleAsync(); // Khai báo phương thức lấy danh sách kỹ thuật viên (đơn giản) để phân công bảo hành.
    }
}

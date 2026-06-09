using PBL3.Shared.DTOs.Common; // Sử dụng DTO dùng chung ApiResult và PagedResult.
using PBL3.Shared.DTOs.Customers; // Sử dụng các DTO quản lý khách hàng.
using PBL3.Shared.DTOs.Products; // Sử dụng các DTO sản phẩm.
using System; // Sử dụng các kiểu dữ liệu cơ bản của hệ thống.
using System.Threading.Tasks; // Sử dụng Task lập trình bất đồng bộ.

namespace PBL3.Application.Customers // Khai báo namespace cho tầng Application của module khách hàng.
{
    public interface ICustomerService // Định nghĩa giao diện dịch vụ khách hàng ICustomerService.
    {
        Task<ApiResult<PagedResult<CustomerDto>>> GetPagedListAsync(CustomerFilterRequest filter); // Khai báo phương thức lấy danh sách khách hàng phân trang bất đồng bộ.
        Task<ApiResult<CustomerDetailDto>> GetByIdAsync(Guid id); // Khai báo phương thức lấy chi tiết thông tin khách hàng theo Id.
        Task<ApiResult<CustomerDto>> CreateAsync(CreateCustomerRequest request); // Khai báo phương thức tạo mới thông tin khách hàng.
        Task<ApiResult<CustomerDto>> UpdateAsync(Guid id, UpdateCustomerRequest request); // Khai báo phương thức cập nhật thông tin khách hàng.
        Task<ApiResult<bool>> DeactivateAsync(Guid id, string? lockReason); // Khai báo phương thức khóa tài khoản khách hàng tạm thời kèm lý do.
        Task<ApiResult<bool>> ReactivateAsync(Guid id); // Khai báo phương thức kích hoạt lại tài khoản khách hàng bị khóa.
    }
}

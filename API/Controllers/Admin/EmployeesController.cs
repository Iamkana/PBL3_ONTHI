using Microsoft.AspNetCore.Authorization; // Sử dụng để phân quyền truy cập (Authorize).
using Microsoft.AspNetCore.Mvc; // Sử dụng các thành phần MVC như Controller, Action, HttpGet, HttpPost...
using PBL3.Application.Employees; // Sử dụng tầng dịch vụ quản lý nhân viên IEmployeeService.
using PBL3.Shared.DTOs.Common; // Sử dụng các DTO dùng chung như ApiResult, PagedResult.
using PBL3.Shared.DTOs.Employees; // Sử dụng các DTO liên quan đến nhân viên.
using System; // Sử dụng thư viện hệ thống cơ bản.
using System.Collections.Generic; // Sử dụng cấu trúc danh sách như List.
using System.Threading.Tasks; // Sử dụng thư viện lập trình bất đồng bộ.
using PBL3.API.Extensions; // Sử dụng phương thức mở rộng ToActionResult để map kết quả trả về.

namespace PBL3.API.Controllers.Admin // Khai báo namespace cho Controllers thuộc khu vực quản trị (Admin).
{
    [Route("api/[controller]")] // Định nghĩa route truy cập: api/employees.
    [ApiController] // Khai báo đây là một API Controller có sẵn cơ chế tự động validate Model.
    [Authorize(Roles = "Admin")] // Yêu cầu xác thực tài khoản thuộc vai trò Admin mới được truy cập.
    public class EmployeesController : ControllerBase // Định nghĩa lớp EmployeesController kế thừa từ ControllerBase.
    {
        private readonly IEmployeeService _employeeService; // Khai báo service quản lý nhân viên.

        public EmployeesController(IEmployeeService employeeService) // Constructor injection tiêm IEmployeeService từ DI container.
        {
            _employeeService = employeeService; // Gán service được tiêm vào trường nội bộ.
        }

        [HttpGet] // Định nghĩa HTTP GET Method cho endpoint api/employees.
        [ProducesResponseType(typeof(ApiResult<PagedResult<EmployeeListDto>>), StatusCodes.Status200OK)] // Định nghĩa kiểu dữ liệu trả về khi thành công.
        public async Task<IActionResult> GetList( // Lấy danh sách nhân viên phân trang kèm bộ lọc từ Query String.
            [FromQuery] string? keyword, // Từ khóa tìm kiếm (họ tên, email, sđt).
            [FromQuery] bool? isActive, // Bộ lọc trạng thái hoạt động.
            [FromQuery] byte? gender, // Bộ lọc giới tính.
            [FromQuery] int pageNumber = 1, // Thứ tự trang (mặc định trang 1).
            [FromQuery] int pageSize = 10, // Kích thước trang (mặc định 10 bản ghi).
            [FromQuery] string? sortBy = null, // Trường cần sắp xếp.
            [FromQuery] bool sortDescending = true) // Hướng sắp xếp (mặc định giảm dần).
        {
            var filter = new EmployeeFilterRequest // Khởi tạo request lọc nhân viên.
            {
                Keyword = keyword, // Gán từ khóa.
                IsActive = isActive, // Gán trạng thái hoạt động.
                Gender = gender, // Gán giới tính.
                PageNumber = pageNumber, // Gán số trang.
                PageSize = pageSize, // Gán kích thước trang.
                SortBy = sortBy, // Gán trường sắp xếp.
                SortDescending = sortDescending // Gán cờ sắp xếp giảm dần.
            };

            var result = await _employeeService.GetPagedListAsync(filter); // Gọi service lấy danh sách nhân viên phân trang.
            return Ok(result); // Trả về HTTP 200 OK kèm kết quả phân trang.
        }

        [HttpPost] // Định nghĩa HTTP POST Method tạo nhân viên mới (api/employees).
        [ProducesResponseType(typeof(ApiResult<EmployeeListDto>), StatusCodes.Status201Created)] // Tạo thành công trả về HTTP 201.
        public async Task<IActionResult> Create([FromBody] CreateEmployeeRequest request) // Nhận thông tin nhân viên từ Request Body.
        {
            var result = await _employeeService.CreateAsync(request); // Gọi service tạo nhân viên mới.
            if (!result.Success) return result.ToActionResult(this); // Nếu tạo thất bại thì map lỗi tương ứng trả về.

            return CreatedAtAction(nameof(GetList), result); // Trả về HTTP 201 Created kèm dữ liệu nhân viên vừa tạo.
        }

        [HttpPut("{id:guid}")] // Định nghĩa HTTP PUT Method cập nhật nhân viên theo Id (api/employees/{id}).
        [ProducesResponseType(typeof(ApiResult<EmployeeListDto>), StatusCodes.Status200OK)] // Cập nhật thành công.
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateEmployeeRequest request) // Cập nhật thông tin nhân viên theo Id.
        {
            var result = await _employeeService.UpdateAsync(id, request); // Gọi service cập nhật thông tin.
            return result.ToActionResult(this); // Trả về kết quả qua extension ToActionResult.
        }

        [HttpDelete("{id:guid}")] // Định nghĩa HTTP DELETE Method khóa tài khoản theo Id (api/employees/{id}).
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status200OK)] // Khóa thành công.
        public async Task<IActionResult> Deactivate(Guid id, [FromQuery] string? lockReason = null) // Khóa tài khoản nhân viên kèm lý do khóa.
        {
            var result = await _employeeService.DeactivateAsync(id, lockReason); // Gọi service khóa tài khoản nhân viên.
            return result.ToActionResult(this); // Trả về kết quả qua extension ToActionResult.
        }

        [HttpPut("{id:guid}/activate")] // Định nghĩa HTTP PUT Method mở khóa (api/employees/{id}/activate).
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status200OK)] // Kích hoạt thành công.
        public async Task<IActionResult> Reactivate(Guid id) // Kích hoạt lại tài khoản nhân viên theo Id.
        {
            var result = await _employeeService.ReactivateAsync(id); // Gọi service kích hoạt lại tài khoản nhân viên.
            return result.ToActionResult(this); // Trả về kết quả qua extension ToActionResult.
        }

        [HttpGet("{id:guid}")] // Định nghĩa HTTP GET Method lấy chi tiết nhân viên theo Id (api/employees/{id}).
        [ProducesResponseType(typeof(ApiResult<EmployeeListDto>), StatusCodes.Status200OK)] // Thành công.
        public async Task<IActionResult> GetById(Guid id) // Lấy thông tin chi tiết nhân viên theo Id.
        {
            var result = await _employeeService.GetByIdAsync(id); // Gọi service lấy chi tiết nhân viên.
            return result.ToActionResult(this); // Trả về kết quả qua extension ToActionResult.
        }

        [AllowAnonymous] // Cho phép tất cả mọi người truy cập công khai không cần đăng nhập.
        [HttpGet("technicians")] // Định nghĩa GET Method lấy danh sách kỹ thuật viên (api/employees/technicians).
        [ProducesResponseType(typeof(ApiResult<List<EmployeeDto>>), StatusCodes.Status200OK)] // Thành công.
        public async Task<IActionResult> GetTechnicians() // Lấy danh sách kỹ thuật viên rút gọn.
        {
            var result = await _employeeService.GetTechniciansSimpleAsync(); // Gọi service lấy danh sách kỹ thuật viên.
            return Ok(result); // Trả về HTTP 200 OK kèm danh sách.
        }
    }
}

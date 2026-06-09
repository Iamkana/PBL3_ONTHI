using Microsoft.AspNetCore.Authorization; // Sử dụng để phân quyền truy cập.
using Microsoft.AspNetCore.Mvc; // Sử dụng các thành phần MVC cho Web API.
using PBL3.Application.BuildPc; // Sử dụng tầng dịch vụ Build PC IBuildPcService.
using PBL3.Shared.DTOs.BuildPc; // Sử dụng các DTO liên quan đến chức năng Build PC.
using PBL3.Shared.DTOs.Common; // Sử dụng các DTO dùng chung như ApiResult.

namespace PBL3.API.Controllers.Admin // Khai báo namespace cho Controllers quản trị.
{
    [Route("api/build-pc")] // Định nghĩa route truy cập mặc định: api/build-pc.
    [ApiController] // Khai báo lớp là một API Controller có sẵn cơ chế tự động validate dữ liệu đầu vào.
    public class BuildPcController : ControllerBase // Định nghĩa lớp BuildPcController kế thừa từ ControllerBase.
    {
        private readonly IBuildPcService _buildPcService; // Khai báo trường dịch vụ Build PC.

        public BuildPcController(IBuildPcService buildPcService) // Constructor injection tiêm IBuildPcService.
        {
            _buildPcService = buildPcService; // Gán dịch vụ được tiêm.
        }

        [AllowAnonymous] // Cho phép tất cả người dùng truy cập công khai mà không cần đăng nhập.
        [HttpPost("export")] // Định nghĩa HTTP POST Method cho endpoint api/build-pc/export.
        public async Task<IActionResult> ExportToExcel([FromBody] ExportBuildPcRequest request) // Xuất cấu hình PC ra file Excel.
        {
            if (request.Items == null || !request.Items.Any()) // Kiểm tra nếu danh sách linh kiện gửi lên bị rỗng.
                return BadRequest(ApiResult<object>.Fail("Chưa có linh kiện nào trong cấu hình.")); // Trả về lỗi HTTP 400 BadRequest.

            var bytes = await _buildPcService.ExportToExcelAsync(request); // Gọi service để sinh tệp Excel dưới dạng mảng byte.
            return File(bytes, // Trả về phản hồi dạng File cho Client tải xuống.
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", // MIME Type của file Excel .xlsx.
                "cau-hinh-pc.xlsx"); // Tên mặc định của tệp khi tải về.
        }
    }
}

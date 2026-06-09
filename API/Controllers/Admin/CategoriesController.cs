using Microsoft.AspNetCore.Authorization; // Sử dụng để cấu hình phân quyền truy cập.
using Microsoft.AspNetCore.Mvc; // Sử dụng các lớp cơ bản của ASP.NET Core MVC (ControllerBase, HttpGet...).
using PBL3.Application.Categories; // Tham chiếu tầng dịch vụ ICategoryService.
using PBL3.Shared.DTOs.Categories; // Sử dụng các DTO liên quan đến danh mục sản phẩm.
using PBL3.Shared.DTOs.Common; // Sử dụng DTO dùng chung như ApiResult.
using PBL3.API.Extensions; // Sử dụng phương thức mở rộng để map kết quả trả về.

namespace PBL3.API.Controllers.Admin // Khai báo namespace cho các Controller thuộc khu vực quản trị.
{
    [ApiController] // Đánh dấu lớp là Web API Controller, kích hoạt tính năng tự động kiểm tra model state.
    [Route("api/[controller]")] // Định nghĩa route truy cập mặc định: api/categories.
    [Produces("application/json")] // Quy định kiểu dữ liệu trả về mặc định là định dạng JSON.
    [Authorize(Roles = "Admin, Employee")] // Yêu cầu xác thực tài khoản thuộc vai trò Admin hoặc Employee mới được truy cập các API (ngoại trừ được ghi đè).
    public class CategoriesController : ControllerBase // Định nghĩa lớp CategoriesController kế thừa từ ControllerBase.
    {
        private readonly ICategoryService _categoryService; // Khai báo trường lưu trữ dịch vụ danh mục sản phẩm.

        public CategoriesController(ICategoryService categoryService) // Constructor tiêm ICategoryService từ DI Container.
        {
            _categoryService = categoryService; // Gán dịch vụ được tiêm vào biến thành viên.
        }

        /// <summary>
        /// Lấy toàn bộ cây danh mục (Recursive Tree).
        /// </summary>
        [AllowAnonymous] // Cho phép tất cả người dùng (kể cả khách vãng lai) truy cập công khai API này.
        [HttpGet("tree")] // Định nghĩa HTTP GET Method cho route api/categories/tree.
        [ProducesResponseType(typeof(ApiResult<List<CategoryTreeDto>>), StatusCodes.Status200OK)] // Khai báo kết quả trả về thành công 200 OK kèm dữ liệu cây danh mục.
        public async Task<IActionResult> GetTree() // Lấy cây danh mục sản phẩm bất đồng bộ.
        {
            var result = await _categoryService.GetTreeAsync(); // Gọi service để xử lý lấy cây danh mục dạng đệ quy.
            return Ok(result); // Trả về kết quả HTTP 200 OK.
        }

        /// <summary>
        /// Lấy chi tiết 1 danh mục theo Id.
        /// </summary>
        [AllowAnonymous] // Cho phép truy cập công khai không cần đăng nhập.
        [HttpGet("{id:int}")] // Định nghĩa HTTP GET Method với tham số id kiểu int (api/categories/{id}).
        [ProducesResponseType(typeof(ApiResult<CategoryDto>), StatusCodes.Status200OK)] // Trả về chi tiết danh mục khi tìm thấy.
        [ProducesResponseType(typeof(ApiResult<CategoryDto>), StatusCodes.Status404NotFound)] // Trả về lỗi 404 khi không tìm thấy.
        public async Task<IActionResult> GetById(int id) // Lấy thông tin chi tiết danh mục theo Id.
        {
            var result = await _categoryService.GetByIdAsync(id); // Gọi service lấy danh mục theo Id.
            return result.ToActionResult(this); // Map ApiResult sang IActionResult tương ứng và trả về.
        }

        /// <summary>
        /// Tạo mới danh mục.
        /// </summary>
        [HttpPost] // Định nghĩa HTTP POST Method để thêm mới danh mục (api/categories).
        [ProducesResponseType(typeof(ApiResult<CategoryDto>), StatusCodes.Status201Created)] // Trả về HTTP 201 Created khi tạo thành công.
        [ProducesResponseType(typeof(ApiResult<CategoryDto>), StatusCodes.Status400BadRequest)] // Trả về HTTP 400 BadRequest khi dữ liệu không hợp lệ.
        public async Task<IActionResult> Create([FromBody] CreateCategoryRequest request) // Thêm danh mục mới nhận thông tin từ Body.
        {
            var result = await _categoryService.CreateAsync(request); // Gọi service thực hiện lưu danh mục vào DB.

            if (!result.Success) return result.ToActionResult(this); // Nếu lưu thất bại thì trả về kết quả lỗi tương ứng.

            return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result); // Trả về HTTP 201 Created kèm route lấy chi tiết danh mục vừa tạo.
        }

        /// <summary>
        /// Cập nhật danh mục (bao gồm kiểm tra tham chiếu vòng).
        /// </summary>
        [HttpPut("{id:int}")] // Định nghĩa HTTP PUT Method cập nhật danh mục theo Id (api/categories/{id}).
        [ProducesResponseType(typeof(ApiResult<CategoryDto>), StatusCodes.Status200OK)] // Thành công.
        [ProducesResponseType(typeof(ApiResult<CategoryDto>), StatusCodes.Status400BadRequest)] // Dữ liệu đầu vào hoặc logic nghiệp vụ lỗi.
        [ProducesResponseType(typeof(ApiResult<CategoryDto>), StatusCodes.Status404NotFound)] // Không tìm thấy.
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCategoryRequest request) // Cập nhật danh mục theo Id.
        {
            var result = await _categoryService.UpdateAsync(id, request); // Gọi service cập nhật danh mục (bao gồm chống vòng lặp vô tận ParentId).
            return result.ToActionResult(this); // Trả về kết quả tương ứng.
        }

        /// <summary>
        /// Xóa mềm danh mục (Soft Delete).
        /// </summary>
        [HttpDelete("{id:int}")] // Định nghĩa HTTP DELETE Method xóa mềm danh mục theo Id (api/categories/{id}).
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status200OK)] // Xóa thành công.
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status400BadRequest)] // Không được xóa do có ràng buộc sản phẩm/danh mục con.
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status404NotFound)] // Không tìm thấy.
        public async Task<IActionResult> Delete(int id) // Xóa danh mục theo Id.
        {
            var result = await _categoryService.DeleteAsync(id); // Gọi service thực hiện đánh dấu xóa mềm danh mục.
            return result.ToActionResult(this); // Trả về kết quả tương ứng.
        }
    }
}

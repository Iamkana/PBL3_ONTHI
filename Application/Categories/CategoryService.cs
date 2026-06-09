using Microsoft.Extensions.Logging; // Sử dụng thư viện logging hệ thống.
using PBL3.Core.Entities; // Sử dụng các thực thể Category của Core.
using PBL3.Core.Interfaces; // Sử dụng các interface ICategoryRepository.
using PBL3.Shared.DTOs.Categories; // Sử dụng các DTO của module danh mục.
using PBL3.Shared.DTOs.Common; // Sử dụng DTO dùng chung ApiResult.

namespace PBL3.Application.Categories // Khai báo namespace cho lớp dịch vụ CategoryService thuộc tầng Application.
{
    public class CategoryService( // Định nghĩa lớp CategoryService sử dụng Primary Constructor.
        ICategoryRepository categoryRepo, // Tiêm ICategoryRepository.
        ILogger<CategoryService> logger) : ICategoryService // Tiêm ILogger và kế thừa giao diện ICategoryService.
    {
        private readonly ICategoryRepository _categoryRepo = // Gán repository vào trường thành viên.
            categoryRepo ?? throw new ArgumentNullException(nameof(categoryRepo)); // Kiểm tra null cho repository.
        private readonly ILogger<CategoryService> _logger = // Gán logger vào trường thành viên.
            logger ?? throw new ArgumentNullException(nameof(logger)); // Kiểm tra null cho logger.

        public async Task<ApiResult<List<CategoryTreeDto>>> GetTreeAsync() // Phương thức lấy cây danh mục đệ quy bất đồng bộ.
        {
            var allCategories = await _categoryRepo.GetAllActiveAsync(); // Lấy tất cả các danh mục đang hoạt động từ DB.
            var treeDtos = BuildTree(allCategories, parentId: null); // Gọi hàm xây dựng cây danh mục bắt đầu từ danh mục gốc (ParentId = null).
            return ApiResult<List<CategoryTreeDto>>.Ok(treeDtos); // Trả về kết quả thành công chứa danh sách cây danh mục.
        }

        public async Task<ApiResult<CategoryDto>> GetByIdAsync(int id) // Phương thức lấy chi tiết danh mục theo Id.
        {
            var category = await _categoryRepo.GetByIdAsync(id, includeParent: true); // Lấy danh mục theo Id từ DB bao gồm cả danh mục cha liên kết.

            if (category == null) // Nếu không tìm thấy danh mục.
                return ApiResult<CategoryDto>.Fail("Không tìm thấy danh mục yêu cầu.", ApiErrorCode.NotFound); // Trả về lỗi NotFound 404.

            var dto = MapToDto(category); // Ánh xạ thực thể Category sang CategoryDto.
            return ApiResult<CategoryDto>.Ok(dto); // Trả về kết quả thành công chứa DTO.
        }

        public async Task<ApiResult<CategoryDto>> CreateAsync(CreateCategoryRequest request) // Phương thức tạo mới danh mục.
        {
            if (await _categoryRepo.IsDuplicateNameAsync(request.ParentId, request.Name)) // Kiểm tra xem tên danh mục có bị trùng ở cùng một cấp danh mục cha hay không.
                return ApiResult<CategoryDto>.Fail("Tên danh mục đã tồn tại trong cấp này.", ApiErrorCode.Conflict); // Trả về lỗi trùng lặp Conflict.

            if (await _categoryRepo.IsDuplicateSlugAsync(request.Slug)) // Kiểm tra xem Slug danh mục đã tồn tại trên hệ thống chưa.
                return ApiResult<CategoryDto>.Fail("Slug đã tồn tại. Vui lòng chọn slug khác.", ApiErrorCode.Conflict); // Trả về lỗi trùng Slug.

            int level = 0; // Khởi tạo cấp độ danh mục mặc định là 0 (Cấp cao nhất).
            if (request.ParentId.HasValue) // Nếu có chỉ định danh mục cha.
            {
                var parent = await _categoryRepo.GetByIdAsync(request.ParentId.Value); // Lấy thông tin danh mục cha từ Database.

                if (parent == null) // Nếu danh mục cha không tồn tại thực tế.
                    return ApiResult<CategoryDto>.Fail("Danh mục cha không tồn tại."); // Báo lỗi danh mục cha không tồn tại.

                level = parent.Level + 1; // Cấp độ danh mục con sẽ bằng cấp độ cha cộng thêm 1.
            }

            var category = new Category // Khởi tạo thực thể Category mới.
            {
                Name = request.Name, // Gán tên danh mục.
                Slug = request.Slug, // Gán slug.
                ParentId = request.ParentId, // Gán mã danh mục cha.
                Level = level, // Gán cấp độ.
                ImageUrl = request.ImageUrl, // Gán ảnh đại diện.
                SortOrder = request.SortOrder, // Gán thứ tự hiển thị.
                IsVisible = request.IsVisible, // Gán trạng thái ẩn/hiển thị.
                CreatedDate = DateTime.UtcNow // Gán thời gian tạo.
            };

            await _categoryRepo.AddAsync(category); // Thêm thực thể danh mục mới vào DB context.
            await _categoryRepo.SaveChangesAsync(); // Lưu thay đổi vào DB.

            _logger.LogInformation("Tạo danh mục mới: {CategoryName} (Id: {CategoryId})", category.Name, category.Id); // Ghi log thông báo tạo danh mục thành công.

            var created = await _categoryRepo.GetByIdAsync(category.Id, includeParent: true); // Lấy lại danh mục vừa tạo kèm thông tin cha để phản hồi.
            var dto = MapToDto(created!); // Ánh xạ sang DTO.

            return ApiResult<CategoryDto>.Ok(dto, "Tạo danh mục thành công."); // Trả về kết quả thành công.
        }

        public async Task<ApiResult<CategoryDto>> UpdateAsync(int id, UpdateCategoryRequest request) // Phương thức cập nhật thông tin danh mục.
        {
            var category = await _categoryRepo.GetByIdAsync(id); // Lấy danh mục cần cập nhật từ DB.

            if (category == null) // Nếu không tìm thấy danh mục.
                return ApiResult<CategoryDto>.Fail("Không tìm thấy danh mục yêu cầu.", ApiErrorCode.NotFound); // Trả về lỗi NotFound.

            if (await _categoryRepo.IsDuplicateNameAsync(request.ParentId, request.Name, excludeId: id)) // Kiểm tra trùng tên ở cùng một cấp cha (bỏ quan chính nó).
                return ApiResult<CategoryDto>.Fail("Tên danh mục đã tồn tại trong cấp này.", ApiErrorCode.Conflict); // Trả về lỗi Conflict.

            if (await _categoryRepo.IsDuplicateSlugAsync(request.Slug, excludeId: id)) // Kiểm tra trùng Slug trên toàn hệ thống (bỏ quan chính nó).
                return ApiResult<CategoryDto>.Fail("Slug đã tồn tại. Vui lòng chọn slug khác.", ApiErrorCode.Conflict); // Trả về lỗi trùng Slug.

            if (request.ParentId.HasValue) // Nếu có chỉnh sửa danh mục cha mới.
            {
                if (request.ParentId.Value == id) // Nếu danh mục cha được chọn trùng khớp với chính Id của danh mục hiện tại.
                    return ApiResult<CategoryDto>.Fail("Lỗi tham chiếu vòng: Không thể chọn chính mình làm danh mục cha."); // Trả về lỗi tham chiếu vòng.

                var isCircular = await DetectCircularReferenceAsync(id, request.ParentId.Value); // Gọi hàm phát hiện tham chiếu vòng đệ quy trong cây danh mục.
                if (isCircular) // Nếu phát hiện tham chiếu vòng (chọn danh mục con của chính nó làm cha).
                    return ApiResult<CategoryDto>.Fail("Lỗi tham chiếu vòng: Không thể chọn cấp dưới làm cha của cấp trên."); // Báo lỗi tham chiếu vòng.
            }

            int newLevel = 0; // Khởi tạo cấp độ mới mặc định là 0.
            if (request.ParentId.HasValue) // Nếu có danh mục cha.
            {
                var parent = await _categoryRepo.GetByIdAsync(request.ParentId.Value); // Tìm thông tin danh mục cha mới.

                if (parent == null) // Nếu danh mục cha mới không tồn tại.
                    return ApiResult<CategoryDto>.Fail("Danh mục cha không tồn tại."); // Báo lỗi không tồn tại.

                newLevel = parent.Level + 1; // Cấp độ mới bằng cấp độ cha cộng thêm 1.
            }

            bool parentChanged = category.ParentId != request.ParentId; // Kiểm tra xem danh mục cha có bị thay đổi so với ban đầu hay không.

            category.Name = request.Name; // Cập nhật tên danh mục.
            category.Slug = request.Slug; // Cập nhật slug.
            category.ParentId = request.ParentId; // Cập nhật mã danh mục cha.
            category.Level = newLevel; // Cập nhật cấp độ danh mục.
            category.ImageUrl = request.ImageUrl; // Cập nhật ảnh đại diện.
            category.SortOrder = request.SortOrder; // Cập nhật thứ tự sắp xếp.
            category.IsVisible = request.IsVisible; // Cập nhật trạng thái ẩn/hiện.
            category.ModifiedDate = DateTime.UtcNow; // Gán thời gian chỉnh sửa hiện tại.

            if (parentChanged) // Nếu danh mục cha bị thay đổi.
            {
                await UpdateSubtreeLevelsAsync(category.Id, newLevel); // Tiến hành cập nhật lại Level một cách đệ quy cho toàn bộ các danh mục con cháu phía dưới.
            }

            await _categoryRepo.SaveChangesAsync(); // Lưu tất cả thay đổi vào cơ sở dữ liệu.

            _logger.LogInformation("Cập nhật danh mục: {CategoryName} (Id: {CategoryId})", category.Name, category.Id); // Ghi log cập nhật danh mục thành công.

            var updated = await _categoryRepo.GetByIdAsync(category.Id, includeParent: true); // Lấy lại danh mục sau cập nhật kèm thông tin danh mục cha.
            var dto = MapToDto(updated!); // Ánh xạ sang DTO.

            return ApiResult<CategoryDto>.Ok(dto, "Cập nhật danh mục thành công."); // Trả về kết quả thành công.
        }

        public async Task<ApiResult<bool>> DeleteAsync(int id) // Phương thức xóa mềm danh mục.
        {
            var category = await _categoryRepo.GetByIdAsync(id); // Lấy danh mục cần xóa theo Id từ DB.

            if (category == null) // Nếu không tìm thấy.
                return ApiResult<bool>.Fail("Không tìm thấy danh mục yêu cầu.", ApiErrorCode.NotFound); // Trả về lỗi NotFound.

            if (await _categoryRepo.HasActiveChildrenAsync(id)) // Kiểm tra xem danh mục hiện tại có chứa các danh mục con đang hoạt động hay không.
                return ApiResult<bool>.Fail("Không thể xóa danh mục đang có danh mục con hoặc sản phẩm."); // Báo lỗi không thể xóa nếu có con.

            if (await _categoryRepo.HasProductsAsync(id)) // Kiểm tra xem danh mục hiện tại có chứa các sản phẩm nào hay không.
                return ApiResult<bool>.Fail("Không thể xóa danh mục đang có danh mục con hoặc sản phẩm."); // Báo lỗi không thể xóa nếu có sản phẩm.

            category.IsDeleted = true; // Đánh dấu xóa mềm thực thể (IsDeleted = true).
            category.DeletedDate = DateTime.UtcNow; // Ghi nhận thời gian xóa mềm.

            await _categoryRepo.SaveChangesAsync(); // Lưu thay đổi vào DB.

            _logger.LogInformation("Xóa mềm danh mục: {CategoryName} (Id: {CategoryId})", category.Name, category.Id); // Ghi log xóa mềm thành công.

            return ApiResult<bool>.Ok(true, "Xóa danh mục thành công."); // Trả về kết quả thành công.
        }

        private async Task<bool> DetectCircularReferenceAsync(int categoryId, int newParentId) // Hàm phụ trợ phát hiện tham chiếu vòng đệ quy.
        {
            var allCategories = await _categoryRepo.GetAllCategoryParentMapAsync(); // Lấy bản đồ (Dictionary) ánh xạ giữa các CategoryId và ParentId của chúng từ DB.

            var currentId = newParentId; // Bắt đầu duyệt từ danh mục cha mới đề xuất.
            var visited = new HashSet<int>(); // Khởi tạo danh sách các nút đã đi qua để tránh vòng lặp vô hạn.

            while (allCategories.ContainsKey(currentId)) // Duyệt ngược lên trên cây danh mục theo liên kết cha.
            {
                if (currentId == categoryId) // Nếu gặp lại chính Id danh mục đang cần cập nhật.
                    return true; // Xác nhận có xảy ra tham chiếu vòng.

                if (!visited.Add(currentId)) // Nếu danh mục cha này đã được duyệt qua trước đó (vòng lặp vô hạn xảy ra ở nhánh khác).
                    return true; // Xác nhận có tham chiếu vòng.

                var parentId = allCategories[currentId]; // Lấy danh mục cha của danh mục hiện tại trong bản đồ.

                if (!parentId.HasValue) // Nếu không còn danh mục cha nào cao hơn (đã tới nút gốc).
                    return false; // Kết luận không xảy ra tham chiếu vòng.

                currentId = parentId.Value; // Tiếp tục đi lên nút cha tiếp theo.
            }

            return false; // Không xảy ra tham chiếu vòng.
        }

        private async Task UpdateSubtreeLevelsAsync(int parentId, int parentLevel) // Hàm phụ trợ cập nhật cấp độ (Level) đệ quy cho toàn bộ nhánh con cháu.
        {
            var children = await _categoryRepo.GetChildrenAsync(parentId); // Lấy toàn bộ danh mục con trực tiếp của danh mục cha hiện tại.

            foreach (var child in children) // Duyệt qua từng danh mục con.
            {
                child.Level = parentLevel + 1; // Cập nhật cấp độ mới cho con bằng cấp độ cha cộng thêm 1.
                await UpdateSubtreeLevelsAsync(child.Id, child.Level); // Tiếp tục gọi đệ quy để cập nhật các cấp độ con cháu sâu hơn.
            }
        }

        private List<CategoryTreeDto> BuildTree(List<Category> allCategories, int? parentId) // Hàm phụ trợ xây dựng cây danh mục DTO đệ quy từ danh sách phẳng.
        {
            return allCategories // Duyệt danh sách các danh mục.
                .Where(c => c.ParentId == parentId) // Lọc các danh mục có ParentId khớp với mã cha đang xét.
                .OrderBy(c => c.SortOrder) // Sắp xếp theo thứ tự hiển thị được cấu hình.
                .ThenBy(c => c.Name) // Sắp xếp phụ theo thứ tự bảng chữ cái tên danh mục.
                .Select(c => new CategoryTreeDto // Khởi tạo DTO nút cây danh mục.
                {
                    Id = c.Id, // Ánh xạ Id.
                    Name = c.Name, // Ánh xạ tên.
                    Slug = c.Slug, // Ánh xạ slug.
                    ParentId = c.ParentId, // Ánh xạ parent id.
                    Level = c.Level, // Ánh xạ level.
                    ImageUrl = c.ImageUrl, // Ánh xạ hình ảnh.
                    SortOrder = c.SortOrder, // Ánh xạ thứ tự sắp xếp.
                    IsVisible = c.IsVisible, // Ánh xạ trạng thái ẩn/hiện.
                    Children = BuildTree(allCategories, c.Id) // Gọi đệ quy xây dựng danh sách con của danh mục hiện tại.
                })
                .ToList(); // Chuyển đổi thành danh sách.
        }

        private static CategoryDto MapToDto(Category entity) // Hàm phụ trợ ánh xạ từ thực thể sang DTO phẳng.
        {
            return new CategoryDto // Khởi tạo DTO danh mục phẳng.
            {
                Id = entity.Id, // Ánh xạ Id.
                Name = entity.Name, // Ánh xạ tên danh mục.
                Slug = entity.Slug, // Ánh xạ slug.
                ParentId = entity.ParentId, // Ánh xạ mã cha.
                ParentName = entity.Parent?.Name, // Ánh xạ tên danh mục cha (nếu có).
                Level = entity.Level, // Ánh xạ cấp độ danh mục.
                ImageUrl = entity.ImageUrl, // Ánh xạ hình ảnh đại diện.
                SortOrder = entity.SortOrder, // Ánh xạ thứ tự sắp xếp.
                IsVisible = entity.IsVisible, // Ánh xạ trạng thái ẩn/hiện.
                CreatedDate = entity.CreatedDate // Ánh xạ ngày tạo lập.
            };
        }
    }
}

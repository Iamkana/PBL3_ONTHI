using PBL3.Core.Entities; // Nhập namespace chứa các thực thể nghiệp vụ cốt lõi của hệ thống.

namespace PBL3.Core.Interfaces // Định nghĩa namespace PBL3.Core.Interfaces để quản lý thực thể/giao diện.
{
    /// <summary>
    /// Repository interface cho Category.
    /// </summary>
    public interface ICategoryRepository // Định nghĩa giao diện (interface) ICategoryRepository quy định hợp đồng cho tầng dữ liệu.
    {
        Task<List<Category>> GetAllActiveAsync(); // Định nghĩa phương thức bất đồng bộ 'GetAllActiveAsync' không tham số trả về kiểu Task<List<Category>>.
        Task<Category?> GetByIdAsync(int id, bool includeParent = false); // Định nghĩa phương thức bất đồng bộ 'GetByIdAsync' với tham số (id, false) trả về kiểu Task<Category?>.
        Task<bool> IsDuplicateNameAsync(int? parentId, string name, int? excludeId = null); // Định nghĩa phương thức bất đồng bộ 'IsDuplicateNameAsync' với tham số (parentId, name, null) trả về kiểu Task<bool>.
        Task<bool> IsDuplicateSlugAsync(string slug, int? excludeId = null); // Định nghĩa phương thức bất đồng bộ 'IsDuplicateSlugAsync' với tham số (slug, null) trả về kiểu Task<bool>.
        Task<bool> HasActiveChildrenAsync(int id); // Định nghĩa phương thức bất đồng bộ 'HasActiveChildrenAsync' với tham số (id) trả về kiểu Task<bool>.
        Task<bool> HasProductsAsync(int id); // Định nghĩa phương thức bất đồng bộ 'HasProductsAsync' với tham số (id) trả về kiểu Task<bool>.
        Task<Dictionary<int, int?>> GetAllCategoryParentMapAsync(); // Định nghĩa phương thức bất đồng bộ 'GetAllCategoryParentMapAsync' không tham số trả về kiểu Task<Dictionary<int, int?>>.
        Task<List<Category>> GetChildrenAsync(int parentId); // Định nghĩa phương thức bất đồng bộ 'GetChildrenAsync' với tham số (parentId) trả về kiểu Task<List<Category>>.
        Task AddAsync(Category category); // Định nghĩa phương thức bất đồng bộ 'AddAsync' với tham số (category) trả về kiểu Task.
        Task SaveChangesAsync(); // Định nghĩa phương thức bất đồng bộ 'SaveChangesAsync' không tham số trả về kiểu Task.
    }
}

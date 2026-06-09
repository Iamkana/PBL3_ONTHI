using PBL3.Core.Entities; // Nhập namespace chứa các thực thể nghiệp vụ cốt lõi của hệ thống.

namespace PBL3.Core.Interfaces // Định nghĩa namespace PBL3.Core.Interfaces để quản lý thực thể/giao diện.
{
    /// <summary>
    /// Repository interface cho Product.
    /// </summary>
    public interface IProductRepository // Định nghĩa giao diện (interface) IProductRepository quy định hợp đồng cho tầng dữ liệu.
    {
        Task<Product?> GetByIdAsync(int id); // Định nghĩa phương thức bất đồng bộ 'GetByIdAsync' với tham số (id) trả về kiểu Task<Product?>.
        Task<Product?> GetByIdWithDetailsAsync(int id); // Định nghĩa phương thức bất đồng bộ 'GetByIdWithDetailsAsync' với tham số (id) trả về kiểu Task<Product?>.
        Task<bool> IsSkuExistsAsync(string sku, int? excludeVariantId = null); // Định nghĩa phương thức bất đồng bộ 'IsSkuExistsAsync' với tham số (sku, null) trả về kiểu Task<bool>.
        Task<bool> ManufacturerExistsAsync(int manufacturerId); // Định nghĩa phương thức bất đồng bộ 'ManufacturerExistsAsync' với tham số (manufacturerId) trả về kiểu Task<bool>.
        Task<bool> CategoryExistsAsync(int categoryId); // Định nghĩa phương thức bất đồng bộ 'CategoryExistsAsync' với tham số (categoryId) trả về kiểu Task<bool>.

        /// <summary>
        /// Lấy danh sách Id category con (đệ quy) từ một category cha.
        /// </summary>
        Task<List<int>> GetCategoryChildIdsAsync(int categoryId); // Định nghĩa phương thức bất đồng bộ 'GetCategoryChildIdsAsync' với tham số (categoryId) trả về kiểu Task<List<int>>.

        /// <summary>
        /// Lấy danh sách sản phẩm có phân trang và bộ lọc.
        /// Trả về (items, totalCount).
        /// </summary>
        Task<(List<Product> Items, int TotalCount)> GetPagedListAsync( // Khai báo thành phần cấu trúc nghiệp vụ.
            string? keyword, // Khai báo thành phần cấu trúc nghiệp vụ.
            List<int>? categoryIds, // Khai báo thành phần cấu trúc nghiệp vụ.
            int? manufacturerId, // Khai báo thành phần cấu trúc nghiệp vụ.
            decimal? priceMin, // Khai báo thành phần cấu trúc nghiệp vụ.
            decimal? priceMax, // Khai báo thành phần cấu trúc nghiệp vụ.
            int? status, // Khai báo thành phần cấu trúc nghiệp vụ.
            int pageNumber, // Khai báo thành phần cấu trúc nghiệp vụ.
            int pageSize, // Khai báo thành phần cấu trúc nghiệp vụ.
            string? sortBy, // Khai báo thành phần cấu trúc nghiệp vụ.
            bool sortDescending); // Khai báo thành phần cấu trúc nghiệp vụ.

        /// <summary>
        /// Lấy danh sách VariantId tồn tại (chưa bị xoá) từ danh sách Id.
        /// </summary>
        Task<List<int>> GetExistingVariantIdsAsync(List<int> variantIds); // Định nghĩa phương thức bất đồng bộ 'GetExistingVariantIdsAsync' với tham số (variantIds) trả về kiểu Task<List<int>>.

        /// <summary>
        /// Lấy danh sách CategoryId (distinct) của các sản phẩm chứa Variant trong danh sách.
        /// Dùng để kiểm tra điều kiện danh mục áp dụng của voucher trong checkout.
        /// </summary>
        Task<List<int>> GetCategoryIdsByVariantIdsAsync(List<int> variantIds); // Định nghĩa phương thức bất đồng bộ 'GetCategoryIdsByVariantIdsAsync' với tham số (variantIds) trả về kiểu Task<List<int>>.

        /// <summary>
        /// Lọc ProductVariant theo thông số kỹ thuật JSON.
        /// </summary>
        Task<List<ProductVariant>> FilterBySpecificationAsync(string specKey, string specValue); // Định nghĩa phương thức bất đồng bộ 'FilterBySpecificationAsync' với tham số (specKey, specValue) trả về kiểu Task<List<ProductVariant>>.

        /// <summary>
        /// Lấy Variant theo Id (có tracking để update).
        /// </summary>
        Task<ProductVariant?> GetVariantByIdAsync(int variantId); // Định nghĩa phương thức bất đồng bộ 'GetVariantByIdAsync' với tham số (variantId) trả về kiểu Task<ProductVariant?>.

        Task AddAsync(Product product); // Định nghĩa phương thức bất đồng bộ 'AddAsync' với tham số (product) trả về kiểu Task.
        Task AddVariantAsync(ProductVariant variant); // Định nghĩa phương thức bất đồng bộ 'AddVariantAsync' với tham số (variant) trả về kiểu Task.
        Task RemoveVariant(ProductVariant variant); // Định nghĩa phương thức bất đồng bộ 'RemoveVariant' với tham số (variant) trả về kiểu Task.
        Task ReplaceProductImagesAsync(int productId, List<ProductImage> newImages); // Định nghĩa phương thức bất đồng bộ 'ReplaceProductImagesAsync' với tham số (productId, newImages) trả về kiểu Task.
        Task SaveChangesAsync(); // Định nghĩa phương thức bất đồng bộ 'SaveChangesAsync' không tham số trả về kiểu Task.
    }
}

using Microsoft.EntityFrameworkCore; // Nhập thư viện Microsoft.EntityFrameworkCore để làm việc với CSDL qua ORM.
using PBL3.Core.Entities; // Nhập namespace chứa các thực thể nghiệp vụ cốt lưu trữ của hệ thống.
using PBL3.Core.Interfaces; // Nhập namespace chứa các giao diện (interface) dùng chung của hệ thống.
using PBL3.Infrastructure.Data; // Nhập namespace chứa DbContext và các cấu hình dữ liệu của Infrastructure.

namespace PBL3.Infrastructure.Repositories // Định nghĩa namespace PBL3.Infrastructure.Repositories quản lý cấu trúc code.
{
    public class ProductRepository(HushStoreDbContext context) : IProductRepository // Định nghĩa lớp ProductRepository sử dụng Primary Constructor nhận (HushStoreDbContext context) triển khai/kế thừa IProductRepository.
    {
        private readonly HushStoreDbContext _context = // Khai báo trường dữ liệu private readonly và bắt đầu gán giá trị.
            context ?? throw new ArgumentNullException(nameof(context)); // Kiểm tra null và ném ra ngoại lệ ArgumentNullException nếu đối tượng context bị null.

        public async Task<Product?> GetByIdAsync(int id) // Thực hiện xử lý bất đồng bộ phương thức 'GetByIdAsync' nhận tham số (id) trả về kiểu Task<Product?>.
        {
            return await _context.Products // Chờ và trả về giá trị của tác vụ bất đồng bộ '_context.Products'.
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted); // Lấy phần tử đầu tiên thỏa mãn điều kiện hoặc trả về null nếu không tìm thấy.
        }

        public async Task<Product?> GetByIdWithDetailsAsync(int id) // Thực hiện xử lý bất đồng bộ phương thức 'GetByIdWithDetailsAsync' nhận tham số (id) trả về kiểu Task<Product?>.
        {
            return await _context.Products // Chờ và trả về giá trị của tác vụ bất đồng bộ '_context.Products'.
                .Include(p => p.Manufacturer) // Nạp kèm (Eager load) dữ liệu quan hệ liên quan: .Include(p => p.Manufacturer).
                .Include(p => p.Category) // Nạp kèm (Eager load) dữ liệu quan hệ liên quan: .Include(p => p.Category).
                .Include(p => p.Variants.Where(v => !v.IsDeleted)) // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(v => !v.IsDeleted)).
                    .ThenInclude(v => v.Images) // Thực thi dòng lệnh nghiệp vụ.
                .AsNoTracking() // Tắt tính năng theo dõi trạng thái thực thể (AsNoTracking) để tối ưu hiệu năng đọc dữ liệu.
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted); // Lấy phần tử đầu tiên thỏa mãn điều kiện hoặc trả về null nếu không tìm thấy.
        }

        public async Task<bool> IsSkuExistsAsync(string sku, int? excludeVariantId = null) // Thực hiện xử lý bất đồng bộ phương thức 'IsSkuExistsAsync' nhận tham số (sku, null) trả về kiểu Task<bool>.
        {
            var query = _context.ProductVariants // Thực hiện gán giá trị của biểu thức '_context.ProductVariants' cho biến 'query'.
                .Where(v => v.SKU == sku && !v.IsDeleted); // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(v => v.SKU == sku && !v.IsDeleted);.

            if (excludeVariantId.HasValue) // Kiểm tra điều kiện: 'excludeVariantId.HasValue'.
                query = query.Where(v => v.Id != excludeVariantId.Value); // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(v => v.Id != excludeVariantId.Value);.

            return await query.AnyAsync(); // Kiểm tra xem có bất kỳ bản ghi nào khớp với điều kiện hay không.
        }

        public async Task<bool> ManufacturerExistsAsync(int manufacturerId) // Thực hiện xử lý bất đồng bộ phương thức 'ManufacturerExistsAsync' nhận tham số (manufacturerId) trả về kiểu Task<bool>.
        {
            return await _context.Manufacturers // Chờ và trả về giá trị của tác vụ bất đồng bộ '_context.Manufacturers'.
                .AnyAsync(m => m.Id == manufacturerId && !m.IsDeleted); // Kiểm tra xem có bất kỳ bản ghi nào khớp với điều kiện hay không.
        }

        public async Task<bool> CategoryExistsAsync(int categoryId) // Thực hiện xử lý bất đồng bộ phương thức 'CategoryExistsAsync' nhận tham số (categoryId) trả về kiểu Task<bool>.
        {
            return await _context.Categories // Chờ và trả về giá trị của tác vụ bất đồng bộ '_context.Categories'.
                .AnyAsync(c => c.Id == categoryId && !c.IsDeleted); // Kiểm tra xem có bất kỳ bản ghi nào khớp với điều kiện hay không.
        }

        public async Task<List<int>> GetCategoryChildIdsAsync(int categoryId) // Thực hiện xử lý bất đồng bộ phương thức 'GetCategoryChildIdsAsync' nhận tham số (categoryId) trả về kiểu Task<List<int>>.
        {
            // Lấy tất cả category con (đệ quy) để hỗ trợ filter
            var allCategories = await _context.Categories // Chờ tác vụ bất đồng bộ hoàn thành và gán kết quả cho biến 'allCategories'.
                .AsNoTracking() // Tắt tính năng theo dõi trạng thái thực thể (AsNoTracking) để tối ưu hiệu năng đọc dữ liệu.
                .Where(c => !c.IsDeleted) // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(c => !c.IsDeleted).
                .Select(c => new { c.Id, c.ParentId }) // Thực thi dòng lệnh nghiệp vụ.
                .ToListAsync(); // Truy vấn bất đồng bộ và chuyển đổi kết quả trả về dạng danh sách List.

            var result = new List<int> { categoryId }; // Thực hiện gán giá trị của biểu thức 'new List<int> { categoryId }' cho biến 'result'.
            var queue = new Queue<int>(); // Thực hiện gán giá trị của biểu thức 'new Queue<int>()' cho biến 'queue'.
            queue.Enqueue(categoryId); // Thực thi dòng lệnh nghiệp vụ.

            while (queue.Count > 0) // Thực thi dòng lệnh nghiệp vụ.
            {
                var currentId = queue.Dequeue(); // Thực hiện gán giá trị của biểu thức 'queue.Dequeue()' cho biến 'currentId'.
                var children = allCategories // Thực hiện gán giá trị của biểu thức 'allCategories' cho biến 'children'.
                    .Where(c => c.ParentId == currentId) // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(c => c.ParentId == currentId).
                    .Select(c => c.Id) // Thực thi dòng lệnh nghiệp vụ.
                    .ToList(); // Thực thi dòng lệnh nghiệp vụ.

                foreach (var childId in children) // Thực thi dòng lệnh nghiệp vụ.
                {
                    result.Add(childId); // Thêm thực thể mới vào CSDL thông qua DbSet tương ứng.
                    queue.Enqueue(childId); // Thực thi dòng lệnh nghiệp vụ.
                }
            }

            return result; // Trả về giá trị của biểu thức 'result'.
        }

        public async Task<(List<Product> Items, int TotalCount)> GetPagedListAsync( // Thực hiện xử lý bất đồng bộ phương thức 'Task<' nhận tham số (Items, TotalCount) trả về kiểu .
            string? keyword, // Thực thi dòng lệnh nghiệp vụ.
            List<int>? categoryIds, // Thực thi dòng lệnh nghiệp vụ.
            int? manufacturerId, // Thực thi dòng lệnh nghiệp vụ.
            decimal? priceMin, // Thực thi dòng lệnh nghiệp vụ.
            decimal? priceMax, // Thực thi dòng lệnh nghiệp vụ.
            int? status, // Thực thi dòng lệnh nghiệp vụ.
            int pageNumber, // Thực thi dòng lệnh nghiệp vụ.
            int pageSize, // Thực thi dòng lệnh nghiệp vụ.
            string? sortBy, // Thực thi dòng lệnh nghiệp vụ.
            bool sortDescending) // Thực thi dòng lệnh nghiệp vụ.
        {
            var query = _context.Products // Thực hiện gán giá trị của biểu thức '_context.Products' cho biến 'query'.
                .AsNoTracking() // Tắt tính năng theo dõi trạng thái thực thể (AsNoTracking) để tối ưu hiệu năng đọc dữ liệu.
                .Include(p => p.Manufacturer) // Nạp kèm (Eager load) dữ liệu quan hệ liên quan: .Include(p => p.Manufacturer).
                .Include(p => p.Category) // Nạp kèm (Eager load) dữ liệu quan hệ liên quan: .Include(p => p.Category).
                .Include(p => p.Variants.Where(v => !v.IsDeleted)) // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(v => !v.IsDeleted)).
                    .ThenInclude(v => v.Images) // Thực thi dòng lệnh nghiệp vụ.
                .Where(p => !p.IsDeleted); // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(p => !p.IsDeleted);.

            // Filter: Keyword (tìm theo tên sản phẩm)
            if (!string.IsNullOrWhiteSpace(keyword)) // Kiểm tra điều kiện: '!string.IsNullOrWhiteSpace(keyword'.
            {
                var kw = keyword.Trim().ToLower(); // Thực hiện gán giá trị của biểu thức 'keyword.Trim().ToLower()' cho biến 'kw'.
                query = query.Where(p => // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(p =>.
                    p.Name.ToLower().Contains(kw) || // Thực thi dòng lệnh nghiệp vụ.
                    (p.ShortDescription != null && p.ShortDescription.ToLower().Contains(kw)) || // Thực thi dòng lệnh nghiệp vụ.
                    p.Variants.Any(v => v.SKU.ToLower().Contains(kw))); // Thực thi dòng lệnh nghiệp vụ.
            }

            // Filter: Category (bao gồm category con)
            if (categoryIds != null && categoryIds.Count > 0) // Kiểm tra điều kiện: 'categoryIds != null && categoryIds.Count > 0'.
            {
                query = query.Where(p => categoryIds.Contains(p.CategoryId)); // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(p => categoryIds.Contains(p.CategoryId));.
            }

            // Filter: Manufacturer
            if (manufacturerId.HasValue) // Kiểm tra điều kiện: 'manufacturerId.HasValue'.
            {
                query = query.Where(p => p.ManufacturerId == manufacturerId.Value); // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(p => p.ManufacturerId == manufacturerId.Value);.
            }

            // Filter: Price range (dựa trên giá Variant)
            if (priceMin.HasValue) // Kiểm tra điều kiện: 'priceMin.HasValue'.
            {
                query = query.Where(p => p.Variants.Any(v => v.Price >= priceMin.Value)); // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(p => p.Variants.Any(v => v.Price >= priceMin.Value));.
            }
            if (priceMax.HasValue) // Kiểm tra điều kiện: 'priceMax.HasValue'.
            {
                query = query.Where(p => p.Variants.Any(v => v.Price <= priceMax.Value)); // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(p => p.Variants.Any(v => v.Price <= priceMax.Value));.
            }

            // Filter: Status
            if (status.HasValue) // Kiểm tra điều kiện: 'status.HasValue'.
            {
                query = query.Where(p => p.Status == status.Value); // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(p => p.Status == status.Value);.
            }

            // Total count (before paging)
            var totalCount = await query.CountAsync(); // Tính toán tổng số lượng bản ghi thỏa mãn điều kiện bất đồng bộ.

            // Sorting
            query = sortBy?.ToLower() switch // Thực hiện gán giá trị của biểu thức 'sortBy?.ToLower() switch' cho biến 'query'.
            {
                "name" => sortDescending // Thực thi dòng lệnh nghiệp vụ.
                    ? query.OrderByDescending(p => p.Name) // Sắp xếp danh sách kết quả theo thứ tự giảm dần.
                    : query.OrderBy(p => p.Name), // Sắp xếp danh sách kết quả theo thứ tự tăng dần.
                "price" => sortDescending // Thực thi dòng lệnh nghiệp vụ.
                    ? query.OrderByDescending(p => p.Variants.Min(v => v.Price)) // Sắp xếp danh sách kết quả theo thứ tự giảm dần.
                    : query.OrderBy(p => p.Variants.Min(v => v.Price)), // Sắp xếp danh sách kết quả theo thứ tự tăng dần.
                "created" => sortDescending // Thực thi dòng lệnh nghiệp vụ.
                    ? query.OrderByDescending(p => p.CreatedDate) // Sắp xếp danh sách kết quả theo thứ tự giảm dần.
                    : query.OrderBy(p => p.CreatedDate), // Sắp xếp danh sách kết quả theo thứ tự tăng dần.
                _ => query.OrderByDescending(p => p.CreatedDate) // Sắp xếp danh sách kết quả theo thứ tự giảm dần.
            };

            // Paging
            var items = await query // Chờ tác vụ bất đồng bộ hoàn thành và gán kết quả cho biến 'items'.
                .Skip((pageNumber - 1) * pageSize) // Thực thi dòng lệnh nghiệp vụ.
                .Take(pageSize) // Thực thi dòng lệnh nghiệp vụ.
                .ToListAsync(); // Truy vấn bất đồng bộ và chuyển đổi kết quả trả về dạng danh sách List.

            return (items, totalCount); // Trả về giá trị của biểu thức '(items, totalCount)'.
        }

        public async Task AddAsync(Product product) // Thực hiện xử lý bất đồng bộ phương thức 'AddAsync' nhận tham số (product) trả về kiểu Task.
        {
            await _context.Products.AddAsync(product); // Thêm thực thể mới vào CSDL thông qua DbSet tương ứng.
        }

        public async Task AddVariantAsync(ProductVariant variant) // Thực hiện xử lý bất đồng bộ phương thức 'AddVariantAsync' nhận tham số (variant) trả về kiểu Task.
        {
            await _context.ProductVariants.AddAsync(variant); // Thêm thực thể mới vào CSDL thông qua DbSet tương ứng.
        }

        public void RemoveVariant(ProductVariant variant) // Thực hiện xử lý phương thức 'RemoveVariant' nhận tham số (variant) trả về kiểu void.
        {
            _context.ProductVariants.Remove(variant); // Xóa thực thể khỏi CSDL thông qua DbSet tương ứng.
        }

        // Fix: interface khai báo Task RemoveVariant nhưng implementation là void
        // Sửa lại cho khớp
        Task IProductRepository.RemoveVariant(ProductVariant variant) // Thực hiện xử lý phương thức 'IProductRepository.RemoveVariant' nhận tham số (variant) trả về kiểu Task.
        {
            _context.ProductVariants.Remove(variant); // Xóa thực thể khỏi CSDL thông qua DbSet tương ứng.
            return Task.CompletedTask; // Trả về giá trị của biểu thức 'Task.CompletedTask'.
        }

        public async Task<List<int>> GetExistingVariantIdsAsync(List<int> variantIds) // Thực hiện xử lý bất đồng bộ phương thức 'GetExistingVariantIdsAsync' nhận tham số (variantIds) trả về kiểu Task<List<int>>.
        {
            return await _context.ProductVariants // Chờ và trả về giá trị của tác vụ bất đồng bộ '_context.ProductVariants'.
                .AsNoTracking() // Tắt tính năng theo dõi trạng thái thực thể (AsNoTracking) để tối ưu hiệu năng đọc dữ liệu.
                .Where(v => variantIds.Contains(v.Id) && !v.IsDeleted) // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(v => variantIds.Contains(v.Id) && !v.IsDeleted).
                .Select(v => v.Id) // Thực thi dòng lệnh nghiệp vụ.
                .ToListAsync(); // Truy vấn bất đồng bộ và chuyển đổi kết quả trả về dạng danh sách List.
        }

        public async Task<List<int>> GetCategoryIdsByVariantIdsAsync(List<int> variantIds) // Thực hiện xử lý bất đồng bộ phương thức 'GetCategoryIdsByVariantIdsAsync' nhận tham số (variantIds) trả về kiểu Task<List<int>>.
        {
            return await _context.ProductVariants // Chờ và trả về giá trị của tác vụ bất đồng bộ '_context.ProductVariants'.
                .AsNoTracking() // Tắt tính năng theo dõi trạng thái thực thể (AsNoTracking) để tối ưu hiệu năng đọc dữ liệu.
                .Where(v => variantIds.Contains(v.Id)) // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(v => variantIds.Contains(v.Id)).
                .Select(v => v.Product.CategoryId) // Thực thi dòng lệnh nghiệp vụ.
                .Distinct() // Thực thi dòng lệnh nghiệp vụ.
                .ToListAsync(); // Truy vấn bất đồng bộ và chuyển đổi kết quả trả về dạng danh sách List.
        }

        public async Task<List<ProductVariant>> FilterBySpecificationAsync(string specKey, string specValue) // Thực hiện xử lý bất đồng bộ phương thức 'FilterBySpecificationAsync' nhận tham số (specKey, specValue) trả về kiểu Task<List<ProductVariant>>.
        {
            string jsonPath = $"$.{specKey}"; // Thực hiện gán giá trị của biểu thức '$"$.{specKey}"' cho biến 'string jsonPath'.
            return await _context.ProductVariants // Chờ và trả về giá trị của tác vụ bất đồng bộ '_context.ProductVariants'.
                .FromSqlInterpolated($"SELECT * FROM ProductVariants WHERE JSON_VALUE(Specifications, {jsonPath}) = {specValue}") // Thực thi dòng lệnh nghiệp vụ.
                .AsNoTracking() // Tắt tính năng theo dõi trạng thái thực thể (AsNoTracking) để tối ưu hiệu năng đọc dữ liệu.
                .Where(v => !v.IsDeleted) // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(v => !v.IsDeleted).
                .Include(v => v.Product) // Nạp kèm (Eager load) dữ liệu quan hệ liên quan: .Include(v => v.Product).
                .Include(v => v.Images) // Nạp kèm (Eager load) dữ liệu quan hệ liên quan: .Include(v => v.Images).
                .ToListAsync(); // Truy vấn bất đồng bộ và chuyển đổi kết quả trả về dạng danh sách List.
        }

        public async Task<ProductVariant?> GetVariantByIdAsync(int variantId) // Thực hiện xử lý bất đồng bộ phương thức 'GetVariantByIdAsync' nhận tham số (variantId) trả về kiểu Task<ProductVariant?>.
        {
            return await _context.ProductVariants // Chờ và trả về giá trị của tác vụ bất đồng bộ '_context.ProductVariants'.
                .FirstOrDefaultAsync(v => v.Id == variantId && !v.IsDeleted); // Lấy phần tử đầu tiên thỏa mãn điều kiện hoặc trả về null nếu không tìm thấy.
        }

        public async Task ReplaceProductImagesAsync(int productId, List<ProductImage> newImages) // Thực hiện xử lý bất đồng bộ phương thức 'ReplaceProductImagesAsync' nhận tham số (productId, newImages) trả về kiểu Task.
        {
            var variantIds = await _context.ProductVariants // Chờ tác vụ bất đồng bộ hoàn thành và gán kết quả cho biến 'variantIds'.
                .Where(v => v.ProductId == productId && !v.IsDeleted) // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(v => v.ProductId == productId && !v.IsDeleted).
                .Select(v => v.Id) // Thực thi dòng lệnh nghiệp vụ.
                .ToListAsync(); // Truy vấn bất đồng bộ và chuyển đổi kết quả trả về dạng danh sách List.

            var oldImages = await _context.ProductImages // Chờ tác vụ bất đồng bộ hoàn thành và gán kết quả cho biến 'oldImages'.
                .Where(i => variantIds.Contains(i.VariantId)) // Lọc danh sách bản ghi dựa trên biểu thức điều kiện: .Where(i => variantIds.Contains(i.VariantId)).
                .ToListAsync(); // Truy vấn bất đồng bộ và chuyển đổi kết quả trả về dạng danh sách List.

            _context.ProductImages.RemoveRange(oldImages); // Thực thi dòng lệnh nghiệp vụ.

            foreach (var variantId in variantIds) // Thực thi dòng lệnh nghiệp vụ.
            {
                foreach (var img in newImages) // Thực thi dòng lệnh nghiệp vụ.
                {
                    await _context.ProductImages.AddAsync(new ProductImage // Thêm thực thể mới vào CSDL thông qua DbSet tương ứng.
                    {
                        VariantId = variantId, // Thực hiện gán giá trị của biểu thức 'variantId,' cho biến 'VariantId'.
                        ImageUrl = img.ImageUrl, // Thực hiện gán giá trị của biểu thức 'img.ImageUrl,' cho biến 'ImageUrl'.
                        IsMain = img.IsMain, // Thực hiện gán giá trị của biểu thức 'img.IsMain,' cho biến 'IsMain'.
                        SortOrder = img.SortOrder // Thực hiện gán giá trị của biểu thức 'img.SortOrder' cho biến 'SortOrder'.
                    }); // Thực thi dòng lệnh nghiệp vụ.
                }
            }
        }

        public async Task SaveChangesAsync() // Thực hiện xử lý bất đồng bộ phương thức 'SaveChangesAsync' không tham số trả về kiểu Task.
        {
            await _context.SaveChangesAsync(); // Thực thi dòng lệnh nghiệp vụ.
        }
    }
}

using Microsoft.EntityFrameworkCore; // Sử dụng Entity Framework Core.
using PBL3.Core.Interfaces; // Sử dụng các giao diện repository từ tầng Core.
using PBL3.Infrastructure.Data; // Sử dụng DBContext của ứng dụng.
using PBL3.Shared.DTOs.Common; // Sử dụng DTO dùng chung ApiResult.
using PBL3.Shared.DTOs.Storefront; // Sử dụng DTO liên quan đến giao diện Storefront cửa hàng.

namespace PBL3.Application.Storefront // Khai báo namespace cho tầng Application của module Storefront.
{
    public class StorefrontService( // Định nghĩa lớp StorefrontService sử dụng Primary Constructor.
        HushStoreDbContext context, // Tiêm DbContext của ứng dụng.
        IProductReviewRepository reviewRepo) : IStorefrontService // Tiêm repository đánh giá sản phẩm và kế thừa giao diện IStorefrontService.
    {
        private readonly HushStoreDbContext _context = // Gán DbContext vào trường thành viên.
            context ?? throw new ArgumentNullException(nameof(context)); // Kiểm tra null cho context.
        private readonly IProductReviewRepository _reviewRepo = // Gán repository đánh giá vào trường thành viên.
            reviewRepo ?? throw new ArgumentNullException(nameof(reviewRepo)); // Kiểm tra null cho reviewRepo.

        private async Task<Dictionary<int, (double Avg, int Count)>> GetRatingMapAsync(List<int> productIds) // Định nghĩa phương thức phụ trợ bất đồng bộ lấy bản đồ rating của danh sách sản phẩm.
        {
            if (productIds.Count == 0) return new(); // Nếu danh sách Id sản phẩm rỗng, trả về dictionary trống ngay lập tức.
            return await _context.ProductReviews // Truy vấn bảng ProductReviews trong cơ sở dữ liệu.
                .AsNoTracking() // Không theo dõi thay đổi để tối ưu hóa hiệu năng đọc.
                .Where(r => productIds.Contains(r.ProductId)) // Lọc các đánh giá thuộc danh sách sản phẩm yêu cầu.
                .GroupBy(r => r.ProductId) // Nhóm các bản ghi theo ProductId.
                .Select(g => new { ProductId = g.Key, Avg = g.Average(r => (double)r.Rating), Count = g.Count() }) // Chọn Id sản phẩm, tính điểm trung bình và đếm tổng số đánh giá.
                .ToDictionaryAsync(x => x.ProductId, x => (x.Avg, x.Count)); // Chuyển kết quả thành Dictionary dạng Id sản phẩm làm khóa và bộ (Điểm trung bình, Số lượng) làm giá trị.
        }

        public async Task<ApiResult<List<CategoryMenuResponse>>> GetActiveCategoriesAsync() // Định nghĩa phương thức lấy danh sách danh mục đang hoạt động.
        {
            var categories = await _context.Categories // Truy vấn thực thể Categories.
                .AsNoTracking() // Không theo dõi thay đổi.
                .Where(c => c.IsVisible && !c.IsDeleted) // Lọc các danh mục hiển thị công khai và chưa bị xóa mềm.
                .Select(c => new CategoryMenuResponse // Ánh xạ từng danh mục sang DTO hiển thị menu.
                {
                    Id = c.Id, // Ánh xạ Id.
                    Name = c.Name, // Ánh xạ tên danh mục.
                    Slug = c.Slug, // Ánh xạ đường dẫn slug.
                    IconUrl = c.ImageUrl, // Ánh xạ đường dẫn ảnh biểu tượng.
                    Level = c.Level, // Ánh xạ cấp độ của danh mục trong cây phân cấp.
                    ParentId = c.ParentId // Ánh xạ Id danh mục cha.
                }) // Kết thúc biểu thức Select.
                .ToListAsync(); // Chuyển đổi và tải danh sách bất đồng bộ về bộ nhớ.

            return ApiResult<List<CategoryMenuResponse>>.Ok(categories); // Trả về kết quả danh sách danh mục thành công.
        }

        public async Task<ApiResult<List<ProductCardResponse>>> GetFeaturedProductsAsync(int? categoryId, int take = 5) // Định nghĩa phương thức lấy danh sách sản phẩm nổi bật.
        {
            var query = _context.Products // Tạo câu truy vấn cơ bản trên bảng Products.
                .AsNoTracking() // Không theo dõi các thực thể được đọc ra.
                .Where(p => p.Status == 1 && !p.IsDeleted); // Chỉ lấy sản phẩm đang bán (Status = 1) và chưa bị xóa mềm.

            if (categoryId.HasValue) // Nếu có tham số lọc theo danh mục cụ thể.
            {
                query = query.Where(p => p.CategoryId == categoryId.Value); // Bổ sung điều kiện lọc theo CategoryId.
            } // Kết thúc khối điều kiện.

            var products = await query // Tải danh sách sản phẩm nổi bật từ cơ sở dữ liệu.
                .Select(p => new // Ánh xạ sang kiểu nặc danh để chỉ tải các trường cần thiết.
                {
                    p.Id, // Gán trường Id sản phẩm.
                    p.Name, // Gán trường tên sản phẩm.
                    p.Slug, // Gán trường đường dẫn slug.
                    p.CategoryId, // Gán Id danh mục sản phẩm.
                    CategoryName = p.Category != null ? p.Category.Name : string.Empty, // Lấy tên danh mục hoặc để trống nếu null.
                    ActiveVariants = p.Variants.Where(v => !v.IsDeleted), // Lọc các biến thể của sản phẩm chưa bị xóa mềm.
                    MainImage = p.Variants.Where(v => !v.IsDeleted) // Lấy danh sách biến thể chưa bị xóa để tìm ảnh đại diện.
                        .SelectMany(v => v.Images) // Gộp tất cả ảnh của các biến thể.
                        .OrderByDescending(i => i.IsMain) // Ưu tiên ảnh chính lên đầu.
                        .ThenBy(i => i.SortOrder) // Tiếp theo sắp xếp theo thứ tự hiển thị.
                        .FirstOrDefault() // Lấy ảnh đầu tiên tìm được làm ảnh đại diện.
                }) // Kết thúc Select kiểu nặc danh.
                .Take(take) // Giới hạn số lượng sản phẩm cần lấy.
                .ToListAsync(); // Tải dữ liệu bất đồng bộ xuống bộ nhớ RAM.

            var ratingMapFeatured = await GetRatingMapAsync(products.Select(p => p.Id).ToList()); // Lấy bản đồ điểm đánh giá cho danh sách sản phẩm nổi bật này.

            var result = products.Select(p => // Ánh xạ danh sách sản phẩm kiểu nặc danh sang DTO ProductCardResponse.
            {
                var variants = p.ActiveVariants.ToList(); // Chuyển danh sách biến thể đang hoạt động sang List.
                decimal currentPrice = 0; // Khởi tạo giá hiện tại mặc định bằng 0.
                decimal oldPrice = 0; // Khởi tạo giá cũ/gốc mặc định bằng 0.

                if (variants.Any()) // Nếu sản phẩm có chứa biến thể hoạt động nào.
                {
                    currentPrice = variants.Min(v => v.Price); // Lấy giá của biến thể rẻ nhất làm giá bán hiện tại hiển thị.
                    oldPrice = variants.Max(v => v.OriginalPrice ?? v.Price); // Lấy giá niêm yết lớn nhất của biến thể làm giá cũ.
                    if (oldPrice < currentPrice) oldPrice = currentPrice; // Nếu giá cũ nhỏ hơn giá hiện tại thì đồng bộ bằng giá hiện tại.
                } // Kết thúc khối kiểm tra biến thể.

                int discountPercent = 0; // Khởi tạo tỷ lệ phần trăm giảm giá bằng 0.
                if (oldPrice > 0 && currentPrice < oldPrice) // Nếu có giá cũ và giá hiện tại nhỏ hơn giá cũ.
                {
                    discountPercent = (int)Math.Round((oldPrice - currentPrice) / oldPrice * 100); // Tính toán phần trăm giảm giá làm tròn.
                } // Kết thúc khối tính phần trăm giảm giá.

                var ratings = ratingMapFeatured.GetValueOrDefault(p.Id, (0.0, 0)); // Lấy rating từ bản đồ rating, nếu không có mặc định là (0.0, 0).
                return new ProductCardResponse // Trả về đối tượng DTO.
                {
                    Id = p.Id, // Gán Id sản phẩm.
                    Name = p.Name, // Gán tên sản phẩm.
                    Slug = p.Slug, // Gán đường dẫn slug.
                    CategoryId = p.CategoryId, // Gán mã danh mục.
                    CategoryName = p.CategoryName, // Gán tên danh mục.
                    ThumbnailUrl = p.MainImage?.ImageUrl, // Gán ảnh đại diện.
                    CurrentPrice = currentPrice, // Gán giá bán hiện hành.
                    OldPrice = oldPrice, // Gán giá cũ niêm yết.
                    DiscountPercent = discountPercent, // Gán phần trăm giảm giá.
                    IsAvailable = variants.Any(v => v.StockQuantity > 0), // Kiểm tra xem còn biến thể nào còn hàng tồn kho vật lý không.
                    Rating = ratings.Item1, // Gán điểm đánh giá trung bình.
                    ReviewCount = ratings.Item2 // Gán tổng số lượt đánh giá.
                }; // Kết thúc khởi tạo DTO.
            }).ToList(); // Chuyển kết quả ánh xạ sang danh sách List.

            return ApiResult<List<ProductCardResponse>>.Ok(result); // Trả về API Result thành công chứa danh sách sản phẩm.
        }

        public async Task<ApiResult<ProductDetailResponse>> GetProductDetailAsync(string slug) // Định nghĩa phương thức lấy chi tiết sản phẩm theo slug.
        {
            var product = await _context.Products // Truy vấn trên bảng Products.
                .AsNoTracking() // Không theo dõi thay đổi thực thể.
                .Include(p => p.Manufacturer) // Tải kèm thực thể thông tin hãng sản xuất.
                .Include(p => p.Category) // Tải kèm thực thể thông tin danh mục.
                .Include(p => p.Variants.Where(v => !v.IsDeleted)) // Tải kèm danh sách các biến thể chưa bị xóa mềm.
                    .ThenInclude(v => v.Images) // Tải kèm bộ sưu tập hình ảnh tương ứng của từng biến thể.
                .Where(p => p.Slug == slug && p.Status == 1 && !p.IsDeleted) // Lọc sản phẩm theo đường dẫn slug, trạng thái kích hoạt và chưa bị xóa.
                .FirstOrDefaultAsync(); // Lấy bản ghi đầu tiên hoặc trả về null.

            if (product == null) // Nếu không tìm thấy sản phẩm nào.
            {
                return ApiResult<ProductDetailResponse>.Fail("Không tìm thấy sản phẩm yêu cầu.", ApiErrorCode.NotFound); // Trả về kết quả lỗi NotFound.
            } // Kết thúc khối kiểm tra sản phẩm.

            var activeVariants = product.Variants.ToList(); // Lấy danh sách các biến thể đang hoạt động.

            var images = activeVariants // Trích xuất danh sách tất cả hình ảnh từ các biến thể hoạt động.
                .SelectMany(v => v.Images) // Gộp tất cả đối tượng hình ảnh thành một danh sách phẳng.
                .OrderByDescending(i => i.IsMain) // Ưu tiên xếp các ảnh chính lên đầu tiên.
                .ThenBy(i => i.SortOrder) // Sắp xếp theo thứ tự ưu tiên hiển thị.
                .Select(i => i.ImageUrl) // Chỉ lấy trường đường dẫn URL của hình ảnh.
                .Distinct() // Loại bỏ các URL ảnh bị trùng lặp.
                .ToList(); // Đóng gói thành danh sách List.

            var variantResponses = activeVariants.Select(v => new StorefrontVariantResponse // Ánh xạ danh sách biến thể thực thể sang danh sách DTO của Storefront.
            {
                Id = v.Id, // Ánh xạ Id của biến thể.
                VariantName = v.VariantName, // Ánh xạ tên biến thể.
                Price = v.Price, // Ánh xạ đơn giá của biến thể.
                OriginalPrice = v.OriginalPrice > v.Price ? v.OriginalPrice : null, // Gán giá cũ/gốc nếu nó lớn hơn giá bán hiện tại, ngược lại trả về null.
                IsAvailable = v.StockQuantity > 0, // Đánh dấu khả năng mua được nếu lượng tồn kho lớn hơn 0.
                StockQuantity = v.StockQuantity, // Ánh xạ số lượng tồn kho vật lý hiện tại.
                Specifications = v.Specifications ?? new() // Ánh xạ thông số kỹ thuật, mặc định khởi tạo mới nếu bị null.
            }).ToList(); // Chuyển đổi sang kiểu danh sách List.

            var defaultVariant = activeVariants.OrderBy(v => v.Price).FirstOrDefault(); // Chọn biến thể có đơn giá thấp nhất làm biến thể mặc định hiển thị ban đầu.
            string? specsJson = null; // Khởi tạo chuỗi JSON chứa thông số kỹ thuật mặc định là null.
            List<string> shortFeatures = new(); // Khởi tạo danh sách các đặc điểm kỹ thuật tóm tắt.

            if (defaultVariant?.Specifications != null && defaultVariant.Specifications.Any()) // Nếu biến thể mặc định có chứa cấu hình thông số kỹ thuật.
            {
                specsJson = System.Text.Json.JsonSerializer.Serialize(defaultVariant.Specifications); // Tiến hành tuần tự hóa Dictionary sang chuỗi JSON.
                shortFeatures = defaultVariant.Specifications.Select(kvp => $"{kvp.Key}: {kvp.Value}").ToList(); // Ánh xạ các cặp thuộc tính thành danh sách các chuỗi định dạng "Tên thông số: Giá trị".
            } // Kết thúc khối xử lý thông số kỹ thuật.

            var reviewStats = await _context.ProductReviews // Truy vấn thống kê đánh giá của sản phẩm này từ cơ sở dữ liệu.
                .AsNoTracking() // Không theo dõi thực thể.
                .Where(r => r.ProductId == product.Id) // Lọc đánh giá thuộc sản phẩm hiện tại.
                .GroupBy(r => r.ProductId) // Nhóm theo ProductId.
                .Select(g => new { Avg = g.Average(r => (double)r.Rating), Count = g.Count() }) // Tính điểm trung bình và đếm tổng số bình luận.
                .FirstOrDefaultAsync(); // Lấy thống kê đầu tiên hoặc null.

            var response = new ProductDetailResponse // Khởi tạo DTO phản hồi chi tiết sản phẩm.
            {
                Id = product.Id, // Gán mã sản phẩm.
                Name = product.Name, // Gán tên sản phẩm.
                Slug = product.Slug, // Gán đường dẫn slug.
                CategoryId = product.CategoryId, // Gán mã danh mục.
                CategoryName = product.Category?.Name ?? string.Empty, // Gán tên danh mục sản phẩm.
                ManufacturerName = product.Manufacturer?.Name ?? string.Empty, // Gán tên hãng sản xuất.
                Description = product.Description, // Gán bài mô tả chi tiết của sản phẩm.
                Specifications = specsJson, // Gán chuỗi JSON thông số kỹ thuật mặc định.
                Images = images, // Gán danh sách hình ảnh trượt.
                Variants = variantResponses, // Gán danh sách các biến thể có thể chọn.
                ShortFeatures = shortFeatures, // Gán danh sách đặc điểm tóm tắt.
                Rating = reviewStats?.Avg ?? 0.0, // Gán điểm đánh giá trung bình, mặc định là 0.0 nếu chưa có ai đánh giá.
                ReviewCount = reviewStats?.Count ?? 0 // Gán tổng số lượt đánh giá, mặc định là 0.
            }; // Kết thúc khởi tạo DTO chi tiết sản phẩm.

            return ApiResult<ProductDetailResponse>.Ok(response); // Trả về kết quả API thành công kèm theo DTO chi tiết.
        }

        public async Task<ApiResult<List<ProductCardResponse>>> GetRelatedProductsAsync(string slug) // Định nghĩa phương thức lấy danh sách sản phẩm liên quan.
        {
            var currentProduct = await _context.Products // Lấy thông tin cơ bản của sản phẩm hiện tại dựa trên slug.
                .AsNoTracking() // Không theo dõi thực thể.
                .Where(p => p.Slug == slug && p.Status == 1 && !p.IsDeleted) // Điều kiện lọc sản phẩm hoạt động.
                .Select(p => new { p.Id, p.CategoryId }) // Chỉ chọn Id và CategoryId.
                .FirstOrDefaultAsync(); // Lấy bản ghi đầu tiên hoặc null.

            if (currentProduct == null) // Nếu không tìm thấy sản phẩm hiện tại.
            {
                return ApiResult<List<ProductCardResponse>>.Ok(new List<ProductCardResponse>()); // Trả về danh sách sản phẩm liên quan rỗng.
            } // Kết thúc khối kiểm tra sản phẩm hiện tại.

            var relatedProductsQuery = _context.Products // Tạo truy vấn lấy các sản phẩm liên quan cùng danh mục.
                .AsNoTracking() // Không theo dõi thay đổi.
                .Where(p => p.CategoryId == currentProduct.CategoryId && p.Id != currentProduct.Id && p.Status == 1 && !p.IsDeleted) // Cùng danh mục, khác chính nó, đang bán và chưa xóa.
                .Select(p => new // Ánh xạ sang kiểu nặc danh để tối ưu cột nạp từ DB.
                {
                    p.Id, // Chọn Id sản phẩm.
                    p.Name, // Chọn tên sản phẩm.
                    p.Slug, // Chọn đường dẫn slug.
                    p.CategoryId, // Chọn Id danh mục.
                    CategoryName = p.Category != null ? p.Category.Name : string.Empty, // Chọn tên danh mục.
                    ActiveVariants = p.Variants.Where(v => !v.IsDeleted), // Lọc danh sách biến thể chưa bị xóa.
                    MainImage = p.Variants.Where(v => !v.IsDeleted) // Tìm ảnh đại diện từ các biến thể.
                        .SelectMany(v => v.Images) // Gộp tất cả danh sách ảnh.
                        .OrderByDescending(i => i.IsMain) // Ưu tiên ảnh chính.
                        .ThenBy(i => i.SortOrder) // Thứ tự sắp xếp ảnh.
                        .FirstOrDefault() // Lấy ảnh đầu tiên.
                }) // Kết thúc biểu thức Select kiểu nặc danh.
                .Take(5); // Chỉ lấy tối đa 5 sản phẩm liên quan.

            var products = await relatedProductsQuery.ToListAsync(); // Thực thi truy vấn bất đồng bộ để tải danh sách sản phẩm.

            var ratingMapRelated = await GetRatingMapAsync(products.Select(p => p.Id).ToList()); // Lấy bản đồ đánh giá của danh sách sản phẩm liên quan này.

            var result = products.Select(p => // Ánh xạ danh sách sản phẩm sang danh sách DTO ProductCardResponse.
            {
                var variants = p.ActiveVariants.ToList(); // Lấy danh sách biến thể hoạt động.
                decimal currentPrice = 0; // Giá bán hiện hành.
                decimal oldPrice = 0; // Giá niêm yết cũ.

                if (variants.Any()) // Nếu có biến thể.
                {
                    currentPrice = variants.Min(v => v.Price); // Chọn đơn giá thấp nhất làm giá bán hiện tại.
                    oldPrice = variants.Max(v => v.OriginalPrice ?? v.Price); // Chọn giá gốc lớn nhất làm giá cũ.
                    if (oldPrice < currentPrice) oldPrice = currentPrice; // Đảm bảo giá cũ luôn lớn hơn hoặc bằng giá hiện tại.
                } // Kết thúc khối kiểm tra biến thể.

                int discountPercent = 0; // Phần trăm chiết khấu.
                if (oldPrice > 0 && currentPrice < oldPrice) // Nếu có giảm giá.
                {
                    discountPercent = (int)Math.Round((oldPrice - currentPrice) / oldPrice * 100); // Tính phần trăm giảm.
                } // Kết thúc khối tính chiết khấu.

                var ratings = ratingMapRelated.GetValueOrDefault(p.Id, (0.0, 0)); // Lấy rating tương ứng từ bản đồ rating.
                return new ProductCardResponse // Khởi tạo DTO hiển thị thẻ sản phẩm.
                {
                    Id = p.Id, // Gán Id sản phẩm.
                    Name = p.Name, // Gán tên sản phẩm.
                    Slug = p.Slug, // Gán đường dẫn slug.
                    CategoryId = p.CategoryId, // Gán mã danh mục.
                    CategoryName = p.CategoryName, // Gán tên danh mục.
                    ThumbnailUrl = p.MainImage?.ImageUrl, // Gán đường dẫn ảnh thumbnail.
                    CurrentPrice = currentPrice, // Gán giá bán hiện tại.
                    OldPrice = oldPrice, // Gán giá cũ.
                    DiscountPercent = discountPercent, // Gán tỷ lệ phần trăm giảm giá.
                    IsAvailable = variants.Any(v => v.StockQuantity > 0), // Kiểm tra trạng thái tồn kho còn hàng hay không.
                    Rating = ratings.Item1, // Gán điểm đánh giá trung bình.
                    ReviewCount = ratings.Item2 // Gán tổng số bình luận.
                }; // Kết thúc DTO.
            }).ToList(); // Chuyển kết quả sang danh sách List.

            return ApiResult<List<ProductCardResponse>>.Ok(result); // Trả về kết quả danh sách sản phẩm liên quan thành công.
        }

        public async Task<ApiResult<CategoryDetailResponse>> GetCategoryBySlugAsync(string slug) // Định nghĩa phương thức lấy chi tiết danh mục theo slug.
        {
            var category = await _context.Categories // Truy vấn bảng Categories.
                .AsNoTracking() // Không theo dõi thay đổi.
                .Where(c => c.Slug == slug && c.IsVisible && !c.IsDeleted) // Lọc theo slug, trạng thái hiển thị và chưa bị xóa.
                .Select(c => new CategoryDetailResponse // Ánh xạ sang DTO CategoryDetailResponse.
                {
                    Id = c.Id, // Ánh xạ Id danh mục.
                    Name = c.Name, // Ánh xạ tên danh mục.
                    Slug = c.Slug, // Ánh xạ đường dẫn slug.
                    IconUrl = c.ImageUrl // Ánh xạ ảnh đại diện của danh mục.
                }) // Kết thúc Select.
                .FirstOrDefaultAsync(); // Lấy bản ghi đầu tiên hoặc trả về null.

            if (category == null) // Nếu không tìm thấy danh mục nào thỏa mãn.
            {
                return ApiResult<CategoryDetailResponse>.Fail("Không tìm thấy danh mục yêu cầu.", ApiErrorCode.NotFound); // Báo lỗi NotFound.
            } // Kết thúc kiểm tra danh mục.

            return ApiResult<CategoryDetailResponse>.Ok(category); // Trả về DTO chi tiết danh mục thành công.
        }

        public async Task<ApiResult<PagedResult<ProductCardResponse>>> SearchProductsAsync( // Định nghĩa phương thức tìm kiếm nâng cao và lọc sản phẩm.
            string? keyword, int? categoryId, decimal? priceMin, decimal? priceMax, // Từ khóa, Id danh mục, giá tối thiểu, giá tối đa.
            int page, int pageSize) // Số trang và kích thước trang.
        {
            var query = _context.Products // Bắt đầu tạo câu truy vấn trên bảng Products.
                .AsNoTracking() // Không theo dõi các thực thể được đọc ra.
                .Where(p => p.Status == 1 && !p.IsDeleted); // Chỉ lấy sản phẩm đang hoạt động bán và chưa bị xóa mềm.

            if (!string.IsNullOrWhiteSpace(keyword)) // Nếu có từ khóa tìm kiếm.
            {
                var kw = keyword.Trim().ToLower(); // Chuẩn hóa từ khóa về chữ viết thường và cắt khoảng trắng hai đầu.
                query = query.Where(p => // Bổ sung điều kiện tìm kiếm theo tên hoặc mô tả ngắn.
                    p.Name.ToLower().Contains(kw) || // Tìm kiếm từ khóa xuất hiện trong tên sản phẩm.
                    (p.ShortDescription != null && p.ShortDescription.ToLower().Contains(kw))); // Tìm kiếm từ khóa xuất hiện trong mô tả ngắn.
            } // Kết thúc khối lọc từ khóa.

            if (categoryId.HasValue) // Nếu có lọc theo danh mục cụ thể.
                query = query.Where(p => p.CategoryId == categoryId.Value); // Lọc theo đúng CategoryId.

            if (priceMin.HasValue) // Nếu lọc theo mức giá tối thiểu.
                query = query.Where(p => p.Variants.Any(v => !v.IsDeleted && v.Price >= priceMin.Value)); // Lấy sản phẩm có ít nhất một biến thể có giá lớn hơn hoặc bằng priceMin.

            if (priceMax.HasValue) // Nếu lọc theo mức giá tối đa.
                query = query.Where(p => p.Variants.Any(v => !v.IsDeleted && v.Price <= priceMax.Value)); // Lấy sản phẩm có ít nhất một biến thể có giá nhỏ hơn hoặc bằng priceMax.

            var totalCount = await query.CountAsync(); // Thực thi đếm tổng số sản phẩm thỏa mãn điều kiện lọc để phân trang.

            var products = await query // Truy vấn danh sách sản phẩm phân trang.
                .OrderByDescending(p => p.CreatedDate) // Sắp xếp theo ngày tạo giảm dần (sản phẩm mới nhất xếp đầu).
                .Skip((page - 1) * pageSize) // Bỏ qua các bản ghi của các trang trước.
                .Take(pageSize) // Lấy số lượng bản ghi của trang hiện tại.
                .Select(p => new // Ánh xạ sang kiểu nặc danh để giảm thiểu dữ liệu tải lên từ DB.
                {
                    p.Id, // Chọn Id sản phẩm.
                    p.Name, // Chọn tên sản phẩm.
                    p.Slug, // Chọn đường dẫn slug.
                    p.CategoryId, // Chọn mã danh mục.
                    CategoryName = p.Category != null ? p.Category.Name : string.Empty, // Lấy tên danh mục liên kết.
                    ManufacturerName = p.Manufacturer != null ? p.Manufacturer.Name : string.Empty, // Lấy tên hãng sản xuất liên kết.
                    ActiveVariants = p.Variants.Where(v => !v.IsDeleted), // Lọc các biến thể hoạt động.
                    MainImage = p.Variants.Where(v => !v.IsDeleted) // Tìm ảnh đại diện từ các biến thể.
                        .SelectMany(v => v.Images) // Gộp danh sách ảnh.
                        .OrderByDescending(i => i.IsMain) // Ưu tiên ảnh chính.
                        .ThenBy(i => i.SortOrder) // Thứ tự sắp xếp.
                        .FirstOrDefault() // Lấy ảnh đầu tiên.
                }) // Kết thúc Select.
                .ToListAsync(); // Tải danh sách bất đồng bộ về RAM.

            var ratingMapSearch = await GetRatingMapAsync(products.Select(p => p.Id).ToList()); // Lấy bản đồ đánh giá của danh sách sản phẩm vừa tìm được.

            var result = products.Select(p => // Ánh xạ danh sách sản phẩm kiểu nặc danh sang DTO ProductCardResponse.
            {
                var variants = p.ActiveVariants.ToList(); // Lấy danh sách biến thể hoạt động.
                decimal currentPrice = 0; // Giá bán hiện hành.
                decimal oldPrice = 0; // Giá cũ niêm yết.

                if (variants.Any()) // Nếu sản phẩm có biến thể.
                {
                    currentPrice = variants.Min(v => v.Price); // Chọn đơn giá thấp nhất làm giá bán hiện tại.
                    oldPrice = variants.Max(v => v.OriginalPrice ?? v.Price); // Chọn giá gốc lớn nhất làm giá cũ.
                    if (oldPrice < currentPrice) oldPrice = currentPrice; // Đảm bảo tính nhất quán của khoảng giá cũ.
                } // Kết thúc kiểm tra biến thể.

                int discountPercent = 0; // Phần trăm giảm giá.
                if (oldPrice > 0 && currentPrice < oldPrice) // Nếu có giảm giá.
                    discountPercent = (int)Math.Round((oldPrice - currentPrice) / oldPrice * 100); // Tính phần trăm giảm giá.

                var ratings = ratingMapSearch.GetValueOrDefault(p.Id, (0.0, 0)); // Lấy thông tin điểm đánh giá từ bản đồ rating.
                return new ProductCardResponse // Khởi tạo DTO hiển thị thẻ sản phẩm.
                {
                    Id = p.Id, // Gán Id sản phẩm.
                    Name = p.Name, // Gán tên sản phẩm.
                    Slug = p.Slug, // Gán đường dẫn slug.
                    CategoryId = p.CategoryId, // Gán mã danh mục.
                    CategoryName = p.CategoryName, // Gán tên danh mục.
                    ThumbnailUrl = p.MainImage?.ImageUrl, // Gán đường dẫn ảnh thumbnail.
                    ManufacturerName = p.ManufacturerName, // Gán tên hãng sản xuất.
                    CurrentPrice = currentPrice, // Gán giá bán hiện hành.
                    OldPrice = oldPrice, // Gán giá cũ.
                    DiscountPercent = discountPercent, // Gán tỷ lệ giảm giá.
                    IsAvailable = variants.Any(v => v.StockQuantity > 0), // Kiểm tra trạng thái tồn kho còn hàng hay không.
                    Rating = ratings.Item1, // Gán điểm rating trung bình.
                    ReviewCount = ratings.Item2 // Gán tổng số lượt đánh giá.
                }; // Kết thúc DTO.
            }).ToList(); // Chuyển kết quả sang danh sách List.

            var pagedResult = new PagedResult<ProductCardResponse> // Khởi tạo đối tượng phân trang kết quả tìm kiếm.
            {
                Items = result, // Gán danh sách DTO sản phẩm.
                TotalCount = totalCount, // Gán tổng số lượng bản ghi thỏa mãn.
                PageNumber = page, // Gán số trang hiện tại.
                PageSize = pageSize // Gán kích thước trang.
            }; // Kết thúc khởi tạo PagedResult.

            return ApiResult<PagedResult<ProductCardResponse>>.Ok(pagedResult); // Trả về kết quả phân trang thành công.
        }

        public async Task<ApiResult<PagedResult<ProductCardResponse>>> GetProductsByCategoryAsync( // Định nghĩa phương thức lấy danh sách sản phẩm theo danh mục và đệ quy con cháu.
            string categorySlug, int page, int pageSize) // Nhận slug danh mục, số trang và kích thước trang.
        {
            var rootCategory = await _context.Categories // Bước 1: Tìm danh mục gốc tương ứng với slug được yêu cầu.
                .AsNoTracking() // Không theo dõi thay đổi.
                .Where(c => c.Slug == categorySlug && c.IsVisible && !c.IsDeleted) // Điều kiện lọc danh mục hoạt động.
                .Select(c => new { c.Id }) // Chỉ chọn lấy Id danh mục.
                .FirstOrDefaultAsync(); // Lấy bản ghi đầu tiên hoặc null.

            if (rootCategory == null) // Nếu không tìm thấy danh mục gốc nào thỏa mãn.
            {
                return ApiResult<PagedResult<ProductCardResponse>>.Fail("Không tìm thấy danh mục yêu cầu.", ApiErrorCode.NotFound); // Báo lỗi NotFound.
            } // Kết thúc kiểm tra danh mục gốc.

            var allCategories = await _context.Categories // Bước 2: Tải toàn bộ danh sách danh mục phẳng đang hoạt động vào bộ nhớ RAM.
                .AsNoTracking() // Không theo dõi thay đổi.
                .Where(c => c.IsVisible && !c.IsDeleted) // Lọc các danh mục hiển thị công khai và chưa bị xóa.
                .Select(c => new { c.Id, c.ParentId }) // Chỉ lấy cột Id và ParentId để tối ưu hóa bộ nhớ RAM.
                .ToListAsync(); // Tải danh sách bất đồng bộ.

            var categoryIds = new HashSet<int> { rootCategory.Id }; // Khởi tạo HashSet chứa danh sách Id danh mục con cháu, bắt đầu bằng chính Id gốc.
            var queue = new Queue<int>(); // Khởi tạo hàng đợi Queue để thực hiện thuật toán duyệt cây Breadth-First Search (BFS).
            queue.Enqueue(rootCategory.Id); // Đưa Id gốc vào hàng đợi để chuẩn bị duyệt đệ quy.

            while (queue.Count > 0) // Vòng lặp duyệt cây BFS đến khi hàng đợi rỗng.
            {
                var currentId = queue.Dequeue(); // Lấy Id danh mục hiện tại ra khỏi hàng đợi.
                var children = allCategories.Where(c => c.ParentId == currentId).ToList(); // Tìm tất cả danh mục con trực tiếp của danh mục hiện tại trong bộ nhớ RAM.
                foreach (var child in children) // Duyệt qua từng danh mục con tìm được.
                {
                    if (categoryIds.Add(child.Id)) // Thêm Id danh mục con vào HashSet (nếu chưa có).
                    {
                        queue.Enqueue(child.Id); // Tiếp tục đẩy Id danh mục con vừa tìm được vào hàng đợi để duyệt cấp tiếp theo.
                    } // Kết thúc kiểm tra.
                } // Kết thúc vòng lặp foreach.
            } // Kết thúc vòng lặp BFS.

            var baseQuery = _context.Products // Bước 3: Tạo truy vấn lấy sản phẩm có CategoryId nằm trong tập hợp các Id danh mục con cháu đã gom được.
                .AsNoTracking() // Không theo dõi thay đổi.
                .Where(p => categoryIds.Contains(p.CategoryId) && p.Status == 1 && !p.IsDeleted); // Lọc sản phẩm thuộc danh mục, đang hoạt động bán và chưa xóa.

            var totalCount = await baseQuery.CountAsync(); // Thực thi đếm tổng số sản phẩm thỏa mãn điều kiện lọc danh mục đệ quy.

            var products = await baseQuery // Truy vấn danh sách sản phẩm phân trang.
                .OrderByDescending(p => p.CreatedDate) // Sắp xếp theo ngày tạo giảm dần.
                .Skip((page - 1) * pageSize) // Bỏ qua các bản ghi của các trang trước đó.
                .Take(pageSize) // Lấy số lượng bản ghi của trang hiện tại.
                .Select(p => new // Ánh xạ sang kiểu nặc danh để tối ưu hóa cột truyền qua mạng.
                {
                    p.Id, // Chọn Id sản phẩm.
                    p.Name, // Chọn tên sản phẩm.
                    p.Slug, // Chọn đường dẫn slug.
                    p.CategoryId, // Chọn mã danh mục.
                    CategoryName = p.Category != null ? p.Category.Name : string.Empty, // Lấy tên danh mục liên kết.
                    ManufacturerName = p.Manufacturer != null ? p.Manufacturer.Name : string.Empty, // Lấy tên hãng sản xuất liên kết.
                    ActiveVariants = p.Variants.Where(v => !v.IsDeleted), // Lọc danh sách biến thể hoạt động.
                    MainImage = p.Variants.Where(v => !v.IsDeleted) // Tìm ảnh đại diện từ các biến thể.
                        .SelectMany(v => v.Images) // Gộp tất cả ảnh.
                        .OrderByDescending(i => i.IsMain) // Ưu tiên ảnh chính.
                        .ThenBy(i => i.SortOrder) // Thứ tự hiển thị.
                        .FirstOrDefault() // Lấy ảnh đầu tiên.
                }) // Kết thúc biểu thức Select.
                .ToListAsync(); // Tải danh sách bất đồng bộ về bộ nhớ RAM.

            var ratingMapCategory = await GetRatingMapAsync(products.Select(p => p.Id).ToList()); // Lấy bản đồ đánh giá của danh sách sản phẩm thuộc danh mục phân cấp này.

            var result = products.Select(p => // Ánh xạ danh sách thực thể kiểu nặc danh sang DTO ProductCardResponse.
            {
                var variants = p.ActiveVariants.ToList(); // Lấy danh sách biến thể hoạt động.
                decimal currentPrice = 0; // Giá bán hiện tại.
                decimal oldPrice = 0; // Giá niêm yết cũ.

                if (variants.Any()) // Nếu sản phẩm có biến thể.
                {
                    currentPrice = variants.Min(v => v.Price); // Chọn giá thấp nhất làm giá bán hiện tại.
                    oldPrice = variants.Max(v => v.OriginalPrice ?? v.Price); // Chọn giá gốc lớn nhất làm giá cũ.
                    if (oldPrice < currentPrice) oldPrice = currentPrice; // Đồng bộ giá cũ tối thiểu bằng giá hiện hành.
                } // Kết thúc khối kiểm tra biến thể.

                int discountPercent = 0; // Phần trăm giảm giá.
                if (oldPrice > 0 && currentPrice < oldPrice) // Nếu có giảm giá.
                {
                    discountPercent = (int)Math.Round((oldPrice - currentPrice) / oldPrice * 100); // Tính phần trăm giảm giá làm tròn.
                } // Kết thúc khối tính phần trăm giảm giá.

                var ratings = ratingMapCategory.GetValueOrDefault(p.Id, (0.0, 0)); // Lấy rating tương ứng từ bản đồ rating.
                return new ProductCardResponse // Khởi tạo DTO hiển thị thẻ sản phẩm.
                {
                    Id = p.Id, // Gán Id sản phẩm.
                    Name = p.Name, // Gán tên sản phẩm.
                    Slug = p.Slug, // Gán đường dẫn slug.
                    CategoryId = p.CategoryId, // Gán mã danh mục.
                    CategoryName = p.CategoryName, // Gán tên danh mục.
                    ThumbnailUrl = p.MainImage?.ImageUrl, // Gán đường dẫn ảnh đại diện.
                    ManufacturerName = p.ManufacturerName, // Gán tên hãng sản xuất.
                    CurrentPrice = currentPrice, // Gán giá bán hiện tại.
                    OldPrice = oldPrice, // Gán giá cũ.
                    DiscountPercent = discountPercent, // Gán tỷ lệ phần trăm giảm giá.
                    IsAvailable = variants.Any(v => v.StockQuantity > 0), // Kiểm tra trạng thái tồn kho còn hàng hay không.
                    Rating = ratings.Item1, // Gán điểm đánh giá trung bình.
                    ReviewCount = ratings.Item2 // Gán tổng số lượt đánh giá.
                }; // Kết thúc DTO.
            }).ToList(); // Chuyển đổi kết quả sang danh sách List.

            var pagedResult = new PagedResult<ProductCardResponse> // Khởi tạo đối tượng phân trang kết quả danh mục đệ quy.
            {
                Items = result, // Gán danh sách DTO sản phẩm.
                TotalCount = totalCount, // Gán tổng số lượng bản ghi thỏa mãn.
                PageNumber = page, // Gán số trang hiện tại.
                PageSize = pageSize // Gán kích thước trang.
            }; // Kết thúc khởi tạo.

            return ApiResult<PagedResult<ProductCardResponse>>.Ok(pagedResult); // Trả về kết quả phân trang thành công.
        }
    }
}

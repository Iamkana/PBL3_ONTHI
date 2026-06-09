using System.Globalization; // Sử dụng thư viện toàn cầu hóa định dạng tiền tệ/ngày tháng.
using System.Text; // Sử dụng thư viện mã hóa và StringBuilder.
using System.Text.Json; // Sử dụng thư viện tuần tự hóa/giải tuần tự hóa JSON.
using System.Text.RegularExpressions; // Sử dụng thư viện biểu thức chính quy (Regex).
using Microsoft.Extensions.Logging; // Sử dụng thư viện ghi log hệ thống.
using PBL3.Core.Entities; // Sử dụng các thực thể từ tầng Core.
using PBL3.Core.Interfaces; // Sử dụng giao diện Repository từ tầng Core.
using PBL3.Shared.DTOs.Common; // Sử dụng DTO dùng chung ApiResult.
using PBL3.Shared.DTOs.Products; // Sử dụng DTO của module sản phẩm.

namespace PBL3.Application.Products // Khai báo namespace cho tầng Application của module quản lý sản phẩm.
{
    public class ProductService( // Định nghĩa lớp ProductService sử dụng Primary Constructor.
        IProductRepository productRepo, // Tiêm repository sản phẩm.
        ILogger<ProductService> logger) : IProductService // Tiêm logger hệ thống và kế thừa giao diện IProductService.
    {
        private readonly IProductRepository _productRepo = // Gán repository sản phẩm vào trường thành viên.
            productRepo ?? throw new ArgumentNullException(nameof(productRepo)); // Kiểm tra null cho productRepo.
        private readonly ILogger<ProductService> _logger = // Gán logger vào trường thành viên.
            logger ?? throw new ArgumentNullException(nameof(logger)); // Kiểm tra null cho logger.

        public async Task<ApiResult<PagedResult<ProductListDto>>> GetListAsync(ProductFilterRequest request) // Định nghĩa phương thức lấy danh sách sản phẩm phân trang có bộ lọc.
        {
            List<int>? categoryIds = null; // Khởi tạo danh sách Id danh mục lọc đệ quy bằng null.
            if (request.CategoryId.HasValue) // Nếu yêu cầu lọc có chỉ định Id danh mục cụ thể.
            {
                categoryIds = await _productRepo.GetCategoryChildIdsAsync(request.CategoryId.Value); // Truy vấn danh sách Id danh mục bao gồm danh mục gốc và mọi danh mục con đệ quy bên dưới.
            } // Kết thúc khối điều kiện kiểm tra danh mục.

            var (items, totalCount) = await _productRepo.GetPagedListAsync( // Gọi repository thực hiện lấy danh sách sản phẩm phân trang và đếm tổng số lượng bản ghi thỏa mãn.
                request.Keyword, // Truyền từ khóa tìm kiếm.
                categoryIds, // Truyền danh sách Id danh mục (bao gồm cả các con cháu).
                request.ManufacturerId, // Truyền Id hãng sản xuất.
                request.PriceMin, // Truyền khoảng giá tối thiểu.
                request.PriceMax, // Truyền khoảng giá tối đa.
                request.Status.HasValue ? (int)request.Status.Value : null, // Ép kiểu và truyền trạng thái hoạt động của sản phẩm.
                request.PageNumber, // Truyền số trang hiện tại.
                request.PageSize, // Truyền số phần tử tối đa trên trang.
                request.SortBy, // Truyền cột cần sắp xếp.
                request.SortDescending); // Truyền hướng sắp xếp giảm dần (true) hay tăng dần (false).

            var dtos = items.Select(MapToListDto).ToList(); // Ánh xạ danh sách thực thể sản phẩm sang danh sách DTO ProductListDto.

            var pagedResult = new PagedResult<ProductListDto> // Khởi tạo DTO phân trang kết quả trả về.
            {
                Items = dtos, // Gán danh sách DTO sản phẩm.
                TotalCount = totalCount, // Gán tổng số lượng bản ghi.
                PageNumber = request.PageNumber, // Gán số trang hiện tại.
                PageSize = request.PageSize // Gán số phần tử trên trang.
            }; // Kết thúc khởi tạo.

            return ApiResult<PagedResult<ProductListDto>>.Ok(pagedResult); // Trả về kết quả phân trang thành công.
        }

        public async Task<ApiResult<ProductDetailDto>> GetByIdAsync(int id) // Định nghĩa phương thức lấy chi tiết sản phẩm theo Id.
        {
            var product = await _productRepo.GetByIdWithDetailsAsync(id); // Lấy thực thể sản phẩm kèm theo thông tin chi tiết (biến thể, hãng sản xuất, danh mục) từ repository.

            if (product == null) // Nếu không tìm thấy sản phẩm nào khớp với Id.
                return ApiResult<ProductDetailDto>.Fail("Không tìm thấy sản phẩm yêu cầu.", ApiErrorCode.NotFound); // Trả về lỗi NotFound.

            var dto = MapToDetailDto(product); // Ánh xạ thực thể sản phẩm sang DTO chi tiết ProductDetailDto.
            return ApiResult<ProductDetailDto>.Ok(dto); // Trả về kết quả DTO chi tiết sản phẩm thành công.
        }

        public async Task<ApiResult<ProductDetailDto>> CreateAsync(CreateProductRequest request) // Định nghĩa phương thức tạo mới sản phẩm kèm theo các biến thể.
        {
            if (!await _productRepo.ManufacturerExistsAsync(request.ManufacturerId)) // Kiểm tra xem hãng sản xuất được chỉ định có tồn tại trong hệ thống hay không.
                return ApiResult<ProductDetailDto>.Fail("Nhà sản xuất không tồn tại."); // Báo lỗi nếu nhà sản xuất không tồn tại.

            if (!await _productRepo.CategoryExistsAsync(request.CategoryId)) // Kiểm tra xem danh mục được chọn có tồn tại trong hệ thống hay không.
                return ApiResult<ProductDetailDto>.Fail("Danh mục không tồn tại."); // Báo lỗi nếu danh mục không tồn tại.

            foreach (var variant in request.Variants) // Duyệt qua từng biến thể trong yêu cầu tạo mới sản phẩm.
            {
                if (await _productRepo.IsSkuExistsAsync(variant.SKU)) // Kiểm tra xem mã SKU của biến thể đã tồn tại trong cơ sở dữ liệu hệ thống hay chưa.
                    return ApiResult<ProductDetailDto>.Fail($"Mã SKU '{variant.SKU}' đã tồn tại trong hệ thống.", ApiErrorCode.Conflict); // Báo lỗi xung đột mã SKU nếu đã tồn tại.
            } // Kết thúc vòng lặp.

            var skus = request.Variants.Select(v => v.SKU.ToUpper()).ToList(); // Trích xuất danh sách mã SKU viết hoa từ yêu cầu để kiểm tra chéo trùng lặp nội bộ.
            if (skus.Distinct().Count() != skus.Count) // Nếu số lượng SKU duy nhất khác số lượng SKU ban đầu (nghĩa là có SKU bị trùng trong cùng một request).
                return ApiResult<ProductDetailDto>.Fail("Các phiên bản trong cùng sản phẩm không được trùng mã SKU.", ApiErrorCode.Conflict); // Báo lỗi trùng mã SKU nội bộ.

            var product = new Product // Khởi tạo thực thể sản phẩm cha.
            {
                Name = request.Name, // Gán tên sản phẩm.
                Slug = GenerateProductSlug(request.Name), // Sinh đường dẫn tĩnh slug chuẩn SEO dựa trên tên sản phẩm.
                ShortDescription = request.ShortDescription, // Gán mô tả tóm tắt.
                Description = request.Description, // Gán bài viết mô tả chi tiết sản phẩm.
                ManufacturerId = request.ManufacturerId, // Gán mã hãng sản xuất.
                CategoryId = request.CategoryId, // Gán mã danh mục sản phẩm.
                Status = 1, // Mặc định gán trạng thái hoạt động kích hoạt.
                CreatedDate = DateTime.UtcNow // Gán thời gian tạo theo UTC.
            }; // Kết thúc khởi tạo thực thể sản phẩm.

            foreach (var variantReq in request.Variants) // Duyệt qua từng biến thể trong yêu cầu để ánh xạ và tạo liên kết với sản phẩm cha.
            {
                var variant = new ProductVariant // Khởi tạo thực thể biến thể sản phẩm mới.
                {
                    SKU = variantReq.SKU, // Gán mã SKU độc bản.
                    VariantName = variantReq.VariantName, // Gán tên riêng của biến thể.
                    Slug = GenerateSlug(request.Name, variantReq.SKU), // Sinh đường dẫn tĩnh slug chuẩn SEO kết hợp tên sản phẩm cha và mã SKU mới.
                    Price = variantReq.Price, // Gán giá bán hiện tại.
                    OriginalPrice = variantReq.OriginalPrice, // Gán giá cũ niêm yết (nếu có).
                    WarrantyMonth = variantReq.WarrantyMonth, // Gán số tháng bảo hành chính hãng.
                    Specifications = variantReq.Specifications ?? new(), // Gán cấu hình thông số kỹ thuật dạng Dictionary.
                    CreatedDate = DateTime.UtcNow // Gán ngày tạo theo UTC.
                }; // Kết thúc khởi tạo biến thể.

                foreach (var imgReq in variantReq.Images) // Duyệt qua danh sách ảnh của biến thể hiện tại.
                {
                    variant.Images.Add(new ProductImage // Thêm ảnh mới vào tập hợp ảnh của biến thể.
                    {
                        ImageUrl = imgReq.ImageUrl, // Gán đường dẫn ảnh.
                        IsMain = imgReq.IsMain, // Xác định đây có phải ảnh đại diện chính của biến thể hay không.
                        SortOrder = imgReq.SortOrder // Gán số thứ tự sắp xếp hiển thị ảnh.
                    }); // Kết thúc thêm ảnh.
                } // Kết thúc vòng lặp duyệt ảnh.

                product.Variants.Add(variant); // Gắn thực thể biến thể vừa thiết lập vào tập hợp biến thể của sản phẩm cha.
            } // Kết thúc vòng lặp duyệt biến thể.

            await _productRepo.AddAsync(product); // Gọi repository lưu thực thể sản phẩm cha (EF Core tự động lưu kèm các thực thể con phụ thuộc do mối quan hệ 1-N).
            await _productRepo.SaveChangesAsync(); // Lưu các thay đổi xuống cơ sở dữ liệu.

            _logger.LogInformation("Tạo sản phẩm mới: {ProductName} (Id: {ProductId}) với {VariantCount} phiên bản.", // Ghi log thông báo tạo sản phẩm mới thành công.
                product.Name, product.Id, product.Variants.Count); // Các tham số thông tin đi kèm.

            var created = await _productRepo.GetByIdWithDetailsAsync(product.Id); // Lấy lại thực thể sản phẩm kèm đầy đủ thông tin chi tiết liên quan từ repository.
            var dto = MapToDetailDto(created!); // Ánh xạ thực thể vừa lấy sang DTO chi tiết ProductDetailDto.

            return ApiResult<ProductDetailDto>.Ok(dto, "Tạo sản phẩm thành công."); // Trả về kết quả tạo mới sản phẩm thành công.
        }

        public async Task<ApiResult<ProductDetailDto>> UpdateAsync(int id, UpdateProductRequest request) // Định nghĩa phương thức cập nhật thông tin sản phẩm theo Id.
        {
            var product = await _productRepo.GetByIdAsync(id); // Lấy thực thể sản phẩm theo Id từ repository.

            if (product == null) // Nếu không tìm thấy sản phẩm nào thỏa mãn Id.
                return ApiResult<ProductDetailDto>.Fail("Không tìm thấy sản phẩm yêu cầu.", ApiErrorCode.NotFound); // Trả về lỗi NotFound.

            if (!await _productRepo.ManufacturerExistsAsync(request.ManufacturerId)) // Kiểm tra sự tồn tại của hãng sản xuất được cập nhật.
                return ApiResult<ProductDetailDto>.Fail("Nhà sản xuất không tồn tại."); // Báo lỗi nếu hãng không tồn tại.

            if (!await _productRepo.CategoryExistsAsync(request.CategoryId)) // Kiểm tra sự tồn tại của danh mục được cập nhật.
                return ApiResult<ProductDetailDto>.Fail("Danh mục không tồn tại."); // Báo lỗi nếu danh mục không tồn tại.

            product.Name = request.Name; // Cập nhật lại tên sản phẩm.
            product.Slug = GenerateProductSlug(request.Name); // Tái cấu trúc và cập nhật lại đường dẫn slug SEO theo tên mới.
            product.ShortDescription = request.ShortDescription; // Cập nhật lại mô tả tóm tắt.
            product.Description = request.Description; // Cập nhật lại mô tả chi tiết.
            product.ManufacturerId = request.ManufacturerId; // Cập nhật lại Id nhà sản xuất.
            product.CategoryId = request.CategoryId; // Cập nhật lại Id danh mục.
            product.Status = (byte)request.Status; // Cập nhật lại trạng thái sản phẩm (Ví dụ: Đang bán, Ngừng kinh doanh).
            product.ModifiedDate = DateTime.UtcNow; // Ghi nhận thời gian chỉnh sửa mới nhất theo giờ UTC.

            await _productRepo.SaveChangesAsync(); // Lưu thay đổi vào cơ sở dữ liệu.

            _logger.LogInformation("Cập nhật sản phẩm: {ProductName} (Id: {ProductId})", // Ghi log thông báo cập nhật sản phẩm thành công.
                product.Name, product.Id); // Các tham số thông tin đi kèm.

            var updated = await _productRepo.GetByIdWithDetailsAsync(product.Id); // Tải lại thông tin thực thể sản phẩm hoàn chỉnh từ DB.
            var dto = MapToDetailDto(updated!); // Ánh xạ thực thể vừa lấy sang DTO chi tiết.

            return ApiResult<ProductDetailDto>.Ok(dto, "Cập nhật sản phẩm thành công."); // Trả về kết quả cập nhật sản phẩm thành công.
        }

        public async Task<ApiResult<ProductVariantDto>> AddVariantAsync(int productId, SaveVariantRequest request) // Định nghĩa phương thức thêm mới một biến thể cho sản phẩm sẵn có.
        {
            var product = await _productRepo.GetByIdAsync(productId); // Lấy thực thể sản phẩm theo Id của sản phẩm cha.

            if (product == null) // Nếu không tìm thấy sản phẩm cha nào thỏa mãn.
                return ApiResult<ProductVariantDto>.Fail("Không tìm thấy sản phẩm yêu cầu.", ApiErrorCode.NotFound); // Trả về lỗi NotFound.

            if (await _productRepo.IsSkuExistsAsync(request.SKU)) // Kiểm tra xem mã SKU của biến thể mới thêm có bị trùng lặp trong hệ thống hay không.
                return ApiResult<ProductVariantDto>.Fail($"Mã SKU '{request.SKU}' đã tồn tại trong hệ thống.", ApiErrorCode.Conflict); // Báo lỗi Conflict nếu trùng SKU.

            var variant = new ProductVariant // Khởi tạo thực thể biến thể sản phẩm mới.
            {
                ProductId = productId, // Gán Id sản phẩm cha liên kết.
                SKU = request.SKU, // Gán mã SKU độc bản.
                VariantName = request.VariantName, // Gán tên riêng của phiên bản này.
                Slug = GenerateSlug(product.Name, request.SKU), // Sinh đường dẫn tĩnh slug chuẩn SEO kết hợp tên sản phẩm cha và mã SKU mới.
                Price = request.Price, // Gán giá bán hiện tại.
                OriginalPrice = request.OriginalPrice, // Gán giá gốc.
                WarrantyMonth = request.WarrantyMonth, // Gán số tháng bảo hành.
                Specifications = request.Specifications ?? new(), // Gán thông số kỹ thuật.
                CreatedDate = DateTime.UtcNow // Gán thời gian tạo.
            }; // Kết thúc khởi tạo biến thể.

            foreach (var imgReq in request.Images) // Duyệt qua danh sách hình ảnh được gửi lên cho biến thể mới này.
            {
                variant.Images.Add(new ProductImage // Thêm ảnh mới vào tập hợp ảnh của biến thể.
                {
                    ImageUrl = imgReq.ImageUrl, // Gán đường dẫn ảnh.
                    IsMain = imgReq.IsMain, // Xác định ảnh chính.
                    SortOrder = imgReq.SortOrder // Gán thứ tự sắp xếp.
                }); // Kết thúc thêm ảnh.
            } // Kết thúc vòng lặp.

            await _productRepo.AddVariantAsync(variant); // Gọi repository thực hiện thêm mới biến thể vào cơ sở dữ liệu.
            await _productRepo.SaveChangesAsync(); // Lưu các thay đổi.

            _logger.LogInformation("Thêm phiên bản '{VariantName}' (SKU: {SKU}) cho sản phẩm Id: {ProductId}", // Ghi log thông báo thêm biến thể thành công.
                variant.VariantName, variant.SKU, productId); // Các tham số thông tin chi tiết.

            var dto = MapToVariantDto(variant); // Ánh xạ thực thể biến thể vừa thêm thành công sang DTO hiển thị.
            return ApiResult<ProductVariantDto>.Ok(dto, "Thêm phiên bản thành công."); // Trả về kết quả thêm biến thể thành công.
        }

        public async Task<ApiResult<bool>> UpdateImagesAsync(int productId, List<SaveImageRequest> images) // Định nghĩa phương thức cập nhật/thay thế danh sách hình ảnh của sản phẩm.
        {
            var product = await _productRepo.GetByIdAsync(productId); // Lấy thực thể sản phẩm theo Id từ repository.
            if (product == null) // Nếu không tìm thấy sản phẩm.
                return ApiResult<bool>.Fail("Không tìm thấy sản phẩm yêu cầu.", ApiErrorCode.NotFound); // Trả về lỗi NotFound.

            var newImages = images.Select((img, idx) => new ProductImage // Ánh xạ danh sách SaveImageRequest sang danh sách thực thể ProductImage mới.
            {
                ImageUrl = img.ImageUrl, // Gán đường dẫn ảnh.
                IsMain = idx == 0, // Mặc định chọn ảnh đầu tiên làm ảnh đại diện chính (IsMain = true).
                SortOrder = idx // Gán thứ tự hiển thị sắp xếp tăng dần theo chỉ số vị trí.
            }).ToList(); // Đóng gói thành danh sách List.

            await _productRepo.ReplaceProductImagesAsync(productId, newImages); // Gọi repository thay thế toàn bộ ảnh hiện tại của sản phẩm bằng danh sách ảnh mới.
            await _productRepo.SaveChangesAsync(); // Lưu các thay đổi xuống DB.

            return ApiResult<bool>.Ok(true, "Cập nhật ảnh sản phẩm thành công."); // Trả về kết quả cập nhật ảnh sản phẩm thành công.
        }

        public async Task<ApiResult<bool>> DeleteAsync(int id) // Định nghĩa phương thức xóa mềm sản phẩm.
        {
            var product = await _productRepo.GetByIdAsync(id); // Lấy thực thể sản phẩm theo Id từ repository.

            if (product == null) // Nếu không tìm thấy sản phẩm cần xóa.
                return ApiResult<bool>.Fail("Không tìm thấy sản phẩm yêu cầu.", ApiErrorCode.NotFound); // Báo lỗi NotFound.

            product.IsDeleted = true; // Thực hiện xóa mềm bằng cách đánh dấu trường IsDeleted bằng true.
            product.DeletedDate = DateTime.UtcNow; // Ghi nhận thời điểm thực hiện hành động xóa theo giờ UTC.
            product.Status = (byte)ProductStatus.StopBusiness; // Đồng thời chuyển trạng thái hoạt động của sản phẩm sang Ngừng kinh doanh.

            await _productRepo.SaveChangesAsync(); // Lưu thay đổi vào cơ sở dữ liệu.

            _logger.LogInformation("Xóa mềm sản phẩm: {ProductName} (Id: {ProductId})", // Ghi log thông báo xóa mềm sản phẩm thành công.
                product.Name, product.Id); // Các tham số thông tin chi tiết.

            return ApiResult<bool>.Ok(true, "Xóa sản phẩm thành công."); // Trả về kết quả xóa thành công.
        }

        private static ProductDetailDto MapToDetailDto(Product entity) // Định nghĩa hàm phụ trợ tĩnh ánh xạ thực thể sản phẩm sang DTO chi tiết.
        {
            return new ProductDetailDto // Khởi tạo DTO chi tiết sản phẩm.
            {
                Id = entity.Id, // Ánh xạ Id.
                Name = entity.Name, // Ánh xạ tên.
                Slug = entity.Slug, // Ánh xạ slug.
                ShortDescription = entity.ShortDescription, // Ánh xạ mô tả ngắn.
                Description = entity.Description, // Ánh xạ mô tả chi tiết.
                ManufacturerId = entity.ManufacturerId, // Ánh xạ Id hãng sản xuất.
                ManufacturerName = entity.Manufacturer?.Name ?? string.Empty, // Ánh xạ tên hãng sản xuất liên kết.
                CategoryId = entity.CategoryId, // Ánh xạ Id danh mục.
                CategoryName = entity.Category?.Name ?? string.Empty, // Ánh xạ tên danh mục liên kết.
                Status = (ProductStatus)entity.Status, // Ánh xạ và chuyển đổi kiểu trạng thái sang Enum.
                CreatedDate = entity.CreatedDate, // Ánh xạ ngày tạo.
                Variants = entity.Variants // Ánh xạ danh sách biến thể sản phẩm.
                    .Where(v => !v.IsDeleted) // Chỉ lấy các biến thể chưa bị xóa mềm.
                    .Select(MapToVariantDto) // Ánh xạ từng biến thể thực thể sang DTO.
                    .ToList() // Chuyển đổi sang danh sách List.
            }; // Kết thúc DTO.
        }

        private static ProductVariantDto MapToVariantDto(ProductVariant v) // Định nghĩa hàm phụ trợ tĩnh ánh xạ thực thể biến thể sang DTO.
        {
            return new ProductVariantDto // Khởi tạo DTO biến thể sản phẩm.
            {
                Id = v.Id, // Ánh xạ Id của biến thể.
                SKU = v.SKU, // Ánh xạ mã SKU độc bản.
                VariantName = v.VariantName, // Ánh xạ tên riêng biến thể.
                Slug = v.Slug, // Ánh xạ đường dẫn slug.
                Price = v.Price, // Ánh xạ giá bán.
                OriginalPrice = v.OriginalPrice, // Ánh xạ giá gốc niêm yết.
                StockQuantity = v.StockQuantity, // Ánh xạ số lượng tồn kho vật lý hiện hành.
                WarrantyMonth = v.WarrantyMonth, // Ánh xạ số tháng bảo hành.
                Specifications = v.Specifications, // Ánh xạ thông số kỹ thuật dạng Dictionary.
                Images = v.Images?.OrderBy(i => i.SortOrder).Select(i => new ProductImageDto // Ánh xạ danh sách hình ảnh đã được sắp xếp theo thứ tự hiển thị.
                {
                    Id = i.Id, // Ánh xạ Id ảnh.
                    ImageUrl = i.ImageUrl, // Ánh xạ URL ảnh.
                    IsMain = i.IsMain, // Ánh xạ trạng thái ảnh đại diện chính.
                    SortOrder = i.SortOrder // Ánh xạ số thứ tự sắp xếp hiển thị.
                }).ToList() ?? new() // Khởi tạo danh sách mới nếu danh sách ảnh rỗng.
            }; // Kết thúc DTO.
        }

        private static ProductListDto MapToListDto(Product entity) // Định nghĩa hàm phụ trợ tĩnh ánh xạ thực thể sản phẩm sang DTO danh sách rút gọn.
        {
            var activeVariants = entity.Variants?.Where(v => !v.IsDeleted).ToList() ?? new(); // Lấy danh sách các biến thể đang hoạt động, chưa bị xóa mềm.
            var minPrice = activeVariants.Any() ? activeVariants.Min(v => v.Price) : 0; // Tìm mức giá thấp nhất trong tất cả biến thể, mặc định là 0 nếu không có biến thể nào.
            var maxPrice = activeVariants.Any() ? activeVariants.Max(v => v.Price) : 0; // Tìm mức giá cao nhất trong tất cả biến thể, mặc định là 0 nếu không có biến thể nào.
            string? thumbnailUrl = null; // Khởi tạo biến lưu đường dẫn ảnh thumbnail sản phẩm là null.
            if (activeVariants.Any()) // Nếu có biến thể hoạt động.
            {
                var firstVariant = activeVariants.First(); // Chọn lấy biến thể đầu tiên trong danh sách.
                thumbnailUrl = firstVariant.Images? // Lấy ảnh đại diện từ bộ sưu tập ảnh của biến thể đó.
                    .OrderByDescending(i => i.IsMain) // Ưu tiên xếp ảnh chính lên hàng đầu.
                    .ThenBy(i => i.SortOrder) // Thứ tự sắp xếp tiếp theo.
                    .FirstOrDefault()?.ImageUrl; // Trích xuất đường dẫn ảnh đầu tiên tìm thấy.
            } // Kết thúc khối xử lý thumbnail.

            return new ProductListDto // Khởi tạo DTO hiển thị danh sách rút gọn.
            {
                Id = entity.Id, // Ánh xạ Id.
                Name = entity.Name, // Ánh xạ tên sản phẩm.
                ShortDescription = entity.ShortDescription, // Ánh xạ mô tả ngắn.
                ManufacturerName = entity.Manufacturer?.Name ?? string.Empty, // Ánh xạ tên nhà sản xuất.
                CategoryName = entity.Category?.Name ?? string.Empty, // Ánh xạ tên danh mục.
                Status = (ProductStatus)entity.Status, // Ánh xạ trạng thái sản phẩm dưới dạng Enum.
                MinPrice = minPrice, // Gán giá nhỏ nhất của các biến thể.
                MaxPrice = maxPrice, // Gán giá lớn nhất của các biến thể.
                PriceRange = FormatPriceRange(minPrice, maxPrice), // Định dạng khoảng giá hiển thị thân thiện (Ví dụ: "10.000đ - 20.000đ").
                ThumbnailUrl = thumbnailUrl, // Gán ảnh thumbnail đại diện.
                TotalStock = activeVariants.Sum(v => v.StockQuantity), // Tính tổng số lượng hàng tồn kho của tất cả biến thể cộng lại.
                VariantCount = activeVariants.Count, // Đếm tổng số lượng biến thể đang hoạt động của dòng sản phẩm này.
                CreatedDate = entity.CreatedDate // Ánh xạ ngày khởi tạo sản phẩm.
            }; // Kết thúc khởi tạo DTO.
        }

        private static string FormatPriceRange(decimal min, decimal max) // Định nghĩa phương thức phụ trợ tĩnh định dạng khoảng giá hiển thị.
        {
            var culture = new CultureInfo("vi-VN"); // Khởi tạo cấu hình ngôn ngữ Việt Nam để định dạng đơn vị tiền tệ chính xác.
            if (min == max) // Nếu mức giá thấp nhất bằng mức giá cao nhất (Sản phẩm chỉ có một mức giá duy nhất).
                return min.ToString("N0", culture) + "đ"; // Trả về chuỗi định dạng mức giá duy nhất kèm chữ đ ở cuối.

            return $"{min.ToString("N0", culture)}đ - {max.ToString("N0", culture)}đ"; // Trả về chuỗi định dạng khoảng giá từ Min đến Max ngăn cách bằng dấu gạch ngang.
        }

        private static string GenerateSlug(string productName, string sku) // Định nghĩa phương thức phụ trợ tĩnh sinh đường dẫn tĩnh (URL Slug) cho biến thể sản phẩm.
        {
            var combined = $"{productName} {sku}"; // Gộp tên sản phẩm cha và mã SKU biến thể ngăn cách bằng dấu cách.
            var normalized = combined.Normalize(NormalizationForm.FormD); // Chuẩn hóa chuỗi gộp dưới dạng Unicode FormD để phân rã các ký tự tiếng Việt có dấu.
            var sb = new StringBuilder(); // Khởi tạo đối tượng StringBuilder để lưu chuỗi đã lọc dấu.
            foreach (var c in normalized) // Duyệt qua từng ký tự của chuỗi Unicode đã phân rã.
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c); // Lấy nhóm danh mục Unicode của ký tự hiện hành.
                if (unicodeCategory != UnicodeCategory.NonSpacingMark) // Nếu ký tự hiện hành không thuộc nhóm dấu thanh/dấu phụ (NonSpacingMark).
                    sb.Append(c); // Thêm ký tự gốc sạch dấu vào StringBuilder.
            } // Kết thúc vòng lặp.
            var noDiacritics = sb.ToString().Normalize(NormalizationForm.FormC); // Tái tổ hợp chuỗi sạch dấu về dạng Unicode FormC tiêu chuẩn để xử lý chuỗi bình thường.

            var slug = Regex.Replace(noDiacritics.ToLower(), @"[^a-z0-9\s-]", ""); // Chuyển chữ thường và sử dụng Regex để loại bỏ toàn bộ ký tự đặc biệt trừ chữ cái, chữ số, khoảng trắng và dấu gạch.
            slug = Regex.Replace(slug, @"[\s-]+", "-").Trim('-'); // Thay thế các chuỗi khoảng trắng hoặc dấu gạch nối liên tiếp bằng một dấu gạch (-) duy nhất và cắt bỏ các dấu gạch dư thừa ở hai đầu.

            return slug; // Trả về chuỗi slug hoàn chỉnh cho biến thể.
        }

        private static string GenerateProductSlug(string productName) // Định nghĩa phương thức phụ trợ tĩnh sinh đường dẫn tĩnh (URL Slug) cho sản phẩm cha.
        {
            var normalized = productName.Normalize(NormalizationForm.FormD); // Chuẩn hóa tên sản phẩm sang Unicode FormD để phân rã dấu tiếng Việt.
            var sb = new StringBuilder(); // Khởi tạo đối tượng StringBuilder.
            foreach (var c in normalized) // Duyệt qua từng ký tự.
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c); // Lấy danh mục nhóm Unicode.
                if (unicodeCategory != UnicodeCategory.NonSpacingMark) // Lọc bỏ tất cả dấu thanh và dấu phụ.
                    sb.Append(c); // Thêm ký tự gốc sạch dấu vào StringBuilder.
            } // Kết thúc vòng lặp.
            var noDiacritics = sb.ToString().Normalize(NormalizationForm.FormC); // Tái tổ hợp chuỗi về Unicode FormC tiêu chuẩn.

            var slug = Regex.Replace(noDiacritics.ToLower(), @"[^a-z0-9\s-]", ""); // Chuyển chữ thường và loại bỏ các ký tự đặc biệt.
            slug = Regex.Replace(slug, @"[\s-]+", "-").Trim('-'); // Đồng nhất khoảng trắng/dấu nối thành một dấu gạch ngang duy nhất và dọn dẹp hai đầu.

            return slug; // Trả về chuỗi slug chuẩn SEO cho sản phẩm cha.
        }
    }
}

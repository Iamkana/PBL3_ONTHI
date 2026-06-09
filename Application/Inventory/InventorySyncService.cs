using System.Collections.Generic; // Sử dụng lớp generic danh sách và tập hợp.
using System.Linq; // Sử dụng thư viện LINQ truy vấn tập hợp dữ liệu.
using System.Threading.Tasks; // Sử dụng Task lập trình bất đồng bộ.
using Microsoft.EntityFrameworkCore; // Sử dụng các hàm mở rộng của Entity Framework Core.
using PBL3.Core.Interfaces; // Sử dụng giao diện IInventorySyncService.
using PBL3.Infrastructure.Data; // Sử dụng cơ sở dữ liệu DbContext HushStoreDbContext.

namespace PBL3.Application.Inventory // Khai báo namespace cho lớp dịch vụ đồng bộ tồn kho thuộc tầng Application.
{
    public class InventorySyncService( // Định nghĩa lớp InventorySyncService bằng Primary Constructor.
        HushStoreDbContext context) : IInventorySyncService // Tiêm HushStoreDbContext để truy cập DB trực tiếp và triển khai IInventorySyncService.
    {
        private readonly HushStoreDbContext _context = // Gán DBContext vào trường thành viên.
            context ?? throw new ArgumentNullException(nameof(context)); // Kiểm tra null cho DbContext.

        public async Task SyncStockAsync(int variantId) // Định nghĩa phương thức đồng bộ số lượng tồn kho cho một biến thể sản phẩm cụ thể.
        {
            var availableCount = await _context.ProductSerials // Thực hiện đếm số lượng serial của biến thể có sẵn trong bảng ProductSerials.
                .CountAsync(s => s.VariantId == variantId && s.Status == 0); // Đếm bất đồng bộ các bản ghi khớp với VariantId và Status = 0 (Available).

            await _context.ProductVariants // Lấy các biến thể sản phẩm trong DB Context.
                .Where(v => v.Id == variantId) // Lọc biến thể cần cập nhật theo mã Id.
                .ExecuteUpdateAsync(s => // Gọi ExecuteUpdateAsync để cập nhật trực tiếp xuống DB mà không cần tải thực thể lên bộ nhớ.
                    s.SetProperty(v => v.StockQuantity, availableCount)); // Thiết lập thuộc tính StockQuantity bằng số lượng serial sẵn có vừa đếm được.
        }

        public async Task SyncStockBatchAsync(IEnumerable<int> variantIds) // Định nghĩa phương thức đồng bộ tồn kho hàng loạt cho danh sách biến thể.
        {
            var ids = variantIds.Distinct().ToList(); // Loại bỏ các mã trùng lặp trong danh sách đầu vào và chuyển thành List.
            if (ids.Count == 0) return; // Nếu danh sách đầu vào trống, lập tức thoát phương thức.

            var stockCounts = await _context.ProductSerials // Truy vấn đếm số lượng tồn kho của các biến thể tương ứng hàng loạt.
                .Where(s => ids.Contains(s.VariantId) && s.Status == 0) // Lọc các serial thuộc danh sách Id và có trạng thái sẵn có (Status = 0).
                .GroupBy(s => s.VariantId) // Nhóm kết quả theo VariantId.
                .Select(g => new { VariantId = g.Key, Count = g.Count() }) // Chọn ra cặp thông tin VariantId và số lượng đếm được.
                .ToDictionaryAsync(x => x.VariantId, x => x.Count); // Chuyển đổi kết quả bất đồng bộ thành một Dictionary để tra cứu nhanh.

            foreach (var variantId in ids) // Duyệt qua từng mã biến thể sản phẩm cần đồng bộ.
            {
                var count = stockCounts.GetValueOrDefault(variantId, 0); // Lấy số lượng tồn kho từ Dictionary, nếu không tìm thấy gán mặc định là 0.
                
                await _context.ProductVariants // Truy cập bảng ProductVariants trong DB Context.
                    .Where(v => v.Id == variantId) // Lọc biến thể tương ứng.
                    .ExecuteUpdateAsync(s => // Cập nhật trực tiếp xuống DB.
                        s.SetProperty(v => v.StockQuantity, count)); // Cập nhật thuộc tính StockQuantity bằng số lượng mới.
            }
        }
    }
}

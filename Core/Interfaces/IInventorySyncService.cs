using System.Collections.Generic; // Nhập thư viện làm việc với các kiểu danh sách, bộ sưu tập.
using System.Threading.Tasks; // Nhập thư viện hỗ trợ lập trình bất đồng bộ.

namespace PBL3.Core.Interfaces // Định nghĩa namespace PBL3.Core.Interfaces để quản lý thực thể/giao diện.
{
    /// <summary>
    /// Service đồng bộ StockQuantity vật lý trong ProductVariants.
    /// Được gọi SAU MỌI thao tác thay đổi trạng thái Serial.
    /// Luôn luôn tuân thủ nguyên tắc CQRS: Tách biệt Read/Write để tối ưu.
    /// </summary>
    public interface IInventorySyncService // Định nghĩa giao diện (interface) IInventorySyncService quy định hợp đồng cho tầng dữ liệu.
    {
        /// <summary>
        /// Đồng bộ StockQuantity cho 1 VariantId cụ thể.
        /// Count Serials WHERE Status = 0 (Available) rồi UPDATE vào cột StockQuantity.
        /// </summary>
        Task SyncStockAsync(int variantId); // Định nghĩa phương thức bất đồng bộ 'SyncStockAsync' với tham số (variantId) trả về kiểu Task.

        /// <summary>
        /// Đồng bộ StockQuantity cho nhiều VariantId cùng lúc (batch).
        /// Dùng khi nhập kho nhiều dòng hoặc xử lý đơn hàng nhiều sản phẩm.
        /// Sử dụng bulk update để giảm queries.
        /// </summary>
        Task SyncStockBatchAsync(IEnumerable<int> variantIds); // Định nghĩa phương thức bất đồng bộ 'SyncStockBatchAsync' với tham số (variantIds) trả về kiểu Task.
    }
}

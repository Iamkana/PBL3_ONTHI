using PBL3.Core.Entities; // Nhập namespace chứa các thực thể nghiệp vụ cốt lõi của hệ thống.

namespace PBL3.Core.Interfaces // Định nghĩa namespace PBL3.Core.Interfaces để quản lý thực thể/giao diện.
{
    /// <summary>
    /// Repository interface cho ProductSerial.
    /// </summary>
    public interface IProductSerialRepository // Định nghĩa giao diện (interface) IProductSerialRepository quy định hợp đồng cho tầng dữ liệu.
    {
        /// <summary>
        /// Kiểm tra danh sách Serial đã tồn tại trong DB chưa.
        /// Trả về danh sách Serial bị trùng.
        /// </summary>
        Task<List<string>> GetExistingSerialsAsync(List<string> serialNumbers); // Định nghĩa phương thức bất đồng bộ 'GetExistingSerialsAsync' với tham số (serialNumbers) trả về kiểu Task<List<string>>.

        /// <summary>
        /// Kiểm tra một Serial Number đã tồn tại trong DB chưa (theo VariantId).
        /// Dùng cho check real-time khi quét mã vạch.
        /// </summary>
        Task<bool> ExistsAsync(string serialNumber, int variantId); // Định nghĩa phương thức bất đồng bộ 'ExistsAsync' với tham số (serialNumber, variantId) trả về kiểu Task<bool>.

        /// <summary>
        /// Lấy danh sách SerialNumber theo ReceiptId và VariantId.
        /// </summary>
        Task<List<string>> GetSerialsByReceiptAndVariantAsync(int receiptId, int variantId); // Định nghĩa phương thức bất đồng bộ 'GetSerialsByReceiptAndVariantAsync' với tham số (receiptId, variantId) trả về kiểu Task<List<string>>.

        Task AddRangeAsync(IEnumerable<ProductSerial> serials); // Định nghĩa phương thức bất đồng bộ 'AddRangeAsync' với tham số (serials) trả về kiểu Task.

        /// <summary>
        /// Lấy chi tiết Serial kèm Variant và Product.
        /// </summary>
        Task<ProductSerial?> GetBySerialNumberAsync(string serialNumber); // Định nghĩa phương thức bất đồng bộ 'GetBySerialNumberAsync' với tham số (serialNumber) trả về kiểu Task<ProductSerial?>.

        /// <summary>
        /// Lấy danh sách ProductSerial theo SerialNumbers (WITH TRACKING để update).
        /// Dùng cho luồng xuất kho — cần ghi nhận trạng thái Sold.
        /// </summary>
        Task<List<ProductSerial>> GetSerialsWithTrackingAsync(List<string> serialNumbers); // Định nghĩa phương thức bất đồng bộ 'GetSerialsWithTrackingAsync' với tham số (serialNumbers) trả về kiểu Task<List<ProductSerial>>.

        /// <summary>
        /// Lấy chi tiết Serial theo Id (WITH TRACKING để update).
        /// </summary>
        Task<ProductSerial?> GetByIdWithTrackingAsync(int id); // Định nghĩa phương thức bất đồng bộ 'GetByIdWithTrackingAsync' với tham số (id) trả về kiểu Task<ProductSerial?>.

        /// <summary>
        /// Lấy N Serials đang Available của một Variant (dùng cho hàng generic khi checkout).
        /// </summary>
        Task<List<ProductSerial>> GetAvailableSerialsByVariantAsync(int variantId, int count); // Định nghĩa phương thức bất đồng bộ 'GetAvailableSerialsByVariantAsync' với tham số (variantId, count) trả về kiểu Task<List<ProductSerial>>.

        /// <summary>
        /// Đếm số Serial Available (Status=0) theo danh sách VariantId — batch query.
        /// </summary>
        Task<Dictionary<int, int>> CountAvailableByVariantIdsAsync(List<int> variantIds); // Định nghĩa phương thức bất đồng bộ 'CountAvailableByVariantIdsAsync' với tham số (variantIds) trả về kiểu Task<Dictionary<int, int>>.

        /// <summary>
        /// Lấy tất cả serials Available theo danh sách VariantId (batch) — dùng cho snapshot kiểm kê.
        /// Trả về (SerialId, VariantId, SerialNumber).
        /// </summary>
        Task<List<(int SerialId, int VariantId, string SerialNumber)>> GetAvailableSerialsBatchAsync(List<int> variantIds); // Định nghĩa phương thức bất đồng bộ 'Task<List<' với tham số (SerialId, VariantId, SerialNumber) trả về kiểu .

        Task SaveChangesAsync(); // Định nghĩa phương thức bất đồng bộ 'SaveChangesAsync' không tham số trả về kiểu Task.

        /// <summary>
        /// Danh sách phân trang ProductSerial với bộ lọc đa điều kiện.
        /// </summary>
        Task<(List<ProductSerial> Items, int TotalCount)> GetPagedListAsync( // Khai báo thành phần cấu trúc nghiệp vụ.
            string? keyword, int? productId, int? variantId, // Khai báo thành phần cấu trúc nghiệp vụ.
            byte? status, DateTime? fromDate, DateTime? toDate, // Khai báo thành phần cấu trúc nghiệp vụ.
            int pageNumber, int pageSize, string? sortBy, bool sortDescending); // Khai báo thành phần cấu trúc nghiệp vụ.

        /// <summary>
        /// Đếm số Serial theo trạng thái (GROUP BY Status). Có thể lọc theo productId hoặc variantId.
        /// </summary>
        Task<Dictionary<byte, int>> GetStatusCountsAsync(int? productId, int? variantId); // Định nghĩa phương thức bất đồng bộ 'GetStatusCountsAsync' với tham số (productId, variantId) trả về kiểu Task<Dictionary<byte, int>>.

        /// <summary>
        /// Lấy chi tiết Serial kèm Variant, Product, ImportReceipt, Supplier (read-only).
        /// </summary>
        Task<ProductSerial?> GetByIdWithDetailsAsync(int id); // Định nghĩa phương thức bất đồng bộ 'GetByIdWithDetailsAsync' với tham số (id) trả về kiểu Task<ProductSerial?>.
    }
}

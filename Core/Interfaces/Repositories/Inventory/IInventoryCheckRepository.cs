using PBL3.Core.Entities; // Nhập namespace chứa các thực thể nghiệp vụ cốt lõi của hệ thống.

namespace PBL3.Core.Interfaces // Định nghĩa namespace PBL3.Core.Interfaces để quản lý thực thể/giao diện.
{
    /// <summary>
    /// Repository interface cho InventoryCheck (Kiểm kê kho hàng).
    /// </summary>
    public interface IInventoryCheckRepository // Định nghĩa giao diện (interface) IInventoryCheckRepository quy định hợp đồng cho tầng dữ liệu.
    {
        /// <summary>
        /// Lấy phiếu kiểm kê theo Id (WithTracking cho update).
        /// </summary>
        Task<InventoryCheck?> GetByIdAsync(int id); // Định nghĩa phương thức bất đồng bộ 'GetByIdAsync' với tham số (id) trả về kiểu Task<InventoryCheck?>.

        /// <summary>
        /// Lấy phiếu kiểm kê theo Id kèm Details + DetailSerials (AsNoTracking, read-only).
        /// </summary>
        Task<InventoryCheck?> GetByIdWithDetailsAsync(int id); // Định nghĩa phương thức bất đồng bộ 'GetByIdWithDetailsAsync' với tham số (id) trả về kiểu Task<InventoryCheck?>.

        /// <summary>
        /// Lấy danh sách phiếu kiểm kê phân trang.
        /// </summary>
        Task<(List<InventoryCheck> Items, int TotalCount)> GetPagedListAsync( // Khai báo thành phần cấu trúc nghiệp vụ.
            string? keyword, // Khai báo thành phần cấu trúc nghiệp vụ.
            byte? status, // Khai báo thành phần cấu trúc nghiệp vụ.
            DateTime? fromDate, // Khai báo thành phần cấu trúc nghiệp vụ.
            DateTime? toDate, // Khai báo thành phần cấu trúc nghiệp vụ.
            Guid? employeeId, // Khai báo thành phần cấu trúc nghiệp vụ.
            int pageNumber, // Khai báo thành phần cấu trúc nghiệp vụ.
            int pageSize, // Khai báo thành phần cấu trúc nghiệp vụ.
            string? sortBy, // Khai báo thành phần cấu trúc nghiệp vụ.
            bool sortDescending); // Khai báo thành phần cấu trúc nghiệp vụ.

        /// <summary>
        /// Lấy mã phiếu kiểm kê cuối cùng theo ngày (để sinh mã tự động KK-yyyyMMdd-NNN).
        /// </summary>
        Task<string?> GetLastCheckCodeByDateAsync(string datePrefix); // Định nghĩa phương thức bất đồng bộ 'GetLastCheckCodeByDateAsync' với tham số (datePrefix) trả về kiểu Task<string?>.

        /// <summary>
        /// Lấy 1 row InventoryCheckDetailSerial theo Id.
        /// </summary>
        Task<InventoryCheckDetailSerial?> GetDetailSerialAsync(int detailSerialId, bool withTracking = false); // Định nghĩa phương thức bất đồng bộ 'GetDetailSerialAsync' với tham số (detailSerialId, false) trả về kiểu Task<InventoryCheckDetailSerial?>.

        /// <summary>
        /// Lấy row InventoryCheckDetailSerial theo CheckId + SerialId (WithTracking để update khi quét).
        /// </summary>
        Task<InventoryCheckDetailSerial?> GetPendingDetailSerialBySerialIdAsync(int checkId, int serialId); // Định nghĩa phương thức bất đồng bộ 'GetPendingDetailSerialBySerialIdAsync' với tham số (checkId, serialId) trả về kiểu Task<InventoryCheckDetailSerial?>.

        /// <summary>
        /// Kiểm tra SerialNumberRaw đã được quét trong phiếu này chưa (chống quét trùng).
        /// </summary>
        Task<bool> IsSerialAlreadyScannedAsync(int checkId, string serialNumberRaw); // Định nghĩa phương thức bất đồng bộ 'IsSerialAlreadyScannedAsync' với tham số (checkId, serialNumberRaw) trả về kiểu Task<bool>.

        /// <summary>
        /// Lấy tất cả rows Pending của phiếu (để chuyển Missing khi submit).
        /// </summary>
        Task<List<InventoryCheckDetailSerial>> GetPendingDetailSerialsAsync(int checkId); // Định nghĩa phương thức bất đồng bộ 'GetPendingDetailSerialsAsync' với tham số (checkId) trả về kiểu Task<List<InventoryCheckDetailSerial>>.

        /// <summary>
        /// Lấy tất cả rows Surplus/UnknownSurplus của phiếu (để xóa khi reject returnToDraft).
        /// </summary>
        Task<List<InventoryCheckDetailSerial>> GetSurplusDetailSerialsAsync(int checkId); // Định nghĩa phương thức bất đồng bộ 'GetSurplusDetailSerialsAsync' với tham số (checkId) trả về kiểu Task<List<InventoryCheckDetailSerial>>.

        /// <summary>
        /// Lấy danh sách InventoryCheckDetailSerial phân trang, lọc theo ScanStatus.
        /// </summary>
        Task<(List<InventoryCheckDetailSerial> Items, int TotalCount)> GetDetailSerialsPagedAsync( // Khai báo thành phần cấu trúc nghiệp vụ.
            int checkId, // Khai báo thành phần cấu trúc nghiệp vụ.
            byte? scanStatus, // Khai báo thành phần cấu trúc nghiệp vụ.
            int? variantId, // Khai báo thành phần cấu trúc nghiệp vụ.
            int pageNumber, // Khai báo thành phần cấu trúc nghiệp vụ.
            int pageSize); // Khai báo thành phần cấu trúc nghiệp vụ.

        /// <summary>
        /// Đếm số lượng theo từng ScanStatus của phiếu (phục vụ dashboard).
        /// </summary>
        Task<Dictionary<byte, int>> GetGroupedCountsByCheckAsync(int checkId); // Định nghĩa phương thức bất đồng bộ 'GetGroupedCountsByCheckAsync' với tham số (checkId) trả về kiểu Task<Dictionary<byte, int>>.

        /// <summary>
        /// Lấy InventoryCheckDetail (WithTracking) để cập nhật counts sau khi quét.
        /// </summary>
        Task<InventoryCheckDetail?> GetDetailByCheckAndVariantAsync(int checkId, int variantId, bool withTracking = false); // Định nghĩa phương thức bất đồng bộ 'GetDetailByCheckAndVariantAsync' với tham số (checkId, variantId, false) trả về kiểu Task<InventoryCheckDetail?>.

        /// <summary>
        /// Lấy tất cả rows Missing kèm ProductSerial hiện tại (để approve - mark Lost).
        /// Trả về kèm tracking để có thể cập nhật ProductSerial.Status.
        /// </summary>
        Task<List<InventoryCheckDetailSerial>> GetMissingDetailSerialsWithSerialAsync(int checkId); // Định nghĩa phương thức bất đồng bộ 'GetMissingDetailSerialsWithSerialAsync' với tham số (checkId) trả về kiểu Task<List<InventoryCheckDetailSerial>>.

        /// <summary>
        /// Lấy tất cả rows Defective kèm ProductSerial (để approve - mark Defective).
        /// </summary>
        Task<List<InventoryCheckDetailSerial>> GetDefectiveDetailSerialsWithSerialAsync(int checkId); // Định nghĩa phương thức bất đồng bộ 'GetDefectiveDetailSerialsWithSerialAsync' với tham số (checkId) trả về kiểu Task<List<InventoryCheckDetailSerial>>.

        Task AddAsync(InventoryCheck check); // Định nghĩa phương thức bất đồng bộ 'AddAsync' với tham số (check) trả về kiểu Task.
        Task AddDetailAsync(InventoryCheckDetail detail); // Định nghĩa phương thức bất đồng bộ 'AddDetailAsync' với tham số (detail) trả về kiểu Task.
        Task AddDetailSerialAsync(InventoryCheckDetailSerial detailSerial); // Định nghĩa phương thức bất đồng bộ 'AddDetailSerialAsync' với tham số (detailSerial) trả về kiểu Task.
        Task AddDetailSerialsAsync(IEnumerable<InventoryCheckDetailSerial> detailSerials); // Định nghĩa phương thức bất đồng bộ 'AddDetailSerialsAsync' với tham số (detailSerials) trả về kiểu Task.
        Task AddAdjustmentLogsAsync(IEnumerable<InventoryAdjustmentLog> logs); // Định nghĩa phương thức bất đồng bộ 'AddAdjustmentLogsAsync' với tham số (logs) trả về kiểu Task.
        Task RemoveDetailSerialsAsync(IEnumerable<InventoryCheckDetailSerial> serials); // Định nghĩa phương thức bất đồng bộ 'RemoveDetailSerialsAsync' với tham số (serials) trả về kiểu Task.
        Task SaveChangesAsync(); // Định nghĩa phương thức bất đồng bộ 'SaveChangesAsync' không tham số trả về kiểu Task.
    }
}

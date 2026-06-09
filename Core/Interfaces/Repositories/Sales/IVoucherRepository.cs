using PBL3.Core.Entities; // Nhập namespace chứa các thực thể nghiệp vụ cốt lõi của hệ thống.

namespace PBL3.Core.Interfaces // Định nghĩa namespace PBL3.Core.Interfaces để quản lý thực thể/giao diện.
{
    /// <summary>
    /// Repository interface cho Voucher &amp; VoucherUsage.
    /// </summary>
    public interface IVoucherRepository // Định nghĩa giao diện (interface) IVoucherRepository quy định hợp đồng cho tầng dữ liệu.
    {
        // ==================== MANAGEMENT CRUD ====================

        /// <summary>
        /// Lấy danh sách voucher phân trang, hỗ trợ lọc theo keyword, trạng thái, date range.
        /// </summary>
        Task<(List<Voucher> Items, int TotalCount)> GetPagedListAsync( // Khai báo thành phần cấu trúc nghiệp vụ.
            string? keyword, // Khai báo thành phần cấu trúc nghiệp vụ.
            string? statusFilter, // Khai báo thành phần cấu trúc nghiệp vụ.
            DateTime? fromDate, // Khai báo thành phần cấu trúc nghiệp vụ.
            DateTime? toDate, // Khai báo thành phần cấu trúc nghiệp vụ.
            int pageNumber, // Khai báo thành phần cấu trúc nghiệp vụ.
            int pageSize, // Khai báo thành phần cấu trúc nghiệp vụ.
            string? sortBy, // Khai báo thành phần cấu trúc nghiệp vụ.
            bool sortDescending); // Khai báo thành phần cấu trúc nghiệp vụ.

        /// <summary>
        /// Lấy voucher theo Id, bao gồm VoucherCategories. Có tracking để update.
        /// </summary>
        Task<Voucher?> GetByIdWithCategoriesAsync(int id); // Định nghĩa phương thức bất đồng bộ 'GetByIdWithCategoriesAsync' với tham số (id) trả về kiểu Task<Voucher?>.

        /// <summary>
        /// Lấy voucher theo Id, không tracking. Dùng cho read-only operations.
        /// </summary>
        Task<Voucher?> GetByIdNoTrackingAsync(int id); // Định nghĩa phương thức bất đồng bộ 'GetByIdNoTrackingAsync' với tham số (id) trả về kiểu Task<Voucher?>.

        /// <summary>
        /// Kiểm tra mã Code đã tồn tại chưa (excludeId bỏ qua chính nó khi update).
        /// </summary>
        Task<bool> IsDuplicateCodeAsync(string code, int? excludeId = null); // Định nghĩa phương thức bất đồng bộ 'IsDuplicateCodeAsync' với tham số (code, null) trả về kiểu Task<bool>.

        /// <summary>
        /// Thêm voucher mới vào context.
        /// </summary>
        Task AddAsync(Voucher voucher); // Định nghĩa phương thức bất đồng bộ 'AddAsync' với tham số (voucher) trả về kiểu Task.

        // ==================== CHECKOUT USAGE ====================

        /// <summary>
        /// Lấy danh sách Voucher theo danh sách mã Code, bao gồm VoucherCategories.
        /// Dùng cho checkout với category restriction check.
        /// </summary>
        Task<List<Voucher>> GetByCodesWithCategoriesAsync(List<string> codes); // Định nghĩa phương thức bất đồng bộ 'GetByCodesWithCategoriesAsync' với tham số (codes) trả về kiểu Task<List<Voucher>>.

        /// <summary>
        /// Lấy danh sách Voucher theo danh sách mã Code (không include categories).
        /// </summary>
        Task<List<Voucher>> GetByCodesAsync(List<string> codes); // Định nghĩa phương thức bất đồng bộ 'GetByCodesAsync' với tham số (codes) trả về kiểu Task<List<Voucher>>.

        /// <summary>
        /// Đếm số lần user đã dùng mỗi voucher trong danh sách.
        /// Trả về Dictionary(VoucherId → số lần dùng).
        /// Dùng thay GetUsedVoucherIdsByUserAsync để hỗ trợ MaxUsesPerUser.
        /// </summary>
        Task<Dictionary<int, int>> GetUserVoucherUsageCountsAsync(Guid userId, List<int> voucherIds); // Định nghĩa phương thức bất đồng bộ 'GetUserVoucherUsageCountsAsync' với tham số (userId, voucherIds) trả về kiểu Task<Dictionary<int, int>>.

        /// <summary>
        /// Kiểm tra danh sách cặp (UserId, VoucherId) đã tồn tại trong VoucherUsages chưa.
        /// Trả về danh sách VoucherId mà User này đã dùng.
        /// </summary>
        Task<List<int>> GetUsedVoucherIdsByUserAsync(Guid userId, List<int> voucherIds); // Định nghĩa phương thức bất đồng bộ 'GetUsedVoucherIdsByUserAsync' với tham số (userId, voucherIds) trả về kiểu Task<List<int>>.

        /// <summary>
        /// Thêm danh sách VoucherUsage vào context.
        /// </summary>
        Task AddUsagesAsync(IEnumerable<VoucherUsage> usages); // Định nghĩa phương thức bất đồng bộ 'AddUsagesAsync' với tham số (usages) trả về kiểu Task.

        /// <summary>
        /// Lấy tất cả voucher active, trong thời hạn hiệu lực, chưa hết số lượng.
        /// Dùng cho popup chọn voucher ở trang Checkout.
        /// </summary>
        Task<List<Voucher>> GetActiveVouchersForCustomerAsync(); // Định nghĩa phương thức bất đồng bộ 'GetActiveVouchersForCustomerAsync' không tham số trả về kiểu Task<List<Voucher>>.

        Task SaveChangesAsync(); // Định nghĩa phương thức bất đồng bộ 'SaveChangesAsync' không tham số trả về kiểu Task.
    }
}

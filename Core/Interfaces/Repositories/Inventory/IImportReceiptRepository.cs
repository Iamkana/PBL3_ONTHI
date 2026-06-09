using PBL3.Core.Entities; // Nhập namespace chứa các thực thể nghiệp vụ cốt lõi của hệ thống.

namespace PBL3.Core.Interfaces // Định nghĩa namespace PBL3.Core.Interfaces để quản lý thực thể/giao diện.
{
    /// <summary>
    /// Repository interface cho ImportReceipt.
    /// </summary>
    public interface IImportReceiptRepository // Định nghĩa giao diện (interface) IImportReceiptRepository quy định hợp đồng cho tầng dữ liệu.
    {
        /// <summary>
        /// Lấy danh sách phiếu nhập có phân trang, tìm kiếm và Include Supplier.
        /// </summary>
        Task<(List<ImportReceipt> Items, int TotalCount)> GetPagedListAsync( // Khai báo thành phần cấu trúc nghiệp vụ.
            string? keyword, // Khai báo thành phần cấu trúc nghiệp vụ.
            DateTime? fromDate, // Khai báo thành phần cấu trúc nghiệp vụ.
            DateTime? toDate, // Khai báo thành phần cấu trúc nghiệp vụ.
            int? supplierId, // Khai báo thành phần cấu trúc nghiệp vụ.
            int pageNumber, // Khai báo thành phần cấu trúc nghiệp vụ.
            int pageSize, // Khai báo thành phần cấu trúc nghiệp vụ.
            string? sortBy, // Khai báo thành phần cấu trúc nghiệp vụ.
            bool sortDescending); // Khai báo thành phần cấu trúc nghiệp vụ.

        /// <summary>
        /// Lấy chi tiết phiếu nhập theo Id, bao gồm Details, Variant, và ProductSerials.
        /// </summary>
        Task<ImportReceipt?> GetByIdWithDetailsAsync(int id); // Định nghĩa phương thức bất đồng bộ 'GetByIdWithDetailsAsync' với tham số (id) trả về kiểu Task<ImportReceipt?>.

        /// <summary>
        /// Lấy mã phiếu nhập cuối cùng theo ngày (để sinh mã tự động).
        /// </summary>
        Task<string?> GetLastReceiptCodeByDateAsync(string datePrefix); // Định nghĩa phương thức bất đồng bộ 'GetLastReceiptCodeByDateAsync' với tham số (datePrefix) trả về kiểu Task<string?>.

        Task AddAsync(ImportReceipt receipt); // Định nghĩa phương thức bất đồng bộ 'AddAsync' với tham số (receipt) trả về kiểu Task.
        Task AddDetailAsync(ImportReceiptDetail detail); // Định nghĩa phương thức bất đồng bộ 'AddDetailAsync' với tham số (detail) trả về kiểu Task.
        Task SaveChangesAsync(); // Định nghĩa phương thức bất đồng bộ 'SaveChangesAsync' không tham số trả về kiểu Task.
    }
}

using PBL3.Core.Entities; // Nhập namespace chứa các thực thể nghiệp vụ cốt lõi của hệ thống.

namespace PBL3.Core.Interfaces // Định nghĩa namespace PBL3.Core.Interfaces để quản lý thực thể/giao diện.
{
    /// <summary>
    /// Repository interface cho Banner (Banner hiển thị trên trang chủ).
    /// </summary>
    public interface IBannerRepository // Định nghĩa giao diện (interface) IBannerRepository quy định hợp đồng cho tầng dữ liệu.
    {
        /// <summary>
        /// Lấy danh sách banner phân trang + tìm kiếm theo Title.
        /// </summary>
        Task<(List<Banner> Items, int TotalCount)> GetPagedListAsync( // Khai báo thành phần cấu trúc nghiệp vụ.
            string? keyword, // Khai báo thành phần cấu trúc nghiệp vụ.
            int pageNumber, // Khai báo thành phần cấu trúc nghiệp vụ.
            int pageSize, // Khai báo thành phần cấu trúc nghiệp vụ.
            string? sortBy, // Khai báo thành phần cấu trúc nghiệp vụ.
            bool sortDescending); // Khai báo thành phần cấu trúc nghiệp vụ.

        /// <summary>
        /// Lấy chi tiết banner theo Id (no tracking).
        /// </summary>
        Task<Banner?> GetByIdAsync(int id); // Định nghĩa phương thức bất đồng bộ 'GetByIdAsync' với tham số (id) trả về kiểu Task<Banner?>.

        /// <summary>
        /// Lấy chi tiết banner theo Id (with tracking) để cập nhật/soft-delete.
        /// </summary>
        Task<Banner?> GetByIdWithTrackingAsync(int id); // Định nghĩa phương thức bất đồng bộ 'GetByIdWithTrackingAsync' với tham số (id) trả về kiểu Task<Banner?>.

        /// <summary>
        /// Lấy danh sách banner đang hiệu lực (IsActive + trong khoảng StartDate/EndDate),
        /// sắp xếp theo SortOrder ASC, Id ASC. Dùng cho trang chủ public.
        /// </summary>
        Task<List<Banner>> GetActiveAsync(DateTime nowUtc); // Định nghĩa phương thức bất đồng bộ 'GetActiveAsync' với tham số (nowUtc) trả về kiểu Task<List<Banner>>.

        Task AddAsync(Banner banner); // Định nghĩa phương thức bất đồng bộ 'AddAsync' với tham số (banner) trả về kiểu Task.
        Task SaveChangesAsync(); // Định nghĩa phương thức bất đồng bộ 'SaveChangesAsync' không tham số trả về kiểu Task.
    }
}

using PBL3.Core.Entities; // Nhập namespace chứa các thực thể nghiệp vụ cốt lõi của hệ thống.

namespace PBL3.Core.Interfaces // Định nghĩa namespace PBL3.Core.Interfaces để quản lý thực thể/giao diện.
{
    /// <summary>
    /// Repository interface cho Customer (Quản lý User).
    /// </summary>
    public interface ICustomerRepository // Định nghĩa giao diện (interface) ICustomerRepository quy định hợp đồng cho tầng dữ liệu.
    {
        /// <summary>
        /// Lấy danh sách khách hàng có phân trang và bộ lọc.
        /// </summary>
        Task<(List<AppUser> Items, int TotalCount)> GetPagedListAsync( // Khai báo thành phần cấu trúc nghiệp vụ.
            string? keyword, // Khai báo thành phần cấu trúc nghiệp vụ.
            bool? isActive, // Khai báo thành phần cấu trúc nghiệp vụ.
            byte? gender, // Khai báo thành phần cấu trúc nghiệp vụ.
            int pageNumber, // Khai báo thành phần cấu trúc nghiệp vụ.
            int pageSize, // Khai báo thành phần cấu trúc nghiệp vụ.
            string? sortBy, // Khai báo thành phần cấu trúc nghiệp vụ.
            bool sortDescending); // Khai báo thành phần cấu trúc nghiệp vụ.

        /// <summary>
        /// Lấy chi tiết khách hàng và profile.
        /// </summary>
        Task<AppUser?> GetByIdWithProfileAsync(Guid id); // Định nghĩa phương thức bất đồng bộ 'GetByIdWithProfileAsync' với tham số (id) trả về kiểu Task<AppUser?>.

        /// <summary>
        /// Lấy danh sách N đơn hàng mới nhất của người dùng.
        /// </summary>
        Task<List<Order>> GetRecentOrdersAsync(Guid userId, int count); // Định nghĩa phương thức bất đồng bộ 'GetRecentOrdersAsync' với tham số (userId, count) trả về kiểu Task<List<Order>>.

        /// <summary>
        /// Kiểm tra xem người dùng có đơn hàng nào chưa hoàn tất không.
        /// (0: Pending, 1: Confirmed, 2: Shipping)
        /// </summary>
        Task<bool> HasPendingOrdersAsync(Guid userId); // Định nghĩa phương thức bất đồng bộ 'HasPendingOrdersAsync' với tham số (userId) trả về kiểu Task<bool>.
    }
}

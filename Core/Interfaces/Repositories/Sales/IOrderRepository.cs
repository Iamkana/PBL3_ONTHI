using PBL3.Core.Entities; // Nhập namespace chứa các thực thể nghiệp vụ cốt lõi của hệ thống.

namespace PBL3.Core.Interfaces // Định nghĩa namespace PBL3.Core.Interfaces để quản lý thực thể/giao diện.
{
    /// <summary>
    /// Repository interface cho Order.
    /// </summary>
    public interface IOrderRepository // Định nghĩa giao diện (interface) IOrderRepository quy định hợp đồng cho tầng dữ liệu.
    {
        IQueryable<Order> GetQueryable(); // Khai báo thành phần cấu trúc nghiệp vụ.
        Task<Order?> GetByIdAsync(int id); // Định nghĩa phương thức bất đồng bộ 'GetByIdAsync' với tham số (id) trả về kiểu Task<Order?>.

        /// <summary>
        /// Lấy chi tiết đơn hàng theo Id, bao gồm Details và Serials (AsNoTracking — dùng cho read-only).
        /// </summary>
        Task<Order?> GetByIdWithDetailsAsync(int id); // Định nghĩa phương thức bất đồng bộ 'GetByIdWithDetailsAsync' với tham số (id) trả về kiểu Task<Order?>.

        /// <summary>
        /// Lấy chi tiết đơn hàng theo Id kèm tracking (dùng cho các thao tác ghi: xuất kho, cập nhật trạng thái...).
        /// </summary>
        Task<Order?> GetByIdWithDetailsTrackedAsync(int id); // Định nghĩa phương thức bất đồng bộ 'GetByIdWithDetailsTrackedAsync' với tham số (id) trả về kiểu Task<Order?>.

        Task<string?> GetLastOrderCodeByDateAsync(string datePrefix); // Định nghĩa phương thức bất đồng bộ 'GetLastOrderCodeByDateAsync' với tham số (datePrefix) trả về kiểu Task<string?>.

        /// <summary>
        /// Lấy danh sách các đơn POS đang lưu nháp bởi một nhân viên.
        /// </summary>
        Task<List<Order>> GetDraftsByEmployeeAsync(Guid employeeId); // Định nghĩa phương thức bất đồng bộ 'GetDraftsByEmployeeAsync' với tham số (employeeId) trả về kiểu Task<List<Order>>.

        /// <summary>
        /// Tính tổng Quantity đã đặt theo VariantId cho các đơn Active (Status 0 hoặc 1).
        /// Dùng cho Virtual Inventory Hold — 1 query batch, không N+1.
        /// </summary>
        Task<Dictionary<int, int>> GetActiveOrderQuantitiesByVariantIdsAsync(List<int> variantIds); // Định nghĩa phương thức bất đồng bộ 'GetActiveOrderQuantitiesByVariantIdsAsync' với tham số (variantIds) trả về kiểu Task<Dictionary<int, int>>.

        Task AddAsync(Order order); // Định nghĩa phương thức bất đồng bộ 'AddAsync' với tham số (order) trả về kiểu Task.
        Task SaveChangesAsync(); // Định nghĩa phương thức bất đồng bộ 'SaveChangesAsync' không tham số trả về kiểu Task.
    }
}

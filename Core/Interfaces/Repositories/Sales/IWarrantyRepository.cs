using PBL3.Core.Entities; // Nhập namespace chứa các thực thể nghiệp vụ cốt lõi của hệ thống.

namespace PBL3.Core.Interfaces // Định nghĩa namespace PBL3.Core.Interfaces để quản lý thực thể/giao diện.
{
    /// <summary>
    /// Repository interface cho Warranty.
    /// </summary>
    public interface IWarrantyRepository // Định nghĩa giao diện (interface) IWarrantyRepository quy định hợp đồng cho tầng dữ liệu.
    {
        /// <summary>
        /// Lấy danh sách bảo hành active (status != Claimed) của một serial, sắp xếp theo EndDate giảm dần.
        /// </summary>
        Task<List<Warranty>> GetActiveBySerialIdAsync(int serialId); // Định nghĩa phương thức bất đồng bộ 'GetActiveBySerialIdAsync' với tham số (serialId) trả về kiểu Task<List<Warranty>>.

        /// <summary>
        /// Lấy bảo hành theo Id, có tracking để update.
        /// </summary>
        Task<Warranty?> GetByIdWithTrackingAsync(int id); // Định nghĩa phương thức bất đồng bộ 'GetByIdWithTrackingAsync' với tham số (id) trả về kiểu Task<Warranty?>.

        Task AddAsync(Warranty warranty); // Định nghĩa phương thức bất đồng bộ 'AddAsync' với tham số (warranty) trả về kiểu Task.
        Task AddRangeAsync(IEnumerable<Warranty> warranties); // Định nghĩa phương thức bất đồng bộ 'AddRangeAsync' với tham số (warranties) trả về kiểu Task.
        Task SaveChangesAsync(); // Định nghĩa phương thức bất đồng bộ 'SaveChangesAsync' không tham số trả về kiểu Task.
    }
}

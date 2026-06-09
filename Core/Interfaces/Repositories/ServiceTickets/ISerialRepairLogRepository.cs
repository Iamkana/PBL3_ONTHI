using PBL3.Core.Entities; // Nhập namespace chứa các thực thể nghiệp vụ cốt lõi của hệ thống.

namespace PBL3.Core.Interfaces // Định nghĩa namespace PBL3.Core.Interfaces để quản lý thực thể/giao diện.
{
    /// <summary>
    /// Repository interface cho SerialRepairLog.
    /// </summary>
    public interface ISerialRepairLogRepository // Định nghĩa giao diện (interface) ISerialRepairLogRepository quy định hợp đồng cho tầng dữ liệu.
    {
        /// <summary>
        /// Lấy danh sách lịch sửa chữa của một serial (history lâu dài).
        /// </summary>
        Task<List<SerialRepairLog>> GetBySerialIdAsync(int serialId); // Định nghĩa phương thức bất đồng bộ 'GetBySerialIdAsync' với tham số (serialId) trả về kiểu Task<List<SerialRepairLog>>.

        /// <summary>
        /// Lấy log theo Id, với tracking để update.
        /// </summary>
        Task<SerialRepairLog?> GetByIdWithTrackingAsync(int id); // Định nghĩa phương thức bất đồng bộ 'GetByIdWithTrackingAsync' với tham số (id) trả về kiểu Task<SerialRepairLog?>.

        Task AddAsync(SerialRepairLog log); // Định nghĩa phương thức bất đồng bộ 'AddAsync' với tham số (log) trả về kiểu Task.
        Task SaveChangesAsync(); // Định nghĩa phương thức bất đồng bộ 'SaveChangesAsync' không tham số trả về kiểu Task.
    }
}

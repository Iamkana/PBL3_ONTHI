using PBL3.Core.Entities; // Nhập namespace chứa các thực thể nghiệp vụ cốt lõi của hệ thống.

namespace PBL3.Core.Interfaces // Định nghĩa namespace PBL3.Core.Interfaces để quản lý thực thể/giao diện.
{
    /// <summary>
    /// Repository interface cho UserAddress.
    /// </summary>
    public interface IUserAddressRepository // Định nghĩa giao diện (interface) IUserAddressRepository quy định hợp đồng cho tầng dữ liệu.
    {
        Task<UserAddress?> GetByIdAsync(int id); // Định nghĩa phương thức bất đồng bộ 'GetByIdAsync' với tham số (id) trả về kiểu Task<UserAddress?>.
        Task<List<UserAddress>> GetByUserIdAsync(Guid userId); // Định nghĩa phương thức bất đồng bộ 'GetByUserIdAsync' với tham số (userId) trả về kiểu Task<List<UserAddress>>.
        Task AddAsync(UserAddress address); // Định nghĩa phương thức bất đồng bộ 'AddAsync' với tham số (address) trả về kiểu Task.
        Task ClearUserDefaultsAsync(Guid userId); // Định nghĩa phương thức bất đồng bộ 'ClearUserDefaultsAsync' với tham số (userId) trả về kiểu Task.
        Task SaveChangesAsync(); // Định nghĩa phương thức bất đồng bộ 'SaveChangesAsync' không tham số trả về kiểu Task.
    }
}

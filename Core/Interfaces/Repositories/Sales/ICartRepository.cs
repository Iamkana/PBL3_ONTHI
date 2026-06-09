using PBL3.Core.Entities; // Nhập namespace chứa các thực thể nghiệp vụ cốt lõi của hệ thống.

namespace PBL3.Core.Interfaces // Định nghĩa namespace PBL3.Core.Interfaces để quản lý thực thể/giao diện.
{
    /// <summary>
    /// Repository interface cho Cart.
    /// </summary>
    public interface ICartRepository // Định nghĩa giao diện (interface) ICartRepository quy định hợp đồng cho tầng dữ liệu.
    {
        Task<List<Cart>> GetCartItemsByUserAsync(Guid userId); // Định nghĩa phương thức bất đồng bộ 'GetCartItemsByUserAsync' với tham số (userId) trả về kiểu Task<List<Cart>>.
        Task<List<Cart>> GetCartItemsWithTrackingAsync(Guid userId); // Định nghĩa phương thức bất đồng bộ 'GetCartItemsWithTrackingAsync' với tham số (userId) trả về kiểu Task<List<Cart>>.

        /// <summary>
        /// Lấy 1 item trong giỏ theo Id (WITH TRACKING để update/delete).
        /// </summary>
        Task<Cart?> GetCartItemAsync(int cartItemId, Guid userId); // Định nghĩa phương thức bất đồng bộ 'GetCartItemAsync' với tham số (cartItemId, userId) trả về kiểu Task<Cart?>.

        /// <summary>
        /// Tìm item trong giỏ theo UserId + VariantId (WITH TRACKING để cộng dồn Quantity).
        /// </summary>
        Task<Cart?> FindByUserAndVariantAsync(Guid userId, int variantId); // Định nghĩa phương thức bất đồng bộ 'FindByUserAndVariantAsync' với tham số (userId, variantId) trả về kiểu Task<Cart?>.

        Task AddAsync(Cart cart); // Định nghĩa phương thức bất đồng bộ 'AddAsync' với tham số (cart) trả về kiểu Task.
        void Remove(Cart cart); // Định nghĩa phương thức bất đồng bộ 'Remove' với tham số (cart) trả về kiểu void.
        void RemoveRange(IEnumerable<Cart> carts); // Định nghĩa phương thức bất đồng bộ 'RemoveRange' với tham số (carts) trả về kiểu void.
        Task SaveChangesAsync(); // Định nghĩa phương thức bất đồng bộ 'SaveChangesAsync' không tham số trả về kiểu Task.
    }
}

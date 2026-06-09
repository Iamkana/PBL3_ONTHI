using PBL3.Core.Entities; // Nhập namespace chứa các thực thể nghiệp vụ cốt lõi của hệ thống.

namespace PBL3.Core.Interfaces // Định nghĩa namespace PBL3.Core.Interfaces để quản lý thực thể/giao diện.
{
    /// <summary>
    /// Repository interface cho Manufacturer (Hãng sản xuất).
    /// </summary>
    public interface IManufacturerRepository // Định nghĩa giao diện (interface) IManufacturerRepository quy định hợp đồng cho tầng dữ liệu.
    {
        /// <summary>
        /// Lấy danh sách hãng sản xuất có phân trang và tìm kiếm.
        /// </summary>
        Task<(List<Manufacturer> Items, int TotalCount)> GetPagedListAsync( // Khai báo thành phần cấu trúc nghiệp vụ.
            string? keyword, // Khai báo thành phần cấu trúc nghiệp vụ.
            int pageNumber, // Khai báo thành phần cấu trúc nghiệp vụ.
            int pageSize, // Khai báo thành phần cấu trúc nghiệp vụ.
            string? sortBy, // Khai báo thành phần cấu trúc nghiệp vụ.
            bool sortDescending); // Khai báo thành phần cấu trúc nghiệp vụ.

        /// <summary>
        /// Lấy chi tiết hãng sản xuất theo Id.
        /// </summary>
        Task<Manufacturer?> GetByIdAsync(int id); // Định nghĩa phương thức bất đồng bộ 'GetByIdAsync' với tham số (id) trả về kiểu Task<Manufacturer?>.

        /// <summary>
        /// Lấy danh sách tất cả hãng (Id + Name) dùng cho dropdown.
        /// </summary>
        Task<List<Manufacturer>> GetAllActiveAsync(); // Định nghĩa phương thức bất đồng bộ 'GetAllActiveAsync' không tham số trả về kiểu Task<List<Manufacturer>>.

        /// <summary>
        /// Kiểm tra hãng có sản phẩm nào đang active không (dùng cho logic xoá).
        /// </summary>
        Task<bool> HasProductsAsync(int manufacturerId); // Định nghĩa phương thức bất đồng bộ 'HasProductsAsync' với tham số (manufacturerId) trả về kiểu Task<bool>.

        /// <summary>
        /// Kiểm tra tên hãng đã tồn tại chưa (tránh trùng lặp).
        /// </summary>
        Task<bool> IsDuplicateNameAsync(string name, int? excludeId = null); // Định nghĩa phương thức bất đồng bộ 'IsDuplicateNameAsync' với tham số (name, null) trả về kiểu Task<bool>.

        Task AddAsync(Manufacturer manufacturer); // Định nghĩa phương thức bất đồng bộ 'AddAsync' với tham số (manufacturer) trả về kiểu Task.
        Task SaveChangesAsync(); // Định nghĩa phương thức bất đồng bộ 'SaveChangesAsync' không tham số trả về kiểu Task.
    }
}

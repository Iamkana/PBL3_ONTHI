using PBL3.Core.Entities; // Nhập namespace chứa các thực thể nghiệp vụ cốt lõi của hệ thống.

namespace PBL3.Core.Interfaces // Định nghĩa namespace PBL3.Core.Interfaces để quản lý thực thể/giao diện.
{
    /// <summary>
    /// Repository interface cho Employee (Type = 1).
    /// </summary>
    public interface IEmployeeRepository // Định nghĩa giao diện (interface) IEmployeeRepository quy định hợp đồng cho tầng dữ liệu.
    {
        Task<(List<AppUser> Items, int TotalCount)> GetPagedListAsync( // Khai báo thành phần cấu trúc nghiệp vụ.
            string? keyword, // Khai báo thành phần cấu trúc nghiệp vụ.
            bool? isActive, // Khai báo thành phần cấu trúc nghiệp vụ.
            byte? gender, // Khai báo thành phần cấu trúc nghiệp vụ.
            int pageNumber, // Khai báo thành phần cấu trúc nghiệp vụ.
            int pageSize, // Khai báo thành phần cấu trúc nghiệp vụ.
            string? sortBy, // Khai báo thành phần cấu trúc nghiệp vụ.
            bool sortDescending); // Khai báo thành phần cấu trúc nghiệp vụ.

        Task<AppUser?> GetByIdWithProfileAsync(Guid id); // Định nghĩa phương thức bất đồng bộ 'GetByIdWithProfileAsync' với tham số (id) trả về kiểu Task<AppUser?>.
    }
}

using System; // Nhập thư viện hệ thống cơ bản.
using System.Collections.Generic; // Nhập thư viện làm việc với các kiểu danh sách, bộ sưu tập.
using PBL3.Shared.DTOs.Products; // Nhập (import) namespace PBL3.Shared.DTOs.Products để sử dụng các thành phần bên trong.
using PBL3.Shared.DTOs.Sale; // Nhập (import) namespace PBL3.Shared.DTOs.Sale để sử dụng các thành phần bên trong.

namespace PBL3.Shared.DTOs.Customers // Định nghĩa namespace PBL3.Shared.DTOs.Customers quản lý cấu trúc code truyền tải và validator.
{
    public class CustomerDetailDto : CustomerDto // Định nghĩa lớp DTO truyền tải dữ liệu CustomerDetailDto triển khai/kế thừa CustomerDto.
    {
        // Simple representation of orders to avoid circular dependency
        // In reality, this would use an OrderLiteDto or similar depending on existing Sales DTOs
        public List<CustomerOrderHistoryDto> RecentOrders { get; set; } = new(); // Thuộc tính RecentOrders kiểu dữ liệu List<CustomerOrderHistoryDto> lưu trữ thông tin truyền tải với giá trị mặc định là new().
        public List<CustomerCartItemDto> CartItems { get; set; } = new(); // Thuộc tính CartItems kiểu dữ liệu List<CustomerCartItemDto> lưu trữ thông tin truyền tải với giá trị mặc định là new().
    }
}

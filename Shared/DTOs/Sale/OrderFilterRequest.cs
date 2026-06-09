using System; // Nhập thư viện hệ thống cơ bản.
using PBL3.Shared.DTOs.Common; // Nhập (import) namespace PBL3.Shared.DTOs.Common để sử dụng các thành phần bên trong.

namespace PBL3.Shared.DTOs.Sale // Định nghĩa namespace PBL3.Shared.DTOs.Sale quản lý cấu trúc code truyền tải và validator.
{
    public class OrderFilterRequest // Định nghĩa lớp DTO truyền tải dữ liệu OrderFilterRequest.
    {
        public string? Keyword { get; set; } // Thuộc tính Keyword kiểu dữ liệu string? lưu trữ thông tin truyền tải.
        public int? Status { get; set; } // Thuộc tính Status kiểu dữ liệu int? lưu trữ thông tin truyền tải.
        public byte? MinStatus { get; set; } // Thuộc tính MinStatus kiểu dữ liệu byte? lưu trữ thông tin truyền tải.
        public byte? MaxStatus { get; set; } // Thuộc tính MaxStatus kiểu dữ liệu byte? lưu trữ thông tin truyền tải.
        public DateTime? FromDate { get; set; } // Thuộc tính FromDate kiểu dữ liệu DateTime? lưu trữ thông tin truyền tải.
        public DateTime? ToDate { get; set; } // Thuộc tính ToDate kiểu dữ liệu DateTime? lưu trữ thông tin truyền tải.
        public int PageIndex { get; set; } = 1; // Thuộc tính PageIndex kiểu dữ liệu int lưu trữ thông tin truyền tải với giá trị mặc định là 1.
        public int PageSize { get; set; } = 10; // Thuộc tính PageSize kiểu dữ liệu int lưu trữ thông tin truyền tải với giá trị mặc định là 10.
    }
}

namespace PBL3.Shared.DTOs.Inventory // Định nghĩa namespace PBL3.Shared.DTOs.Inventory quản lý cấu trúc code truyền tải và validator.
{
    public class ProductSerialFilterRequest // Định nghĩa lớp DTO truyền tải dữ liệu ProductSerialFilterRequest.
    {
        public string? Keyword { get; set; } // Thuộc tính Keyword kiểu dữ liệu string? lưu trữ thông tin truyền tải.
        public int? ProductId { get; set; } // Thuộc tính ProductId kiểu dữ liệu int? lưu trữ thông tin truyền tải.
        public int? VariantId { get; set; } // Thuộc tính VariantId kiểu dữ liệu int? lưu trữ thông tin truyền tải.
        public byte? Status { get; set; } // Thuộc tính Status kiểu dữ liệu byte? lưu trữ thông tin truyền tải.
        public DateTime? FromDate { get; set; } // Thuộc tính FromDate kiểu dữ liệu DateTime? lưu trữ thông tin truyền tải.
        public DateTime? ToDate { get; set; } // Thuộc tính ToDate kiểu dữ liệu DateTime? lưu trữ thông tin truyền tải.
        public int PageNumber { get; set; } = 1; // Thuộc tính PageNumber kiểu dữ liệu int lưu trữ thông tin truyền tải với giá trị mặc định là 1.
        public int PageSize { get; set; } = 10; // Thuộc tính PageSize kiểu dữ liệu int lưu trữ thông tin truyền tải với giá trị mặc định là 10.
        public string? SortBy { get; set; } // Thuộc tính SortBy kiểu dữ liệu string? lưu trữ thông tin truyền tải.
        public bool SortDescending { get; set; } = true; // Thuộc tính SortDescending kiểu dữ liệu bool lưu trữ thông tin truyền tải với giá trị mặc định là true.
    }
}

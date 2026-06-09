namespace PBL3.Shared.DTOs.Vouchers // Định nghĩa namespace PBL3.Shared.DTOs.Vouchers quản lý cấu trúc code truyền tải và validator.
{
    /// <summary>
    /// Bộ lọc cho danh sách voucher.
    /// StatusFilter: null/""=Tất cả, "active"=Đang hoạt động, "upcoming"=Sắp diễn ra,
    ///               "expired"=Hết hạn, "exhausted"=Hết lượt dùng, "paused"=Tạm dừng
    /// </summary>
    public class VoucherFilterRequest // Định nghĩa lớp DTO truyền tải dữ liệu VoucherFilterRequest.
    {
        public string? Keyword { get; set; } // Thuộc tính Keyword kiểu dữ liệu string? lưu trữ thông tin truyền tải.
        public string? StatusFilter { get; set; } // Thuộc tính StatusFilter kiểu dữ liệu string? lưu trữ thông tin truyền tải.
        public DateTime? FromDate { get; set; } // Thuộc tính FromDate kiểu dữ liệu DateTime? lưu trữ thông tin truyền tải.
        public DateTime? ToDate { get; set; } // Thuộc tính ToDate kiểu dữ liệu DateTime? lưu trữ thông tin truyền tải.
        public int PageNumber { get; set; } = 1; // Thuộc tính PageNumber kiểu dữ liệu int lưu trữ thông tin truyền tải với giá trị mặc định là 1.
        public int PageSize { get; set; } = 10; // Thuộc tính PageSize kiểu dữ liệu int lưu trữ thông tin truyền tải với giá trị mặc định là 10.
        public string? SortBy { get; set; } // Thuộc tính SortBy kiểu dữ liệu string? lưu trữ thông tin truyền tải.
        public bool SortDescending { get; set; } // Thuộc tính SortDescending kiểu dữ liệu bool lưu trữ thông tin truyền tải.
    }
}

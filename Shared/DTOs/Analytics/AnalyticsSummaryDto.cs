namespace PBL3.Shared.DTOs.Analytics // Định nghĩa namespace PBL3.Shared.DTOs.Analytics quản lý cấu trúc code truyền tải và validator.
{
    public class AnalyticsSummaryDto // Định nghĩa lớp DTO truyền tải dữ liệu AnalyticsSummaryDto.
    {
        public decimal TotalRevenue { get; set; } // Thuộc tính TotalRevenue kiểu dữ liệu decimal lưu trữ thông tin truyền tải.
        public decimal GrossProfit { get; set; } // Thuộc tính GrossProfit kiểu dữ liệu decimal lưu trữ thông tin truyền tải.
        public int TotalOrders { get; set; } // Thuộc tính TotalOrders kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public int CancelledOrders { get; set; } // Thuộc tính CancelledOrders kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public decimal CancelRate { get; set; } // Thuộc tính CancelRate kiểu dữ liệu decimal lưu trữ thông tin truyền tải.
        public decimal AverageOrderValue { get; set; } // Thuộc tính AverageOrderValue kiểu dữ liệu decimal lưu trữ thông tin truyền tải.
        public int NewCustomers { get; set; } // Thuộc tính NewCustomers kiểu dữ liệu int lưu trữ thông tin truyền tải.

        public int PayCod { get; set; } // Thuộc tính PayCod kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public int PayBanking { get; set; } // Thuộc tính PayBanking kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public int PayVnPay { get; set; } // Thuộc tính PayVnPay kiểu dữ liệu int lưu trữ thông tin truyền tải.
    }
}

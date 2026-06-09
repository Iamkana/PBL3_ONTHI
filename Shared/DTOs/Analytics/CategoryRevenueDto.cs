namespace PBL3.Shared.DTOs.Analytics // Định nghĩa namespace PBL3.Shared.DTOs.Analytics quản lý cấu trúc code truyền tải và validator.
{
    public class CategoryRevenueDto // Định nghĩa lớp DTO truyền tải dữ liệu CategoryRevenueDto.
    {
        public string CategoryName { get; set; } = string.Empty; // Thuộc tính CategoryName kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public int OrderCount { get; set; } // Thuộc tính OrderCount kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public int UnitsSold { get; set; } // Thuộc tính UnitsSold kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public decimal Revenue { get; set; } // Thuộc tính Revenue kiểu dữ liệu decimal lưu trữ thông tin truyền tải.
    }
}

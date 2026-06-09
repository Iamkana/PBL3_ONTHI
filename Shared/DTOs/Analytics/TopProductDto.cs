namespace PBL3.Shared.DTOs.Analytics // Định nghĩa namespace PBL3.Shared.DTOs.Analytics quản lý cấu trúc code truyền tải và validator.
{
    public class TopProductDto // Định nghĩa lớp DTO truyền tải dữ liệu TopProductDto.
    {
        public string ProductName { get; set; } = string.Empty; // Thuộc tính ProductName kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public string VariantName { get; set; } = string.Empty; // Thuộc tính VariantName kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public int UnitsSold { get; set; } // Thuộc tính UnitsSold kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public decimal Revenue { get; set; } // Thuộc tính Revenue kiểu dữ liệu decimal lưu trữ thông tin truyền tải.
    }
}

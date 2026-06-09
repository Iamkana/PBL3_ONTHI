namespace PBL3.Shared.DTOs.Suppliers // Định nghĩa namespace PBL3.Shared.DTOs.Suppliers quản lý cấu trúc code truyền tải và validator.
{
    /// <summary>
    /// DTO hiển thị thông tin nhà cung cấp.
    /// </summary>
    public class SupplierDto // Định nghĩa lớp DTO truyền tải dữ liệu SupplierDto.
    {
        public int Id { get; set; } // Thuộc tính Id kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public string Name { get; set; } = string.Empty; // Thuộc tính Name kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public string? ContactPerson { get; set; } // Thuộc tính ContactPerson kiểu dữ liệu string? lưu trữ thông tin truyền tải.
        public string PhoneNumber { get; set; } = string.Empty; // Thuộc tính PhoneNumber kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public string? Email { get; set; } // Thuộc tính Email kiểu dữ liệu string? lưu trữ thông tin truyền tải.
        public string? Address { get; set; } // Thuộc tính Address kiểu dữ liệu string? lưu trữ thông tin truyền tải.
        public string? TaxCode { get; set; } // Thuộc tính TaxCode kiểu dữ liệu string? lưu trữ thông tin truyền tải.
        public DateTime CreatedDate { get; set; } // Thuộc tính CreatedDate kiểu dữ liệu DateTime lưu trữ thông tin truyền tải.
    }
}

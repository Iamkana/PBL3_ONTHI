namespace PBL3.Shared.DTOs.ServiceTickets // Định nghĩa namespace PBL3.Shared.DTOs.ServiceTickets quản lý cấu trúc code truyền tải và validator.
{
    public class QuotationCreateDto // Định nghĩa lớp DTO truyền tải dữ liệu QuotationCreateDto.
    {
        public decimal LaborCost { get; set; } // Thuộc tính LaborCost kiểu dữ liệu decimal lưu trữ thông tin truyền tải.
        public List<QuotationItemCreateDto> Items { get; set; } = new(); // Thuộc tính Items kiểu dữ liệu List<QuotationItemCreateDto> lưu trữ thông tin truyền tải với giá trị mặc định là new().
    }
}

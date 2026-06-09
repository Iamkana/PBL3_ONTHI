namespace PBL3.Shared.DTOs.Common // Định nghĩa namespace PBL3.Shared.DTOs.Common quản lý cấu trúc code truyền tải và validator.
{
    public class EmployeeDto // Định nghĩa lớp DTO truyền tải dữ liệu EmployeeDto.
    {
        public Guid Id { get; set; } // Thuộc tính Id kiểu dữ liệu Guid lưu trữ thông tin truyền tải.
        public string FullName { get; set; } = string.Empty; // Thuộc tính FullName kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
    }
}

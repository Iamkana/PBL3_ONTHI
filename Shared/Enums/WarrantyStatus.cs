namespace PBL3.Shared.Enums // Định nghĩa namespace PBL3.Shared.Enums quản lý cấu trúc code truyền tải và validator.
{
    public enum WarrantyStatus : byte // Định nghĩa kiểu liệt kê (enum) WarrantyStatus kiểu byte đại diện cho các trạng thái trong hệ thống.
    {
        Active = 0, // Giá trị liệt kê 'Active' có giá trị nguyên là 0.
        Expired = 1, // Giá trị liệt kê 'Expired' có giá trị nguyên là 1.
        Claimed = 2 // Giá trị liệt kê 'Claimed' có giá trị nguyên là 2.
    }
}

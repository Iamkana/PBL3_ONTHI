namespace PBL3.Shared.Enums // Định nghĩa namespace PBL3.Shared.Enums quản lý cấu trúc code truyền tải và validator.
{
    public enum OrderStatus : byte // Định nghĩa kiểu liệt kê (enum) OrderStatus kiểu byte đại diện cho các trạng thái trong hệ thống.
    {
        Pending = 0, // Giá trị liệt kê 'Pending' có giá trị nguyên là 0.
        Confirmed = 1, // Giá trị liệt kê 'Confirmed' có giá trị nguyên là 1.
        Exported = 2, // Giá trị liệt kê 'Exported' có giá trị nguyên là 2.
        Success = 3, // Giá trị liệt kê 'Success' có giá trị nguyên là 3.
        Cancelled = 4, // Giá trị liệt kê 'Cancelled' có giá trị nguyên là 4.
        Returned = 5, // Giá trị liệt kê 'Returned' có giá trị nguyên là 5.
        PosDraft = 6 // Giá trị liệt kê 'PosDraft' có giá trị nguyên là 6.
    }
}

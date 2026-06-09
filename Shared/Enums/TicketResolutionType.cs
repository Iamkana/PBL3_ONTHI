namespace PBL3.Shared.Enums; // Định nghĩa namespace PBL3.Shared.Enums quản lý cấu trúc code truyền tải và validator.

public enum TicketResolutionType : byte // Định nghĩa kiểu liệt kê (enum) TicketResolutionType kiểu byte đại diện cho các trạng thái trong hệ thống.
{
    Pending = 0, // Giá trị liệt kê 'Pending' có giá trị nguyên là 0.
    InternalRepair = 1, // Giá trị liệt kê 'InternalRepair' có giá trị nguyên là 1.
    Rma = 2, // Giá trị liệt kê 'Rma' có giá trị nguyên là 2.
    Swap = 3, // Giá trị liệt kê 'Swap' có giá trị nguyên là 3.
    PaidRepair = 4, // Giá trị liệt kê 'PaidRepair' có giá trị nguyên là 4.
    Rejected = 5, // Giá trị liệt kê 'Rejected' có giá trị nguyên là 5.
    Cancelled = 6 // Giá trị liệt kê 'Cancelled' có giá trị nguyên là 6.
}

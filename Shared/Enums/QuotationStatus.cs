namespace PBL3.Shared.Enums; // Định nghĩa namespace PBL3.Shared.Enums quản lý cấu trúc code truyền tải và validator.

public enum QuotationStatus : byte // Định nghĩa kiểu liệt kê (enum) QuotationStatus kiểu byte đại diện cho các trạng thái trong hệ thống.
{
    Pending = 0, // Giá trị liệt kê 'Pending' có giá trị nguyên là 0.
    Accepted = 1, // Giá trị liệt kê 'Accepted' có giá trị nguyên là 1.
    Rejected = 2, // Giá trị liệt kê 'Rejected' có giá trị nguyên là 2.
    Superseded = 3 // Giá trị liệt kê 'Superseded' có giá trị nguyên là 3.
}

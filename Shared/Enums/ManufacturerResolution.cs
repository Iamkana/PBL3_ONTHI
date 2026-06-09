namespace PBL3.Shared.Enums; // Định nghĩa namespace PBL3.Shared.Enums quản lý cấu trúc code truyền tải và validator.

public enum ManufacturerResolution : byte // Định nghĩa kiểu liệt kê (enum) ManufacturerResolution kiểu byte đại diện cho các trạng thái trong hệ thống.
{
    Pending = 0, // Giá trị liệt kê 'Pending' có giá trị nguyên là 0.
    Repaired = 1, // Giá trị liệt kê 'Repaired' có giá trị nguyên là 1.
    Replaced = 2, // Giá trị liệt kê 'Replaced' có giá trị nguyên là 2.
    Refused = 3 // Giá trị liệt kê 'Refused' có giá trị nguyên là 3.
}

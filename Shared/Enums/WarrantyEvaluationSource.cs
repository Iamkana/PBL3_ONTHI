namespace PBL3.Shared.Enums; // Định nghĩa namespace PBL3.Shared.Enums quản lý cấu trúc code truyền tải và validator.

public enum WarrantyEvaluationSource : byte // Định nghĩa kiểu liệt kê (enum) WarrantyEvaluationSource kiểu byte đại diện cho các trạng thái trong hệ thống.
{
    WarrantyRow = 0, // Giá trị liệt kê 'WarrantyRow' có giá trị nguyên là 0.
    ComputedFromSoldDate = 1, // Giá trị liệt kê 'ComputedFromSoldDate' có giá trị nguyên là 1.
    NoWarranty = 2 // Giá trị liệt kê 'NoWarranty' có giá trị nguyên là 2.
}

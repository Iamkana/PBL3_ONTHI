namespace PBL3.Shared.Enums // Định nghĩa namespace PBL3.Shared.Enums quản lý cấu trúc code truyền tải và validator.
{
    /// <summary>
    /// Trạng thái của phiếu kiểm kê kho hàng.
    /// </summary>
    public enum InventoryCheckStatus : byte // Định nghĩa kiểu liệt kê (enum) InventoryCheckStatus kiểu byte đại diện cho các trạng thái trong hệ thống.
    {
        /// <summary>Đang soạn thảo / kiểm đếm.</summary>
        Draft = 0, // Giá trị liệt kê 'Draft' có giá trị nguyên là 0.

        /// <summary>Đã gửi, đang chờ Admin phê duyệt.</summary>
        AwaitingApproval = 1, // Giá trị liệt kê 'AwaitingApproval' có giá trị nguyên là 1.

        /// <summary>Đã phê duyệt và cân bằng kho hoàn tất.</summary>
        Completed = 2, // Giá trị liệt kê 'Completed' có giá trị nguyên là 2.

        /// <summary>Đã hủy.</summary>
        Cancelled = 3 // Giá trị liệt kê 'Cancelled' có giá trị nguyên là 3.
    }
}

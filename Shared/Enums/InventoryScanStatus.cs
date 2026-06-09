namespace PBL3.Shared.Enums // Định nghĩa namespace PBL3.Shared.Enums quản lý cấu trúc code truyền tải và validator.
{
    /// <summary>
    /// Trạng thái của một mã Serial trong phiếu kiểm kê.
    /// </summary>
    public enum InventoryScanStatus : byte // Định nghĩa kiểu liệt kê (enum) InventoryScanStatus kiểu byte đại diện cho các trạng thái trong hệ thống.
    {
        /// <summary>Nằm trong snapshot nhưng chưa được quét (trạng thái ban đầu sau chốt).</summary>
        Pending = 0, // Giá trị liệt kê 'Pending' có giá trị nguyên là 0.

        /// <summary>Quét thấy, trùng khớp với snapshot Available.</summary>
        Matched = 1, // Giá trị liệt kê 'Matched' có giá trị nguyên là 1.

        /// <summary>Không quét thấy thực tế (phát sinh khi submit phiếu).</summary>
        Missing = 2, // Giá trị liệt kê 'Missing' có giá trị nguyên là 2.

        /// <summary>Quét thấy nhưng Serial tồn tại trong DB với trạng thái không phải Available (Sold/Reserved/...).</summary>
        Surplus = 3, // Giá trị liệt kê 'Surplus' có giá trị nguyên là 3.

        /// <summary>Quét thấy nhưng Serial hoàn toàn không tồn tại trong DB.</summary>
        UnknownSurplus = 4, // Giá trị liệt kê 'UnknownSurplus' có giá trị nguyên là 4.

        /// <summary>Được nhân viên đánh dấu là hàng lỗi vật lý.</summary>
        Defective = 5 // Giá trị liệt kê 'Defective' có giá trị nguyên là 5.
    }
}

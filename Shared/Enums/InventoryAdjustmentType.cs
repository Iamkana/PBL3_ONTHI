namespace PBL3.Shared.Enums // Định nghĩa namespace PBL3.Shared.Enums quản lý cấu trúc code truyền tải và validator.
{
    /// <summary>
    /// Loại điều chỉnh kho trong bút toán kiểm kê.
    /// </summary>
    public enum InventoryAdjustmentType : byte // Định nghĩa kiểu liệt kê (enum) InventoryAdjustmentType kiểu byte đại diện cho các trạng thái trong hệ thống.
    {
        /// <summary>Serial thất thoát (Missing → Lost).</summary>
        Lost = 1, // Giá trị liệt kê 'Lost' có giá trị nguyên là 1.

        /// <summary>Serial lỗi vật lý (Matched/Defective → Defective).</summary>
        Defective = 2 // Giá trị liệt kê 'Defective' có giá trị nguyên là 2.
    }
}

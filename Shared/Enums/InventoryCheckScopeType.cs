namespace PBL3.Shared.Enums // Định nghĩa namespace PBL3.Shared.Enums quản lý cấu trúc code truyền tải và validator.
{
    /// <summary>
    /// Phạm vi kiểm kê kho hàng.
    /// </summary>
    public enum InventoryCheckScopeType : byte // Định nghĩa kiểu liệt kê (enum) InventoryCheckScopeType kiểu byte đại diện cho các trạng thái trong hệ thống.
    {
        /// <summary>Toàn bộ kho.</summary>
        AllStore = 0, // Giá trị liệt kê 'AllStore' có giá trị nguyên là 0.

        /// <summary>Theo danh mục sản phẩm (bao gồm cây con).</summary>
        Category = 1 // Giá trị liệt kê 'Category' có giá trị nguyên là 1.
    }
}

namespace PBL3.Shared.DTOs.Products // Định nghĩa namespace PBL3.Shared.DTOs.Products quản lý cấu trúc code truyền tải và validator.
{
    /// <summary>
    /// Trạng thái sản phẩm.
    /// </summary>
    public enum ProductStatus // Định nghĩa kiểu liệt kê (enum) ProductStatus đại diện cho các trạng thái trong hệ thống.
    {
        Draft = 0, // Giá trị liệt kê 'Draft' có giá trị nguyên là 0.
        Active = 1, // Giá trị liệt kê 'Active' có giá trị nguyên là 1.
        StopBusiness = 2 // Giá trị liệt kê 'StopBusiness' có giá trị nguyên là 2.
    }
}

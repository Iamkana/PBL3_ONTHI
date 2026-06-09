namespace PBL3.Shared.Enums // Định nghĩa namespace PBL3.Shared.Enums quản lý cấu trúc code truyền tải và validator.
{
    /// <summary>
    /// Trạng thái của Serial sản phẩm trong kho.
    /// </summary>
    public enum SerialStatus : byte // Định nghĩa kiểu liệt kê (enum) SerialStatus kiểu byte đại diện cho các trạng thái trong hệ thống.
    {
        /// <summary>Trong kho, sẵn sàng bán.</summary>
        Available = 0, // Giá trị liệt kê 'Available' có giá trị nguyên là 0.

        /// <summary>Có khách đặt Online (đang chờ giao).</summary>
        Reserved = 1, // Giá trị liệt kê 'Reserved' có giá trị nguyên là 1.

        /// <summary>Đã bán thành công.</summary>
        Sold = 2, // Giá trị liệt kê 'Sold' có giá trị nguyên là 2.

        /// <summary>Hàng lỗi, chờ trả hãng.</summary>
        Defective = 3, // Giá trị liệt kê 'Defective' có giá trị nguyên là 3.

        /// <summary>Đã trả lại.</summary>
        Returned = 4, // Giá trị liệt kê 'Returned' có giá trị nguyên là 4.

        /// <summary>Thất thoát (phát hiện qua kiểm kê).</summary>
        Lost = 5 // Giá trị liệt kê 'Lost' có giá trị nguyên là 5.
    }
}

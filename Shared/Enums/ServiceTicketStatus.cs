namespace PBL3.Shared.Enums; // Định nghĩa namespace PBL3.Shared.Enums quản lý cấu trúc code truyền tải và validator.

public enum ServiceTicketStatus : byte // Định nghĩa kiểu liệt kê (enum) ServiceTicketStatus kiểu byte đại diện cho các trạng thái trong hệ thống.
{
    Received = 0, // Giá trị liệt kê 'Received' có giá trị nguyên là 0.
    Diagnosing = 1, // Giá trị liệt kê 'Diagnosing' có giá trị nguyên là 1.
    QuoteSent = 2, // Giá trị liệt kê 'QuoteSent' có giá trị nguyên là 2.
    QuoteRejected = 3, // Giá trị liệt kê 'QuoteRejected' có giá trị nguyên là 3.
    WaitingParts = 4, // Giá trị liệt kê 'WaitingParts' có giá trị nguyên là 4.
    InRepair = 5, // Giá trị liệt kê 'InRepair' có giá trị nguyên là 5.
    SentToManufacturer = 6, // Giá trị liệt kê 'SentToManufacturer' có giá trị nguyên là 6.
    ReceivedFromManufacturer = 7, // Giá trị liệt kê 'ReceivedFromManufacturer' có giá trị nguyên là 7.
    Swapped = 8, // Giá trị liệt kê 'Swapped' có giá trị nguyên là 8.
    Completed = 9, // Giá trị liệt kê 'Completed' có giá trị nguyên là 9.
    Cancelled = 10 // Giá trị liệt kê 'Cancelled' có giá trị nguyên là 10.
}

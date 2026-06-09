using System; // Nhập thư viện hệ thống cơ bản.
using System.Collections.Generic; // Nhập thư viện làm việc với các kiểu danh sách, bộ sưu tập.

namespace PBL3.Shared.DTOs.Sale // Định nghĩa namespace PBL3.Shared.DTOs.Sale quản lý cấu trúc code truyền tải và validator.
{
    /// <summary>
    /// DTO hiển thị chi tiết đơn hàng (kèm danh sách voucher đã dùng).
    /// </summary>
    public class OrderDetailDto // Định nghĩa lớp DTO truyền tải dữ liệu OrderDetailDto.
    {
        public int Id { get; set; } // Thuộc tính Id kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public string OrderCode { get; set; } = string.Empty; // Thuộc tính OrderCode kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public DateTime OrderDate { get; set; } // Thuộc tính OrderDate kiểu dữ liệu DateTime lưu trữ thông tin truyền tải.
        public byte Status { get; set; } // Thuộc tính Status kiểu dữ liệu byte lưu trữ thông tin truyền tải.

        // Shipping
        public string ShipName { get; set; } = string.Empty; // Thuộc tính ShipName kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public string ShipPhone { get; set; } = string.Empty; // Thuộc tính ShipPhone kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public string ShipAddress { get; set; } = string.Empty; // Thuộc tính ShipAddress kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public string ShipCity { get; set; } = string.Empty; // Thuộc tính ShipCity kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.

        // Order metadata
        public byte PaymentMethod { get; set; } // Thuộc tính PaymentMethod kiểu dữ liệu byte lưu trữ thông tin truyền tải.
        public byte PaymentStatus { get; set; } // Thuộc tính PaymentStatus kiểu dữ liệu byte lưu trữ thông tin truyền tải.
        public byte OrderType { get; set; } // Thuộc tính OrderType kiểu dữ liệu byte lưu trữ thông tin truyền tải.
        public string? Note { get; set; } // Thuộc tính Note kiểu dữ liệu string? lưu trữ thông tin truyền tải.
        public string? CancelReason { get; set; } // Thuộc tính CancelReason kiểu dữ liệu string? lưu trữ thông tin truyền tải.

        // Money
        public decimal SubTotal { get; set; } // Thuộc tính SubTotal kiểu dữ liệu decimal lưu trữ thông tin truyền tải.
        public decimal ShippingFee { get; set; } // Thuộc tính ShippingFee kiểu dữ liệu decimal lưu trữ thông tin truyền tải.
        public decimal DiscountAmount { get; set; } // Thuộc tính DiscountAmount kiểu dữ liệu decimal lưu trữ thông tin truyền tải.
        public decimal TotalAmount { get; set; } // Thuộc tính TotalAmount kiểu dữ liệu decimal lưu trữ thông tin truyền tải.

        // Nested
        public List<OrderDetailLineDto> Items { get; set; } = new(); // Thuộc tính Items kiểu dữ liệu List<OrderDetailLineDto> lưu trữ thông tin truyền tải với giá trị mặc định là new().
        public List<VoucherUsageDto> AppliedVouchers { get; set; } = new(); // Thuộc tính AppliedVouchers kiểu dữ liệu List<VoucherUsageDto> lưu trữ thông tin truyền tải với giá trị mặc định là new().
    }
}

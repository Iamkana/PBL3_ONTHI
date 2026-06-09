using System.Collections.Generic; // Nhập thư viện làm việc với các kiểu danh sách, bộ sưu tập.
using System.ComponentModel.DataAnnotations; // Nhập (import) namespace System.ComponentModel.DataAnnotations để sử dụng các thành phần bên trong.

namespace PBL3.Shared.DTOs.Sale // Định nghĩa namespace PBL3.Shared.DTOs.Sale quản lý cấu trúc code truyền tải và validator.
{
    public class CreateOrderRequest // Định nghĩa lớp DTO truyền tải dữ liệu CreateOrderRequest.
    {
        // --- Shipping Info ---
        [Required] public string ShipName { get; set; } = string.Empty; // Thuộc tính ShipName kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        [Required] public string ShipPhone { get; set; } = string.Empty; // Thuộc tính ShipPhone kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        [Required] public string ShipAddress { get; set; } = string.Empty; // Thuộc tính ShipAddress kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        [Required] public string ShipCity { get; set; } = string.Empty; // Thuộc tính ShipCity kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.

        // --- Payment ---
        public byte PaymentMethod { get; set; } // Thuộc tính PaymentMethod kiểu dữ liệu byte lưu trữ thông tin truyền tải.
        public string? Note { get; set; } // Thuộc tính Note kiểu dữ liệu string? lưu trữ thông tin truyền tải.

        // --- Vouchers (Nhận DANH SÁCH mã giảm giá) ---
        /// <summary>
        /// Danh sách mã voucher (Code, không phải Id).
        /// Có thể rỗng nếu không áp dụng voucher.
        /// </summary>
        public List<string>? VoucherCodes { get; set; } // Thuộc tính VoucherCodes kiểu dữ liệu List<string>? lưu trữ thông tin truyền tải.

        // --- Cart Items ---
        public List<CreateOrderDetailRequest> Items { get; set; } = new(); // Thuộc tính Items kiểu dữ liệu List<CreateOrderDetailRequest> lưu trữ thông tin truyền tải với giá trị mặc định là new().
    }

    public class CreateOrderDetailRequest // Định nghĩa lớp DTO truyền tải dữ liệu CreateOrderDetailRequest.
    {
        public int VariantId { get; set; } // Thuộc tính VariantId kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public int Quantity { get; set; } // Thuộc tính Quantity kiểu dữ liệu int lưu trữ thông tin truyền tải.
    }
}

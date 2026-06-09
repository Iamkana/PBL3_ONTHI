using System.Collections.Generic; // Nhập thư viện làm việc với các kiểu danh sách, bộ sưu tập.
using System.ComponentModel.DataAnnotations; // Nhập (import) namespace System.ComponentModel.DataAnnotations để sử dụng các thành phần bên trong.

namespace PBL3.Shared.DTOs.Sale // Định nghĩa namespace PBL3.Shared.DTOs.Sale quản lý cấu trúc code truyền tải và validator.
{
    public class CheckoutRequest // Định nghĩa lớp DTO truyền tải dữ liệu CheckoutRequest.
    {
        [Required] // Thực thi dòng lệnh nghiệp vụ.
        public int UserAddressId { get; set; } // Thuộc tính UserAddressId kiểu dữ liệu int lưu trữ thông tin truyền tải.

        public List<string>? VoucherCodes { get; set; } // Thuộc tính VoucherCodes kiểu dữ liệu List<string>? lưu trữ thông tin truyền tải.

        /// <summary>
        /// 0: COD, 1: Online (MoMo/VNPay)
        /// </summary>
        public byte PaymentMethod { get; set; } // Thuộc tính PaymentMethod kiểu dữ liệu byte lưu trữ thông tin truyền tải.

        /// <summary>
        /// Phí vận chuyển (tạm nhận từ Frontend hoặc fix cứng 30.000đ)
        /// </summary>
        public decimal ShippingFee { get; set; } = 30000; // Thuộc tính ShippingFee kiểu dữ liệu decimal lưu trữ thông tin truyền tải với giá trị mặc định là 30000.

        [MaxLength(500)] // Thực thi dòng lệnh nghiệp vụ.
        public string? Note { get; set; } // Thuộc tính Note kiểu dữ liệu string? lưu trữ thông tin truyền tải.

        // --- Buy Now Flow ---
        public bool IsBuyNow { get; set; } // Thuộc tính IsBuyNow kiểu dữ liệu bool lưu trữ thông tin truyền tải.
        public int? BuyNowVariantId { get; set; } // Thuộc tính BuyNowVariantId kiểu dữ liệu int? lưu trữ thông tin truyền tải.
        public int? BuyNowQuantity { get; set; } // Thuộc tính BuyNowQuantity kiểu dữ liệu int? lưu trữ thông tin truyền tải.
    }
}

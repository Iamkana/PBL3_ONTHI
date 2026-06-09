using System; // Nhập thư viện hệ thống cơ bản.
using System.Collections.Generic; // Nhập thư viện làm việc với các kiểu danh sách, bộ sưu tập.

namespace PBL3.Shared.DTOs.Sale // Định nghĩa namespace PBL3.Shared.DTOs.Sale quản lý cấu trúc code truyền tải và validator.
{
    /// <summary>
    /// Thông tin voucher đã áp dụng (hiển thị trong chi tiết đơn).
    /// </summary>
    public class VoucherUsageDto // Định nghĩa lớp DTO truyền tải dữ liệu VoucherUsageDto.
    {
        public string VoucherCode { get; set; } = string.Empty; // Thuộc tính VoucherCode kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public string VoucherName { get; set; } = string.Empty; // Thuộc tính VoucherName kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public byte DiscountType { get; set; } // Thuộc tính DiscountType kiểu dữ liệu byte lưu trữ thông tin truyền tải.
        public decimal DiscountValue { get; set; } // Thuộc tính DiscountValue kiểu dữ liệu decimal lưu trữ thông tin truyền tải.
        public decimal DiscountApplied { get; set; } // Thuộc tính DiscountApplied kiểu dữ liệu decimal lưu trữ thông tin truyền tải.
    }
}

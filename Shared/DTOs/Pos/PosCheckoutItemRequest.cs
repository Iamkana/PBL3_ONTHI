using System.Collections.Generic; // Nhập thư viện làm việc với các kiểu danh sách, bộ sưu tập.
using System.ComponentModel.DataAnnotations; // Nhập (import) namespace System.ComponentModel.DataAnnotations để sử dụng các thành phần bên trong.
using System; // Nhập thư viện hệ thống cơ bản.

namespace PBL3.Shared.DTOs.Pos // Định nghĩa namespace PBL3.Shared.DTOs.Pos quản lý cấu trúc code truyền tải và validator.
{
    public class PosCheckoutItemRequest // Định nghĩa lớp DTO truyền tải dữ liệu PosCheckoutItemRequest.
    {
        public int SerialId { get; set; } // Thuộc tính SerialId kiểu dữ liệu int lưu trữ thông tin truyền tải.
        // For items without serials, we'd pass VariantId and Quantity, but Use Case flow says:
        // "Quét mã vạch sản phẩm hoặc nhập thủ công mã Seri/IMEI/Mã linh kiện"
        // Let's assume everything will be resolved to a SerialId or handled carefully.
        // Actually, if generic items are allowed, we might need:
        public int? VariantId { get; set; } // Thuộc tính VariantId kiểu dữ liệu int? lưu trữ thông tin truyền tải.
        public int Quantity { get; set; } = 1; // Thuộc tính Quantity kiểu dữ liệu int lưu trữ thông tin truyền tải với giá trị mặc định là 1.
    }
}

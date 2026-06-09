using System; // Sử dụng các kiểu dữ liệu cơ bản của hệ thống.
using System.Collections.Generic; // Sử dụng các cấu trúc tập hợp danh sách.
using System.Threading.Tasks; // Sử dụng các tác vụ lập trình bất đồng bộ.
using PBL3.Core.Entities; // Sử dụng các thực thể từ tầng Core.
using PBL3.Core.Interfaces; // Sử dụng giao diện Repository từ tầng Core.
using PBL3.Shared.DTOs.ServiceTickets; // Sử dụng DTO của module phiếu sửa chữa/bảo hành.

namespace PBL3.Application.ServiceTickets // Khai báo namespace cho tầng Application của module phiếu dịch vụ.
{
    public class WarrantyEvaluator // Định nghĩa lớp tĩnh WarrantyEvaluator hỗ trợ thẩm định thời hạn bảo hành.
    {
        public static async Task<WarrantyEvaluation> EvaluateAsync( // Định nghĩa phương thức tĩnh thẩm định thời hạn bảo hành thiết bị bất đồng bộ.
            ProductSerial serial, // Thực thể số Serial cần thẩm định.
            ProductVariant variant, // Thực thể biến thể sản phẩm đi kèm để lấy thông tin tháng bảo hành mặc định.
            IWarrantyRepository warrantyRepository) // Repository bảo hành để tra cứu lịch sử ghi nhận.
        {
            var now = DateTime.UtcNow; // Lấy thời gian hiện tại theo giờ UTC.

            var activeWarranties = await warrantyRepository.GetActiveBySerialIdAsync(serial.Id); // Bước 1: Thử truy vấn tìm kiếm các dòng ghi nhận bảo hành đang hoạt động trong bảng Warranties.
            if (activeWarranties.Count > 0) // Nếu tìm thấy bản ghi bảo hành đang hoạt động cho số Serial này.
            {
                var warranty = activeWarranties[0]; // Chọn bản ghi bảo hành đầu tiên (được sắp xếp theo EndDate giảm dần).
                return new WarrantyEvaluation( // Trả về kết quả thẩm định bảo hành.
                    IsInWarranty: warranty.EndDate > now, // Thiết bị còn hạn bảo hành nếu ngày kết thúc bảo hành lớn hơn thời gian hiện tại.
                    ExpiresOn: warranty.EndDate, // Ngày hết hạn bảo hành.
                    Source: 0, // Nguồn xác định bảo hành (0: Có bản ghi trong bảng bảo hành Warranties).
                    WarrantyId: warranty.Id // Gán Id của bản ghi bảo hành.
                ); // Kết thúc khởi tạo record.
            } // Kết thúc khối kiểm tra bản ghi bảo hành.

            if (serial.SoldDate.HasValue && variant.WarrantyMonth > 0) // Bước 2: Phương án dự phòng (Fallback): Tính toán hạn bảo hành dựa trên ngày bán và số tháng bảo hành của sản phẩm.
            {
                var expiresOn = serial.SoldDate.Value.AddMonths(variant.WarrantyMonth); // Tính ngày hết hạn bằng cách cộng thêm số tháng bảo hành mặc định vào ngày bán hàng.
                return new WarrantyEvaluation( // Trả về kết quả thẩm định bảo hành tính toán.
                    IsInWarranty: expiresOn > now, // Thiết bị còn hạn bảo hành nếu ngày hết hạn ước tính lớn hơn thời điểm hiện tại.
                    ExpiresOn: expiresOn, // Ngày hết hạn ước tính.
                    Source: 1, // Nguồn xác định (1: Tính toán tự động từ ngày bán hàng và số tháng bảo hành).
                    WarrantyId: null // Không có bản ghi bảo hành cụ thể nào liên kết.
                ); // Kết thúc khởi tạo record.
            } // Kết thúc khối tính toán.

            return new WarrantyEvaluation( // Bước 3: Thiết bị hoàn toàn không có bảo hành hoặc hết hạn.
                IsInWarranty: false, // Không còn/không có bảo hành.
                ExpiresOn: null, // Ngày hết hạn bảo hành bằng null.
                Source: 2, // Nguồn xác định (2: Không tìm thấy bất kỳ thông tin bảo hành nào).
                WarrantyId: null // Không có bản ghi bảo hành.
            ); // Kết thúc khởi tạo record.
        }
    }

    public record WarrantyEvaluation(bool IsInWarranty, DateTime? ExpiresOn, byte Source, int? WarrantyId); // Định nghĩa cấu trúc record chứa thông tin kết quả thẩm định bảo hành.
}

using System.Threading.Tasks; // Sử dụng Task lập trình bất đồng bộ.
using PBL3.Shared.DTOs.Common; // Sử dụng DTO dùng chung ApiResult.
using PBL3.Shared.DTOs.Inventory; // Sử dụng các DTO của module xuất kho.

namespace PBL3.Application.Inventory // Khai báo namespace cho tầng Application của module kiểm kê/xuất kho.
{
    public interface IInventoryExportService // Định nghĩa giao diện dịch vụ xuất kho IInventoryExportService.
    {
        Task<ApiResult<bool>> ExportOrderAsync(ExportOrderRequest request); // Khai báo phương thức thực hiện nghiệp vụ xuất kho cho đơn hàng (gắn serial, đổi trạng thái đơn).
        Task<ApiResult<bool>> ValidateSerialAsync(string serialNo, int variantId); // Khai báo phương thức kiểm tra tính hợp lệ của serial trước khi thực hiện xuất kho.
    }
}

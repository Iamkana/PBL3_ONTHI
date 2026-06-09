using PBL3.Shared.DTOs.BuildPc; // Sử dụng các DTO của module Build PC.

namespace PBL3.Application.BuildPc // Khai báo namespace cho tầng Application của module Build PC.
{
    public interface IBuildPcService // Khai báo interface IBuildPcService cung cấp dịch vụ xuất cấu hình PC.
    {
        Task<byte[]> ExportToExcelAsync(ExportBuildPcRequest request); // Khai báo phương thức xuất cấu hình PC sang file Excel dạng mảng byte.
    }
}

namespace PBL3.Application.Storage; // Khai báo namespace cho tầng Application của module lưu trữ.

public interface IStorageService // Định nghĩa giao diện dịch vụ lưu trữ IStorageService.
{
    Task<string> UploadAsync(Stream content, string fileName, string contentType, string folder, CancellationToken ct = default); // Khai báo phương thức tải lên tệp tin bất đồng bộ.
}

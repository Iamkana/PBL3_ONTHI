using Amazon.S3; // Sử dụng các thư viện ngoại lệ từ AWS S3.
using Microsoft.AspNetCore.Authorization; // Sử dụng phân quyền truy cập.
using Microsoft.AspNetCore.Mvc; // Sử dụng các thành phần MVC.
using Microsoft.Extensions.Logging; // Sử dụng ghi log.
using PBL3.Application.Storage; // Sử dụng dịch vụ lưu trữ tệp IStorageService.
using PBL3.Shared.DTOs.Common; // Sử dụng các DTO dùng chung.

namespace PBL3.API.Controllers.Admin; // Định nghĩa namespace cho Controllers.

[ApiController] // Khai báo lớp là một Web API Controller.
[Route("api/images")] // Định nghĩa route truy cập: api/images.
[Authorize] // Yêu cầu người dùng phải đăng nhập để upload ảnh.
public class ImageController : ControllerBase // Định nghĩa lớp ImageController kế thừa từ ControllerBase.
{
    private static readonly string[] AllowedContentTypes = // Danh sách các kiểu định dạng ảnh được phép tải lên hệ thống.
        ["image/jpeg", "image/png", "image/webp", "image/gif"]; // Cho phép JPEG, PNG, WebP và GIF.

    private const long MaxFileSizeBytes = 5 * 1024 * 1024; // Định nghĩa kích thước file tối đa cho phép là 5MB.

    private readonly IStorageService _storage; // Khai báo dịch vụ lưu trữ.
    private readonly ILogger<ImageController> _logger; // Khai báo logger.

    public ImageController(IStorageService storage, ILogger<ImageController> logger) // Constructor injection tiêm Storage Service và Logger.
    {
        _storage = storage; // Gán dịch vụ lưu trữ.
        _logger = logger; // Gán logger.
    }

    [HttpPost("upload")] // Định nghĩa HTTP POST Method tải ảnh (api/images/upload).
    [RequestSizeLimit(MaxFileSizeBytes)] // Giới hạn kích thước tối đa của request gửi lên là 5MB.
    public async Task<IActionResult> Upload(IFormFile file, [FromQuery] string folder = "general", CancellationToken ct = default) // Tải ảnh lên và lưu vào folder tùy chọn.
    {
        if (file == null || file.Length == 0) // Kiểm tra file gửi lên có trống không.
            return BadRequest(ApiResult<UploadImageResponse>.Fail("Vui lòng chọn file ảnh để upload.")); // Trả về lỗi HTTP 400 BadRequest.

        if (file.Length > MaxFileSizeBytes) // Kiểm tra kích thước file có vượt quá 5MB không.
            return BadRequest(ApiResult<UploadImageResponse>.Fail("File ảnh không được vượt quá 5MB.")); // Trả về lỗi HTTP 400.

        if (!AllowedContentTypes.Contains(file.ContentType, StringComparer.OrdinalIgnoreCase)) // Kiểm tra định dạng Content-Type có hợp lệ không.
            return BadRequest(ApiResult<UploadImageResponse>.Fail("Chỉ chấp nhận file ảnh định dạng JPEG, PNG, WebP hoặc GIF.")); // Trả về lỗi HTTP 400.

        try // Bắt đầu khối try upload file lên đám mây AWS S3.
        {
            using var stream = file.OpenReadStream(); // Mở luồng dữ liệu đọc nội dung file tải lên.
            var url = await _storage.UploadAsync(stream, file.FileName, file.ContentType, folder, ct); // Gọi service tải luồng dữ liệu lên AWS S3 và trả về URL ảnh công khai.
            return Ok(ApiResult<UploadImageResponse>.Ok(new UploadImageResponse(url), "Upload ảnh thành công.")); // Trả về HTTP 200 OK kèm URL ảnh.
        }
        catch (AmazonS3Exception ex) // Bắt các lỗi cụ thể phát sinh từ phía AWS S3.
        {
            _logger.LogError(ex, "Lỗi S3 khi upload ảnh: {ErrorCode}", ex.ErrorCode); // Ghi nhận log lỗi hệ thống kèm mã lỗi S3.
            return StatusCode(503, ApiResult<UploadImageResponse>.Fail("Dịch vụ lưu trữ ảnh hiện không khả dụng. Vui lòng thử lại sau.")); // Trả về HTTP 503 Service Unavailable.
        }
    }
}

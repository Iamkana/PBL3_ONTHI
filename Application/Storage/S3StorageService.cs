using Amazon.S3; // Sử dụng thư viện AWS SDK S3 để thao tác với Cloud Storage.
using Amazon.S3.Model; // Sử dụng các model yêu cầu và phản hồi của AWS S3.
using Microsoft.Extensions.Configuration; // Sử dụng thư viện đọc cấu hình file appsettings.json.

namespace PBL3.Application.Storage; // Khai báo namespace cho tầng Application của module lưu trữ.

public class S3StorageService( // Định nghĩa lớp S3StorageService bằng Primary Constructor.
    IAmazonS3 s3, // Tiêm client AWS S3.
    IConfiguration configuration) : IStorageService // Tiêm IConfiguration để đọc thiết lập AWS và triển khai IStorageService.
{
    private readonly IAmazonS3 _s3 = // Gán AWS S3 client vào trường thành viên.
        s3 ?? throw new ArgumentNullException(nameof(s3)); // Kiểm tra null cho s3.
    private readonly string _bucketName = // Gán tên S3 Bucket từ cấu hình AwsSettings:BucketName.
        (configuration ?? throw new ArgumentNullException(nameof(configuration)))["AwsSettings:BucketName"] // Đọc chuỗi cấu hình BucketName.
            ?? throw new InvalidOperationException("AwsSettings:BucketName chưa được cấu hình."); // Báo lỗi nếu cấu hình trống.
    private readonly string _region = // Gán mã AWS Region từ cấu hình AwsSettings:Region.
        (configuration ?? throw new ArgumentNullException(nameof(configuration)))["AwsSettings:Region"] // Đọc chuỗi cấu hình Region.
            ?? throw new InvalidOperationException("AwsSettings:Region chưa được cấu hình."); // Báo lỗi nếu cấu hình trống.

    public async Task<string> UploadAsync(Stream content, string fileName, string contentType, string folder, CancellationToken ct = default) // Định nghĩa phương thức tải tệp lên AWS S3 bất đồng bộ.
    {
        var ext = Path.GetExtension(fileName).ToLowerInvariant(); // Lấy phần mở rộng của tệp tin và chuyển về chữ thường.
        var key = $"{folder.Trim('/')}/{Guid.NewGuid()}{ext}"; // Tạo đường dẫn (key) lưu trữ ngẫu nhiên trên S3 để tránh trùng lặp tệp.

        var request = new PutObjectRequest // Khởi tạo đối tượng yêu cầu PutObjectRequest để tải lên S3.
        {
            BucketName = _bucketName, // Gán tên Bucket.
            Key = key, // Gán đường dẫn lưu trữ.
            InputStream = content, // Gán luồng dữ liệu tệp tin.
            ContentType = contentType // Gán định dạng nội dung tệp tin (MIME Type).
        };

        await _s3.PutObjectAsync(request, ct); // Thực hiện gọi AWS S3 API để tải tệp tin lên.

        return $"https://{_bucketName}.s3.{_region}.amazonaws.com/{key}"; // Trả về đường dẫn URL công khai của tệp vừa được tải lên S3.
    }
}

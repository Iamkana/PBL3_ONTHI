using System.Threading.RateLimiting; // Sử dụng thư viện cấu hình kiểm soát tần suất Rate Limiting của .NET.
using Amazon; // Sử dụng thư viện SDK của AWS.
using Amazon.Runtime; // Sử dụng các lớp Runtime của AWS SDK để xử lý Credentials.
using Amazon.S3; // Sử dụng thư viện Amazon S3 để lưu trữ file (hình ảnh).
using FluentValidation; // Sử dụng thư viện FluentValidation kiểm tra tính hợp lệ dữ liệu.
using Microsoft.AspNetCore.RateLimiting; // Sử dụng middleware Rate Limiting của ASP.NET Core.
using PBL3.Application.Analytics; // Tham chiếu AnalyticsService từ tầng Application.
using PBL3.Application.Auth; // Tham chiếu AuthService từ tầng Application.
using PBL3.Core.Interfaces; // Tham chiếu các giao diện (interface) dùng chung từ tầng Core.
using PBL3.Application.Banners; // Tham chiếu BannerService.
using PBL3.Application.Cart; // Tham chiếu CartService.
using PBL3.Application.Categories; // Tham chiếu CategoryService.
using PBL3.Application.Customers; // Tham chiếu CustomerService.
using PBL3.Application.Employees; // Tham chiếu EmployeeService.
using PBL3.Application.ImportReceipts; // Tham chiếu ImportReceiptService.
using PBL3.Application.Inventory; // Tham chiếu các service quản lý kho hàng.
using PBL3.Application.Manufacturers; // Tham chiếu ManufacturerService.
using PBL3.Application.Orders; // Tham chiếu OrderService.
using PBL3.Application.Pos; // Tham chiếu PosService.
using PBL3.Application.ProductSerials; // Tham chiếu ProductSerialService.
using PBL3.Application.Products; // Tham chiếu ProductService.
using PBL3.Application.Reviews; // Tham chiếu ProductReviewService.
using PBL3.Application.ServiceInvoices; // Tham chiếu ServiceInvoiceService.
using PBL3.Application.ServiceTickets; // Tham chiếu ServiceTicketService.
using PBL3.Application.Storage; // Tham chiếu các giao diện và lớp lưu trữ tệp.
using PBL3.Application.Storefront; // Tham chiếu StorefrontService.
using PBL3.Application.Suppliers; // Tham chiếu SupplierService.
using PBL3.Application.Vouchers; // Tham chiếu VoucherService.
using PBL3.Shared.DTOs.Banners; // Sử dụng các DTO của Banner dùng chung.
using PBL3.Shared.DTOs.Inventory; // Sử dụng các DTO của Kho dùng chung.
using PBL3.Shared.DTOs.Reviews; // Sử dụng các DTO của Review dùng chung.
using PBL3.Shared.Validators.Banners; // Sử dụng các lớp Validator của Banner.
using PBL3.Shared.Validators.Inventory; // Sử dụng các lớp Validator của Kho.
using PBL3.Shared.Validators.Reviews; // Sử dụng các lớp Validator của Review.

namespace PBL3.API.Extensions; // Định nghĩa namespace cho các lớp mở rộng ở tầng API.

public static class ApplicationServiceExtensions // Định nghĩa lớp tĩnh ApplicationServiceExtensions để đăng ký các dịch vụ nghiệp vụ của tầng Application.
{
    public static IServiceCollection AddApplicationServices( // Định nghĩa phương thức mở rộng AddApplicationServices cho IServiceCollection.
        this IServiceCollection services, // Đối tượng IServiceCollection dùng để đăng ký các dịch vụ.
        IConfiguration configuration) // Đối tượng IConfiguration dùng để đọc cấu hình ứng dụng từ appsettings.json.
    {
        // Application services
        services.AddScoped<ICategoryService, CategoryService>(); // Đăng ký CategoryService với vòng đời Scoped.
        services.AddScoped<IProductService, ProductService>(); // Đăng ký ProductService với vòng đời Scoped.
        services.AddScoped<IManufacturerService, ManufacturerService>(); // Đăng ký ManufacturerService với vòng đời Scoped.
        services.AddScoped<ISupplierService, SupplierService>(); // Đăng ký SupplierService với vòng đời Scoped.
        services.AddScoped<IImportReceiptService, ImportReceiptService>(); // Đăng ký ImportReceiptService với vòng đời Scoped.
        services.AddScoped<IProductSerialService, ProductSerialService>(); // Đăng ký ProductSerialService với vòng đời Scoped.
        services.AddScoped<IInventorySyncService, InventorySyncService>(); // Đăng ký InventorySyncService với vòng đời Scoped.
        services.AddScoped<IInventoryCheckService, InventoryCheckService>(); // Đăng ký InventoryCheckService với vòng đời Scoped.
        services.AddScoped<IInventoryExportService, InventoryExportService>(); // Đăng ký InventoryExportService với vòng đời Scoped.
        services.AddScoped<IPosService, PosService>(); // Đăng ký PosService với vòng đời Scoped.
        services.AddScoped<ICustomerService, CustomerService>(); // Đăng ký CustomerService với vòng đời Scoped.
        services.AddScoped<IEmployeeService, EmployeeService>(); // Đăng ký EmployeeService với vòng đời Scoped.
        services.AddScoped<IStorefrontService, StorefrontService>(); // Đăng ký StorefrontService với vòng đời Scoped.
        services.AddScoped<ICartService, CartService>(); // Đăng ký CartService với vòng đời Scoped.
        services.AddScoped<IOrderService, OrderService>(); // Đăng ký OrderService với vòng đời Scoped.
        services.AddScoped<IVoucherService, VoucherService>(); // Đăng ký VoucherService với vòng đời Scoped.
        services.AddScoped<PBL3.Application.BuildPc.IBuildPcService, PBL3.Application.BuildPc.BuildPcService>(); // Đăng ký BuildPcService với vòng đời Scoped.
        services.AddScoped<IServiceTicketService, ServiceTicketService>(); // Đăng ký ServiceTicketService với vòng đời Scoped.
        services.AddScoped<IServiceInvoiceService, ServiceInvoiceService>(); // Đăng ký ServiceInvoiceService với vòng đời Scoped.
        services.AddScoped<IAnalyticsService, AnalyticsService>(); // Đăng ký AnalyticsService với vòng đời Scoped.
        services.AddScoped<IProductReviewService, ProductReviewService>(); // Đăng ký ProductReviewService với vòng đời Scoped.
        services.AddScoped<IBannerService, BannerService>(); // Đăng ký BannerService với vòng đời Scoped.
        services.AddScoped<IAuthService, AuthService>(); // Đăng ký AuthService với vòng đời Scoped.

        // FluentValidation validators
        services.AddScoped<IValidator<CreateReviewRequest>, CreateReviewRequestValidator>(); // Đăng ký validator tạo đánh giá sản phẩm.
        services.AddScoped<IValidator<CreateBannerRequest>, CreateBannerRequestValidator>(); // Đăng ký validator tạo banner.
        services.AddScoped<IValidator<UpdateBannerRequest>, UpdateBannerRequestValidator>(); // Đăng ký validator cập nhật banner.
        services.AddScoped<IValidator<CreateInventoryCheckRequest>, CreateInventoryCheckRequestValidator>(); // Đăng ký validator tạo phiếu kiểm kê.
        services.AddScoped<IValidator<ScanSerialRequest>, ScanSerialRequestValidator>(); // Đăng ký validator quét số serial sản phẩm.
        services.AddScoped<IValidator<UpdateScanReasonRequest>, UpdateScanReasonRequestValidator>(); // Đăng ký validator cập nhật lý do quét serial.
        services.AddScoped<IValidator<RejectInventoryCheckRequest>, RejectInventoryCheckRequestValidator>(); // Đăng ký validator từ chối phiếu kiểm kê.

        // AWS S3 Storage
        var awsCfg = configuration.GetSection("AwsSettings"); // Đọc phần cấu hình "AwsSettings" trong file appsettings.json.
        services.AddSingleton<IAmazonS3>(_ => // Đăng ký dịch vụ AmazonS3 dạng Singleton dùng chung cho toàn ứng dụng.
        {
            var credentials = new BasicAWSCredentials(awsCfg["AccessKeyId"], awsCfg["SecretAccessKey"]); // Tạo thông tin chứng thực AWS AccessKey và SecretKey.
            var region = RegionEndpoint.GetBySystemName(awsCfg["Region"] ?? "ap-southeast-1"); // Thiết lập vùng máy chủ AWS S3 (mặc định Singapore).
            return new AmazonS3Client(credentials, region); // Khởi tạo và trả về đối tượng AWS S3 Client.
        });
        services.AddScoped<IStorageService, PBL3.Application.Storage.S3StorageService>(); // Đăng ký dịch vụ lưu trữ file S3StorageService của dự án.

        // Rate Limiting: Chống DoS & Brute-force cho Login
        services.AddRateLimiter(options => // Cấu hình giới hạn tần suất gửi yêu cầu lên API.
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests; // Đặt mã lỗi trả về khi vượt giới hạn là HTTP 429 Too Many Requests.

            options.AddFixedWindowLimiter("LoginRateLimit", limiter => // Tạo chính sách giới hạn theo cơ chế Fixed Window mang tên "LoginRateLimit".
            {
                limiter.PermitLimit = 5; // Chỉ cho phép tối đa 5 yêu cầu đăng nhập.
                limiter.Window = TimeSpan.FromMinutes(1); // Trong khoảng thời gian cửa sổ là 1 phút.
                limiter.QueueLimit = 0;  // Reject ngay, không queue (từ chối ngay lập tức nếu vượt quá, không xếp hàng đợi).
            });
        });

        return services; // Trả về đối tượng IServiceCollection để tiếp tục cấu hình.
    }
}

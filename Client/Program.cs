using Microsoft.AspNetCore.Components.Web; // Sử dụng các thành phần Web của Blazor.
using Microsoft.AspNetCore.Components.WebAssembly.Hosting; // Sử dụng thư viện cấu hình và khởi chạy Blazor WebAssembly.
using Microsoft.AspNetCore.Components.Authorization; // Sử dụng thư viện quản lý trạng thái xác thực người dùng.
using MudBlazor.Services; // Sử dụng các dịch vụ giao diện MudBlazor.
using Blazored.LocalStorage; // Sử dụng dịch vụ lưu trữ dữ liệu Local Storage của trình duyệt.
using Blazored.FluentValidation; // Sử dụng thư viện FluentValidation cho Blazor để kiểm tra hợp lệ form.
using FluentValidation; // Sử dụng thư viện FluentValidation để viết các validator dữ liệu.
using Client; // Sử dụng namespace chính Client của ứng dụng.
using Client.Auth; // Sử dụng namespace chứa cấu hình xác thực/phân quyền.
using Client.Services; // Sử dụng namespace dịch vụ của Client.
using Client.Services.Auth; // Sử dụng dịch vụ xác thực tài khoản.
using Client.Services.Category; // Sử dụng dịch vụ quản lý danh mục sản phẩm.
using Client.Services.Customer; // Sử dụng dịch vụ quản lý khách hàng.
using Client.Services.Banner; // Sử dụng dịch vụ quản lý banner quảng cáo.
using Client.Services.Employee; // Sử dụng dịch vụ quản lý nhân viên.
using Client.Services.Manufacturer; // Sử dụng dịch vụ quản lý nhà sản xuất.
using Client.Services.Pos; // Sử dụng dịch vụ bán hàng tại quầy POS.
using Client.Services.Product; // Sử dụng dịch vụ quản lý sản phẩm.
using Client.Services.Supplier; // Sử dụng dịch vụ quản lý nhà cung cấp.
using Client.Services.Inventory; // Sử dụng dịch vụ quản lý kho (nhập, xuất, kiểm kho).
using Client.Services.Voucher; // Sử dụng dịch vụ quản lý voucher khuyến mãi.
using Client.Services.Image; // Sử dụng dịch vụ tải và xử lý hình ảnh.
using Client.Services.ServiceTickets; // Sử dụng dịch vụ quản lý phiếu sửa chữa/bảo hành.
using Client.Services.Analytics; // Sử dụng dịch vụ phân tích dữ liệu và báo cáo.
using Client.Services.Comparison; // Sử dụng dịch vụ so sánh các thông số sản phẩm.

var builder = WebAssemblyHostBuilder.CreateDefault(args); // Thực hiện gán giá trị của biểu thức 'WebAssemblyHostBuilder.CreateDefault(args)' cho biến/thuộc tính 'builder'.
builder.RootComponents.Add<App>("#app"); // Thực thi dòng lệnh nghiệp vụ.
builder.RootComponents.Add<HeadOutlet>("head::after"); // Thực thi dòng lệnh nghiệp vụ.

// ===== Authentication & Authorization ===== // Chú thích cấu hình phần xác thực và phân quyền người dùng.
builder.Services.AddBlazoredLocalStorage(); // Thực thi dòng lệnh nghiệp vụ.
builder.Services.AddValidatorsFromAssemblyContaining<PBL3.Shared.DTOs.Auth.LoginRequestValidator>(); // Thực thi dòng lệnh nghiệp vụ.
builder.Services.AddAuthorizationCore(); // Thực thi dòng lệnh nghiệp vụ.
builder.Services.AddScoped<JwtAuthenticationStateProvider>(); // Đăng ký dịch vụ Dependency Injection (Scoped).
builder.Services.AddScoped<AuthenticationStateProvider>(sp => // Đăng ký dịch vụ Dependency Injection (Scoped).
    sp.GetRequiredService<JwtAuthenticationStateProvider>()); // Thực thi dòng lệnh nghiệp vụ.
builder.Services.AddTransient<AuthHeaderHandler>(); // Đăng ký Dependency Injection kiểu Transient cho lớp AuthHeaderHandler.

// ===== HttpClient trỏ về API Backend (có gắn AuthHeaderHandler) ===== // Chú thích phần cấu hình HttpClient gọi API Backend.
var apiBaseUrl = builder.Configuration["ApiBaseUrl"] // Thực hiện gán giá trị của biểu thức 'builder.Configuration["ApiBaseUrl"]' cho biến/thuộc tính 'apiBaseUrl'.
    ?? throw new InvalidOperationException("ApiBaseUrl chưa được cấu hình trong wwwroot/appsettings.json."); // Thực thi dòng lệnh nghiệp vụ.

builder.Services.AddHttpClient("HushStoreAPI", client => // Cấu hình và đăng ký HttpClient có tên 'HushStoreAPI' phục vụ gọi API.
{
    client.BaseAddress = new Uri(apiBaseUrl); // Thực hiện gán giá trị của biểu thức 'new Uri(apiBaseUrl)' cho biến/thuộc tính 'client.BaseAddress'.
}).AddHttpMessageHandler<AuthHeaderHandler>(); // Thực thi dòng lệnh nghiệp vụ.

// HttpClient cho Provinces API (public, không cần auth) // Chú thích HttpClient gọi API hành chính Việt Nam công cộng.
builder.Services.AddHttpClient("ProvincesAPI", client => // Cấu hình và đăng ký HttpClient có tên 'ProvincesAPI' phục vụ gọi API.
{
    client.BaseAddress = new Uri("https://provinces.open-api.vn"); // Thực hiện gán giá trị của biểu thức 'new Uri("https://provinces.open-api.vn")' cho biến/thuộc tính 'client.BaseAddress'.
}); // Thực thi dòng lệnh nghiệp vụ.

// Đăng ký HttpClient mặc định (inject HttpClient trực tiếp) dùng Named client ở trên // Chú thích đăng ký HttpClient mặc định.
builder.Services.AddScoped(sp => // Đăng ký HttpClient mặc định lấy từ HttpClientFactory qua tên định danh.
    sp.GetRequiredService<IHttpClientFactory>().CreateClient("HushStoreAPI")); // Thực thi dòng lệnh nghiệp vụ.

// MudBlazor // Chú thích cấu hình thư viện giao diện.
builder.Services.AddMudServices(); // Thực thi dòng lệnh nghiệp vụ.

// Client Services (DI) // Chú thích đăng ký các dịch vụ nghiệp vụ (Dependency Injection).
builder.Services.AddScoped<ICategoryClientService, CategoryClientService>(); // Đăng ký Dependency Injection kiểu Scoped: giao diện ICategoryClientService thực thi bởi lớp CategoryClientService.
builder.Services.AddScoped<IProductClientService, ProductClientService>(); // Đăng ký Dependency Injection kiểu Scoped: giao diện IProductClientService thực thi bởi lớp ProductClientService.
builder.Services.AddScoped<IManufacturerClientService, ManufacturerClientService>(); // Đăng ký Dependency Injection kiểu Scoped: giao diện IManufacturerClientService thực thi bởi lớp ManufacturerClientService.
builder.Services.AddScoped<ISupplierClientService, SupplierClientService>(); // Đăng ký Dependency Injection kiểu Scoped: giao diện ISupplierClientService thực thi bởi lớp SupplierClientService.
builder.Services.AddScoped<IImportReceiptClientService, ImportReceiptClientService>(); // Đăng ký Dependency Injection kiểu Scoped: giao diện IImportReceiptClientService thực thi bởi lớp ImportReceiptClientService.
builder.Services.AddScoped<IProductSerialClientService, ProductSerialClientService>(); // Đăng ký Dependency Injection kiểu Scoped: giao diện IProductSerialClientService thực thi bởi lớp ProductSerialClientService.
builder.Services.AddScoped<IAuthClientService, AuthClientService>(); // Đăng ký Dependency Injection kiểu Scoped: giao diện IAuthClientService thực thi bởi lớp AuthClientService.
builder.Services.AddScoped<IPosClientService, PosClientService>(); // Đăng ký Dependency Injection kiểu Scoped: giao diện IPosClientService thực thi bởi lớp PosClientService.
builder.Services.AddScoped<ICustomerClientService, CustomerClientService>(); // Đăng ký Dependency Injection kiểu Scoped: giao diện ICustomerClientService thực thi bởi lớp CustomerClientService.
builder.Services.AddScoped<IEmployeeClientService, EmployeeClientService>(); // Đăng ký Dependency Injection kiểu Scoped: giao diện IEmployeeClientService thực thi bởi lớp EmployeeClientService.
builder.Services.AddScoped<IUserAddressClientService, UserAddressClientService>(); // Đăng ký Dependency Injection kiểu Scoped: giao diện IUserAddressClientService thực thi bởi lớp UserAddressClientService.
builder.Services.AddScoped<Client.Services.Orders.IOrderClientService, Client.Services.Orders.OrderClientService>(); // Đăng ký Dependency Injection kiểu Scoped: giao diện Client.Services.Orders.IOrderClientService thực thi bởi lớp Client.Services.Orders.OrderClientService.
builder.Services.AddScoped<IInventoryExportClientService, InventoryExportClientService>(); // Đăng ký Dependency Injection kiểu Scoped: giao diện IInventoryExportClientService thực thi bởi lớp InventoryExportClientService.
builder.Services.AddScoped<Client.Services.Storefront.IStorefrontClientService, Client.Services.Storefront.StorefrontClientService>(); // Đăng ký Dependency Injection kiểu Scoped: giao diện Client.Services.Storefront.IStorefrontClientService thực thi bởi lớp Client.Services.Storefront.StorefrontClientService.
builder.Services.AddScoped<Client.Services.Cart.ICartClientService, Client.Services.Cart.CartClientService>(); // Đăng ký Dependency Injection kiểu Scoped: giao diện Client.Services.Cart.ICartClientService thực thi bởi lớp Client.Services.Cart.CartClientService.
builder.Services.AddScoped<IVoucherClientService, VoucherClientService>(); // Đăng ký Dependency Injection kiểu Scoped: giao diện IVoucherClientService thực thi bởi lớp VoucherClientService.
builder.Services.AddScoped<Client.Services.BuildPc.IBuildPcClientService, Client.Services.BuildPc.BuildPcClientService>(); // Đăng ký Dependency Injection kiểu Scoped: giao diện Client.Services.BuildPc.IBuildPcClientService thực thi bởi lớp Client.Services.BuildPc.BuildPcClientService.
builder.Services.AddScoped<IImageClientService, ImageClientService>(); // Đăng ký Dependency Injection kiểu Scoped: giao diện IImageClientService thực thi bởi lớp ImageClientService.
builder.Services.AddScoped<IServiceTicketClientService, ServiceTicketClientService>(); // Đăng ký Dependency Injection kiểu Scoped: giao diện IServiceTicketClientService thực thi bởi lớp ServiceTicketClientService.
builder.Services.AddScoped<IServiceInvoiceClientService, ServiceInvoiceClientService>(); // Đăng ký Dependency Injection kiểu Scoped: giao diện IServiceInvoiceClientService thực thi bởi lớp ServiceInvoiceClientService.
builder.Services.AddScoped<IAnalyticsClientService, AnalyticsClientService>(); // Đăng ký Dependency Injection kiểu Scoped: giao diện IAnalyticsClientService thực thi bởi lớp AnalyticsClientService.
builder.Services.AddScoped<Client.Services.Reviews.IReviewClientService, Client.Services.Reviews.ReviewClientService>(); // Đăng ký Dependency Injection kiểu Scoped: giao diện Client.Services.Reviews.IReviewClientService thực thi bởi lớp Client.Services.Reviews.ReviewClientService.
builder.Services.AddScoped<IBannerClientService, BannerClientService>(); // Đăng ký Dependency Injection kiểu Scoped: giao diện IBannerClientService thực thi bởi lớp BannerClientService.
builder.Services.AddScoped<IComparisonService, ComparisonService>(); // Đăng ký Dependency Injection kiểu Scoped: giao diện IComparisonService thực thi bởi lớp ComparisonService.
builder.Services.AddScoped<IInventoryCheckClientService, InventoryCheckClientService>(); // Đăng ký Dependency Injection kiểu Scoped: giao diện IInventoryCheckClientService thực thi bởi lớp InventoryCheckClientService.

await builder.Build().RunAsync(); // Thực thi dòng lệnh nghiệp vụ.

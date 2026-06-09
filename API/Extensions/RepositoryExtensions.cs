using PBL3.Core.Interfaces; // Sử dụng các giao diện định nghĩa Repository từ tầng Core.
using PBL3.Infrastructure.Data; // Sử dụng thực thể Unit of Work để đăng ký DI.
using PBL3.Infrastructure.Repositories; // Sử dụng các lớp cài đặt Repository thực tế ở tầng Infrastructure.

namespace PBL3.API.Extensions; // Định nghĩa namespace cho các lớp mở rộng ở tầng API.

public static class RepositoryExtensions // Định nghĩa lớp tĩnh RepositoryExtensions để đăng ký các Repository vào hệ thống Dependency Injection.
{
    public static IServiceCollection AddRepositories(this IServiceCollection services) // Định nghĩa phương thức mở rộng AddRepositories cho IServiceCollection.
    {
        services.AddScoped<ICategoryRepository, CategoryRepository>(); // Đăng ký CategoryRepository với vòng đời Scoped.
        services.AddScoped<IProductRepository, ProductRepository>(); // Đăng ký ProductRepository với vòng đời Scoped.
        services.AddScoped<IManufacturerRepository, ManufacturerRepository>(); // Đăng ký ManufacturerRepository với vòng đời Scoped.
        services.AddScoped<ISupplierRepository, SupplierRepository>(); // Đăng ký SupplierRepository với vòng đời Scoped.
        services.AddScoped<IImportReceiptRepository, ImportReceiptRepository>(); // Đăng ký ImportReceiptRepository với vòng đời Scoped.
        services.AddScoped<IProductSerialRepository, ProductSerialRepository>(); // Đăng ký ProductSerialRepository với vòng đời Scoped.
        services.AddScoped<IInventoryCheckRepository, InventoryCheckRepository>(); // Đăng ký InventoryCheckRepository với vòng đời Scoped.
        services.AddScoped<IWarrantyRepository, WarrantyRepository>(); // Đăng ký WarrantyRepository với vòng đời Scoped.
        services.AddScoped<IOrderRepository, OrderRepository>(); // Đăng ký OrderRepository với vòng đời Scoped.
        services.AddScoped<IVoucherRepository, VoucherRepository>(); // Đăng ký VoucherRepository với vòng đời Scoped.
        services.AddScoped<ICustomerRepository, CustomerRepository>(); // Đăng ký CustomerRepository với vòng đời Scoped.
        services.AddScoped<IEmployeeRepository, EmployeeRepository>(); // Đăng ký EmployeeRepository với vòng đời Scoped.
        services.AddScoped<ICartRepository, CartRepository>(); // Đăng ký CartRepository với vòng đời Scoped.
        services.AddScoped<IUserAddressRepository, UserAddressRepository>(); // Đăng ký UserAddressRepository với vòng đời Scoped.
        services.AddScoped<IProductReviewRepository, ProductReviewRepository>(); // Đăng ký ProductReviewRepository với vòng đời Scoped.
        services.AddScoped<IServiceTicketRepository, ServiceTicketRepository>(); // Đăng ký ServiceTicketRepository với vòng đời Scoped.
        services.AddScoped<IQuotationRepository, QuotationRepository>(); // Đăng ký QuotationRepository với vòng đời Scoped.
        services.AddScoped<IRmaShipmentRepository, RmaShipmentRepository>(); // Đăng ký RmaShipmentRepository với vòng đời Scoped.
        services.AddScoped<IServiceInvoiceRepository, ServiceInvoiceRepository>(); // Đăng ký ServiceInvoiceRepository với vòng đời Scoped.
        services.AddScoped<ISerialRepairLogRepository, SerialRepairLogRepository>(); // Đăng ký SerialRepairLogRepository với vòng đời Scoped.
        services.AddScoped<IBannerRepository, BannerRepository>(); // Đăng ký BannerRepository với vòng đời Scoped.

        // Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>(); // Đăng ký UnitOfWork với vòng đời Scoped để quản lý chung các Transaction và Lưu thay đổi đồng bộ.

        return services; // Trả về đối tượng IServiceCollection để tiếp tục cấu hình.
    }
}

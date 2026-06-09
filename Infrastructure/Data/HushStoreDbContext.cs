using Microsoft.AspNetCore.Identity; // Nhập thư viện Identity để làm việc với tài khoản người dùng và vai trò.
using Microsoft.AspNetCore.Identity.EntityFrameworkCore; // Nhập thư viện tích hợp Identity với Entity Framework Core.
using Microsoft.EntityFrameworkCore; // Nhập thư viện Microsoft.EntityFrameworkCore để làm việc với CSDL qua ORM.
using Microsoft.EntityFrameworkCore.ChangeTracking; // Nhập thư viện hỗ trợ theo dõi sự thay đổi trạng thái của thực thể.
using PBL3.Core.Entities; // Nhập namespace chứa các thực thể nghiệp vụ cốt lưu trữ của hệ thống.

namespace PBL3.Infrastructure.Data // Định nghĩa namespace PBL3.Infrastructure.Data quản lý cấu trúc code.
{
    public class HushStoreDbContext : IdentityDbContext<AppUser, AppRole, Guid, // Định nghĩa lớp HushStoreDbContext triển khai/kế thừa IdentityDbContext<AppUser, AppRole, Guid,.
        IdentityUserClaim<Guid>, IdentityUserRole<Guid>, IdentityUserLogin<Guid>, // Thực thi dòng lệnh nghiệp vụ.
        IdentityRoleClaim<Guid>, IdentityUserToken<Guid>> // Thực thi dòng lệnh nghiệp vụ.
    {
        public HushStoreDbContext(DbContextOptions<HushStoreDbContext> options) : base(options) // Hàm khởi tạo (Constructor) của lớp HushStoreDbContext nhận tham số: options.
        {
        }

        // Product
        public DbSet<Manufacturer> Manufacturers { get; set; } // Khai báo DbSet để truy vấn và lưu trữ thực thể Manufacturer qua bảng Manufacturers.
        public DbSet<Category> Categories { get; set; } // Khai báo DbSet để truy vấn và lưu trữ thực thể Category qua bảng Categories.
        public DbSet<Product> Products { get; set; } // Khai báo DbSet để truy vấn và lưu trữ thực thể Product qua bảng Products.
        public DbSet<ProductVariant> ProductVariants { get; set; } // Khai báo DbSet để truy vấn và lưu trữ thực thể ProductVariant qua bảng ProductVariants.
        public DbSet<ProductImage> ProductImages { get; set; } // Khai báo DbSet để truy vấn và lưu trữ thực thể ProductImage qua bảng ProductImages.

        // Inventory
        public DbSet<Supplier> Suppliers { get; set; } // Khai báo DbSet để truy vấn và lưu trữ thực thể Supplier qua bảng Suppliers.
        public DbSet<ImportReceipt> ImportReceipts { get; set; } // Khai báo DbSet để truy vấn và lưu trữ thực thể ImportReceipt qua bảng ImportReceipts.
        public DbSet<ImportReceiptDetail> ImportReceiptDetails { get; set; } // Khai báo DbSet để truy vấn và lưu trữ thực thể ImportReceiptDetail qua bảng ImportReceiptDetails.
        public DbSet<ProductSerial> ProductSerials { get; set; } // Khai báo DbSet để truy vấn và lưu trữ thực thể ProductSerial qua bảng ProductSerials.
        public DbSet<InventoryCheck> InventoryChecks { get; set; } // Khai báo DbSet để truy vấn và lưu trữ thực thể InventoryCheck qua bảng InventoryChecks.
        public DbSet<InventoryCheckDetail> InventoryCheckDetails { get; set; } // Khai báo DbSet để truy vấn và lưu trữ thực thể InventoryCheckDetail qua bảng InventoryCheckDetails.
        public DbSet<InventoryCheckDetailSerial> InventoryCheckDetailSerials { get; set; } // Khai báo DbSet để truy vấn và lưu trữ thực thể InventoryCheckDetailSerial qua bảng InventoryCheckDetailSerials.
        public DbSet<InventoryAdjustmentLog> InventoryAdjustmentLogs { get; set; } // Khai báo DbSet để truy vấn và lưu trữ thực thể InventoryAdjustmentLog qua bảng InventoryAdjustmentLogs.

        // Sale
        public DbSet<Voucher> Vouchers { get; set; } // Khai báo DbSet để truy vấn và lưu trữ thực thể Voucher qua bảng Vouchers.
        public DbSet<VoucherCategory> VoucherCategories { get; set; } // Khai báo DbSet để truy vấn và lưu trữ thực thể VoucherCategory qua bảng VoucherCategories.
        public DbSet<Order> Orders { get; set; } // Khai báo DbSet để truy vấn và lưu trữ thực thể Order qua bảng Orders.
        public DbSet<OrderDetail> OrderDetails { get; set; } // Khai báo DbSet để truy vấn và lưu trữ thực thể OrderDetail qua bảng OrderDetails.
        public DbSet<OrderSerial> OrderSerials { get; set; } // Khai báo DbSet để truy vấn và lưu trữ thực thể OrderSerial qua bảng OrderSerials.
        public DbSet<Cart> Carts { get; set; } // Khai báo DbSet để truy vấn và lưu trữ thực thể Cart qua bảng Carts.
        public DbSet<VoucherUsage> VoucherUsages { get; set; } // Khai báo DbSet để truy vấn và lưu trữ thực thể VoucherUsage qua bảng VoucherUsages.
        public DbSet<Warranty> Warranties { get; set; } // Khai báo DbSet để truy vấn và lưu trữ thực thể Warranty qua bảng Warranties.
        public DbSet<UserAddress> UserAddresses { get; set; } // Khai báo DbSet để truy vấn và lưu trữ thực thể UserAddress qua bảng UserAddresses.
        public DbSet<ProductReview> ProductReviews { get; set; } // Khai báo DbSet để truy vấn và lưu trữ thực thể ProductReview qua bảng ProductReviews.

        // Service & Warranty
        public DbSet<ServiceTicket> ServiceTickets { get; set; } // Khai báo DbSet để truy vấn và lưu trữ thực thể ServiceTicket qua bảng ServiceTickets.
        public DbSet<ServiceTicketStatusHistory> ServiceTicketStatusHistories { get; set; } // Khai báo DbSet để truy vấn và lưu trữ thực thể ServiceTicketStatusHistory qua bảng ServiceTicketStatusHistories.
        public DbSet<Quotation> Quotations { get; set; } // Khai báo DbSet để truy vấn và lưu trữ thực thể Quotation qua bảng Quotations.
        public DbSet<QuotationItem> QuotationItems { get; set; } // Khai báo DbSet để truy vấn và lưu trữ thực thể QuotationItem qua bảng QuotationItems.
        public DbSet<RmaShipment> RmaShipments { get; set; } // Khai báo DbSet để truy vấn và lưu trữ thực thể RmaShipment qua bảng RmaShipments.
        public DbSet<ServiceInvoice> ServiceInvoices { get; set; } // Khai báo DbSet để truy vấn và lưu trữ thực thể ServiceInvoice qua bảng ServiceInvoices.
        public DbSet<ServiceInvoiceItem> ServiceInvoiceItems { get; set; } // Khai báo DbSet để truy vấn và lưu trữ thực thể ServiceInvoiceItem qua bảng ServiceInvoiceItems.
        public DbSet<SerialRepairLog> SerialRepairLogs { get; set; } // Khai báo DbSet để truy vấn và lưu trữ thực thể SerialRepairLog qua bảng SerialRepairLogs.

        // Auth
        public DbSet<UserProfile> UserProfiles { get; set; } // Khai báo DbSet để truy vấn và lưu trữ thực thể UserProfile qua bảng UserProfiles.
        public DbSet<RefreshToken> RefreshTokens { get; set; } // Khai báo DbSet để truy vấn và lưu trữ thực thể RefreshToken qua bảng RefreshTokens.

        // Storefront
        public DbSet<Banner> Banners { get; set; } = null!; // Khai báo DbSet để truy vấn và lưu trữ thực thể Banner qua bảng Banners.

        protected override void OnModelCreating(ModelBuilder modelBuilder) // Thực hiện xử lý phương thức 'OnModelCreating' nhận tham số (modelBuilder) trả về kiểu void.
        {
            base.OnModelCreating(modelBuilder); // Thực thi dòng lệnh nghiệp vụ.

            // --- AUTH: Rename Identity tables theo convention ---
            modelBuilder.Entity<AppUser>(entity => // Thực thi dòng lệnh nghiệp vụ.
            {
                entity.ToTable("AppUsers"); // Ánh xạ thực thể cấu hình sang tên bảng 'AppUsers' trong cơ sở dữ liệu.
                entity.Property(u => u.Id).HasDefaultValueSql("NEWSEQUENTIALID()"); // Cấu hình giá trị mặc định của cột sử dụng câu lệnh SQL 'NEWSEQUENTIALID()'.
                entity.Property(u => u.PhoneNumber).HasMaxLength(20).IsUnicode(false); // Giới hạn độ dài tối đa của cột là 20 ký tự.
            }); // Thực thi dòng lệnh nghiệp vụ.
            modelBuilder.Entity<UserProfile>(entity => // Thực thi dòng lệnh nghiệp vụ.
            {
                entity.ToTable("UserProfiles"); // Ánh xạ thực thể cấu hình sang tên bảng 'UserProfiles' trong cơ sở dữ liệu.
                entity.HasOne(p => p.User) // Cấu hình quan hệ một-nhiều, bắt đầu với một phía thực thể: p => p.User.
                      .WithOne(u => u.Profile) // Cấu hình quan hệ liên kết với thực thể một phía tương ứng: u => u.Profile.
                      .HasForeignKey<UserProfile>(p => p.UserId) // Chỉ định thuộc tính khóa ngoại liên kết cho quan hệ: p => p.UserId.
                      .OnDelete(DeleteBehavior.Cascade); // Cấu hình hành vi xóa tự động khi bản ghi chính bị xóa: DeleteBehavior.Cascade.
            }); // Thực thi dòng lệnh nghiệp vụ.
            modelBuilder.Entity<AppRole>(entity => // Thực thi dòng lệnh nghiệp vụ.
            {
                entity.ToTable("AppRoles"); // Ánh xạ thực thể cấu hình sang tên bảng 'AppRoles' trong cơ sở dữ liệu.
                entity.Property(r => r.Id).HasDefaultValueSql("NEWSEQUENTIALID()"); // Cấu hình giá trị mặc định của cột sử dụng câu lệnh SQL 'NEWSEQUENTIALID()'.
                entity.HasIndex(r => r.RoleCode).IsUnique(); // Cấu hình chỉ mục độc nhất (Unique Index) cho thực thể trên thuộc tính r => r.RoleCode.
            }); // Thực thi dòng lệnh nghiệp vụ.
            modelBuilder.Entity<IdentityUserRole<Guid>>().ToTable("AppUserRoles"); // Ánh xạ thực thể cấu hình sang tên bảng 'AppUserRoles' trong cơ sở dữ liệu.
            modelBuilder.Entity<IdentityUserClaim<Guid>>().ToTable("AppUserClaims"); // Ánh xạ thực thể cấu hình sang tên bảng 'AppUserClaims' trong cơ sở dữ liệu.
            modelBuilder.Entity<IdentityUserLogin<Guid>>().ToTable("AppUserLogins"); // Ánh xạ thực thể cấu hình sang tên bảng 'AppUserLogins' trong cơ sở dữ liệu.
            modelBuilder.Entity<IdentityRoleClaim<Guid>>().ToTable("AppRoleClaims"); // Ánh xạ thực thể cấu hình sang tên bảng 'AppRoleClaims' trong cơ sở dữ liệu.
            modelBuilder.Entity<IdentityUserToken<Guid>>().ToTable("AppUserTokens"); // Ánh xạ thực thể cấu hình sang tên bảng 'AppUserTokens' trong cơ sở dữ liệu.

            // --- PRODUCT ---
            modelBuilder.Entity<Product>(entity => // Thực thi dòng lệnh nghiệp vụ.
            {
                entity.HasIndex(p => p.Slug).IsUnique(); // Cấu hình chỉ mục độc nhất (Unique Index) cho thực thể trên thuộc tính p => p.Slug.
            }); // Thực thi dòng lệnh nghiệp vụ.

            modelBuilder.Entity<Category>(entity => // Thực thi dòng lệnh nghiệp vụ.
            {
                entity.HasIndex(c => c.Slug).IsUnique(); // Cấu hình chỉ mục độc nhất (Unique Index) cho thực thể trên thuộc tính c => c.Slug.
                // Recursive Relationship (Adjacency List)
                entity.HasOne(c => c.Parent) // Cấu hình quan hệ một-nhiều, bắt đầu với một phía thực thể: c => c.Parent.
                    .WithMany(p => p.Children) // Cấu hình quan hệ liên kết với thực thể nhiều phía tương ứng: p => p.Children.
                    .HasForeignKey(c => c.ParentId) // Chỉ định thuộc tính khóa ngoại liên kết cho quan hệ: c => c.ParentId.
                    .OnDelete(DeleteBehavior.NoAction); // Cấu hình hành vi xóa tự động khi bản ghi chính bị xóa: DeleteBehavior.NoAction.
            }); // Thực thi dòng lệnh nghiệp vụ.

            modelBuilder.Entity<ProductVariant>(entity => // Thực thi dòng lệnh nghiệp vụ.
            {
                entity.HasIndex(v => v.SKU).IsUnique(); // Cấu hình chỉ mục độc nhất (Unique Index) cho thực thể trên thuộc tính v => v.SKU.
                entity.HasIndex(v => v.ProductId); // Cấu hình chỉ mục (Index) cho thực thể trên thuộc tính v => v.ProductId.
                entity.Property(e => e.Price).HasColumnType("decimal(18,2)"); // Cấu hình kiểu dữ liệu trong CSDL là 'decimal(18,2)'.
                entity.Property(e => e.OriginalPrice).HasColumnType("decimal(18,2)"); // Cấu hình kiểu dữ liệu trong CSDL là 'decimal(18,2)'.

                // StockQuantity - cột vật lý, default = 0
                entity.Property(e => e.StockQuantity) // Thực thi dòng lệnh nghiệp vụ.
                      .HasDefaultValue(0); // Cấu hình giá trị mặc định cho cột là 0.

                // Specifications - JSON column
                // EF Core không tự so sánh được Dictionary<string,string> theo từng cặp key-value.
                // Nếu không có ValueComparer tuỳ chỉnh, EF Core sẽ luôn đánh dấu cột này là "đã thay đổi"
                // mỗi khi SaveChanges() được gọi — dù dữ liệu thực tế không đổi — gây ra UPDATE thừa.
                // ValueComparer cần 3 hàm:
                var specComparer = new ValueComparer<Dictionary<string, string>>( // Thực hiện gán giá trị của biểu thức 'new ValueComparer<Dictionary<string, string>>(' cho biến 'specComparer'.
                    // 1. equalsExpression — so sánh bằng nhau:
                    //    Hai Dictionary bằng nhau khi cùng null, hoặc có cùng số cặp key-value
                    //    và tất cả các cặp trong c1 đều tồn tại trong c2 (Except trả về rỗng).
                    (c1, c2) => c1 == c2 || (c1 != null && c2 != null && // Thực thi dòng lệnh nghiệp vụ.
                                c1.Count == c2.Count && !c1.Except(c2).Any()), // Thực thi dòng lệnh nghiệp vụ.

                    // 2. hashCodeExpression — tính hash code để EF Core lưu "ảnh chụp" trạng thái cũ:
                    //    Nếu null → hash = 0. Nếu có dữ liệu → kết hợp hash của từng cặp key-value
                    //    bằng HashCode.Combine để ra một số duy nhất đại diện cho toàn bộ Dictionary.
                    c => c == null ? 0 : c.Aggregate(0, (a, p) => // Thực thi dòng lệnh nghiệp vụ.
                                HashCode.Combine(a, p.Key.GetHashCode(), // Thực thi dòng lệnh nghiệp vụ.
                                    p.Value == null ? 0 : p.Value.GetHashCode())), // Thực thi dòng lệnh nghiệp vụ.

                    // 3. snapshotExpression — tạo bản sao độc lập (deep copy):
                    //    EF Core cần lưu bản sao của giá trị gốc để so sánh sau khi entity bị sửa.
                    //    Nếu chỉ gán tham chiếu (=), cả hai sẽ trỏ vào cùng object → mất khả năng phát hiện thay đổi.
                    //    "c ?? new()" đảm bảo không bao giờ snapshot thành null.
                    c => new Dictionary<string, string>(c ?? new()) // Thực hiện gán giá trị của biểu thức '> new Dictionary<string, string>(c ?? new())' cho biến 'c'.
                ); // Thực thi dòng lệnh nghiệp vụ.
                entity.Property(e => e.Specifications) // Thực thi dòng lệnh nghiệp vụ.
                      .HasColumnType("nvarchar(max)") // Cấu hình kiểu dữ liệu trong CSDL là 'nvarchar(max)'.
                      .HasConversion( // Cấu hình chuyển đổi kiểu dữ liệu (Value Converter) khi lưu trữ xuống cơ sở dữ liệu.
                          v => System.Text.Json.JsonSerializer.Serialize(v, System.Text.Json.JsonSerializerOptions.Default), // Thực hiện gán giá trị của biểu thức '> System.Text.Json.JsonSerializer.Serialize(v, System.Text.Json.JsonSerializerOptions.Default),' cho biến 'v'.
                          v => System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(v, System.Text.Json.JsonSerializerOptions.Default) // Thực hiện gán giá trị của biểu thức '> System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(v, System.Text.Json.JsonSerializerOptions.Default)' cho biến 'v'.
                               ?? new Dictionary<string, string>() // Thực thi dòng lệnh nghiệp vụ.
                      ) // Thực thi dòng lệnh nghiệp vụ.
                      .Metadata.SetValueComparer(specComparer); // Thiết lập bộ so sánh giá trị tùy chỉnh để EF Core theo dõi sự thay đổi của thực thể.
            }); // Thực thi dòng lệnh nghiệp vụ.

            // --- PRODUCT ---
            modelBuilder.Entity<Manufacturer>(entity => // Thực thi dòng lệnh nghiệp vụ.
            {
                entity.HasQueryFilter(m => !m.IsDeleted); // Cấu hình bộ lọc toàn cục (Global Query Filter) để tự động lọc: m => !m.IsDeleted.
            }); // Thực thi dòng lệnh nghiệp vụ.

            modelBuilder.Entity<Product>(entity => // Thực thi dòng lệnh nghiệp vụ.
            {
                entity.HasQueryFilter(p => !p.IsDeleted && !p.Manufacturer.IsDeleted); // Cấu hình bộ lọc toàn cục (Global Query Filter) để tự động lọc: p => !p.IsDeleted && !p.Manufacturer.IsDeleted.
            }); // Thực thi dòng lệnh nghiệp vụ.

            // --- STOREFRONT: BANNER ---
            modelBuilder.Entity<Banner>(entity => // Thực thi dòng lệnh nghiệp vụ.
            {
                entity.HasQueryFilter(b => !b.IsDeleted); // Cấu hình bộ lọc toàn cục (Global Query Filter) để tự động lọc: b => !b.IsDeleted.
                entity.HasIndex(b => new { b.IsActive, b.SortOrder }); // Cấu hình chỉ mục (Index) cho thực thể trên thuộc tính b => new { b.IsActive, b.SortOrder }.
            }); // Thực thi dòng lệnh nghiệp vụ.

            // --- INVENTORY ---
            // Global Query Filter: Tự động bỏ qua Supplier đã bị xoá mềm
            modelBuilder.Entity<Supplier>(entity => // Thực thi dòng lệnh nghiệp vụ.
            {
                entity.HasQueryFilter(s => !s.IsDeleted); // Cấu hình bộ lọc toàn cục (Global Query Filter) để tự động lọc: s => !s.IsDeleted.
            }); // Thực thi dòng lệnh nghiệp vụ.

            modelBuilder.Entity<ProductSerial>(entity => // Thực thi dòng lệnh nghiệp vụ.
            {
                entity.HasIndex(s => s.SerialNumber).IsUnique(); // Cấu hình chỉ mục độc nhất (Unique Index) cho thực thể trên thuộc tính s => s.SerialNumber.
                entity.HasIndex(s => new { s.VariantId, s.Status }); // Cấu hình chỉ mục (Index) cho thực thể trên thuộc tính s => new { s.VariantId, s.Status }.
            }); // Thực thi dòng lệnh nghiệp vụ.

            modelBuilder.Entity<ImportReceipt>(entity => // Thực thi dòng lệnh nghiệp vụ.
            {
                entity.HasQueryFilter(r => !r.IsDeleted && !r.Supplier.IsDeleted); // Cấu hình bộ lọc toàn cục (Global Query Filter) để tự động lọc: r => !r.IsDeleted && !r.Supplier.IsDeleted.
                entity.HasIndex(r => r.ReceiptCode).IsUnique(); // Cấu hình chỉ mục độc nhất (Unique Index) cho thực thể trên thuộc tính r => r.ReceiptCode.
                entity.Property(e => e.TotalAmount).HasColumnType("decimal(18,2)"); // Cấu hình kiểu dữ liệu trong CSDL là 'decimal(18,2)'.
            }); // Thực thi dòng lệnh nghiệp vụ.

            modelBuilder.Entity<ImportReceiptDetail>(entity => // Thực thi dòng lệnh nghiệp vụ.
            {
                entity.HasIndex(d => d.ReceiptId); // Cấu hình chỉ mục (Index) cho thực thể trên thuộc tính d => d.ReceiptId.
                entity.HasIndex(d => d.VariantId); // Cấu hình chỉ mục (Index) cho thực thể trên thuộc tính d => d.VariantId.
                entity.Property(e => e.ImportPrice).HasColumnType("decimal(18,2)"); // Cấu hình kiểu dữ liệu trong CSDL là 'decimal(18,2)'.
            }); // Thực thi dòng lệnh nghiệp vụ.

            modelBuilder.Entity<InventoryCheck>(entity => // Thực thi dòng lệnh nghiệp vụ.
            {
                entity.HasQueryFilter(c => !c.IsDeleted); // Cấu hình bộ lọc toàn cục (Global Query Filter) để tự động lọc: c => !c.IsDeleted.
                entity.HasIndex(c => c.CheckCode).IsUnique(); // Cấu hình chỉ mục độc nhất (Unique Index) cho thực thể trên thuộc tính c => c.CheckCode.
                entity.HasIndex(c => c.Status); // Cấu hình chỉ mục (Index) cho thực thể trên thuộc tính c => c.Status.
                entity.HasIndex(c => c.CheckDate); // Cấu hình chỉ mục (Index) cho thực thể trên thuộc tính c => c.CheckDate.

                entity.HasOne(c => c.ScopeCategory) // Cấu hình quan hệ một-nhiều, bắt đầu với một phía thực thể: c => c.ScopeCategory.
                      .WithMany() // Cấu hình quan hệ liên kết với thực thể nhiều phía tương ứng: .
                      .HasForeignKey(c => c.ScopeCategoryId) // Chỉ định thuộc tính khóa ngoại liên kết cho quan hệ: c => c.ScopeCategoryId.
                      .OnDelete(DeleteBehavior.NoAction); // Cấu hình hành vi xóa tự động khi bản ghi chính bị xóa: DeleteBehavior.NoAction.
            }); // Thực thi dòng lệnh nghiệp vụ.

            modelBuilder.Entity<InventoryCheckDetail>(entity => // Thực thi dòng lệnh nghiệp vụ.
            {
                entity.HasQueryFilter(d => !d.Check.IsDeleted); // Cấu hình bộ lọc toàn cục (Global Query Filter) để tự động lọc: d => !d.Check.IsDeleted.
                entity.HasIndex(d => d.CheckId); // Cấu hình chỉ mục (Index) cho thực thể trên thuộc tính d => d.CheckId.
                entity.HasIndex(d => d.VariantId); // Cấu hình chỉ mục (Index) cho thực thể trên thuộc tính d => d.VariantId.

                entity.Property(e => e.Difference) // Thực thi dòng lệnh nghiệp vụ.
                      .HasComputedColumnSql("([ActualQuantity] - [SystemQuantity])"); // Cấu hình cột tính toán tự động dựa trên công thức SQL '([ActualQuantity] - [SystemQuantity])'.
            }); // Thực thi dòng lệnh nghiệp vụ.

            modelBuilder.Entity<InventoryCheckDetailSerial>(entity => // Thực thi dòng lệnh nghiệp vụ.
            {
                entity.HasQueryFilter(s => !s.Check.IsDeleted); // Cấu hình bộ lọc toàn cục (Global Query Filter) để tự động lọc: s => !s.Check.IsDeleted.
                entity.HasIndex(s => s.CheckId); // Cấu hình chỉ mục (Index) cho thực thể trên thuộc tính s => s.CheckId.
                entity.HasIndex(s => new { s.CheckId, s.ScanStatus }); // Cấu hình chỉ mục (Index) cho thực thể trên thuộc tính s => new { s.CheckId, s.ScanStatus }.

                // Chống quét trùng trong cùng 1 phiếu
                entity.HasIndex(s => new { s.CheckId, s.SerialNumberRaw }) // Cấu hình chỉ mục (Index) cho thực thể trên thuộc tính s => new { s.CheckId, s.SerialNumberRaw }.
                      .IsUnique() // Cấu hình chỉ mục này là độc nhất (Unique index).
                      .HasDatabaseName("UQ_InventoryCheckDetailSerials_CheckId_SerialNumberRaw"); // Đặt tên cho chỉ mục dưới cơ sở dữ liệu là 'UQ_InventoryCheckDetailSerials_CheckId_SerialNumberRaw'.

                // FK: CheckId → InventoryChecks (cascade delete: xóa phiếu thì xóa serials)
                entity.HasOne(s => s.Check) // Cấu hình quan hệ một-nhiều, bắt đầu với một phía thực thể: s => s.Check.
                      .WithMany(c => c.DetailSerials) // Cấu hình quan hệ liên kết với thực thể nhiều phía tương ứng: c => c.DetailSerials.
                      .HasForeignKey(s => s.CheckId) // Chỉ định thuộc tính khóa ngoại liên kết cho quan hệ: s => s.CheckId.
                      .OnDelete(DeleteBehavior.Cascade); // Cấu hình hành vi xóa tự động khi bản ghi chính bị xóa: DeleteBehavior.Cascade.

                // FK: DetailId → InventoryCheckDetails (NoAction: detail serial có thể null khi UnknownSurplus)
                entity.HasOne(s => s.Detail) // Cấu hình quan hệ một-nhiều, bắt đầu với một phía thực thể: s => s.Detail.
                      .WithMany(d => d.DetailSerials) // Cấu hình quan hệ liên kết với thực thể nhiều phía tương ứng: d => d.DetailSerials.
                      .HasForeignKey(s => s.DetailId) // Chỉ định thuộc tính khóa ngoại liên kết cho quan hệ: s => s.DetailId.
                      .OnDelete(DeleteBehavior.NoAction); // Cấu hình hành vi xóa tự động khi bản ghi chính bị xóa: DeleteBehavior.NoAction.

                // FK: SerialId → ProductSerials (NoAction: tránh multiple cascade qua ProductSerial)
                entity.HasOne(s => s.Serial) // Cấu hình quan hệ một-nhiều, bắt đầu với một phía thực thể: s => s.Serial.
                      .WithMany() // Cấu hình quan hệ liên kết với thực thể nhiều phía tương ứng: .
                      .HasForeignKey(s => s.SerialId) // Chỉ định thuộc tính khóa ngoại liên kết cho quan hệ: s => s.SerialId.
                      .OnDelete(DeleteBehavior.NoAction); // Cấu hình hành vi xóa tự động khi bản ghi chính bị xóa: DeleteBehavior.NoAction.

                // FK: VariantId → ProductVariants (NoAction)
                entity.HasOne(s => s.Variant) // Cấu hình quan hệ một-nhiều, bắt đầu với một phía thực thể: s => s.Variant.
                      .WithMany() // Cấu hình quan hệ liên kết với thực thể nhiều phía tương ứng: .
                      .HasForeignKey(s => s.VariantId) // Chỉ định thuộc tính khóa ngoại liên kết cho quan hệ: s => s.VariantId.
                      .OnDelete(DeleteBehavior.NoAction); // Cấu hình hành vi xóa tự động khi bản ghi chính bị xóa: DeleteBehavior.NoAction.
            }); // Thực thi dòng lệnh nghiệp vụ.

            modelBuilder.Entity<InventoryAdjustmentLog>(entity => // Thực thi dòng lệnh nghiệp vụ.
            {
                entity.HasQueryFilter(l => !l.AuditCheck.IsDeleted); // Cấu hình bộ lọc toàn cục (Global Query Filter) để tự động lọc: l => !l.AuditCheck.IsDeleted.
                entity.HasIndex(l => l.AuditCheckId); // Cấu hình chỉ mục (Index) cho thực thể trên thuộc tính l => l.AuditCheckId.
                entity.HasIndex(l => l.AdjustedDate); // Cấu hình chỉ mục (Index) cho thực thể trên thuộc tính l => l.AdjustedDate.
                entity.HasIndex(l => l.SerialId); // Cấu hình chỉ mục (Index) cho thực thể trên thuộc tính l => l.SerialId.

                entity.Property(e => e.CostImpact).HasColumnType("decimal(18,2)"); // Cấu hình kiểu dữ liệu trong CSDL là 'decimal(18,2)'.

                // FK: AuditCheckId → InventoryChecks (NoAction: log phải tồn tại độc lập với phiếu)
                entity.HasOne(l => l.AuditCheck) // Cấu hình quan hệ một-nhiều, bắt đầu với một phía thực thể: l => l.AuditCheck.
                      .WithMany() // Cấu hình quan hệ liên kết với thực thể nhiều phía tương ứng: .
                      .HasForeignKey(l => l.AuditCheckId) // Chỉ định thuộc tính khóa ngoại liên kết cho quan hệ: l => l.AuditCheckId.
                      .OnDelete(DeleteBehavior.NoAction); // Cấu hình hành vi xóa tự động khi bản ghi chính bị xóa: DeleteBehavior.NoAction.

                // FK: SerialId → ProductSerials (NoAction)
                entity.HasOne(l => l.Serial) // Cấu hình quan hệ một-nhiều, bắt đầu với một phía thực thể: l => l.Serial.
                      .WithMany() // Cấu hình quan hệ liên kết với thực thể nhiều phía tương ứng: .
                      .HasForeignKey(l => l.SerialId) // Chỉ định thuộc tính khóa ngoại liên kết cho quan hệ: l => l.SerialId.
                      .OnDelete(DeleteBehavior.NoAction); // Cấu hình hành vi xóa tự động khi bản ghi chính bị xóa: DeleteBehavior.NoAction.

                // FK: VariantId → ProductVariants (NoAction)
                entity.HasOne(l => l.Variant) // Cấu hình quan hệ một-nhiều, bắt đầu với một phía thực thể: l => l.Variant.
                      .WithMany() // Cấu hình quan hệ liên kết với thực thể nhiều phía tương ứng: .
                      .HasForeignKey(l => l.VariantId) // Chỉ định thuộc tính khóa ngoại liên kết cho quan hệ: l => l.VariantId.
                      .OnDelete(DeleteBehavior.NoAction); // Cấu hình hành vi xóa tự động khi bản ghi chính bị xóa: DeleteBehavior.NoAction.
            }); // Thực thi dòng lệnh nghiệp vụ.

            // --- SALE ---
            modelBuilder.Entity<Voucher>(entity => // Thực thi dòng lệnh nghiệp vụ.
            {
                entity.HasQueryFilter(v => !v.IsDeleted); // Cấu hình bộ lọc toàn cục (Global Query Filter) để tự động lọc: v => !v.IsDeleted.

                entity.ToTable(t => // Ánh xạ thực thể cấu hình sang tên bảng 'bảng' trong cơ sở dữ liệu.
                {
                    t.HasCheckConstraint("CK_Vouchers_Date", "[EndDate] >= [StartDate]"); // Thực thi dòng lệnh nghiệp vụ.
                    // Quantity nullable: null = unlimited
                    t.HasCheckConstraint("CK_Vouchers_Quantity", "[Quantity] IS NULL OR [UsedCount] <= [Quantity]"); // Thực thi dòng lệnh nghiệp vụ.
                }); // Thực thi dòng lệnh nghiệp vụ.
                entity.HasIndex(v => v.Code).IsUnique(); // Cấu hình chỉ mục độc nhất (Unique Index) cho thực thể trên thuộc tính v => v.Code.
                entity.Property(e => e.DiscountValue).HasColumnType("decimal(18,2)"); // Cấu hình kiểu dữ liệu trong CSDL là 'decimal(18,2)'.
                entity.Property(e => e.MinOrderValue).HasColumnType("decimal(18,2)"); // Cấu hình kiểu dữ liệu trong CSDL là 'decimal(18,2)'.
                entity.Property(e => e.MaxDiscountAmount).HasColumnType("decimal(18,2)"); // Cấu hình kiểu dữ liệu trong CSDL là 'decimal(18,2)'.
            }); // Thực thi dòng lệnh nghiệp vụ.

            modelBuilder.Entity<VoucherCategory>(entity => // Thực thi dòng lệnh nghiệp vụ.
            {
                entity.HasQueryFilter(vc => !vc.Voucher.IsDeleted); // Cấu hình bộ lọc toàn cục (Global Query Filter) để tự động lọc: vc => !vc.Voucher.IsDeleted.
                entity.HasKey(vc => new { vc.VoucherId, vc.CategoryId }); // Thiết lập khóa chính của thực thể là thuộc tính vc => new { vc.VoucherId, vc.CategoryId }.

                entity.HasOne(vc => vc.Voucher) // Cấu hình quan hệ một-nhiều, bắt đầu với một phía thực thể: vc => vc.Voucher.
                      .WithMany(v => v.VoucherCategories) // Cấu hình quan hệ liên kết với thực thể nhiều phía tương ứng: v => v.VoucherCategories.
                      .HasForeignKey(vc => vc.VoucherId) // Chỉ định thuộc tính khóa ngoại liên kết cho quan hệ: vc => vc.VoucherId.
                      .OnDelete(DeleteBehavior.Cascade); // Cấu hình hành vi xóa tự động khi bản ghi chính bị xóa: DeleteBehavior.Cascade.

                entity.HasOne(vc => vc.Category) // Cấu hình quan hệ một-nhiều, bắt đầu với một phía thực thể: vc => vc.Category.
                      .WithMany() // Cấu hình quan hệ liên kết với thực thể nhiều phía tương ứng: .
                      .HasForeignKey(vc => vc.CategoryId) // Chỉ định thuộc tính khóa ngoại liên kết cho quan hệ: vc => vc.CategoryId.
                      .OnDelete(DeleteBehavior.Cascade); // Cấu hình hành vi xóa tự động khi bản ghi chính bị xóa: DeleteBehavior.Cascade.
            }); // Thực thi dòng lệnh nghiệp vụ.

            modelBuilder.Entity<Order>(entity => // Thực thi dòng lệnh nghiệp vụ.
            {
                entity.HasIndex(o => o.OrderCode).IsUnique(); // Cấu hình chỉ mục độc nhất (Unique Index) cho thực thể trên thuộc tính o => o.OrderCode.
                entity.HasIndex(o => o.UserId); // Cấu hình chỉ mục (Index) cho thực thể trên thuộc tính o => o.UserId.
                entity.HasIndex(o => o.OrderDate); // Cấu hình chỉ mục (Index) cho thực thể trên thuộc tính o => o.OrderDate.

                entity.Property(e => e.SubTotal).HasColumnType("decimal(18,2)"); // Cấu hình kiểu dữ liệu trong CSDL là 'decimal(18,2)'.
                entity.Property(e => e.ShippingFee).HasColumnType("decimal(18,2)"); // Cấu hình kiểu dữ liệu trong CSDL là 'decimal(18,2)'.
                entity.Property(e => e.DiscountAmount).HasColumnType("decimal(18,2)"); // Cấu hình kiểu dữ liệu trong CSDL là 'decimal(18,2)'.
                entity.Property(e => e.TotalAmount).HasColumnType("decimal(18,2)"); // Cấu hình kiểu dữ liệu trong CSDL là 'decimal(18,2)'.

                entity.HasOne(o => o.User) // Cấu hình quan hệ một-nhiều, bắt đầu với một phía thực thể: o => o.User.
                    .WithMany() // Cấu hình quan hệ liên kết với thực thể nhiều phía tương ứng: .
                    .HasForeignKey(o => o.UserId) // Chỉ định thuộc tính khóa ngoại liên kết cho quan hệ: o => o.UserId.
                    .OnDelete(DeleteBehavior.NoAction); // Cấu hình hành vi xóa tự động khi bản ghi chính bị xóa: DeleteBehavior.NoAction.

                entity.HasOne<AppUser>() // Thực thi dòng lệnh nghiệp vụ.
                    .WithMany() // Cấu hình quan hệ liên kết với thực thể nhiều phía tương ứng: .
                    .HasForeignKey(o => o.EmployeeId) // Chỉ định thuộc tính khóa ngoại liên kết cho quan hệ: o => o.EmployeeId.
                    .OnDelete(DeleteBehavior.NoAction); // Cấu hình hành vi xóa tự động khi bản ghi chính bị xóa: DeleteBehavior.NoAction.
            }); // Thực thi dòng lệnh nghiệp vụ.

            modelBuilder.Entity<VoucherUsage>(entity => // Thực thi dòng lệnh nghiệp vụ.
            {
                entity.HasQueryFilter(vu => !vu.Voucher.IsDeleted); // Cấu hình bộ lọc toàn cục (Global Query Filter) để tự động lọc: vu => !vu.Voucher.IsDeleted.
                // Non-unique index: MaxUsesPerUser cho phép dùng nhiều lần; check bằng count trong service
                entity.HasIndex(vu => new { vu.UserId, vu.VoucherId }) // Cấu hình chỉ mục (Index) cho thực thể trên thuộc tính vu => new { vu.UserId, vu.VoucherId }.
                      .HasDatabaseName("IX_VoucherUsages_UserId_VoucherId"); // Đặt tên cho chỉ mục dưới cơ sở dữ liệu là 'IX_VoucherUsages_UserId_VoucherId'.

                // Index cho truy vấn theo OrderId
                entity.HasIndex(vu => vu.OrderId); // Cấu hình chỉ mục (Index) cho thực thể trên thuộc tính vu => vu.OrderId.

                entity.Property(e => e.DiscountApplied).HasColumnType("decimal(18,2)"); // Cấu hình kiểu dữ liệu trong CSDL là 'decimal(18,2)'.

                // FK -> Voucher
                entity.HasOne(vu => vu.Voucher) // Cấu hình quan hệ một-nhiều, bắt đầu với một phía thực thể: vu => vu.Voucher.
                      .WithMany(v => v.VoucherUsages) // Cấu hình quan hệ liên kết với thực thể nhiều phía tương ứng: v => v.VoucherUsages.
                      .HasForeignKey(vu => vu.VoucherId) // Chỉ định thuộc tính khóa ngoại liên kết cho quan hệ: vu => vu.VoucherId.
                      .OnDelete(DeleteBehavior.NoAction); // Cấu hình hành vi xóa tự động khi bản ghi chính bị xóa: DeleteBehavior.NoAction.

                // FK -> User (AppUser)
                entity.HasOne(vu => vu.User) // Cấu hình quan hệ một-nhiều, bắt đầu với một phía thực thể: vu => vu.User.
                      .WithMany() // Cấu hình quan hệ liên kết với thực thể nhiều phía tương ứng: .
                      .HasForeignKey(vu => vu.UserId) // Chỉ định thuộc tính khóa ngoại liên kết cho quan hệ: vu => vu.UserId.
                      .OnDelete(DeleteBehavior.NoAction); // Cấu hình hành vi xóa tự động khi bản ghi chính bị xóa: DeleteBehavior.NoAction.

                // FK -> Order
                entity.HasOne(vu => vu.Order) // Cấu hình quan hệ một-nhiều, bắt đầu với một phía thực thể: vu => vu.Order.
                      .WithMany(o => o.VoucherUsages) // Cấu hình quan hệ liên kết với thực thể nhiều phía tương ứng: o => o.VoucherUsages.
                      .HasForeignKey(vu => vu.OrderId) // Chỉ định thuộc tính khóa ngoại liên kết cho quan hệ: vu => vu.OrderId.
                      .OnDelete(DeleteBehavior.Cascade); // Cấu hình hành vi xóa tự động khi bản ghi chính bị xóa: DeleteBehavior.Cascade.
            }); // Thực thi dòng lệnh nghiệp vụ.

            modelBuilder.Entity<OrderDetail>(entity => // Thực thi dòng lệnh nghiệp vụ.
            {
                entity.HasIndex(od => od.OrderId); // Cấu hình chỉ mục (Index) cho thực thể trên thuộc tính od => od.OrderId.
                entity.HasIndex(od => od.VariantId); // Cấu hình chỉ mục (Index) cho thực thể trên thuộc tính od => od.VariantId.
                entity.Property(e => e.UnitPrice).HasColumnType("decimal(18,2)"); // Cấu hình kiểu dữ liệu trong CSDL là 'decimal(18,2)'.
                entity.Property(e => e.TotalLine) // Thực thi dòng lệnh nghiệp vụ.
                    .HasColumnType("decimal(18,2)") // Cấu hình kiểu dữ liệu trong CSDL là 'decimal(18,2)'.
                    .HasComputedColumnSql("([Quantity] * [UnitPrice])"); // Cấu hình cột tính toán tự động dựa trên công thức SQL '([Quantity] * [UnitPrice])'.
            }); // Thực thi dòng lệnh nghiệp vụ.

            modelBuilder.Entity<OrderSerial>(entity => // Thực thi dòng lệnh nghiệp vụ.
            {
                entity.HasIndex(os => os.SerialId).IsUnique(); // Cấu hình chỉ mục độc nhất (Unique Index) cho thực thể trên thuộc tính os => os.SerialId.

                // FIX: SQL Server Error 1785 — Multiple cascade paths detected.
                // Cascade path 1: ProductVariant → ImportReceiptDetail → ImportReceipt → ProductSerial → OrderSerial (CASCADE)
                // Cascade path 2: ProductVariant → OrderDetail → OrderSerial (CASCADE)
                // Cả 2 đường đều cascade đến OrderSerial → SQL Server từ chối.
                // Giải pháp: Đặt NoAction cho cả 2 FK, xử lý xoá bằng Service logic.
                entity.HasOne(os => os.OrderDetail) // Cấu hình quan hệ một-nhiều, bắt đầu với một phía thực thể: os => os.OrderDetail.
                    .WithMany(od => od.OrderSerials) // Cấu hình quan hệ liên kết với thực thể nhiều phía tương ứng: od => od.OrderSerials.
                    .HasForeignKey(os => os.OrderDetailId) // Chỉ định thuộc tính khóa ngoại liên kết cho quan hệ: os => os.OrderDetailId.
                    .OnDelete(DeleteBehavior.NoAction); // Cấu hình hành vi xóa tự động khi bản ghi chính bị xóa: DeleteBehavior.NoAction.

                entity.HasOne(os => os.Serial) // Cấu hình quan hệ một-nhiều, bắt đầu với một phía thực thể: os => os.Serial.
                    .WithMany() // Cấu hình quan hệ liên kết với thực thể nhiều phía tương ứng: .
                    .HasForeignKey(os => os.SerialId) // Chỉ định thuộc tính khóa ngoại liên kết cho quan hệ: os => os.SerialId.
                    .OnDelete(DeleteBehavior.NoAction); // Cấu hình hành vi xóa tự động khi bản ghi chính bị xóa: DeleteBehavior.NoAction.
            }); // Thực thi dòng lệnh nghiệp vụ.

            modelBuilder.Entity<Cart>(entity => // Thực thi dòng lệnh nghiệp vụ.
            {
                entity.HasIndex(c => new { c.UserId, c.VariantId }).IsUnique(); // Cấu hình chỉ mục độc nhất (Unique Index) cho thực thể trên thuộc tính c => new { c.UserId, c.VariantId }.
                entity.HasIndex(c => c.VariantId); // Cấu hình chỉ mục (Index) cho thực thể trên thuộc tính c => c.VariantId.
            }); // Thực thi dòng lệnh nghiệp vụ.

            modelBuilder.Entity<Warranty>(entity => // Thực thi dòng lệnh nghiệp vụ.
            {
                entity.HasIndex(w => w.SerialId); // Cấu hình chỉ mục (Index) cho thực thể trên thuộc tính w => w.SerialId.
                entity.HasIndex(w => w.CustomerId); // Cấu hình chỉ mục (Index) cho thực thể trên thuộc tính w => w.CustomerId.
                entity.HasIndex(w => w.OrderId); // Cấu hình chỉ mục (Index) cho thực thể trên thuộc tính w => w.OrderId.
            }); // Thực thi dòng lệnh nghiệp vụ.

            modelBuilder.Entity<ProductReview>(entity => // Thực thi dòng lệnh nghiệp vụ.
            {
                entity.HasQueryFilter(r => !r.IsDeleted); // Cấu hình bộ lọc toàn cục (Global Query Filter) để tự động lọc: r => !r.IsDeleted.

                entity.HasIndex(r => new { r.ProductId, r.UserId }) // Cấu hình chỉ mục (Index) cho thực thể trên thuộc tính r => new { r.ProductId, r.UserId }.
                      .IsUnique() // Cấu hình chỉ mục này là độc nhất (Unique index).
                      .HasDatabaseName("UQ_ProductReviews_ProductId_UserId"); // Đặt tên cho chỉ mục dưới cơ sở dữ liệu là 'UQ_ProductReviews_ProductId_UserId'.

                entity.HasIndex(r => r.ProductId) // Cấu hình chỉ mục (Index) cho thực thể trên thuộc tính r => r.ProductId.
                      .HasDatabaseName("IX_ProductReviews_ProductId"); // Đặt tên cho chỉ mục dưới cơ sở dữ liệu là 'IX_ProductReviews_ProductId'.

                entity.ToTable(t => // Ánh xạ thực thể cấu hình sang tên bảng 'bảng' trong cơ sở dữ liệu.
                    t.HasCheckConstraint("CK_ProductReviews_Rating", "[Rating] BETWEEN 1 AND 5")); // Thực thi dòng lệnh nghiệp vụ.

                entity.HasOne(r => r.Product) // Cấu hình quan hệ một-nhiều, bắt đầu với một phía thực thể: r => r.Product.
                      .WithMany() // Cấu hình quan hệ liên kết với thực thể nhiều phía tương ứng: .
                      .HasForeignKey(r => r.ProductId) // Chỉ định thuộc tính khóa ngoại liên kết cho quan hệ: r => r.ProductId.
                      .OnDelete(DeleteBehavior.Cascade); // Cấu hình hành vi xóa tự động khi bản ghi chính bị xóa: DeleteBehavior.Cascade.

                entity.HasOne(r => r.User) // Cấu hình quan hệ một-nhiều, bắt đầu với một phía thực thể: r => r.User.
                      .WithMany() // Cấu hình quan hệ liên kết với thực thể nhiều phía tương ứng: .
                      .HasForeignKey(r => r.UserId) // Chỉ định thuộc tính khóa ngoại liên kết cho quan hệ: r => r.UserId.
                      .OnDelete(DeleteBehavior.NoAction); // Cấu hình hành vi xóa tự động khi bản ghi chính bị xóa: DeleteBehavior.NoAction.
            }); // Thực thi dòng lệnh nghiệp vụ.

            // --- SERVICE TICKETS ---
            modelBuilder.Entity<ServiceTicket>(entity => // Thực thi dòng lệnh nghiệp vụ.
            {
                entity.HasQueryFilter(t => !t.IsDeleted); // Cấu hình bộ lọc toàn cục (Global Query Filter) để tự động lọc: t => !t.IsDeleted.
                entity.HasIndex(t => t.TicketCode).IsUnique(); // Cấu hình chỉ mục độc nhất (Unique Index) cho thực thể trên thuộc tính t => t.TicketCode.
                entity.HasIndex(t => t.SerialId); // Cấu hình chỉ mục (Index) cho thực thể trên thuộc tính t => t.SerialId.
                entity.HasIndex(t => t.Status); // Cấu hình chỉ mục (Index) cho thực thể trên thuộc tính t => t.Status.
                entity.HasIndex(t => t.CustomerId); // Cấu hình chỉ mục (Index) cho thực thể trên thuộc tính t => t.CustomerId.
                entity.HasIndex(t => t.AssignedEmployeeId); // Cấu hình chỉ mục (Index) cho thực thể trên thuộc tính t => t.AssignedEmployeeId.
                entity.HasIndex(t => t.IntakeDate); // Cấu hình chỉ mục (Index) cho thực thể trên thuộc tính t => t.IntakeDate.

                // Relationship: ServiceTicket -> ProductSerial
                entity.HasOne(t => t.Serial) // Cấu hình quan hệ một-nhiều, bắt đầu với một phía thực thể: t => t.Serial.
                    .WithMany() // Cấu hình quan hệ liên kết với thực thể nhiều phía tương ứng: .
                    .HasForeignKey(t => t.SerialId) // Chỉ định thuộc tính khóa ngoại liên kết cho quan hệ: t => t.SerialId.
                    .OnDelete(DeleteBehavior.NoAction); // Cấu hình hành vi xóa tự động khi bản ghi chính bị xóa: DeleteBehavior.NoAction.

                // Relationship: ServiceTicket -> Order
                entity.HasOne(t => t.OriginalOrder) // Cấu hình quan hệ một-nhiều, bắt đầu với một phía thực thể: t => t.OriginalOrder.
                    .WithMany() // Cấu hình quan hệ liên kết với thực thể nhiều phía tương ứng: .
                    .HasForeignKey(t => t.OriginalOrderId) // Chỉ định thuộc tính khóa ngoại liên kết cho quan hệ: t => t.OriginalOrderId.
                    .OnDelete(DeleteBehavior.NoAction); // Cấu hình hành vi xóa tự động khi bản ghi chính bị xóa: DeleteBehavior.NoAction.

                // Relationship: ServiceTicket -> Customer (AppUser)
                entity.HasOne(t => t.Customer) // Cấu hình quan hệ một-nhiều, bắt đầu với một phía thực thể: t => t.Customer.
                    .WithMany() // Cấu hình quan hệ liên kết với thực thể nhiều phía tương ứng: .
                    .HasForeignKey(t => t.CustomerId) // Chỉ định thuộc tính khóa ngoại liên kết cho quan hệ: t => t.CustomerId.
                    .OnDelete(DeleteBehavior.NoAction); // Cấu hình hành vi xóa tự động khi bản ghi chính bị xóa: DeleteBehavior.NoAction.

                // Relationship: ServiceTicket -> ReplacementSerial
                entity.HasOne(t => t.ReplacementSerial) // Cấu hình quan hệ một-nhiều, bắt đầu với một phía thực thể: t => t.ReplacementSerial.
                    .WithMany() // Cấu hình quan hệ liên kết với thực thể nhiều phía tương ứng: .
                    .HasForeignKey(t => t.ReplacementSerialId) // Chỉ định thuộc tính khóa ngoại liên kết cho quan hệ: t => t.ReplacementSerialId.
                    .OnDelete(DeleteBehavior.NoAction); // Cấu hình hành vi xóa tự động khi bản ghi chính bị xóa: DeleteBehavior.NoAction.

                // Navigation properties
                entity.HasMany(t => t.StatusHistory) // Cấu hình quan hệ nhiều-một, bắt đầu với phía nhiều thực thể: t => t.StatusHistory.
                    .WithOne(h => h.Ticket) // Cấu hình quan hệ liên kết với thực thể một phía tương ứng: h => h.Ticket.
                    .HasForeignKey(h => h.TicketId) // Chỉ định thuộc tính khóa ngoại liên kết cho quan hệ: h => h.TicketId.
                    .OnDelete(DeleteBehavior.Cascade); // Cấu hình hành vi xóa tự động khi bản ghi chính bị xóa: DeleteBehavior.Cascade.

                entity.HasMany(t => t.Quotations) // Cấu hình quan hệ nhiều-một, bắt đầu với phía nhiều thực thể: t => t.Quotations.
                    .WithOne(q => q.Ticket) // Cấu hình quan hệ liên kết với thực thể một phía tương ứng: q => q.Ticket.
                    .HasForeignKey(q => q.TicketId) // Chỉ định thuộc tính khóa ngoại liên kết cho quan hệ: q => q.TicketId.
                    .OnDelete(DeleteBehavior.Cascade); // Cấu hình hành vi xóa tự động khi bản ghi chính bị xóa: DeleteBehavior.Cascade.

                entity.HasOne(t => t.RmaShipment) // Cấu hình quan hệ một-nhiều, bắt đầu với một phía thực thể: t => t.RmaShipment.
                    .WithOne(r => r.Ticket) // Cấu hình quan hệ liên kết với thực thể một phía tương ứng: r => r.Ticket.
                    .HasForeignKey<RmaShipment>(r => r.TicketId) // Chỉ định thuộc tính khóa ngoại liên kết cho quan hệ: r => r.TicketId.
                    .OnDelete(DeleteBehavior.Cascade); // Cấu hình hành vi xóa tự động khi bản ghi chính bị xóa: DeleteBehavior.Cascade.

                entity.HasOne(t => t.Invoice) // Cấu hình quan hệ một-nhiều, bắt đầu với một phía thực thể: t => t.Invoice.
                    .WithOne(i => i.Ticket) // Cấu hình quan hệ liên kết với thực thể một phía tương ứng: i => i.Ticket.
                    .HasForeignKey<ServiceInvoice>(i => i.TicketId) // Chỉ định thuộc tính khóa ngoại liên kết cho quan hệ: i => i.TicketId.
                    .OnDelete(DeleteBehavior.NoAction); // Cấu hình hành vi xóa tự động khi bản ghi chính bị xóa: DeleteBehavior.NoAction.
            }); // Thực thi dòng lệnh nghiệp vụ.

            modelBuilder.Entity<ServiceTicketStatusHistory>(entity => // Thực thi dòng lệnh nghiệp vụ.
            {
                entity.HasQueryFilter(h => !h.Ticket.IsDeleted); // Cấu hình bộ lọc toàn cục (Global Query Filter) để tự động lọc: h => !h.Ticket.IsDeleted.
                entity.HasIndex(h => new { h.TicketId, h.ChangedAt }); // Cấu hình chỉ mục (Index) cho thực thể trên thuộc tính h => new { h.TicketId, h.ChangedAt }.
            }); // Thực thi dòng lệnh nghiệp vụ.

            modelBuilder.Entity<Quotation>(entity => // Thực thi dòng lệnh nghiệp vụ.
            {
                entity.HasQueryFilter(q => !q.Ticket.IsDeleted); // Cấu hình bộ lọc toàn cục (Global Query Filter) để tự động lọc: q => !q.Ticket.IsDeleted.
                entity.HasMany(q => q.Items) // Cấu hình quan hệ nhiều-một, bắt đầu với phía nhiều thực thể: q => q.Items.
                    .WithOne(i => i.Quotation) // Cấu hình quan hệ liên kết với thực thể một phía tương ứng: i => i.Quotation.
                    .HasForeignKey(i => i.QuotationId) // Chỉ định thuộc tính khóa ngoại liên kết cho quan hệ: i => i.QuotationId.
                    .OnDelete(DeleteBehavior.Cascade); // Cấu hình hành vi xóa tự động khi bản ghi chính bị xóa: DeleteBehavior.Cascade.

                entity.Property(e => e.LaborCost).HasColumnType("decimal(18,2)"); // Cấu hình kiểu dữ liệu trong CSDL là 'decimal(18,2)'.
                entity.Property(e => e.PartsTotal).HasColumnType("decimal(18,2)"); // Cấu hình kiểu dữ liệu trong CSDL là 'decimal(18,2)'.
                entity.Property(e => e.GrandTotal).HasColumnType("decimal(18,2)"); // Cấu hình kiểu dữ liệu trong CSDL là 'decimal(18,2)'.
            }); // Thực thi dòng lệnh nghiệp vụ.

            modelBuilder.Entity<QuotationItem>(entity => // Thực thi dòng lệnh nghiệp vụ.
            {
                entity.Property(e => e.UnitPrice).HasColumnType("decimal(18,2)"); // Cấu hình kiểu dữ liệu trong CSDL là 'decimal(18,2)'.
                entity.Property(e => e.LineTotal) // Thực thi dòng lệnh nghiệp vụ.
                    .HasColumnType("decimal(18,2)") // Cấu hình kiểu dữ liệu trong CSDL là 'decimal(18,2)'.
                    .HasComputedColumnSql("([Quantity] * [UnitPrice])"); // Cấu hình cột tính toán tự động dựa trên công thức SQL '([Quantity] * [UnitPrice])'.
            }); // Thực thi dòng lệnh nghiệp vụ.

            modelBuilder.Entity<RmaShipment>(entity => // Thực thi dòng lệnh nghiệp vụ.
            {
                entity.HasQueryFilter(r => !r.Ticket.IsDeleted); // Cấu hình bộ lọc toàn cục (Global Query Filter) để tự động lọc: r => !r.Ticket.IsDeleted.
                entity.HasIndex(r => r.TicketId).IsUnique(); // Cấu hình chỉ mục độc nhất (Unique Index) cho thực thể trên thuộc tính r => r.TicketId.
            }); // Thực thi dòng lệnh nghiệp vụ.

            modelBuilder.Entity<ServiceInvoice>(entity => // Thực thi dòng lệnh nghiệp vụ.
            {
                // CRITICAL: ServiceInvoice does NOT use HasQueryFilter.
                // Financial records must survive ticket soft-delete.
                entity.HasIndex(i => i.InvoiceCode).IsUnique(); // Cấu hình chỉ mục độc nhất (Unique Index) cho thực thể trên thuộc tính i => i.InvoiceCode.
                entity.HasIndex(i => i.TicketId).IsUnique(); // Cấu hình chỉ mục độc nhất (Unique Index) cho thực thể trên thuộc tính i => i.TicketId.

                entity.Property(e => e.LaborCost).HasColumnType("decimal(18,2)"); // Cấu hình kiểu dữ liệu trong CSDL là 'decimal(18,2)'.
                entity.Property(e => e.PartsTotal).HasColumnType("decimal(18,2)"); // Cấu hình kiểu dữ liệu trong CSDL là 'decimal(18,2)'.
                entity.Property(e => e.GrandTotal).HasColumnType("decimal(18,2)"); // Cấu hình kiểu dữ liệu trong CSDL là 'decimal(18,2)'.

                // Relationship: ServiceInvoice -> Quotation (optional)
                entity.HasOne(i => i.Quotation) // Cấu hình quan hệ một-nhiều, bắt đầu với một phía thực thể: i => i.Quotation.
                    .WithMany() // Cấu hình quan hệ liên kết với thực thể nhiều phía tương ứng: .
                    .HasForeignKey(i => i.QuotationId) // Chỉ định thuộc tính khóa ngoại liên kết cho quan hệ: i => i.QuotationId.
                    .OnDelete(DeleteBehavior.NoAction); // Cấu hình hành vi xóa tự động khi bản ghi chính bị xóa: DeleteBehavior.NoAction.

                // Navigation: ServiceInvoice -> Items
                entity.HasMany(i => i.Items) // Cấu hình quan hệ nhiều-một, bắt đầu với phía nhiều thực thể: i => i.Items.
                    .WithOne(ii => ii.Invoice) // Cấu hình quan hệ liên kết với thực thể một phía tương ứng: ii => ii.Invoice.
                    .HasForeignKey(ii => ii.InvoiceId) // Chỉ định thuộc tính khóa ngoại liên kết cho quan hệ: ii => ii.InvoiceId.
                    .OnDelete(DeleteBehavior.Cascade); // Cấu hình hành vi xóa tự động khi bản ghi chính bị xóa: DeleteBehavior.Cascade.
            }); // Thực thi dòng lệnh nghiệp vụ.

            modelBuilder.Entity<ServiceInvoiceItem>(entity => // Thực thi dòng lệnh nghiệp vụ.
            {
                entity.Property(e => e.UnitPrice).HasColumnType("decimal(18,2)"); // Cấu hình kiểu dữ liệu trong CSDL là 'decimal(18,2)'.
                entity.Property(e => e.LineTotal) // Thực thi dòng lệnh nghiệp vụ.
                    .HasColumnType("decimal(18,2)") // Cấu hình kiểu dữ liệu trong CSDL là 'decimal(18,2)'.
                    .HasComputedColumnSql("([Quantity] * [UnitPrice])"); // Cấu hình cột tính toán tự động dựa trên công thức SQL '([Quantity] * [UnitPrice])'.
            }); // Thực thi dòng lệnh nghiệp vụ.

            modelBuilder.Entity<SerialRepairLog>(entity => // Thực thi dòng lệnh nghiệp vụ.
            {
                entity.HasIndex(l => l.SerialId); // Cấu hình chỉ mục (Index) cho thực thể trên thuộc tính l => l.SerialId.

                // Relationship: SerialRepairLog -> ProductSerial (required)
                entity.HasOne(l => l.Serial) // Cấu hình quan hệ một-nhiều, bắt đầu với một phía thực thể: l => l.Serial.
                    .WithMany() // Cấu hình quan hệ liên kết với thực thể nhiều phía tương ứng: .
                    .HasForeignKey(l => l.SerialId) // Chỉ định thuộc tính khóa ngoại liên kết cho quan hệ: l => l.SerialId.
                    .OnDelete(DeleteBehavior.NoAction); // Cấu hình hành vi xóa tự động khi bản ghi chính bị xóa: DeleteBehavior.NoAction.

                // Relationship: SerialRepairLog -> ServiceTicket (optional, SetNull on delete)
                entity.HasOne(l => l.Ticket) // Cấu hình quan hệ một-nhiều, bắt đầu với một phía thực thể: l => l.Ticket.
                    .WithMany() // Cấu hình quan hệ liên kết với thực thể nhiều phía tương ứng: .
                    .HasForeignKey(l => l.TicketId) // Chỉ định thuộc tính khóa ngoại liên kết cho quan hệ: l => l.TicketId.
                    .OnDelete(DeleteBehavior.SetNull); // Cấu hình hành vi xóa tự động khi bản ghi chính bị xóa: DeleteBehavior.SetNull.

                // Relationship: SerialRepairLog -> ReplacedBySerial (optional)
                entity.HasOne(l => l.ReplacedBySerial) // Cấu hình quan hệ một-nhiều, bắt đầu với một phía thực thể: l => l.ReplacedBySerial.
                    .WithMany() // Cấu hình quan hệ liên kết với thực thể nhiều phía tương ứng: .
                    .HasForeignKey(l => l.ReplacedBySerialId) // Chỉ định thuộc tính khóa ngoại liên kết cho quan hệ: l => l.ReplacedBySerialId.
                    .OnDelete(DeleteBehavior.NoAction); // Cấu hình hành vi xóa tự động khi bản ghi chính bị xóa: DeleteBehavior.NoAction.
            }); // Thực thi dòng lệnh nghiệp vụ.
        }
    }
}

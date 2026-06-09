# Giải thích toàn bộ các file .cs trong thư mục Application

Thư mục `Application` là **tầng nghiệp vụ (Business Logic Layer)** trong kiến trúc Clean Architecture. Dưới đây là giải thích chi tiết từng thành phần.

---

## THỨ TỰ HỌC — Từ dễ đến khó

> **Tiêu chí đánh giá:**
> - Số lượng khái niệm C# mới cần nắm
> - Độ phức tạp nghiệp vụ (logic, điều kiện, vòng lặp)
> - Số lượng dependency (thư viện/service bên ngoài)
> - Độ dài file và số bước xử lý trong một phương thức

---

### NHÓM 1 — Rất dễ ★☆☆☆☆
> Chỉ khai báo class/interface đơn giản, không có logic. Học trước để nắm cú pháp cơ bản.

| # | File | Lý do dễ |
|---|------|---------|
| 1 | `Common/Exceptions/BusinessRuleException.cs` | 6 dòng, chỉ kế thừa `Exception`, 1 constructor |
| 2 | `Common/Exceptions/ConflictException.cs` | 6 dòng, cấu trúc y hệt `BusinessRuleException` |
| 3 | `Common/Exceptions/ForbiddenException.cs` | 7 dòng, thêm khái niệm **default parameter** |
| 4 | `Common/Exceptions/NotFoundException.cs` | 9 dòng, thêm khái niệm **constructor nạp chồng** và **string interpolation** `$"..."` |
| 5 | `Storage/IStorageService.cs` | 6 dòng, 1 method, giới thiệu `Stream`, `CancellationToken` |
| 6 | `BuildPc/IBuildPcService.cs` | 8 dòng, 1 method, trả về `byte[]` thay vì `ApiResult` |

---

### NHÓM 2 — Dễ ★★☆☆☆
> Toàn bộ là **Interface** — chỉ khai báo chữ ký phương thức, không có logic. Học để hiểu thiết kế hợp đồng.

| # | File | Khái niệm chính |
|---|------|----------------|
| 7 | `Reviews/IProductReviewService.cs` | Interface nhỏ nhất (3 methods) |
| 8 | `Suppliers/ISupplierService.cs` | CRUD cơ bản: Get/Create/Update/Delete |
| 9 | `Manufacturers/IManufacturerService.cs` | CRUD + dropdown `GetAllForDropdownAsync` |
| 10 | `Banners/IBannerService.cs` | CRUD + `PagedResult<T>` |
| 11 | `Inventory/IInventoryExportService.cs` | 2 methods, interface xuất kho nhỏ gọn |
| 12 | `Inventory/IInventoryExportService.cs` | 3 methods, phiếu nhập kho |
| 13 | `ImportReceipts/IImportReceiptService.cs` | 3 methods, khái niệm Transaction |
| 14 | `ProductSerials/IProductSerialService.cs` | 5 methods, quản lý Serial |
| 15 | `Storage/IStorageService.cs` | 1 method, upload file |
| 16 | `Auth/IAuthService.cs` | Login, Refresh, Register, ChangePassword |
| 17 | `Cart/ICartService.cs` | 5 methods, `Guid userId` |
| 18 | `Categories/ICategoryService.cs` | `CategoryTreeDto` — giới thiệu kiểu cây |
| 19 | `Customers/ICustomerService.cs` | `DeactivateAsync`/`ReactivateAsync` — khóa/mở tài khoản |
| 20 | `Employees/IEmployeeService.cs` | Giống Customer + `GetTechniciansSimpleAsync` |
| 21 | `Analytics/IAnalyticsService.cs` | 6 methods thống kê, tham số `DateTime from, to` |
| 22 | `Products/IProductService.cs` | CRUD sản phẩm + biến thể lồng nhau |
| 23 | `Vouchers/IVoucherService.cs` | 8 methods — giới thiệu `ValidateVoucherCodeAsync` |
| 24 | `Pos/IPosService.cs` | 8 methods POS: scan, lookup, checkout, draft |
| 25 | `Orders/IOrderService.cs` | 11 methods — phân biệt Admin vs Customer ownership |
| 26 | `Storefront/IStorefrontService.cs` | 7 methods giao diện khách hàng, slug-based |
| 27 | `ServiceInvoices/IServiceInvoiceService.cs` | 4 methods, hóa đơn dịch vụ |
| 28 | `Inventory/IInventoryCheckService.cs` | 12 methods — workflow kiểm kê phức tạp |
| 29 | `ServiceTickets/IServiceTicketService.cs` | 25+ methods — workflow bảo hành/sửa chữa phức tạp nhất |

---

### NHÓM 3 — Trung bình ★★★☆☆
> File **Implementation** đơn giản: CRUD tiêu chuẩn, ít logic phức tạp.

| # | File | Khái niệm mới cần học |
|---|------|----------------------|
| 30 | `Storage/S3StorageService.cs` | `IAmazonS3`, `Stream`, `PutObjectRequest`, tạo S3 URL |
| 31 | `Suppliers/SupplierService.cs` | CRUD cơ bản, `MapToDto`, Soft Delete — **mẫu chuẩn nhất** |
| 32 | `Manufacturers/ManufacturerService.cs` | CRUD + kiểm tra trùng tên `IsDuplicateNameAsync` + ràng buộc sản phẩm |
| 33 | `Reviews/ProductReviewService.cs` | Tạo đánh giá, kiểm tra sở hữu `review.UserId != userId`, `ApiErrorCode.Forbidden` |
| 34 | `Banners/BannerService.cs` | **FluentValidation**, `GetActiveAsync`, `MapToDto` lambda, Soft Delete |
| 35 | `BuildPc/BuildPcService.cs` | **EPPlus Excel**: merge cells, style, color, number format, `byte[]` |

---

### NHÓM 4 — Trung bình khá ★★★★☆
> Logic nghiệp vụ phức tạp hơn, nhiều dependency, kiểm tra điều kiện đa tầng.

| # | File | Khái niệm mới cần học |
|---|------|----------------------|
| 36 | `Cart/CartService.cs` | Null-conditional `?.`, upsert pattern, kiểm tra tồn kho, `RemoveRange` |
| 37 | `Customers/CustomerService.cs` | **ASP.NET Identity** (`UserManager`), **MemoryCache** `_cache.Remove(...)`, `GenerateRandomPassword` (Fisher-Yates shuffle), xóa RefreshToken khi khóa |
| 38 | `Employees/EmployeeService.cs` | **RoleManager**, thêm/xóa vai trò động (`AddToRoleAsync`/`RemoveFromRoleAsync`), `ToHashSet()` |
| 39 | `Auth/AuthService.cs` | **JWT** (`JwtSecurityToken`, `SigningCredentials`), **SHA256** hash, **Refresh Token Rotation**, `ValidateLifetime=false`, pattern matching `is not JwtSecurityToken` |
| 40 | `Categories/CategoryService.cs` | **Đệ quy** (`BuildTree`, `UpdateSubtreeLevelsAsync`), **phát hiện vòng** (Circular Reference + `HashSet`), `ThenBy` |

---

### NHÓM 5 — Khó ★★★★★
> Logic nghiệp vụ rất phức tạp, nhiều bảng join, multi-step transaction, state machine.

| # | File | Khái niệm mới cần học |
|---|------|----------------------|
| 41 | `Analytics/AnalyticsService.cs` | **Value Tuple** + Destructuring, **LINQ query syntax join phức hợp** (composite key), `ToDictionaryAsync`, vòng lặp tạo điểm biểu đồ, `GroupBy + g.Key` |
| 42 | `Products/ProductService.cs` | Quản lý Product + Variant lồng nhau, nhiều bảng join |
| 43 | `Storefront/StorefrontService.cs` | Truy vấn sản phẩm public, tìm kiếm đa tiêu chí, slug-based |
| 44 | `ImportReceipts/ImportReceiptService.cs` | **DbTransaction**, tạo hàng loạt Serial, cập nhật tồn kho |
| 45 | `Inventory/InventorySyncService.cs` | Đồng bộ tồn kho từ Serial về ProductVariant |
| 46 | `ProductSerials/ProductSerialService.cs` | Vòng đời Serial (Available → Reserved → Sold → Defective) |
| 47 | `Inventory/InventoryExportService.cs` | Gắn Serial vào OrderDetail, cập nhật trạng thái đơn hàng |
| 48 | `Vouchers/VoucherService.cs` | Validate voucher đa quy tắc, stacking rules, tính giá trị giảm giá |
| 49 | `Orders/OrderService.cs` | Checkout + tạo đơn hoàn chỉnh, áp dụng voucher, gán Serial, transaction |
| 50 | `Pos/PosService.cs` | Scan Serial tại quầy, Draft order, Checkout POS |
| 51 | `Inventory/InventoryCheckService.cs` | **State machine** kiểm kê: Draft → Submitted → Approved/Rejected, scan Serial, cân bằng tồn kho |
| 52 | `ServiceTickets/WarrantyEvaluator.cs` | Logic đánh giá bảo hành từ lịch sử Serial |
| 53 | `ServiceInvoices/ServiceInvoiceService.cs` | Hóa đơn dịch vụ kỹ thuật, đánh dấu đã thanh toán |
| 54 | `ServiceTickets/ServiceTicketService.cs` | **State machine phức tạp nhất**: 15+ trạng thái, RMA workflow, 1-đổi-1, phân công kỹ thuật viên, lịch sử trạng thái |

---

### Lộ trình học đề xuất

```
Tuần 1: Nhóm 1 + Nhóm 2 (các Interface)
  → Nắm vững: namespace, interface, Task<T>, ApiResult<T>, Guid, async/await

Tuần 2: Nhóm 3 (Implementation đơn giản)
  → Nắm vững: CRUD, MapToDto, Soft Delete, FluentValidation, Primary Constructor

Tuần 3: Nhóm 4 (Logic vừa)
  → Nắm vững: Identity, JWT, Cache, Đệ quy, Null-conditional ?.

Tuần 4+: Nhóm 5 (Logic phức tạp)
  → Nắm vững: LINQ query syntax, Transaction, State machine, Composite key join
```

---

---

## 1. Các khái niệm C# dùng xuyên suốt

### Primary Constructor (C# 12)
```csharp
public class BannerService(
    IBannerRepository bannerRepo,
    ILogger<BannerService> logger) : IBannerService
```
- **Primary Constructor**: Khai báo tham số ngay trên dòng class, thay thế cho constructor truyền thống `public BannerService(...) { ... }`
- Các tham số được tiêm tự động bởi **Dependency Injection container** khi ứng dụng khởi động
- `: IBannerService` → lớp này **triển khai** (implement) interface `IBannerService`

### Gán trường `readonly` + kiểm tra null
```csharp
private readonly IBannerRepository _bannerRepo =
    bannerRepo ?? throw new ArgumentNullException(nameof(bannerRepo));
```
- `private readonly` → trường không thể thay đổi sau khi gán (bất biến)
- `??` (**null-coalescing**): nếu `bannerRepo` là null thì ném ngoại lệ `ArgumentNullException`
- `nameof(bannerRepo)` → trả về chuỗi `"bannerRepo"` mà không hardcode chuỗi ký tự

### Async/Await
```csharp
public async Task<ApiResult<BannerDto>> GetByIdAsync(int id)
{
    var banner = await _bannerRepo.GetByIdAsync(id);
}
```
- `async` → đánh dấu phương thức bất đồng bộ
- `await` → chờ kết quả trả về từ tác vụ bất đồng bộ mà không chặn luồng chính
- `Task<T>` → kiểu trả về đại diện cho một tác vụ bất đồng bộ cho ra kết quả kiểu `T`

### `ApiResult<T>` — Wrapper phản hồi nhất quán
```csharp
return ApiResult<BannerDto>.Ok(dto);
return ApiResult<BannerDto>.Fail("Không tìm thấy.", ApiErrorCode.NotFound);
```
- Mọi phương thức đều bọc kết quả trong `ApiResult<T>` để Controller/API tầng trên có thể xử lý đồng nhất (thành công, thất bại, mã lỗi)

---

## 2. Common/Exceptions/ — Ngoại lệ tùy chỉnh

| File | Kế thừa | Mục đích |
|------|---------|----------|
| `BusinessRuleException.cs` | `Exception` | Vi phạm quy tắc nghiệp vụ (VD: không đủ tồn kho) |
| `ConflictException.cs` | `Exception` | Xung đột dữ liệu (VD: email đã tồn tại) |
| `ForbiddenException.cs` | `Exception` | Không có quyền truy cập (HTTP 403), có **default message** |
| `NotFoundException.cs` | `Exception` | Không tìm thấy tài nguyên (HTTP 404), có **2 constructor nạp chồng** |

Cú pháp quan trọng trong `NotFoundException.cs`:
```csharp
public NotFoundException(string entityName, object key)
    : base($"Không tìm thấy {entityName} với ID '{key}'.") { }
```
- `: base(...)` → gọi **constructor của lớp cha** `Exception` với thông điệp lỗi
- `$"..."` → **chuỗi nội suy** (string interpolation) nhúng biến trực tiếp vào chuỗi

---

## 3. Auth/ — Xác thực & Phân quyền

### IAuthService.cs
```csharp
public interface IAuthService
{
    Task<ApiResult<TokenResponse>> LoginAsync(LoginRequest request);
    Task<ApiResult<TokenResponse>> RefreshTokenAsync(RefreshTokenRequest request);
    Task<ApiResult<bool>> RegisterAsync(RegisterCustomerRequest request);
    Task<ApiResult<bool>> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword);
}
```
- `interface` → **hợp đồng** chỉ khai báo chữ ký, không có phần thân — lớp implement sẽ tự viết logic
- `Guid userId` → kiểu định danh duy nhất toàn cầu, không thể đoán được như `int`

### AuthService.cs — Các điểm nổi bật

**JWT Token Generation:**
```csharp
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
var token = new JwtSecurityToken(issuer, audience, claims, expires, credentials);
return new JwtSecurityTokenHandler().WriteToken(token);
```
- `SymmetricSecurityKey` → khóa đối xứng mã hóa JWT
- `HmacSha256` → thuật toán băm ký số
- `JwtSecurityTokenHandler().WriteToken(token)` → chuyển đối tượng JWT thành **chuỗi văn bản** gửi về client

**Claims List:**
```csharp
var claims = new List<Claim>
{
    new(ClaimTypes.NameIdentifier, user.Id.ToString()),
    new(ClaimTypes.Email, user.Email ?? string.Empty),
    new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
};
```
- `Claim` → một cặp key-value nhúng trong JWT mang thông tin nhận dạng người dùng
- `?? string.Empty` → nếu `Email` null thì thay bằng chuỗi rỗng (tránh lỗi)
- `JwtRegisteredClaimNames.Jti` → mã định danh duy nhất của token, chống tấn công replay

**Refresh Token Rotation:**
```csharp
private static string GenerateRefreshToken()
{
    var randomBytes = new byte[64];
    using var rng = RandomNumberGenerator.Create();
    rng.GetBytes(randomBytes);
    return Convert.ToBase64String(randomBytes);
}
```
- `using var` → giải phóng tài nguyên `rng` tự động khi ra khỏi phạm vi
- `RandomNumberGenerator` → tạo số ngẫu nhiên **bảo mật mã hóa** (tốt hơn `Random`)
- `Convert.ToBase64String` → mã hóa byte[] thành chuỗi có thể truyền qua HTTP

**Băm SHA256 để lưu DB:**
```csharp
private static string HashToken(string token)
{
    var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
    return Convert.ToBase64String(bytes);
}
```
- Không lưu Refresh Token trần vào DB → lưu **bản băm một chiều** để bảo mật nếu DB bị lộ

**Bóc tách Claims từ Token hết hạn:**
```csharp
var tokenValidationParameters = new TokenValidationParameters
{
    ValidateLifetime = false // Không kiểm tra thời hạn sống — dùng để refresh token
};
var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
if (securityToken is not JwtSecurityToken jwtToken ||
    !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
        StringComparison.InvariantCultureIgnoreCase))
    return null;
```
- `ValidateLifetime = false` → cho phép giải mã token đã hết hạn để lấy Claims
- `out var securityToken` → tham số `out` trả về token đã parse dù không khai báo trước
- `is not JwtSecurityToken` → **pattern matching** kiểm tra kiểu và ép kiểu cùng lúc

---

## 4. Analytics/ — Thống kê

### IAnalyticsService.cs
```csharp
Task<ApiResult<AnalyticsSummaryDto>> GetSummaryAsync(DateTime from, DateTime to);
Task<ApiResult<RevenueTrendDto>> GetRevenueTrendAsync(DateTime from, DateTime to);
Task<ApiResult<OrderChannelDto>> GetOrderChannelsAsync(DateTime from, DateTime to);
Task<ApiResult<List<TopProductDto>>> GetTopProductsAsync(DateTime from, DateTime to, int top);
Task<ApiResult<List<CategoryRevenueDto>>> GetCategoryRevenueAsync(DateTime from, DateTime to, int top);
Task<ApiResult<InventorySummaryDto>> GetInventorySummaryAsync();
```

### AnalyticsService.cs — Các kỹ thuật nổi bật

**Tuple return từ hàm nội bộ:**
```csharp
private (bool valid, string error) ValidateRange(DateTime from, DateTime to)
{
    if (to < from) return (false, "Ngày kết thúc phải lớn hơn...");
    if ((to - from).TotalDays > 366) return (false, "Khoảng thời gian tối đa...");
    return (true, string.Empty);
}

// Gọi và giải nén Tuple:
var (valid, error) = ValidateRange(from, to);
```
- `(bool, string)` → **Value Tuple** trả về 2 giá trị cùng lúc, không cần class wrapper
- `var (valid, error) = ...` → **Destructuring** (giải nén) Tuple thành 2 biến riêng

**LINQ Join phức tạp để truy vết giá vốn:**
```csharp
var totalCost = await (
    from o in _context.Orders
    where o.Status == 3 ...
    join od in _context.OrderDetails on o.Id equals od.OrderId
    join os in _context.OrderSerials on od.Id equals os.OrderDetailId
    join ps in _context.ProductSerials on os.SerialId equals ps.Id
    join ird in _context.ImportReceiptDetails
        on new { ps.ImportReceiptId, ps.VariantId }
        equals new { ImportReceiptId = ird.ReceiptId, ird.VariantId }
    select (decimal?)ird.ImportPrice
).SumAsync() ?? 0m;
```
- `from ... join ... on ... equals` → **query syntax LINQ** (giống SQL)
- `on new { A, B } equals new { C, D }` → join theo **khóa phức hợp** (composite key)
- `(decimal?)` → ép kiểu nullable để `SumAsync()` có thể trả về null
- `?? 0m` → mặc định 0 nếu không có dữ liệu, `m` là hậu tố kiểu `decimal`

**Dictionary + vòng lặp tạo điểm biểu đồ:**
```csharp
var revenueByDay = await ... .ToDictionaryAsync(
    x => new DateTime(x.Key.Year, x.Key.Month, x.Key.Day),
    x => x.Revenue);

for (var day = from.Date; day <= to.Date; day = day.AddDays(1))
{
    var rev = revenueByDay.GetValueOrDefault(day, 0m);
    points.Add(new DailyRevenuePointDto { Label = day.ToString("dd/MM"), Revenue = rev });
}
```
- `ToDictionaryAsync(keySelector, valueSelector)` → chuyển IQueryable thành Dictionary
- `GetValueOrDefault(key, defaultValue)` → lấy giá trị, trả về mặc định nếu không có key
- `day.ToString("dd/MM")` → định dạng ngày thành chuỗi dạng `"01/06"`

**Thống kê Serial theo trạng thái:**
```csharp
var statusCounts = await _context.ProductSerials.AsNoTracking()
    .GroupBy(s => s.Status)
    .Select(g => new { Status = g.Key, Count = g.Count() })
    .ToListAsync();

TotalAvailable = statusCounts.FirstOrDefault(x => x.Status == 0)?.Count ?? 0, // Sẵn sàng
TotalReserved  = statusCounts.FirstOrDefault(x => x.Status == 1)?.Count ?? 0, // Tạm giữ
TotalSold      = statusCounts.FirstOrDefault(x => x.Status == 2)?.Count ?? 0, // Đã bán
TotalDefective = statusCounts.FirstOrDefault(x => x.Status == 3)?.Count ?? 0, // Lỗi/bảo hành
TotalReturned  = statusCounts.FirstOrDefault(x => x.Status == 4)?.Count ?? 0  // Trả nhà cung cấp
```
- `GroupBy(s => s.Status)` → nhóm dữ liệu theo giá trị cột Status
- `g.Key` → giá trị được dùng để nhóm (ở đây là giá trị Status)
- `?.Count` → null-conditional: nếu `FirstOrDefault` trả về null thì `?.Count` cũng là null

---

## 5. Banners/ — Quản lý Banner

### IBannerService.cs
```csharp
Task<ApiResult<PagedResult<BannerDto>>> GetPagedListAsync(BannerFilterRequest filter);
Task<ApiResult<BannerDto>> GetByIdAsync(int id);
Task<ApiResult<List<BannerPublicDto>>> GetActiveAsync();
Task<ApiResult<BannerDto>> CreateAsync(CreateBannerRequest request);
Task<ApiResult<BannerDto>> UpdateAsync(int id, UpdateBannerRequest request);
Task<ApiResult<bool>> DeleteAsync(int id);
```
- `PagedResult<BannerDto>` → kiểu generic chứa danh sách phân trang và metadata (tổng số, trang hiện tại)

### BannerService.cs

**FluentValidation:**
```csharp
var validation = await _createValidator.ValidateAsync(request);
if (!validation.IsValid)
    return ApiResult<BannerDto>.Fail(validation.Errors.First().ErrorMessage, ApiErrorCode.Validation);
```
- `IValidator<T>` → interface của thư viện FluentValidation, kiểm tra tính hợp lệ đầu vào
- `.Errors.First().ErrorMessage` → lấy thông báo lỗi validate đầu tiên gặp phải

**Xóa mềm (Soft Delete):**
```csharp
banner.IsDeleted   = true;
banner.DeletedDate = DateTime.UtcNow;
await _bannerRepo.SaveChangesAsync();
```
- Không xóa khỏi DB vật lý mà chỉ đặt cờ `IsDeleted = true` — dữ liệu vẫn được giữ lại để kiểm toán

**Lambda expression-bodied MapToDto:**
```csharp
private static BannerDto MapToDto(Banner entity) => new()
{
    Id = entity.Id, Title = entity.Title, ...
};
```
- `=>` → **expression-bodied member**: thân phương thức chỉ là một biểu thức
- `new()` → **target-typed new** (C# 9): trình biên dịch tự suy ra kiểu `BannerDto` từ kiểu trả về

**Kiểm tra `LinkUrl` nullable:**
```csharp
LinkUrl = string.IsNullOrWhiteSpace(request.LinkUrl) ? null : request.LinkUrl.Trim()
```
- `string.IsNullOrWhiteSpace` → trả về `true` nếu chuỗi null, rỗng hoặc chỉ có khoảng trắng
- `.Trim()` → loại bỏ khoảng trắng đầu và cuối chuỗi

---

## 6. Cart/ — Giỏ hàng

### ICartService.cs
```csharp
Task<ApiResult<CartResponse>> GetMyCartAsync(Guid userId);
Task<ApiResult<CartResponse>> AddToCartAsync(Guid userId, AddToCartRequest request);
Task<ApiResult<CartResponse>> UpdateQuantityAsync(Guid userId, int cartItemId, UpdateCartItemRequest request);
Task<ApiResult<CartResponse>> RemoveItemAsync(Guid userId, int cartItemId);
Task<ApiResult<bool>> ClearCartAsync(Guid userId);
```

### CartService.cs

**Toán tử `?.` và `??` phối hợp:**
```csharp
var image = cart.Variant.Images.FirstOrDefault(i => i.IsMain)
            ?? cart.Variant.Images.OrderBy(i => i.SortOrder).FirstOrDefault();
```
- `FirstOrDefault(predicate)` → tìm phần tử đầu tiên thỏa điều kiện, trả `null` nếu không có
- `??` → nếu không tìm được ảnh chính (`IsMain`) thì lấy ảnh đầu theo thứ tự `SortOrder`

**Kiểm tra tồn kho khi thêm giỏ:**
```csharp
var currentQty = existingCart?.Quantity ?? 0;
var newTotal   = currentQty + request.Quantity;

if (newTotal > variant.StockQuantity)
{
    var remaining = variant.StockQuantity - currentQty;
    return ApiResult<CartResponse>.Fail(remaining <= 0
        ? "Sản phẩm đã đạt giới hạn tồn kho trong giỏ hàng."
        : $"Chỉ có thể thêm tối đa {remaining} sản phẩm nữa.");
}
```
- `existingCart?.Quantity` → **null-conditional**: nếu `existingCart` là null thì biểu thức trả null
- Toán tử ba ngôi `? :` cho thông báo lỗi phù hợp từng tình huống tồn kho

**Upsert pattern (Insert hoặc Update):**
```csharp
if (existingCart != null)
    existingCart.Quantity = newTotal;          // Cập nhật số lượng
else
{
    var newCart = new Cart { UserId = userId, VariantId = request.VariantId, ... };
    await _cartRepo.AddAsync(newCart);         // Thêm mới
}
await _unitOfWork.SaveChangesAsync();
```
- Kiểm tra trước khi insert/update giúp tránh bản ghi trùng lặp trong giỏ hàng

**Xóa hàng loạt:**
```csharp
var carts = await _cartRepo.GetCartItemsWithTrackingAsync(userId);
if (carts.Any())
{
    _cartRepo.RemoveRange(carts);
    await _unitOfWork.SaveChangesAsync();
}
```
- `.Any()` → kiểm tra danh sách có ít nhất một phần tử không (nhanh hơn `.Count() > 0`)
- `RemoveRange` → xóa nhiều bản ghi trong một lần gọi thay vì vòng lặp

---

## 7. Categories/ — Danh mục

### ICategoryService.cs
```csharp
Task<ApiResult<List<CategoryTreeDto>>> GetTreeAsync();
Task<ApiResult<CategoryDto>> GetByIdAsync(int id);
Task<ApiResult<CategoryDto>> CreateAsync(CreateCategoryRequest request);
Task<ApiResult<CategoryDto>> UpdateAsync(int id, UpdateCategoryRequest request);
Task<ApiResult<bool>> DeleteAsync(int id);
```

### CategoryService.cs

**Phát hiện tham chiếu vòng (Circular Reference Detection):**
```csharp
private async Task<bool> DetectCircularReferenceAsync(int categoryId, int newParentId)
{
    var allCategories = await _categoryRepo.GetAllCategoryParentMapAsync();
    // allCategories: Dictionary<int, int?> — ánh xạ ID -> ParentID

    var currentId = newParentId;
    var visited   = new HashSet<int>();

    while (allCategories.ContainsKey(currentId))
    {
        if (currentId == categoryId) return true; // Tìm thấy vòng
        if (!visited.Add(currentId)) return true; // Vòng lặp vô hạn
        var parentId = allCategories[currentId];
        if (!parentId.HasValue) return false;     // Đã tới gốc
        currentId = parentId.Value;
    }
    return false;
}
```
- `HashSet<int>` → tập hợp không trùng lặp, `.Add()` trả về `false` nếu phần tử đã tồn tại
- `parentId.HasValue` → kiểm tra `int?` (nullable int) có chứa giá trị không

**Cập nhật Level đệ quy (Recursive Level Update):**
```csharp
private async Task UpdateSubtreeLevelsAsync(int parentId, int parentLevel)
{
    var children = await _categoryRepo.GetChildrenAsync(parentId);
    foreach (var child in children)
    {
        child.Level = parentLevel + 1;
        await UpdateSubtreeLevelsAsync(child.Id, child.Level); // Đệ quy
    }
}
```
- Khi di chuyển một danh mục sang cha mới, toàn bộ con cháu phải cập nhật lại `Level`

**Xây dựng cây đệ quy từ danh sách phẳng:**
```csharp
private List<CategoryTreeDto> BuildTree(List<Category> allCategories, int? parentId)
{
    return allCategories
        .Where(c => c.ParentId == parentId)
        .OrderBy(c => c.SortOrder).ThenBy(c => c.Name)
        .Select(c => new CategoryTreeDto
        {
            Id = c.Id, Name = c.Name, Slug = c.Slug,
            Children = BuildTree(allCategories, c.Id) // Đệ quy xây con
        }).ToList();
}
```
- `ThenBy` → sắp xếp phụ khi các phần tử có cùng `SortOrder`
- Hàm tự gọi lại chính nó với `parentId = c.Id` → xây cây đệ quy không giới hạn tầng

---

## 8. Tổng hợp các Pattern kiến trúc lặp lại trong tất cả file

| Pattern | Mô tả | Ví dụ |
|---------|--------|-------|
| **Interface + Implementation** | `IXxxService.cs` (hợp đồng) và `XxxService.cs` (triển khai) | `IBannerService` / `BannerService` |
| **Repository Pattern** | Service gọi qua `IXxxRepository`, không truy cập `DbContext` trực tiếp | `_bannerRepo.GetByIdAsync(id)` |
| **Unit of Work** | `IUnitOfWork.SaveChangesAsync()` gộp toàn bộ thay đổi thành 1 transaction | `await _unitOfWork.SaveChangesAsync()` |
| **Soft Delete** | `IsDeleted = true` + `DeletedDate = now` thay vì xóa vật lý | `banner.IsDeleted = true` |
| **DTO Mapping** | Hàm `MapToDto()` tách biệt Entity domain khỏi dữ liệu trả về API | `private static BannerDto MapToDto(Banner entity)` |
| **AsNoTracking()** | Dùng cho truy vấn read-only, EF Core không theo dõi thực thể, tăng hiệu suất | `_context.Orders.AsNoTracking()` |
| **Primary Constructor** | C# 12, thay thế boilerplate constructor + field assignment | `public class XxxService(IRepo repo) : IXxxService` |
| **Null Guard** | `?? throw new ArgumentNullException(...)` trong mọi constructor | `repo ?? throw new ArgumentNullException(nameof(repo))` |
| **try-catch + logger** | Mọi phương thức public bắt `Exception`, ghi log, trả về `ApiResult.Fail` | `catch (Exception ex) { _logger.LogError(ex, "..."); }` |
| **FluentValidation** | Validate request trước khi thực hiện logic nghiệp vụ | `await _validator.ValidateAsync(request)` |
| **LINQ Query Syntax** | Dùng `from ... join ... select` cho query phức tạp nhiều bảng | Truy vết giá vốn trong `AnalyticsService` |
| **Value Tuple** | Trả về nhiều giá trị không cần class wrapper | `private (bool valid, string error) ValidateRange(...)` |
| **Target-typed new** | `new()` thay vì `new BannerDto()` khi kiểu đã biết từ ngữ cảnh | `private static BannerDto MapToDto(...) => new() { ... }` |

---

## 9. Bảng tham chiếu nhanh các kiểu dữ liệu quan trọng

| Kiểu | Ý nghĩa |
|------|---------|
| `Task<T>` | Tác vụ bất đồng bộ cho ra kết quả kiểu `T` |
| `ApiResult<T>` | Wrapper phản hồi API gồm trạng thái, dữ liệu và thông báo |
| `PagedResult<T>` | Kết quả phân trang gồm danh sách `Items`, `TotalCount`, `PageNumber`, `PageSize` |
| `Guid` | Mã định danh duy nhất 128-bit, dùng làm khóa chính người dùng |
| `DateTime` | Mốc thời gian; `.UtcNow` luôn dùng UTC để tránh lỗi múi giờ |
| `decimal` | Kiểu số thực chính xác dùng cho tiền tệ; hậu tố `m` (VD: `0m`, `100.5m`) |
| `int?` / `Guid?` | Kiểu nullable — có thể nhận giá trị `null` thêm vào kiểu gốc |
| `HashSet<T>` | Tập hợp không trùng lặp, tìm kiếm O(1) |
| `Dictionary<K,V>` | Ánh xạ key-value, tra cứu nhanh O(1) |
| `List<T>` | Danh sách động, truy cập theo chỉ số |

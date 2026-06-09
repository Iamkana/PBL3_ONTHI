# 📚 TỔNG ÔN PROJECT PBL3 - HushStore

## 🎯 Project Là Gì?

**HushStore** là một hệ thống **quản lý bán hàng và kho hàng chuyên nghiệp cho cửa hàng bán linh kiện máy tính**.

Nói đơn giản:
- 👥 **Khách hàng** vào website, xem sản phẩm, mua hàng online
- 📦 **Nhân viên kho** quản lý tồn kho, xác nhận đơn hàng
- 👨‍💼 **Quản lý viên** xem báo cáo, quản lý sản phẩm
- 🖥️ **Hệ thống POS** bán trực tiếp tại quầy không cần qua giỏ hàng

---

## ⚙️ Stack Công Nghệ (Kỹ Thuật Dùng)

### Backend (Phía Server)
```
┌─────────────────────────────────────┐
│    ASP.NET Core 10 (C#)             │
│    - Tạo các API endpoint           │
│    - Xử lý logic nghiệp vụ         │
│    - Kiểm tra người dùng (JWT)      │
└─────────────────────────────────────┘
         ⬇️ Kết nối
┌─────────────────────────────────────┐
│    SQL Server 2025                  │
│    - Lưu trữ dữ liệu (DB)          │
│    - Sản phẩm, đơn hàng, khách     │
│    - Tồn kho, tài khoản người dùng │
└─────────────────────────────────────┘
```

**Tại sao ASP.NET Core?**
- ✅ Nhanh, hiệu năng cao
- ✅ An toàn (bảo mật tốt)
- ✅ Dễ phát triển web API (RESTful)
- ✅ Hỗ trợ tốt từ Microsoft

### Frontend (Phía Người Dùng)
```
┌─────────────────────────────────────┐
│    Blazor WebAssembly               │
│    - Chạy trong trình duyệt (C#)   │
│    - Tạo giao diện đẹp              │
│    - Tương tác mượt mà              │
├─────────────────────────────────────┤
│    MudBlazor (Thư viện UI)          │
│    - Các component sẵn có (button,  │
│      dialog, table, form)           │
│    - Design đẹp, chuyên nghiệp     │
└─────────────────────────────────────┘
```

**Tại sao Blazor + MudBlazor?**
- ✅ Viết C# thay vì JavaScript (dễ hơn)
- ✅ Tái sử dụng code giữa Backend và Frontend
- ✅ Chạy trực tiếp trình duyệt (nhanh)
- ✅ MudBlazor có sẵn giao diện đẹp, không phải thiết kế lại

---

## 🏗️ Kiến Trúc Dự Án (Cách Tổ Chức Code)

Project chia thành **6 thư mục chính**, mỗi cái có vai trò riêng:

```
┌─────────────────────────────────────────────────────────────┐
│                                                             │
│  Shared                                                    │
│  - Dữ liệu dùng chung (DTO, Enum, Response)               │
│  - Validator kiểm tra form                                 │
│  ↑              ↑                                           │
│  │              │ phụ thuộc                               │
│  │              │                                          │
│  ├──────────────┤                                          │
│  │              │                                          │
│  Client         API (ASP.NET Core)                        │
│ (Blazor)        - Các endpoint HTTP                       │
│ - Giao diện     - Xử lý request, call Service             │
│ - Tương tác     - Trả response                            │
│ người dùng      │                                          │
│                 ↓                                          │
│             Service (Business Logic)                       │
│             - Xử lý logic nghiệp vụ                       │
│             - Gọi Repository lấy/lưu data                 │
│             - AutoMapper (chuyển đổi data)                │
│                 │                                          │
│                 ↓                                          │
│           Infrastructure (Data Access)                     │
│           - EF Core (kết nối DB)                          │
│           - Repository (lấy/lưu data)                     │
│           - Migration (cập nhật DB schema)                │
│                 │                                          │
│                 ↓                                          │
│           Core (Business Entities)                         │
│           - Các class đại diện cho DB (Product, Order)    │
│           - Interface Repository                          │
│           - Không phụ thuộc project khác                  │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

### Chi tiết từng project:

| Project | Dùng để làm gì |
|---------|---|
| **Core** | Định nghĩa các **entity** (bảng DB): Product, Order, Customer, ... Không có logic gì cả, chỉ định hình data |
| **Shared** | Định nghĩa **DTO** (dữ liệu để gửi/nhận API), **Enum** (tình trạng đơn hàng), **Validator** (kiểm tra dữ liệu) |
| **Infrastructure** | Kết nối **SQL Server**, lưu code **EF Core**, **migration** (sửa schema DB) |
| **Service** | Chứa **logic kinh doanh**: tính toán, kiểm tra điều kiện, gọi Repository để lấy data |
| **API** | Tạo các **endpoint HTTP** (GET, POST, PUT, DELETE), nhận request từ Client → gọi Service → trả response |
| **Client** | **Blazor WebAssembly** - giao diện web, gọi API để lấy dữ liệu |

---

## 🔐 Xác Thực & Phân Quyền (Authentication & Authorization)

### Cách hoạt động:

```
1. Khách hàng đăng nhập
   ↓
2. Server kiểm tra username/password
   ↓
3. Nếu đúng → tạo JWT token (khóa bảo mật)
   ↓
4. Client lưu token, mỗi lần gọi API gửi kèm token
   ↓
5. Server xác minh token, nếu hợp lệ → thực hiện request
```

### Các Role (vai trò):
- 👤 **Customer** (Khách hàng): Mua hàng, xem đơn của mình
- 👨‍⚙️ **Employee** (Nhân viên): Xác nhận đơn, quản lý kho
- 👨‍💼 **Admin** (Quản trị viên): Quản lý mọi thứ

**Hết hạn token:**
- Access token: **15 phút** (ngắn vì an toàn)
- Refresh token: **7 ngày** (lâu hơn, dùng để lấy token mới)

---

## 📦 Các Khái Niệm Chủ Chốt

### 1. **Repository Pattern** (Lấy/Lưu Data)
```csharp
// Thay vì code nào cũng truy cập DB trực tiếp,
// ta dùng Interface để chuẩn hóa

IProductRepository productRepo = ...;
var product = await productRepo.GetByIdAsync(123);
// Cách này dễ test, dễ thay đổi sau
```

### 2. **DTO** (Data Transfer Object)
```csharp
// Entity (DB) - có mọi thứ
public class Product {
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public string InternalNote { get; set; }  // ← Bí mật, không cho client
}

// DTO - chỉ gửi những thứ cần thiết
public class ProductDto {
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    // InternalNote không có → bảo vệ dữ liệu
}
```

### 3. **EF Core** (Entity Framework)
Thư viện giúp:
- Viết query DB dễ dàng (không phải viết SQL thô)
- Tự động ánh xạ DB ↔ C# object
- Tự động tạo, cập nhật schema DB (migration)

```csharp
// Query dễ dàng
var products = await dbContext.Products
    .Where(p => p.Price > 10000)
    .ToListAsync();  // Không cần viết SQL
```

### 4. **AutoMapper** (Chuyển đổi Data)
```csharp
// Thay vì manual:
var productDto = new ProductDto {
    Id = product.Id,
    Name = product.Name,
    Price = product.Price
};

// Dùng AutoMapper:
var productDto = mapper.Map<ProductDto>(product);  // Tự động!
```

### 5. **MudBlazor** (UI Component)
```razor
<!-- Thay vì viết CSS từ đầu -->
<MudButton Variant="Variant.Filled" Color="Color.Primary">
    Nhấn tôi
</MudButton>

<MudTable Items="products">
    <HeaderContent>
        <MudTh>Tên sản phẩm</MudTh>
        <MudTh>Giá</MudTh>
    </HeaderContent>
    <RowTemplate>
        <MudTd>@context.Name</MudTd>
        <MudTd>@context.Price</MudTd>
    </RowTemplate>
</MudTable>
```

---

## 🚀 Cách Chạy Project

### 1. **Khởi động Database**
```bash
cd Infrastructure/db
docker-compose up -d
# Khởi động SQL Server trong container
```

### 2. **Build Solution**
```bash
dotnet build PBL3.sln
```

### 3. **Tạo/Cập nhật Database**
```bash
dotnet ef database update --project src/Infrastructure --startup-project src/API
```

### 4. **Chạy API Server** (cổng 7010)
```bash
dotnet run --project src/API/API.csproj
# Truy cập: https://localhost:7010
```

### 5. **Chạy Blazor Client** (cổng 7107)
```bash
dotnet run --project src/Client/Client.csproj
# Truy cập: https://localhost:7107
```

---

## 🎯 Quy Tắc Viết Code Bắt Buộc

### 1. **Luôn .AsNoTracking() cho query read-only**
```csharp
// ❌ Sai
var products = await dbContext.Products.ToListAsync();

// ✅ Đúng
var products = await dbContext.Products.AsNoTracking().ToListAsync();
// Lý do: EF không cần track, nhanh 2-5 lần
```

### 2. **Luôn dùng Async (.ToListAsync(), FirstOrDefaultAsync())**
```csharp
// ❌ Sai - gây deadlock
var product = dbContext.Products.FirstOrDefault();

// ✅ Đúng
var product = await dbContext.Products.FirstOrDefaultAsync();
```

### 3. **Luôn phải có Pagination (Skip + Take)**
```csharp
// ❌ Sai - trả về 1 triệu sản phẩm?!
var products = await dbContext.Products.ToListAsync();

// ✅ Đúng
var products = await dbContext.Products
    .Skip((pageNumber - 1) * pageSize)
    .Take(pageSize)
    .ToListAsync();
```

### 4. **Dùng Include() để tránh N+1 Query**
```csharp
// ❌ Sai - 1001 query: 1 để lấy orders + 1000 query để lấy từng customer
var orders = await dbContext.Orders.ToListAsync();
foreach (var order in orders)
    Console.WriteLine(order.Customer.Name);  // ← 1000 query thêm!

// ✅ Đúng - chỉ 1 query
var orders = await dbContext.Orders
    .Include(o => o.Customer)
    .ToListAsync();
foreach (var order in orders)
    Console.WriteLine(order.Customer.Name);  // ← Không query thêm
```

### 5. **Thông báo lỗi phải bằng TIẾNG VIỆT**
```csharp
// ❌ Sai
return BadRequest("Product not found");

// ✅ Đúng
return BadRequest("Không tìm thấy sản phẩm yêu cầu.");
```

### 6. **Kiểm tra IDOR (người dùng chỉ được xem của mình)**
```csharp
// ❌ Sai - user A có thể xem order của user B!
var order = await dbContext.Orders.FirstOrDefaultAsync(o => o.Id == orderId);

// ✅ Đúng - chỉ trả về order của user hiện tại
var order = await dbContext.Orders
    .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == currentUserId);
if (order == null) return Forbid();
```

---

## 📊 Các Model Chính (Entities)

### Product (Sản phẩm)
```
Category (Danh mục)
    ↓
Product (Sản phẩm cụ thể, VD: "CPU Intel i7")
    ↓
ProductVariant (Biến thể - SKU, VD: "i7-13700K")
    ↓
ProductSerial (Cá nhân - số serial, VD: "SN123456")
```

**Stock (Tồn kho):**
- Tồn kho = Đếm số `ProductSerial` có status `Available`
- **Không được** tay động cập nhật số tồn kho
- Nó tự động tính bằng code!

### Order (Đơn hàng)
```
Order (Đơn hàng)
    ├── OrderDetail (Chi tiết sản phẩm)
    └── OrderSerial (Serial cụ thể - gán khi xác nhận)
```

**Trạng thái Serial:**
- `Available` → `Reserved` (có đơn) → `Sold` (bán rồi) hoặc `Defective` (hỏng)

---

## ⚠️ Những Lỗi Phổ Biến Cần Tránh

| Lỗi | Hậu quả | Cách tránh |
|-----|---------|-----------|
| Quên `.AsNoTracking()` | Chậm, EF track tất cả entity | Thêm `.AsNoTracking()` cho read-only |
| Dùng `.Result` hoặc `.Wait()` | Deadlock, ứng dụng chết | Dùng `await` |
| Trả về Entity thay vì DTO | Lộ dữ liệu bí mật | Map sang DTO bằng AutoMapper |
| N+1 Query | Database quá chậm | Dùng `.Include()` |
| Quên Pagination | Request lâu, RAM tràn | Thêm `.Skip().Take()` |
| Không kiểm tra IDOR | Bảo mật - user B xem được của A | Check `o.UserId == currentUserId` |
| Không `await` async | Không chạy hết code | Luôn `await` async method |

---

## 📚 File Quan Trọng Cần Biết

```
PBL3/
├── src/
│   ├── Core/                    ← Các entity (Product, Order, ...)
│   ├── Shared/                  ← DTO, Enum, Validator
│   ├── Infrastructure/          ← DbContext, Repository, Migration
│   ├── Service/                 ← Business logic
│   ├── API/
│   │   ├── Controllers/         ← HTTP endpoint
│   │   └── Program.cs           ← Cấu hình DI Container
│   └── Client/
│       ├── Pages/               ← Blazor pages (.razor)
│       ├── Components/          ← Reusable component
│       └── Services/            ← HttpClient service
├── CLAUDE.md                    ← Hướng dẫn phát triển
└── HelpMe.md                    ← File này
```

---

## 🤔 FAQ (Câu Hỏi Thường Gặp)

**Q: Tại sao chia thành 6 project?**
A: Để tách riêng trách nhiệm (Separation of Concerns). Dễ test, dễ bảo trì, dễ thay đổi sau. Nếu viết gộp 1 chỗ sẽ rất phức tạp.

**Q: Sự khác biệt giữa Entity và DTO?**
A: Entity là bảng DB (có tất cả field), DTO là dữ liệu để gửi client (chỉ những gì cần). Mục đích bảo vệ dữ liệu nhạy cảm.

**Q: Tại sao phải .AsNoTracking()?**
A: EF Core có Change Tracker để theo dõi thay đổi entity (dùng cho Update). Nhưng khi chỉ đọc, tracker thừa → gọi .AsNoTracking() tắt nó đi, tăng tốc độ.

**Q: Token hết hạn sau 15 phút, phải đăng nhập lại?**
A: Có **Refresh Token** (7 ngày). Khi Access Token hết, client dùng Refresh Token để lấy token mới mà không cần đăng nhập lại.

**Q: Làm sao kiểm tra API endpoint?**
A: Dùng Swagger UI (trong API): https://localhost:7010/swagger
Hoặc dùng Postman, Thunder Client, REST Client extension.

---

## 💡 Lời Khuyên Cuối Cùng

1. **Bắt đầu từ Core** - hiểu entity trước
2. **Đọc CLAUDE.md** - nó chi tiết hơn file này
3. **Chạy project lên** - làm quen với UI
4. **Thay vì sợ, hãy thử** - sửa lỗi là cách tốt nhất học
5. **Google + StackOverflow** - là bạn tốt của lập trình viên
6. **Đọc error message kỹ** - nó thường chỉ ra vấn đề

---

## 📖 Tài Liệu Bổ Sung

- `/AI_context/` folder có file chi tiết về kiến trúc, bảo mật, database optimization
- `CLAUDE.md` - hướng dẫn đầy đủ cho lập trình viên

**Chúc bạn học tập hiệu quả! 🎓**

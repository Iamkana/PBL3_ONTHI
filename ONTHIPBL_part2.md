# Ôn thi PBL3 - Phần 2: Lý thuyết & Tư duy phản biện

## PHẦN 1: 5 CÂU HỎI LÝ THUYẾT NỀN TẢNG VỀ CLEAN ARCHITECTURE

- **Câu 1:** Clean Architecture là gì? Nguyên tắc cốt lõi nhất của nó về Hướng phụ thuộc (Dependency Direction) là gì? Tại sao cấu trúc này lại giúp hệ thống dễ bảo trì và dễ viết Unit Test hơn mô hình 3 lớp (3-Tier) truyền thống?
  - **Trả lời:**
    - **Clean Architecture** là kiến trúc phần mềm tập trung vào việc tách biệt các mối quan tâm (Separation of Concerns) nhằm xây dựng hệ thống độc lập với framework, database, giao diện UI và các thư viện bên ngoài.
    - **Nguyên tắc cốt lõi về Hướng phụ thuộc (Dependency Direction):** Chiều phụ thuộc luôn đi từ ngoài vào trong. Các tầng ở vòng trong (Core, Application) là trung tâm và hoàn toàn không được phép tham chiếu, biết hoặc phụ thuộc vào các tầng vòng ngoài (Infrastructure, Presentation).
    - **Lý do dễ bảo trì và viết Unit Test hơn mô hình 3-Tier:**
      - *3-Tier truyền thống:* Tầng Business Logic (BLL) phụ thuộc trực tiếp vào Data Access (DAL). Nếu DB thay đổi, BLL bị ảnh hưởng trực tiếp. Việc viết Unit Test rất khó khăn do bị dính chặt với các truy vấn DB thật.
      - *Clean Architecture:* Tầng chứa logic nghiệp vụ (Application & Core) nằm ở vòng trong, độc lập với cơ sở dữ liệu và giao diện. Tầng dữ liệu (Infrastructure) phụ thuộc ngược vào các Interface định nghĩa ở Core (áp dụng Dependency Inversion). Vì thế, khi đổi DB hay thư viện UI, code nghiệp vụ hoàn toàn giữ nguyên. Khi viết Unit Test cho tầng nghiệp vụ, ta có thể dễ dàng sử dụng Mock/Stub cho các Interface mà không cần chạy Database thật.

- **Câu 2:** Hãy phân biệt vai trò nhiệm vụ của 4 lớp (thư mục con) chính trong mã nguồn dự án của chúng ta:
  - Core (Domain)
  - Application
  - Infrastructure
  - Presentation (API và Client)
  *Lớp nào trong 4 lớp trên là quan trọng nhất, chứa đựng các thực thể nghiệp vụ (Business Entities) và hoàn toàn không phụ thuộc vào bất kỳ thư viện hay công nghệ bên ngoài nào?*
  - **Trả lời:**
    - **Core (Domain):** Chứa các thực thể nghiệp vụ (Domain Entities), các quy tắc nghiệp vụ cốt lõi (Domain Rules), enum và các interfaces của Repository.
    - **Application:** Chứa logic nghiệp vụ ứng dụng (Use Cases / Services như `OrderService`), điều phối luồng dữ liệu, chứa các interface dịch vụ, custom exceptions và DTOs.
    - **Infrastructure:** Triển khai các Interface được định nghĩa ở Core/Application. Tầng này kết nối database (EF Core `HushStoreDbContext`, Repositories thực tế), tích hợp bên thứ ba (JWT Token Generator, Cổng thanh toán, v.v.).
    - **Presentation (API và Client):** API (`src/API`) tiếp nhận HTTP Request, gọi tầng Application thông qua DI để xử lý nghiệp vụ và trả về kết quả JSON. Client (`src/Client` chạy Blazor WASM) hiển thị UI và gửi Request đến API.
    - *Lớp quan trọng nhất:* Tầng **Core (Domain)** là quan trọng nhất, chứa đựng các thực thể nghiệp vụ và tuyệt đối độc lập với mọi thư viện hay công nghệ bên ngoài.

- **Câu 3:** Trong Clean Architecture, tại sao các Interfaces (ví dụ: `IUnitOfWork`, `IProductRepository`) lại được định nghĩa ở lớp Core hoặc Application, nhưng phần cài đặt code chi tiết (Implementation) lại nằm ở lớp Infrastructure? Mô hình này đang áp dụng nguyên lý nào trong bộ nguyên lý SOLID?
  - **Trả lời:**
    - **Lý do đặt Interface ở Core/Application và Implementation ở Infrastructure:** Để tuân thủ Quy tắc phụ thuộc (Dependency Rule). Vì Core/Application ở vòng trong không được phép phụ thuộc vào Infrastructure ở vòng ngoài, nên Core/Application định nghĩa ra các giao kèo (Interface) mô tả những gì nó cần. Tầng Infrastructure nằm ở vòng ngoài sẽ tham chiếu vào Core/Application và thực thi chi tiết (Implementation) giao kèo đó bằng các công nghệ cụ thể (EF Core, SQL Server, v.v.).
    - **Nguyên lý SOLID áp dụng:** Áp dụng nguyên lý **Dependency Inversion Principle (DIP)** - Nguyên lý đảo ngược phụ thuộc (chữ D trong SOLID): Các module cấp cao không nên phụ thuộc vào module cấp thấp, cả hai nên phụ thuộc vào sự trừu tượng (Interface).
    - **Lợi ích:** Giúp hệ thống độc lập với công nghệ DB. Ta có thể thay đổi cơ sở dữ liệu (từ SQL Server sang PostgreSQL hay MongoDB) bằng cách viết một Class triển khai mới ở tầng Infrastructure và đăng ký lại DI mà không chạm vào một dòng code nào của Core/Application.

- **Câu 4:** DTO (Data Transfer Object) là gì? Tại sao trong dự án chúng ta lại tách riêng một project tên là `src/Shared` để chứa các DTO và các bộ Validator (FluentValidation) mà không dùng trực tiếp các Domain Entities (như `Product`, `Order`) để truyền nhận dữ liệu giữa Blazor WASM và Web API?
  - **Trả lời:**
    - **DTO (Data Transfer Object):** Là đối tượng chỉ dùng để đóng gói và chuyển giao dữ liệu giữa các tầng hoặc giữa Client và Server, không chứa logic nghiệp vụ.
    - **Tại sao tách riêng project `src/Shared`:** Để cả project `src/Client` (Blazor WASM) và `src/API` (Web API) đều có thể tham chiếu và sử dụng chung các cấu trúc dữ liệu truyền nhận cùng bộ quy tắc Validate.
    - **Tại sao không dùng trực tiếp Domain Entities:**
      - *Bảo mật:* Entity chứa các trường nhạy cảm (`PasswordHash`, `LockReason`, `IsDeleted`) không được phép gửi ra ngoài Client.
      - *Hiệu năng:* Entities có các mối quan hệ phức tạp (Navigation Properties) dễ gây lỗi vòng lặp (circular reference) khi Serialize sang JSON.
      - *Kiểm soát dữ liệu:* Client chỉ cần các dữ liệu tinh gọn để hiển thị (ví dụ: `OrderDetailDto` thay vì cả object `Order` cồng kềnh chứa đầy đủ quan hệ dữ liệu). DTO giúp tối ưu băng thông mạng.
      - *Đồng nhất validate:* DTO kết hợp FluentValidation trong `src/Shared` cho phép thực hiện validate dữ liệu đầu vào ngay tại Client để báo lỗi nhanh lên UI, đồng thời tái sử dụng chính xác bộ validate đó ở Backend API nhằm đảm bảo an toàn tuyệt đối.

- **Câu 5:** Khi khách hàng bấm nút "Thanh toán" (Checkout) trên giao diện Web (Client), một HTTP Request sẽ được gửi đến hệ thống Backend (API). Hãy mô tả ngắn gọn luồng đi của dữ liệu sẽ xuyên qua các lớp nào của Clean Architecture theo đúng thứ tự để hoàn thành tác vụ.
  - **Trả lời:**
    1. **Presentation (Client - Blazor WASM):** Nhận sự kiện bấm nút thanh toán từ người dùng, đóng gói thông tin đơn hàng vào `CheckoutRequest` DTO, gọi `HttpClient` gửi HTTP POST Request tới API endpoint `api/orders/checkout`.
    2. **Presentation (API - Web API Controller):** Tiếp nhận HTTP Request. Đi qua các Middleware như CORS, Rate Limiting, Authentication (giải mã token), và UserStatusMiddleware (kiểm tra tài khoản). Sau khi hợp lệ, Controller gọi hàm `CheckoutAsync` của `IOrderService` ở tầng Application.
    3. **Application (Service - `OrderService`):** Nhận DTO, thực thi nghiệp vụ: kiểm tra tồn kho ảo, áp dụng các voucher (gọi `ApplyVouchersAsync`), mở Transaction thông qua `IUnitOfWork`, và tạo entity `Order` cùng `OrderDetails`.
    4. **Infrastructure (Data/Repositories - `OrderRepository` & `UnitOfWork`):** Lưu dữ liệu entity vào `HushStoreDbContext`, tương tác trực tiếp với Database SQL Server thông qua Entity Framework Core. Gọi `SaveChangesAsync()` và Commit Transaction.
    5. **Trả về kết quả:** Phản hồi từ Database truyền ngược từ Infrastructure -> Application -> API Controller (đóng gói kết quả vào `ApiResult<CheckoutResponse>`) -> Client (Blazor WASM hiển thị màn hình đặt hàng thành công).

---

## CHẶNG 1: 5 CÂU HỎI LÝ THUYẾT NỀN TẢNG VỀ CLEAN ARCHITECTURE
*(Mục tiêu: Giúp người học hiểu tại sao lại đẻ ra cấu trúc này và quy tắc bất di bất dịch của nó)*

### Câu 1: Quy tắc phụ thuộc (Dependency Rule) là gì?
- **Nội dung:** Trong kiến trúc Clean Architecture, các tầng được xếp theo các vòng tròn đồng tâm (hoặc các Layer xếp chồng). Quy tắc cốt lõi về hướng mũi tên phụ thuộc (Dependency Direction) giữa các tầng là gì? Vòng tròn bên trong có được phép biết vòng tròn bên ngoài đang viết gì không?
- **Ý nghĩa:** Nếu vi phạm quy tắc này (ví dụ: Tầng Core đi gọi code của tầng API), hệ thống sẽ gặp hậu quả gì khi phát triển lâu dài?
- **Trả lời:**
  - **Quy tắc phụ thuộc:** Hướng phụ thuộc của code chỉ được phép đi từ ngoài vào trong. Vòng tròn bên trong tuyệt đối không được phép biết, tham chiếu hay chứa bất kỳ thông tin nào về vòng tròn bên ngoài.
  - **Hậu quả khi vi phạm:** Nếu tầng Core gọi code của tầng API hay Infrastructure:
    - Code bị **khóa cứng công nghệ (Tight Coupling)**, mất đi khả năng độc lập. Domain logic sẽ bị ô nhiễm bởi các thư viện cụ thể như EF Core hay ASP.NET Core.
    - Không thể viết Unit Test cho tầng Core một cách cô lập.
    - Mất khả năng thay đổi công nghệ bên ngoài (UI/DB) mà không làm sập logic nghiệp vụ bên trong.

### Câu 2: Bản đồ 4 tầng trong dự án HushStore
- **Nội dung:** Dự án của chúng ta được chia thành các thư mục: `src/Core`, `src/Application`, `src/Infrastructure`, và `src/API`. Em hãy giải thích vai trò của từng tầng này.
- **Trọng tâm:** Tầng nào được coi là "Trái tim" của hệ thống, chứa các thực thể nghiệp vụ (Domain Entities) và tuyệt đối độc lập, không phụ thuộc vào bất kỳ thư viện bên ngoài nào (như Entity Framework, JWT, AWS)?
- **Trả lời:**
  - **src/Core:** Định nghĩa các Business Entities (`Product`, `Order`), Enums, Domain Exceptions, và các Repository Interfaces. Đây chính là **"Trái tim"** của hệ thống, tuyệt đối độc lập và không phụ thuộc vào bất kỳ thư viện ngoài nào.
  - **src/Application:** Điều phối luồng nghiệp vụ thông qua các Application Services (`OrderService`, `CustomerService`), DTOs, và mapping. Tầng này chỉ phụ thuộc vào tầng Core.
  - **src/Infrastructure:** Chứa cấu hình Database, kết nối EF Core (`HushStoreDbContext`), cài đặt các Repositories, và tích hợp bên thứ ba (gửi mail, bảo mật JWT).
  - **src/API:** Tầng Presentation Backend, định nghĩa các Controller, Middleware, cấu hình Pipeline, và cung cấp các API Endpoints cho Client.
  - *Trọng tâm:* Tầng **src/Core** là "Trái tim" chứa Domain Entities và hoàn toàn độc lập với mọi thư viện hay công nghệ bên ngoài.

### Câu 3: Tại sao lại dùng Interface? (Nguyên lý Dependency Inversion - chữ D trong SOLID)
- **Nội dung:** Tại sao Clean Architecture lại bắt buộc chúng ta phải khai báo Interface (ví dụ: `IProductRepository`) ở tầng bên trong (Core/Application), nhưng phần viết code chi tiết (Implementation) kết nối Database thực tế lại vứt ra tầng ngoài cùng (Infrastructure)? Cơ chế này giúp ích gì cho việc thay đổi công nghệ sau này?
- **Trả lời:**
  - **Lý do dùng Interface:** Để thực thi nguyên lý **Dependency Inversion Principle (DIP)**. Tầng nghiệp vụ (Core/Application) chỉ tương tác với sự trừu tượng (Interface) chứ không phụ thuộc vào class triển khai cụ thể của tầng ngoài (Infrastructure), giữ cho tầng nghiệp vụ hoàn toàn độc lập với database/thư viện ngoài.
  - **Cơ chế giúp thay đổi công nghệ:** Nếu sau này dự án muốn thay SQL Server bằng PostgreSQL hay MongoDB, ta chỉ việc viết một class cụ thể mới (ví dụ: `ProductMongoRepository`) triển khai `IProductRepository` ở tầng Infrastructure, sau đó thay đổi cấu hình đăng ký DI trong `Program.cs`. Toàn bộ code xử lý nghiệp vụ ở tầng Application vẫn được giữ nguyên vẹn 100%.

### Câu 4: Vai trò của tầng Shared (src/Shared)
- **Nội dung:** Dự án có một Project độc lập là `src/Shared` chứa các DTO (Data Transfer Object) và các bộ kiểm tra dữ liệu đầu vào (Validators). Tại sao chúng ta không dùng luôn các Entity trong tầng Core (như thực thể `Order`, `Product`) để truyền dữ liệu qua lại giữa Client và API, mà phải đẻ ra DTO?
- **Trả lời:**
  - **Bảo mật thông tin:** Tránh việc để lộ cấu trúc dữ liệu nhạy cảm của Database hoặc các trường nghiệp vụ nội bộ (`PasswordHash`, `IsDeleted`) ra ngoài Internet.
  - **Tối ưu hiệu năng:** Giảm thiểu dung lượng tải (payload) truyền qua mạng bằng cách lọc bỏ các Navigation Properties cồng kềnh của Entity.
  - **Bẻ gãy lỗi Serialization:** Tránh lỗi tham chiếu vòng tròn (circular reference loop) khi chuyển đổi Entity sang định dạng JSON để gửi qua HTTP.
  - **Validate đồng nhất:** Tái sử dụng các bộ kiểm tra dữ liệu (`FluentValidation`) của `src/Shared` cho cả Client (Blazor WASM validate nhanh trên UI) và Server API (Web API validate lại để đảm bảo an toàn).

### Câu 5: Luồng đi của dữ liệu (Data Flow)
- **Nội dung:** Khi người dùng bấm nút đặt hàng trên giao diện, dữ liệu sẽ đi qua tầng API, chuyển vào Application, tương tác với Core và lưu xuống Infrastructure. Hãy giải thích tại sao luồng dữ liệu (Data Flow) có thể đi từ ngoài vào trong rồi lại ra ngoài, nhưng hướng phụ thuộc (Dependency Direction) của code thì vẫn chỉ có một chiều duy nhất?
- **Trả lời:**
  - **Phân biệt Luồng chạy dữ liệu (Runtime Data Flow) và Hướng phụ thuộc mã nguồn (Source Code Dependency Direction):**
    - *Runtime Data Flow:* Khi chạy ứng dụng, dữ liệu đi từ API -> Application -> Core -> Infrastructure. Đây là luồng động khi thực thi.
    - *Source Code Dependency:* Các tham chiếu trong code luôn hướng từ ngoài vào trong. Tầng API tham chiếu Application. Tầng Application tham chiếu Core. Tầng Infrastructure tham chiếu Core và Application.
  - **Tại sao chiều phụ thuộc code vẫn giữ 1 chiều:** Nhờ việc áp dụng **Dependency Inversion**. Tầng Application khi cần lấy dữ liệu không gọi trực tiếp class ở Infrastructure, mà gọi Interface của Core. Tầng Infrastructure kế thừa Interface đó. Do đó, mã nguồn của Infrastructure phụ thuộc ngược vào Core. Khi chạy, .NET DI Container tự động phân giải Interface thành class thật, giúp dữ liệu chạy ra tầng Infrastructure mà không cần Core/Application phải tham chiếu đến nó.

---

## CHẶNG 2: 10 CÂU HỎI TƯ DUY & PHẢN BIỆN QUA CODE THỰC TẾ CỦA DỰ ÁN
*(Mục tiêu: Đưa mã nguồn ra làm bằng chứng, bắt bạn của bạn phải truy vết file để hiểu cách bạn tổ chức Clean Architecture)*

### Câu 6: Đảo ngược phụ thuộc trong Repository Pattern
- **Vết code:** 
  - Interface: `src/Core/Interfaces/Repositories/Products/IProductRepository.cs`
  - Code triển khai: `src/Infrastructure/Repositories/Products/ProductRepository.cs`
- **Câu hỏi hội đồng:** "Tại sao em lại tách đôi một chức năng ra làm 2 file ở 2 tầng khác nhau như thế này? Tầng API khi muốn gọi hàm lấy sản phẩm thì nó sẽ gọi thông qua Interface ở tầng Core hay gọi trực tiếp Class ở tầng Infrastructure? Tại sao?"
- **Trả lời:**
  - Tách đôi để **cô lập công nghệ truy xuất cơ sở dữ liệu** khỏi logic nghiệp vụ của hệ thống (Separation of Concerns).
  - Tầng API (thông qua tầng Application) sẽ gọi thông qua **Interface `IProductRepository` ở tầng Core**, chứ tuyệt đối không gọi trực tiếp Class `ProductRepository` ở tầng Infrastructure.
  - *Lý do:* Để tuân thủ Dependency Rule của Clean Architecture. Nếu gọi trực tiếp Class ở Infrastructure, tầng API/Application sẽ phụ thuộc cứng vào Entity Framework Core và Database SQL Server, làm hỏng cấu trúc lỏng (Loose Coupling) và làm mất đi khả năng viết Unit Test độc lập cho các Service.

### Câu 7: Đăng ký Dependency Injection (DI) xuyên tầng
- **Vết code:** Trong file `src/API/Program.cs`, có hai dòng lệnh:
  ```csharp
  builder.Services.AddRepositories();
  builder.Services.AddApplicationServices(builder.Configuration);
  ```
- **Câu hỏi hội đồng:** "Các hàm mở rộng `AddRepositories()` và `AddApplicationServices()` này thực chất được viết ở đâu (Đường dẫn: `src/API/Extensions/`)? Tại sao tầng API (vòng ngoài cùng) lại là nơi đứng ra nạp tất cả các Dependency của tầng Application và Infrastructure vào bộ nhớ của ứng dụng?"
- **Trả lời:**
  - Hàm mở rộng `AddRepositories()` được định nghĩa tại file `src/API/Extensions/RepositoryExtensions.cs`, còn `AddApplicationServices()` được định nghĩa tại `src/API/Extensions/ApplicationServiceExtensions.cs` (hoặc file tương đương của tầng API).
  - Tầng API đứng ra nạp tất cả các Dependency vì trong mô hình lập trình, API đóng vai trò là **Composition Root** (Điểm lắp ráp ứng dụng). Chỉ có tầng ngoài cùng này mới có quyền và nhiệm vụ biết tất cả các tầng khác để khởi tạo, đăng ký vòng đời dịch vụ (Service Lifetimes) và liên kết chúng lại thành một chương trình chạy hoàn chỉnh trước khi khởi động Runtime.

### Câu 8: Cách cô lập Database Access (EF Core)
- **Vết code:** Tìm file `src/Infrastructure/Data/HushStoreDbContext.cs` và đoạn cấu hình DbContext trong `src/API/Program.cs`:
  ```csharp
  builder.Services.AddDbContext<HushStoreDbContext>(options => { ... });
  ```
- **Câu hỏi hội đồng:** "Thư viện `Microsoft.EntityFrameworkCore` được cài đặt ở tầng nào? Nếu tôi muốn đổi SQL Server sang PostgreSQL, tôi sẽ chỉnh sửa code ở tầng nào và tầng nào sẽ hoàn toàn KHÔNG bị ảnh hưởng một dòng code nào?"
- **Trả lời:**
  - Thư viện `Microsoft.EntityFrameworkCore` được cài đặt và cấu hình trực tiếp ở tầng **Infrastructure** (và được tham chiếu ở API để phục vụ khởi tạo).
  - Khi đổi sang PostgreSQL, ta chỉ cần chỉnh sửa code ở tầng **Infrastructure** (thay đổi Provider trong `HushStoreDbContext.cs` hoặc file cấu hình ở API) và cài đặt thư viện PostgreSQL.
  - Tầng **Core** và **Application** sẽ hoàn toàn **KHÔNG bị ảnh hưởng** bất kỳ một dòng code nào vì chúng chỉ giao tiếp qua các Interface trừu tượng không hề chứa khái niệm kỹ thuật của EF Core.

### Câu 9: Xử lý ngoại lệ (Exception) tập trung dưới góc nhìn Kiến trúc
- **Vết code:** Các Custom Exception nằm ở `src/Application/Common/Exceptions/NotFoundException.cs`. Đoạn code bắt Exception bằng Middleware nằm ở tầng ngoài `src/API/Program.cs` (`app.UseExceptionHandler(...)`).
- **Câu hỏi hội đồng:** "Khi một Service ở tầng Application không tìm thấy sản phẩm, nó sẽ throw new `NotFoundException()`. Tại sao việc ném lỗi diễn ra ở tầng nghiệp vụ (Application) nhưng việc dịch lỗi thành HTTP Status Code (404 Not Found) để trả về cho Client lại được thực hiện ở tầng API? Như vậy có vi phạm ranh giới kiến trúc không?"
- **Trả lời:**
  - Việc này hoàn toàn **không vi phạm ranh giới kiến trúc**, mà trái lại là tuân thủ chặt chẽ nguyên lý tách biệt mối quan tâm.
  - *Giải thích:* Tầng nghiệp vụ (Application) chỉ có nhiệm vụ phát hiện lỗi nghiệp vụ (ví dụ: sản phẩm không tồn tại) và ném ra các Exception thuần túy C# (`NotFoundException`). Nó không được phép biết về khái niệm giao thức HTTP (như 404, 500, 200). Tầng API là tầng giao tiếp ngoài cùng, có nhiệm vụ bắt các Exception C# đó để chuyển ngữ (dịch) chúng thành HTTP Status Code thích hợp gửi về cho Client. Điều này giúp tầng Application độc lập với giao thức truyền dẫn HTTP.

### Câu 10: Ranh giới của Tầng Shared (src/Shared)
- **Vết code:** Hãy xem file `src/Shared/Validators/Products/ProductValidators.cs` và cách tầng API sử dụng nó.
- **Câu hỏi hội đồng:** "Project `src/Shared` được cả `src/API` và `src/Client` (Blazor WASM) tham chiếu vào. Việc một dự án dùng chung cho cả Frontend và Backend như vậy có làm hỏng quy tắc cô lập các tầng của Clean Architecture không? Lợi ích của nó là gì?"
- **Trả lời:**
  - Hoàn toàn **không làm hỏng quy tắc cô lập**, vì `src/Shared` chỉ chứa các DTOs (Data Transfer Objects), Enums, Helper tĩnh và Validators. Nó hoàn toàn không chứa bất kỳ logic nghiệp vụ cốt lõi hay mã truy vấn cơ sở dữ liệu nào.
  - *Lợi ích:* Đạt được tính đồng nhất tuyệt đối về cấu trúc dữ liệu và quy tắc kiểm thử đầu vào (Validation). Giúp tiết kiệm thời gian phát triển, tránh lặp lại code kiểm thử ở cả 2 đầu: Client kiểm thử ngay lập tức để tăng UX, Server kiểm thử lại một lần nữa để bảo mật tối đa trước khi lưu DB.

### Câu 11: Logic nghiệp vụ nằm ở đâu? (Domain vs Application)
- **Vết code:** File thực thể `src/Core/Entities/Sales/Order.cs` và File xử lý `src/Application/Orders/OrderService.cs`.
- **Câu hỏi hội đồng:** "Khi tính tổng tiền của một đơn hàng bao gồm cả áp mã giảm giá (Voucher), logic tính toán đó nên được viết thành một hàm nằm trong Entity `Order` (tầng Core) hay nằm trong `OrderService` (tầng Application)? Dựa vào quy tắc Clean Architecture, hãy phân biệt Anemic Domain Model và Rich Domain Model thông qua dự án của em."
- **Trả lời:**
  - Logic tính toán tổng tiền thuộc về quy tắc nghiệp vụ cốt lõi của đơn hàng (Domain Rule) nên được viết thành một hàm nằm trực tiếp trong Entity `Order` ở tầng **Core** (Domain). Tầng `OrderService` (Application) chỉ đóng vai trò điều phối (gọi dữ liệu từ repo, truyền voucher vào hàm của order và thực thi).
  - *Phân biệt:*
    - **Anemic Domain Model (Mô hình Domain nghèo nàn):** Các Class Entity chỉ chứa các thuộc tính Getter/Setter thuần túy để lưu dữ liệu (như các DTO), còn toàn bộ logic nghiệp vụ bị đẩy hết sang các lớp Service ở tầng Application.
    - **Rich Domain Model (Mô hình Domain giàu có - Dự án đang áp dụng):** Các Entity chứa cả dữ liệu lẫn các hành vi, quy tắc nghiệp vụ xoay quanh thực thể đó (ví dụ: hàm `CalculateTotal()`, `ApplyDiscount()` nằm ngay trong Class `Order`). Cách tiếp cận này giúp cô lập logic nghiệp vụ sâu sắc hơn, đúng chuẩn thiết kế hướng đối tượng (OOP) và Domain-Driven Design (DDD).

### Câu 12: Đảm bảo tính toàn vẹn dữ liệu qua Unit of Work
- **Vết code:** Interface `src/Core/Interfaces/IUnitOfWork.cs` và file cài đặt `src/Infrastructure/Data/UnitOfWork.cs`.
- **Câu hỏi hội đồng:** "Tầng Application sử dụng `IUnitOfWork` để làm gì? Tại sao chúng ta không gọi thẳng hàm `SaveChanges()` của `DbContext` từ tầng Application mà phải bọc nó qua Interface Unit of Work ở tầng Core?"
- **Trả lời:**
  - Tầng Application sử dụng `IUnitOfWork` để quản lý Transaction (giao dịch). Khi một nghiệp vụ cập nhật nhiều bảng cùng lúc, UoW đảm bảo tất cả các lệnh ghi dữ liệu đều được lưu thành công (Commit), hoặc nếu có 1 lệnh lỗi thì toàn bộ thay đổi trước đó sẽ bị thu hồi (Rollback).
  - Ta không gọi trực tiếp `DbContext.SaveChanges()` vì `DbContext` là class cụ thể thuộc thư viện Entity Framework Core nằm ở tầng Infrastructure. Nếu gọi trực tiếp, tầng Application sẽ bị dính chặt vào EF Core. Việc bọc qua Interface `IUnitOfWork` định nghĩa tại tầng Core giúp tầng Application hoàn toàn độc lập với EF Core và dễ dàng Mock khi viết Unit Test.

### Câu 13: Cấu hình DbContext đặc biệt và quy tắc Nghiệp vụ
- **Vết code:** Đoạn code cấu hình DbContext trong `src/API/Program.cs`:
  ```csharp
  // ServiceInvoice intentionally omits the query filter so financial records
  // remain queryable even after the parent ServiceTicket is soft-deleted.
  options.ConfigureWarnings(w => w.Ignore(...));
  ```
- **Câu hỏi hội đồng:** "Đoạn code này xử lý một quy tắc nghiệp vụ: Khi phiếu dịch vụ (ServiceTicket) bị xóa mềm, hóa đơn tài chính (ServiceInvoice) vẫn phải truy vấn được. Tại sao cấu hình kỹ thuật này (EF Core Query Filter) nằm ở tầng ngoài cùng nhưng lại phải phục vụ khắt khe cho quy tắc nghiệp vụ định nghĩa từ tầng trong?"
- **Trả lời:**
  - Đây là cấu hình kỹ thuật của EF Core (để tắt cảnh báo Possible Incorrect Required Navigation khi áp dụng Query Filter cho thực thể cha mà bỏ qua ở thực thể con). Nó phải nằm ở tầng ngoài (`Program.cs`) vì đó là nơi cấu hình khởi tạo DbContext.
  - Tuy nhiên, cấu hình kỹ thuật này được sinh ra nhằm bảo vệ tính đúng đắn của **Nghiệp vụ tài chính** (Hóa đơn không được phép biến mất khi Phiếu sửa chữa bị xóa). Điều này chứng minh rằng: *Mọi công nghệ và kỹ thuật ở tầng ngoài cùng của Clean Architecture sinh ra chỉ để phục vụ làm nền tảng cho việc thực thi trọn vẹn và an toàn các Quy tắc nghiệp vụ định nghĩa từ các tầng bên trong.*

### Câu 14: Middleware kiểm tra trạng thái User
- **Vết code:** Dòng lệnh `app.UseMiddleware<UserStatusMiddleware>();` trong `src/API/Program.cs` và file middleware tại `src/API/Middlewares/UserStatusMiddleware.cs`.
- **Câu hỏi hội đồng:** "Việc kiểm tra tài khoản User có bị khóa (`IsActive`) hay không là một logic kiểm tra quyền. Tại sao nó lại được tổ chức dưới dạng một Middleware ở tầng API thay vì viết thành code kiểm tra bên trong từng hàm của tầng Application? Tách biệt như vậy giải quyết bài toán gì trong Clean Architecture?"
- **Trả lời:**
  - Tổ chức dạng Middleware giúp **chặn đứng request không hợp lệ ngay từ cửa ngõ** trước khi nó đi sâu vào hệ thống, giải quyết bài toán kiểm tra tập trung (Cross-cutting Concerns).
  - *Lợi ích:* 
    1. **Tối ưu hóa tài nguyên:** Nếu viết kiểm tra ở Service, request phải đi qua tầng Controller, khởi tạo các Dependency của Service rồi mới phát hiện User bị khóa -> Lãng phí CPU và bộ nhớ. Middleware chặn ngay từ đầu sẽ giải phóng tài nguyên lập tức.
    2. **Tránh trùng lặp code:** Lập trình viên không cần phải viết đi viết lại đoạn code check `IsActive` ở từng Service hay Controller mới tạo, tránh rủi ro quên kiểm tra bảo mật ở các chức năng mới.

### Câu 15: Chiến lược viết Unit Test nhờ Clean Architecture
- **Vết code:** Xem cấu trúc thư mục test: `tests/PBL3.UnitTests/`.
- **Câu hỏi hội đồng:** "Giả sử tôi muốn test logic tạo đơn hàng trong `OrderService` (tầng Application) để xem nó có trừ đúng số lượng tồn kho không, nhưng tôi KHÔNG muốn chạy SQL Server thật để tránh sinh dữ liệu rác. Nhờ vào Clean Architecture, em sẽ dùng kỹ thuật gì (Gợi ý: Mocking các Interface của tầng Core) để thực hiện bài test này một cách cô lập hoàn toàn?"
- **Trả lời:**
  - Em sẽ sử dụng kỹ thuật **Mocking** (sử dụng thư viện như Moq hoặc NSubstitute) để tạo ra các thực thể giả lập của các Interface ở tầng Core như `IProductRepository`, `IOrderRepository`, và `IUnitOfWork`.
  - Khi viết Test, em sẽ thiết lập hành vi giả lập cho Mock (ví dụ: khi gọi `IProductRepository.GetVariantByIdAsync` thì trả về một Variant giả định có Price và Stock quy định sẵn trong RAM). Sau đó inject các Mock này vào constructor của `OrderService`.
  - Nhờ vậy, Unit Test có thể chạy kiểm tra logic tính toán của `OrderService` một cách cô lập hoàn toàn, không cần kết nối SQL Server thật, tốc độ chạy kiểm thử cực nhanh (chỉ mất vài mili-giây) và độc lập với trạng thái database.

---

## CHẶNG 3: LÝ THUYẾT NỀN TẢNG VỀ DATA ANNOTATIONS VS FLUENT API (Phần Tnguyn bị sấy)

### Câu 1: Định nghĩa và Bản chất kỹ thuật
- **Nội dung:** Hãy phân biệt bản chất của Data Annotations và Fluent API trong Entity Framework Core. Lập trình viên viết chúng ở đâu trong mã nguồn?
- **Bản chất:** 
  - **Data Annotations:** Là các thuộc tính (Attributes) được đặt ngay trên đầu của các Class hoặc các Property của Entity (ví dụ: `[Key]`, `[Required]`).
  - **Fluent API:** Là các chuỗi phương thức (Method Chaining) được viết tập trung bên trong hàm `OnModelCreating` của file kế thừa `DbContext`.
- **Trả lời:**
  - **Data Annotations:** Viết trực tiếp trên các thuộc tính/lớp Entity (nằm ở tầng Core). Ví dụ: `[Required]`, `[MaxLength(255)]`. Nó trực quan, dễ đọc đối với các cấu hình cơ bản.
  - **Fluent API:** Viết tập trung bên trong hàm `OnModelCreating` của `HushStoreDbContext` (nằm ở tầng Infrastructure). Nó sử dụng API hướng phương thức (Method Chaining) để định cấu hình.
  - *Lý do phân biệt:* Data Annotations dùng để định nghĩa nhanh các ràng buộc đơn giản, trong khi Fluent API cung cấp sức mạnh cấu hình tuyệt đối cho các mối quan hệ và đặc tính nâng cao của Database.

### Câu 2: Thứ tự ưu tiên (Precedence) khi EF Core quét cấu hình
- **Nội dung:** Nếu một thuộc tính (ví dụ: Tên sản phẩm `Name`) vừa được cấu hình độ dài tối đa là 100 bằng Data Annotation (`[MaxLength(100)]`), nhưng trong Fluent API lại viết là `.HasMaxLength(255)`, thì khi chạy Migration, EF Core sẽ tạo cột trong Database với độ dài bao nhiêu?
- **Quy tắc cốt lõi:** Thứ tự ghi đè cấu hình của EF Core diễn ra như thế nào giữa: Default Conventions (Mặc định) -> Data Annotations -> Fluent API?
- **Trả lời:**
  - EF Core sẽ tạo cột trong Database với độ dài **255**.
  - **Quy tắc cốt lõi về thứ tự ưu tiên:**
    1. **Default Conventions (Mặc định):** EF Core tự suy luận dựa vào kiểu dữ liệu C# (ví dụ: `string` -> `nvarchar(max)`).
    2. **Data Annotations:** Các Attribute ghi đè lên cấu hình mặc định (ví dụ: `[MaxLength(100)]` -> `nvarchar(100)`).
    3. **Fluent API:** Cấu hình trong DbContext là tối cao, sẽ ghi đè lên cả Data Annotations và Default Conventions (ví dụ: `.HasMaxLength(255)` sẽ ghi đè thuộc tính `100` thành `255`).

### Câu 3: Bản chất kiến trúc - Tại sao Clean Architecture lại khắt khe với Data Annotations?
- **Nội dung:** Xét về mặt lý thuyết Clean Architecture, tầng Core (Domain) chứa các Entity cần phải là POCO (Plain Old CLR Object) - tức là các Class C# thuần khiết, không bị phụ thuộc (ô nhiễm) bởi bất kỳ thư viện lưu trữ dữ liệu nào. Việc chúng ta dùng Data Annotations thuộc namespace `System.ComponentModel.DataAnnotations.Schema` có vi phạm nguyên lý này không? Tại sao nhiều lập trình viên theo trường phái "Clean" tuyệt đối lại cấm dùng Data Annotations và bắt buộc đẩy 100% cấu hình ra Fluent API ở tầng Infrastructure?
- **Trả lời:**
  - Việc dùng Data Annotations từ namespace của EF Core/Schema thực chất **có vi phạm nhẹ nguyên lý POCO**, vì nó bắt các class Entity nằm ở tầng Core phải mang theo các Annotation liên quan đến Database (Metadata phụ thuộc vào cách lưu trữ vật lý).
  - Nhiều lập trình viên "Clean" cấm dùng Data Annotations vì họ muốn:
    1. **Tách biệt tuyệt đối:** Tầng Core chỉ chứa thông tin nghiệp vụ thuần túy C#, không được chứa bất kỳ thông tin nào gợi ý về cách lưu trữ cơ sở dữ liệu (Database Metadata).
    2. **Độc lập lưu trữ:** Giữ các Entity hoàn toàn sạch sẽ để nếu chuyển sang NoSQL (như MongoDB không dùng schema quan hệ) thì không phải sửa các Attribute trên Entity ở tầng Core. Toàn bộ cấu hình DB vật lý phải được đẩy hết ra tầng Infrastructure thông qua Fluent API.

### Câu 4: Giới hạn năng lực - Việc gì Data Annotations KHÔNG THỂ LÀM ĐƯỢC?
- **Nội dung:** Có phải tất cả những gì cấu hình được bằng Fluent API thì đều có thể viết được bằng Data Annotations không? Hãy kể tên ít nhất 3 tính năng cấu hình Database nâng cao mà Data Annotations hoàn toàn bất lực, bắt buộc phải dùng Fluent API.
- **Trả lời:**
  - KHÔNG, Data Annotations không thể cấu hình được mọi thứ. Rất nhiều tính năng nâng cao chỉ có thể viết bằng Fluent API.
  - **3 tính năng Data Annotations không thể làm được:**
    1. **Global Query Filters:** Thiết lập bộ lọc mặc định trên toàn hệ thống (ví dụ: Xóa mềm tự động lọc bỏ các bản ghi có `IsDeleted == true` như trong `HushStoreDbContext.cs`).
    2. **Computed Columns:** Định nghĩa cột ảo tự động tính toán từ các cột khác ở dưới database (ví dụ: `TotalLine = Quantity * UnitPrice` thông qua `.HasComputedColumnSql()`).
    3. **Unique Index & Composite Keys:** Cấu hình chỉ mục duy nhất cho một trường (Unique Index) hoặc thiết lập khóa chính gồm nhiều cột (Composite Primary Key - ví dụ khóa chính bảng `VoucherCategory`).

### Câu 5: Tách biệt mối quan tâm (Separation of Concerns)
- **Nội dung:** Việc gom toàn bộ cấu hình Database vào Fluent API trong file `DbContext` (hoặc các file `IEntityTypeConfiguration<T>`) giúp ích gì cho việc quản lý mã nguồn so với việc rải rác các Attribute khắp các file Entity trong thư mục Core?
- **Trả lời:**
  - **Quản lý mã nguồn tập trung:** Giúp lập trình viên chỉ cần mở thư mục Data cấu hình trong tầng Infrastructure để xem toàn bộ cấu trúc sơ đồ Database thay vì phải đi dò tìm từng file Entity ở tầng Core.
  - **Entity tinh gọn, dễ đọc:** Các class Entity ở tầng Core chỉ tập trung mô tả nghiệp vụ và các trường dữ liệu, không bị che mắt bởi hàng chục dòng Annotation cấu hình DB dài dòng.
  - **Dễ bảo trì và phân tách cấu hình:** Khi dự án lớn, ta có thể tách cấu hình của từng bảng ra các file cấu hình riêng biệt (kế thừa `IEntityTypeConfiguration<T>`), giúp code sạch sẽ, dễ làm việc nhóm và tránh xung đột khi merge code.

---

## CHẶNG 4: TƯ DUY PHẢN BIỆN VÀ TRA CỨU CODE THỰC TẾ DỰ ÁN HUSHSTORE
*Hãy bắt bạn của bạn mở hai file `Product.cs` và `HushStoreDbContext.cs` lên để đối chiếu và trả lời các câu hỏi thực tế sau:*

### Câu 6: Đọc hiểu Data Annotations trong Entity
- **Vết code:** Mở file `src/Core/Entities/Products/Product.cs`.
- **Câu hỏi hội đồng:** "Nhìn vào file `Product.cs`, em hãy giải thích ý nghĩa của các Annotation `[Table("Products")]`, `[Key]`, `[Required]`, `[MaxLength(255)]` và `[ForeignKey("ManufacturerId")]`. Nếu tôi xóa Attribute `[Required]` ở trường `Slug` đi, Database sẽ thay đổi như thế nào (cho phép NULL hay NOT NULL)?"
- **Trả lời:**
  - Ý nghĩa các Annotation trong `Product.cs`:
    - `[Table("Products")]`: Yêu cầu EF Core map Entity này với bảng tên là `Products` trong SQL Server.
    - `[Key]`: Xác định thuộc tính này là Khóa chính (Primary Key) của bảng.
    - `[Required]`: Trường dữ liệu bắt buộc nhập, tương ứng cột `NOT NULL` dưới Database.
    - `[MaxLength(255)]`: Độ dài ký tự tối đa là 255, tương ứng kiểu dữ liệu `nvarchar(255)`.
    - `[ForeignKey("ManufacturerId")]`: Thiết lập quan hệ khóa ngoại liên kết tới thực thể `Manufacturer`.
  - Nếu xóa Attribute `[Required]` ở trường `Slug`, cột tương ứng dưới Database sẽ thay đổi từ **NOT NULL thành NULL** (cho phép chứa giá trị rỗng).

### Câu 7: Cấu hình Index và Unique bằng cách nào?
- **Vết code:** Mở file `src/Infrastructure/Data/HushStoreDbContext.cs`, tìm đoạn:
  ```csharp
  modelBuilder.Entity<Product>(entity => {
      entity.HasIndex(p => p.Slug).IsUnique();
  });
  ```
- **Câu hỏi hội đồng:** "Tại sao cấu hình Unique Index cho trường `Slug` (đảm bảo không trùng đường dẫn sản phẩm) lại phải viết ở Fluent API trong file DbContext mà không dùng Data Annotation bên file `Product.cs`? Data Annotation có Attribute nào tên là `[Unique]` không? Nếu không thì 'chỗ này không ổn' nếu cố viết ở Entity đúng không?"
- **Trả lời:**
  - Vì **Data Annotations trong C# không có thuộc tính nào tên là `[Unique]`** để thiết lập tính độc nhất độc lập cho một cột.
  - Do đó, việc cấu hình Unique Index bắt buộc phải viết bằng Fluent API thông qua hàm `.HasIndex(p => p.Slug).IsUnique()`. Cố tình tìm cách viết ở Entity bằng Data Annotation cho trường hợp này là không thể và không chuẩn xác.

### Câu 8: Global Query Filter (Xóa mềm) - Giới hạn tối thượng của Fluent API
- **Vết code:** Tìm trong `HushStoreDbContext.cs` đoạn cấu hình cho `Manufacturer` và `Product`:
  ```csharp
  modelBuilder.Entity<Manufacturer>(entity => {
      entity.HasQueryFilter(m => !m.IsDeleted);
  });
  modelBuilder.Entity<Product>(entity => {
      entity.HasQueryFilter(p => !p.IsDeleted && !p.Manufacturer.IsDeleted);
  });
  ```
- **Câu hỏi hội đồng:** "Cơ chế Xóa mềm (Soft Delete) hoạt động như thế nào thông qua hàm `.HasQueryFilter()`? Tại sao tính năng này bắt buộc phải dùng Fluent API mà không thể dùng Data Annotations? Khi một `Manufacturer` bị đặt `IsDeleted = true`, tại sao câu lệnh truy vấn `Product` của hệ thống lại tự động ẩn luôn các sản phẩm thuộc hãng đó?"
- **Trả lời:**
  - **Cơ chế hoạt động:** Hàm `.HasQueryFilter()` định cấu hình một bộ lọc mặc định. Mỗi khi lập trình viên viết câu lệnh LINQ để truy vấn thực thể này, EF Core sẽ tự động chèn thêm điều kiện `WHERE IsDeleted = 0` vào câu lệnh SQL được sinh ra mà lập trình viên không cần viết thủ công.
  - **Tại sao bắt buộc dùng Fluent API:** Vì Data Annotations hoàn toàn bất lực trong việc cấu hình các logic lọc dữ liệu động toàn cục như Query Filter.
  - **Tại sao ẩn luôn sản phẩm của hãng bị xóa:** Vì cấu hình Query Filter của `Product` được thiết lập điều kiện kép: `entity.HasQueryFilter(p => !p.IsDeleted && !p.Manufacturer.IsDeleted);`. Khi hãng sản xuất bị đánh dấu `IsDeleted = true`, điều kiện `!p.Manufacturer.IsDeleted` sẽ trả về `false`, khiến cho tất cả sản phẩm thuộc hãng đó tự động bị loại bỏ khỏi kết quả truy vấn.

### Câu 9: Xử lý kiểu dữ liệu đặc biệt (Decimal trong tài chính)
- **Vết code:** Xem cấu hình các trường tiền tệ trong `HushStoreDbContext.cs`:
  ```csharp
  entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
  ```
- **Câu hỏi hội đồng:** "Tại sao các trường lưu giá tiền như `Price`, `TotalAmount` bắt buộc phải cấu hình `HasColumnType("decimal(18,2)")`? Nếu không cấu hình rõ ràng bằng Fluent API hoặc Data Annotation `[Column(TypeName = "decimal(18,2)")]`, EF Core sẽ cảnh báo gì khi chạy Migration và Database sẽ bị ảnh hưởng gì về độ chính xác của tiền tệ?"
- **Trả lời:**
  - Kiểu dữ liệu `decimal` trong C# khi chuyển đổi xuống database mà không chỉ định chi tiết, EF Core mặc định sẽ cảnh báo vì SQL Server không biết độ chính xác mong muốn (mặc định sẽ dùng `decimal(18,2)` hoặc ném ra cảnh báo mất dữ liệu).
  - Cấu hình `"decimal(18,2)"` xác định cột lưu trữ có tối đa 18 chữ số, trong đó có 2 chữ số sau dấu thập phân.
  - Nếu không cấu hình, EF Core sẽ in cảnh báo cảnh giác mất dữ liệu thập phân khi chạy Migration. Dưới database, tiền tệ có thể bị làm tròn thành số nguyên, gây mất mát số lẻ và sai lệch tính toán tài chính nghiêm trọng cho doanh nghiệp.

### Câu 10: Column ảo tự động tính toán (Computed Column)
- **Vết code:** Đoạn code cấu hình `OrderDetail` trong `HushStoreDbContext.cs`:
  ```csharp
  modelBuilder.Entity<OrderDetail>(entity => {
      entity.Property(e => e.TotalLine)
            .HasColumnType("decimal(18,2)")
            .HasComputedColumnSql("([Quantity] * [UnitPrice])");
  });
  ```
- **Câu hỏi hội đồng:** "Trường `TotalLine` (Thành tiền của item đơn hàng) được cấu hình bằng hàm `.HasComputedColumnSql()`. Điều này nghĩa là gì? Lập trình viên có cần phải gán giá trị cho trường này khi code ở tầng Application không, hay Database sẽ tự tính? Data Annotations có làm được điều này trực tiếp không?"
- **Trả lời:**
  - Điều này nghĩa là `TotalLine` là một **cột tính toán (Computed Column)** ở mức Database. SQL Server sẽ tự động tính toán giá trị của cột này bằng công thức `Quantity * UnitPrice` mỗi khi dòng dữ liệu được truy vấn hoặc cập nhật.
  - Lập trình viên **hoàn toàn KHÔNG cần gán giá trị** cho trường này khi viết code tạo mới đơn hàng ở tầng Application. EF Core sẽ tự bỏ qua trường này trong câu lệnh `INSERT` và chỉ đọc giá trị tính sẵn từ Database lên.
  - Data Annotations không thể cấu hình trực tiếp công thức tính toán SQL này xuống database.

### Câu 11: Mối quan hệ phức tạp và Đệ quy (Recursive Relationship)
- **Vết code:** Đoạn code cấu hình `Category` (Danh mục cha - danh mục con):
  ```csharp
  entity.HasOne(c => c.Parent)
        .WithMany(p => p.Children)
        .HasForeignKey(c => c.ParentId)
        .OnDelete(DeleteBehavior.NoAction);
  ```
- **Câu hỏi hội đồng:** "Đây là cấu hình cho mối quan hệ Đệ quy (Danh mục chứa danh mục con bên trong). Tại sao việc chỉ định rõ ràng `DeleteBehavior.NoAction` ở đây lại cực kỳ quan trọng và vì sao cấu hình quan hệ phức tạp này Fluent API lại chiếm ưu thế tuyệt đối so với Data Annotations?"
- **Trả lời:**
  - Việc chỉ định `DeleteBehavior.NoAction` là cực kỳ quan trọng để **tránh lỗi Cascade Loop** (vòng lặp xóa dữ liệu). Nếu để mặc định xóa cascade, khi xóa một danh mục cha, SQL Server sẽ cố gắng tự động xóa toàn bộ danh mục con, danh mục cháu... gây lỗi bảo mật toàn vẹn hoặc bị SQL Server từ chối biên dịch cấu hình.
  - Fluent API chiếm ưu thế tuyệt đối vì cho phép định cấu hình chi tiết và trực quan mối quan hệ 1-N đệ quy bao gồm cả chiều khóa ngoại, navigation property (`Parent`, `Children`) và hành vi xóa (`NoAction`), điều mà Data Annotations chỉ cấu hình được một phần rất sơ sài bằng `[ForeignKey]` mà không kiểm soát được hành vi xóa cascade.

### Câu 12: Đỉnh cao Fluent API - Cấu hình Json Column và Value Comparer nâng cao
- **Vết code:** Hãy nhìn đoạn code "nặng đô" nhất trong `HushStoreDbContext.cs` xử lý trường `Specifications` (Thông số kỹ thuật lưu dạng JSON) của `ProductVariant`:
  ```csharp
  var specComparer = new ValueComparer<Dictionary<string, string>>(...);
  entity.Property(e => e.Specifications)
        .HasColumnType("nvarchar(max)")
        .HasConversion(...)
        .Metadata.SetValueComparer(specComparer);
  ```
- **Câu hỏi hội đồng:** "Trường `Specifications` trong code C# là một `Dictionary<string, string>`, nhưng xuống SQL Server nó được lưu thành chuỗi JSON dạng `nvarchar(max)`. Em đã dùng Fluent API làm những việc gì ở đây (`HasConversion`, `SetValueComparer`)? Hãy giải thích tại sao nếu không có `ValueComparer` tự chế này, EF Core sẽ liên tục sinh ra các câu lệnh UPDATE thừa dù dữ liệu không đổi? Code như vậy có bị đần không nếu cố dùng Data Annotation cho trường hợp này?"
- **Trả lời:**
  - Fluent API đã thực hiện 2 việc:
    1. `HasConversion`: Tự động serialize `Dictionary` thành chuỗi JSON khi lưu xuống DB, và deserialize chuỗi JSON ngược thành `Dictionary` khi đọc dữ liệu lên C#.
    2. `SetValueComparer`: Cung cấp bộ so sánh tùy chỉnh cho EF Core để nó biết cách so sánh hai `Dictionary` bằng cách kiểm tra số lượng và giá trị của từng cặp Key-Value bên trong.
  - *Tại sao cần ValueComparer:* Class `Dictionary` là một kiểu dữ liệu tham chiếu (Reference Type). Mặc định, EF Core so sánh theo địa chỉ ô nhớ. Khi đọc lên và chạy `SaveChanges`, dù nội dung bên trong không đổi nhưng tham chiếu khác nhau -> EF Core hiểu lầm dữ liệu đã bị sửa đổi và tự động chạy câu lệnh UPDATE thừa lên Database.
  - Cố tình dùng Data Annotation cho trường hợp này là không thể thực hiện được vì Data Annotation hoàn toàn không hỗ trợ cơ chế Value Converter và Value Comparer phức tạp của EF Core.

### Câu 13: Khắc phục lỗi Multiple Cascade Paths (Lỗi vòng lặp xóa dữ liệu)
- **Vết code:** Xem đoạn comment FIX lỗi trong cấu hình `OrderSerial`:
  ```csharp
  // FIX: SQL Server Error 1785 — Multiple cascade paths detected.
  entity.HasOne(os => os.OrderDetail)...OnDelete(DeleteBehavior.NoAction);
  ```
- **Câu hỏi hội đồng:** "Lỗi 'Multiple cascade paths' (Error 1785) của SQL Server là gì? Tại sao việc sử dụng Fluent API để ép kiểu xóa về `DeleteBehavior.NoAction` lại giải quyết được vấn đề này? Nếu chúng ta dùng Data Annotations mặc định, hệ thống có tự nhận biết để tránh lỗi này được không?"
- **Trả lời:**
  - **Lỗi Multiple Cascade Paths:** Xảy ra khi một bảng có nhiều đường liên kết khóa ngoại (Foreign Keys) dẫn tới việc xóa một bản ghi ở bảng cha sẽ kích hoạt hành vi xóa lan tỏa (Cascade Delete) theo nhiều đường dẫn khác nhau đến cùng một bảng con. SQL Server ngăn chặn việc này để tránh xung đột hoặc khóa chết dữ liệu (Deadlock).
  - **Giải pháp dùng NoAction:** Fluent API ép kiểu xóa về `DeleteBehavior.NoAction` để tắt cơ chế tự động xóa cascade của SQL Server trên đường dẫn khóa ngoại đó. Thay vào đó, việc xóa bản ghi liên quan sẽ được kiểm soát an toàn bằng code nghiệp vụ (Service logic).
  - Nếu dùng Data Annotations mặc định, hệ thống **không thể tự nhận biết để tránh lỗi này**. EF Core sẽ tự tạo Migration với thuộc tính Cascade mặc định và SQL Server sẽ lập tức ném lỗi 1785 khi chạy cập nhật cơ sở dữ liệu.

### Câu 14: Đổi tên bảng hàng loạt cho ASP.NET Core Identity
- **Vết code:** Xem đoạn code cấu hình các thực thể Auth ở đầu hàm `OnModelCreating`:
  ```csharp
  modelBuilder.Entity<AppUser>(entity => { entity.ToTable("AppUsers"); ... });
  modelBuilder.Entity<IdentityUserRole<Guid>>().ToTable("AppUserRoles");
  ```
- **Câu hỏi hội đồng:** "Mặc định, các bảng của thư viện ASP.NET Core Identity sẽ có tên là `AspNetUsers`, `AspNetUserRoles`. Nhờ vào Fluent API, em đã đổi tên chúng thành `AppUsers`, `AppUserRoles` theo convention của dự án bằng cách nào? Tại sao chúng ta không thể mở file `IdentityUserRole` ra để thêm thuộc tính `[Table]` vào đó?"
- **Trả lời:**
  - Em đổi tên bảng bằng cách sử dụng phương thức `.ToTable("AppUsers")` và `.ToTable("AppUserRoles")` cho từng thực thể Identity tương ứng trong hàm `OnModelCreating`.
  - Chúng ta **không thể mở file `IdentityUserRole` để thêm thuộc tính `[Table]`** vì Class này thuộc thư viện đóng gói sẵn (Compiled Library) của Microsoft (ASP.NET Core Identity). Lập trình viên không thể can thiệp hay sửa đổi trực tiếp mã nguồn của các class hệ thống này. Cách duy nhất để thay đổi cấu hình bảng của chúng là cấu hình đè qua Fluent API.

### Câu 15: Chốt hạ - Chiến lược phối hợp trong dự án thực tế
- **Câu hỏi hội đồng:** "Tóm lại, trong dự án HushStore này, em đã phối hợp Data Annotations và Fluent API theo chiến lược nào? Tại sao em không chọn giải pháp cực đoan là bỏ hẳn 1 trong 2 mà lại dùng cả hai?"
- **Trả lời:**
  - **Chiến lược phối hợp trong dự án HushStore:**
    - *Data Annotations:* Dùng cho các cấu hình cơ bản, ràng buộc nghiệp vụ mang tính hiển thị ngay tại tầng Core như: Bắt buộc (`[Required]`), Độ dài chuỗi (`[MaxLength]`), Tên bảng (`[Table]`), Khóa chính (`[Key]`). Điều này giúp Class Entity trực quan, dễ đọc, và các bộ thư viện khác (như validation ở Client) có thể đọc nhanh metadata này.
    - *Fluent API:* Dùng cho các cấu hình nâng cao, phức tạp liên quan mật thiết tới database vật lý ở tầng Infrastructure như: Chỉ mục độc nhất (Unique Index), Bộ lọc toàn cục (Global Query Filters), Hành vi xóa (DeleteBehavior), Kiểu dữ liệu đặc biệt (`decimal(18,2)`), các cột tính toán ảo (Computed Columns) và các Value Converters/Comparers.
  - *Tại sao không cực đoan bỏ 1 trong 2:* Sự phối hợp này đem lại điểm cân bằng hoàn hảo giữa tính trực quan của code ở tầng Domain (nhìn vào Entity biết ngay thuộc tính nào bắt buộc hoặc giới hạn độ dài) và sức mạnh cấu hình mạnh mẽ của Fluent API ở tầng Infrastructure mà Data Annotations hoàn toàn bất lực.

---

## PHẦN 3: 5 CÂU HỎI LÝ THUYẾT NỀN TẢNG VỀ DI VÀ REPOSITORY PATTERN

### Câu 1: Bản chất của DI (Dependency Injection) và IoC Container
- **Nội dung:** Dependency Injection (DI) là gì? Nó khác gì với Inversion of Control (IoC)?
- **Trọng tâm:** Trong .NET, việc chúng ta sử dụng `builder.Services` chính là đang cấu hình cho bộ IoC Container mặc định của Microsoft. Cơ chế này giúp chúng ta quản lý việc khởi tạo các đối tượng (Object Lifecycle) tự động như thế nào thay vì dùng từ khóa `new` thủ công?
- **Trả lời:**
  - **Dependency Injection (DI):** Là một mẫu thiết kế (Design Pattern) dùng để thực hiện nguyên lý Inversion of Control. Nó cung cấp các đối tượng phụ thuộc (Dependencies) cho một đối tượng khác từ bên ngoài thay vì để đối tượng đó tự khởi tạo.
  - **Inversion of Control (IoC):** Là một nguyên lý thiết kế rộng hơn, định nghĩa việc đảo ngược quyền điều khiển luồng hoạt động của ứng dụng từ lập trình viên sang cho Framework. DI là một cách hiện thực hóa IoC.
  - **Cơ chế quản lý khởi tạo tự động của IoC Container:**
    - Khi khởi chạy ứng dụng, IoC Container sẽ quét các đăng ký dịch vụ trong `builder.Services`.
    - Khi có một Request yêu cầu sử dụng một dịch vụ (ví dụ: Controller yêu cầu `IOrderService`), IoC Container sẽ tự phân tích cây phụ thuộc (Constructor Dependency Tree), tự động tạo mới instance của class triển khai cụ thể của `IOrderService` (cùng các repository phụ thuộc của nó) và tự động thu hồi/giải phóng bộ nhớ khi kết thúc vòng đời được cấu hình. Việc này loại bỏ hoàn toàn mã nguồn khởi tạo thủ công bằng từ khóa `new`.

### Câu 2: Phân biệt 3 Service Lifetimes (Transient, Scoped, Singleton)
- **Nội dung:** Hãy giải thích tường tận sự khác nhau về vòng đời của 3 chế độ nạp dịch vụ trong .NET:
  - `AddTransient()`
  - `AddScoped()`
  - `AddSingleton()`
- **Câu hỏi cốt lõi:** Một HTTP Request gửi từ Blazor WebAssembly đến API sẽ sinh ra một vòng đời Scoped như thế nào? Khi HTTP Request đó kết thúc, chuyện gì xảy ra với các đối tượng được gán nhãn Scoped?
- **Trả lời:**
  - **Transient (`AddTransient`):** Một instance mới sẽ được tạo ra **mỗi lần** dịch vụ được yêu cầu (mỗi lần resolve hoặc inject).
  - **Scoped (`AddScoped`):** Một instance duy nhất được tạo ra cho **mỗi vòng đời của một Request HTTP**. Trong cùng một request, tất cả các thành phần gọi đến dịch vụ này sẽ dùng chung một đối tượng.
  - **Singleton (`AddSingleton`):** Một instance duy nhất được tạo ra cho **toàn bộ vòng đời chạy của ứng dụng**. Tất cả các request của tất cả người dùng đều dùng chung một instance này.
  - **Vòng đời Scoped của một HTTP Request:**
    - Khi API nhận được HTTP Request từ Client, .NET Web Host sẽ tạo ra một Scope chứa các Service.
    - Suốt quá trình xử lý request từ API Controller qua tầng Service tới Repository, các instance Scoped (như `HushStoreDbContext`) được tạo ra và dùng chung.
    - Khi HTTP Request hoàn tất và Response được gửi ngược về cho Client, Scope này sẽ bị phá hủy. .NET Runtime sẽ tự động gọi hàm `Dispose()` trên tất cả đối tượng Scoped để thu hồi bộ nhớ, đóng kết nối Database.

### Câu 3: Bản chất của Repository Pattern
- **Nội dung:** Repository Pattern là gì? Tại sao Clean Architecture lại khuyên dùng Repository để bọc (wrap) lớp truy xuất dữ liệu lại, thay vì cho phép tầng Application gọi trực tiếp `DbContext` của Entity Framework Core để kéo dữ liệu?
- **Trả lời:**
  - **Repository Pattern:** Là lớp trừu tượng nằm giữa tầng nghiệp vụ (Application Logic) và tầng truy cập dữ liệu (Data Access), đóng vai trò bọc lại các câu lệnh truy vấn dữ liệu thô thành các phương thức nghiệp vụ rõ ràng (như `GetByIdAsync`, `GetPagedListAsync`).
  - **Tại sao khuyên dùng thay vì gọi trực tiếp DbContext:**
    1. **Tách biệt mối quan tâm:** Giữ cho tầng Application sạch sẽ, không bị ô nhiễm bởi các câu truy vấn LINQ phức tạp hay cú pháp đặc thù của EF Core.
    2. **Dễ bảo trì:** Nếu cơ sở dữ liệu thay đổi cấu trúc bảng, ta chỉ cần vào một file Repository tương ứng để sửa câu query, không cần đi rà soát và chỉnh sửa ở hàng chục file Service của tầng Application.
    3. **Hỗ trợ Unit Test tối đa:** Giúp tầng Application dễ dàng viết test độc lập bằng cách giả lập (Mock) Interface Repository mà không phải khởi dựng hay kết nối thật tới database DbContext phức tạp.

### Câu 4: Cặp bài trùng - Tại sao DI và Repository thường phải đi chung?
- **Nội dung:** Tại sao người ta nói: "Nếu dùng Repository Pattern mà không dùng Dependency Injection thì kiến trúc đó cực kỳ cồng kềnh, thậm chí là đần"? Cơ chế DI giúp tầng Application gọi các hàm trong Repository mà không cần biết class cụ thể nào đang triển khai (Implementation) code dưới Database bằng cách nào?
- **Trả lời:**
  - **Tại sao nói cồng kềnh/đần nếu không có DI:** Nếu không dùng DI, tầng Application muốn dùng Repository sẽ phải tự khởi tạo bằng từ khóa `new ProductRepository(new HushStoreDbContext(options))`. Việc tự khởi tạo này làm hỏng ý nghĩa độc lập của tầng nghiệp vụ, khiến Application phụ thuộc cứng vào Class cụ thể của Infrastructure, gây ra sự phụ thuộc chặt chẽ (Tight Coupling) và triệt tiêu khả năng viết Unit Test cô lập.
  - **Cơ chế DI giúp ích như thế nào:** Tầng Application chỉ khai báo Interface (ví dụ: `IProductRepository`) trong Constructor của nó. Khi ứng dụng khởi chạy, DI Container tự động phân giải và tiêm Class triển khai thật (`ProductRepository`) vào. Tầng Application hoàn toàn không biết và không quan tâm Class đó lấy dữ liệu bằng EF Core SQL Server hay API bên thứ 3, nó chỉ việc gọi hàm định nghĩa sẵn trên Interface để lấy dữ liệu.

### Câu 5: Khái niệm Unit of Work (UoW) Pattern
- **Nội dung:** Unit of Work là gì? Tại sao khi hệ thống có nhiều Repository riêng lẻ (như `IOrderRepository`, `IProductRepository`) thì chúng lại cần một ông "quản ca" tên là `IUnitOfWork` để quản lý chung? Mối quan hệ giữa Repository và Unit of Work trong một chuỗi các thao tác ghi dữ liệu (Insert/Update/Delete) là gì?
- **Trả lời:**
  - **Unit of Work:** Là mẫu thiết kế quản lý một Transaction (giao dịch) duy nhất xuyên suốt nhiều Repository khác nhau.
  - **Tại sao cần Unit of Work làm "quản ca":** Nếu mỗi Repository tự quản lý một DbContext riêng và tự gọi `SaveChanges()` riêng, khi thực hiện một nghiệp vụ phức tạp (ví dụ đặt hàng: vừa tạo Đơn hàng ở `OrderRepository` vừa trừ Tồn kho ở `ProductRepository`), nếu bước trừ tồn kho bị lỗi, ta không thể rollback được bản ghi đơn hàng đã lưu thành công trước đó ở `OrderRepository`.
  - **Mối quan hệ trong chuỗi thao tác ghi:**
    - Cả `OrderRepository` và `ProductRepository` cùng chia sẻ chung một instance `DbContext` do Unit of Work quản lý.
    - Trong chuỗi thao tác, các repository chỉ đưa các thay đổi vào DbContext (ghi nhận tạm thời).
    - Sau khi tất cả các repository đã hoàn thành công việc của mình, tầng Application sẽ gọi hàm `Save` của `IUnitOfWork` một lần duy nhất để gửi toàn bộ các thay đổi xuống Database dưới dạng một Transaction duy nhất. Nếu có bất kỳ lỗi nào xảy ra trong chuỗi, Transaction sẽ bị hủy bỏ (Rollback) toàn bộ, đảm bảo tính toàn vẹn dữ liệu.

---

## PHẦN 4: 10 CÂU HỎI TƯ DUY, PHẢN BIỆN QUA CODE THỰC TẾ CỦA DỰ ÁN HUSHSTORE
*(Hãy bắt bạn của bạn mở file `src/API/Extensions/RepositoryExtensions.cs` ra để vừa đối chiếu code và trả lời)*

### Câu 6: Tại sao 100% Repository trong dự án lại dùng AddScoped?
- **Vết code:** Nhìn vào toàn bộ file `RepositoryExtensions.cs`:
  ```csharp
  services.AddScoped<ICategoryRepository, CategoryRepository>();
  services.AddScoped<IProductRepository, ProductRepository>();
  // ...
  ```
- **Câu hỏi hội đồng:** "Tại sao em lại chọn `AddScoped` cho toàn bộ các Repository và Unit of Work? Nếu tôi đổi tất cả thành `AddTransient` hoặc `AddSingleton` thì hệ thống sẽ bị lỗi gì? Nếu dùng `Singleton` cho Repository tương tác với `DbContext` thì chỗ này không ổn ở điểm nào?"
- **Trả lời:**
  - Em chọn `AddScoped` vì các Repository này cần sử dụng `HushStoreDbContext` để truy cập cơ sở dữ liệu. Bản thân `DbContext` của EF Core được mặc định đăng ký ở dạng Scoped nhằm đảm bảo an toàn giao dịch trên mỗi HTTP Request.
  - **Nếu đổi sang: **
    - *Transient:* Mỗi khi Resolve một Repository mới, một DbContext mới lại được khởi tạo. Điều này gây lãng phí tài nguyên và làm mất tính nhất quán giao dịch (các repo không dùng chung 1 DbContext -> Unit of Work mất tác dụng).
    - *Singleton:* Gây lỗi xung đột luồng nghiêm trọng (Thread Safety Exception). `DbContext` không được thiết kế cho xử lý đồng thời (not thread-safe). Khi nhiều Request của nhiều người dùng cùng gọi API một lúc, việc dùng chung một instance DbContext sẽ lập tức gây lỗi crash hệ thống hoặc rò rỉ dữ liệu.

### Câu 7: Truy vết luồng phân giải DI (Dependency Resolution)
- **Vết code:** Một Service bất kỳ ở tầng Application có constructor nhận vào `IProductRepository`.
- **Câu hỏi hội đồng:** "Khi một Request gọi đến API, .NET Runtime sẽ làm cách nào để biết cần phải tạo ra instance của class `ProductRepository` (nằm ở tầng Infrastructure) để truyền vào constructor của Service? Hãy chỉ ra dòng lệnh trong file `RepositoryExtensions.cs` thực hiện giao kèo (mapping) này."
- **Trả lời:**
  - .NET Runtime làm được điều này nhờ cơ chế **Constructor Injection** tự động của IoC Container.
  - Khi Request gọi đến API, IoC Container kiểm tra constructor của Service cần khởi tạo. Thấy Service yêu cầu một tham số kiểu `IProductRepository`, IoC Container sẽ tra cứu trong danh sách đăng ký dịch vụ đã nạp từ trước.
  - Dòng lệnh thực hiện giao kèo này nằm ở dòng 12 trong file `src/API/Extensions/RepositoryExtensions.cs`:
    ```csharp
    services.AddScoped<IProductRepository, ProductRepository>();
    ```
    Dòng này đăng ký với IoC Container rằng: Mỗi khi có đối tượng yêu cầu Interface `IProductRepository`, hãy khởi tạo và tiêm (inject) class cụ thể `ProductRepository` vào.

### Câu 8: Phản biện cực gắt từ Hội đồng về "Bọc DbContext" (Thầy cô rất hay hỏi câu này)
- **Vết code:** Dòng cuối cùng của file `RepositoryExtensions.cs`:
  ```csharp
  services.AddScoped<IUnitOfWork, UnitOfWork>();
  ```
- **Câu hỏi hội đồng:** "Bản thân DbContext của Entity Framework Core đã được thiết kế theo Pattern Unit of Work, và mỗi `DbSet<T>` đã là một Repository rồi. Tại sao em lại còn mất thời gian viết thêm một lớp `IUnitOfWork` và các `IRepository` tự chế bọc bên ngoài DbContext làm gì cho phức tạp và tốn bộ nhớ? Code vậy có thấy bị đần không?"
- **Mẹo trả lời để ăn điểm tuyệt đối:**
  - Khẳng định: Code thiết kế như vậy hoàn toàn **không bị đần**, mà là một quyết định kiến trúc chiến lược dựa trên các mục tiêu sau:
    1. **Bảo vệ tính cô lập của tầng Application (Separation of Concerns):** Nếu gọi trực tiếp DbContext từ tầng Service, tầng Application sẽ bị ràng buộc trực tiếp và phụ thuộc hoàn toàn vào thư viện Entity Framework Core. Nếu tương lai muốn thay EF Core bằng Dapper hoặc gọi API ngoài để lấy dữ liệu, ta sẽ phải sửa đổi code ở toàn bộ tầng Application. Tách biệt qua Repository/UoW giúp tầng Application hoàn toàn độc lập với thư viện truy cập dữ liệu bên ngoài.
    2. **Độc lập kiểm thử (Unit Testing):** EF Core DbContext là một class cực kỳ phức tạp và rất khó để Mock khi viết Unit Test. Bọc DbContext bằng các Interface đơn giản (`IProductRepository`, `IUnitOfWork`) giúp chúng ta Mock dữ liệu một cách cực kỳ dễ dàng và tin cậy mà không cần chạy SQL Server vật lý.

### Câu 9: Quản lý lỗi Captive Dependency (Lỗi vòng đời phụ thuộc)
- **Câu hỏi hội đồng:** "Nếu trong file `Program.cs` hoặc cấu hình dịch vụ, tôi đăng ký `IUnitOfWork` là Scoped, nhưng một Service xử lý Background Job (chạy nền suốt đời ứng dụng) được đăng ký là Singleton lại gọi đến `IUnitOfWork` này trong constructor của nó. Hệ thống sẽ báo lỗi gì khi khởi chạy (Runtime Error) và tại sao .NET lại nghiêm cấm hành vi này?"
- **Trả lời:**
  - Hệ thống sẽ báo lỗi **Captive Dependency** (Lỗi giam giữ phụ thuộc) ngay khi khởi chạy hoặc ném ra Exception khi ứng dụng phân giải phụ thuộc ở runtime.
  - **Lý do .NET nghiêm cấm:** Vì một dịch vụ có tuổi thọ dài (Singleton Background Job) nếu chứa trong constructor một dịch vụ có tuổi thọ ngắn (Scoped UoW/DbContext) sẽ khiến đối tượng Scoped đó bị "giữ chặt" (captive) và tồn tại suốt đời của Singleton Service. Điều này phá hỏng cơ chế thu hồi bộ nhớ tự động của Scoped (không thể giải phóng DbContext, không thể đóng kết nối DB), dẫn đến rò rỉ bộ nhớ (Memory Leak) và lỗi xung đột kết nối dữ liệu khi nhiều luồng cùng gọi.

### Câu 10: Vai trò của IUnitOfWork trong việc quản lý Transaction nghiệp vụ
- **Vết code:** Hãy tưởng tượng kịch bản tạo đơn hàng (Checkout): Hệ thống phải lưu thông tin đơn hàng vào bảng `Orders` (thông qua `IOrderRepository`), đồng thời phải giảm số lượng tồn kho trong bảng `ProductVariants` (thông qua `IProductRepository`).
- **Câu hỏi hội đồng:** "Làm thế nào để em đảm bảo nếu việc trừ tồn kho bị lỗi (ví dụ: hết hàng giữa chừng), thì thông tin đơn hàng đã thêm trước đó phải được hủy bỏ (Rollback) hoàn toàn khỏi Database để tránh lệch dữ liệu? Dòng code `services.AddScoped<IUnitOfWork, UnitOfWork>();` giải quyết bài toán một Transaction duy nhất này như thế nào?"
- **Trả lời:**
  - Để đảm bảo tính toàn vẹn dữ liệu, toàn bộ quy trình checkout được thực hiện bên trong một Database Transaction được mở ra thông qua Unit of Work.
  - Dòng code đăng ký `IUnitOfWork` giải quyết bài toán này như sau:
    1. Cả `IOrderRepository` và `IProductRepository` cùng sử dụng chung một instance `HushStoreDbContext` duy nhất trong cùng một Request HTTP nhờ cơ chế Scoped.
    2. Khi bắt đầu Checkout, ta gọi `await _unitOfWork.BeginTransactionAsync();`.
    3. Lưu đơn hàng và trừ tồn kho chỉ mới được cập nhật tạm thời trong bộ nhớ của DbContext.
    4. Nếu quá trình trừ tồn kho bị lỗi, code rơi vào khối `catch` và thực thi lệnh `await _unitOfWork.RollbackAsync();`. SQL Server lập tức hủy bỏ toàn bộ các lệnh ghi trước đó.
    5. Nếu mọi thứ suôn sẻ, lệnh `await _unitOfWork.CommitAsync();` sẽ ghi nhận vĩnh viễn tất cả các thay đổi cùng một lúc xuống Database.

### Câu 11: Tư duy thiết kế - Rò rỉ cấu trúc EF Core (Leaking Abstraction)
- **Câu hỏi hội đồng:** "Trong các hàm định nghĩa ở Interface Repository (tầng Core), nếu em viết một hàm trả về kiểu dữ liệu là `IQueryable<Product>`, điều này mang lại lợi ích gì về mặt hiệu năng (Deferred Execution - thực thi trì hoãn)? Nhưng xét về mặt Clean Architecture, tại sao việc trả về `IQueryable` ra khỏi tầng Infrastructure lại khiến chỗ này không ổn và làm hỏng ý nghĩa của Repository Pattern?"
- **Trả lời:**
  - *Lợi ích hiệu năng:* Trả về `IQueryable` cho phép tầng ngoài tiếp tục áp dụng thêm các bộ lọc LINQ (như Select, Where) và chỉ thực sự truy vấn Database khi gọi các hàm kết thúc (`ToList()`, `Count()`), giúp tối ưu hóa câu lệnh SQL được sinh ra dưới database.
  - *Tại sao làm hỏng ý nghĩa Repository (Leaking Abstraction):* 
    - `IQueryable` là một sự trừu tượng bị rò rỉ công nghệ (Leaking Abstraction). Để biên dịch và thực thi tiếp `IQueryable`, các tầng ngoài (Application/Presentation) buộc phải cài đặt thư viện `Microsoft.EntityFrameworkCore` và sử dụng LINQ-to-Entities. Điều này làm lộ công nghệ truy xuất dữ liệu của tầng Infrastructure ra các tầng trong.
    - Làm mất tác dụng bảo vệ của Repository Pattern: Tầng Application giờ đây có thể tự do viết các câu query tùy ý lên `IQueryable`, dẫn đến logic truy vấn DB bị phân tán rải rác khắp nơi thay vì được gom tụ tại Repository.

### Câu 12: Generic Repository vs Specific Repository (Repository chung hay riêng)
- **Vết code:** Trong dự án, bạn đăng ký rất nhiều Repository cụ thể như `IProductReviewRepository`, `IServiceTicketRepository`, `IRmaShipmentRepository`.
- **Câu hỏi hội đồng:** "Tại sao em không thiết kế một `IGenericRepository<T>` dùng chung cho tất cả các thực thể để file này chỉ có 1 dòng đăng ký DI, mà lại phải viết thủ công từng Repository cụ thể cho từng thực thể như vậy? Ưu và nhược điểm của hai cách tiếp cận này trong dự án thực tế là gì?"
- **Trả lời:**
  - Em chọn thiết kế Specific Repository (Repository riêng biệt) thay vì Generic Repository vì các nghiệp vụ của dự án tương đối phức tạp và có nhiều logic truy vấn đặc thù (ví dụ: `IProductRepository` có hàm `FilterBySpecificationAsync` xử lý JSON Column).
  - **Ưu và nhược điểm:**
    - *Generic Repository (`IGenericRepository<T>`):*
      - *Ưu điểm:* Tiết kiệm thời gian viết code lúc đầu, chỉ cần viết một Interface và một Class dùng chung cho mọi thực thể, cấu hình DI cực nhanh.
      - *Nhược điểm:* Khó viết các câu query phức tạp (như Join nhiều bảng, Include phân tầng, xử lý truy vấn SQL thuần). Dễ dẫn đến việc rò rỉ `IQueryable` ra ngoài để tầng ngoài tự viết query.
    - *Specific Repository (Repository riêng biệt - Dự án áp dụng):*
      - *Ưu điểm:* Kiểm soát chặt chẽ các câu lệnh truy vấn DB của từng thực thể, tối ưu hóa hiệu năng câu query dễ dàng, không bị rò rỉ công nghệ ra ngoài.
      - *Nhược điểm:* Tốn thời gian viết code ban đầu (mỗi thực thể phải tự viết 1 file Interface và 1 file Class), file đăng ký DI phình to.

### Câu 13: Bản chất kiến trúc của file RepositoryExtensions.cs
- **Vết code:** File này nằm ở thư mục `src/API/Extensions/`, sử dụng các namespace:
  ```csharp
  using PBL3.Core.Interfaces;
  using PBL3.Infrastructure.Data;
  using PBL3.Infrastructure.Repositories;
  ```
- **Câu hỏi hội đồng:** "File này nằm ở tầng API (Presentation) nhưng lại trực tiếp using cả tầng Core lẫn tầng Infrastructure. Việc tầng API biết hết tất cả các tầng bên trong và đứng ra làm nhiệm vụ cấu hình DI này có vi phạm nguyên tắc 'các tầng vòng ngoài không được biết nhau' của Clean Architecture không? Tại sao?"
- **Trả lời:**
  - Hoàn toàn **KHÔNG vi phạm** nguyên tắc của Clean Architecture.
  - *Lý do:* Tầng API đóng vai trò là **Composition Root** (Điểm lắp ráp ứng dụng). Để một chương trình có thể chạy được, bắt buộc phải có một điểm đứng ra thu thập tất cả các mảnh ghép độc lập (Core, Application, Infrastructure) để kết nối chúng lại thành một thực thể duy nhất. Việc using ở file Extension tại tầng API chỉ phục vụ cho cấu hình DI lúc khởi động ứng dụng, hoàn toàn không làm ảnh hưởng đến tính độc lập nghiệp vụ của tầng Core hay Application ở Runtime.

### Câu 14: Kỹ thuật Mocking phục vụ kiểm thử nhờ DI và Repository
- **Câu hỏi hội đồng:** "Nhờ vào việc tách biệt Interface (như `IOrderRepository`) và Class triển khai cụ thể, nếu bây giờ tôi yêu cầu em viết code Unit Test để kiểm tra logic tính tiền đơn hàng mà KHÔNG ĐƯỢC phép cài đặt hay kết nối vào SQL Server thật, em sẽ tận dụng DI và Interface này như thế nào để truyền một dữ liệu 'giả lập' (Mock Data) vào hệ thống?"
- **Trả lời:**
  - Em sẽ sử dụng một thư viện Mocking (như Moq trong C#) để khởi tạo một đối tượng giả lập từ Interface `IOrderRepository`:
    ```csharp
    var mockOrderRepo = new Mock<IOrderRepository>();
    ```
  - Sau đó cấu hình hành vi giả định cho đối tượng Mock này (ví dụ: khi gọi hàm `GetByIdAsync(1)` thì trả về một object `Order` giả lập lưu trong bộ nhớ RAM có sẵn giá tiền và số lượng).
  - Tiếp theo, em truyền đối tượng `mockOrderRepo.Object` này vào constructor của Service lúc viết test. Nhờ có DI sử dụng Interface, Service sẽ chấp nhận đối tượng Mock này như một Repository thật và chạy logic bình thường, giúp em kiểm thử được logic nghiệp vụ một cách nhanh chóng và chính xác mà không cần động vào cơ sở dữ liệu SQL Server.

### Câu 15: Phản biện về Hiệu năng (Performance Overhead)
- **Câu hỏi hội đồng:** "Việc đi qua quá nhiều lớp trung gian (Tầng API -> Tầng Service -> Interface Unit of Work -> Interface Repository -> Class Repository -> DbContext -> SQL Server) có khiến ứng dụng của em chạy chậm hơn so với việc tôi viết một file code duy nhất kéo dữ liệu thẳng từ Database lên giao diện không? Đổi lại sự đánh đổi về hiệu năng siêu nhỏ đó, đồ án của em đạt được lợi ích tối thượng gì về mặt kỹ thuật phần mềm?"
- **Trả lời:**
  - **Về hiệu năng:** Về mặt lý thuyết, việc gọi qua các hàm ủy quyền trung gian có gây ra một chi phí hiệu năng (Overhead) cực kỳ nhỏ (cỡ nano-giây), hoàn toàn không thể nhận thấy được trong thực tế so với thời gian chờ kết nối mạng hoặc thời gian truy vấn dưới cơ sở dữ liệu.
  - **Lợi ích tối thượng đạt được:**
    1. **Tính bảo trì (Maintainability):** Code được phân tách rõ ràng, khi có lỗi xảy ra dễ dàng khoanh vùng để sửa chữa (lỗi SQL sửa ở repo, lỗi logic sửa ở service, lỗi UI sửa ở client).
    2. **Tính mở rộng (Scalability):** Dễ dàng nâng cấp, thay thế công nghệ (đổi database, đổi cổng thanh toán) mà không lo phá vỡ logic cũ.
    3. **Tính dễ kiểm thử (Testability):** Giúp viết Unit Test bao phủ mã nguồn, đảm bảo hệ thống chạy ổn định và tự tin khi refactor hoặc thêm tính năng mới.

---

## CHẶNG 5: LÝ THUYẾT & TƯ DUY PHẢN BIỆN VỀ RESTful API VÀ BẢO MẬT (JWT, MIDDLEWARE)

### Câu 1: Bản chất của RESTful API và ràng buộc "Không trạng thái" (Stateless)
- **Nội dung:** REST là gì? Một Web API được gọi là đạt chuẩn RESTful khi tuân thủ những ràng buộc nào?
- **Trọng tâm phản biện:** Hãy giải thích cơ chế Stateless (Không lưu trạng thái) của RESTful API. Nếu hệ thống Backend không hề lưu bất kỳ thông tin nào về phiên đăng nhập của người dùng vào bộ nhớ Server (Session), thì làm sao ở Request tiếp theo, API biết được người dùng đó là ai để cho phép họ đặt hàng?
- **Trả lời:**
  - **REST (Representational State Transfer):** Là một phong cách kiến trúc thiết kế hệ thống phân tán sử dụng giao thức truyền thông phi trạng thái (thường là HTTP).
  - **Các ràng buộc đạt chuẩn RESTful:** (1) Client-Server, (2) Stateless (Không trạng thái), (3) Cacheable (Có thể lưu cache), (4) Uniform Interface (Giao diện đồng nhất), (5) Layered System (Hệ thống phân tầng).
  - **Cơ chế Stateless và Xác thực:** 
    - Vì API là Stateless, Server không lưu trữ Session của Client trong bộ nhớ (RAM Server).
    - Thay vào đó, mỗi khi Client đăng nhập thành công, Server sẽ trả về một chuỗi mã hóa gọi là **JSON Web Token (JWT)** chứa thông tin định danh của người dùng.
    - Ở mỗi HTTP Request tiếp theo (ví dụ: gửi yêu cầu đặt hàng), Client bắt buộc phải đính kèm Token này vào Header của Request (`Authorization: Bearer <Token>`).
    - API Server chỉ việc giải mã chữ ký của Token này tại thời điểm nhận request để biết người dùng đó là ai và có quyền gì, thực hiện xử lý lập tức rồi trả về kết quả mà không cần lưu giữ bất kỳ trạng thái nào trước đó.

### Câu 2: Cấu trúc một HTTP Request/Response & Tư duy thiết kế Status Code
- **Nội dung:** Một HTTP Request gửi từ Blazor WASM lên API bao gồm những thành phần cốt lõi nào (Method, URL, Headers, Body)? Tương tự với một HTTP Response trả về.
- **Trọng tâm phản biện:** Thầy cô rất dị ứng với việc thiết kế lỗi. Nếu API thực hiện một tác vụ thất bại (ví dụ: Không tìm thấy sản phẩm, hoặc người dùng nhập sai mật khẩu), nhưng API vẫn trả về mã HTTP Status Code 200 OK kèm theo một chuỗi JSON `{ "success": false, "message": "Lỗi rồi" }`, thì code vậy có thấy bị đần không? Tại sao việc lạm dụng mã 200 OK cho mọi tình huống lại phá hỏng tính chuẩn hóa của kiến trúc API?
- **Trả lời:**
  - **HTTP Request:** Gồm các thành phần: HTTP Method (`GET`, `POST`...), URL/Endpoint, Headers (chứa metadata như `Content-Type`, `Authorization`), và Body (chứa dữ liệu gửi lên dưới dạng JSON).
  - **HTTP Response:** Gồm: Status Code (mã trạng thái như `200`, `404`), Headers, và Body (dữ liệu phản hồi JSON).
  - **Phản biện về việc lạm dụng mã 200 OK:**
    - Code thiết kế trả về 200 OK cho các lỗi nghiệp vụ/hệ thống là **không chuẩn hóa và không chuyên nghiệp**.
    - *Lý do:* Làm sụp đổ cơ chế hoạt động của giao thức HTTP chuẩn. Các công cụ giám sát hệ thống (Monitoring Tools), bộ lọc CDN, Cổng bảo mật (API Gateways), và thư viện HTTP Client ở Frontend hoạt động dựa trên mã Status Code để nhận biết lỗi và ghi log tự động. Nếu luôn trả về 200 OK, hệ thống giám sát sẽ hiểu nhầm tất cả các request đều thành công tốt đẹp, trong khi người dùng thực tế đang gặp lỗi tràn ngập. Thiết kế chuẩn phải sử dụng đúng mã HTTP Status Code (ví dụ: `404` cho không tìm thấy, `401` cho chưa đăng nhập, `422` cho vi phạm nghiệp vụ dữ liệu).

### Câu 3: Định tuyến (Routing) và Vai trò của [ApiController] trong ASP.NET Core
- **Nội dung:** Hãy giải thích cơ chế định tuyến thuộc tính (Attribute Routing) trong ASP.NET Core thông qua các Annotation như `[Route("api/[controller]")]`, `[HttpGet]`, `[HttpPost]`.
- **Trọng tâm phản biện:** Khi đặt thẻ `[ApiController]` trên đầu một Class Controller, thẻ này tự động kích hoạt những tính năng ngầm định nào cho chúng ta (ví dụ: Tự động Validate Model trạng thái, tự động bắt buộc Attribute Routing)? Nếu không có thẻ này, chúng ta sẽ phải viết thêm những đoạn code thủ công nào ở từng Action Method?
- **Trả lời:**
  - **Attribute Routing (Định tuyến thuộc tính):** Là cơ chế ánh xạ trực tiếp URL của HTTP Request tới Action Method cụ thể trong Controller bằng các Annotation đặt trước Class hoặc Method (ví dụ: `[Route("api/[controller]")]` thay thế tên Controller vào URL; `[HttpGet("{id}")]` ánh xạ HTTP GET kèm tham số ID).
  - **Tính năng tự động của `[ApiController]`:**
    1. **Tự động Model Validation:** Tự động kiểm tra `ModelState.IsValid`. Nếu dữ liệu DTO gửi lên vi phạm quy tắc validate (ví dụ thiếu trường bắt buộc), API sẽ tự động trả về lỗi `400 Bad Request` kèm chi tiết lỗi mà không cần vào đến code của Action Method.
    2. **Bắt buộc Attribute Routing:** Ngăn chặn việc sử dụng định tuyến truyền thống kiểu cũ, tăng tính tường minh cho REST API.
    3. **Tự động suy luận tham số nguồn (Binding Source Inference):** Tự hiểu tham số phức tạp lấy từ Body (`[FromBody]`), tham số cơ bản lấy từ Query (`[FromQuery]`).
  - **Nếu không có thẻ này:** Ở mỗi Action Method, lập trình viên sẽ phải viết code kiểm tra thủ công dạng:
    ```csharp
    if (!ModelState.IsValid) { return BadRequest(ModelState); }
    ```
    Và phải đính kèm các thẻ Binding nguồn như `[FromBody]`, `[FromQuery]` một cách thủ công ở trước từng tham số rất rườm rà.

### Câu 4: Phân biệt rạch ròi giữa Authentication (Xác thực) và Authorization (Phân quyền)
- **Nội dung:** Hãy định nghĩa và phân biệt rõ ràng sự khác nhau giữa Xác thực (Authentication) và Phân quyền (Authorization).
- **Trọng tâm phản biện:** Trong file `src/API/Program.cs`, chúng ta bắt buộc phải gọi theo thứ tự:
  ```csharp
  app.UseAuthentication();
  app.UseAuthorization();
  ```
  Nếu một lập trình viên vô tình đảo ngược vị trí của hai dòng này thành `UseAuthorization()` đứng trước, thì chỗ này không ổn ở điểm nào? Hệ thống sẽ sập hay người dùng sẽ luôn bị từ chối truy cập (403 Forbidden)? Hãy giải thích bản chất luồng đi của Request Pipeline.
- **Trả lời:**
  - **Authentication (Xác thực):** Là quá trình xác định danh tính của người dùng (Trả lời câu hỏi: *"Bạn là ai?"* thông qua việc kiểm tra Username/Password hoặc giải mã chữ ký JWT Token).
  - **Authorization (Phân quyền):** Là quá trình kiểm tra quyền hạn của người dùng đã được xác thực (Trả lời câu hỏi: *"Với danh tính này, bạn được phép làm những gì?"* thông qua kiểm tra Role hoặc Policy).
  - **Hậu quả khi đảo ngược thứ tự đăng ký Middleware:**
    - Người dùng sẽ **luôn bị từ chối truy cập (403 Forbidden hoặc 401 Unauthorized)** đối với tất cả các API yêu cầu bảo mật.
    - *Giải thích luồng Request Pipeline:* request đi qua `UseAuthorization()` trước. Do chưa chạy `UseAuthentication()`, hệ thống chưa hề giải mã JWT Token, thông tin người dùng (`HttpContext.User`) lúc này hoàn toàn trống rỗng (Anonymous). Khi check quyền, middleware phân quyền thấy user vô danh -> lập tức chặn lại và trả về lỗi từ chối truy cập, request không bao giờ đi tiếp được đến middleware xác thực ở sau.

### Câu 5: Bản chất kỹ thuật của JSON Web Token (JWT) và cơ chế mã hóa Signature
- **Nội dung:** JWT cấu tạo gồm 3 phần tách biệt bởi dấu chấm `.`: Header, Payload, và Signature. Hãy giải thích ý nghĩa nghiệp vụ của từng phần này. Phần nào chứa các thông tin công khai của User (Claims) và phần nào dùng để chống giả mạo dữ liệu?
- **Trọng tâm phản biện:** Chuỗi JWT hoàn toàn có thể bị hacker dùng các công cụ (như jwt.io) để giải mã ngược ra dạng TEXT để đọc thông tin Payload bên trong. Vậy tại sao JWT vẫn được coi là an toàn và hacker không thể tự ý sửa đổi số tiền hay vai trò (Role) của mình bên trong Token để hack hệ thống? Vai trò của Khóa bí mật (Secret Key) đặt trên Server lúc này là gì?
- **Trả lời:**
  - **Header:** Chứa thông tin về kiểu token (JWT) và thuật toán băm (ví dụ HS256).
  - **Payload:** Chứa thông tin công khai của User (Claims) như UserId, Username, Roles, thời gian hết hạn (`exp`). Đây là phần chứa các thông tin công khai.
  - **Signature:** Phần chữ ký số dùng để chống giả mạo dữ liệu. Nó được tạo ra bằng cách lấy (Header + Payload) mã hóa kết hợp với một Khóa bí mật (Secret Key) chỉ duy nhất Server được biết.
  - **Tại sao JWT an toàn dù bị đọc thông tin công khai:**
    - Hacker có thể đọc được Payload nhưng **không thể tự ý sửa đổi dữ liệu** (ví dụ sửa Role từ `Customer` thành `Admin`).
    - *Lý do:* Nếu hacker thay đổi bất kỳ ký tự nào ở Payload, khi gửi Token lên Server, Server sẽ lấy Payload bị sửa đổi đó kết hợp với Secret Key của Server để tự tính toán lại Signature mới. Khi đối chiếu Signature tự tính và Signature đính kèm trên Token thấy lệch nhau, Server biết ngay Token đã bị giả mạo và từ chối xử lý lập tức.
    - *Vai trò của Secret Key:* Là chốt chặn bảo mật tối thượng. Chỉ có Server giữ khóa này mới có thể ký hợp lệ cho Token.

### Câu 6: Phân quyền theo Vai trò (Role-based) vs Theo Chính sách (Policy-based)
- **Nội dung:** Hãy phân biệt cơ chế phân quyền dựa trên Vai trò (Role-based Authorization) và dựa trên Chính sách (Policy-based Authorization) trong ASP.NET Core.
- **Trọng tâm phản biện:** Giả sử hệ thống HushStore mở rộng nghiệp vụ: "Một Kỹ thuật viên (Technician) chỉ có quyền chỉnh sửa Phiếu sửa chữa (ServiceTicket) do chính họ được phân công, không được sửa phiếu của người khác". Nếu chỉ dùng phân quyền dạng `[Authorize(Roles = "Technician")]`, em có giải quyết được bài toán này không? Tại sao lúc này bắt buộc phải nâng cấp lên Phân quyền dựa trên thuộc tính (Resource-based / Policy-based Authorization)?
- **Trả lời:**
  - **Role-based Authorization:** Phân quyền tĩnh dựa trên vai trò của người dùng (ví dụ: User thuộc nhóm Admin thì được vào, thuộc nhóm Customer thì bị chặn). Rất đơn giản nhưng thiếu linh hoạt.
  - **Policy-based Authorization:** Phân quyền linh hoạt dựa trên các chính sách (Policies) phức tạp kết hợp nhiều điều kiện (ví dụ: tuổi lớn hơn 18, có vai trò Admin và sở hữu email công ty).
  - **Phản biện về bài toán Kỹ thuật viên sửa phiếu:**
    - Nếu chỉ dùng `[Authorize(Roles = "Technician")]`, ta **không thể giải quyết được bài toán này**. Vì thẻ này chỉ kiểm tra xem người dùng có phải là Technician hay không, chứ không thể kiểm tra mối quan hệ logic giữa người dùng hiện tại và dữ liệu phiếu sửa chữa (ai là người được phân công).
    - Lúc này bắt buộc phải dùng **Resource-based Authorization (Phân quyền dựa trên tài nguyên)** hoặc viết một Policy tùy chỉnh kế thừa `AuthorizationHandler<TRequirement, TResource>`. Khi đó, code handler sẽ nhận vào thông tin User đăng nhập và Object `ServiceTicket` truy vấn được từ DB lên để so sánh: `ticket.AssignedEmployeeId == currentUserId`. Nếu trùng khớp mới cho phép thực hiện chỉnh sửa.

### Câu 7: Lỗ hổng bảo mật Mass Assignment (Over-posting) và Khiên chắn DTO
- **Nội dung:** Lỗ hổng bảo mật Mass Assignment hoặc Over-posting ở tầng API là gì?
- **Trọng tâm phản biện:** Tại sao việc cho phép các Action Method ở Controller nhận trực tiếp các Class Entity Model từ tầng Core (ví dụ: `public IActionResult UpdateProduct(Product model)`) lại là một sai lầm chết người về bảo mật? Hãy giải thích cách kiến trúc Clean Architecture dùng DTO (Data Transfer Object) kết hợp với các bộ kiểm thử dữ liệu đầu vào (FluentValidation nằm ở tầng Shared) để bẻ gãy hoàn toàn lỗ hổng bảo mật này.
- **Trả lời:**
  - **Lỗ hổng Mass Assignment:** Xảy ra khi một kẻ tấn công lợi dụng việc Server tự động liên kết (bind) dữ liệu HTTP Request vào đối tượng Model để cố tình gửi thêm các trường dữ liệu nhạy cảm mà họ không được phép chỉnh sửa dưới Database.
  - **Tại sao nhận trực tiếp Entity là sai lầm chết người:**
    - Giả sử Entity `Product` có trường `IsDeleted` hoặc `CreatedBy`. Khi gọi API cập nhật sản phẩm, kẻ tấn công dùng công cụ (như Postman) đính kèm thêm `"IsDeleted": true` hoặc sửa ID người tạo gửi lên.
    - EF Core khi nhận Entity này từ Action Method sẽ tự động đánh dấu toàn bộ trạng thái Entity là Modified và lưu xuống DB, khiến kẻ tấn công xóa hoặc thay đổi thuộc tính nhạy cảm của sản phẩm một cách dễ dàng.
  - **Cách DTO và FluentValidation bẻ gãy lỗ hổng:**
    - DTO chỉ định nghĩa **chính xác các trường dữ liệu được phép truyền nhận** (ví dụ: `UpdateProductDto` chỉ chứa `Name`, `Price`, `Description`, hoàn toàn không chứa trường `IsDeleted` hay `CreatedBy`).
    - API chỉ liên kết dữ liệu Request vào DTO, sau đó dùng FluentValidation kiểm tra các giá trị đó có hợp lệ không.
    - Cuối cùng, Service chỉ lấy dữ liệu an toàn từ DTO ánh xạ (map) sang Entity. Mọi trường nhạy cảm không nằm trong DTO sẽ hoàn toàn bị bỏ qua, triệt tiêu tận gốc lỗ hổng Mass Assignment.

### Câu 8: Chiến lược quản lý Token - Tại sao cần đẻ ra cặp Access Token ngắn hạn và Refresh Token dài hạn?
- **Nội dung:** Tại sao trong các hệ thống thực tế như dự án của chúng ta, người ta không cấp một mã Access Token có thời hạn sống dài (ví dụ: 1 năm) cho tiện sử dụng, mà lại bắt buộc phải cài đặt cơ chế: Access Token chỉ sống 15-30 phút, đi kèm một Refresh Token sống vài tuần lưu trong Database?
- **Trọng tâm phản biện:** Cơ chế này giải quyết bài toán gì khi một người dùng bị lộ máy tính hoặc tài khoản bị Admin ép buộc đăng xuất từ xa (Revoke Token)? Nếu hacker ăn trộm được Access Token ngắn hạn, thiệt hại của hệ thống được giới hạn ra sao?
- **Trả lời:**
  - **Lý do cần cặp Access Token ngắn hạn và Refresh Token dài hạn:**
    - JWT Access Token bản chất là Stateless (không lưu trạng thái ở Server). Một khi đã được cấp, Server không có cách nào thu hồi/hủy bỏ hiệu lực của token đó một cách trực tiếp cho đến khi tự nó hết hạn.
    - Nếu cấp Access Token sống 1 năm, khi token bị hacker đánh cắp, hacker sẽ có toàn quyền truy cập hệ thống suốt 1 năm đó mà Server hoàn toàn bất lực.
  - **Giải quyết bài toán khi lộ máy tính hoặc bị Admin khóa/đăng xuất từ xa:**
    - Access Token chỉ cho sống rất ngắn (ví dụ 15 phút) để nếu có bị lộ, hacker cũng chỉ lợi dụng được tối đa 15 phút đó.
    - Khi hết 15 phút, Client buộc phải gửi **Refresh Token** lên API để xin cấp Access Token mới.
    - Khác với Access Token, **Refresh Token được lưu ở Database Server**. Khi người dùng báo mất máy hoặc Admin thực hiện khóa tài khoản/đăng xuất từ xa, Server sẽ lập tức xóa hoặc đánh dấu hủy bỏ (`IsUsed`, `IsRevoked`) bản ghi Refresh Token đó dưới DB.
    - Lần tiếp theo Client của hacker gửi Refresh Token lên, Server đối chiếu thấy đã bị vô hiệu hóa -> Từ chối cấp token mới, đá hacker ra khỏi hệ thống lập tức.

### Câu 9: Cơ chế lội ngược dòng của Middleware Pipeline và vị trí đặt các chốt chặn Bảo mật
- **Nội dung:** Hãy giải thích kiến trúc Request-Response Pipeline (bản chất các Middleware gọi nhau qua hàm `next()`) trong ASP.NET Core hoạt động như thế nào.
- **Trọng tâm phản biện:** Tại sao các cấu hình chốt chặn như CORS (Chia sẻ tài nguyên cấu hình tên miền), Rate Limiting (Giới hạn lượt gọi API tránh DDoS), và Authentication Middleware luôn được ưu tiên đặt ở ngay đầu file `Program.cs`? Việc chặn đứng các Request không hợp lệ từ sớm (Short-circuiting the pipeline) giúp ích gì cho việc tối ưu tài nguyên của máy chủ và Database ở vòng trong?
- **Trả lời:**
  - **Cơ chế hoạt động của Middleware Pipeline:**
    - Request đi qua một chuỗi các Middleware theo thứ tự đăng ký trong `Program.cs`. Mỗi middleware thực hiện logic của mình, sau đó gọi `await next(context)` để chuyển tiếp Request cho middleware tiếp theo.
    - Khi Request đi đến cuối Pipeline (Controller xử lý xong), phản hồi (Response) sẽ đi ngược dòng trở lại qua các Middleware theo thứ tự đảo ngược trước khi gửi về cho Client.
  - **Tại sao ưu tiên đặt CORS, Rate Limiting, Authentication ở đầu:**
    - Để thực hiện cơ chế **Ngắt mạch sớm (Short-circuiting the pipeline)**.
    - *Lợi ích tối ưu tài nguyên:* Nếu request vi phạm cấu hình CORS (gọi từ nguồn lạ), vượt quá tần suất cho phép (Rate Limit), hoặc gửi Token giả mạo (Authentication), các middleware đầu tiên này sẽ lập tức xử lý, ghi lỗi và trả về HTTP Response luôn mà **không gọi hàm `next()`**.
    - Việc chặn đứng từ sớm ngăn không cho request đi sâu vào các middleware khởi tạo nặng ở phía sau (như kết nối DbContext, gọi Service, thực thi SQL query), giúp bảo vệ máy chủ khỏi nguy cơ quá tải CPU/RAM và giảm thiểu rủi ro bị tấn công DDoS hiệu quả.

### Câu 10: Chuẩn hóa Xử lý lỗi hệ thống qua chuẩn RFC 7807 (Problem Details)
- **Nội dung:** Trong file `src/API/Program.cs`, em đã cấu hình một khối lệnh `app.UseExceptionHandler(...)` rất chi tiết để bắt tập trung các lỗi như `NotFoundException`, `BusinessRuleException`.
- **Trọng tâm phản biện:** Tại sao chúng ta không đặt các khối lệnh `try-catch` trực tiếp ở từng hàm trong Controller để xử lý lỗi tại chỗ cho tường minh, mà lại phải đẩy lỗi ra một Middleware xử lý lỗi tập trung ở vòng ngoài cùng? Việc trả về một định dạng lỗi cấu trúc đồng nhất (chuẩn Problem Details) mang lại lợi ích gì cho việc lập trình giao diện Frontend bên phía Blazor WebAssembly?
- **Trả lời:**
  - **Tại sao dùng Middleware xử lý lỗi tập trung thay vì try-catch từng hàm:**
    1. **Tránh trùng lặp code (DRY):** Nếu dùng try-catch ở từng hàm, code Controller sẽ tràn ngập các khối try-catch giống hệt nhau, gây loãng code hiển thị.
    2. **Đảm bảo không rò rỉ lỗi hệ thống:** Nếu lập trình viên quên viết try-catch ở một hàm mới, khi sập nguồn DB, lỗi hệ thống chi tiết (như SQL Connection string, Stack Trace) sẽ bị phơi bày ra giao diện UI -> Lỗ hổng bảo mật nghiêm trọng. Middleware tập trung đảm bảo mọi lỗi chưa được bắt đều được xử lý an toàn.
  - **Lợi ích của chuẩn hóa lỗi RFC 7807 (Problem Details) cho Blazor WASM:**
    - Cung cấp một cấu trúc JSON báo lỗi thống nhất toàn hệ thống (ví dụ gồm các trường: `title`, `status`, `detail`, `errors`).
    - Phía Client Blazor WASM chỉ cần viết duy nhất một bộ xử lý lỗi chung (Global HTTP Interceptor). Bộ này tự động bóc tách dữ liệu lỗi theo cùng một cấu trúc để hiển thị thông báo đỏ (Toast/Alert) hoặc hiển thị lỗi Validation ngay cạnh các ô Input của Form một cách đồng bộ và tự động.

---

*Học bằng cách vẽ sơ đồ Pipeline: Bảo bạn ấy lấy giấy bút ra, vẽ luồng đi của 1 HTTP Request đi xuyên qua các lớp màng lọc: Rate Limiting -> CORS -> Authentication -> UserStatusMiddleware -> Authorization -> Controller -> Service -> DB. Khi bạn ấy tự tay vẽ được đường đi này và giải thích được chuyện gì xảy ra ở từng nút thắt, bạn ấy sẽ không sợ bất kỳ câu hỏi ép sườn nào của hội đồng nữa.*

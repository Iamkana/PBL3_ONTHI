# Kiến trúc hệ thống

## Mảng 1: Bức tranh toàn cảnh (Client - Server)
* **Mục tiêu**: Hiểu tại sao dự án lại bị cắt làm đôi (Frontend riêng, Backend riêng) thay vì code chung một cục.
* **Câu hỏi & Trả lời**:
  * **Hỏi**: "Giải thích cho tôi khái niệm Mô hình Client - Server trong lập trình web là gì? Dùng một ví dụ thực tế trong đời sống (như đi ăn nhà hàng) để minh họa nhé."
    * **Trả lời**: 
      * **Mô hình Client - Server** là mô hình mạng máy tính chia nhiệm vụ làm 2 phần: **Client (Khách)** là thiết bị/ứng dụng gửi yêu cầu (Request), và **Server (Chủ)** là máy chủ tiếp nhận, xử lý yêu cầu đó và trả kết quả về (Response).
      * **Ví dụ thực tế**: Quy trình đi ăn ở nhà hàng.
        * **Client (Khách hàng)**: Là bạn. Bạn xem thực đơn, chọn món và gọi món. Bạn không tự xuống bếp nấu ăn.
        * **Server (Nhà bếp)**: Nơi chứa nguyên liệu, đầu bếp nấu nướng và chế biến các món ăn theo yêu cầu.
        * **API/Mạng truyền dẫn (Người phục vụ)**: Nhận order từ bạn mang vào bếp, sau đó bưng món ăn từ bếp ra bàn cho bạn.
  * **Hỏi**: "Đồ án của tôi dùng Blazor WebAssembly làm Frontend (Client) và ASP.NET Core API làm Backend (Server). Phân biệt vai trò của 2 thằng này giúp tôi. Đứa nào giữ dữ liệu, đứa nào chỉ để hiển thị?"
    * **Trả lời**:
      * **Blazor WebAssembly (Client - Giao diện)**: Chỉ dùng để **hiển thị (Presentation)** và tương tác với người dùng. Mã C# được biên dịch sang WebAssembly chạy trực tiếp trên trình duyệt của client. Nó chỉ lưu tạm dữ liệu hiển thị (ví dụ: danh sách sản phẩm lấy từ API) hoặc Token đăng nhập ở LocalStorage, không giữ dữ liệu gốc.
      * **ASP.NET Core Web API (Server - Logic & Dữ liệu)**: Là đứa **nắm giữ dữ liệu gốc**. Nó trực tiếp tương tác với Database (SQL Server qua EF Core `HushStoreDbContext`). Server chịu trách nhiệm thực thi các quy tắc nghiệp vụ (Business Rules), bảo mật, phân quyền và đảm bảo tính toàn vẹn dữ liệu.
  * **Hỏi**: "Tại sao người ta không code chung giao diện và logic vào một cục (như mô hình MVC ngày xưa) mà lại tách ra API riêng và Frontend riêng? Việc tách ra như vậy mang lại những lợi ích gì cho dự án thực tế?"
    * **Trả lời**:
      * **Tách biệt mối quan tâm (Separation of Concerns)**: Giúp Frontend (Blazor) chỉ tập trung tối ưu trải nghiệm giao diện (UX), còn Backend (ASP.NET API) chỉ tập trung tối ưu hiệu năng, bảo mật và logic nghiệp vụ.
      * **Tái sử dụng API (Reusability)**: Backend API là duy nhất nhưng có thể phục vụ cho nhiều Client khác nhau: Web app (Blazor WASM), Mobile App (iOS/Android), hoặc các dịch vụ bên thứ ba (như đơn vị vận chuyển, cổng thanh toán) mà không cần code lại logic.
      * **Tối ưu hóa tài nguyên & Giảm tải cho Server**: Server không phải làm nhiệm vụ render HTML gửi về (Server-Side Rendering). CPU của trình duyệt Client tự xử lý phần render giao diện, giúp Server chịu tải được nhiều lượt truy cập hơn.
      * **Phát triển độc lập**: Đội ngũ phát triển Frontend và Backend có thể làm việc song song, kiểm thử và deploy (triển khai) độc lập mà không sợ ảnh hưởng lẫn nhau.
  * **Hỏi**: "Làm sao cái giao diện Blazor (Client) của tôi lại lấy được dữ liệu từ cái ASP.NET API (Server)? Bọn chúng 'nói chuyện' với nhau qua cái gì? (Giải thích giúp tôi khái niệm HTTP Request, GET, POST, PUT, DELETE)."
    * **Trả lời**:
      * Blazor WASM gửi các **HTTP Request** thông qua class `HttpClient` đến các đầu endpoint của ASP.NET API qua mạng Internet, và API trả về dữ liệu dưới định dạng **JSON (HTTP Response)**.
      * **Các HTTP Methods (Hành động)**:
        * **GET**: Client yêu cầu lấy dữ liệu (Ví dụ: `GetFromJsonAsync<ApiResult<ProductDetailDto>>("api/products/1")` để đọc chi tiết sản phẩm 1).
        * **POST**: Client gửi dữ liệu lên để tạo mới (Ví dụ: gửi thông tin đơn hàng mới lên server để lưu vào database).
        * **PUT**: Client gửi dữ liệu lên để cập nhật bản ghi cũ (Ví dụ: cập nhật profile khách hàng).
        * **DELETE**: Client yêu cầu xóa dữ liệu (Trong dự án thường là Soft Delete - đổi cờ `IsDeleted = true`).

---

## Mảng 2: "Tiêm" phụ thuộc - Dependency Injection (DI)
* **Mục tiêu**: Hiểu được từ khóa DI và tại sao `Program.cs` lại có một đống `AddScoped`, `AddTransient`.
* **Câu hỏi & Trả lời**:
  * **Hỏi**: 5. "Trong lập trình, 'Dependency' (sự phụ thuộc) nghĩa là gì? Nếu class A gọi thẳng từ khóa `new ClassB()` ở bên trong nó thì có vấn đề gì xảy ra khi dự án phình to?"
    * **Trả lời**:
      * **Dependency**: Là khi Class A cần sử dụng Class B để hoàn thành công việc của nó (Class A phụ thuộc vào Class B).
      * **Vấn đề khi dùng `new ClassB()` trực tiếp**:
        * **Khóa cứng (Tight Coupling)**: Nếu Class B thay đổi constructor (ví dụ: thêm tham số kết nối DB), ta phải tìm và sửa tất cả các file có dòng `new ClassB()`.
        * **Không thể viết Unit Test**: Ta không thể cô lập Class A để kiểm thử độc lập, buộc phải chạy thật cả Class B (ví dụ Class B tương tác với DB thật).
        * **Khó mở rộng**: Nếu muốn đổi sang dùng `ClassC` thay thế `ClassB`, ta phải sửa đổi trực tiếp code bên trong Class A.
  * **Hỏi**: 6. "Dependency Injection (DI) là gì? Thay vì dùng từ khóa `new`, DI giải quyết vấn đề phụ thuộc ở câu trên như thế nào? Dùng ví dụ dễ hiểu nhất nhé."
    * **Trả lời**:
      * **DI (Tiêm phụ thuộc)** là một design phần mềm nhằm đảo ngược việc tạo các phụ thuộc. Thay vì class tự khởi tạo các phụ thuộc bằng từ khóa `new`, DI container của framework (ví dụ .NET Core) sẽ quản lý việc khởi tạo đó và tự động "tiêm" (inject) đối tượng đó vào qua Constructor.
      * **Ví dụ**: Bạn cần xe máy đi làm.
        * *Dùng `new`*: Bạn phải tự xây dựng nhà máy sản xuất xe máy, đổ xăng, bảo dưỡng xe (tự `new`).
        * *Dùng DI*: Bạn đăng ký dịch vụ: "Tôi cần xe máy đi làm" (Khai báo Interface trong constructor). Framework DI đóng vai trò là dịch vụ cung cấp xe máy, tự đem xe đến cửa cho bạn đi. Khi bạn muốn đổi sang xe điện, bạn chỉ cần báo dịch vụ cấp xe điện mà không cần thay đổi cách lái xe của bạn.
  * **Hỏi**: 7. "Trong file `Program.cs` ở đồ án của tôi (viết bằng .NET), tôi thấy có đăng ký DI bằng các hàm: `AddTransient`, `AddScoped`, `AddSingleton`. Phân biệt vòng đời (lifetime) của 3 thằng này giúp tôi."
    * **Trả lời**:
      * **Transient (`AddTransient`)**: Tạo một instance mới **mỗi lần** được yêu cầu. Thích hợp cho các class nhẹ, không lưu trạng thái (state-less).
      * **Scoped (`AddScoped`)**: Tạo một instance duy nhất cho **mỗi Request HTTP** (mỗi kết nối từ client). Trong suốt request đó, mọi chỗ gọi class này đều dùng chung 1 instance. Request kết thúc thì instance bị hủy. Ví dụ: Repositories và `HushStoreDbContext` được đăng ký Scoped để đảm bảo an toàn giao dịch trên mỗi request của user.
      * **Singleton (`AddSingleton`)**: Chỉ tạo duy nhất một instance cho **toàn bộ vòng đời ứng dụng**. Tất cả mọi người, mọi request đều dùng chung một instance này. Thích hợp cho Caching tĩnh, Configuration, hoặc Logger.
  * **Hỏi**: 8. "Riêng trong Blazor WebAssembly (chạy hoàn toàn trên trình duyệt), hàm `AddScoped` hoạt động như thế nào? Nó có giống với `AddScoped` trên server API không, hay nó giống Singleton hơn?"
    * **Trả lời**:
      * Trong **Blazor WebAssembly**, ứng dụng chạy hoàn toàn trên Single Thread của trình duyệt và không có khái niệm Request HTTP vòng đời giống như API Server.
      * Do đó, **`AddScoped` trong Blazor WASM hoạt động tương tự như `AddSingleton`**. Đối tượng được tạo ra sẽ tồn tại mãi trong suốt thời gian sống của tab trình duyệt hiện tại. Nó chỉ bị giải phóng khi người dùng tắt tab hoặc nhấn F5 để tải lại trang.

---

## Mảng 3: Gom nhóm dữ liệu - Repository Pattern
* **Mục tiêu**: Hiểu tại sao lại đẻ ra cái tầng Infrastructure và các file Repository thay vì gọi thẳng Database.
* **Câu hỏi & Trả lời**:
  * **Hỏi**: 9. "Trong lập trình, 'Repository Pattern' là gì? Tại sao tôi không viết các câu lệnh truy vấn Database (như LINQ của EF Core) trực tiếp bên trong Controller/Service luôn cho nhanh?"
    * **Trả lời**:
      * **Repository Pattern** là lớp trung gian nằm giữa Business Logic (Service) và Data Access (DbContext/Database). Nó bọc toàn bộ mã nguồn truy vấn dữ liệu thô (như SQL, LINQ) lại thành các hàm nghiệp vụ rõ ràng (ví dụ: `GetByIdAsync`, `GetPagedListAsync`).
      * **Tại sao không viết trực tiếp**:
        * **Tránh trùng lặp code (DRY)**: Nếu viết trực tiếp, câu query lấy sản phẩm chưa bị xóa và còn hàng sẽ phải viết đi viết lại ở nhiều controller.
        * **Dễ bảo trì**: Nếu DB đổi tên cột, ta chỉ cần vào duy nhất 1 file Repository sửa lại câu query thay vì đi dò hàng chục Service để sửa.
        * **Tách biệt nhiệm vụ**: Service chỉ cần tập trung xử lý logic bán hàng, không cần quan tâm dữ liệu được lấy từ EF Core SQL Server hay từ nơi nào khác.
  * **Hỏi**: 10. "Viết Repository Pattern mang lại lợi ích gì về mặt tái sử dụng code (Reusability) và kiểm thử (Testing)?"
    * **Trả lời**:
      * **Reusability (Tái sử dụng)**: Các câu query phức tạp (ví dụ: phân trang sản phẩm kèm include ảnh và thương hiệu) được viết tập trung trong `ProductRepository.cs`. Bất kỳ Service nào cần cũng chỉ cần gọi hàm `GetPagedListAsync` là có dữ liệu.
      * **Testing (Kiểm thử)**: Khi viết Unit Test cho Service, ta có thể dễ dàng tạo ra một Repository giả lập (Mock) thông qua Interface (ví dụ: trả về danh sách cứng trong RAM) mà không cần kết nối thật đến Database SQL Server, giúp test chạy cực nhanh và tin cậy.
  * **Hỏi**: 11. "Trong đồ án của tôi có file `ProductRepository.cs` và interface `IProductRepository.cs`. Việc tách ra Interface (chữ I) rồi mới đến class thực thi (Implementation) có ý nghĩa gì liên quan đến Dependency Injection (DI)?"
    * **Trả lời**:
      * Giúp thực thi nguyên lý **Dependency Inversion** (D trong SOLID): Tầng nghiệp vụ không phụ thuộc trực tiếp vào class cụ thể `ProductRepository.cs` (liên quan EF Core) mà phụ thuộc vào interface trừu tượng `IProductRepository.cs`.
      * Khi đăng ký DI: `builder.Services.AddScoped<IProductRepository, ProductRepository>();`. Nếu tương lai ta chuyển từ SQL Server sang MongoDB, ta chỉ cần viết class mới `ProductMongoRepository` kế thừa `IProductRepository` rồi sửa cấu hình ở `Program.cs` mà không phải sửa bất kỳ dòng code nào ở tầng Service.

---

## Mảng 4: Toàn vẹn dữ liệu - Unit of Work Pattern
* **Mục tiêu**: Hiểu cách quản lý Transaction (giao dịch) khi cập nhật nhiều bảng cùng lúc.
* **Câu hỏi & Trả lời**:
  * **Hỏi**: 12. "Unit of Work Pattern là gì? Nó thường đi cặp với Repository Pattern để giải quyết vấn đề gì?"
    * **Trả lời**:
      * **Unit of Work (UoW)** là design pattern quản lý một Transaction duy nhất xuyên suốt nhiều Repository.
      * **Vấn đề giải quyết**: Đảm bảo tất cả các thay đổi trên nhiều bảng khác nhau (thông qua các Repository khác nhau) đều được lưu xuống Database thành công cùng lúc (Commit), hoặc nếu có một thao tác bị lỗi thì toàn bộ thay đổi trước đó sẽ bị hủy bỏ hoàn toàn (Rollback). Tránh tình trạng lưu dữ liệu nửa chừng gây sai lệch.
  * **Hỏi**: 13. "Hãy giải thích khái niệm 'Transaction' (Giao dịch) trong cơ sở dữ liệu. Nếu tôi có một nghiệp vụ: Vừa lưu Đơn hàng, vừa trừ Tồn kho, vừa xóa Giỏ hàng. Nếu xóa giỏ hàng bị lỗi rớt mạng thì Transaction giúp tôi xử lý ra sao?"
    * **Trả lời**:
      * **Transaction (Giao dịch)** là tập hợp các thao tác DB phải được thực thi theo nguyên tắc "Tất cả hoặc không có gì" (Atomicity).
      * **Xử lý khi lỗi xóa giỏ hàng**:
        * Khi bắt đầu nghiệp vụ, UoW sẽ mở một transaction: `await _unitOfWork.BeginTransactionAsync();`.
        * Hệ thống thực hiện: (1) Lưu đơn hàng, (2) Trừ tồn kho.
        * Đến bước (3) Xóa giỏ hàng bị lỗi mạng văng Exception.
        * Khối `catch` của Service bắt được lỗi sẽ gọi `await _unitOfWork.RollbackAsync();`.
        * SQL Server lập tức hủy bỏ dữ liệu đơn hàng và tồn kho đã ghi tạm ở bước 1 và 2, trả DB về trạng thái sạch sẽ ban đầu như chưa có chuyện gì xảy ra.
  * **Hỏi**: 14. "Trong code C# Entity Framework Core, hàm `_context.SaveChangesAsync()` đóng vai trò gì trong mô hình Unit of Work? Tại sao người ta lại gom các thay đổi từ nhiều Repository lại rồi mới gọi hàm `SaveChanges()` một lần duy nhất?"
    * **Trả lời**:
      * Hàm `_context.SaveChangesAsync()` quét qua toàn bộ thực thể đang được EF Core theo dõi trạng thái, dịch chúng thành các câu lệnh SQL INSERT, UPDATE, DELETE tương ứng và gửi đi trong một lô (batch) xuống DB.
      * **Tại sao gom lại gọi 1 lần**:
        * **Hiệu năng**: Giảm số lượng kết nối mạng gửi qua lại (round-trips) giữa Web Server và Database Server.
        * **Tính nhất quán**: EF Core tự động bọc các thay đổi trong `SaveChangesAsync()` vào một transaction nội bộ. Nếu có một lệnh SQL trong lô bị lỗi, toàn bộ các lệnh còn lại sẽ tự động bị rollback.

---

## Mảng 1: Kiến trúc tổng thể (Decoupled Architecture)
* **Kiến trúc Decoupled giữa Blazor WASM và ASP.NET API**:
  * **Hỏi**: "Giải thích cho tôi kiến trúc Decoupled (tách rời) giữa Frontend (Blazor WebAssembly) và Backend (ASP.NET Core Web API). Nếu so với mô hình MVC truyền thống (giao diện và logic dính chùm) thì việc tách rời này giải quyết được bài toán gì trong thực tế?"
    * **Trả lời**:
      * **Kiến trúc Decoupled** tách biệt hoàn toàn ứng dụng thành 2 dự án chạy độc lập: Frontend (chạy trên trình duyệt client) và Backend API (chạy trên Web Server), giao tiếp với nhau bằng JSON qua HTTP.
      * **So với MVC truyền thống**:
        * **Giải phóng Server**: Server không cần render giao diện HTML (Server-side rendering), giúp giảm tải CPU/RAM của server.
        * **Tăng trải nghiệm (UX)**: Chuyển trang mượt mà lập tức (Single Page Application) mà không bị tải lại toàn bộ trang (no full page reload).
        * **Hỗ trợ đa nền tảng**: Backend API viết 1 lần có thể dùng chung cho cả Web App, Mobile App (iOS/Android), IoT...
  * **Hỏi**: "Nhược điểm hay sự đánh đổi (trade-off) của việc tách rời Frontend và Backend là gì? Có phải lúc nào cũng nên tách ra không?"
    * **Trả lời**:
      * **Nhược điểm**:
        * **SEO kém**: Vì nội dung trang web được render động bằng JavaScript/WebAssembly ở Client, các bot tìm kiếm đời cũ khó thu thập dữ liệu (cần cấu hình Server-Side Prerendering để khắc phục).
        * **Tải lần đầu chậm (First Load)**: Client phải download file runtime WebAssembly (.wasm) và các file .dll của client về máy trước khi chạy.
        * **Phức tạp hơn**: Phải cấu hình CORS, xác thực bảo mật stateless (JWT), xử lý đồng bộ dữ liệu và bắt lỗi kết nối mạng.
      * **Khi nào không tách**: Nếu làm trang Web tin tức cần SEO cực cao, hoặc trang quản trị đơn giản ít tương tác động thì nên dùng MVC hoặc Razor Pages để tiết kiệm thời gian phát triển.
  * **Hỏi**: "Trong kiến trúc dự án của tôi có 1 project tên là `Shared` (chứa DTO, Enum, Models dùng chung). Project `Shared` này đóng vai trò gì trong việc liên kết giữa Frontend và Backend? Nếu không có nó thì code sẽ bị lặp lại như thế nào?"
    * **Trả lời**:
      * **Vai trò**: `Shared` là cầu nối chứa các định nghĩa dữ liệu dùng chung (như DTOs, Enums, Validation Rules). Cả dự án Client và API đều tham chiếu đến `Shared`.
      * **Nếu không có**: Ta sẽ bị lặp code. Ví dụ: khi định nghĩa class `ProductDetailDto`, ta phải viết 1 lần bằng C# ở Backend API và tự copy viết lại 1 lần nữa ở Client. Nếu thay đổi thuộc tính, ta phải sửa thủ công ở cả 2 bên, rất dễ xảy ra lỗi không khớp kiểu dữ liệu khi truyền nhận JSON.

---

## Mảng 2: Nguyên lý IoC (Đảo ngược điều khiển) & DTO
* **Mục tiêu**: Hiểu tại sao code không đi đường thẳng mà cứ phải "đi vòng" qua Interface và DTO.
* **Câu hỏi & Trả lời**:
  * **Hỏi**: 53. "DTO (Data Transfer Object) pattern là gì? Tại sao tôi không lấy thẳng thực thể (Entity) trong Database như bảng Product, User để ném thẳng xuống giao diện Client luôn cho lẹ, mà phải tạo ra các class DTO như `ProductDto`, rồi mất công map (chuyển đổi) qua lại?"
    * **Trả lời**:
      * **DTO** là đối tượng chỉ chứa dữ liệu thô phục vụ truyền tải qua mạng mà không chứa bất cứ logic nghiệp vụ nào.
      * **Tại sao không ném thẳng Entity**:
        * **Bảo mật**: Entity khớp 1-1 với Database, chứa dữ liệu nhạy cảm (như mật khẩu băm của User, trường kiểm toán `CreatedBy`, `IsDeleted`). Ném trực tiếp Entity xuống Client sẽ bị lộ cấu trúc DB và rò rỉ dữ liệu.
        * **Tránh lỗi tuần hoàn (Circular Reference)**: Entity có các mối quan hệ (như Product chứa Category, Category lại chứa danh sách Products). Khi serialize sang JSON sẽ gây lỗi lặp vô hạn.
        * **Tối ưu băng thông**: Entity chứa nhiều thông tin không cần hiển thị. DTO giúp ta tinh gọn dữ liệu, chỉ gửi những gì client thực sự cần.
  * **Hỏi**: 54. "Nguyên lý Đảo ngược điều khiển (Inversion of Control - IoC) trong thiết kế phần mềm là gì? Hãy giải thích bằng ví dụ dễ hiểu nhất đời thường."
    * **Trả lời**:
      * **IoC** là triết lý thiết kế đảo ngược quyền kiểm soát luồng chạy của ứng dụng. Thay vì code của bạn tự kiểm soát tất cả mọi thứ và tự khởi tạo các phụ thuộc, một framework bên ngoài sẽ nắm quyền đó và gọi code của bạn khi cần.
      * **Ví dụ đời thường**: Lái xe taxi.
        * *Lập trình truyền thống*: Bạn tự mua xe, tự đổ xăng, tự lái xe đi làm (bạn kiểm soát tất cả).
        * *Lập trình IoC*: Bạn gọi xe công nghệ Grab. Bạn chỉ việc lên xe, đưa địa chỉ đến (khai báo mục tiêu). Tài xế Grab và hệ thống định vị của họ sẽ tự lái xe đưa bạn đi (hệ thống nắm quyền kiểm soát luồng đi).
  * **Hỏi**: 55. "Tại sao trong code, người ta luôn tiêm (inject) Interface (ví dụ: `IProductRepository`) thay vì tiêm thẳng Class thực thi (`ProductRepository`)? Làm vậy để giải quyết vấn đề gì khi dự án cần nâng cấp, bảo trì hoặc viết Unit Test?"
    * **Trả lời**:
      * **Loose Coupling (Khớp nối lỏng)**: Giúp tầng Service không bị phụ thuộc vào cách cài đặt cụ thể của Repository (dùng EF Core SQL Server).
      * **Bảo trì, nâng cấp**: Dễ dàng đổi class thực thi sang công nghệ khác (như MongoDB, Dapper) mà không cần chỉnh sửa code của tầng Service.
      * **Viết Unit Test**: Ta có thể tạo một mock repository triển khai `IProductRepository` trả về dữ liệu ảo trong RAM để test logic của Service mà không cần đụng đến Database thật, giúp test chạy độc lập và cực nhanh.

---

## Mảng 3: Dependency Injection (DI) thực chiến
* **Mục tiêu**: Cần hiểu kỹ DI để tránh lỗi rò rỉ bộ nhớ, sai lệch dữ liệu.
* **Câu hỏi & Trả lời**:
  * **Hỏi**: 62. "Dependency Injection (DI) là gì? Nó giúp thực thi nguyên lý IoC ở câu trên như thế nào?"
    * **Trả lời**:
      * **DI** là kỹ thuật cung cấp các đối tượng phụ thuộc cho một class từ bên ngoài. DI là công cụ hiện thực hóa triết lý IoC. Thay vì class tự khởi tạo phụ thuộc, ta khai báo chúng ở Constructor, và DI Container của .NET sẽ chịu trách nhiệm tìm kiếm, khởi tạo và truyền instance vào cho class hoạt động.
  * **Hỏi**: 63. "Trong .NET Core, khi đăng ký DI có 3 vòng đời (lifetime) chính: `AddTransient`, `AddScoped`, và `AddSingleton`. Hãy phân biệt sự khác nhau của 3 thằng này bằng ví dụ thực tế (ví dụ như gọi nước ở quán cafe)."
    * **Trả lời**:
      * **Transient (`AddTransient`) - Chiếc cốc giấy**: Mỗi lần bạn gọi nước, quán sẽ lấy một chiếc cốc giấy mới hoàn toàn. Uống xong vứt đi, lần sau lại lấy cốc mới.
      * **Scoped (`AddScoped`) - Bình trà chung**: Mỗi bàn khách (mỗi HTTP Request) được phục vụ một bình trà. Cả bàn rót uống chung từ bình đó. Khi bàn đó về và bàn mới tới, quán sẽ dọn bình cũ đi và cấp một bình trà mới.
      * **Singleton (`AddSingleton`) - Máy pha cafe của quán**: Cả quán chỉ có duy nhất một chiếc máy pha cafe. Mọi bàn khách, mọi lượt gọi nước đều dùng chung một chiếc máy pha cafe duy nhất này từ sáng đến tối.
  * **Hỏi**: 64. "Frontend của tôi dùng Blazor WebAssembly (chạy trên trình duyệt). Việc dùng `AddScoped` trong Blazor WebAssembly có gì khác biệt so với `AddScoped` ở Backend API? (Gợi ý: Liên quan đến vòng đời của 1 tab trình duyệt)."
    * **Trả lời**:
      * Ở **Backend API**: Một scope bắt đầu khi có request HTTP đến và kết thúc khi trả response về. Có hàng ngàn request đồng thời tương ứng với hàng ngàn scope.
      * Ở **Blazor WASM**: Do ứng dụng chạy local trên trình duyệt của 1 user (SPA), nên một scope sẽ kéo dài suốt toàn bộ thời gian sống của tab trình duyệt đó. Vì thế, **`AddScoped` ở Blazor WASM hoạt động như `AddSingleton`**, dữ liệu được lưu trong class scoped sẽ được giữ lại suốt phiên làm việc cho đến khi user reload trang (F5) hoặc đóng tab.

---

## Mảng 4: Tầng Data Access (Repository & Unit of Work)
* **Mục tiêu**: Hiểu cách quản lý và thao tác với Database một cách chuyên nghiệp, an toàn.
* **Câu hỏi & Trả lời**:
  * **Hỏi**: 71. "Repository Pattern là gì? Thay vì tôi viết thẳng các lệnh LINQ (của Entity Framework) vào trong Controller để gọi Database cho lẹ, tại sao tôi phải tốn công tạo thêm tầng Repository để bọc chúng lại?"
    * **Trả lời**:
      * Repository Pattern bọc các truy vấn EF Core vào các hàm có tên nghiệp vụ cụ thể.
      * **Tại sao không viết trực tiếp vào Controller/Service**:
        * **Tránh trùng lặp code**: Một câu query lọc sản phẩm có thể được dùng ở trang chủ, trang quản lý kho, trang chi tiết. Nếu DB đổi cấu trúc, ta chỉ cần sửa ở 1 nơi (`ProductRepository.cs`).
        * **Bảo vệ tính đóng gói**: Giúp tầng logic nghiệp vụ hoàn toàn độc lập với công nghệ truy cập dữ liệu (EF Core).
  * **Hỏi**: 72. "Unit of Work (UoW) Pattern là gì? Tại sao nó thường luôn đi cặp với Repository Pattern?"
    * **Trả lời**:
      * **Unit of Work** là lớp quản lý transaction cho DbContext.
      * **Tại sao đi cặp**: Vì Repository xử lý riêng lẻ từng thực thể. Khi một nghiệp vụ cần thao tác trên nhiều Repository khác nhau, UoW sẽ đứng ra dùng chung 1 `DbContext` duy nhất để đảm bảo tất cả thay đổi được lưu đồng thời trong cùng 1 Transaction, tránh lỗi mất nhất quán dữ liệu.
  * **Hỏi**: 73. "Giải thích khái niệm 'Transaction' (Giao dịch) thông qua Unit of Work. Ví dụ: Tôi có 3 nghiệp vụ cần làm cùng lúc: (1) Lưu đơn hàng, (2) Trừ tồn kho, (3) Xóa giỏ hàng. Nếu đang làm đến bước 3 thì bị lỗi văng exception, Unit of Work sẽ làm gì để cứu dữ liệu không bị sai lệch (Rollback)?"
    * **Trả lời**:
      * Giao dịch được UoW mở ra qua `BeginTransactionAsync()`.
      * Khi bước 3 (Xóa giỏ hàng) bị lỗi, Exception được ném ra. Hệ thống nhảy vào khối `catch` và thực thi `await _unitOfWork.RollbackAsync();`.
      * SQL Server sẽ hủy toàn bộ các thay đổi tạm thời của bước 1 (Lưu đơn hàng) và bước 2 (Trừ kho) trên DB. Dữ liệu quay lại trạng thái ban đầu, ngăn chặn hoàn toàn lỗi logic (có đơn hàng nhưng không trừ kho hoặc ngược lại).

---

## Mảng 5: Các quyết định thiết kế (Architectural Decisions)
* **Mục tiêu**: Hiểu các pattern ngầm định trong việc xử lý nghiệp vụ.
* **Câu hỏi & Trả lời**:
  * **Hỏi**: 80. "Soft Delete (Xóa mềm) là một pattern phổ biến trong thiết kế Database. Nó là gì? Tại sao trong các bảng như Product hay Supplier, tôi không gọi lệnh DELETE trong SQL để xóa hẳn, mà lại tạo thêm 1 cột `IsDeleted = true`?"
    * **Trả lời**:
      * **Soft Delete** là kỹ thuật đánh dấu bản ghi đã xóa bằng cờ `IsDeleted = true` thay vì dùng lệnh `DELETE` vật lý.
      * **Tại sao dùng**:
        * **Bảo toàn dữ liệu lịch sử**: Nếu xóa vĩnh viễn một sản phẩm, các đơn hàng cũ tham chiếu khóa ngoại đến sản phẩm đó sẽ bị lỗi ràng buộc hoặc bị xóa mất lịch sử doanh thu.
        * **Khôi phục dễ dàng**: Khôi phục dữ liệu chỉ cần set `IsDeleted = false`.
        * **Kiểm toán (Audit)**: Phục vụ lưu vết lịch sử hệ thống.
  * **Hỏi**: 81. "Trong Entity Framework Core, 'Global Query Filter' là gì? Nó giúp ích gì cho mô hình Xóa mềm (Soft Delete) ở trên để tôi không bị quên lọc dữ liệu ở mọi câu query?"
    * **Trả lời**:
      * **Global Query Filter** cấu hình bộ lọc mặc định trong DbContext.
      * Ví dụ ở `HushStoreDbContext.cs` (Dòng 164): `modelBuilder.Entity<Product>().HasQueryFilter(p => !p.IsDeleted...);`.
      * **Lợi ích**: EF Core tự động thêm `WHERE IsDeleted = 0` vào mọi câu lệnh LINQ truy vấn Products. Lập trình viên không bao giờ sợ quên lọc các bản ghi đã xóa mềm khi viết code lấy dữ liệu.

---

## Mảng 6: So sánh với "Cựu binh" (MVC Pattern)
* **Mục tiêu**: Nắm vững lý do đồ án chọn Web API + Blazor WASM thay vì MVC.
* **Câu hỏi & Trả lời**:
  * **Hỏi**: 88. "Mô hình MVC (Model-View-Controller) là gì? Hãy phân tích rõ vai trò của từng thành phần (Model làm gì, View làm gì, Controller làm gì) thông qua một ví dụ thực tế dễ hiểu như quy trình phục vụ gọi món ở nhà hàng."
    * **Trả lời**:
      * **Model**: Chứa dữ liệu và logic tương tác database.
      * **View**: Giao diện HTML hiển thị cho người dùng.
      * **Controller**: Tiếp nhận request, gọi Model lấy dữ liệu, truyền dữ liệu vào View để render thành HTML hoàn chỉnh.
      * **Ví dụ nhà hàng**:
        * *Controller (Bồi bàn)*: Nhận yêu cầu gọi món của khách, gửi yêu cầu vào bếp.
        * *Model (Nhà bếp)*: Chế biến món ăn theo yêu cầu.
        * *View (Bày biện đĩa ăn)*: Cách trình bày món ăn đẹp mắt lên đĩa trước khi bưng ra cho khách thưởng thức.
  * **Hỏi**: 89. "Đồ án của tôi dùng kiến trúc Decoupled (ASP.NET Core Web API làm Backend và Blazor WebAssembly làm Frontend). Nếu giảng viên hỏi: 'Tại sao em không dùng ASP.NET Core MVC (render sẵn HTML từ server) để làm luôn cho nhanh, tách ra thế này cho phức tạp', thì tôi phải phản biện thế nào về khả năng tái sử dụng (cho Mobile App) và hiệu năng của 2 cách làm này?"
    * **Trả lời**:
      * **Khả năng tái sử dụng**: Backend API độc lập hoàn toàn với giao diện. Nếu sau này làm thêm App Mobile, ta chỉ việc dùng lại các API này mà không cần code lại logic. Với MVC, giao diện dính chặt với server nên việc làm app mobile cực kỳ khó khăn.
      * **Hiệu năng**: Server API chỉ truyền dữ liệu JSON siêu nhẹ, không phải tốn CPU/RAM render HTML gửi về (Client tự render). Giúp giảm tải cho Server và tiết kiệm băng thông mạng rất nhiều.
      * **Trải nghiệm (UX)**: Blazor WASM là Single Page Application (SPA), chuyển trang tức thì, không bị tải lại trang toàn bộ mang lại trải nghiệm mượt mà giống phần mềm Desktop.

---

## Mảng 7: Phân chia cấu trúc code (N-Layer / N-Tier Architecture)
* **Mục tiêu**: Hiểu cấu trúc tổ chức code trong Solution.
* **Câu hỏi & Trả lời**:
  * **Hỏi**: 96. "Kiến trúc N-Layer (Kiến trúc phân tầng) là gì? Tại sao trong các dự án thực tế, người ta luôn chia code thành các tầng cơ bản như Presentation Layer (Giao diện), Business Logic Layer (Nghiệp vụ), và Data Access Layer (Truy cập dữ liệu) thay vì vứt hết vào chung một project?"
    * **Trả lời**:
      * **N-Layer** là việc tách biệt code thành các project (layers) có trách nhiệm riêng biệt: `API/Client` (Presentation), `Application` (Business Logic), `Infrastructure` (Data Access), `Core` (Domain).
      * **Tại sao chia**:
        * Tránh code chồng chéo, dễ bảo trì và mở rộng.
        * Dễ dàng viết Unit Test cho từng phần độc lập.
        * Khi thay đổi giao diện hoặc database, các phần còn lại không bị ảnh hưởng.
  * **Hỏi**: 97. "Hãy phân biệt sự khác nhau giữa 'Layer' (Tầng logic) và 'Tier' (Tầng vật lý). Trong đồ án của tôi, Backend API chạy trên Server và Frontend Blazor WASM chạy trên trình duyệt của người dùng thì hệ thống này đang áp dụng mấy Tier?"
    * **Trả lời**:
      * **Layer (Tầng logic)**: Phân tách code về mặt cấu trúc thư mục/project bên trong cùng 1 ứng dụng chạy trên 1 tiến trình vật lý.
      * **Tier (Tầng vật lý)**: Phân tách về mặt phần cứng/máy chủ vật lý chạy độc lập.
      * **Hệ thống áp dụng 3-Tier**:
        1. *Client Tier*: Trình duyệt người dùng chạy Blazor WASM.
        2. *Application Tier*: Web Server chạy ASP.NET Core API.
        3. *Database Tier*: Máy chủ database SQL Server.
  * **Hỏi**: 98. "Trong kiến trúc N-Layer (hoặc Clean Architecture/Vertical Slice), nguyên tắc phụ thuộc chiều sâu (Dependency Rule) hoạt động như thế nào? Tại sao project chứa Business Logic (Core) lại **Tuyệt đối không được phép** tham chiếu (reference) ngược ra project Infrastructure (chứa DbContext/SQL)? Nếu làm sai nguyên tắc này thì chuyện gì sẽ xảy ra?"
    * **Trả lời**:
      * **Dependency Rule**: Các tầng bên ngoài (Presentation, Infrastructure) tham chiếu vào tầng bên trong (Core). Core là trung tâm chứa thực thể và interface trừu tượng, không được phụ thuộc vào bên ngoài.
      * **Tại sao Core không được tham chiếu ngược Infrastructure**: Core chứa nghiệp vụ cốt lõi không đổi, còn Infrastructure chứa công nghệ cụ thể (EF Core, SQL Server) dễ thay đổi.
      * **Hậu quả nếu làm sai**: Gây lỗi vòng lặp tham chiếu (Circular Dependency) khiến project không thể build được. Đồng thời phá vỡ kiến trúc Clean Architecture, làm cho tầng nghiệp vụ bị ràng buộc chặt chẽ với EF Core, không thể viết test độc lập và khó chuyển đổi công nghệ.

---

# EFCore, Database

## Mảng 1: Triết lý thiết kế - Code-First & Migrations
* **Mục tiêu**: Hiểu quy trình đồng bộ hóa code C# và Database.
* **Câu hỏi & Trả lời**:
  * **Hỏi**: "Trong Entity Framework Core, phương pháp 'Code-First' là gì? Tại sao trong đồ án này, tôi không mở SQL Server Management Studio lên để tạo bảng, tạo cột bằng tay, mà lại viết code C# (các class Entity) trước rồi mới sinh ra Database?"
    * **Trả lời**:
      * **Code-First** là phương pháp phát triển mà lập trình viên thiết kế các Class thực thể trong C# trước, sau đó dùng công cụ của EF Core dịch code đó để tự tạo ra Database tương ứng.
      * **Tại sao dùng**:
        * Lập trình viên chỉ cần sử dụng C# hướng đối tượng để thiết kế hệ thống, không cần thành thạo các câu lệnh SQL viết trực tiếp.
        * Dễ dàng quản lý, kiểm soát lịch sử thay đổi của Database bằng Git thông qua các file code Migration.
  * **Hỏi**: "Khái niệm 'Migrations' trong EF Core là gì? Khi tôi muốn thêm một cột `Discount` vào bảng Product, các lệnh như `Add-Migration` và `Update-Database` hoạt động đằng sau như thế nào để đồng bộ giữa code C# và Database đang chạy?"
    * **Trả lời**:
      * **Migrations** là cơ chế ghi nhận và đồng bộ các thay đổi cấu trúc code C# xuống Database.
      * **Cơ chế hoạt động**:
        * `Add-Migration AddDiscountToProduct`: EF Core so sánh cấu trúc thực thể hiện tại với Snapshot cũ để sinh ra file migration mới chứa hàm `Up()` (thêm cột) và `Down()` (xóa cột).
        * `Update-Database`: EF Core đọc bảng `__EFMigrationsHistory` ở Database để biết migration nào chưa được áp dụng, sau đó chạy hàm `Up()` để tạo cột `Discount` trực tiếp trên SQL Server.

---

## Mảng 2: Trái tim của Data - DbContext & Fluent API
* **Mục tiêu**: Hiểu cách hoạt động của file cấu hình DB `HushStoreDbContext.cs`.
* **Câu hỏi & Trả lời**:
  * **Hỏi**: 3. "Class `DbContext` trong Entity Framework Core đóng vai trò gì? Hãy giải thích nó như một 'người phiên dịch' giữa ngôn ngữ hướng đối tượng (C#) và ngôn ngữ cơ sở dữ liệu (SQL)."
    * **Trả lời**:
      * `DbContext` là cầu nối trung gian:
        * Khi ta viết câu query LINQ trong C#, `DbContext` dịch nó sang câu lệnh SQL chuẩn để chạy xuống Database.
        * Khi SQL Server trả về các bản ghi, nó tự động ánh xạ (map) thành các đối tượng Object C# tương ứng.
        * Nó quản lý kết nối, theo dõi thay đổi (Change Tracking) và điều phối các Transaction.
  * **Hỏi**: 4. "Trong hàm `OnModelCreating` của file `HushStoreDbContext`, tôi thấy dùng rất nhiều lệnh như `HasIndex`, `HasColumnType`, `HasForeignKey`. Cách cấu hình này gọi là 'Fluent API'. Tại sao các lập trình viên có kinh nghiệm lại chuộng dùng Fluent API ở đây thay vì viết các thẻ (Data Annotations như `[Table]`, `[MaxLength]`) trực tiếp vào các class Entity? Việc tách cấu hình ra như vậy giúp gì cho nguyên lý thiết kế (Clean Architecture)?"
    * **Trả lời**:
      * **Fluent API** giữ cho các class Entity hoàn toàn sạch sẽ (POCO), không bị phụ thuộc vào các thư viện EF Core, giúp tuân thủ Clean Architecture.
      * Nó tách biệt cấu hình Database ra khỏi thực thể logic. Ngoài ra, Fluent API mạnh mẽ hơn, hỗ trợ các cấu hình phức tạp (như Global Query Filter, Check Constraint, Computed Column) mà Data Annotations không thể viết được.
  * **Hỏi**: 5. "Trong bảng `ProductVariant`, cột `Specifications` (chứa cấu hình máy tính) được lưu dưới dạng chuỗi JSON bằng hàm `HasConversion`. Việc lưu một cấu trúc động (Dictionary) thành chuỗi JSON vào 1 cột duy nhất thay vì tạo thêm 1 bảng riêng (`ProductAttributes` nối 1-n) có ưu và nhược điểm gì? (Gợi ý: Trả lời về tốc độ truy vấn NoSQL so với RDBMS)."
    * **Trả lời**:
      * **Ưu điểm**:
        * **Linh hoạt cao**: Mỗi sản phẩm (RAM, CPU, Chuột) có các thuộc tính kỹ thuật hoàn toàn khác nhau. Lưu JSON giúp ta lưu mọi thuộc tính động mà không cần tạo hàng chục cột null hoặc tạo bảng quan hệ 1-n phức tạp.
        * **Đọc nhanh (Eager Load)**: Chỉ cần 1 câu query lấy Variant lên là có đầy đủ cấu hình kỹ thuật mà không cần dùng câu lệnh JOIN sang bảng thuộc tính khác.
      * **Nhược điểm**:
        * **Lọc (Filter) chậm**: Khó đánh index lên các thuộc tính nằm sâu bên trong chuỗi JSON, khiến tốc độ lọc sản phẩm theo thuộc tính chậm hơn so với bảng quan hệ chuẩn.
        * Không ràng buộc được toàn vẹn kiểu dữ liệu ở mức Database.

---

## Mảng 3: ASP.NET Core Identity (Bảo mật & Quản lý User)
* **Mục tiêu**: Hiểu cách custom Identity phục vụ bảo mật hệ thống.
* **Câu hỏi & Trả lời**:
  * **Hỏi**: 6. "ASP.NET Core Identity là gì? Tại sao tôi không tự tạo bảng User (gồm username, password), tự mã hóa MD5 rồi tự viết code đăng nhập, mà lại phải kế thừa từ `IdentityDbContext` cho phức tạp?"
    * **Trả lời**:
      * **Identity** cung cấp sẵn các chức năng quản lý tài khoản đạt tiêu chuẩn bảo mật cao (Password Hashing PBKDF2 cực mạnh, đăng nhập 2 lớp, khóa tài khoản chống brute-force, quản lý role...).
      * Tự viết hệ thống Auth rất dễ dính lỗ hổng bảo mật. Kế thừa `IdentityDbContext` giúp tận dụng các thư viện bảo mật chuẩn đã được Microsoft kiểm duyệt kỹ càng, an toàn tuyệt đối.
  * **Hỏi**: 7. "Mặc định, Identity dùng kiểu chuỗi (string) cho Khóa chính (Id) của User. Tuy nhiên, trong đồ án này, class `HushStoreDbContext` lại được custom thành `IdentityDbContext<AppUser, AppRole, Guid...>` để dùng kiểu Guid làm khóa chính, đồng thời set default SQL là `NEWSEQUENTIALID()`. Việc dùng Guid thay vì số nguyên tự tăng (int identity) có lợi ích gì về mặt bảo mật và mở rộng hệ thống?"
    * **Trả lời**:
      * **Bảo mật**: Dùng Guid tránh tấn công dò đoán ID (Enumeration Attack) vì chuỗi Guid là ngẫu nhiên và không đoán trước được (khác với số nguyên tự tăng 1, 2, 3...).
      * **Mở rộng (Distributed DB)**: Ta có thể sinh khóa Guid duy nhất ở Client/Server mà không cần kết nối DB và không sợ trùng khóa khi đồng bộ dữ liệu.
      * **NEWSEQUENTIALID()**: Sinh ra các Guid tuần tự giúp SQL Server ghi các bản ghi mới vào cuối bảng index vật lý, hạn chế tối đa tình trạng **Index Fragmentation** (phân mảnh chỉ mục), tăng tốc độ ghi dữ liệu.

---

## Mảng 4: Vũ khí tối thượng - Global Query Filter
* **Mục tiêu**: Hiểu cơ chế Xóa mềm tự động.
* **Câu hỏi & Trả lời**:
  * **Hỏi**: 8. "Trong Database, Xóa mềm (Soft Delete) là thay vì gọi lệnh DELETE, ta thêm cột `IsDeleted = true`. Nhưng nếu làm vậy, mọi câu lệnh SELECT đều phải nhớ thêm điều kiện `WHERE IsDeleted == false`. Trong EF Core, tính năng 'Global Query Filter' (sử dụng lệnh `HasQueryFilter`) giúp giải quyết vấn đề quên lọc dữ liệu này một cách tự động như thế nào?"
    * **Trả lời**:
      * Khi ta khai báo `HasQueryFilter(m => !m.IsDeleted)` trong `OnModelCreating`, EF Core sẽ tự động chèn thêm điều kiện `AND IsDeleted = 0` vào cuối mọi câu lệnh SQL sinh ra từ LINQ.
      * Nhờ đó, lập trình viên không bao giờ bị quên lọc bản ghi đã xóa mềm khi viết code truy vấn dữ liệu. Muốn bỏ lọc ta chỉ việc dùng `.IgnoreQueryFilters()`.

---

## Mảng 5: Xử lý Quan hệ & Lỗi "Multiple Cascade Paths"
* **Mục tiêu**: Giải thích cấu hình khóa ngoại phức tạp.
* **Câu hỏi & Trả lời**:
  * **Hỏi**: 9. "Trong thiết kế Database, khi xóa một bản ghi cha (Ví dụ: Đơn hàng), thì các bản ghi con (Ví dụ: Chi tiết đơn hàng) sẽ bị xóa theo. Hành động này trong EF Core gọi là `DeleteBehavior.Cascade`. Vậy `DeleteBehavior.NoAction` nghĩa là gì và dùng khi nào?"
    * **Trả lời**:
      * **DeleteBehavior.NoAction**: DB sẽ chặn đứng hành động xóa bản ghi cha nếu vẫn còn bản ghi con đang tham chiếu khóa ngoại tới nó.
      * **Dùng khi**: Muốn bảo toàn dữ liệu con, tránh việc vô tình xóa mất dữ liệu liên đới quan trọng (ví dụ: không cho xóa Khách hàng khi họ vẫn còn Đơn hàng trong DB).
  * **Hỏi**: 10. "Trong hàm cấu hình bảng `OrderSerial` của tôi, có comment nhắc đến lỗi 'SQL Server Error 1785: Multiple cascade paths detected'. Sau đó tôi phải sửa lại bằng cách dùng `OnDelete(DeleteBehavior.NoAction)`. Lỗi 'Đa luồng xóa dây chuyền' này trong SQL Server nghĩa là gì? Hãy giải thích một cách dễ hiểu nhất."
    * **Trả lời**:
      * Lỗi xảy ra khi một bảng con có thể bị xóa dây chuyền (Cascade) từ nhiều con đường (bảng cha) khác nhau.
      * **Ví dụ**: Xóa `ProductVariant` sẽ xóa `ProductSerial` -> xóa `OrderSerial` (Đường 1). Đồng thời xóa `ProductVariant` sẽ xóa `OrderDetail` -> xóa `OrderSerial` (Đường 2).
      * SQL Server từ chối vì không xác định được thứ tự xóa và sợ vòng lặp vô hạn.
      * **Giải pháp**: Sửa thành `DeleteBehavior.NoAction` (Dòng 396, 401 của `HushStoreDbContext.cs`) và xử lý xóa dữ liệu liên quan thủ công ở tầng Service.

---

## Mảng 6: Tối ưu hiệu năng (Performance & Tracking)
* **Mục tiêu**: Chứng minh khả năng tối ưu hóa ứng dụng.
* **Câu hỏi & Trả lời**:
  * **Hỏi**: 11. "Trong các hàm lấy danh sách (như `GetPagedListAsync` ở `ProductRepository`), tôi thấy code dùng lệnh `.AsNoTracking()`. 'Tracking' trong Entity Framework Core là gì? Tại sao khi chỉ đọc dữ liệu ra xem (Read-only) thì **BẮT BUỘC** phải thêm `.AsNoTracking()`, nó giúp tối ưu RAM và CPU của Server như thế nào?"
    * **Trả lời**:
      * **Tracking**: Là cơ chế EF Core lưu bản sao thực thể trong bộ nhớ RAM (Tracker) để phát hiện thay đổi và sinh lệnh UPDATE khi gọi `SaveChanges()`.
      * **Tại sao dùng `.AsNoTracking()`**: Khi chỉ đọc dữ liệu (Read-only), việc tắt bộ theo dõi giúp EF Core bỏ qua bước tạo bản sao trong RAM và bỏ qua bộ kiểm tra thay đổi.
      * Giúp tiết kiệm lượng lớn bộ nhớ RAM của Server và tăng tốc độ xử lý CPU khi truy vấn danh sách lớn.
  * **Hỏi**: 12. "Trong EF Core có khái niệm Eager Loading (Tải gộp) và Lazy Loading (Tải lười). Ở đồ án này, tôi thấy dùng rất nhiều lệnh `.Include(p => p.Manufacturer)` hoặc `.ThenInclude(...)`. Đây là Eager Loading đúng không? Nếu tôi không dùng `.Include` mà cứ thế gọi `product.Manufacturer.Name` thì EF Core sẽ sinh ra lỗi gì (N+1 Query)? Lỗi N+1 nguy hiểm ra sao?"
    * **Trả lời**:
      * Đúng, đó là **Eager Loading** (tải trước bằng lệnh JOIN bảng trong SQL).
      * Nếu không dùng `.Include` mà gọi thuộc tính quan hệ: EF Core sẽ ném ra lỗi Null Reference (nếu tắt Lazy Loading) hoặc kích hoạt **N+1 Query** (nếu bật Lazy Loading).
      * **Lỗi N+1**: Lấy 100 sản phẩm mất 1 query. Lặp qua 100 sản phẩm để lấy tên nhà sản xuất mất thêm 100 query nữa. Tổng cộng chạy **101 query** xuống DB, gây nghẽn băng thông và treo hệ thống DB dưới tải lớn.
  * **Hỏi**: 13. "Sự khác biệt giữa `IQueryable` và `IEnumerable` (hoặc `List`) trong C# là gì? Khi tôi viết các hàm `.Where().OrderBy()` trên `IQueryable` thì lệnh SQL đã chạy xuống Database chưa? Hàm `.ToListAsync()` đóng vai trò gì để 'bóp cò' cho câu lệnh SQL thực thi?"
    * **Trả lời**:
      * `IQueryable` chứa câu lệnh truy vấn đang được xây dựng trong RAM (chưa chạy dưới DB).
      * `IEnumerable`/`List` chứa dữ liệu thực tế đã được tải hoàn toàn vào bộ nhớ RAM của Web Server.
      * Viết `.Where().OrderBy()` trên `IQueryable` thì **chưa chạy SQL xuống DB**.
      * Hàm `.ToListAsync()` đóng vai trò là "Cú bóp cò", yêu cầu EF Core dịch toàn bộ logic của `IQueryable` thành SQL, gửi xuống DB chạy và nạp kết quả vào RAM.

---

## Mảng 7: Khóa "chết" ở Database (Database-level Constraints)
* **Mục tiêu**: Chặn lỗi ở tầng Database để dữ liệu không bị rác.
* **Câu hỏi & Trả lời**:
  * **Hỏi**: 14. "Trong file cấu hình Database (`HushStoreDbContext`), tôi thấy có đoạn: `.HasCheckConstraint("CK_Vouchers_Quantity", "[UsedCount] <= [Quantity]")`. 'Check Constraint' là gì? Tại sao tôi không dùng lệnh `if (UsedCount > Quantity)` ở trong C# để bắt lỗi thôi mà phải ép logic này xuống tận Database?"
    * **Trả lời**:
      * **Check Constraint** là ràng buộc kiểm tra tính hợp lệ dữ liệu trực tiếp ở mức Database Server.
      * **Tại sao không chỉ bắt bằng `if` ở C#**: Trong môi trường nhiều người dùng cùng lúc (Concurrency), 2 luồng có thể đồng thời vượt qua điều kiện `if` của C# cùng lúc. Chặn ở Database là lớp phòng ngự cuối cùng tuyệt đối an toàn để chống lỗi race condition ghi đè dữ liệu.
  * **Hỏi**: 15. "Cũng trong file đó, cột `Difference` của bảng `InventoryCheckDetail` được cấu hình là: `.HasComputedColumnSql("([ActualQuantity] - [SystemQuantity])")`. 'Computed Column' (Cột tính toán) là gì? Tại sao lại bắt Database trừ 2 cột này cho nhau thay vì tôi tự lấy A - B rồi lưu vào DB bằng C#?"
    * **Trả lời**:
      * **Computed Column** là cột tự động tính giá trị dựa trên công thức toán học từ các cột khác trong bảng.
      * **Tại sao dùng**:
        * Đảm bảo tính nhất quán toán học tuyệt đối (tránh việc lập trình viên viết code tính sai).
        * Tiết kiệm bộ nhớ vì DB không cần lưu vật lý giá trị cột này (chỉ tính toán động khi truy vấn).
  * **Hỏi**: 16. "Tôi thấy code dùng rất nhiều lệnh `.HasIndex(p => p.Slug).IsUnique()`. Đánh 'Index' (Chỉ mục) trong Database là gì? Hãy ví nó như cái mục lục của quyển sách. Việc thêm đuôi `.IsUnique()` có tác dụng gì trong việc ngăn chặn 2 sản phẩm trùng mã SKU hoặc trùng Slug?"
    * **Trả lời**:
      * **Index (Chỉ mục)**: Giúp tăng tốc độ tìm kiếm bản ghi. Thay vì quét toàn bộ bảng (Table Scan), SQL Server dò tìm trên chỉ mục cây để tìm thấy dòng cần thiết ngay lập tức.
      * **Tác dụng `.IsUnique()`**: Tạo ràng buộc độc nhất ở mức Database, chặn đứng tuyệt đối mọi hành vi ghi đè dữ liệu trùng mã SKU hoặc Slug sản phẩm (kể cả do lỗi concurrency hoặc nhập liệu sai của nhân viên).

---

## Mảng 8: Xử lý đồng thời (Concurrency) & Phân trang
* **Mục tiêu**: Xử lý bài toán thực tế dưới tải lớn.
* **Câu hỏi & Trả lời**:
  * **Hỏi**: 17. "Khái niệm 'Concurrency' (Xử lý đồng thời) trong Database là gì? Giả sử tại quầy POS, có 2 nhân viên cùng bấm thanh toán và quét trúng 1 mã Serial duy nhất ở cùng 1 phần nghìn giây. EF Core và Database (SQL Server) có cơ chế gì để chặn lại, đảm bảo mã Serial đó không bị bán đúp cho cả 2 hóa đơn?"
    * **Trả lời**:
      * **Concurrency**: Là việc xử lý khi nhiều tác vụ cùng truy cập/ghi vào một tài nguyên DB cùng lúc.
      * **Cơ chế chặn**: Database sử dụng các cấp độ Khóa (Locks) và Transactions. Khi luồng 1 cập nhật trạng thái Serial sang "Đã bán", nó sẽ khóa dòng đó lại (Row Lock). Luồng 2 phải xếp hàng chờ. Khi luồng 2 vào xử lý sau đó, DB kiểm tra thấy trạng thái đã thay đổi nên sẽ ném lỗi Concurrency Exception và rollback transaction của nhân viên thứ 2.
  * **Hỏi**: 18. "Khi tôi gọi hàm `.Skip((pageNumber - 1) * pageSize).Take(pageSize)` trong C#, câu lệnh SQL sinh ra dưới SQL Server sẽ dùng từ khóa gì (Gợi ý: OFFSET FETCH)? Nếu tôi bỏ `.Skip.Take` đi, lấy `.ToList()` nguyên cả bảng Product có 1 triệu dòng lên RAM rồi mới dùng C# cắt ra 10 dòng đầu tiên thì hậu quả bị đần như thế nào?"
    * **Trả lời**:
      * SQL Server sẽ sinh ra từ khóa: `OFFSET ... ROWS FETCH NEXT ... ROWS ONLY`.
      * **Hậu quả nếu lấy `.ToList()` toàn bộ trước**: Web Server sẽ phải tải 1 triệu bản ghi từ DB truyền qua mạng vào RAM của nó. Việc này sẽ ngay lập tức gây nghẽn băng thông mạng, tràn bộ nhớ RAM (OutOfMemory Exception) và làm sập Web Server tức khắc.

---

## Mảng 9: Seed Data & Quản lý vòng đời
* **Mục tiêu**: Các cơ chế tiện ích của Database.
* **Câu hỏi & Trả lời**:
  * **Hỏi**: 19. "Tôi thấy một số cột được set giá trị mặc định bằng hàm `.HasDefaultValue(0)` hoặc `.HasDefaultValueSql("NEWSEQUENTIALID()")`. Khác biệt giữa việc set default ở tận Database với việc tôi gán giá trị mặc định ở class C# (ví dụ: `public int StockQuantity { get; set; } = 0;`) là gì?"
    * **Trả lời**:
      * Gán mặc định ở C# chỉ hoạt động khi ta khởi tạo đối tượng mới bằng code C# trong ứng dụng.
      * Set default ở Database đảm bảo nếu có một phần mềm khác (ví dụ: tool SQL import, hoặc Database admin) chèn dữ liệu trực tiếp vào DB mà không thông qua code C#, Database vẫn tự động điền các giá trị mặc định này.
  * **Hỏi**: 20. "Khi làm việc nhóm (Teamwork) với EF Core Migrations, giả sử 2 người (A và B) cùng chạy lệnh `Add-Migration` ở 2 máy khác nhau và cùng push code lên Github. Chuyện gì sẽ xảy ra với file Database khi gộp code (Merge)? Cách giải quyết 'Conflict Migrations' này là gì?"
    * **Trả lời**:
      * **Hiện tượng**: File `HushStoreDbContextModelSnapshot.cs` và các file migration sẽ bị xung đột (Conflict) vì chứa ID lịch sử migration khác nhau.
      * **Cách giải quyết**: Người merge code sẽ xóa file migration cục bộ của mình đi bằng lệnh `Remove-Migration`, tiến hành pull code mới nhất của người kia về để cập nhật Snapshot chính xác, sau đó chạy lại lệnh `Add-Migration` trên nền DB mới để sinh ra file migration nối tiếp đồng bộ.

---

# Bảo mật

## Mảng 1: Cơ chế cốt lõi - JWT (JSON Web Token)
* **Mục tiêu**: Hiểu rõ chìa khóa bảo mật stateless của hệ thống.
* **Câu hỏi & Trả lời**:
  * **Hỏi**: "JWT (JSON Web Token) là gì? Hãy giải thích cơ chế đăng nhập bằng JWT qua một ví dụ thực tế như việc mua vé vào rạp chiếu phim."
    * **Trả lời**:
      * **JWT** là một chuỗi ký tự mã hóa an toàn chứa thông tin (claims) dạng JSON, được ký số để đảm bảo tính xác thực.
      * **Ví dụ thực tế**: Mua vé xem phim online.
        * Bạn đăng nhập (Mua vé thành công). Hệ thống cấp cho bạn một Vé xem phim (JWT) chứa thông tin: Tên phim, Số ghế, Giờ chiếu (Payload) có đóng dấu mộc đỏ của rạp (Signature).
        * Khi vào phòng chiếu, bạn chỉ trình vé cho bảo vệ (gửi JWT). Bảo vệ kiểm tra mộc đỏ còn nguyên và hợp lệ (giải mã chữ ký bằng khóa bí mật) là cho bạn vào, không cần gọi điện vào quầy vé để đối chiếu (Stateless).
  * **Hỏi**: "Tại sao trong kiến trúc tách rời (Web API + Blazor WASM), người ta lại dùng JWT (Token-based Authentication) thay vì dùng Session/Cookie như mô hình MVC truyền thống? (Gợi ý: Trả lời về tính phi trạng thái - Stateless của API)."
    * **Trả lời**:
      * Vì Backend API là **Stateless (Phi trạng thái)**. Server không lưu trạng thái đăng nhập của user trong bộ nhớ RAM của nó.
      * Dùng Session bắt Server phải lưu trữ đống session data của hàng ngàn user, gây tốn RAM và khó mở rộng sang nhiều server chạy song song (Load Balancing). Dùng JWT giúp server hoàn toàn rảnh tay, chỉ cần giải mã token nhận được từ client để xác thực quyền truy cập.
  * **Hỏi**: "Một chuỗi JWT (ví dụ: eyJhb...) thường gồm 3 phần (Header, Payload, Signature). Hãy giải thích chức năng của từng phần. Tại sao Frontend có thể đọc được dữ liệu (như Role, Email) từ Payload mà không sợ bị người dùng tự ý sửa đổi?"
    * **Trả lời**:
      * **Header**: Chứa kiểu token (JWT) và thuật toán băm chữ ký (ví dụ HS256).
      * **Payload**: Chứa các Claims dữ liệu của user (UserId, Roles, Expire...). Được mã hóa dạng Base64 nên Frontend có thể giải mã và đọc dễ dàng để hiển thị UI phù hợp.
      * **Signature (Chữ ký)**: Được tạo ra bằng cách lấy (Header + Payload) kết hợp với khóa bí mật (Secret Key) chỉ server biết để băm lại.
      * **Tại sao không sợ sửa đổi**: Nếu user cố tình sửa đổi Role từ "User" thành "Admin" ở phần Payload, khi gửi lên API, Server chạy lại thuật toán băm chữ ký sẽ phát hiện Signature không khớp với Payload mới và lập tức từ chối token (401 Unauthorized).
  * **Hỏi**: "Refresh Token là gì? Tại sao Access Token lại có tuổi thọ ngắn (ví dụ: 15 phút), còn Refresh Token lại có tuổi thọ dài (ví dụ: 7 ngày)? Cơ chế này giúp bảo mật hệ thống như thế nào nếu Access Token bị lộ?"
    * **Trả lời**:
      * **Access Token**: Có tuổi thọ ngắn (15 phút), gửi kèm liên tục ở mọi request nên nguy cơ bị lộ cao. Hạn ngắn giúp giảm thiểu thời gian hacker có thể lợi dụng token đó nếu bị đánh cắp.
      * **Refresh Token**: Tuổi thọ dài (7 ngày), dùng để gửi lên API đổi lấy Access Token mới mà không bắt user phải đăng nhập lại bằng mật khẩu.
      * Nếu Access Token bị lộ, nó sẽ nhanh chóng hết hạn sau 15 phút. Hacker sẽ không thể tiếp tục phá hoại trừ khi có Refresh Token (vốn được lưu trữ kỹ lưỡng hơn và chỉ gửi đi 1 lần khi cần refresh).

---

## Mảng 2: Luồng xử lý ở Frontend (Blazor WebAssembly)
* **Mục tiêu**: Hiểu cách Frontend nhận, lưu trữ và tự động gửi Token.
* **Câu hỏi & Trả lời**:
  * **Hỏi**: 5. "Trong code của đồ án, hàm `AuthClientService.LoginAsync` sẽ lưu Access Token và Refresh Token vào LocalStorage của trình duyệt. LocalStorage là gì? Tại sao không lưu vào biến (variable) trong C# mà lại phải lưu vào LocalStorage?"
    * **Trả lời**:
      * **LocalStorage** là bộ nhớ lưu trữ key-value của trình duyệt giúp lưu dữ liệu vĩnh viễn (không bị mất khi tắt tab hoặc tắt máy).
      * Không lưu vào biến C# vì biến chỉ sống trong RAM của ứng dụng Blazor. Khi user reload trang (F5) hoặc mở tab mới, ứng dụng khởi tạo lại, RAM bị xóa sạch và bắt user đăng nhập lại từ đầu.
  * **Hỏi**: 6. "Trong Blazor WebAssembly, class `AuthHeaderHandler` (kế thừa `DelegatingHandler`) đóng vai trò gì? Hãy giải thích cách nó tự động chặn (intercept) mọi Request từ HttpClient, lấy Token từ LocalStorage và nhét vào Header (`Authorization: Bearer <token>`) trước khi gửi lên API."
    * **Trả lời**:
      * `AuthHeaderHandler` đóng vai trò là một **HTTP Client Middleware** (Dòng 11 của `AuthHeaderHandler.cs`).
      * Mỗi khi HttpClient gửi request đi, `AuthHeaderHandler` sẽ tự động nhảy vào: đọc token từ LocalStorage, thêm chuỗi `"Bearer " + token` vào header `Authorization`, rồi mới chuyển tiếp request lên server API.
  * **Hỏi**: 7. "Thử tưởng tượng nếu không có cái `AuthHeaderHandler` này, thì mỗi lần tôi muốn gọi hàm `GetOrdersAsync`, tôi sẽ phải viết code gửi kèm Token lặp đi lặp lại như thế nào? Cách giải quyết bằng `DelegatingHandler` giúp ích gì cho nguyên lý DRY (Don't Repeat Yourself)?"
    * **Trả lời**:
      * Nếu không có, ở mọi hàm gọi API ở các Service, ta phải thủ công gọi LocalStorage lấy token rồi nhét vào header của HttpContent gửi đi. Code sẽ bị lặp lại hàng chục lần ở mọi API call.
      * `DelegatingHandler` giúp giải quyết tập trung ở 1 nơi duy nhất cho toàn bộ project Client, tuân thủ tuyệt đối nguyên tắc DRY.

---

## Mảng 3: Quản lý Trạng thái Đăng nhập & Phân quyền Giao diện
* **Mục tiêu**: Hiểu cách Blazor quản lý trạng thái hiển thị giao diện phân quyền.
* **Câu hỏi & Trả lời**:
  * **Hỏi**: 8. "Trong Blazor, class `JwtAuthenticationStateProvider` (kế thừa `AuthenticationStateProvider`) làm nhiệm vụ gì? Làm sao nó dịch ngược được chuỗi Base64 Payload của JWT từ LocalStorage để rút trích ra các thông tin (Claims) như Role, Email, Tên user?"
    * **Trả lời**:
      * `JwtAuthenticationStateProvider` cung cấp trạng thái xác thực (`AuthenticationState`) cho toàn bộ Client Blazor.
      * Nó lấy token từ LocalStorage, cắt lấy phần Payload (phần thứ 2), decode Base64Url (thêm lại padding `=` ở cuối ở dòng 142 để tránh lỗi format), parse thành Dictionary JSON rồi chuyển đổi các thông tin thành danh sách các đối tượng `Claim` để tạo nên `ClaimsPrincipal` cho user.
  * **Hỏi**: 9. "Tại sao hàm `NotifyAuthStateChanged` lại được gọi mỗi khi user Đăng nhập hoặc Đăng xuất thành công? Hành động này sẽ 'báo cáo' cho các Component trên màn hình (như Header) cập nhật lại giao diện như thế nào?"
    * **Trả lời**:
      * Hàm này kích hoạt sự kiện cập nhật trạng thái Auth cho Blazor.
      * Các component đang lắng nghe trạng thái này (như Header chứa thông tin User) sẽ tự động kích hoạt quá trình re-render (vẽ lại giao diện) để ẩn nút Đăng nhập và hiện tên User ngay lập tức mà không cần F5 trang web.
  * **Hỏi**: 10. "Trong code Blazor, tôi thấy sử dụng Component `<AuthorizeView>`. Hãy giải thích cách nó dùng để ẩn/hiện nút 'Đăng nhập' (khi chưa login) và nút 'Đăng xuất' (khi đã login)."
    * **Trả lời**:
      * `<AuthorizeView>` tự động đọc trạng thái đăng nhập từ Provider:
        * Khi chưa đăng nhập: Nội dung trong thẻ `<NotAuthorized>` (Nút Đăng nhập/Đăng ký) sẽ được hiển thị.
        * Khi đã đăng nhập: Nội dung trong thẻ `<Authorized>` (Nút Đăng xuất, tên User) sẽ hiển thị.
  * **Hỏi**: 11. "Làm sao để tôi chặn người dùng truy cập vào một trang (ví dụ: `/admin/orders`) nếu họ chưa đăng nhập, hoặc đăng nhập rồi nhưng không có Role là 'Admin'? Hãy giải thích cách sử dụng thuộc tính `[Authorize]` và `<AuthorizeRouteView>`."
    * **Trả lời**:
      * Khai báo ở trang: `@attribute [Authorize(Roles = "Admin")]` trên đầu file trang Admin.
      * Ở `App.razor`, dùng component `<AuthorizeRouteView>` bọc Router. Khi người dùng truy cập URL nhạy cảm, router sẽ chặn kiểm tra Claims của user, nếu chưa login hoặc thiếu Role Admin sẽ lập tức redirect sang trang đăng nhập hoặc báo lỗi không đủ quyền truy cập.

---

## Mảng 4: Câu hỏi "xoáy" (Nâng cao)
* **Mục tiêu**: Hiểu rõ lỗ hổng bảo mật.
* **Câu hỏi & Trả lời**:
  * **Hỏi**: 12. "Lưu JWT Token vào LocalStorage có một rủi ro bảo mật nổi tiếng là tấn công XSS (Cross-Site Scripting). Tấn công XSS là gì? Làm sao một đoạn mã độc JavaScript có thể 'ăn cắp' Token của tôi từ LocalStorage? (Gợi ý thêm: Tại sao một số dự án lớn lại khuyên lưu Token vào HttpOnly Cookie thay vì LocalStorage?)."
    * **Trả lời**:
      * **XSS** là lỗ hổng cho phép hacker chèn các đoạn script độc hại (JavaScript) chạy trực tiếp trên trình duyệt của nạn nhân thông qua nội dung trang web.
      * **Hacker ăn cắp token**: Mã độc JS chỉ cần thực thi lệnh đơn giản `localStorage.getItem('authToken')` là lấy được token và gửi lén lút về server của hacker.
      * **HttpOnly Cookie**: Cookie này được gắn cờ `HttpOnly` khiến cho mã JavaScript trên trình duyệt hoàn toàn bị cấm truy cập. Trình duyệt tự đính kèm cookie này khi gọi API, giúp chống lại hoàn toàn tấn công đánh cắp token bằng XSS.

---

# API

## Mảng 1: API là cái quái gì? (Khái niệm & Vai trò)
* **Mục tiêu**: Hiểu rõ mục đích tồn tại của tầng API.
* **Câu hỏi & Trả lời**:
  * **Hỏi**: "API (Application Programming Interface) là gì? Hãy giải thích cho tôi thông qua ví dụ kinh điển: Khách hàng (Client), Bồi bàn (API), và Nhà bếp (Server/Database)."
    * **Trả lời**:
      * **API** là giao diện lập trình ứng dụng đóng vai trò làm trung gian truyền tải thông tin, dữ liệu giữa các phần mềm độc lập.
      * **Ví dụ thực tế**:
        * *Khách hàng (Client)*: Bạn ngồi ở bàn gọi món (xem giao diện Blazor).
        * *Bồi bàn (API)*: Ghi nhận yêu cầu món ăn từ bạn, chạy vào bếp chuyển đạt lại cho đầu bếp, và bê món ăn ra cho bạn sau khi chế biến xong.
        * *Nhà bếp (Server/Database)*: Nơi chế biến thức ăn và quản lý nguyên liệu trong tủ lạnh.
  * **Hỏi**: "Trong đồ án của tôi, Frontend chạy bằng Blazor WebAssembly, còn Backend viết bằng ASP.NET Core Web API. Tại sao Frontend không móc thẳng vào Database SQL Server để lấy dữ liệu cho lẹ, mà phải nhờ vả qua thằng API? Tự nhiên làm mọi thứ chậm lại?"
    * **Trả lời**:
      * **Bảo mật tuyệt đối**: Blazor WASM chạy trên trình duyệt máy khách. Nếu kết nối trực tiếp DB, ta buộc phải ghi lộ thông tin kết nối DB (Connection String) trong code tải về máy khách. Hacker sẽ lấy được và chiếm quyền kiểm soát toàn bộ Database của bạn.
      * **Thực thi nghiệp vụ & Bảo vệ dữ liệu**: API đóng vai trò là "Người gác cổng". Nó lọc dữ liệu, xác thực quyền hạn (Authentication) và kiểm tra tính hợp lệ trước khi cho phép dữ liệu ghi xuống Database.

---

## Mảng 2: RESTful API & 4 Hành động cơ bản (HTTP Methods)
* **Mục tiêu**: Hiểu chuẩn thiết kế endpoint thế giới đang dùng.
* **Câu hỏi & Trả lời**:
  * **Hỏi**: 3. "RESTful API là gì? Nó quy định những quy tắc gì khi thiết kế các đường dẫn (URL)?"
    * **Trả lời**:
      * **RESTful API** là chuẩn thiết kế API dựa trên giao thức truyền thống HTTP.
      * **Quy tắc thiết kế URL**:
        * Sử dụng danh từ số nhiều (ví dụ: `api/products`, `api/customers` thay vì `api/getProducts`).
        * Không chứa động từ trong URL.
        * Phân tách mối quan hệ phân cấp bằng dấu gạch chéo (ví dụ: `api/products/1/variants`).
        * Dùng các HTTP Methods để biểu thị hành động cụ thể trên tài nguyên đó.
  * **Hỏi**: 4. "Trong file `ProductClientService.cs` của tôi có dùng các hàm của `_httpClient` như `GetFromJsonAsync`, `PostAsJsonAsync`, `PutAsJsonAsync`, `DeleteAsync`. Hãy giải thích chức năng của 4 HTTP Method (GET, POST, PUT, DELETE) này ứng với các thao tác CRUD (Create, Read, Update, Delete)."
    * **Trả lời**:
      * **GET** tương ứng với **Read (Đọc)**: Lấy danh sách hoặc thông tin chi tiết (Ví dụ: `GetFromJsonAsync`).
      * **POST** tương ứng với **Create (Tạo mới)**: Gửi dữ liệu tạo bản ghi mới lên server (`PostAsJsonAsync`).
      * **PUT** tương ứng với **Update (Cập nhật)**: Gửi thông tin thay đổi để ghi đè bản ghi cũ (`PutAsJsonAsync`).
      * **DELETE** tương ứng với **Delete (Xóa)**: Yêu cầu xóa bản ghi (`DeleteAsync`).
  * **Hỏi**: 5. "Khi tôi gọi `GetFromJsonAsync("api/products/1")` và `DeleteAsync("api/products/1")`, cái URL (đường dẫn) y chang nhau, nhưng làm sao Backend biết lúc nào tôi muốn 'Lấy thông tin' sản phẩm số 1, lúc nào tôi muốn 'Xóa' sản phẩm số 1?"
    * **Trả lời**:
      * Backend phân biệt dựa trên **HTTP Method (Verb)** đính kèm trong tiêu đề Request gửi lên. Bộ định tuyến (Routing) của ASP.NET Core Web API sẽ tự động đọc Method này để điều hướng:
        * Nếu là GET -> trỏ vào hàm xử lý `GetById`.
        * Nếu là DELETE -> trỏ vào hàm xử lý `Delete`.

---

## Mảng 3: Ngôn ngữ giao tiếp (JSON & DTO)
* **Mục tiêu**: Hiểu định dạng dữ liệu truyền tải.
* **Câu hỏi & Trả lời**:
  * **Hỏi**: 6. "Khi Client gửi dữ liệu (ví dụ: tạo sản phẩm) lên Server, chúng dùng định dạng JSON. JSON là gì? Tại sao nó lại trở thành chuẩn giao tiếp phổ biến nhất hiện nay?"
    * **Trả lời**:
      * **JSON** là định dạng văn bản nhẹ, biểu diễn dữ liệu dưới dạng key-value, rất dễ đọc hiểu với con người và dễ phân tích đối với máy tính.
      * **Phổ biến vì**: Nó độc lập ngôn ngữ (C#, Python, JS... đều hỗ trợ tốt) và dung lượng siêu nhẹ giúp truyền tải qua mạng internet cực kỳ nhanh chóng.
  * **Hỏi**: 7. "Trong đồ án, tôi thấy mọi API trả về đều được bọc trong một class tên là `ApiResult<T>` (ví dụ: `ApiResult<ProductDetailDto>`). Việc bọc dữ liệu trong một khung chuẩn có thuộc tính `Success`, `Message`, `Data` như thế này mang lại lợi ích gì cho Frontend khi xử lý lỗi so với việc chỉ trả về mỗi dữ liệu thô?"
    * **Trả lời**:
      * Giúp Frontend xử lý lỗi đồng nhất. Khi nhận phản hồi, Client chỉ cần check thuộc tính `Success` để biết thành công hay thất bại.
      * Nếu thất bại, ta lập tức lấy nội dung trong `Message` hiển thị cảnh báo lên UI (qua MudSnackbar), còn `Data` chứa kết quả thực tế nếu thành công, giúp viết code frontend cực kỳ gọn gàng.

---

## Mảng 4: Cách truyền tham số (Query String & Route Data)
* **Mục tiêu**: Hiểu cách truyền các bộ lọc dữ liệu lên Server.
* **Câu hỏi & Trả lời**:
  * **Hỏi**: 8. "Trong code của `CustomerClientService.cs`, tôi thấy có đoạn nối chuỗi URL thế này: `var url = $"api/customers?Keyword=...&PageNumber=1&PageSize=10";`. Phần phía sau dấu chấm hỏi (?) gọi là Query String đúng không? Nó hoạt động thế nào trong HTTP GET request?"
    * **Trả lời**:
      * Đúng, phần sau dấu `?` là **Query String**. Các cặp key-value được liên kết với nhau bằng dấu `&`.
      * Trong HTTP GET, request không có phần body dữ liệu, nên toàn bộ tham số tìm kiếm, phân trang bắt buộc phải đính trực tiếp lên URL thông qua Query String để Server đọc và xử lý lọc dữ liệu.
  * **Hỏi**: 9. "Tại sao khi gán `Keyword` vào Query String, code lại phải dùng hàm `Uri.EscapeDataString(request.Keyword)`? (Ví dụ: Nếu người dùng nhập tìm kiếm là Laptop & PC, chữ & sẽ gây lỗi gì nếu không có Escape?)."
    * **Trả lời**:
      * Hàm `Uri.EscapeDataString()` mã hóa các ký tự đặc biệt sang định dạng URL (ví dụ: chữ `&` biến thành `%26`).
      * **Nếu không có**: Chữ `&` trong "Laptop & PC" sẽ bị trình duyệt hiểu nhầm là dấu nối sang tham số Query String tiếp theo (lọc tham số tên là " PC"), làm sai lệch hoàn toàn tham số tìm kiếm gửi lên Server.

---

## Mảng 5: Quản lý HttpClient ở Frontend (Cực kỳ quan trọng)
* **Mục tiêu**: Hiểu cách cấu hình kết nối mạng.
* **Câu hỏi & Trả lời**:
  * **Hỏi**: 10. "Trong file `Program.cs` of Client, tôi thấy đăng ký API như sau: `builder.Services.AddHttpClient("HushStoreAPI", client => { client.BaseAddress = new Uri("https://localhost:7010"); });`. Khái niệm 'Base Address' (Địa chỉ cơ sở) là gì? Tại sao tôi phải cấu hình nó ở đây thay vì ghi chết địa chỉ `https://localhost:7010` vào từng file Service lẻ tẻ?"
    * **Trả lời**:
      * **Base Address** là địa chỉ IP/Domain gốc của Server API Backend.
      * Cấu hình tập trung giúp ta dễ dàng thay đổi môi trường chạy (từ chạy thử nghiệm localhost sang triển khai server thật) chỉ cần đổi 1 nơi duy nhất ở `appsettings.json` thay vì đi sửa thủ công ở hàng chục service.
  * **Hỏi**: 11. "Cũng trong file `Program.cs`, tôi thấy dùng `IHttpClientFactory` để tạo client. Tại sao trong các project .NET, lập trình viên không đơn giản dùng lệnh `var client = new HttpClient();` ở mọi nơi cần gọi API cho lẹ? Việc khởi tạo bừa bãi `new HttpClient()` sẽ dẫn đến lỗi nghiêm trọng gì liên quan đến cổng mạng (Socket Exhaustion)?"
    * **Trả lời**:
      * Mặc dù class `HttpClient` triển khai `IDisposable`, việc giải phóng nó sau mỗi lần dùng không đóng cổng socket kết nối mạng ngay lập tức, hệ điều hành sẽ giữ nó ở trạng thái `TIME_WAIT` vài phút.
      * **Socket Exhaustion (Cạn kiệt cổng mạng)**: Nếu `new HttpClient()` bừa bãi dưới tải cao, các socket sẽ bị dùng hết, khiến server/client treo cứng và không thể tạo thêm bất kỳ kết nối mạng nào khác.
      * Dùng `IHttpClientFactory` giúp quản lý tập trung và tái sử dụng các socket kết nối cũ một cách thông minh và an toàn.

---

## Mảng 6: Bảo vệ API (Bảo mật & Phân quyền)
* **Mục tiêu**: Cơ chế khóa cửa bảo mật đầu API Backend.
* **Câu hỏi & Trả lời**:
  * **Hỏi**: 12. "Trong Backend API, tôi thấy có thuộc tính `[Authorize]` đặt trên đầu các Controller hoặc hàm. Thuộc tính này đóng vai trò gì? Điều gì xảy ra nếu Client gọi một API có `[Authorize]` mà không gửi kèm Token trong Header?"
    * **Trả lời**:
      * Thuộc tính `[Authorize]` đóng vai trò bảo vệ endpoint, yêu cầu request gửi đến phải đính kèm JWT Token hợp lệ trong Authorization Header.
      * Nếu không gửi kèm Token hợp lệ, Server API lập tức chặn cuộc gọi và trả về HTTP Status Code **401 Unauthorized**.
  * **Hỏi**: 13. "Nếu tôi muốn một API chỉ dành riêng cho Admin (ví dụ: API xóa người dùng), tôi dùng `[Authorize(Roles = "Admin")]`. Backend sẽ làm cách nào để biết Token mà Client gửi lên có chứa Role là 'Admin' hay không? (Gợi ý: Trả lời về việc đọc Claims từ Payload của JWT)."
    * **Trả lời**:
      * Khi nhận request, Middleware Authentication của ASP.NET Core sử dụng Secret Key giải mã chữ ký Token.
      * Nếu chữ ký hợp lệ, nó đọc phần Payload để trích xuất các thông tin (Claims).
      * Nó kiểm tra xem trong danh sách Claims của user đó có claim nào mang type Role mang giá trị là "Admin" hay không. Nếu có thì cho phép thực thi, nếu không có sẽ trả về **403 Forbidden** (Đã login nhưng không đủ quyền hạn).

---

## Mảng 7: Hiệu năng & Tối ưu (Performance & Caching)
* **Mục tiêu**: Làm cho API hoạt động nhanh hơn.
* **Câu hỏi & Trả lời**:
  * **Hỏi**: 14. "Tại sao không nên gọi API quá nhiều lần cho cùng một dữ liệu không bao giờ thay đổi (ví dụ: danh sách các tỉnh/thành phố)? Khái niệm 'Caching' (bộ nhớ đệm) ở Frontend hoặc Backend giúp giải quyết vấn đề này như thế nào?"
    * **Trả lời**:
      * Gọi liên tục gây tốn tài nguyên mạng và CPU xử lý của Server không cần thiết.
      * **Caching**: Lưu trữ kết quả của lần gọi đầu tiên vào bộ nhớ tạm (LocalStorage hoặc MemoryCache). Lần yêu cầu tiếp theo, ta đọc trực tiếp từ cache ra hiển thị ngay lập tức, tốc độ phản hồi gần như bằng 0.
  * **Hỏi**: 15. "Pagination (Phân trang) là gì? Trong các API lấy danh sách (như `GetPagedOrdersAsync`), tại sao tôi phải truyền `PageIndex` và `PageSize` thay vì lấy toàn bộ 1 triệu đơn hàng về Client rồi mới chia trang trên giao diện? Phân trang ở Backend giúp tiết kiệm những tài nguyên gì (băng thông mạng, RAM của cả Client và Server)?"
    * **Trả lời**:
      * **Phân trang** là việc chia nhỏ danh sách kết quả trả về thành các trang nhỏ (ví dụ mỗi trang 10 sản phẩm).
      * **Tiết kiệm tài nguyên**:
        * *Băng thông mạng*: Chỉ truyền vài KB của 10 đơn hàng thay vì hàng trăm MB của 1 triệu đơn hàng.
        * *RAM Server*: Chỉ query và nạp vào bộ nhớ đúng 10 dòng từ DB.
        * *RAM Client*: Trình duyệt không bị quá tải và crash vì phải hiển thị quá nhiều phần tử HTML cùng lúc.

---

## Mảng 8: Quản lý & Bảo trì API (Versioning & Documentation)
* **Mục tiêu**: Tư duy phát triển dự án thực tế.
* **Câu hỏi & Trả lời**:
  * **Hỏi**: 16. "Khái niệm 'API Versioning' (Quản lý phiên bản API) là gì? Nếu hôm nay tôi có API `api/v1/products`, ngày mai tôi muốn thay đổi cấu trúc dữ liệu trả về nhưng không muốn làm hỏng các App đang xài bản cũ, thì tôi sẽ phải thiết kế URL như thế nào?"
    * **Trả lời**:
      * **API Versioning** là cơ chế định rõ phiên bản của API để quản lý các thay đổi cấu trúc dữ liệu nâng cấp theo thời gian.
      * **Thiết kế URL**: Ta tạo ra endpoint mới mang tên `api/v2/products` phục vụ cấu trúc mới. Các ứng dụng cũ vẫn tiếp tục gọi `api/v1/products` bình thường mà không bị crash (Backward Compatibility).
  * **Hỏi**: 17. "Swagger (hoặc OpenAPI) là gì? Trong đồ án ASP.NET Core Web API, tại sao người ta lại hay cài Swagger? Nó giúp ích gì cho lập trình viên Frontend khi họ cần biết Backend có những API nào, nhận tham số gì và trả về cái gì?"
    * **Trả lời**:
      * **Swagger** là thư viện tự động tạo trang tài liệu tương tác trực quan cho API từ mã nguồn C#.
      * Frontend dev nhìn vào trang Swagger sẽ biết toàn bộ danh sách API hiện có, các tham số đầu vào yêu cầu kiểu dữ liệu gì (JSON, Query string) và mẫu dữ liệu JSON trả về ra sao, thậm chí có thể bấm test gửi dữ liệu thật trực tiếp trên web Swagger mà không cần dùng tool Postman.

---

## Mảng 9: Xử lý lỗi (Error Handling)
* **Mục tiêu**: Báo lỗi chuyên nghiệp để Client xử lý tốt.
* **Câu hỏi & Trả lời**:
  * **Hỏi**: 18. "HTTP Status Code (Mã trạng thái HTTP) là gì? Hãy phân loại ý nghĩa của các nhóm mã: 2xx (ví dụ: 200, 201), 4xx (ví dụ: 400, 401, 403, 404), và 5xx (ví dụ: 500). Việc Backend trả về đúng Status Code quan trọng như thế nào đối với việc xử lý lỗi ở Frontend?"
    * **Trả lời**:
      * **HTTP Status Code** là các mã số chuẩn hóa được Server gửi về để báo tình trạng xử lý yêu cầu.
      * **Ý nghĩa các nhóm**:
        * **2xx (Thành công)**: `200 OK`, `201 Created`.
        * **4xx (Lỗi do Client gửi sai)**: `400 Bad Request` (sai định dạng), `401 Unauthorized` (chưa đăng nhập), `403 Forbidden` (không đủ quyền), `404 Not Found` (không tìm thấy thực thể).
        * **5xx (Lỗi do Server crash)**: `500 Internal Server Error`.
      * **Tầm quan trọng**: Giúp Client bắt lỗi tự động chính xác để điều hướng giao diện phù hợp (Ví dụ: gặp 401 thì tự chuyển hướng sang trang Login, gặp 403 thì thông báo "Không đủ quyền truy cập").

---

# Xử lý bất đồng bộ

## Mảng 1: Khái niệm cốt lõi - Đừng bắt ai phải chờ đợi
* **Mục tiêu**: Hiểu tại sao cần lập trình bất đồng bộ.
* **Câu hỏi & Trả lời**:
  * **Hỏi**: "Trong lập trình, Lập trình đồng bộ (Synchronous) và Bất đồng bộ (Asynchronous) khác nhau thế nào? Hãy giải thích cho tôi bằng ví dụ thực tế về một người phục vụ bàn trong quán cà phê: lúc nhận order và lúc đứng chờ pha chế."
    * **Trả lời**:
      * **Đồng bộ (Sync)**: Thực thi tuần tự từng dòng code. Dòng trước phải chạy xong hoàn toàn thì dòng sau mới bắt đầu.
      * **Bất đồng bộ (Async)**: Cho phép chạy tác vụ ngầm tốn thời gian mà không khóa cứng luồng xử lý chính.
      * **Ví dụ quán cafe**:
        * *Đồng bộ*: Người bồi bàn nhận order của bạn, đi vào quầy pha chế, đứng im nhìn ly cafe nhỏ giọt 5 phút. Trong 5 phút đó, quán có khách hàng mới vào cũng không ai được phục vụ vì người bồi bàn đang bận đứng chờ.
        * *Bất đồng bộ*: Người bồi bàn nhận order của bạn, đưa giấy vào quầy pha chế, quay ra tiếp nhận order bàn tiếp theo. Khi quầy pha chế làm xong cafe và rung chuông, người bồi bàn quay lại bưng nước ra cho bạn.
  * **Hỏi**: "Khi lập trình C#, những tác vụ nào được gọi là 'I/O Bound' (Ví dụ: Gọi API qua mạng, Truy vấn Database, Đọc ghi file)? Tại sao đối với các tác vụ I/O Bound này, việc dùng Bất đồng bộ lại **BẮT BUỘC** phải làm để tối ưu hệ thống?"
    * **Trả lời**:
      * **I/O Bound** là các tác vụ phải chờ thiết bị phần cứng bên ngoài phản hồi (ổ đĩa đọc ghi, SQL Server xử lý câu query, mạng Internet truyền dữ liệu), không tiêu tốn tài nguyên tính toán của CPU Server.
      * **Tại sao bắt buộc**: Nếu dùng đồng bộ, mỗi request gửi lên sẽ khóa cứng 1 luồng của Server để đứng im chờ đợi DB. Khi có hàng ngàn người truy cập cùng lúc, Server sẽ cạn sạch luồng trống để nhận request mới (Thread Pool Starvation) gây sập hệ thống mặc dù CPU Server vẫn cực kỳ rảnh.

---

## Mảng 2: "Thần chú" C# - Task, async, await
* **Mục tiêu**: Hiểu ý nghĩa các từ khóa lặp lại liên tục.
* **Câu hỏi & Trả lời**:
  * **Hỏi**: 3. "Trong C#, `Task` và `Task<T>` là gì? Nó có giống như một cái 'Lời hứa' (Promise) rằng trong tương lai sẽ có một kết quả trả về không?"
    * **Trả lời**:
      * `Task` đại diện cho một hoạt động bất đồng bộ không trả về kết quả (tương tự kiểu `void`).
      * `Task<T>` đại diện cho hoạt động bất đồng bộ sẽ trả về kết quả có kiểu dữ liệu `T` trong tương lai.
      * Đúng, nó tương tự như một "Lời hứa" (Promise): "Tôi hứa sẽ thực thi việc này dưới nền, khi có kết quả tôi sẽ trả lại cho bạn".
  * **Hỏi**: 4. "Từ khóa `async` và `await` đi cặp với nhau như thế nào? Cụ thể, khi dòng code C# chạy đến chữ `await` (ví dụ: `await _httpClient.GetFromJsonAsync(...)`), luồng thực thi (thread) sẽ làm gì tiếp theo? Nó đứng đó chờ chết cứng, hay nó đi làm việc khác rồi quay lại sau?"
    * **Trả lời**:
      * Từ khóa `async` khai báo trên đầu hàm để cho phép sử dụng từ khóa `await` bên trong hàm đó.
      * Khi chạy đến chữ `await`: Luồng xử lý hiện tại sẽ **ngay lập tức được giải phóng** quay lại Thread Pool của hệ thống để thực hiện các request khác. Khi tác vụ ngầm chạy xong, hệ thống phân bổ một luồng trống bất kỳ quay lại chạy tiếp phần code nằm dưới chữ `await`.

---

## Mảng 3: Ứng dụng ở Frontend (Blazor WebAssembly) - Cứu rỗi giao diện
* **Mục tiêu**: Hiểu tầm quan trọng của Async đối với giao diện Web.
* **Câu hỏi & Trả lời**:
  * **Hỏi**: 5. "Trong file `ProductClientService.cs` của đồ án, tôi gọi API bằng lệnh `await _httpClient.GetFromJsonAsync(...)`. Trong Blazor WebAssembly (chạy trên trình duyệt của 1 user), nếu tôi cố tình không dùng `await` mà bắt luồng chính phải chờ (Block thread), thì chuyện gì sẽ xảy ra với giao diện web lúc người dùng đang bấm nút hoặc cuộn trang?"
    * **Trả lời**:
      * Blazor WASM chạy đơn luồng trên trình duyệt.
      * Nếu không dùng `await` mà block luồng chính (gọi `.Result`), toàn bộ giao diện Web của người dùng sẽ bị **treo cứng (Freeze) hoàn toàn**. Họ không thể click chuột, không thể cuộn trang, màn hình bị đơ đần và trình duyệt sẽ hiện cảnh báo "Trang web không phản hồi".
  * **Hỏi**: 6. "Ở các màn hình giao diện (ví dụ màn hình Load danh sách sản phẩm), tôi thấy thường có một biến `_isLoading = true;` bật lên trước khi gọi hàm await, và tắt đi (`_isLoading = false;`) ở khối `finally`. Tại sao phải làm vậy? Điều này mang lại trải nghiệm gì cho người dùng (UX)?"
    * **Trả lời**:
      * Bật `_isLoading = true` để hiện biểu tượng Loading Spinner báo hiệu hệ thống đang xử lý, đồng thời khóa các nút bấm lại chống bấm đúp (double-click) đơn hàng.
      * Đặt `_isLoading = false` trong khối `finally` đảm bảo biến này chắc chắn được reset về `false` kể cả khi gọi API thành công hay gặp lỗi. Giúp trải nghiệm người dùng mượt mà và trực quan.

---

## Mảng 4: Ứng dụng ở Backend (ASP.NET Core & EF Core) - Giải phóng Server
* **Mục tiêu**: Hiểu hiệu quả chịu tải của Server khi dùng Async.
* **Câu hỏi & Trả lời**:
  * **Hỏi**: 7. "Ở tầng Backend, trong file `ProductRepository.cs`, tôi dùng lệnh của Entity Framework Core là `await query.ToListAsync()` để chọc xuống SQL Server. Trong thời gian chờ SQL Server trả data về (có thể mất vài mili-giây), 'Thread Pool' (Hồ chứa luồng) của ASP.NET Core sẽ làm gì? Tại sao việc dùng `await` ở Backend lại giúp Server chịu tải được nhiều user hơn (Scalability)?"
    * **Trả lời**:
      * Thread Pool của ASP.NET Core sẽ rút luồng đó về để xử lý các request HTTP khác của những user khác gửi tới.
      * Giúp Server chỉ cần một lượng nhỏ luồng vẫn có thể xử lý đồng thời hàng chục ngàn lượt truy cập cùng lúc (tăng khả năng Scalability).
  * **Hỏi**: 8. "Cũng ở Backend, nếu hàm Repository của tôi dùng `.ToList()` (Đồng bộ) thay vì `.ToListAsync()` (Bất đồng bộ), điều gì sẽ xảy ra với CPU và RAM của Server nếu có 10,000 người cùng F5 trang web một lúc?"
    * **Trả lời**:
      * 10,000 request sẽ chiếm dụng cứng 10,000 luồng để đứng chờ DB.
      * Server sẽ cạn kiệt luồng trống (Thread Starvation), CPU quá tải vì chuyển đổi luồng liên tục, RAM tăng vọt để duy trì các luồng dẫn đến sập toàn bộ Web Server.

---

## Mảng 5: Bắt bẻ lỗi "bị đần" khi dùng Async
* **Mục tiêu**: Tránh các lỗi cơ bản khi lập trình bất đồng bộ.
* **Câu hỏi & Trả lời**:
  * **Hỏi**: 9. "Lỗi 'Quên await' (Fire and forget) là gì? Trong đồ án, nếu tôi gọi hàm `AuthService.LoginAsync(_loginRequest)` mà vô tình xóa mất chữ `await` ở đầu, thì biến `result` sẽ nhận giá trị gì? Dòng code bên dưới nó có chờ đăng nhập xong rồi mới chạy tiếp không?"
    * **Trả lời**:
      * Biến `result` sẽ nhận về kiểu dữ liệu là `Task<ApiResult<TokenResponse>>` chứ không phải là dữ liệu `ApiResult` mong muốn.
      * Dòng code bên dưới sẽ **chạy tiếp ngay lập tức** mà không thèm chờ kết quả đăng nhập, dẫn đến các lỗi logic dữ liệu chưa được khởi tạo.
  * **Hỏi**: 10. "Tại sao các chuyên gia .NET lại khuyên 'Tuyệt đối không dùng async void' (trừ trường hợp làm event handler như click nút)? Việc dùng `async void` thay vì `async Task` sẽ gây ra thảm họa gì khi ứng dụng văng lỗi (Exception)?"
    * **Trả lời**:
      * Vì khi một hàm `async void` xảy ra lỗi (Exception), Exception đó sẽ không được quản lý bởi đối tượng `Task` nào, dẫn đến khối `try-catch` bên ngoài không thể bắt được lỗi.
      * Lỗi này sẽ văng thẳng lên luồng chính của hệ thống và lập tức **làm crash (sập) toàn bộ ứng dụng** ngay lập tức.

---

## Mảng 6: Chạy đua với thời gian (Task.WhenAll)
* **Mục tiêu**: Tối ưu hóa hiệu năng khi gọi nhiều API/Query.
* **Câu hỏi & Trả lời**:
  * **Hỏi**: 11. "Trong lập trình, hãy phân biệt giữa Xử lý đồng thời (Concurrency) và Xử lý song song (Parallelism). Từ khóa async/await thuộc về nhóm nào?"
    * **Trả lời**:
      * **Concurrency (Đồng thời)**: Quản lý nhiều công việc tại 1 thời điểm bằng cách chuyển đổi qua lại nhanh chóng giữa chúng (giống như bồi bàn phục vụ nhiều bàn khách). Async/await thuộc nhóm này.
      * **Parallelism (Song song)**: Thực thi vật lý nhiều công việc cùng 1 thời điểm trên các nhân CPU khác nhau.
  * **Hỏi**: 12. "Giả sử trang Home của đồ án cần gọi 3 API độc lập: Lấy danh sách Laptop (tốn 1s), Lấy danh sách PC (tốn 1s), và Lấy danh sách Category (tốn 1s). Nếu tôi viết 3 dòng await liên tiếp nhau thì mất tổng cộng 3 giây. Hàm `Task.WhenAll()` trong C# là gì và nó giúp tôi gom 3 việc này lại chạy cùng lúc để tổng thời gian chỉ còn 1 giây như thế nào?"
    * **Trả lời**:
      * `Task.WhenAll()` nhận vào một danh sách các Task và chỉ chạy await khi toàn bộ các Task đó đã hoàn thành.
      * **Cách làm**:
        * Gọi cả 3 Task chạy ngầm mà không có từ khóa `await` đứng đầu (3 Task chạy đồng thời dưới nền).
        * Chạy lệnh `await Task.WhenAll(laptopTask, pcTask, catTask);`.
        * Lúc này cả 3 API được gọi song song, tổng thời gian chờ đợi giảm từ 3 giây xuống còn đúng 1 giây (bằng thời gian của Task chạy lâu nhất).

---

## Mảng 7: Hố tử thần - Khóa chéo (Deadlock)
* **Mục tiêu**: Hiểu lý do tránh pha trộn code đồng bộ và bất đồng bộ.
* **Câu hỏi & Trả lời**:
  * **Hỏi**: 13. "Lỗi Deadlock (khóa chéo) là gì? Nếu trong một hàm bình thường (không có chữ async), tôi cố tình gọi một hàm bất đồng bộ bằng cách thêm chữ `.Result` hoặc `.Wait()` ở cuối (ví dụ: `_httpClient.GetFromJsonAsync(...).Result`), tại sao ứng dụng của tôi lại dễ bị treo (đơ cứng) hoàn toàn?"
    * **Trả lời**:
      * **Deadlock** là hiện tượng các tiến trình chờ khóa giải phóng của nhau vĩnh viễn khiến ứng dụng treo cứng.
      * **Tại sao dùng `.Result` gây Deadlock**:
        * Luồng chính (chạy đồng bộ) gọi `.Result` sẽ bị khóa cứng để đứng chờ Task hoàn thành.
        * Tuy nhiên, khi Task bất đồng bộ chạy xong dưới nền, nó cần quay lại luồng chính ban đầu để chạy nốt phần logic còn lại.
        * Nhưng luồng chính lúc này đang bị khóa cứng bởi lệnh `.Result`.
        * Hai bên chờ đợi lẫn nhau vĩnh viễn gây đơ ứng dụng.

---

## Mảng 8: Phanh gấp giữa chừng (CancellationToken)
* **Mục tiêu**: Tiết kiệm tài nguyên khi người dùng hủy thao tác.
* **Câu hỏi & Trả lời**:
  * **Hỏi**: 14. "Trong C#, `CancellationToken` là gì? Tôi thấy ở Frontend (ví dụ trong component MudTable của đồ án), hàm `ServerReload` có truyền vào tham số `CancellationToken cancellationToken`. Nó dùng để làm gì?"
    * **Trả lời**:
      * `CancellationToken` là đối tượng dùng để truyền tín hiệu hủy bỏ (Cancellation) yêu cầu thực thi của tác vụ bất đồng bộ đang chạy ngầm khi không còn cần thiết.
  * **Hỏi**: 15. "Hãy tưởng tượng người dùng bấm vào trang 'Danh sách Đơn hàng', Blazor gọi API và Backend đang chọc xuống SQL Server để lấy 1 triệu dòng dữ liệu. Ngay lúc đó, người dùng mất kiên nhẫn và bấm chuyển sang trang 'Trang chủ'. `CancellationToken` sẽ truyền tín hiệu 'Hủy bỏ' từ trình duyệt, qua API, xuống tận SQL Server để bắt nó dừng lệnh Query lại ngay lập tức như thế nào để tiết kiệm RAM và CPU?"
    * **Trả lời**:
      * Khi người dùng chuyển trang, trình duyệt đóng kết nối HTTP cũ.
      * HttpClient của Blazor lập tức kích hoạt tín hiệu hủy trên `CancellationToken` (Dòng 59 của `AuthHeaderHandler.cs` kiểm tra hủy).
      * Tín hiệu truyền lên Web API qua tham số `cancellationToken` ở Controller.
      * API chuyển tiếp tín hiệu hủy này xuống EF Core qua hàm `ToListAsync(cancellationToken)`.
      * EF Core gửi lệnh ngắt kết nối/hủy query xuống SQL Server. SQL Server lập tức dừng việc quét tìm dữ liệu. Toàn bộ tài nguyên CPU/RAM được giải phóng tức khắc.
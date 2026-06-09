# So sánh Sơ đồ Sequence Diagram và Mã nguồn Dự án (Checkout & Thanh toán)

Tài liệu này phân tích mối tương quan giữa sơ đồ Sequence Diagram (đặt hàng & thanh toán) và mã nguồn thực tế của dự án, đồng thời chỉ ra các điểm khác biệt quan trọng về mặt logic và kiến trúc.

---

## 1. Mối quan hệ giữa các mũi tên (Messages) trong sơ đồ và Code

Dưới đây là bảng đối chiếu chi tiết các bước trong sơ đồ Sequence Diagram với các thành phần Class, Method và API tương ứng trong mã nguồn dự án:

| Bước trong Sơ đồ | Tên mũi tên (Sơ đồ) | Thành phần Code tương ứng | Mô tả kỹ thuật trong Code |
| :--- | :--- | :--- | :--- |
| **1** | `ChonSanPhamVaNhapDatHang()` | Client UI | Khách hàng chọn mua sản phẩm từ Giỏ hàng hoặc chọn "Mua ngay" tại trang Chi tiết sản phẩm. |
| **2** | `LayThongTinDonHang(danhSachSanPham)` | `CartController.GetMyCart()` | API nhận yêu cầu lấy thông tin giỏ hàng hiện tại của khách hàng. |
| **3** | `XuLyThongTinDonHang(danhSachSanPham)` | `CartService.GetMyCartAsync(userId)` | Service xử lý tính toán tiền tạm tính, lấy thông tin hình ảnh, tên và giá của từng sản phẩm. |
| **4 & 5** | `TruyVanChiTietSanPham(danhSachId)` <br> $\rightarrow$ `danhSachChiTietSanPham` | `ICartRepository.GetCartItemsByUserAsync(userId)` | Query cơ sở dữ liệu để lấy danh sách sản phẩm trong giỏ kèm thông tin Variant liên kết. |
| **6** | `thongTinDonHangTongHop` | `CartResponse` | DTO chứa danh sách chi tiết các mặt hàng kèm `TotalAmount` (tổng tiền giỏ hàng). |
| **7 & 8** | `HienThiDuLieuDonHang` <br> $\rightarrow$ `HienThiGiaoDienDonHang()` | Client UI | Trả về kết quả JSON dạng `ApiResult<CartResponse>` để hiển thị giao diện thanh toán. |
| **Alt: 3c** | `NhanHuyBo()` <br> $\rightarrow$ `XoaDuLieuTamThoi()` | Không có API tương ứng | Do dữ liệu giỏ hàng được quản lý động ở Client, việc "Hủy bỏ" lúc này chỉ là chuyển trang trên UI, không cần gọi API để xóa dữ liệu tạm thời. |
| **Opt: 3a** | `ThayDoiThongTinNhanHang(...)` | Client UI | Cập nhật state (địa chỉ nhận) cục bộ tại Client, lưu trữ vào biến tạm trước khi gửi request Checkout. |
| **Opt: 3b** | `ThayDoiPhuongThucThanhToan(...)` | Client UI | Cập nhật phương thức thanh toán (COD hoặc Online) tại state Client. |
| **9** | `NhanXacNhanDatHang()` | Client UI | Khách hàng nhấn nút "Xác nhận đặt hàng". |
| **10** | `GuiYeuCauTaoDonHang(...)` | `OrdersController.Checkout(CheckoutRequest request)` | API POST `/api/orders/checkout` nhận thông tin địa chỉ, ghi chú, mã voucher, phương thức thanh toán. |
| **11** | `KiemTraTonKhoVaTaoDonHang(...)` | `OrderService.CheckoutAsync(...)` | Service xử lý đặt hàng chính, thực hiện kiểm tra nghiệp vụ và mở Transaction để ghi dữ liệu đơn hàng. |
| **12 & 13** | `KiemTraSoLuongTonKho(...)` <br> $\rightarrow$ `ketQuaTonKho` | `_productSerialRepo.CountAvailableByVariantIdsAsync` <br> và `_orderRepo.GetActiveOrderQuantitiesByVariantIdsAsync` | Kiểm tra tồn kho ảo khả dụng bằng cách lấy số lượng Serial sẵn có trừ đi số lượng đang bị giữ chỗ trong các đơn hàng chưa hoàn thành. |
| **Alt: 5a** | `TraVeLoi("Sản phẩm không đủ số lượng")` | `ApiResult<CheckoutResponse>.Fail(...)` | Nếu tồn kho ảo không đủ, hệ thống ném lỗi ngay lập tức, trả về HTTP 400 BadRequest cho UI. |
| **Thanh toán trực tuyến** | `TraVeUrlThanhToan()` | `CheckoutResponse.PaymentUrl = null` | **[Chưa tích hợp]** Hiện tại thuộc tính `PaymentUrl` đang được hardcode là `null` kèm ghi chú tích hợp sau. |
| **Thanh toán COD** | `LuuDonHangCOD(...)` <br> $\rightarrow$ `luuThanhCong` | `_orderRepo.AddAsync(order)` <br> & `_unitOfWork.SaveChangesAsync()` | Lưu thông tin đơn hàng mới với trạng thái `Status = Pending` (Chờ duyệt) và `PaymentStatus = 0` (Chưa thanh toán). |
| **Thanh toán COD** | `CapNhatTonKho()` <br> $\rightarrow$ `capNhatThanhCong` | `InventoryExportService.ExportOrderAsync(...)` | **[Khác biệt luồng]** Việc cập nhật kho vật lý không diễn ra lúc checkout mà diễn ra sau đó khi nhân viên thực hiện xuất kho đơn hàng. |
| **Thanh toán trực tuyến** | `ThucHienGiaoDichTrucTuyen()` <br> $\rightarrow$ `ketQuaGiaoDich` | Cổng thanh toán (VNPay/MoMo) | **[Chưa tích hợp]** Luồng tương tác trực tiếp với cổng thanh toán và khách hàng chưa được code xử lý. |
| **Thanh toán trực tuyến** | `QuayLaiTrangKetQua(...)` <br> $\rightarrow$ `XacNhanKetQuaThanhToan(...)` | Không có API tương ứng | Chưa có API nhận Callback/IPN hoặc Redirect URL từ Cổng thanh toán để xử lý kết quả giao dịch. |

---

## 2. Những điểm khác biệt quan trọng giữa Sơ đồ và Code

Qua đối chiếu mã nguồn và sơ đồ, có 4 điểm khác biệt cốt lõi dưới đây:

### 1. Tích hợp Cổng thanh toán trực tuyến (Payment Gateway)
* **Trong sơ đồ:** Mô tả một quy trình hoàn chỉnh kết nối với Cổng Thanh Toán trực tuyến. Khách hàng thực hiện giao dịch trực tiếp trên cổng thanh toán, sau đó chuyển hướng về UI $\rightarrow$ gọi API `XacNhanKetQuaThanhToan(ketQua)` để xử lý nghiệp vụ.
* **Trong thực tế Code:** Luồng thanh toán trực tuyến **chưa được triển khai**.
  * Trong `OrderService.CheckoutAsync` (dòng 193):
    ```csharp
    PaymentUrl = null // Link thanh toán VNPay/Momo sẽ được tích hợp sau.
    ```
  * Không hề có API Controller hay Service nào trong dự án chịu trách nhiệm nhận Callback/IPN từ VNPay hay MoMo để xác nhận giao dịch thành công/thất bại (`XacNhanKetQuaThanhToan` / `XuLyKetQuaThanhToan`).

### 2. Thời điểm lưu đơn hàng (Order Creation Timing)
* **Trong sơ đồ:** Đối với trường hợp thanh toán trực tuyến, đơn hàng chỉ được lưu vào database (`LuuDonHangTrucTuyen`) **sau khi** giao dịch trực tuyến thành công (`Giao dịch thành công`).
* **Trong thực tế Code:** Đơn hàng được tạo và lưu trữ ngay lập tức vào database ở trạng thái `Pending` (Chờ duyệt) trước khi có bất kỳ cổng thanh toán nào được gọi. Nếu tích hợp thanh toán online sau này, hệ thống sẽ phải tạo đơn trước, lấy `OrderId` để làm mã thanh toán gửi sang cổng thanh toán, sau đó mới cập nhật trạng thái thanh toán từ `Chưa thanh toán` sang `Đã thanh toán` khi nhận được callback thành công.

### 3. Cơ chế Cập nhật Tồn kho (Inventory Update Strategy)
* **Trong sơ đồ:** Mô tả hành động `CapNhatTonKho()` diễn ra **ngay lập tức** và tự động trong luồng đặt hàng thành công (cho cả COD và Online).
* **Trong thực tế Code:** Quá trình trừ kho vật lý được tách biệt hoàn toàn khỏi luồng Đặt hàng:
  * Khi khách hàng đặt hàng (checkout), hệ thống chỉ kiểm tra tồn kho ảo và giữ chỗ (`reservedInOrders`). Tồn kho vật lý của biến thể sản phẩm (`StockQuantity`) vẫn giữ nguyên.
  * Việc trừ kho vật lý và gán số Serial thực tế cho đơn hàng chỉ diễn ra khi thủ kho/admin thực hiện hành động **Xuất kho** thông qua `InventoryExportService.ExportOrderAsync` (khi đơn hàng ở trạng thái `Confirmed`). Lúc này, trạng thái của ProductSerial mới chuyển thành `Sold` và số lượng tồn kho của Variant mới được cập nhật thông qua `_inventorySyncService.SyncStockBatchAsync`.

### 4. Quản lý trạng thái và dữ liệu tạm thời
* **Trong sơ đồ:** Có bước `XoaDuLieuTamThoi()` khi khách hàng nhấn hủy bỏ tiến trình đặt hàng.
* **Trong thực tế Code:** Không có khái niệm dữ liệu tạm thời (Draft Order) được lưu trên Database trong suốt tiến trình chuẩn bị checkout của Khách hàng. Giỏ hàng nằm sẵn trong Database, chỉ khi đặt hàng thành công mới thực hiện xóa giỏ hàng (`_cartRepo.RemoveRange(cartsToRemove)`). Nếu hủy bỏ giữa chừng, giỏ hàng vẫn giữ nguyên và không có thay đổi nào cần rollback trên server.

---

## 3. Đánh giá & Khuyến nghị cho Đồ án/Bảo vệ môn học

* **Giải trình về thanh toán trực tuyến:** Khi bảo vệ đồ án trước hội đồng, bạn nên trình bày rõ rằng luồng thanh toán qua VNPay/Momo đang ở dạng cấu trúc chờ (placeholder) và sẽ được tích hợp thông qua một thư viện dịch vụ độc lập ở giai đoạn tiếp theo của dự án.
* **Giải trình về quản lý kho:** Cơ chế trừ kho ảo khi đặt hàng (qua việc đếm serial khả dụng và trừ đi lượng đặt giữ chỗ trong đơn `Pending/Confirmed`) là thiết kế tối ưu hơn sơ đồ Sequence. Nó tránh hiện tượng trừ kho vật lý khi đơn hàng chưa thực sự được giao đi, giúp bảo toàn tính chính xác của dữ liệu kho hàng.
* **Đề xuất hiệu chỉnh sơ đồ:** Để sơ đồ Sequence khớp hoàn toàn với mã nguồn, bạn nên gộp các bước cập nhật tồn kho vật lý và gán số Serial vào một sơ đồ Sequence riêng cho luồng **"Xuất kho hàng"** của Nhân viên kho (Thủ kho).

---

# So sánh Sơ đồ Sequence Diagram và Mã nguồn Dự án (Xây dựng Cấu hình PC - Build PC)

Tài liệu này phân tích mối tương quan giữa sơ đồ Sequence Diagram (Xây dựng cấu hình PC) và mã nguồn thực tế của dự án, đồng thời chỉ ra các điểm khác biệt quan trọng về mặt logic và kiến trúc.

---

## 1. Mối quan hệ giữa các mũi tên (Messages) trong sơ đồ và Code

Dưới đây là bảng đối chiếu các bước trong sơ đồ Sequence Diagram Build PC với các thành phần Class, Method và API tương ứng trong mã nguồn dự án:

| Bước trong Sơ đồ | Tên mũi tên (Sơ đồ) | Thành phần Code tương ứng | Mô tả kỹ thuật trong Code |
| :--- | :--- | :--- | :--- |
| **1** | `NhanNutBuildPC()` | Client UI | Khách hàng truy cập vào trang Xây dựng cấu hình PC. |
| **2 & 3** | `LayDanhSachThanhPhan()` <br> $\rightarrow$ `XuLyLayThanhPhan()` | `StorefrontController.GetCategories()` | API lấy danh sách các danh mục sản phẩm đang hoạt động trên hệ thống (để hiện danh sách linh kiện). |
| **4 & 5** | `TruyVanDanhMucLinhKien()` <br> $\rightarrow$ `danhSachThanhPhan` | `IStorefrontService.GetActiveCategoriesAsync()` | Truy vấn database để lấy cây danh mục sản phẩm hoạt động. |
| **6 & 7 & 8** | `duLieuThanhPhan` <br> $\rightarrow$ `traVeGiaoDien` <br> $\rightarrow$ `HienThiGiaoDienBuildPC()` | Client UI | Trả về cấu trúc JSON chứa danh mục linh kiện, UI hiển thị các ô tương ứng (CPU, Mainboard, RAM,...). |
| **Loop: Chọn** | `NhanChonLinhKien(loaiLinhKien)` | Client UI | Khách hàng nhấn chọn tìm kiếm linh kiện cho một ô (Ví dụ: CPU). |
| **Loop: Tương thích** | `LayLinhKienTuongThich(...)` <br> $\rightarrow$ `KiemTraVaLocTuongThich(...)` | `StorefrontController.SearchProducts(...)` | **[Khác biệt logic]** Hệ thống sử dụng chung API tìm kiếm sản phẩm theo category chứ không có API kiểm tra tương thích chuyên biệt. |
| **Loop: Query** | `TruyVanLinhKienPhuHop(...)` | `IStorefrontService.SearchProductsAsync(...)` | Truy vấn các sản phẩm thuộc danh mục linh kiện được chọn từ Database. |
| **Loop: Kết quả** | `hienThiDanhSach` <br> $\rightarrow$ `HienThiDanhSachChoPhapLoc()` | Client UI | Trả về danh sách sản phẩm và hiển thị lên modal/dropdown cho khách hàng chọn. |
| **Opt: Thêm** | `ChonSanPhamVaSoLuong(...)` <br> $\rightarrow$ `CapNhatCauHinhVaTongChiPhi()` | Client UI | Được xử lý hoàn toàn cục bộ trên client (SPA local state), cộng dồn giá sản phẩm vào tổng chi phí của PC. |
| **Opt: 7a** | `NhanXoaSanPham(idSanPham)` <br> $\rightarrow$ `LoaiBoKhoiDanhSachVaCapNhat()` | Client UI | Xóa sản phẩm khỏi cấu hình hiện tại ở client và cập nhật lại tổng tiền trên giao diện. |
| **Opt: 7b** | `NhanLamMoiToanBo()` <br> $\rightarrow$ `XoaTrangDanhSachCauHinh()` | Client UI | Reset trạng thái cấu hình về rỗng cục bộ ở client. |
| **Opt: 7c** | `NhanXuatFileCauHinh()` <br> $\rightarrow$ `YeuCauTaoFileCauHinh(...)` | `BuildPcController.ExportToExcel(...)` | API nhận danh sách linh kiện đã chọn từ Client gửi lên dưới định dạng JSON trong body. |
| **Opt: 7c** | `TaoFileExcelHoacPdf(...)` | `BuildPcService.ExportToExcelAsync(...)` | Sử dụng thư viện EPPlus để sinh file Excel (.xlsx) chứa bảng cấu hình chi tiết và tính tổng tiền, trả về mảng byte. |
| **Opt: 7c** | `fileDuLieu` $\rightarrow$ `traVeFile` <br> $\rightarrow$ `TaiFileVeThietBi()` | `BuildPcClientService.ExportBuildPcAsync(...)` | Client nhận mảng byte từ API, chuyển sang Base64 và kích hoạt hàm javascript `downloadFile` để lưu về máy. |
| **Opt: 7d** | `LuuCauHinhTamThoi(...)` <br> $\rightarrow$ `GhiNhanCauHinhTam(...)` | Không có API tương ứng | **[Chưa tích hợp]** Dự án hiện chưa hỗ trợ lưu cấu hình PC của khách hàng vào database hoặc cookie thông qua API của backend. |
| **Giỏ hàng** | `GuiYeuCauThemVaoGio(...)` | `CartController.AddToCart(...)` | **[Khác biệt luồng]** Không có API thêm hàng loạt. Client sẽ phải gọi API thêm vào giỏ từng sản phẩm một (vòng lặp gọi API). |
| **Giỏ hàng** | `KiemTraTonKhoToanBo(...)` <br> $\rightarrow$ `TruyVanSoLuongTonKho(...)` | `CartService.AddToCartAsync(...)` | Kiểm tra tồn kho được thực hiện độc lập cho từng linh kiện khi gọi API thêm vào giỏ hàng. |
| **Giỏ hàng** | `ThemToanBoVaoGioHang(...)` <br> $\rightarrow$ `luuThanhCong` | `_cartRepo.AddAsync(...)` <br> & `_unitOfWork.SaveChangesAsync()` | Lưu các bản ghi giỏ hàng tương ứng xuống Database cho từng sản phẩm. |

---

## 2. Những điểm khác biệt quan trọng giữa Sơ đồ và Code

### 1. Cơ chế lọc tương thích linh kiện (Compatibility Checking)
* **Trong sơ đồ:** Mô tả một công cụ lọc tương thích chuyên sâu (`KiemTraVaLocTuongThich`), nhận vào loại linh kiện và danh sách đã chọn để lọc ra các linh kiện tương thích từ Database (Ví dụ: Mainboard chỉ đi với CPU hỗ trợ cùng Socket).
* **Trong thực tế Code:** **Chưa có động cơ kiểm tra tính tương thích**. 
  * Cả `BuildPcController` và `BuildPcService` đều không có phương thức hay thuộc tính nào lưu trữ hay so khớp các thông số kỹ thuật (như Socket, TDP, Chuẩn RAM, Công suất nguồn...).
  * Thực tế, khi người dùng click chọn linh kiện, Client chỉ gọi API tìm kiếm sản phẩm thông thường thuộc Category đó (`StorefrontController.SearchProducts`) và trả về toàn bộ sản phẩm thuộc nhóm đó mà không áp dụng bộ lọc tương thích kỹ thuật nào.

### 2. Tính năng Lưu cấu hình tạm thời (Save Draft Build)
* **Trong sơ đồ:** Có luồng `LuuCauHinhTamThoi` lưu thông tin cấu hình của khách hàng vào Cơ sở dữ liệu hoặc Cookie ở phía Backend để giữ trạng thái khi thoát trang.
* **Trong thực tế Code:** Tính năng này **không được triển khai trên Backend**.
  * Không có bất kỳ API lưu cấu hình tạm, cũng như không có bảng Cấu hình PC (`BuildPcConfiguration` hay tương đương) trong Cơ sở dữ liệu.
  * Cấu hình hiện tại của khách hàng chỉ tồn tại trong bộ nhớ tạm thời của trình duyệt (Client State) và bị mất khi tải lại trang, trừ khi Client tự lưu vào LocalStorage (phía client tự quản lý hoàn toàn).

### 3. Tương tác Thêm toàn bộ cấu hình vào giỏ hàng (Batch Add-to-Cart)
* **Trong sơ đồ:** Mô tả hành động gửi toàn bộ cấu hình trong một request duy nhất `GuiYeuCauThemVaoGio(danhSachCauHinh)`. Hệ thống kiểm tra tồn kho tổng thể và thêm hàng loạt sản phẩm vào Database (`ThemToanBoVaoGioHang`).
* **Trong thực tế Code:** **Chưa hỗ trợ API Batch (hàng loạt)**.
  * `CartController` chỉ có duy nhất endpoint `HttpPost` thêm một sản phẩm đơn lẻ (`AddToCart`). 
  * Do đó, để chuyển đổi cấu hình PC vào giỏ hàng, Client buộc phải thực hiện một vòng lặp gọi API tuần tự đối với từng linh kiện trong danh sách. Điều này tăng số lượng request lên máy chủ và không đảm bảo tính toàn vẹn (ví dụ: một số sản phẩm thêm thành công, một số bị lỗi hết hàng giữa chừng).

### 4. Chức năng chính được thực hiện ở Backend
* **Chức năng duy nhất được triển khai ở Backend** của module Build PC là **Xuất File Excel** (`ExportToExcelAsync`). Service sử dụng thư viện `EPPlus` để thiết kế bố cục, điền dữ liệu linh kiện gửi từ client lên, tính tổng tiền, tô màu sọc, định dạng tiền tệ và trả về file Excel để người dùng tải về thiết bị.

---

## 3. Đánh giá & Khuyến nghị cho Đồ án/Bảo vệ môn học

* **Giải trình về nghiệp vụ tương thích:** Trong đồ án, bạn nên lưu ý rằng bộ lọc tương thích kỹ thuật (như check socket CPU với Mainboard) hiện tại đang được xử lý ở mức **nguyên mẫu (Prototype) hoặc quản lý trực tiếp bằng cách phân loại danh mục sản phẩm của cửa hàng**, chưa có cơ chế kiểm tra chéo tự động ở tầng Database.
* **Đồng bộ hóa sơ đồ:** Bạn nên sửa lại sơ đồ để phản ánh đúng thực tế:
  * Loại bỏ các bước `KiemTraVaLocTuongThich` và `GhiNhanCauHinhTam` trên Backend.
  * Sơ đồ hóa luồng thêm vào giỏ bằng một vòng lặp (loop) gọi API `AddToCart` tuần tự trên Client UI thay vì một API hàng loạt.
  * Nhấn mạnh tính năng **Xuất file cấu hình Excel** làm điểm nhấn chính cho phần xử lý nghiệp vụ của Backend đối với module Build PC này.

---

# So sánh Sơ đồ Sequence Diagram và Mã nguồn Dự án (So sánh sản phẩm - Product Comparison)

Tài liệu này phân tích mối tương quan giữa sơ đồ Sequence Diagram (So sánh sản phẩm) và mã nguồn thực tế của dự án, đồng thời chỉ ra các điểm khác biệt quan trọng về mặt logic và kiến trúc.

---

## 1. Mối quan hệ giữa các mũi tên (Messages) trong sơ đồ và Code

Dưới đây là bảng đối chiếu các bước trong sơ đồ Sequence Diagram So sánh sản phẩm với các thành phần Class, Method và API tương ứng trong mã nguồn dự án:

| Bước trong Sơ đồ | Tên mũi tên (Sơ đồ) | Thành phần Code tương ứng | Mô tả thực tế trong Code |
| :--- | :--- | :--- | :--- |
| **1** | `NhanThemVaoSoSanh(idSanPham)` | Client UI | Khách hàng nhấn nút "Thêm vào so sánh" trên thẻ sản phẩm. |
| **2** | `YeuCauThemVaoSoSanh(idSanPham, danhSachTam)` | Không có API tương ứng | **[Khác biệt cốt lõi]** Không có API nhận yêu cầu thêm sản phẩm vào danh sách so sánh tạm thời. |
| **3** | `KiemTraDieuKienSoSanh(...)` | Không có Service tương ứng | Không có logic kiểm tra điều kiện so sánh ở Backend. |
| **4 & 5** | `LayThongTinDanhMuc(idSanPham)` <br> $\rightarrow$ `thongTinDanhMuc` | `IStorefrontService.GetProductDetailAsync(slug)` | Thực tế code chỉ có API lấy chi tiết sản phẩm và danh mục qua slug chứ không lấy trực tiếp danh mục qua Id sản phẩm riêng lẻ phục vụ so sánh. |
| **Alt: 1b** | `TraVeLoi("Tối đa 3 sản phẩm")` <br> $\rightarrow$ `HienThiLoiSoLuong()` | Client UI (Nếu có) | Điều kiện giới hạn số lượng so sánh tối đa (3 sản phẩm) phải được thực hiện hoàn toàn ở phía client (JavaScript/Blazor state) thay vì Backend. |
| **Alt: 1a** | `TraVeLoi("Khác danh mục")` <br> $\rightarrow$ `HienThiLoiDanhMuc()` | Client UI (Nếu có) | Việc so khớp các sản phẩm xem có cùng danh mục hay không cũng phải thực hiện ở Client. |
| **Alt: Đủ điều kiện** | `LuuSanPhamVaoDanhSachTam(...)` <br> $\rightarrow$ `TraVeThanhCong()` | Client LocalStorage / Session | Danh sách sản phẩm so sánh tạm thời không lưu vào Database mà được lưu trữ trong bộ nhớ trình duyệt của người dùng. |
| **Opt: 3a** | `NhanXoaTatCa()` <br> $\rightarrow$ `YeuCauXoaToanBo()` <br> $\rightarrow$ `XoaDanhSachTam()` | Không có API / Service | Hành động làm trống danh sách so sánh được thực hiện ở Client bằng cách clear LocalStorage/Session. |
| **2 sản phẩm trở lên** | `NhanSoSanhNgay()` <br> $\rightarrow$ `LayChiTietSoSanh(danhSachId)` | Không có API tương ứng | Không có API nhận danh sách ID sản phẩm và trả về bảng thông số so sánh tổng hợp. |
| **Query specs** | `XuLyDuLieuSoSanh(...)` <br> $\rightarrow$ `TruyVanThongSoKyThuat(...)` | `StorefrontController.GetProductDetail(slug)` | Thực tế để so sánh, Client phải tải thông tin chi tiết từng sản phẩm thông qua slug của sản phẩm đó và tự so khớp thông số kỹ thuật. |
| **Opt: 4a** | `NhanXoaMotSanPham(idSanPham)` <br> $\rightarrow$ `YeuCauXoaSanPham(idSanPham)` | Không có API tương ứng | Xóa sản phẩm khỏi so sánh được xử lý cục bộ trên Client (cập nhật lại mảng IDs lưu ở LocalStorage). |
| **Opt: 4b** | `ChonChiHienThiKhacBiet()` <br> $\rightarrow$ `LocCacThongSoGiongNhau()` | Client UI | Xử lý hoàn toàn ở giao diện (Blazor/React): so sánh các key-value của Dictionary thông số kỹ thuật và ẩn các dòng trùng nhau. |

---

## 2. Những điểm khác biệt quan trọng giữa Sơ đồ và Code

### 1. Tính năng So sánh sản phẩm chưa được xây dựng ở Backend
* **Trong sơ đồ:** Mô tả một quy trình đầy đủ với các API (`YeuCauThemVaoSoSanh`, `LayChiTietSoSanh`, `YeuCauXoaSanPham`, `YeuCauXoaToanBo`) và các xử lý tương ứng trong Service/Database để quản lý danh sách sản phẩm so sánh tạm thời.
* **Trong thực tế Code:** **Tính năng này hoàn toàn không tồn tại ở phía Backend**.
  * Dự án không có bất kỳ Controller nào tên là `CompareController` hay Service `CompareService`.
  * Database cũng không thiết kế bảng nào để lưu danh sách so sánh tạm thời của người dùng.

### 2. Quản lý danh sách so sánh tạm thời (Session/Cookie vs Database)
* **Trong sơ đồ:** Danh sách tạm thời được lưu xuống Database (`LuuSanPhamVaoDanhSachTam`).
* **Trong thực tế Code:** Trong các hệ thống TMĐT hiện đại, danh sách so sánh tạm thời hiếm khi được lưu vào Database của Backend để tránh gánh nặng truy vấn và rác dữ liệu. Thay vào đó, danh sách so sánh (chứa tối đa 3-4 ID sản phẩm) sẽ được lưu trữ hoàn toàn dưới Client (trong LocalStorage hoặc Cookie của trình duyệt). Do đó, sơ đồ Sequence đang vẽ theo hướng lưu trữ tập trung ở Backend, trong khi thực tế dự án (hoặc các thiết kế tối ưu hơn) sẽ xử lý phi tập trung ở Client.

### 3. Logic kiểm tra điều kiện so sánh (Validation)
* **Trong sơ đồ:** Các kiểm tra như "Tối đa 3 sản phẩm" và "Sản phẩm phải cùng danh mục" được thực hiện ở Backend (`KiemTraDieuKienSoSanh`).
* **Trong thực tế Code:** Vì Backend không có API so sánh, toàn bộ logic validation này phải được viết ở Client UI khi khách hàng nhấn thêm sản phẩm (Client kiểm tra mảng IDs hiện tại trong LocalStorage, nếu chiều dài $\ge 3$ thì báo lỗi; nếu sản phẩm mới khác danh mục với sản phẩm có sẵn thì báo lỗi).

### 4. Truy vấn thông số kỹ thuật (Product Specifications Query)
* **Trong sơ đồ:** Có API `LayChiTietSoSanh(danhSachId)` gọi đến Database để truy vấn thông số kỹ thuật của danh sách sản phẩm và trả về cấu trúc bảng so sánh tổng hợp (`bangDuLieuSoSanh`).
* **Trong thực tế Code:** Không có API trả về bảng so sánh tổng hợp. Client sẽ phải gọi API lấy chi tiết của từng sản phẩm (`api/storefront/products/{slug}`) và tự tổ chức hiển thị dưới dạng bảng so sánh so khớp các thuộc tính kỹ thuật (EF Core lưu trữ thông số kỹ thuật dưới dạng JSON Dictionary `Dictionary<string, string>` của sản phẩm, được cấu hình `specComparer` trong `HushStoreDbContext.cs`).

---

## 3. Đánh giá & Khuyến nghị cho Đồ án/Bảo vệ môn học

* **Báo cáo trung thực và giải trình:** Khi bảo vệ đồ án, bạn nên lưu ý rằng **Tính năng so sánh sản phẩm hiện đang được xử lý 100% Client-side (ở phía giao diện)** bằng cách lưu trữ tạm danh sách sản phẩm trong LocalStorage của trình duyệt và truy vấn thông tin chi tiết qua các API storefront thông thường, thay vì triển khai lưu trữ trên Backend và Database như sơ đồ vẽ. Điều này giúp tối ưu hóa hiệu năng hệ thống.
* **Cải tiến sơ đồ:** Để sơ đồ Sequence phản ánh đúng kiến trúc thực tế, bạn nên:
  * Loại bỏ các thực thể `Service` và `Database` ở các bước thêm/xóa sản phẩm so sánh.
  * Chỉ rõ các tương tác thêm, xóa, kiểm tra số lượng ($\le 3$), kiểm tra cùng danh mục được thực hiện nội bộ trong đối tượng `Giao diện (UI)`.
  * Ở bước so sánh, UI chỉ gọi API storefront `api/storefront/products/{slug}` để lấy thông tin chi tiết của các sản phẩm cần so sánh, sau đó tự xử lý so khớp và hiển thị trên giao diện.

---

# So sánh Sơ đồ Sequence Diagram và Mã nguồn Dự án (Bán hàng tại quầy - POS)

Tài liệu này phân tích mối tương quan giữa sơ đồ Sequence Diagram (Bán hàng tại quầy - POS) và mã nguồn thực tế của dự án, đồng thời chỉ ra các điểm khác biệt quan trọng về mặt logic và kiến trúc.

---

## 1. Mối quan hệ giữa các mũi tên (Messages) trong sơ đồ và Code

Dưới đây là bảng đối chiếu các bước trong sơ đồ Sequence Diagram POS với các thành phần Class, Method và API tương ứng trong mã nguồn dự án:

| Bước trong Sơ đồ | Tên mũi tên (Sơ đồ) | Thành phần Code tương ứng | Mô tả kỹ thuật trong Code |
| :--- | :--- | :--- | :--- |
| **Loop: 1** | `QuetMaseri(maSeri)` | Client POS UI | Nhân viên dùng máy quét hoặc gõ số serial của sản phẩm cần thanh toán. |
| **Loop: 2** | `KiemTraVaThemSanPham(maSeri)` | `PosController.ScanSerial(...)` | API POST `/api/pos/scan` nhận yêu cầu tìm thông tin sản phẩm từ mã serial. |
| **Loop: 3** | `XuLyThemSanPham(maSeri)` | `PosService.ScanSerialAsync(maSeri)` | Dịch vụ kiểm tra sự tồn tại và tính khả dụng của serial trong kho. |
| **Loop: 4 & 5** | `TruyVanTrangThaiSeri(maSeri)` <br> $\rightarrow$ `ketQuaSeri` | `_serialRepo.GetBySerialNumberAsync(maSeri)` | Truy vấn thông tin thực thể `ProductSerial` kèm theo `Variant` và `Product` từ DB. |
| **Alt: Seri lỗi** | `TraVeLoi("Seri không hợp lệ")` <br> $\rightarrow$ `HienThiLoi()` | `ApiResult<PosScanResponse>.Fail(...)` | Trả về lỗi nếu không tìm thấy serial, hoặc serial không ở trạng thái sẵn sàng để bán (`Status != SerialStatus.Available`). |
| **Alt: Seri OK** | `thongTinSanPham` <br> $\rightarrow$ `CapNhatDanhSachCho()` | `ApiResult<PosScanResponse>.Ok(...)` | Trả về thông tin sản phẩm (Tên, Biến thể, Giá, Thời hạn bảo hành). Client UI thêm mặt hàng vào danh sách chờ thanh toán. |
| **Opt: Phụ trợ** | `XuLyThongTinPhuTro(...)` <br> $\rightarrow$ `CapNhatDonTam(...)` | `PosController.LookupCustomer(...)` và `ValidateVoucher(...)` | **[Khác biệt logic]** Các thao tác tra cứu khách hàng và áp dụng voucher được xử lý bằng các API riêng biệt thay vì một API "Cập nhật đơn tạm". |
| **Opt: Ghi nhận** | `XuLyCapNhatDonTam(...)` <br> $\rightarrow$ `TruyVanHoacLuuDuLieu(...)` | `PosService.LookupCustomerAsync(...)` & `ValidateVoucherAsync(...)` | Dịch vụ xử lý tra cứu thông tin tài khoản qua SĐT và kiểm tra tính hợp lệ của voucher dưới DB. |
| **Thanh toán** | `ChonThanhToanVaHoanTat(...)` <br> $\rightarrow$ `GuiYeuCauHoanTatDon(...)` | `PosController.Checkout(...)` | API POST `/api/pos/checkout` nhận thông tin giỏ hàng, thông tin khách hàng, phương thức thanh toán và mã voucher. |
| **Thanh toán** | `XuLyHoanTatDonBanHang(...)` | `PosService.CheckoutAsync(...)` | Dịch vụ mở Transaction DB để tạo đơn hàng POS mới ở trạng thái hoàn thành (`Status = Success`), dọn dẹp các serial, kích hoạt bảo hành. |
| **Thanh toán** | `TruTonKhoVaKichHoatBaoHanh()` <br> $\rightarrow$ `capNhatThanhCong` | `ProductSerial.Status = SerialStatus.Sold` <br> & `_warrantyRepo.AddRangeAsync(...)` | Cập nhật trạng thái từng serial sang `Sold`, ghi nhận ngày bán và lưu các bản ghi bảo hành tương ứng. |
| **Thanh toán** | `LuuHoaDonBanHang()` <br> $\rightarrow$ `luuThanhCong` | `_orderRepo.AddAsync(order)` <br> & `_unitOfWork.SaveChangesAsync()` | Lưu thông tin đơn hàng và các dòng chi tiết đơn hàng xuống DB, kết thúc bằng `CommitAsync()`. |
| **In & Mở két** | `YeuCauInHoaDonVaMoKet()` <br> $\rightarrow$ `MoKetTienVaInHoaDon()` | Client POS UI | Sau khi Checkout trả về thành công (HTTP 200), giao diện client tự kích hoạt lệnh in bill và gửi tín hiệu mở két tiền vật lý. |

---

## 2. Những điểm khác biệt quan trọng giữa Sơ đồ và Code

### 1. Tính năng Lưu hóa đơn nháp (Draft Order)
* **Trong sơ đồ:** Quy trình cập nhật và lưu đơn hàng tạm (`CapNhatDonTam` $\rightarrow$ `XuLyCapNhatDonTam`) diễn ra liên tục để duy trì trạng thái hóa đơn nháp.
* **Trong thực tế Code:** 
  * Dự án đã định nghĩa API lưu đơn nháp `PosController.SaveDraft` và xóa đơn nháp `DeleteDraft`, tuy nhiên phương thức xử lý chính **`PosService.SaveDraftAsync` hiện tại chưa hoàn thành** và trả về thông báo lỗi: 
    ```csharp
    return await Task.FromResult(ApiResult<PosDraftDto>.Fail("Tính năng lưu tạm đang được xây dựng (cần thống nhất cách lưu trữ SerialDraft)."));
    ```
  * Trạng thái giỏ hàng POS đang thanh toán dở dang hoàn toàn được lưu giữ tạm thời ở bộ nhớ Client (Client-side state) chứ không liên tục gửi API đồng bộ xuống Database.

### 2. Thiết kế API cho các thao tác phụ trợ (Customer Lookup & Voucher Validation)
* **Trong sơ đồ:** Mô tả một API chung `CapNhatDonTam(thongTinYeuCau)` để ghi nhận mọi thông tin phụ trợ.
* **Trong thực tế Code:** Tách biệt thành các API độc lập, rõ ràng:
  * `api/pos/customer?phone=...`: Chỉ dùng để tìm kiếm thông tin khách hàng thành viên.
  * `api/pos/voucher/validate`: Xác thực voucher và tính số tiền được giảm dựa trên tổng tiền tạm tính.
  * Client UI gọi các API này khi nhân viên thao tác nhập SĐT khách hoặc áp dụng voucher, sau đó chỉ gửi kết quả tổng hợp cuối cùng lên API `checkout`.

### 3. Tương tác với thiết bị phần cứng (In bill & Mở két tiền)
* **Trong sơ đồ:** Vẽ luồng `YeuCauInHoaDonVaMoKet()` trả về từ API Controller.
* **Trong thực tế Code:** Backend chỉ trả về kết quả JSON chứa thông tin hóa đơn vừa tạo thành công (`ApiResult<PosOrderDto>`). Hành động in hóa đơn (gửi lệnh đến máy in nhiệt) và mở két tiền (gửi xung điện qua cổng RJ11 của máy in bill đến két) được thực hiện hoàn toàn bởi mã nguồn Client UI (sử dụng Web API của trình duyệt hoặc thư viện JS kết nối phần cứng) sau khi nhận tín hiệu thành công từ Backend.

---

## 3. Đánh giá & Khuyến nghị cho Đồ án/Bảo vệ môn học

* **Lưu ý về tính năng đơn nháp (Save Draft):** Trong slide thuyết trình hoặc báo cáo, bạn cần chú thích rõ tính năng lưu hóa đơn nháp (khi khách đang tính tiền mà muốn mua thêm sản phẩm, thu ngân lưu nháp để thanh toán cho khách sau) đang ở dạng **thiết kế/phát triển dở dang ở tầng Service**, hiện tại giao diện đang duy trì tạm thời tại Client State.
* **Giải trình về kiểm kho vật lý:** Do bán hàng tại quầy (POS) là xuất hàng trực tiếp cho khách, mã nguồn thực hiện **trừ kho vật lý ngay lập tức** bằng cách chuyển trạng thái số Serial quét được sang `Sold` và gọi dịch vụ đồng bộ `SyncStockBatchAsync` ngay khi click thanh toán. Điểm này khác biệt với luồng đặt hàng Online (chỉ trừ kho ảo lúc checkout và trừ kho vật lý lúc xuất kho). Thiết kế này rất hợp lý và thực tế.
* **Đồng bộ hóa sơ đồ:** Bạn nên điều chỉnh sơ đồ bằng cách tách các bước `LookupCustomer` và `ValidateVoucher` thành các nhánh tương tác riêng lẻ thay vì dùng chung thông điệp `CapNhatDonTam` khái quát.

---

# So sánh Sơ đồ Sequence Diagram và Mã nguồn Dự án (Nhập kho hàng - Goods Receipt / Import)

Tài liệu này phân tích mối tương quan giữa sơ đồ Sequence Diagram (Nhập kho hàng) và mã nguồn thực tế của dự án, đồng thời chỉ ra các điểm khác biệt quan trọng về mặt logic và kiến trúc.

---

## 1. Mối quan hệ giữa các mũi tên (Messages) trong sơ đồ và Code

Dưới đây là bảng đối chiếu các bước trong sơ đồ Sequence Diagram Nhập kho với các thành phần Class, Method và API tương ứng trong mã nguồn dự án:

| Bước trong Sơ đồ | Tên mũi tên (Sơ đồ) | Thành phần Code tương ứng | Mô tả thực tế trong Code |
| :--- | :--- | :--- | :--- |
| **Bản vẽ** | `ChonChucNhanNhapKho()` | Client UI | Nhân viên kho truy cập chức năng Nhập kho trên giao diện quản trị. |
| **Opt: 3b** | `NhapThongTinNhaCungCapMoi(...)` | Không hỗ trợ trong luồng | **[Khác biệt luồng]** Phải tạo Nhà cung cấp trước ở module quản lý Supplier chứ không tạo tạm ở giao diện nhập kho. |
| **Opt: 3a** | `NhapThongTinSanPhamMoi(...)` <br> $\rightarrow$ `GuiYeuCauTaoSanPham(...)` | Không hỗ trợ trong luồng | **[Khác biệt luồng]** Phải tạo Sản phẩm và Biến thể trước ở module quản lý Product chứ không tạo trực tiếp khi đang nhập kho. |
| **Lượng & Giá** | `NhapGiaVaSoLuongTong(...)` | Client UI | Nhập số lượng tổng cần nhập của biến thể và đơn giá nhập tương ứng. |
| **Alt: Quét tay** | `QuetMaSerial(maSerial)` | Client UI | Dùng máy quét mã vạch để quét từng số serial của thiết bị. |
| **Alt: Quét tay** | `KiemTraTinhDuyNhatSerial(maSerial)` | Không có API tương ứng | **[Khác biệt logic]** Không có API kiểm tra trùng lặp đơn lẻ cho từng serial tại thời điểm quét. |
| **Alt: Quét tay** | `TruyVanKiemTraSerial(maSerial)` | `_serialRepo.GetExistingSerialsAsync(list)` | Thực tế việc check trùng với DB được gộp lại và thực hiện một lần khi hoàn tất lưu phiếu nhập. |
| **Alt: Excel** | `TaiLenFileExcel(fileDuLieu)` | Không hỗ trợ | **[Chưa tích hợp]** Dự án hiện chưa phát triển tính năng tải file Excel để import danh sách serial. |
| **Alt: Excel** | `DocFileVaKiemTraDanhSachSerial(...)` | Không hỗ trợ | Không có code xử lý đọc và phân tích dữ liệu serial từ file Excel gửi lên Backend. |
| **Submit** | `NhanHoanTatPhieuNhap(...)` <br> $\rightarrow$ `GuiYeuCauLuuPhieuNhap(...)` | `ImportReceiptsController.Create(...)` | API POST `/api/import-receipts` nhận toàn bộ DTO chứa thông tin phiếu nhập, chi tiết số lượng và danh sách serial. |
| **Validate** | `KiemTraSoLuongSeriVaSoLuongTong()` | `ImportReceiptDetailRequestValidator` | FluentValidation kiểm tra `SerialNumbers.Count == Quantity` và tìm các mã serial trùng lặp nội bộ. |
| **Alt: Sai số lượng**| `TraVeLoi("Số lượng không khớp")` | `ApiResult.Fail(...)` | Nếu số lượng serial không bằng số lượng nhập (`Quantity`), hệ thống trả về HTTP 400 BadRequest kèm thông báo lỗi. |
| **Lưu NCC** | `LuuNhaCungCapMoi(...)` | Không hỗ trợ trong luồng | Backend yêu cầu `SupplierId` phải tồn tại từ trước, nếu không sẽ trả về lỗi: "Nhà cung cấp không tồn tại". |
| **Cập nhật kho** | `CapNhatTonKhoVaGiaVon(...)` | `_inventorySyncService.SyncStockBatchAsync(...)` | Backend cập nhật số lượng tồn kho khả dụng bằng cách đồng bộ số lượng serial thực tế trong kho. |
| **Lưu Serials** | `LuuDanhSachSerialMoi(...)` | `_serialRepo.AddRangeAsync(allNewSerials)` | Lưu hàng loạt (Bulk Insert) các thực thể `ProductSerial` vào Database ở trạng thái `Status = 0` (Available). |
| **Lưu Phiếu** | `LuuPhieuNhapKho(...)` | `_receiptRepo.AddAsync(receipt)` <br> & `_unitOfWork.CommitAsync()` | Lưu thông tin phiếu nhập (`ImportReceipt`) và các dòng chi tiết (`ImportReceiptDetail`), hoàn tất Transaction. |

---

## 2. Những điểm khác biệt quan trọng giữa Sơ đồ và Code

### 1. Chưa hỗ trợ Nhập Serial bằng file Excel (Import Excel)
* **Trong sơ đồ:** Mô tả quy trình cho phép nhân viên tải file Excel chứa danh sách serial (`TaiLenFileExcel`), hệ thống sẽ đọc và kiểm tra danh sách mã từ file (`DocFileVaKiemTraDanhSachSerial`), bôi đỏ các mã lỗi và điền các mã hợp lệ.
* **Trong thực tế Code:** **Chưa hề phát triển luồng này**.
  * Dự án chỉ dùng thư viện EPPlus cho việc **Xuất file cấu hình Build PC ra Excel** (chiều ghi ra), hoàn toàn không có bất kỳ API hay Service nào xử lý việc **Đọc file Excel** (chiều đọc vào) để phân tích danh sách Serial.
  * Việc nhập serial hiện tại bắt buộc phải thực hiện bằng cách truyền mảng chuỗi (`List<string> SerialNumbers`) qua body JSON của request tạo phiếu nhập.

### 2. Không tạo nóng Nhà cung cấp / Sản phẩm trong luồng Nhập kho
* **Trong sơ đồ:** Cho phép tạo nóng nhà cung cấp mới (`LuuNhaCungCapMoi`) và sản phẩm mới (`LuuSanPhamMoi`) ngay trong tiến trình lập phiếu nhập kho nếu phát hiện thông tin chưa có trong hệ thống.
* **Trong thực tế Code:** Luồng xử lý Backend cực kỳ nghiêm ngặt và không hỗ trợ tạo bắc cầu:
  * Trong `ImportReceiptService.CreateAsync`, hệ thống sẽ chặn và báo lỗi ngay lập tức nếu `SupplierId` hoặc `VariantId` của sản phẩm không tìm thấy trong DB (lần lượt trả về các lỗi: *"Nhà cung cấp không tồn tại..."* và *"Biến thể sản phẩm không tồn tại..."*).
  * Nhân viên bắt buộc phải truy cập các module quản lý tương ứng (`SuppliersController` và `ProductsController`) để thêm mới nhà cung cấp / sản phẩm trước khi có thể gọi API tạo phiếu nhập kho.

### 3. Thời điểm và cơ chế Kiểm tra trùng lặp Serial (Validation Timing)
* **Trong sơ đồ:** Khi quét từng serial lẻ bằng tay, Client sẽ liên tục gửi request kiểm tra trùng lặp (`KiemTraTinhDuyNhatSerial`) để báo lỗi thời gian thực (Real-time alert).
* **Trong thực tế Code:** Không có API kiểm tra trùng lặp serial đơn lẻ.
  * Việc kiểm tra trùng lặp được gom lại và thực hiện **đồng loạt một lần** ở Backend khi nhân viên nhấn hoàn tất phiếu nhập:
    * FluentValidation (`CreateImportReceiptRequestValidator`) kiểm tra xem có serial nào bị nhập trùng lặp 2 lần trong chính phiếu nhập đó hay không (`duplicateInternal`).
    * Service kiểm tra xem các serial nhập vào đã tồn tại trong Database từ trước hay chưa thông qua hàm `_serialRepo.GetExistingSerialsAsync(allSerials)`. Nếu phát hiện bất kỳ trùng lặp nào, toàn bộ giao dịch sẽ bị rollback và báo lỗi.

---

## 3. Đánh giá & Khuyến nghị cho Đồ án/Bảo vệ môn học

* **Giải trình về Excel Import:** Bạn nên lưu ý rằng tính năng nhập serial bằng Excel hiện tại là **ở dạng đề xuất thiết kế trên sơ đồ**, chưa được lập trình thực tế ở Backend. Trong đồ án, bạn có thể trình bày đây là hướng phát triển tiếp theo (Future Work) để hỗ trợ thủ kho nhập lô hàng lớn nhanh chóng hơn.
* **Giải trình về ràng buộc nghiệp vụ:** Việc tách biệt luồng tạo Nhà cung cấp / Sản phẩm khỏi phiếu nhập kho giúp đảm bảo nguyên tắc Clean Architecture và Single Responsibility. Nó ngăn chặn việc dữ liệu danh mục bị rác hoặc thiếu kiểm soát (như nhập sai tên nhà cung cấp, thiếu thuộc tính sản phẩm) khi tạo vội trong lúc nhập kho.
* **Đồng bộ hóa sơ đồ:** Để sơ đồ khớp 100% với Code:
  * Loại bỏ nhánh rẽ "Nhập Serial bằng file Excel".
  * Loại bỏ các bước `GuiYeuCauTaoSanPham` và `LuuNhaCungCapMoi` ra khỏi sơ đồ sequence của Nhập kho.
  * Thay thế vòng lặp check trùng serial đơn lẻ `KiemTraTinhDuyNhatSerial` bằng một bước validation tổng thể ở Backend khi hoàn tất phiếu nhập.

---

# So sánh Sơ đồ Sequence Diagram và Mã nguồn Dự án (Xuất kho hàng - Inventory Export / Goods Issue)

Tài liệu này phân tích mối tương quan giữa sơ đồ Sequence Diagram (Xuất kho hàng) và mã nguồn thực tế của dự án, đồng thời chỉ ra các điểm khác biệt quan trọng về mặt logic và kiến trúc.

---

## 1. Mối quan hệ giữa các mũi tên (Messages) trong sơ đồ và Code

Dưới đây là bảng đối chiếu các bước trong sơ đồ Sequence Diagram Xuất kho với các thành phần Class, Method và API tương ứng trong mã nguồn dự án:

| Bước trong Sơ đồ | Tên mũi tên (Sơ đồ) | Thành phần Code tương ứng | Mô tả thực tế trong Code |
| :--- | :--- | :--- | :--- |
| **Khởi động** | `ChonYeuCauXuatKho(idYeuCau)` | Client UI | Thủ kho chọn đơn hàng ở trạng thái **Đã xác nhận (Confirmed)** cần xuất trên giao diện. |
| **Chi tiết** | `LayChiTietYeuCau(idYeuCau)` <br> $\rightarrow$ `XuLyLayChiTiet(idYeuCau)` | `OrdersController.GetById(id)` | Gọi API lấy chi tiết đơn hàng trực tiếp để hiển thị danh sách sản phẩm và số lượng tương ứng cần xuất kho. |
| **Chi tiết DB** | `TruyVanChiTietYeuCau(idYeuCau)` | `IOrderRepository.GetByIdAsync(id)` | Truy vấn thông tin đơn hàng và các dòng `OrderDetail` từ Database. |
| **Quét Serial** | `QuetMaSerial(maSerial)` | Client UI | Thủ kho quét mã số serial của thiết bị xuất kho. |
| **Xác thực** | `KiemTraSerial(maSerial, idSanPham)` <br> $\rightarrow$ `XuLyKiemTraSerial(maSerial)` | `InventoryController.ValidateSerial(...)` | API GET `/api/inventory/serials/validate` nhận mã serial và variantId để xác thực tính khả dụng. |
| **Xác thực DB** | `TruyVanTrangThaiSerial(maSerial)` <br> $\rightarrow$ `trangThaiHienTai` | `_serialRepo.GetBySerialNumberAsync(serialNo)` | Truy vấn thực thể `ProductSerial` từ Database. Serial phải tồn tại, khớp `variantId` và có trạng thái `Available` (0). |
| **Lưu tạm** | `LuuTamVaoPhienXuat(maSerial)` | Không có API tương ứng | **[Khác biệt logic]** Danh sách các serial quét thành công được lưu trữ tạm thời tại Client State (bộ nhớ trình duyệt) chứ không ghi nhận tạm xuống DB. |
| **Alt: Hủy** | `NhanHuyBoPhien()` <br> $\rightarrow$ `YeuCauXoaPhienTam()` | Không hỗ trợ | Không có API hủy phiên tạm trên Backend. Client tự xóa mảng serials đã quét khỏi bộ nhớ local khi đóng giao diện. |
| **Hoàn tất** | `NhanLuuPhieuXuat()` <br> $\rightarrow$ `GuiYeuCauLuuPhieu(idYeuCau)` | `InventoryController.ExportOrder(...)` | API POST `/api/inventory/export-order` nhận yêu cầu xuất kho gồm `OrderId` và map chi tiết các serial đã quét cho từng dòng hàng. |
| **Check đủ** | `KiemTraSoLuongQuet(idYeuCau)` <br> $\rightarrow$ `soLuongThucTe` | `InventoryExportService.ExportOrderAsync` | Service so khớp: Số lượng mã serial được gửi lên cho mỗi `OrderDetailId` phải bằng chính xác số lượng mua (`Quantity`). |
| **Alt: Thiếu** | `TraVeLoi("Chưa đủ số lượng")` | `ApiResult.Fail(...)` | Nếu số lượng serial đã quét không khớp với số lượng trên đơn, Backend ném exception trả về lỗi và dừng transaction. |
| **Trừ kho** | `TruTonKhoVaCapNhatSerial()` | `ProductSerial.Status = SerialStatus.Sold` | Chuyển trạng thái các serial sang `Sold` (Đã bán - 2), gán `SoldDate = UtcNow`, cập nhật `OrderId`. |
| **Lưu phiếu** | `LuuThongTinPhieuXuat()` | `order.Status = OrderStatus.Exported` <br> & `_unitOfWork.CommitAsync()` | Chuyển trạng thái đơn hàng sang `Exported` (Đã xuất kho - 4), lưu bản ghi liên kết `OrderSerial` và commit. |
| **Đồng bộ** | Không có trên sơ đồ | `_inventorySyncService.SyncStockBatchAsync(...)` | Kích hoạt dịch vụ đồng bộ trừ tồn kho vật lý (`StockQuantity`) của các biến thể sản phẩm liên quan trong Database. |

---

## 2. Những điểm khác biệt quan trọng giữa Sơ đồ và Code

### 1. Không có bảng tạm quản lý "Phiên xuất kho" ở Database
* **Trong sơ đồ:** Thể hiện việc lưu tạm các mã serial hợp lệ đã quét xuống Database thông qua thông điệp `LuuTamVaoPhienXuat(maSerial)`. Nếu nhân viên nhấn hủy phiên, hệ thống gọi API `YeuCauXoaPhienTam()` để dọn dẹp dữ liệu tạm trên DB.
* **Trong thực tế Code:** **Xử lý phi tập trung (Decentralized) ở Client**.
  * Backend không lưu vết các serial đã quét dở dang. Trạng thái quét (được bao nhiêu cái, mã nào) được lưu trữ hoàn toàn trên giao diện Client (React/Blazor state).
  * Backend chỉ cung cấp một API kiểm tra tính hợp lệ nhanh của serial (`ValidateSerial`). 
  * Khi thủ kho quét xong toàn bộ và nhấn "Lưu phiếu xuất", toàn bộ danh sách serial mới được gửi đồng loạt lên API `export-order`. Nếu thủ kho hủy bỏ giữa chừng, Client tự giải phóng bộ nhớ mà không cần gọi API dọn dẹp nào xuống DB.

### 2. Định nghĩa "Yêu cầu xuất kho"
* **Trong sơ đồ:** Sử dụng thuật ngữ tổng quát "Yêu cầu xuất kho" (`idYeuCau`).
* **Trong thực tế Code:** Bản chất "Yêu cầu xuất kho" chính là **Đơn hàng (Order) ở trạng thái Confirmed**. Do đó, Backend không thiết kế bảng riêng cho Yêu cầu xuất kho, mà tận dụng luôn thực thể `Order` và `OrderDetail` để làm căn cứ đối chiếu số lượng cần xuất.

### 3. Quy trình đồng bộ hóa tồn kho vật lý
* **Trong sơ đồ:** Mô tả bước `TruTonKhoVaCapNhatSerial` chung.
* **Trong thực tế Code:** Quá trình này được phân tách và kiểm soát chặt chẽ:
  1. Cập nhật trạng thái cụ thể của từng Serial vật lý (chuyển sang `Sold` và gán khóa ngoại `OrderId`).
  2. Lưu lịch sử liên kết thiết bị cụ thể với đơn hàng vào bảng `OrderSerial`.
  3. Gọi dịch vụ đồng bộ `_inventorySyncService.SyncStockBatchAsync(variantIds)` để tính toán lại số lượng tồn kho vật lý (`StockQuantity`) của Variant dựa trên số lượng serial còn ở trạng thái `Available` trong DB.

---

## 3. Đánh giá & Khuyến nghị cho Đồ án/Bảo vệ môn học

* **Giải trình về việc lưu trữ phiên tạm:** Bạn nên thuyết minh rõ: Thiết kế lưu trữ danh sách serial đã quét tạm thời ở bộ nhớ trình duyệt Client thay vì lưu DB là thiết kế tối ưu, giúp giảm thiểu số lượng ghi (Write operations) rác xuống database khi thủ kho hủy phiên làm việc.
* **Đồng bộ hóa sơ đồ:** 
  * Loại bỏ các bước `LuuTamVaoPhienXuat` và `YeuCauXoaPhienTam` liên quan đến Database.
  * Chỉ rõ bước quét serial thành công sẽ được Client UI tự cộng dồn vào danh sách hiển thị trên giao diện.
  * Thay thế các đối tượng "Yêu cầu xuất kho" bằng thực thể "Đơn hàng" (Order) để sát với mã nguồn thực tế của dự án.

---

# So sánh Sơ đồ Sequence Diagram và Mã nguồn Dự án (Kiểm kê kho hàng - Inventory Audit / Inventory Check)

Tài liệu này phân tích mối tương quan giữa sơ đồ Sequence Diagram (Kiểm kê kho hàng) và mã nguồn thực tế của dự án, đồng thời chỉ ra các điểm khác biệt quan trọng về mặt logic và kiến trúc.

---

## 1. Mối quan hệ giữa các mũi tên (Messages) trong sơ đồ và Code

Dưới đây là bảng đối chiếu các bước trong sơ đồ Sequence Diagram Kiểm kê kho hàng với các thành phần Class, Method và API tương ứng trong mã nguồn dự án:

| Bước trong Sơ đồ | Tên mũi tên (Sơ đồ) | Thành phần Code tương ứng | Mô tả thực tế trong Code |
| :--- | :--- | :--- | :--- |
| **Khởi động** | `TaoPhieuKiemKe(phamVi)` | Client UI | Nhân viên chọn tạo phiếu kiểm kê toàn kho hoặc theo danh mục linh kiện cụ thể. |
| **Yêu cầu API** | `YouCauTaoPhieuKiemKe(phamVi)` <br> $\rightarrow$ `XuLyTaoPhieuKiemKe(phamVi)` | `InventoryChecksController.Create(...)` | API POST `/api/inventory-checks` nhận phạm vi kiểm kê từ body request và gọi Service. |
| **Snapshot** | `LayTonKhoLyThuyetVaKhoa(phamVi)` <br> $\rightarrow$ `danhSachLyThuyet` | `InventoryCheckService.CreateAsync` | Truy vấn các biến thể trong phạm vi kiểm kê và đếm số lượng ProductSerial khả dụng (`Available`) làm mốc snapshot đối chiếu. |
| **Quét Serial** | `QuetMaSerial(maSerial)` | Client UI | Thủ kho tiến hành quét mã vạch serial của sản phẩm vật lý tại quầy/kệ kho. |
| **API Đối chiếu** | `DoiChieuMaSerial(maSerial)` <br> $\rightarrow$ `KiemTraTrangThaiSerial(maSerial)` | `InventoryChecksController.ScanSerial(...)` | API POST `/api/inventory-checks/{id}/scan` nhận mã serial và variantId đề xuất (nếu là mã lạ) từ Client gửi lên. |
| **DB Lịch sử** | `TruyVanLichSuSerial(maSerial)` <br> $\rightarrow$ `thongTinSerial` | `_serialRepo.GetBySerialNumberAsync(...)` | Truy vấn DB để lấy thông tin serial. Nếu serial không tồn tại, ghi nhận log quét thừa mã lạ (`UnknownSurplus`). |
| **Alt: Hàng thừa** | `TraVeThongTinSerialLa()` <br> $\rightarrow$ `GoiYNhapKhoHoacTaoMoi()` | `ScanResultDto.RequiresVariantInput` | Trả về cờ yêu cầu chỉ định VariantId nếu quét phải serial không tồn tại trong DB, thủ kho không thể tạo nóng sản phẩm. |
| **Alt: Hàng lỗi** | `ChuyenTrangThaiHangLoi(maSerial)` | `InventoryChecksController.MarkDefective(...)` | **[Khác biệt luồng]** Phải gọi API PUT `mark-defective` riêng biệt để chuyển trạng thái serial thành `Defective` trong phiếu. |
| **Alt: Khớp** | `TraVeKetQuaKhop()` <br> $\rightarrow$ `CapNhatBangSoSanh()` | `ScanResultDto` (Trạng thái `Matched`) | Trả về kết quả khớp và cập nhật tiến độ (ActualQuantity++, MatchedQuantity++) trên giao diện kiểm kê. |
| **Opt: Tạm dừng** | `NhanLuuNhap()` <br> $\rightarrow$ `YeuCauLuuNhapPhieu()` | Không hỗ trợ API riêng | **[Khác biệt logic]** Mỗi lượt quét thành công đều được cập nhật trực tiếp xuống DB (`Draft`). Không cần nút lưu nháp thủ công. |
| **Opt: Cân bằng** | `NhapLyDoLechVaCanBang(lyDo)` <br> $\rightarrow$ `GuiYeuCauCanBangKho(...)` | `InventoryChecksController.Approve(...)` | **[Phân quyền Admin]** Nhân viên chỉ được gửi duyệt (`Submit`). Chỉ **Admin** mới có quyền gọi API POST `/approve` để thực thi cân bằng kho. |
| **Duyệt DB** | `CapNhatTrangThaiSerial(danhSachThuaThieu)` | `ApproveAsync` | Cân bằng kho thực tế: Đổi trạng thái các serial bị thiếu sang `Lost` (Mất - 4) và serial bị lỗi sang `Defective` (Lỗi - 3). |
| **Hạch toán** | `TaoButToanHachToanKiemKe()` | `adjustmentLogs.Add(...)` | Không hạch toán kế toán. Hệ thống chỉ lưu lịch sử điều chỉnh kho (`InventoryAdjustmentLog`) để làm căn cứ kiểm toán. |
| **Hoàn tất** | `MoKhoaSanPhamValuuPhieu()` | `check.Status = Completed` <br> & `_inventorySyncService.SyncStockBatchAsync` | Chuyển trạng thái phiếu sang `Completed` và đồng bộ số lượng tồn kho khả dụng mới lên website. |

---

## 2. Những điểm khác biệt quan trọng giữa Sơ đồ và Code

### 1. Không có cơ chế "Khóa sản phẩm" khi kiểm kê (No Auditing Lock)
* **Trong sơ đồ:** Thể hiện việc khóa sản phẩm tại bước `LayTonKhoLyThuyetVaKhoa` và mở khóa tại bước `MoKhoaSanPhamValuuPhieu` để đảm bảo dữ liệu tồn kho không bị biến động trong quá trình kiểm kê.
* **Trong thực tế Code:** **Hoàn toàn không có logic khóa sản phẩm**.
  * Trong suốt thời gian kiểm kê (phiếu ở trạng thái `Draft` hoặc `AwaitingApproval`), khách hàng vẫn có thể mua sắm và nhân viên vẫn có thể xuất kho các sản phẩm này bình thường.
  * Để giải quyết vấn đề chênh lệch phát sinh khi bán hàng lúc đang kiểm kê, Backend được thiết kế logic **"Cửa sổ kiểm kê" (Audit Window)** ở hàm `ApproveAsync`. Nếu tại thời điểm duyệt, một serial bị thiếu lúc quét nhưng thực tế đã được bán/giữ chỗ bởi đơn hàng mới phát sinh, hệ thống sẽ tự động bỏ qua không ghi nhận thất thoát tài chính cho thủ kho (`ResolvedDuringApproval = true`).

### 2. Phân quyền và Vai trò phê duyệt cân bằng kho (Authorization Roles)
* **Trong sơ đồ:** Nhân viên kiểm kho tự thực hiện chốt số và cân bằng kho (`GuiYeuCauCanBangKho`).
* **Trong thực tế Code:** Phân quyền nghiêm ngặt giữa các vai trò:
  * **Nhân viên (Employee/Thủ kho):** Chỉ có quyền quét serial, ghi nhận lý do và gửi yêu cầu phê duyệt (`SubmitAsync`). Phiếu được chuyển sang trạng thái `AwaitingApproval` (Chờ duyệt).
  * **Quản lý (Admin):** Có quyền phê duyệt (`ApproveAsync`) hoặc Từ chối duyệt (`RejectAsync`). Chỉ khi Admin phê duyệt, hệ thống mới thực hiện cân bằng kho thực tế dưới database (chuyển trạng thái serial và đồng bộ số lượng tồn).

### 3. Quy trình báo hỏng sản phẩm (Defective Handle)
* **Trong sơ đồ:** Thể hiện việc chuyển trạng thái hàng lỗi ngay lập tức trong vòng lặp quét serial (`ChuyenTrangThaiHangLoi`).
* **Trong thực tế Code:** Phân tách thành 2 bước riêng biệt:
  1. Khi quét serial, hệ thống chỉ kiểm tra so khớp và ghi nhận là Khớp (`Matched`).
  2. Nếu sản phẩm đó bị lỗi vật lý, thủ kho phải chọn sản phẩm đó trên màn hình và nhấn "Báo hỏng" $\rightarrow$ UI gọi API PUT `/mark-defective` để cập nhật trạng thái quét trong phiếu thành `Defective`. Trạng thái thực tế của Serial trong Database vẫn giữ nguyên là `Available` và chỉ chuyển sang `Defective` khi Admin chính thức phê duyệt phiếu kiểm kê.

### 4. Bút toán hạch toán kế toán (Accounting)
* **Trong sơ đồ:** Ghi nhận thông điệp `TaoButToanHachToanKiemKe()` dưới Database.
* **Trong thực tế Code:** Dự án không có phân hệ hạch toán tài chính hay kế toán (không phát sinh bút toán Nợ/Có). Hệ thống chỉ ghi nhận log điều chỉnh kho (`InventoryAdjustmentLog`) lưu trữ mã serial, biến thể sản phẩm, kiểu điều chỉnh (Thất thoát / Lỗi hỏng) và giá trị vốn hao tổn (`CostImpact`) nhằm phục vụ công tác đối soát nội bộ.

---

## 3. Đánh giá & Khuyến nghị cho Đồ án/Bảo vệ môn học

* **Giải trình về cơ chế Cửa sổ kiểm kê (Audit Window):** Đây là điểm cộng công nghệ rất lớn trong buổi bảo vệ. Thay vì khóa cứng việc kinh doanh của cửa hàng (gây bất tiện), hệ thống cho phép bán hàng song song và tự động giải quyết các serial bán ra trong thời gian chờ duyệt. Hãy dùng cơ chế `ResolvedDuringApproval` làm điểm nhấn lập luận.
* **Giải trình về phân quyền:** Việc tách biệt API Approve cho Admin giúp ngăn chặn nhân viên tự ý điều chỉnh số lượng tồn kho để che giấu thất thoát hàng hóa tại cửa hàng.
* **Hiệu chỉnh sơ đồ:** Để sơ đồ đồng bộ với code thực tế:
  * Tách biệt rõ 2 phân vai: Nhân viên quét/gửi duyệt và Admin phê duyệt cân bằng kho.
  * Loại bỏ cơ chế khóa cứng sản phẩm và bút toán kế toán.
  * Thêm nhánh xử lý `PUT mark-defective` của Nhân viên kiểm kho.

---

# So sánh Sơ đồ Sequence Diagram và Mã nguồn Dự án (Tiếp nhận và xử lý Bảo hành - RMA)

Tài liệu này phân tích mối tương quan giữa sơ đồ Sequence Diagram (Tiếp nhận và xử lý Bảo hành / RMA) và mã nguồn thực tế của dự án, đồng thời chỉ ra các điểm khác biệt quan trọng về mặt logic và kiến trúc.

---

## 1. Mối quan hệ giữa các mũi tên (Messages) trong sơ đồ và Code

Dưới đây là bảng đối chiếu chi tiết các bước trong sơ đồ Sequence Diagram Tiếp nhận & Xử lý Bảo hành với các thành phần Class, Method và API tương ứng trong mã nguồn dự án:

| Bước trong Sơ đồ | Tên mũi tên (Sơ đồ) | Thành phần Code tương ứng | Mô tả kỹ thuật trong Code |
| :--- | :--- | :--- | :--- |
| **Quét Serial** | `NhapMaSerial(maSerial)` | Client UI | Nhân viên nhập/quét mã serial của sản phẩm khách mang đến bảo hành. |
| **Thẩm định** | `KiemTraQuyenLoiBaoHanh(maSerial)` | `ServiceTicketsController.EvaluateIntake(...)` | API POST `/api/service-tickets/intake` thực hiện kiểm tra sơ bộ quyền lợi bảo hành. |
| **Service gọi** | `XuLyKiemTraBaoHanh(maSerial)` | `ServiceTicketService.GetWarrantyEvaluationAsync(...)` | Service tiếp nhận và xử lý logic kiểm tra điều kiện bảo hành. |
| **Query DB** | `TruyVanLichSuBanHang(maSerial)` <br> $\rightarrow$ `thongTinLichSu` | `_serialRepository.GetBySerialNumberAsync(serialNumber)` | Truy vấn DB để lấy thông tin serial. Nếu serial không tồn tại, trả về `BlockingReason`. |
| **Alt: Sai mã** | `[2a. Serial không tồn tại]` <br> $\rightarrow$ `TraVeLoi("Mã Serial không tồn tại")` | `BlockingReason = "Không tìm thấy Serial..."` | Nếu không tìm thấy serial, hệ thống trả về lý do chặn để UI thông báo lỗi kết thúc. |
| **Alt: Hết hạn** | `[2b. Hết hạn bảo hành]` <br> $\rightarrow$ `TraVeLoi("Hết hạn bảo hành")` | `WarrantyEvaluator.EvaluateAsync(...)` | **[Khác biệt logic]** Kiểm tra bảo hành nếu hết hạn sẽ trả về `IsInWarranty = false` và gợi ý nhánh sửa tính phí (`PaidRepair`) chứ không báo lỗi kết thúc ngay. |
| **Hợp lệ** | `thongTinBaoHanh` <br> $\rightarrow$ `HienThiChiTietLichSu()` | `ServiceTicketIntakeEvaluationDto` | Trả về DTO chứa tên sản phẩm, hạn bảo hành, thông tin khách hàng và danh sách các luồng xử lý được phép (`allowedBranches`). |
| **Chọn nhánh** | `NhapTinhTrangVaHuongXuLy(...)` | Client UI | Nhân viên nhập tình trạng ngoại quan (xước, móp, cháy nổ), lỗi mô tả và chọn nhánh xử lý. |
| **Yêu cầu tạo** | `GuiYeuCauXuLyTiepNhan(...)` | `ServiceTicketsController.CreateTicket(...)` | API POST `/api/service-tickets` nhận yêu cầu tạo phiếu dịch vụ mới từ UI. |
| **Tạo phiếu** | `XuLyTiepNhanBaoHanh(...)` | `ServiceTicketService.CreateTicketFromSerialScanAsync(...)` | Khởi tạo Transaction, sinh mã phiếu tự động dạng `ST-yyyyMMdd-XXX` và lưu vào DB. |
| **Alt: Từ chối** | `[3a. Từ chối bảo hành (Vi phạm chính sách)]` | `InvalidOperationException` hoặc Client UI | **[Khác biệt logic]** Không có bảng `PhieuTuChoiBaoHanh`. Lỗi từ chối được nhân viên quyết định trên UI hoặc hệ thống ném exception nếu vi phạm validation. |
| **Tạo RMA** | `TaoPhieuRMATiepNhan(...)` <br> $\rightarrow$ `luuPhieuThanhCong` | `_ticketRepository.AddAsync(ticket)` <br> & `_ticketRepository.AddStatusHistoryAsync` | Lưu thực thể `ServiceTicket` với trạng thái ban đầu là `0` (Đã tiếp nhận) và lưu lịch sử trạng thái đầu tiên. |
| **Chuyển kho** | `ChuyenSerialSangKhoBaoHanh(maSerial)` | Không có code tương ứng | **[Khác biệt kiến trúc]** Ở bước tiếp nhận, serial vẫn ở trạng thái `Sold` thuộc sở hữu khách hàng. Không chuyển sang kho bảo hành vật lý nào trên DB. |
| **Biên nhận** | `TraVeThanhCong()` <br> $\rightarrow$ `YeuCauInBienNhan()` | `ServiceTicketDetailDto` | Trả về thông tin chi tiết phiếu dịch vụ vừa tạo để UI kích hoạt in biên nhận bàn giao cho khách. |
| **Opt: Cập nhật** | `ChonHuongXuLyRMA(...)` <br> $\rightarrow$ `YeuCauCapNhatRMA(...)` | `ServiceTicketsController.ChooseBranch(...)` | API PUT `/api/service-tickets/{id}/branch` cập nhật `ResolutionType` (Sửa nội bộ, gửi hãng, đổi 1-1, sửa tính phí). |
| **Alt: Đổi 1-1** | `[4a. Xử lý đổi mới 1-1]` | `ServiceTicketService.Perform1For1SwapAsync(...)` | Thực hiện hoán đổi thiết bị mới từ kho của cửa hàng cho khách hàng trong một Transaction. |
| **Đổi 1-1 DB** | `TruTonKhoSerialMoiVaGanDon(...)` | `newSerial.Status = Sold`, `oldOrderSerial.SerialId = newSerialId`, `newWarranty` | Cập nhật serial mới sang `Sold`, gán vào đơn hàng gốc, hủy bảo hành cũ và tạo bảo hành mới kế thừa thời gian bảo hành gốc. |
| **Đổi 1-1 DB** | `ChuyenSerialCuSangKhoLoi(...)` | `oldSerial.Status = Defective` | Đổi trạng thái serial lỗi cũ sang `Defective` (Hỏng/Thu hồi) để chuyển vào kho lỗi kỹ thuật. |
| **Đổi 1-1 DB** | `HoanTatPhieuRMA(idPhieu)` | `ticket.Status = Swapped (8)` <br> & `_logRepository.AddAsync` | Cập nhật trạng thái phiếu dịch vụ thành `Swapped` (Đã đổi 1-1) và ghi nhận nhật ký lỗi `SerialRepairLog` của máy cũ. |
| **Alt: Gửi hãng** | `[4b. Cập nhật trạng thái Gửi trả Hãng]` | `CreateRmaShipmentAsync(...)` <br> & `RecordRmaResolutionAsync(...)` | 1. POST `/api/service-tickets/{id}/rma` tạo phiếu RMA gửi hãng (Status = `6 - RmaSent`). <br> 2. PUT `/resolution` cập nhật phản hồi từ hãng (Nhận lại sửa xong hoặc đổi mới). |
| **Alt: Hoàn tất** | `[4c. Hoàn tất trả hàng cho khách]` | `MarkInternalRepairCompletedAsync(...)` <br> & `IssueServiceInvoiceAsync(...)` | 1. POST `/complete` đổi trạng thái thành `9 - Completed`. <br> 2. POST `/invoice` lập hóa đơn dịch vụ thanh toán linh kiện/tiền công và đổi trạng thái sang `10 - Invoiced`. |
| **Bàn giao** | `TraLaiQuyenSoHuuSeri(maSerial)` | Không cần cập nhật DB | **[Khác biệt logic]** Quyền sở hữu serial của khách không thay đổi (vẫn liên kết `OrderId` và `CustomerId`). Chỉ cần bàn giao máy và đóng phiếu. |

---

## 2. Những điểm khác biệt quan trọng giữa Sơ đồ và Code

### 1. Xử lý khi thiết bị Hết hạn bảo hành (Paid Repair Support)
* **Trong sơ đồ:** Khi kiểm tra thời hạn bảo hành mà phát hiện hết hạn (`2b. Hết hạn bảo hành`), quy trình lập tức báo lỗi lên giao diện và kết thúc tiến trình tiếp nhận.
* **Trong thực tế Code:** Hệ thống **vẫn cho phép tiếp nhận sửa chữa có tính phí**.
  * Trong `ServiceTicketService.GetWarrantyEvaluationAsync` (dòng 95):
    ```csharp
    else // Nếu thiết bị đã hết hạn bảo hành.
    {
        allowedBranches.Add("PaidRepair"); // Chỉ cho phép thực hiện sửa chữa có tính phí dịch vụ.
    }
    ```
  * Thay vì từ chối khách hàng, hệ thống điều hướng phiếu sang luồng sửa tính phí (`PaidRepair` - giá trị 4). Nhân viên kỹ thuật sẽ lập báo giá (`CreateQuotationAsync`), khách duyệt đồng ý (`AcceptQuotationAsync`) thì mới sửa chữa và xuất hóa đơn dịch vụ (`IssueServiceInvoiceAsync`). Điều này tối ưu hóa doanh thu cửa hàng và dịch vụ khách hàng trong thực tế.

### 2. Trạng thái và vị trí vật lý của Serial lúc tiếp nhận (Intake Inventory Status)
* **Trong sơ đồ:** Mô tả việc gọi lệnh `ChuyenSerialSangKhoBaoHanh(maSerial)` ngay khi tiếp nhận phiếu bảo hành để đưa máy vào kho bảo hành trung tâm.
* **Trong thực tế Code:** **Trạng thái serial của khách hàng trong DB vẫn được giữ nguyên là `Sold` (Đã bán)**.
  * Hệ thống chỉ tạo phiếu dịch vụ `ServiceTicket` và liên kết với mã serial. Thiết bị lúc này thuộc quyền sở hữu của khách hàng đang gửi sửa, không phải hàng tồn kho của cửa hàng.
  * Trạng thái của serial cũ chỉ chuyển sang `Defective` (Hỏng - giá trị 4) khi cửa hàng thực hiện **Đổi mới 1-1** (Khách nhận máy mới khác, máy cũ thu hồi thành tài sản hỏng của cửa hàng).

### 3. Quy trình Từ chối bảo hành (RMA Rejection Process)
* **Trong sơ đồ:** Có luồng `LuuPhieuTuChoiBaoHanh` riêng biệt xuống DB khi thiết bị vi phạm chính sách bảo hành (va đập, vào nước...) và yêu cầu trả máy trực tiếp cho khách.
* **Trong thực tế Code:** **Không có thực thể hoặc bảng `PhieuTuChoiBaoHanh`**.
  * Nếu thiết bị vi phạm chính sách bảo hành, nhân viên kỹ thuật có thể từ chối bảo hành miễn phí và cập nhật thông tin chẩn đoán lỗi vào hệ thống.
  * Phiếu lúc này có thể được chuyển đổi hướng xử lý sang sửa chữa tính phí (`PaidRepair`) nếu khách hàng đồng ý trả tiền, hoặc hủy phiếu dịch vụ thông qua API `CancelTicketAsync` (chỉ dành cho Admin) và bàn giao trả máy lại cho khách.

### 4. Cơ chế Kế thừa Bảo hành khi đổi 1-1 (Warranty Inheritance)
* **Trong sơ đồ:** Quy trình đổi mới 1-1 chỉ đề cập đến việc trừ kho máy mới và chuyển máy cũ sang kho lỗi mà chưa làm rõ cơ chế bảo hành của thiết bị mới.
* **Trong thực tế Code:** Backend triển khai cơ chế **kế thừa hạn bảo hành gốc** chặt chẽ để tránh gian lận thương mại (khách hàng liên tục đổi máy mới để gia hạn bảo hành vô hạn):
  * Khi đổi sang serial mới (`newSerialId`), hệ thống hủy kích hoạt bản ghi bảo hành của serial cũ (`Status = Cancelled`).
  * Tạo bản ghi bảo hành mới (`Warranty`) cho serial mới nhưng gán ngày hết hạn bảo hành (`EndDate`) bằng đúng ngày hết hạn bảo hành gốc của thiết bị cũ (`EndDate = oldEndDate`).

### 5. Quản lý Quyền sở hữu và Bàn giao thiết bị
* **Trong sơ đồ:** Mô tả hành động `TraLaiQuyenSoHuuSeri(maSerial)` khi hoàn tất trả máy cho khách hàng.
* **Trong thực tế Code:** Đối với trường hợp sửa chữa thông thường (không đổi máy), hệ thống không cần thực thi câu lệnh SQL nào để cập nhật lại quyền sở hữu, vì trong suốt thời gian sửa chữa thiết bị vẫn được liên kết với `CustomerId` và `OrderId` gốc. Hệ thống chỉ phát hành hóa đơn dịch vụ sửa chữa (`ServiceInvoice`) và cập nhật trạng thái phiếu dịch vụ thành `HandedOver` (Đã bàn giao) để kết thúc quy trình.

---

## 3. Đánh giá & Khuyến nghị cho Đồ án/Bảo vệ môn học

* **Giải trình về phân hệ Sửa chữa tính phí (Paid Repair):** Đây là điểm chứng minh hệ thống có tính thực tiễn cao. Khi bảo vệ đồ án, hãy nhấn mạnh rằng phần mềm không chỉ hỗ trợ bảo hành sản phẩm miễn phí (RMA/Internal Repair) mà còn hỗ trợ quy trình sửa dịch vụ lấy tiền công & linh kiện cho sản phẩm hết hạn bảo hành, bao gồm đầy đủ quy trình lập báo giá, chờ khách duyệt và xuất hóa đơn dịch vụ.
* **Điểm nhấn về logic Đổi mới 1-1 (1-For-1 Swap):** Việc hoán đổi serial mới vào hóa đơn gốc (`OrderSerials`) và kế thừa thời hạn bảo hành cũ (`Warranty.EndDate`) thể hiện tư duy thiết kế cơ sở dữ liệu và nghiệp vụ rất chặt chẽ, hãy đưa phần giải trình này vào bài bảo vệ để thuyết phục hội đồng phản biện.
* **Hiệu chỉnh sơ đồ:** Để sơ đồ khớp 100% với thực tế dự án:
  * Loại bỏ bước `ChuyenSerialSangKhoBaoHanh` lúc tiếp nhận và bước `TraLaiQuyenSoHuuSeri` ở cuối.
  * Bổ sung luồng xử lý báo giá (`Quotation`) và duyệt báo giá đối với nhánh sửa tính phí (`PaidRepair`).
  * Thêm chi tiết về việc cập nhật liên kết đơn hàng cũ (`OrderSerials`) và tạo bản ghi bảo hành kế thừa thời hạn khi thực hiện đổi trả 1-1.

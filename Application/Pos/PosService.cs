using System; // Sử dụng các kiểu dữ liệu cơ bản của hệ thống .NET.
using System.Collections.Generic; // Sử dụng các lớp cấu trúc dữ liệu danh sách như List, Dictionary.
using System.Linq; // Sử dụng các phương thức mở rộng LINQ để xử lý tập hợp.
using System.Threading.Tasks; // Sử dụng lập trình bất đồng bộ Task.
using Microsoft.AspNetCore.Identity; // Sử dụng ASP.NET Core Identity quản lý người dùng.
using Microsoft.EntityFrameworkCore; // Sử dụng Entity Framework Core để truy vấn DB.
using PBL3.Core.Entities; // Sử dụng các thực thể nghiệp vụ cốt lõi từ tầng Core.
using PBL3.Core.Interfaces; // Sử dụng các giao diện repository của tầng Core.
using PBL3.Infrastructure.Data; // Sử dụng DbContext để thực hiện các câu lệnh truy vấn dữ liệu trực tiếp.
using PBL3.Shared.DTOs.Common; // Sử dụng các DTO phản hồi dùng chung ApiResult.
using PBL3.Shared.DTOs.Pos; // Sử dụng các DTO phục vụ cho nghiệp vụ bán hàng tại quầy POS.
using PBL3.Shared.Enums; // Sử dụng các enum mô tả trạng thái của hệ thống.

namespace PBL3.Application.Pos // Khai báo namespace cho tầng Application của module POS.
{
    public class PosService( // Khai báo lớp PosService sử dụng Primary Constructor.
        IUnitOfWork unitOfWork, // Nhận UnitOfWork để quản lý Transaction.
        IOrderRepository orderRepo, // Nhận repository quản lý đơn hàng.
        IProductSerialRepository serialRepo, // Nhận repository quản lý mã số Serial.
        IVoucherRepository voucherRepo, // Nhận repository quản lý mã giảm giá.
        IWarrantyRepository warrantyRepo, // Nhận repository quản lý bảo hành.
        IProductRepository productRepo, // Nhận repository quản lý sản phẩm.
        IInventorySyncService inventorySyncService, // Nhận dịch vụ đồng bộ kho hàng.
        HushStoreDbContext dbContext, // Nhận DbContext để truy vấn trực tiếp.
        UserManager<AppUser> userManager) : IPosService // Nhận UserManager quản lý tài khoản người dùng và kế thừa IPosService.
    {
        private readonly IUnitOfWork _unitOfWork = // Khai báo trường lưu trữ UnitOfWork.
            unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork)); // Gán giá trị và kiểm tra null.
        private readonly IOrderRepository _orderRepo = // Khai báo trường lưu trữ repository đơn hàng.
            orderRepo ?? throw new ArgumentNullException(nameof(orderRepo)); // Gán giá trị và kiểm tra null.
        private readonly IProductSerialRepository _serialRepo = // Khai báo trường lưu trữ repository serial.
            serialRepo ?? throw new ArgumentNullException(nameof(serialRepo)); // Gán giá trị và kiểm tra null.
        private readonly IVoucherRepository _voucherRepo = // Khai báo trường lưu trữ repository voucher.
            voucherRepo ?? throw new ArgumentNullException(nameof(voucherRepo)); // Gán giá trị và kiểm tra null.
        private readonly IWarrantyRepository _warrantyRepo = // Khai báo trường lưu trữ repository bảo hành.
            warrantyRepo ?? throw new ArgumentNullException(nameof(warrantyRepo)); // Gán giá trị và kiểm tra null.
        private readonly IProductRepository _productRepo = // Khai báo trường lưu trữ repository sản phẩm.
            productRepo ?? throw new ArgumentNullException(nameof(productRepo)); // Gán giá trị và kiểm tra null.
        private readonly IInventorySyncService _inventorySyncService = // Khai báo trường lưu trữ dịch vụ đồng bộ kho.
            inventorySyncService ?? throw new ArgumentNullException(nameof(inventorySyncService)); // Gán giá trị và kiểm tra null.
        private readonly HushStoreDbContext _dbContext = // Khai báo trường lưu trữ DbContext phục vụ tra cứu nhanh.
            dbContext ?? throw new ArgumentNullException(nameof(dbContext)); // Gán giá trị và kiểm tra null.
        private readonly UserManager<AppUser> _userManager = // Khai báo trường lưu trữ UserManager.
            userManager ?? throw new ArgumentNullException(nameof(userManager)); // Gán giá trị và kiểm tra null.

        public async Task<ApiResult<PosScanResponse>> ScanSerialAsync(string serialNumber) // Phương thức quét mã vạch (Serial) của thiết bị tại quầy POS.
        {
            var serial = await _serialRepo.GetBySerialNumberAsync(serialNumber); // Tra cứu thông tin Serial Number từ repository.
            
            if (serial == null) // Nếu không tìm thấy số Serial trong hệ thống.
            {
                return ApiResult<PosScanResponse>.Fail("Mã Serial không hợp lệ hoặc không có trong kho.", ApiErrorCode.NotFound); // Trả về mã lỗi NotFound.
            }

            if (serial.Status != (byte)SerialStatus.Available) // Nếu thiết bị không ở trạng thái sẵn sàng bán (Available - 0).
            {
                return ApiResult<PosScanResponse>.Fail("Sản phẩm không ở trạng thái sẵn sàng bán (đã bán hoặc lỗi)."); // Trả về thông báo chặn bán.
            }

            var variant = serial.Variant; // Lấy thông tin phiên bản sản phẩm liên quan.
            var product = variant?.Product; // Lấy thông tin dòng sản phẩm cha.

            if (variant == null || product == null) // Nếu biến thể hoặc sản phẩm bị khuyết thiếu.
                return ApiResult<PosScanResponse>.Fail("Không thể lấy thông tin sản phẩm. Biến thể hoặc sản phẩm không còn tồn tại trong hệ thống.", ApiErrorCode.NotFound); // Trả về lỗi NotFound.

            return ApiResult<PosScanResponse>.Ok(new PosScanResponse // Trả về kết quả quét thành công kèm theo DTO chứa thông tin sản phẩm.
            {
                SerialId = serial.Id, // Gán Id serial.
                SerialNumber = serial.SerialNumber, // Gán số Serial.
                VariantId = variant.Id, // Gán Id biến thể.
                SKU = variant.SKU, // Gán mã SKU sản phẩm.
                VariantName = variant.VariantName, // Gán tên biến thể.
                ProductName = product.Name, // Gán tên dòng sản phẩm.
                Price = variant.Price, // Gán đơn giá hiện hành.
                WarrantyMonth = variant.WarrantyMonth // Gán số tháng bảo hành mặc định.
            }); // Kết thúc trả về.
        }

        public async Task<ApiResult<PosCustomerDto>> LookupCustomerAsync(string phone) // Phương thức tra cứu thông tin khách hàng thành viên tại quầy bằng số điện thoại.
        {
            var user = await _dbContext.Users // Thực hiện truy vấn trực tiếp bảng Users để tìm tài khoản.
                .Include(u => u.Profile) // Nạp thêm thông tin hồ sơ đi kèm (UserProfile).
                .AsNoTracking() // Không theo dõi trạng thái thực thể để tối ưu hiệu năng truy vấn.
                .FirstOrDefaultAsync(u => u.PhoneNumber == phone); // Lọc theo số điện thoại trùng khớp.

            if (user == null) // Nếu không tìm thấy người dùng nào.
            {
                return ApiResult<PosCustomerDto>.Fail("Không tìm thấy khách hàng.", ApiErrorCode.NotFound); // Trả về mã lỗi NotFound.
            }

            return ApiResult<PosCustomerDto>.Ok(new PosCustomerDto // Trả về thông tin khách hàng tìm kiếm được.
            {
                UserId = user.Id, // Gán UserId.
                FullName = user.Profile?.FullName ?? user.UserName ?? "Khách hàng", // Gán họ tên đầy đủ, dự phòng tên tài khoản.
                PhoneNumber = user.PhoneNumber ?? phone, // Gán số điện thoại.
                Email = user.Email // Gán địa chỉ email.
            }); // Kết thúc trả về.
        }

        public async Task<ApiResult<VoucherValidationDto>> ValidateVoucherAsync(string code, decimal subTotal) // Phương thức kiểm tra tính hợp lệ của mã giảm giá khi áp dụng trực tiếp tại quầy.
        {
            var vouchers = await _voucherRepo.GetByCodesAsync(new List<string> { code }); // Tra cứu thực thể Voucher từ database.
            var voucher = vouchers.FirstOrDefault(); // Lấy phần tử đầu tiên trong kết quả trả về.

            if (voucher == null || !voucher.IsActive) // Nếu không tìm thấy hoặc voucher đang bị vô hiệu hóa.
            {
                return ApiResult<VoucherValidationDto>.Fail("Mã Voucher không tồn tại hoặc đã bị khóa."); // Báo lỗi.
            }

            var now = DateTime.UtcNow; // Lấy thời gian UTC hiện tại.
            if (now < voucher.StartDate || now > voucher.EndDate) // Nếu nằm ngoài khoảng thời gian cho phép sử dụng.
            {
                return ApiResult<VoucherValidationDto>.Fail("Mã Voucher đã hết hạn hoặc chưa đến thời gian sử dụng."); // Báo lỗi hết hạn.
            }

            if (voucher.UsedCount >= voucher.Quantity) // Nếu voucher đã dùng hết số lượt phát hành toàn hệ thống.
            {
                return ApiResult<VoucherValidationDto>.Fail("Mã Voucher đã hết lượt sử dụng."); // Báo lỗi hết lượt.
            }

            if (subTotal < voucher.MinOrderValue) // Nếu tổng tiền đơn hàng chưa đạt ngưỡng giá trị đơn hàng tối thiểu.
            {
                return ApiResult<VoucherValidationDto>.Fail($"Đơn hàng chưa đạt giá trị tối thiểu {voucher.MinOrderValue:#,0}đ."); // Báo lỗi tối thiểu đơn hàng.
            }

            decimal discountApplied = 0; // Khởi tạo số tiền được chiết khấu.
            if (voucher.DiscountType == 0) // Loại 0: Giảm tiền mặt cố định.
            {
                discountApplied = voucher.DiscountValue; // Gán tiền giảm bằng chính giá trị định nghĩa.
            }
            else // Loại 1: Giảm theo tỷ lệ phần trăm.
            {
                discountApplied = subTotal * voucher.DiscountValue / 100; // Tính toán tiền giảm theo tỷ lệ.
                if (voucher.MaxDiscountAmount.HasValue && discountApplied > voucher.MaxDiscountAmount.Value) // Nếu vượt quá mức trần tối đa cho phép.
                {
                    discountApplied = voucher.MaxDiscountAmount.Value; // Giới hạn bằng giá trị trần tối đa.
                }
            } // Kết thúc tính toán.

            return ApiResult<VoucherValidationDto>.Ok(new VoucherValidationDto // Trả về DTO chứa thông tin giảm giá hợp lệ.
            {
                IsValid = true, // Cờ hợp lệ là true.
                Code = voucher.Code, // Gán mã code.
                DiscountAmount = discountApplied, // Gán số tiền được giảm.
                Message = "Áp dụng Voucher thành công." // Gán lời nhắn thành công.
            }); // Kết thúc trả về.
        }

        public async Task<ApiResult<PosOrderDto>> CheckoutAsync(PosCheckoutRequest request, Guid employeeId) // Phương thức thanh toán xuất hóa đơn chính thức tại quầy POS.
        {
            if (request.Items == null || !request.Items.Any()) // Nếu giỏ hàng bán trực tiếp tại quầy trống.
            {
                return ApiResult<PosOrderDto>.Fail("Giỏ hàng trống."); // Báo lỗi.
            }

            decimal subTotal = 0; // Khởi tạo tổng tiền hàng trước giảm giá.
            var serialsToUpdate = new List<ProductSerial>(); // Khởi tạo danh sách lưu các serial cần cập nhật trạng thái bán.
            var newWarranties = new List<Warranty>(); // Khởi tạo danh sách lưu các bản ghi bảo hành cần tạo mới.
            var variantIdsToSync = new HashSet<int>(); // Khởi tạo tập hợp lưu các Id biến thể cần đồng bộ tồn kho.

            Guid? customerId = null; // Khởi tạo Id khách mua hàng thành viên là null.
            string? customerName = null; // Khởi tạo tên khách hàng là null.
            if (!string.IsNullOrEmpty(request.CustomerPhone)) // Nếu nhân viên cung cấp số điện thoại khách hàng.
            {
                var userLookup = await LookupCustomerAsync(request.CustomerPhone); // Tra cứu tài khoản thành viên trong hệ thống.
                if (userLookup.Success && userLookup.Data != null) // Nếu tìm thấy thông tin hợp lệ.
                {
                    customerId = userLookup.Data.UserId; // Gán UserId khách hàng.
                    customerName = userLookup.Data.FullName; // Gán tên khách hàng thành viên.
                }
            }

            var orderDetailsMap = new Dictionary<int, OrderDetail>(); // Gom nhóm các Serial vật lý theo biến thể (VariantId) để đưa vào chi tiết đơn hàng (OrderDetail).

            foreach (var item in request.Items) // Duyệt qua từng sản phẩm quét được gửi lên.
            {
                var dbSerial = await _serialRepo.GetByIdWithTrackingAsync(item.SerialId); // Tra cứu thực thể số Serial ở chế độ tracking.

                if (dbSerial == null || dbSerial.Status != (byte)SerialStatus.Available) // Đảm bảo thiết bị còn tồn tại và đang sẵn sàng để bán.
                {
                    return ApiResult<PosOrderDto>.Fail($"Sản phẩm có mã SerialId {item.SerialId} không tồn tại hoặc đã bán."); // Báo lỗi chặn thanh toán.
                }

                subTotal += dbSerial.Variant.Price; // Cộng dồn đơn giá biến thể sản phẩm vào tổng tiền hàng.
                variantIdsToSync.Add(dbSerial.VariantId); // Đưa VariantId vào danh sách cần đồng bộ tồn kho.
                serialsToUpdate.Add(dbSerial); // Đưa thực thể serial vào danh sách cần cập nhật trạng thái.

                if (!orderDetailsMap.ContainsKey(dbSerial.VariantId)) // Nếu chưa tồn tại dòng chi tiết đơn hàng cho biến thể này trong Map.
                {
                    orderDetailsMap[dbSerial.VariantId] = new OrderDetail // Tạo mới dòng chi tiết đơn hàng gộp.
                    {
                        VariantId = dbSerial.VariantId, // Gán Id biến thể.
                        Quantity = 0, // Khởi tạo số lượng bằng 0.
                        UnitPrice = dbSerial.Variant.Price, // Gán đơn giá bán hiện hành.
                    };
                }
                orderDetailsMap[dbSerial.VariantId].Quantity++; // Tăng số lượng mua của biến thể sản phẩm này lên 1 đơn vị.
            }

            decimal discountAmount = 0; // Khởi tạo tổng số tiền giảm giá bằng 0.
            Voucher? appliedVoucher = null; // Khởi tạo đối tượng voucher áp dụng là null.
            if (!string.IsNullOrEmpty(request.VoucherCode)) // Nếu nhân viên nhập mã giảm giá trực tiếp tại quầy.
            {
                var validateRes = await ValidateVoucherAsync(request.VoucherCode, subTotal); // Thực hiện kiểm tra tính hợp lệ của voucher.
                if (!validateRes.Success) // Nếu không hợp lệ.
                    return ApiResult<PosOrderDto>.Fail(validateRes.Message); // Trả về thông báo lỗi chi tiết của voucher.

                discountAmount = validateRes.Data!.DiscountAmount; // Lấy số tiền giảm được phê duyệt.
                appliedVoucher = await _dbContext.Vouchers.FirstOrDefaultAsync(v => v.Code == request.VoucherCode); // Truy vấn thực thể voucher từ DB.
            }

            discountAmount = Math.Min(discountAmount, subTotal); // Giới hạn tiền giảm tối đa không vượt quá tổng giá trị hàng.
            decimal totalAmount = subTotal - discountAmount; // Tính tổng tiền thanh toán sau cùng của hóa đơn.

            string datePrefix = "POS-" + DateTime.Now.ToString("yyyyMMdd"); // Tạo tiền tố mã hóa đơn POS theo ngày đặc thù POS-yyyyMMdd.
            string? lastCode = await _orderRepo.GetLastOrderCodeByDateAsync(datePrefix); // Tra cứu mã hóa đơn POS lớn nhất được xuất trong ngày hôm nay.
            int nextIndex = 1; // Khởi tạo số thứ tự mặc định là 1.
            if (!string.IsNullOrEmpty(lastCode)) // Nếu trong ngày đã tồn tại hóa đơn bán tại quầy khác.
            {
                string suffix = lastCode.Substring(lastCode.LastIndexOf('-') + 1); // Cắt lấy chuỗi số thứ tự ở đuôi.
                if (int.TryParse(suffix, out int lastIndex)) // Chuyển chuỗi số sang số nguyên thành công.
                {
                    nextIndex = lastIndex + 1; // Số thứ tự tiếp theo tăng thêm 1 đơn vị.
                }
            }
            string newOrderCode = $"{datePrefix}-{nextIndex:D3}"; // Tạo mã hóa đơn hoàn chỉnh (ví dụ: POS-20260531-001).

            var order = new Order // Khởi tạo thực thể đơn hàng bán tại quầy mới.
            {
                OrderCode = newOrderCode, // Gán mã hóa đơn tự sinh.
                UserId = customerId, // Gán Id khách hàng (thành viên nếu có).
                EmployeeId = employeeId, // Gán Id nhân viên thu ngân xuất hóa đơn.
                OrderDate = DateTime.UtcNow, // Gán ngày bán.
                Status = (byte)OrderStatus.Success, // Giao dịch tại quầy mặc định ở trạng thái Success (Thành công - 3).
                OrderType = (byte)OrderType.POS, // Loại hình đơn hàng là bán tại quầy POS (1).
                ShipName = customerName ?? "Khách vãng lai", // Gán tên khách hàng hoặc mặc định Khách vãng lai.
                ShipPhone = request.CustomerPhone ?? "", // Gán số điện thoại khách.
                ShipAddress = !string.IsNullOrWhiteSpace(request.ShipAddress) ? request.ShipAddress : "Tại quầy", // Địa chỉ mua hàng tại quầy.
                ShipCity = !string.IsNullOrWhiteSpace(request.ShipCity) ? request.ShipCity : "Tại quầy", // Thành phố mua hàng.
                SubTotal = subTotal, // Gán tổng tiền trước giảm.
                ShippingFee = 0, // Không phát sinh phí vận chuyển.
                DiscountAmount = discountAmount, // Gán tiền chiết khấu giảm giá.
                TotalAmount = totalAmount, // Gán tổng số tiền thanh toán.
                PaymentMethod = request.PaymentMethod, // Gán phương thức thanh toán.
                PaymentStatus = 1, // Thanh toán tại quầy mặc định ở trạng thái Đã thanh toán (Paid - 1).
                Note = request.EmployeeNote // Ghi chú của nhân viên thu ngân.
            };

            await _unitOfWork.BeginTransactionAsync(); // Khởi tạo Transaction bảo đảm đồng bộ nhiều bảng liên quan.
            try // Bắt đầu khối an toàn.
            {
                await _orderRepo.AddAsync(order); // Thêm đơn hàng vào cơ sở dữ liệu.
                await _unitOfWork.SaveChangesAsync(); // Lưu thay đổi để lấy Id tự sinh của đơn hàng.

                foreach (var od in orderDetailsMap.Values) // Duyệt qua từng dòng chi tiết đơn hàng gộp.
                {
                    od.OrderId = order.Id; // Gán liên kết Id đơn hàng cha.
                    await _dbContext.OrderDetails.AddAsync(od); // Lưu dòng chi tiết vào DB.
                }
                await _unitOfWork.SaveChangesAsync(); // Lưu thay đổi chi tiết đơn hàng để lấy Id tự sinh.

                var now = DateTime.UtcNow; // Lấy thời gian hiện tại.
                foreach (var s in serialsToUpdate) // Duyệt qua từng serial thiết bị vật lý xuất bán.
                {
                    s.Status = (byte)SerialStatus.Sold; // Cập nhật trạng thái serial sang Đã bán (2).
                    s.SoldDate = now; // Ghi nhận ngày bán thiết bị.
                    s.OrderId = order.Id; // Liên kết serial với Id hóa đơn bán hàng.

                    var od = orderDetailsMap[s.VariantId]; // Lấy dòng chi tiết đơn hàng tương ứng với biến thể sản phẩm của serial.
                    await _dbContext.OrderSerials.AddAsync(new OrderSerial // Thêm bản ghi liên kết thiết bị vật lý cụ thể vào dòng hóa đơn.
                    {
                        OrderDetailId = od.Id, // Khóa ngoại dòng chi tiết hóa đơn.
                        SerialId = s.Id // Khóa ngoại serial thiết bị.
                    });

                    if (s.Variant.WarrantyMonth > 0) // Nghiệp vụ: Tự động kích hoạt phiếu bảo hành nếu sản phẩm có cấu hình bảo hành.
                    {
                        newWarranties.Add(new Warranty // Tạo mới bản ghi bảo hành.
                        {
                            SerialId = s.Id, // Khóa ngoại serial thiết bị.
                            CustomerId = customerId, // Khóa ngoại khách hàng sở hữu.
                            OrderId = order.Id, // Khóa ngoại đơn hàng.
                            StartDate = now, // Ngày bắt đầu là ngày mua.
                            EndDate = now.AddMonths(s.Variant.WarrantyMonth), // Ngày hết hạn = ngày mua + số tháng bảo hành.
                            Status = (byte)WarrantyStatus.Active // Trạng thái bảo hành hoạt động bình thường (0 - Active).
                        });
                    }
                }

                if (newWarranties.Any()) // Nếu có bản ghi bảo hành cần tạo mới.
                {
                    await _warrantyRepo.AddRangeAsync(newWarranties); // Lưu danh sách bảo hành vào cơ sở dữ liệu.
                }

                if (appliedVoucher != null) // Nếu có áp dụng voucher giảm giá.
                {
                    appliedVoucher.UsedCount++; // Tăng lượt sử dụng của voucher lên 1 đơn vị.
                    if (customerId.HasValue) // Nếu là khách hàng thành viên (có tài khoản).
                    {
                        await _dbContext.VoucherUsages.AddAsync(new VoucherUsage // Lưu lịch sử sử dụng voucher để kiểm soát giới hạn dùng cá nhân.
                        {
                            VoucherId = appliedVoucher.Id, // Id voucher.
                            UserId = customerId.Value, // Id người dùng.
                            OrderId = order.Id, // Id đơn hàng áp dụng.
                            DiscountApplied = discountAmount, // Số tiền được giảm thực tế.
                            UsedDate = now // Thời điểm dùng.
                        });
                    }
                }

                await _unitOfWork.SaveChangesAsync(); // Lưu toàn bộ các thay đổi xuống DB.
                await _unitOfWork.CommitAsync(); // Xác nhận transaction hoàn tất thành công.

                await _inventorySyncService.SyncStockBatchAsync(variantIdsToSync); // Kích hoạt đồng bộ tồn kho khả dụng của biến thể sản phẩm vừa bán trong hệ thống RAM.

                return ApiResult<PosOrderDto>.Ok(new PosOrderDto // Trả về DTO chứa thông tin đơn hàng POS vừa thanh toán.
                {
                    OrderId = order.Id, // Gán Id đơn hàng.
                    OrderCode = order.OrderCode, // Gán mã đơn hàng.
                    OrderDate = order.OrderDate, // Gán ngày lập đơn.
                    SubTotal = order.SubTotal, // Gán tổng tiền trước giảm.
                    DiscountAmount = order.DiscountAmount, // Gán số tiền được giảm.
                    TotalAmount = order.TotalAmount, // Gán tổng số tiền thanh toán cuối cùng.
                    CustomerName = order.ShipName, // Gán tên khách hàng.
                    CustomerPhone = order.ShipPhone // Gán số điện thoại liên hệ.
                }, "Thanh toán thành công."); // Trả về kèm lời nhắn.
            } // Kết thúc khối try.
            catch (Exception ex) // Bắt lỗi hệ thống.
            {
                await _unitOfWork.RollbackAsync(); // Khôi phục trạng thái DB trước transaction để đảm bảo an toàn dữ liệu.
                return ApiResult<PosOrderDto>.Fail("Lỗi khi quá trình thanh toán: " + ex.Message); // Trả về thông báo lỗi.
            } // Kết thúc khối catch.
        }

        public async Task<ApiResult<PosDraftDto>> SaveDraftAsync(PosCheckoutRequest request, Guid employeeId) // Phương thức lưu tạm đơn hàng nháp tại quầy POS.
        {
            return await Task.FromResult(ApiResult<PosDraftDto>.Fail("Tính năng lưu tạm đang được xây dựng (cần thống nhất cách lưu trữ SerialDraft).")); // Trả về thông báo tính năng đang xây dựng.
        }

        public async Task<ApiResult<List<PosDraftDto>>> GetDraftsAsync(Guid employeeId) // Phương thức lấy danh sách các đơn hàng nháp hiện có của nhân viên POS.
        {
            var drafts = await _orderRepo.GetDraftsByEmployeeAsync(employeeId); // Tra cứu danh sách đơn nháp của nhân viên từ repository.
            var dtos = drafts.Select(d => new PosDraftDto // Ánh xạ danh sách thực thể thu được sang danh sách DTO rút gọn.
            {
                OrderId = d.Id, // Gán Id đơn hàng.
                OrderCode = d.OrderCode, // Gán mã đơn hàng.
                OrderDate = d.OrderDate, // Gán ngày tạo đơn.
                TotalAmount = d.TotalAmount // Gán tổng tiền thanh toán của đơn nháp.
            }).ToList(); // Đóng gói thành danh sách List.

            return ApiResult<List<PosDraftDto>>.Ok(dtos); // Trả về kết quả thành công chứa danh sách đơn nháp.
        }

        public async Task<ApiResult<PosDraftDto>> GetDraftByIdAsync(int orderId, Guid employeeId) // Phương thức lấy chi tiết đơn nháp theo Id và employeeId.
        {
             var draft = await _dbContext.Orders.AsNoTracking().FirstOrDefaultAsync(o => o.Id == orderId && o.EmployeeId == employeeId && o.Status == (byte)OrderStatus.PosDraft); // Tra cứu đơn hàng nháp khớp điều kiện.
             if (draft == null) return ApiResult<PosDraftDto>.Fail("Không tìm thấy đơn chờ.", ApiErrorCode.NotFound); // Trả về lỗi NotFound nếu không tìm thấy đơn nháp hợp lệ.
             return ApiResult<PosDraftDto>.Ok(new PosDraftDto { // Trả về kết quả.
                OrderId = draft.Id, // Gán Id.
                OrderCode = draft.OrderCode, // Gán mã đơn.
                OrderDate = draft.OrderDate, // Gán ngày tạo.
                TotalAmount = draft.TotalAmount // Gán tổng tiền.
             }); // Kết thúc trả về.
        }

        public async Task<ApiResult<bool>> DeleteDraftAsync(int orderId, Guid employeeId) // Phương thức xóa đơn hàng nháp tại quầy POS.
        {
             var draft = await _dbContext.Orders.FirstOrDefaultAsync(o => o.Id == orderId && o.EmployeeId == employeeId && o.Status == (byte)OrderStatus.PosDraft); // Lấy thực thể đơn nháp từ DB.
             if (draft == null) return ApiResult<bool>.Fail("Không tìm thấy đơn chờ.", ApiErrorCode.NotFound); // Báo lỗi NotFound nếu không tìm thấy.
             
             _dbContext.Orders.Remove(draft); // Thực hiện xóa bản ghi đơn hàng nháp khỏi DbContext.
             await _dbContext.SaveChangesAsync(); // Lưu thay đổi xuống cơ sở dữ liệu.
             return ApiResult<bool>.Ok(true, "Xoá thành công."); // Trả về thông báo thành công.
        }
    }
}

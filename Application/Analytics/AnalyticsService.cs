using Microsoft.EntityFrameworkCore; // Sử dụng Entity Framework Core cho các truy vấn bất đồng bộ.
using Microsoft.Extensions.Logging; // Sử dụng thư viện ghi log hệ thống.
using PBL3.Infrastructure.Data; // Tham chiếu đến lớp ngữ cảnh cơ sở dữ liệu HushStoreDbContext.
using PBL3.Shared.DTOs.Analytics; // Sử dụng các DTO dùng cho báo cáo thống kê.
using PBL3.Shared.DTOs.Common; // Sử dụng DTO dùng chung ApiResult.

namespace PBL3.Application.Analytics // Khai báo namespace cho tầng Application của module thống kê.
{
    public class AnalyticsService( // Sử dụng Primary Constructor để tiêm các dịch vụ.
        HushStoreDbContext context, // Tiêm HushStoreDbContext làm việc với Database.
        ILogger<AnalyticsService> logger) : IAnalyticsService // Tiêm ILogger ghi nhận vết lỗi của AnalyticsService.
    {
        private readonly HushStoreDbContext _context = // Gán database context vào biến thành viên.
            context ?? throw new ArgumentNullException(nameof(context)); // Kiểm tra null cho context.
        private readonly ILogger<AnalyticsService> _logger = // Gán logger vào biến thành viên.
            logger ?? throw new ArgumentNullException(nameof(logger)); // Kiểm tra null cho logger.

        private (bool valid, string error) ValidateRange(DateTime from, DateTime to) // Khai báo hàm phụ trợ kiểm tra khoảng thời gian thống kê hợp lệ.
        {
            if (to < from) // Nếu ngày kết thúc nhỏ hơn ngày bắt đầu.
                return (false, "Ngày kết thúc phải lớn hơn hoặc bằng ngày bắt đầu."); // Trả về kết quả không hợp lệ kèm thông báo lỗi.
            if ((to - from).TotalDays > 366) // Nếu khoảng cách giữa hai mốc thời gian vượt quá 366 ngày (1 năm).
                return (false, "Khoảng thời gian thống kê tối đa là 366 ngày."); // Trả về kết quả không hợp lệ kèm thông báo lỗi.
            return (true, string.Empty); // Trả về kết quả hợp lệ nếu vượt qua các điều kiện kiểm tra trên.
        }

        public async Task<ApiResult<AnalyticsSummaryDto>> GetSummaryAsync(DateTime from, DateTime to) // Định nghĩa hàm lấy dữ liệu báo cáo tài chính tổng quan bất đồng bộ.
        {
            var (valid, error) = ValidateRange(from, to); // Thực hiện kiểm tra tính hợp lệ của khoảng thời gian truy vấn.
            if (!valid) return ApiResult<AnalyticsSummaryDto>.Fail(error); // Nếu khoảng thời gian không hợp lệ, trả về lỗi.
            try // Khối try bắt các ngoại lệ cơ sở dữ liệu hoặc hệ thống.
            {
                var fromDate = from.Date; // Lấy ngày bắt đầu tại thời điểm 00:00:00.
                var toDate = to.Date.AddDays(1); // Lấy ngày kết thúc cộng thêm 1 ngày để bao quát hết dữ liệu của ngày cuối cùng.

                var successQ = _context.Orders.AsNoTracking() // Truy vấn bảng Orders không theo dõi thực thể để tối ưu tốc độ.
                    .Where(o => o.Status == 3 && o.OrderDate >= fromDate && o.OrderDate < toDate); // Lọc các đơn hàng hoàn thành (Status = 3) trong khoảng thời gian.

                var totalRevenue = await successQ.SumAsync(o => (decimal?)o.TotalAmount) ?? 0m; // Tính tổng doanh thu thực tế, mặc định 0 nếu không có đơn hàng.
                var totalOrders = await successQ.CountAsync(); // Đếm tổng số lượng đơn hàng hoàn thành.

                var totalCost = await ( // Thực hiện truy vết giá vốn thực tế của từng mã Serial đã bán ra.
                    from o in _context.Orders // Từ bảng Orders.
                    where o.Status == 3 && o.OrderDate >= fromDate && o.OrderDate < toDate // Lọc đơn hàng thành công trong khoảng thời gian.
                    join od in _context.OrderDetails on o.Id equals od.OrderId // Join với chi tiết đơn hàng (OrderDetail).
                    join os in _context.OrderSerials on od.Id equals os.OrderDetailId // Join với danh sách Serial đã bán trong đơn (OrderSerial).
                    join ps in _context.ProductSerials on os.SerialId equals ps.Id // Join với mã Serial vật lý (ProductSerial).
                    join ird in _context.ImportReceiptDetails // Join với chi tiết phiếu nhập hàng (ImportReceiptDetail).
                        on new { ps.ImportReceiptId, ps.VariantId } // Khớp khóa ngoại phức hợp gồm mã phiếu nhập.
                        equals new { ImportReceiptId = ird.ReceiptId, ird.VariantId } // Và mã phiên bản sản phẩm để tìm giá nhập chính xác của lô hàng đó.
                    select (decimal?)ird.ImportPrice // Lấy giá nhập gốc tại thời điểm nhập hàng của sản phẩm.
                ).SumAsync() ?? 0m; // Tính tổng cộng giá vốn của tất cả Serial đã bán ra, mặc định 0 nếu trống.

                var cancelledOrders = await _context.Orders.AsNoTracking() // Truy vấn bảng Orders đếm số đơn hàng bị hủy (Status = 4).
                    .Where(o => o.Status == 4 && o.OrderDate >= fromDate && o.OrderDate < toDate) // Lọc đơn hủy trong khoảng thời gian.
                    .CountAsync(); // Đếm số lượng đơn hàng bị hủy.

                var allOrdersInWindow = await _context.Orders.AsNoTracking() // Truy vấn đếm tất cả đơn hàng phát sinh (bất kể trạng thái).
                    .Where(o => o.OrderDate >= fromDate && o.OrderDate < toDate) // Lọc theo khoảng thời gian.
                    .CountAsync(); // Đếm số lượng.

                var newCustomers = await _context.Users.AsNoTracking() // Truy vấn bảng Users để đếm số khách hàng mới đăng ký tài khoản.
                    .Where(u => u.Type == 2 && !u.IsDeleted // Lọc vai trò khách hàng (Type = 2), chưa bị xóa.
                             && u.CreatedDate >= fromDate && u.CreatedDate < toDate) // Lọc theo ngày tạo.
                    .CountAsync(); // Đếm số lượng khách hàng mới.

                var paymentCounts = await _context.Orders.AsNoTracking() // Truy vấn đếm phân bổ các phương thức thanh toán.
                    .Where(o => o.Status == 3 && o.OrderDate >= fromDate && o.OrderDate < toDate) // Lọc đơn hàng thành công trong khoảng thời gian.
                    .GroupBy(o => o.PaymentMethod) // Nhóm theo phương thức thanh toán.
                    .Select(g => new { Method = g.Key, Count = g.Count() }) // Chọn ra tên phương thức và số lượng tương ứng.
                    .ToListAsync(); // Chuyển đổi thành danh sách.

                var cancelRate = allOrdersInWindow > 0 // Tính tỷ lệ hủy đơn hàng.
                    ? Math.Round((decimal)cancelledOrders / allOrdersInWindow * 100, 1) // Làm tròn tỷ lệ hủy lấy 1 chữ số thập phân.
                    : 0m; // Trả về 0 nếu không có đơn hàng nào phát sinh.

                var dto = new AnalyticsSummaryDto // Khởi tạo DTO báo cáo tài chính tổng quan.
                {
                    TotalRevenue = totalRevenue, // Tổng doanh thu.
                    GrossProfit = totalRevenue - totalCost, // Lợi nhuận gộp bằng doanh thu trừ đi tổng giá vốn truy vết được.
                    TotalOrders = totalOrders, // Tổng số đơn hàng thành công.
                    CancelledOrders = cancelledOrders, // Số đơn hàng hủy.
                    CancelRate = cancelRate, // Tỷ lệ hủy đơn.
                    AverageOrderValue = totalOrders > 0 ? Math.Round(totalRevenue / totalOrders, 0) : 0m, // Giá trị trung bình đơn hàng.
                    NewCustomers = newCustomers, // Số lượng khách hàng mới.
                    PayCod = paymentCounts.FirstOrDefault(x => x.Method == 0)?.Count ?? 0, // Số đơn thanh toán COD.
                    PayBanking = paymentCounts.FirstOrDefault(x => x.Method == 1)?.Count ?? 0, // Số đơn chuyển khoản ngân hàng.
                    PayVnPay = paymentCounts.FirstOrDefault(x => x.Method == 2)?.Count ?? 0 // Số đơn thanh toán qua VNPay.
                };
                return ApiResult<AnalyticsSummaryDto>.Ok(dto); // Trả về kết quả thành công kèm DTO báo cáo tổng quan.
            }
            catch (Exception ex) // Bắt lỗi ngoại lệ.
            {
                _logger.LogError(ex, "Lỗi khi lấy tổng quan thống kê."); // Ghi log lỗi hệ thống kèm mô tả chi tiết.
                return ApiResult<AnalyticsSummaryDto>.Fail("Không thể tải dữ liệu thống kê tổng quan."); // Trả về thông báo lỗi hệ thống.
            }
        }

        public async Task<ApiResult<RevenueTrendDto>> GetRevenueTrendAsync(DateTime from, DateTime to) // Định nghĩa hàm lấy xu hướng biến động doanh số hàng ngày.
        {
            var (valid, error) = ValidateRange(from, to); // Kiểm tra tính hợp lệ khoảng thời gian.
            if (!valid) return ApiResult<RevenueTrendDto>.Fail(error); // Nếu không hợp lệ, trả về lỗi.
            try // Khối bẫy lỗi.
            {
                var fromDate = from.Date; // Ngày bắt đầu lúc 00:00:00.
                var toDate = to.Date.AddDays(1); // Ngày kết thúc cộng thêm 1 ngày.

                var revenueByDay = await _context.Orders.AsNoTracking() // Truy vấn lấy doanh thu hàng ngày từ các đơn hàng thành công.
                    .Where(o => o.Status == 3 && o.OrderDate >= fromDate && o.OrderDate < toDate) // Lọc đơn hàng thành công trong khoảng thời gian.
                    .GroupBy(o => new { o.OrderDate.Year, o.OrderDate.Month, o.OrderDate.Day }) // Nhóm theo ngày, tháng, năm.
                    .Select(g => new { g.Key, Revenue = g.Sum(o => o.TotalAmount) }) // Tính tổng doanh số của mỗi ngày.
                    .ToDictionaryAsync( // Chuyển đổi kết quả sang dạng Dictionary để tra cứu nhanh.
                        x => new DateTime(x.Key.Year, x.Key.Month, x.Key.Day), // Khóa là đối tượng DateTime.
                        x => x.Revenue); // Giá trị là tổng doanh thu của ngày đó.

                var costByDay = await ( // Truy vết giá vốn hàng bán hàng ngày bằng cách join tương tự.
                    from o in _context.Orders // Từ bảng Orders.
                    where o.Status == 3 && o.OrderDate >= fromDate && o.OrderDate < toDate // Lọc đơn hàng thành công trong khoảng thời gian.
                    join od in _context.OrderDetails on o.Id equals od.OrderId // Join với chi tiết đơn.
                    join os in _context.OrderSerials on od.Id equals os.OrderDetailId // Join với Serial đã bán.
                    join ps in _context.ProductSerials on os.SerialId equals ps.Id // Join với thực thể Serial vật lý.
                    join ird in _context.ImportReceiptDetails // Join với chi tiết phiếu nhập kho.
                        on new { ps.ImportReceiptId, ps.VariantId } // Khớp khóa ngoại kép.
                        equals new { ImportReceiptId = ird.ReceiptId, ird.VariantId } // Tìm giá nhập lô hàng.
                    group ird.ImportPrice by new { o.OrderDate.Year, o.OrderDate.Month, o.OrderDate.Day } into g // Nhóm giá nhập theo ngày bán ra.
                    select new { g.Key, Cost = g.Sum() } // Tính tổng giá vốn của ngày đó.
                ).ToDictionaryAsync( // Chuyển đổi thành Dictionary để tra cứu.
                    x => new DateTime(x.Key.Year, x.Key.Month, x.Key.Day), // Khóa là đối tượng DateTime.
                    x => x.Cost); // Giá trị là tổng giá vốn của ngày.

                var points = new List<DailyRevenuePointDto>(); // Khởi tạo danh sách các điểm biểu đồ doanh thu ngày.
                for (var day = from.Date; day <= to.Date; day = day.AddDays(1)) // Vòng lặp duyệt qua từng ngày đơn lẻ trong khoảng thời gian lọc.
                {
                    var rev = revenueByDay.GetValueOrDefault(day, 0m); // Lấy doanh số ngày, mặc định 0 nếu không phát sinh đơn hàng.
                    var cost = costByDay.GetValueOrDefault(day, 0m); // Lấy giá vốn ngày, mặc định 0 nếu trống.
                    points.Add(new DailyRevenuePointDto // Thêm điểm dữ liệu biểu đồ.
                    {
                        Label = day.ToString("dd/MM"), // Nhãn hiển thị là chuỗi định dạng Ngày/Tháng.
                        Revenue = rev, // Doanh thu ngày.
                        Profit = rev - cost // Lợi nhuận ngày (Doanh thu - Giá vốn).
                    });
                }

                return ApiResult<RevenueTrendDto>.Ok(new RevenueTrendDto { Points = points }); // Trả về kết quả thành công kèm danh sách điểm biểu đồ.
            }
            catch (Exception ex) // Bắt lỗi hệ thống.
            {
                _logger.LogError(ex, "Lỗi khi lấy xu hướng doanh thu."); // Ghi log.
                return ApiResult<RevenueTrendDto>.Fail("Không thể tải dữ liệu xu hướng doanh thu."); // Trả về lỗi hệ thống.
            }
        }

        public async Task<ApiResult<OrderChannelDto>> GetOrderChannelsAsync(DateTime from, DateTime to) // Định nghĩa hàm phân tích sản lượng đơn hàng theo kênh phân phối.
        {
            var (valid, error) = ValidateRange(from, to); // Kiểm tra tính hợp lệ khoảng thời gian.
            if (!valid) return ApiResult<OrderChannelDto>.Fail(error); // Nếu không hợp lệ, trả về lỗi.
            try // Khối bẫy lỗi.
            {
                var fromDate = from.Date; // Ngày bắt đầu.
                var toDate = to.Date.AddDays(1); // Ngày kết thúc cộng 1.

                var result = await _context.Orders.AsNoTracking() // Truy vấn đếm số lượng đơn hàng thành công theo kênh bán.
                    .Where(o => o.Status == 3 && o.OrderDate >= fromDate && o.OrderDate < toDate) // Lọc đơn hàng thành công trong khoảng thời gian.
                    .GroupBy(o => o.OrderType) // Nhóm theo loại hình đơn hàng (OrderType: 0 = Online, 1 = POS).
                    .Select(g => new { Type = g.Key, Count = g.Count() }) // Chọn loại và đếm số lượng.
                    .ToListAsync(); // Chuyển đổi thành List bất đồng bộ.

                return ApiResult<OrderChannelDto>.Ok(new OrderChannelDto // Trả về kết quả phân bổ kênh bán hàng.
                {
                    OnlineCount = result.FirstOrDefault(x => x.Type == 0)?.Count ?? 0, // Số đơn Online (Type = 0), mặc định 0.
                    PosCount = result.FirstOrDefault(x => x.Type == 1)?.Count ?? 0 // Số đơn POS (Type = 1), mặc định 0.
                });
            }
            catch (Exception ex) // Bắt lỗi hệ thống.
            {
                _logger.LogError(ex, "Lỗi khi lấy thống kê kênh bán hàng."); // Ghi log.
                return ApiResult<OrderChannelDto>.Fail("Không thể tải dữ liệu kênh bán hàng."); // Trả về lỗi hệ thống.
            }
        }

        public async Task<ApiResult<List<TopProductDto>>> GetTopProductsAsync(DateTime from, DateTime to, int top) // Định nghĩa hàm lấy danh sách sản phẩm bán chạy nhất.
        {
            var (valid, error) = ValidateRange(from, to); // Kiểm tra tính hợp lệ của khoảng thời gian.
            if (!valid) return ApiResult<List<TopProductDto>>.Fail(error); // Nếu không hợp lệ, trả về lỗi.
            try // Khối bẫy lỗi.
            {
                var fromDate = from.Date; // Ngày bắt đầu.
                var toDate = to.Date.AddDays(1); // Ngày kết thúc cộng 1.

                var query = // Thực hiện viết truy vấn LINQ JOIN để lấy thông tin sản phẩm bán chạy.
                    from o in _context.Orders // Từ bảng Orders.
                    where o.Status == 3 && o.OrderDate >= fromDate && o.OrderDate < toDate // Lọc đơn hàng thành công trong khoảng thời gian.
                    join od in _context.OrderDetails on o.Id equals od.OrderId // Join với chi tiết đơn hàng (OrderDetail).
                    join v in _context.ProductVariants on od.VariantId equals v.Id // Join với phiên bản sản phẩm (ProductVariant).
                    join p in _context.Products on v.ProductId equals p.Id // Join với sản phẩm gốc (Product) để lấy tên sản phẩm.
                    group od by new { od.VariantId, v.VariantName, p.Name } into g // Nhóm theo Id variant, tên variant và tên sản phẩm.
                    orderby g.Sum(x => x.TotalLine) descending // Sắp xếp giảm dần theo tổng doanh thu mang lại của sản phẩm đó.
                    select new TopProductDto // Khởi tạo DTO hiển thị sản phẩm bán chạy.
                    {
                        ProductName = g.Key.Name, // Tên sản phẩm.
                        VariantName = g.Key.VariantName, // Tên phiên bản.
                        UnitsSold = g.Sum(x => x.Quantity), // Tổng số lượng sản phẩm bán ra thực tế.
                        Revenue = g.Sum(x => x.TotalLine) // Tổng doanh thu tích lũy.
                    };

                var data = await query.Take(top).ToListAsync(); // Lấy số lượng top sản phẩm mong muốn và chạy truy vấn.
                return ApiResult<List<TopProductDto>>.Ok(data); // Trả về danh sách top sản phẩm thành công.
            }
            catch (Exception ex) // Bắt lỗi hệ thống.
            {
                _logger.LogError(ex, "Lỗi khi lấy top sản phẩm bán chạy."); // Ghi log.
                return ApiResult<List<TopProductDto>>.Fail("Không thể tải dữ liệu sản phẩm bán chạy."); // Trả về lỗi hệ thống.
            }
        }

        public async Task<ApiResult<List<CategoryRevenueDto>>> GetCategoryRevenueAsync(DateTime from, DateTime to, int top) // Định nghĩa hàm phân tích cơ cấu doanh thu theo danh mục sản phẩm.
        {
            var (valid, error) = ValidateRange(from, to); // Kiểm tra tính hợp lệ khoảng thời gian.
            if (!valid) return ApiResult<List<CategoryRevenueDto>>.Fail(error); // Nếu không hợp lệ, trả về lỗi.
            try // Khối bẫy lỗi.
            {
                var fromDate = from.Date; // Ngày bắt đầu.
                var toDate = to.Date.AddDays(1); // Ngày kết thúc cộng 1.

                var query = // Thực hiện viết truy vấn LINQ JOIN lấy doanh thu theo danh mục.
                    from o in _context.Orders // Từ bảng Orders.
                    where o.Status == 3 && o.OrderDate >= fromDate && o.OrderDate < toDate // Lọc đơn hàng thành công.
                    join od in _context.OrderDetails on o.Id equals od.OrderId // Join với chi tiết đơn.
                    join v in _context.ProductVariants on od.VariantId equals v.Id // Join với phiên bản sản phẩm.
                    join p in _context.Products on v.ProductId equals p.Id // Join với sản phẩm gốc.
                    join c in _context.Categories on p.CategoryId equals c.Id // Join với danh mục (Category) để phân loại.
                    group od by new { c.Id, c.Name } into g // Nhóm theo mã danh mục và tên danh mục.
                    orderby g.Sum(x => x.TotalLine) descending // Sắp xếp giảm dần theo tổng doanh thu của danh mục đó.
                    select new CategoryRevenueDto // Khởi tạo DTO doanh thu danh mục.
                    {
                        CategoryName = g.Key.Name, // Tên danh mục.
                        OrderCount = g.Select(x => x.OrderId).Distinct().Count(), // Số lượng đơn hàng độc lập có chứa sản phẩm thuộc danh mục.
                        UnitsSold = g.Sum(x => x.Quantity), // Tổng số lượng sản phẩm của danh mục bán ra.
                        Revenue = g.Sum(x => x.TotalLine) // Tổng doanh thu tích lũy.
                    };

                var data = await query.Take(top).ToListAsync(); // Lấy số lượng giới hạn top danh mục.
                return ApiResult<List<CategoryRevenueDto>>.Ok(data); // Trả về danh sách.
            }
            catch (Exception ex) // Bắt lỗi ngoại lệ.
            {
                _logger.LogError(ex, "Lỗi khi lấy doanh thu theo danh mục."); // Ghi log.
                return ApiResult<List<CategoryRevenueDto>>.Fail("Không thể tải dữ liệu doanh thu theo danh mục."); // Trả về lỗi hệ thống.
            }
        }

        public async Task<ApiResult<InventorySummaryDto>> GetInventorySummaryAsync() // Định nghĩa hàm thống kê sức khỏe kho hàng và phân bổ serial.
        {
            try // Khối bẫy lỗi.
            {
                var statusCounts = await _context.ProductSerials.AsNoTracking() // Thống kê số lượng serial theo nhóm trạng thái.
                    .GroupBy(s => s.Status) // Nhóm theo cột Status của ProductSerial.
                    .Select(g => new { Status = g.Key, Count = g.Count() }) // Đếm số lượng serial của mỗi trạng thái.
                    .ToListAsync(); // Chuyển đổi thành danh sách.

                var totalSkus = await _context.ProductVariants.AsNoTracking() // Đếm tổng số lượng SKU biến thể sản phẩm đang tồn tại trong hệ thống.
                    .Where(v => !v.IsDeleted) // Lọc các biến thể chưa bị xóa mềm.
                    .CountAsync(); // Đếm tổng số lượng SKU.

                var lowStockSkus = await _context.ProductVariants.AsNoTracking() // Cảnh báo số lượng SKU có mức tồn kho báo động.
                    .Where(v => !v.IsDeleted && v.StockQuantity <= 3 && v.StockQuantity >= 0) // Lọc các biến thể chưa xóa có tồn kho nhỏ hơn hoặc bằng 3.
                    .CountAsync(); // Đếm số lượng SKU tồn kho thấp.

                var dto = new InventorySummaryDto // Khởi tạo DTO tổng hợp thông tin kho hàng.
                {
                    TotalSkus = totalSkus, // Tổng số lượng SKU.
                    TotalAvailable = statusCounts.FirstOrDefault(x => x.Status == 0)?.Count ?? 0, // Số serial sẵn sàng bán (Status = 0).
                    TotalReserved = statusCounts.FirstOrDefault(x => x.Status == 1)?.Count ?? 0, // Số serial đang tạm giữ đơn hàng (Status = 1).
                    TotalSold = statusCounts.FirstOrDefault(x => x.Status == 2)?.Count ?? 0, // Số serial đã bán hoàn thành (Status = 2).
                    TotalDefective = statusCounts.FirstOrDefault(x => x.Status == 3)?.Count ?? 0, // Số serial bị lỗi/chờ bảo hành (Status = 3).
                    TotalReturned = statusCounts.FirstOrDefault(x => x.Status == 4)?.Count ?? 0, // Số serial đã trả lại nhà cung cấp (Status = 4).
                    LowStockSkus = lowStockSkus // Số lượng SKU sắp hết hàng.
                };
                return ApiResult<InventorySummaryDto>.Ok(dto); // Trả về kết quả thành công kèm DTO.
            }
            catch (Exception ex) // Bắt lỗi hệ thống.
            {
                _logger.LogError(ex, "Lỗi khi lấy thống kê kho hàng."); // Ghi log.
                return ApiResult<InventorySummaryDto>.Fail("Không thể tải dữ liệu thống kê kho hàng."); // Trả về lỗi hệ thống.
            }
        }
    }
}

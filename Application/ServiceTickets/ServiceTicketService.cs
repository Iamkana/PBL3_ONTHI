using System; // Sử dụng các lớp và kiểu dữ liệu cơ bản của .NET.
using System.Collections.Generic; // Sử dụng các cấu trúc tập hợp dữ liệu như List, Dictionary.
using System.Linq; // Sử dụng các phương thức mở rộng để truy vấn tập hợp dữ liệu Linq.
using System.Threading.Tasks; // Sử dụng các tác vụ bất đồng bộ Task.
using Microsoft.EntityFrameworkCore; // Sử dụng các phương thức mở rộng truy vấn DB của EF Core.
using PBL3.Core.Entities; // Sử dụng các thực thể từ tầng Core (ServiceTicket, Warranty, ...).
using PBL3.Core.Interfaces; // Sử dụng các giao diện repository của tầng Core.
using PBL3.Infrastructure.Data; // Sử dụng DbContext để thao tác cơ sở dữ liệu trực tiếp.
using PBL3.Shared.DTOs.ServiceTickets; // Sử dụng các DTO phục vụ cho quản lý phiếu dịch vụ kỹ thuật.
using PBL3.Shared.Enums; // Sử dụng các enum định nghĩa trạng thái hệ thống.

namespace PBL3.Application.ServiceTickets // Khai báo namespace cho tầng Application của module phiếu dịch vụ.
{
    public class ServiceTicketService( // Khai báo lớp dịch vụ ServiceTicketService dùng Primary Constructor.
        IServiceTicketRepository ticketRepository, // Nhận repository quản lý phiếu dịch vụ kỹ thuật.
        IQuotationRepository quotationRepository, // Nhận repository quản lý bảng báo giá dịch vụ.
        IRmaShipmentRepository rmaRepository, // Nhận repository quản lý thông tin gửi bảo hành hãng.
        IServiceInvoiceRepository invoiceRepository, // Nhận repository quản lý hóa đơn thanh toán dịch vụ.
        ISerialRepairLogRepository logRepository, // Nhận repository quản lý nhật ký sửa chữa thiết bị.
        IWarrantyRepository warrantyRepository, // Nhận repository quản lý thông tin bảo hành của thiết bị.
        IProductSerialRepository serialRepository, // Nhận repository quản lý số Serial sản phẩm.
        IOrderRepository orderRepository, // Nhận repository quản lý đơn hàng.
        IUnitOfWork unitOfWork, // Nhận UnitOfWork điều phối transaction.
        HushStoreDbContext dbContext, // Nhận DbContext để thực hiện các lệnh truy vấn trực tiếp.
        IInventorySyncService inventorySyncService) : IServiceTicketService // Nhận dịch vụ đồng bộ kho và kế thừa IServiceTicketService.
    {
        private readonly IServiceTicketRepository _ticketRepository = // Khai báo trường lưu trữ repository phiếu dịch vụ.
            ticketRepository ?? throw new ArgumentNullException(nameof(ticketRepository)); // Gán giá trị và kiểm tra null.
        private readonly IQuotationRepository _quotationRepository = // Khai báo trường lưu trữ repository báo giá.
            quotationRepository ?? throw new ArgumentNullException(nameof(quotationRepository)); // Gán giá trị và kiểm tra null.
        private readonly IRmaShipmentRepository _rmaRepository = // Khai báo trường lưu trữ repository RMA.
            rmaRepository ?? throw new ArgumentNullException(nameof(rmaRepository)); // Gán giá trị và kiểm tra null.
        private readonly IServiceInvoiceRepository _invoiceRepository = // Khai báo trường lưu trữ repository hóa đơn dịch vụ.
            invoiceRepository ?? throw new ArgumentNullException(nameof(invoiceRepository)); // Gán giá trị và kiểm tra null.
        private readonly ISerialRepairLogRepository _logRepository = // Khai báo trường lưu trữ repository nhật ký sửa chữa.
            logRepository ?? throw new ArgumentNullException(nameof(logRepository)); // Gán giá trị và kiểm tra null.
        private readonly IWarrantyRepository _warrantyRepository = // Khai báo trường lưu trữ repository bảo hành.
            warrantyRepository ?? throw new ArgumentNullException(nameof(warrantyRepository)); // Gán giá trị và kiểm tra null.
        private readonly IProductSerialRepository _serialRepository = // Khai báo trường lưu trữ repository serial.
            serialRepository ?? throw new ArgumentNullException(nameof(serialRepository)); // Gán giá trị và kiểm tra null.
        private readonly IOrderRepository _orderRepository = // Khai báo trường lưu trữ repository đơn hàng.
            orderRepository ?? throw new ArgumentNullException(nameof(orderRepository)); // Gán giá trị và kiểm tra null.
        private readonly IUnitOfWork _unitOfWork = // Khai báo trường lưu trữ UnitOfWork.
            unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork)); // Gán giá trị và kiểm tra null.
        private readonly HushStoreDbContext _dbContext = // Khai báo trường lưu trữ DbContext.
            dbContext ?? throw new ArgumentNullException(nameof(dbContext)); // Gán giá trị và kiểm tra null.
        private readonly IInventorySyncService _inventorySyncService = // Khai báo trường lưu trữ dịch vụ đồng bộ kho.
            inventorySyncService ?? throw new ArgumentNullException(nameof(inventorySyncService)); // Gán giá trị và kiểm tra null.

        public async Task<ServiceTicketIntakeEvaluationDto> GetWarrantyEvaluationAsync(string serialNumber) // Định nghĩa phương thức thẩm định bảo hành thiết bị trước khi tiếp nhận sửa chữa.
        {
            var serial = await _serialRepository.GetBySerialNumberAsync(serialNumber); // Tra cứu thông tin Serial Number từ repository.

            if (serial == null) // Nếu số Serial này không tồn tại trong hệ thống.
                return new ServiceTicketIntakeEvaluationDto // Trả về DTO chứa thông báo chặn quy trình.
                {
                    BlockingReason = "Không tìm thấy Serial trong hệ thống." // Báo lý do không tìm thấy thiết bị.
                }; // Kết thúc trả về.

            if (serial.Status != (byte)SerialStatus.Sold) // Kiểm tra xem trạng thái Serial có phải là đã bán (Sold) hay không.
                return new ServiceTicketIntakeEvaluationDto // Trả về DTO chứa thông báo chặn quy trình.
                {
                    BlockingReason = "Sản phẩm này chưa được bán hoặc không ở trạng thái hợp lệ." // Báo lý do sản phẩm chưa bán ra.
                }; // Kết thúc trả về.

            if (!serial.OrderId.HasValue) // Kiểm tra xem Serial có liên kết với đơn hàng bán ra nào không.
                return new ServiceTicketIntakeEvaluationDto // Trả về DTO chứa lý do chặn quy trình.
                {
                    BlockingReason = "Không thể tìm thấy thông tin đơn hàng gốc của sản phẩm này." // Báo lý do thiếu đơn hàng gốc.
                }; // Kết thúc trả về.

            var order = await _orderRepository.GetByIdAsync(serial.OrderId.Value); // Truy vấn thông tin đơn hàng gốc để lấy thông tin khách mua.
            if (order == null) // Nếu đơn hàng không tồn tại trong DB.
                return new ServiceTicketIntakeEvaluationDto // Trả về DTO chứa lý do chặn quy trình.
                {
                    BlockingReason = "Đơn hàng gốc không tồn tại." // Báo lỗi đơn hàng gốc bị khuyết.
                }; // Kết thúc trả về.

            if (await _ticketRepository.HasOpenTicketForSerialAsync(serial.Id)) // Kiểm tra xem thiết bị này đã có phiếu sửa chữa nào đang mở (chưa hoàn tất/chưa hủy) hay chưa.
                return new ServiceTicketIntakeEvaluationDto // Trả về DTO chứa lý do chặn quy trình.
                {
                    BlockingReason = "Sản phẩm này đã có phiếu sửa chữa chưa đóng." // Báo lỗi chặn tiếp nhận trùng lặp.
                }; // Kết thúc trả về.

            var variant = serial.Variant; // Lấy thông tin biến thể sản phẩm liên kết với Serial này.
            var warranty = await WarrantyEvaluator.EvaluateAsync(serial, variant, _warrantyRepository); // Thực hiện thẩm định thời hạn bảo hành thực tế của thiết bị.

            var allowedBranches = new List<string>(); // Khởi tạo danh sách các nhánh xử lý được phép chọn.
            if (warranty.IsInWarranty) // Nếu thiết bị còn trong thời hạn bảo hành hợp lệ.
            {
                allowedBranches.Add("InternalRepair"); // Cho phép sửa chữa bảo hành nội bộ.
                allowedBranches.Add("Rma"); // Cho phép gửi đi bảo hành tại hãng (RMA).
                allowedBranches.Add("Swap"); // Cho phép đổi sản phẩm mới 1-đổi-1.
            } // Kết thúc nhánh còn bảo hành.
            else // Nếu thiết bị đã hết hạn bảo hành.
            {
                allowedBranches.Add("PaidRepair"); // Chỉ cho phép thực hiện sửa chữa có tính phí dịch vụ.
            } // Kết thúc nhánh hết bảo hành.

            return new ServiceTicketIntakeEvaluationDto // Trả về kết quả thẩm định hoàn chỉnh cho giao diện tiếp nhận.
            {
                SerialNumber = serial.SerialNumber, // Gán số Serial.
                ProductName = variant.Product.Name, // Gán tên sản phẩm cha.
                VariantName = variant.VariantName, // Gán tên biến thể cụ thể.
                IsInWarranty = warranty.IsInWarranty, // Gán trạng thái bảo hành.
                WarrantyExpiresOn = warranty.ExpiresOn, // Gán ngày hết hạn bảo hành.
                WarrantySource = GetWarrantySourceLabel(warranty.Source), // Gán nguồn gốc cách xác định bảo hành.
                CustomerId = order.UserId, // Gán Id khách hàng.
                CustomerName = order.User?.Profile?.FullName ?? order.ShipName, // Gán tên khách hàng (ưu tiên thông tin tài khoản).
                CustomerEmail = order.User?.Email, // Gán email liên hệ của khách hàng.
                BlockingReason = null, // Gán lý do chặn bằng null (tiếp nhận hợp lệ).
                AllowedBranches = allowedBranches // Gán danh sách các luồng nghiệp vụ được phép đi tiếp.
            }; // Kết thúc khởi tạo DTO.
        }

        public async Task<ServiceTicketDetailDto?> CreateTicketFromSerialScanAsync(ServiceTicketIntakeRequestDto request, Guid userId) // Định nghĩa phương thức khởi tạo phiếu dịch vụ khi tiếp nhận thiết bị.
        {
            var serial = await _serialRepository.GetBySerialNumberAsync(request.SerialNumber); // Tra cứu số Serial cần tiếp nhận.
            if (serial == null) // Nếu không tìm thấy số Serial.
                throw new InvalidOperationException("Không tìm thấy Serial trong hệ thống."); // Bắn ra ngoại lệ.

            if (serial.Status != (byte)SerialStatus.Sold) // Kiểm tra trạng thái thiết bị.
                throw new InvalidOperationException("Sản phẩm không ở trạng thái Sold."); // Báo lỗi nếu sản phẩm chưa được bán ra.

            if (await _ticketRepository.HasOpenTicketForSerialAsync(serial.Id)) // Kiểm tra xem thiết bị có đang nằm trong phiếu sửa chữa chưa đóng khác hay không.
                throw new InvalidOperationException("Sản phẩm này đã có phiếu sửa chữa chưa đóng."); // Bắn ra ngoại lệ để tránh tiếp nhận trùng.

            var variant = serial.Variant; // Lấy biến thể sản phẩm.
            var order = await _orderRepository.GetByIdAsync(serial.OrderId!.Value); // Lấy thông tin đơn hàng gốc của thiết bị.
            if (order == null) // Nếu không tìm thấy đơn hàng gốc.
                throw new InvalidOperationException("Không tìm thấy đơn hàng gốc."); // Bắn ra ngoại lệ.

            var warranty = await WarrantyEvaluator.EvaluateAsync(serial, variant, _warrantyRepository); // Thẩm định thời gian bảo hành ngay lúc nhận máy để ghi nhận snapshot bảo hành đầu vào.

            var now = DateTime.UtcNow; // Lấy thời gian UTC hiện tại.
            var datePrefix = "ST-" + now.ToString("yyyyMMdd"); // Tạo tiền tố mã phiếu định dạng ST-yyyyMMdd.
            var lastCode = await _ticketRepository.GetLastTicketCodeByDateAsync(datePrefix); // Tra cứu mã phiếu lớn nhất đã được tạo trong ngày hôm nay.
            int nextIndex = 1; // Khởi tạo chỉ số số thứ tự phiếu tiếp theo là 1.
            if (!string.IsNullOrEmpty(lastCode)) // Nếu trong ngày đã có phiếu dịch vụ được khởi tạo.
            {
                var suffix = lastCode.Substring(lastCode.LastIndexOf('-') + 1); // Cắt lấy chuỗi số thứ tự ở cuối.
                if (int.TryParse(suffix, out int lastIdx)) // Chuyển chuỗi số sang số nguyên thành công.
                    nextIndex = lastIdx + 1; // Số thứ tự tiếp theo bằng số cũ cộng 1.
            } // Kết thúc khối tính toán số thứ tự.
            var ticketCode = $"{datePrefix}-{nextIndex:D3}"; // Tạo mã phiếu hoàn chỉnh (ví dụ: ST-20260531-001).

            await _unitOfWork.BeginTransactionAsync(); // Khởi động Transaction để thực hiện ghi nhận phiếu và lịch sử đồng thời.
            try // Bắt đầu khối an toàn.
            {
                var ticket = new ServiceTicket // Khởi tạo thực thể phiếu dịch vụ sửa chữa bảo hành mới.
                {
                    TicketCode = ticketCode, // Gán mã phiếu dịch vụ.
                    SerialId = serial.Id, // Gán Id của số Serial.
                    OriginalOrderId = order.Id, // Gán Id đơn hàng gốc.
                    CustomerId = order.UserId, // Gán Id khách hàng.
                    IntakeDate = now, // Gán thời gian tiếp nhận máy.
                    IntakeEmployeeId = userId, // Gán Id nhân viên tiếp nhận máy.
                    HasScratches = request.HasScratches, // Ghi nhận snapshot thiết bị có vết xước.
                    HasDents = request.HasDents, // Ghi nhận snapshot thiết bị có vết móp.
                    HasBurnMarks = request.HasBurnMarks, // Ghi nhận snapshot thiết bị có vết cháy nổ.
                    HasMissingAccessories = request.HasMissingAccessories, // Ghi nhận snapshot thiết bị bị thiếu phụ kiện kèm theo.
                    CosmeticNotes = request.CosmeticNotes, // Ghi chép chi tiết ghi chú ngoại quan lúc tiếp nhận.
                    CustomerReportedIssue = request.CustomerReportedIssue, // Ghi chép lỗi khách hàng báo lại.
                    WalkInCustomerName = request.WalkInCustomerName, // Tên khách hàng vãng lai (nếu không có tài khoản).
                    WalkInCustomerPhone = request.WalkInCustomerPhone, // Số điện thoại liên hệ của khách vãng lai.
                    WasInWarrantyAtIntake = warranty.IsInWarranty, // Lưu cứng thông tin còn bảo hành lúc nhận máy làm cơ sở thanh toán.
                    WarrantyEndDateAtIntake = warranty.ExpiresOn, // Lưu ngày hết hạn bảo hành tại thời điểm tiếp nhận.
                    WarrantyEvalSource = warranty.Source, // Lưu nguồn xác định bảo hành.
                    Status = (byte)0, // Mặc định gán trạng thái phiếu là Đã tiếp nhận (0).
                    ResolutionType = (byte)0, // Mặc định loại hướng giải quyết là Chờ xác định (0).
                    CreatedDate = now, // Ngày tạo phiếu.
                    ModifiedDate = now // Ngày chỉnh sửa phiếu gần nhất.
                }; // Kết thúc khởi tạo phiếu.

                await _ticketRepository.AddAsync(ticket); // Lưu thực thể phiếu dịch vụ.
                await _unitOfWork.SaveChangesAsync(); // Lưu thay đổi tạm thời xuống DB để sinh Id phiếu.

                await _ticketRepository.AddStatusHistoryAsync(new ServiceTicketStatusHistory // Ghi nhận lịch sử chuyển trạng thái đầu tiên.
                {
                    TicketId = ticket.Id, // Id phiếu dịch vụ tương ứng.
                    FromStatus = (byte)0, // Trạng thái bắt đầu là Đã tiếp nhận.
                    ToStatus = (byte)0, // Trạng thái đích là Đã tiếp nhận.
                    ChangedByEmployeeId = userId, // Người thực hiện.
                    ChangedAt = now, // Thời gian chuyển trạng thái.
                    Note = "Tiếp nhận sản phẩm" // Ghi chú.
                }); // Kết thúc lưu lịch sử.

                await _unitOfWork.SaveChangesAsync(); // Lưu thay đổi lịch sử.
                await _unitOfWork.CommitAsync(); // Xác nhận Transaction thành công, ghi dữ liệu chính thức vào DB.

                return await GetTicketByIdAsync(ticket.Id, userId, false); // Tải lại thông tin phiếu chi tiết kèm quan hệ và trả về DTO.
            } // Kết thúc khối try.
            catch // Nếu xảy ra bất kỳ lỗi gì trong quá trình thực thi.
            {
                await _unitOfWork.RollbackAsync(); // Khôi phục lại toàn bộ trạng thái DB trước khi mở Transaction để bảo toàn dữ liệu.
                throw; // Bắn tiếp ngoại lệ lên tầng trên xử lý.
            } // Kết thúc khối catch.
        }

        public async Task<bool> AssignTechnicianAsync(int ticketId, Guid employeeId, Guid userId) // Định nghĩa phương thức phân công kỹ thuật viên xử lý phiếu.
        {
            var ticket = await _ticketRepository.GetByIdWithTrackingAsync(ticketId); // Lấy thực thể phiếu cần cập nhật ở chế độ tracking.
            if (ticket == null) // Nếu phiếu không tồn tại.
                throw new InvalidOperationException("Phiếu không tồn tại."); // Bắn ngoại lệ.

            ValidateTransition(ticket.Status, (byte)1); // Xác thực xem trạng thái hiện tại của phiếu có được phép chuyển sang Đang chẩn đoán (1) hay không.

            ticket.AssignedEmployeeId = employeeId; // Gán Id kỹ thuật viên được giao phụ trách sửa chữa.
            ticket.Status = (byte)1; // Chuyển trạng thái phiếu sang Đang chẩn đoán (1).
            ticket.ModifiedDate = DateTime.UtcNow; // Ghi nhận thời gian sửa đổi.

            await _ticketRepository.AddStatusHistoryAsync(new ServiceTicketStatusHistory // Thêm lịch sử chuyển đổi trạng thái.
            {
                TicketId = ticketId, // Gán Id phiếu dịch vụ.
                FromStatus = (byte)0, // Từ Đã tiếp nhận.
                ToStatus = (byte)1, // Sang Đang chẩn đoán.
                ChangedByEmployeeId = userId, // Người thực hiện thao tác.
                ChangedAt = DateTime.UtcNow, // Thời điểm ghi nhận.
                Note = "Giao phó cho kỹ thuật viên" // Nội dung ghi chú.
            }); // Kết thúc ghi nhận lịch sử.

            await _ticketRepository.SaveChangesAsync(); // Lưu các thay đổi xuống cơ sở dữ liệu.
            return true; // Trả về kết quả phân công thành công.
        }

        public async Task<bool> RecordDiagnosisAsync(int ticketId, ServiceTicketDiagnosisDto request, Guid userId, bool isAdmin = false) // Định nghĩa phương thức ghi nhận chẩn đoán lỗi của thiết bị.
        {
            var ticket = await _ticketRepository.GetByIdWithTrackingAsync(ticketId); // Lấy thực thể phiếu dịch vụ cần ghi nhận chẩn đoán.
            if (ticket == null) // Nếu phiếu sửa chữa không tồn tại.
                throw new InvalidOperationException("Phiếu không tồn tại."); // Báo lỗi.

            CheckAssignment(ticket, userId, isAdmin); // Xác thực xem người dùng hiện tại có phải kỹ thuật viên phụ trách phiếu hoặc là admin hay không.

            if (ticket.Status != (byte)1) // Ràng buộc: Phiếu phải ở trạng thái Đang chẩn đoán (1).
                throw new InvalidOperationException("Phiếu phải ở trạng thái Đang chẩn đoán."); // Bắn ra ngoại lệ nếu sai trạng thái.

            ticket.DiagnosisFindings = request.DiagnosisFindings; // Ghi chép phát hiện chẩn đoán nguyên nhân lỗi thiết bị.
            ticket.DiagnosedAt = DateTime.UtcNow; // Ghi thời gian chẩn đoán lỗi.
            ticket.DiagnosedByEmployeeId = userId; // Ghi nhận mã kỹ thuật viên thực hiện chẩn đoán.
            ticket.ModifiedDate = DateTime.UtcNow; // Cập nhật mốc thời gian sửa đổi gần nhất.

            await _ticketRepository.SaveChangesAsync(); // Lưu thay đổi vào DB.
            return true; // Trả về thành công.
        }

        public async Task<bool> ChooseBranchAsync(int ticketId, ServiceTicketBranchDto request, Guid userId, bool isAdmin = false) // Định nghĩa phương thức chọn luồng xử lý thiết bị.
        {
            var ticket = await _ticketRepository.GetByIdWithTrackingAsync(ticketId); // Lấy phiếu dịch vụ từ repository ở chế độ tracking.
            if (ticket == null) // Nếu phiếu không tồn tại.
                throw new InvalidOperationException("Phiếu không tồn tại."); // Bắn ra lỗi.

            CheckAssignment(ticket, userId, isAdmin); // Kiểm tra quyền hạn của người thực hiện thao tác.

            if (ticket.Status != (byte)1) // Đảm bảo phiếu đang ở giai đoạn chẩn đoán.
                throw new InvalidOperationException("Phiếu phải ở trạng thái Đang chẩn đoán."); // Bắn ngoại lệ nếu không đúng trạng thái.

            var resolutionType = request.ResolutionType; // Lấy loại hướng giải quyết được lựa chọn từ yêu cầu.

            if (!ticket.WasInWarrantyAtIntake && resolutionType != (byte)4) // Nếu thiết bị hết bảo hành lúc nhận máy, bắt buộc chỉ được chọn sửa tính phí (4 - PaidRepair).
                throw new InvalidOperationException("Sản phẩm đã hết bảo hành, chỉ có thể chọn sửa tính phí."); // Bắn ngoại lệ ngăn chặn sửa bảo hành miễn phí.

            if (ticket.WasInWarrantyAtIntake && resolutionType == (byte)4) // Nếu thiết bị còn hạn bảo hành lúc nhận máy, không được chọn nhánh sửa tính phí (4).
                throw new InvalidOperationException("Sản phẩm còn bảo hành, không thể chọn sửa tính phí."); // Bắn ngoại lệ ngăn chặn tính tiền sai quy định bảo hành.

            ticket.ResolutionType = resolutionType; // Cập nhật loại hướng giải quyết chính thức cho phiếu.
            ticket.ModifiedDate = DateTime.UtcNow; // Ghi nhận thời gian cập nhật.

            await _ticketRepository.SaveChangesAsync(); // Lưu thay đổi vào cơ sở dữ liệu.
            return true; // Trả về thành công.
        }

        public async Task<QuotationDetailDto?> CreateQuotationAsync(int ticketId, QuotationCreateDto request, Guid userId, bool isAdmin = false) // Định nghĩa phương thức tạo báo giá sửa chữa dịch vụ.
        {
            var ticket = await _ticketRepository.GetByIdWithTrackingAsync(ticketId); // Lấy thực thể phiếu sửa chữa cần lập báo giá.
            if (ticket == null) // Nếu không tìm thấy phiếu.
                throw new InvalidOperationException("Phiếu không tồn tại."); // Báo lỗi.

            CheckAssignment(ticket, userId, isAdmin); // Kiểm tra quyền phân công kỹ thuật viên xử lý phiếu này.

            if (ticket.Status != (byte)1) // Kiểm tra xem phiếu có đang ở trạng thái chẩn đoán lỗi hay không.
                throw new InvalidOperationException("Phiếu phải ở trạng thái Đang chẩn đoán."); // Báo lỗi quy trình.

            ValidateTransition(ticket.Status, (byte)2); // Xác thực xem trạng thái phiếu có thể chuyển đổi sang Chờ khách duyệt báo giá (2) hay không.

            if (ticket.ResolutionType != (byte)4) // Đảm bảo chỉ phiếu đi theo nhánh sửa tính phí mới cần lập báo giá.
                throw new InvalidOperationException("Chỉ phiếu sửa tính phí mới có báo giá."); // Báo lỗi nếu lập báo giá cho thiết bị bảo hành miễn phí.

            await _unitOfWork.BeginTransactionAsync(); // Khởi tạo Transaction bảo vệ quy trình lưu trữ đồng thời.
            try // Bắt đầu khối an toàn.
            {
                var oldQuotations = await _quotationRepository.GetByTicketIdAsync(ticketId); // Lấy danh sách toàn bộ báo giá cũ của phiếu dịch vụ này.
                foreach (var q in oldQuotations.Where(q => q.Status == (byte)0)) // Lọc ra các bản báo giá cũ đang ở trạng thái Chờ duyệt (0).
                {
                    q.Status = (byte)3; // Cập nhật trạng thái của chúng thành Bị thay thế (3 - Superceded) để vô hiệu hóa hiệu lực.
                } // Kết thúc vòng lặp hủy báo giá cũ.

                var partsTotal = request.Items.Sum(i => i.Quantity * i.UnitPrice); // Tính tổng chi phí linh kiện thay thế dựa trên danh sách gửi lên.
                var grandTotal = request.LaborCost + partsTotal; // Tính tổng tiền báo giá = Tiền công sửa chữa + Tổng tiền linh kiện.

                var quotation = new Quotation // Khởi tạo thực thể báo giá mới.
                {
                    TicketId = ticketId, // Mã phiếu dịch vụ sửa chữa liên quan.
                    IssuedDate = DateTime.UtcNow, // Ngày lập báo giá.
                    IssuedByEmployeeId = userId, // Nhân viên thực hiện báo giá.
                    LaborCost = request.LaborCost, // Chi phí tiền công sửa chữa.
                    PartsTotal = partsTotal, // Tổng tiền linh kiện.
                    GrandTotal = grandTotal, // Tổng tiền thanh toán dự tính.
                    Status = (byte)0, // Đặt trạng thái ban đầu là Chờ duyệt (0).
                }; // Kết thúc khởi tạo báo giá.

                await _quotationRepository.AddAsync(quotation); // Thêm báo giá mới vào DB.
                await _unitOfWork.SaveChangesAsync(); // Lưu thay đổi tạm thời để lấy Id báo giá.

                foreach (var item in request.Items) // Duyệt qua từng chi tiết hạng mục dịch vụ/linh kiện trong yêu cầu.
                {
                    await _dbContext.QuotationItems.AddAsync(new QuotationItem // Thêm chi tiết khoản mục báo giá vào cơ sở dữ liệu.
                    {
                        QuotationId = quotation.Id, // Liên kết với báo giá cha vừa tạo.
                        VariantId = item.VariantId, // Id biến thể linh kiện thay thế trong kho (nếu có).
                        Description = item.Description, // Mô tả dịch vụ/linh kiện.
                        Quantity = item.Quantity, // Số lượng thay thế.
                        UnitPrice = item.UnitPrice, // Đơn giá linh kiện/dịch vụ.
                        LineTotal = item.Quantity * item.UnitPrice // Tổng tiền của dòng này.
                    }); // Kết thúc thêm dòng chi tiết.
                } // Kết thúc vòng lặp.

                ticket.Status = (byte)2; // Chuyển trạng thái phiếu dịch vụ sang Chờ duyệt báo giá (2).
                await _ticketRepository.AddStatusHistoryAsync(new ServiceTicketStatusHistory // Ghi nhận lịch sử chuyển trạng thái.
                {
                    TicketId = ticketId, // Mã phiếu sửa chữa.
                    FromStatus = (byte)1, // Từ Đang chẩn đoán.
                    ToStatus = (byte)2, // Sang Đã gửi báo giá.
                    ChangedByEmployeeId = userId, // Nhân viên thực hiện gửi.
                    ChangedAt = DateTime.UtcNow, // Mốc thời gian.
                    Note = "Gửi báo giá cho khách" // Nội dung lưu vết.
                }); // Kết thúc ghi nhận.

                await _unitOfWork.SaveChangesAsync(); // Lưu toàn bộ thay đổi.
                await _unitOfWork.CommitAsync(); // Xác nhận hoàn tất Transaction.

                var reloadedQuotation = await _quotationRepository.GetByIdWithItemsAsync(quotation.Id); // Tải lại thông tin báo giá đầy đủ các hạng mục.
                return MapQuotationToDto(reloadedQuotation); // Ánh xạ sang DTO chi tiết và trả về.
            } // Kết thúc khối try.
            catch // Nếu xảy ra sự cố.
            {
                await _unitOfWork.RollbackAsync(); // Khôi phục lại trạng thái cơ sở dữ liệu để tránh dữ liệu rác.
                throw; // Bắn tiếp ngoại lệ lên.
            } // Kết thúc khối catch.
        }

        public async Task<bool> AcceptQuotationAsync(int ticketId, int quotationId, QuotationAcceptDto request, Guid userId, bool isAdmin = false) // Định nghĩa phương thức đồng ý báo giá sửa chữa.
        {
            var ticket = await _ticketRepository.GetByIdWithTrackingAsync(ticketId); // Lấy thông tin phiếu dịch vụ từ DB.
            if (ticket == null) // Nếu phiếu không tồn tại.
                throw new InvalidOperationException("Phiếu không tồn tại."); // Bắn ngoại lệ.

            CheckAssignmentOrCustomer(ticket, userId, isAdmin); // Kiểm tra quyền truy cập (cho phép khách hàng hoặc kỹ thuật viên phụ trách duyệt).

            if (ticket.Status != (byte)2) // Yêu cầu phiếu phải ở trạng thái Chờ duyệt báo giá (2).
                throw new InvalidOperationException("Phiếu phải ở trạng thái Chờ duyệt báo giá."); // Báo lỗi.

            var quotation = await _quotationRepository.GetByIdWithTrackingAsync(quotationId); // Lấy thông tin thực thể báo giá cần duyệt.
            if (quotation == null || quotation.TicketId != ticketId) // Nếu không tìm thấy báo giá hoặc báo giá không thuộc về phiếu dịch vụ hiện tại.
                throw new InvalidOperationException("Báo giá không tồn tại."); // Báo lỗi.

            if (quotation.Status != (byte)0) // Chỉ cho phép phê duyệt đối với báo giá đang ở trạng thái Chờ duyệt (0).
                throw new InvalidOperationException("Chỉ được duyệt báo giá chưa được xử lý."); // Báo lỗi nếu duyệt lại báo giá đã xử lý.

            var nextStatus = request.NextStatus; // Lấy trạng thái tiếp theo được lựa chọn từ DTO (ví dụ: Chờ phụ tùng hoặc Đang sửa).
            if (nextStatus != (byte)4 && nextStatus != (byte)5) // Đảm bảo trạng thái đích chỉ được là Chờ phụ tùng (4) hoặc Đang sửa (5).
                throw new InvalidOperationException("Trạng thái tiếp theo không hợp lệ."); // Báo lỗi nếu chọn sai trạng thái đích.

            ValidateTransition(ticket.Status, nextStatus); // Xác thực xem có thể chuyển từ trạng thái Chờ duyệt báo giá (2) sang trạng thái đích được chọn không.

            await _unitOfWork.BeginTransactionAsync(); // Khởi chạy Transaction.
            try // Bắt đầu khối an toàn.
            {
                quotation.Status = (byte)1; // Chuyển trạng thái báo giá sang Đã duyệt (1 - Approved).
                quotation.CustomerDecidedAt = DateTime.UtcNow; // Ghi nhận thời điểm khách quyết định.

                ticket.Status = nextStatus; // Chuyển trạng thái phiếu dịch vụ sang trạng thái đích mới chọn.
                ticket.ModifiedDate = DateTime.UtcNow; // Cập nhật mốc thời gian sửa đổi gần nhất.

                await _ticketRepository.AddStatusHistoryAsync(new ServiceTicketStatusHistory // Ghi nhận nhật ký lịch sử chuyển trạng thái.
                {
                    TicketId = ticketId, // Mã phiếu sửa chữa.
                    FromStatus = (byte)2, // Từ trạng thái Đã gửi báo giá.
                    ToStatus = nextStatus, // Sang trạng thái đích mới (Chờ phụ tùng hoặc Đang sửa).
                    ChangedByEmployeeId = userId, // Người thực hiện phê duyệt.
                    ChangedAt = DateTime.UtcNow, // Mốc thời gian.
                    Note = "Khách chấp nhận báo giá" // Nội dung ghi nhận lịch sử.
                }); // Kết thúc thêm lịch sử.

                await _unitOfWork.SaveChangesAsync(); // Lưu thay đổi.
                await _unitOfWork.CommitAsync(); // Xác nhận Transaction.
                return true; // Trả về kết quả phê duyệt thành công.
            } // Kết thúc khối try.
            catch // Nếu lỗi.
            {
                await _unitOfWork.RollbackAsync(); // Hủy bỏ thay đổi để tránh lệch trạng thái giữa báo giá và phiếu dịch vụ.
                throw; // Bắn tiếp lỗi lên.
            } // Kết thúc khối catch.
        }

        public async Task<bool> RejectQuotationAsync(int ticketId, int quotationId, QuotationRejectDto request, Guid userId, bool isAdmin = false) // Định nghĩa phương thức từ chối báo giá dịch vụ sửa chữa.
        {
            var ticket = await _ticketRepository.GetByIdWithTrackingAsync(ticketId); // Lấy thực thể phiếu sửa chữa cần cập nhật.
            if (ticket == null) // Nếu không tìm thấy phiếu.
                throw new InvalidOperationException("Phiếu không tồn tại."); // Báo lỗi.

            CheckAssignmentOrCustomer(ticket, userId, isAdmin); // Kiểm tra quyền truy cập của người dùng.

            if (ticket.Status != (byte)2) // Ràng buộc: Phiếu phải ở trạng thái Chờ duyệt báo giá (2).
                throw new InvalidOperationException("Phiếu phải ở trạng thái Chờ duyệt báo giá."); // Báo lỗi trạng thái.

            var quotation = await _quotationRepository.GetByIdWithTrackingAsync(quotationId); // Lấy thực thể báo giá cần xử lý từ chối.
            if (quotation == null || quotation.TicketId != ticketId) // Đảm bảo báo giá có tồn tại và thuộc phiếu dịch vụ này.
                throw new InvalidOperationException("Báo giá không tồn tại."); // Báo lỗi.

            ValidateTransition(ticket.Status, (byte)3); // Xác thực xem phiếu có được phép chuyển sang trạng thái Khách từ chối báo giá (3) hay không.

            await _unitOfWork.BeginTransactionAsync(); // Khởi chạy Transaction bảo đảm đồng bộ trạng thái.
            try // Bắt đầu khối an toàn.
            {
                quotation.Status = (byte)2; // Chuyển trạng thái báo giá sang Đã từ chối (2 - Rejected).
                quotation.CustomerDecisionNote = request.Reason; // Lưu lý do khách hàng từ chối báo giá sửa chữa.
                quotation.CustomerDecidedAt = DateTime.UtcNow; // Ghi thời gian quyết định từ chối.

                ticket.Status = (byte)3; // Chuyển trạng thái phiếu dịch vụ sang Khách từ chối báo giá (3).
                ticket.ModifiedDate = DateTime.UtcNow; // Cập nhật thời điểm sửa đổi.

                await _ticketRepository.AddStatusHistoryAsync(new ServiceTicketStatusHistory // Lưu lịch sử chuyển đổi trạng thái của phiếu.
                {
                    TicketId = ticketId, // Mã phiếu sửa chữa.
                    FromStatus = (byte)2, // Từ Đã gửi báo giá.
                    ToStatus = (byte)3, // Sang Khách từ chối báo giá.
                    ChangedByEmployeeId = userId, // Người thao tác.
                    ChangedAt = DateTime.UtcNow, // Thời điểm ghi nhận.
                    Note = "Khách từ chối báo giá" // Ghi chú lưu vết.
                }); // Kết thúc lưu lịch sử.

                await _unitOfWork.SaveChangesAsync(); // Lưu các thay đổi xuống DB.
                await _unitOfWork.CommitAsync(); // Xác nhận hoàn tất Transaction.
                return true; // Trả về kết quả từ chối thành công.
            } // Kết thúc khối try.
            catch // Nếu xảy ra lỗi.
            {
                await _unitOfWork.RollbackAsync(); // Khôi phục lại trạng thái DB.
                throw; // Bắn tiếp ngoại lệ.
            } // Kết thúc khối catch.
        }

        public async Task<RmaShipmentDetailDto?> CreateRmaShipmentAsync(int ticketId, RmaShipmentCreateDto request, Guid userId, bool isAdmin = false) // Định nghĩa phương thức tạo phiếu gửi bảo hành hãng (RMA Shipment).
        {
            var ticket = await _ticketRepository.GetByIdWithTrackingAsync(ticketId); // Lấy thực thể phiếu sửa chữa cần gửi hãng.
            if (ticket == null) // Nếu phiếu không tồn tại.
                throw new InvalidOperationException("Phiếu không tồn tại."); // Báo lỗi.

            CheckAssignment(ticket, userId, isAdmin); // Kiểm tra quyền phân công xử lý của kỹ thuật viên đối với phiếu này.

            if (ticket.ResolutionType != (byte)2) // Ràng buộc: Phiếu bắt buộc phải chọn luồng giải quyết là gửi bảo hành hãng RMA (2 - Rma).
                throw new InvalidOperationException("Phiếu phải chọn nhánh RMA."); // Báo lỗi nếu chọn sai luồng xử lý.

            if (!ticket.WasInWarrantyAtIntake) // Ràng buộc: Thiết bị phải còn hạn bảo hành đầu vào mới được gửi hãng bảo hành miễn phí.
                throw new InvalidOperationException("Sản phẩm đã hết bảo hành, không thể gửi RMA."); // Báo lỗi ngăn chặn gửi bảo hành đối với thiết bị hết hạn.

            var existingRma = await _rmaRepository.GetByTicketIdAsync(ticketId); // Kiểm tra xem phiếu dịch vụ sửa chữa này đã có thông tin RMA gửi hãng trước đó chưa.
            if (existingRma != null) // Nếu thông tin gửi hãng đã tồn tại.
                throw new InvalidOperationException("Phiếu này đã được gửi hãng rồi."); // Ngăn chặn tạo phiếu gửi hãng trùng lặp.

            ValidateTransition(ticket.Status, (byte)6); // Xác thực tính hợp lệ khi chuyển trạng thái phiếu sang Đang gửi hãng RMA (6 - RmaSent).

            await _unitOfWork.BeginTransactionAsync(); // Khởi tạo Transaction bảo đảm tính toàn vẹn dữ liệu.
            try // Bắt đầu khối an toàn.
            {
                var rma = new RmaShipment // Khởi tạo thực thể thông tin lô hàng gửi bảo hành hãng.
                {
                    TicketId = ticketId, // Id phiếu sửa chữa liên quan.
                    CarrierName = request.CarrierName, // Tên đơn vị vận chuyển hàng lên hãng (ví dụ: Viettel Post, Giaohangnhanh).
                    TrackingCode = request.TrackingCode, // Mã định danh theo dõi đơn vận chuyển hàng hóa.
                    ShippedDate = DateTime.UtcNow, // Thời điểm thực hiện bàn giao hàng cho đơn vị vận chuyển.
                    ShippedByEmployeeId = userId, // Id nhân viên phụ trách xuất hàng gửi đi.
                    ManufacturerResolution = (byte)0 // Mặc định kết quả giải quyết của hãng là Chưa xác định (0 - Pending).
                }; // Kết thúc khởi tạo.

                await _rmaRepository.AddAsync(rma); // Lưu thực thể gửi hãng mới vào cơ sở dữ liệu.

                ticket.Status = (byte)6; // Chuyển trạng thái phiếu dịch vụ sang Đang gửi hãng RMA (6).
                ticket.ModifiedDate = DateTime.UtcNow; // Cập nhật mốc thời gian sửa đổi gần nhất.

                await _ticketRepository.AddStatusHistoryAsync(new ServiceTicketStatusHistory // Ghi nhận lịch sử chuyển dịch trạng thái.
                {
                    TicketId = ticketId, // Mã phiếu sửa chữa.
                    FromStatus = (byte)1, // Từ Đang chẩn đoán.
                    ToStatus = (byte)6, // Sang Đã gửi hãng (RMA).
                    ChangedByEmployeeId = userId, // Nhân viên thực hiện xuất gửi.
                    ChangedAt = DateTime.UtcNow, // Thời điểm thực hiện.
                    Note = $"Gửi hãng qua {request.CarrierName}" // Ghi chú phương thức gửi.
                }); // Kết thúc lưu lịch sử.

                await _unitOfWork.SaveChangesAsync(); // Lưu thay đổi.
                await _unitOfWork.CommitAsync(); // Xác nhận hoàn tất Transaction.

                var reloadedRma = await _rmaRepository.GetByTicketIdAsync(ticketId); // Tải lại thông tin gửi bảo hành hãng vừa tạo.
                return MapRmaShipmentToDto(reloadedRma!); // Ánh xạ sang DTO hiển thị chi tiết và trả về.
            } // Kết thúc khối try.
            catch // Nếu xảy ra ngoại lệ.
            {
                await _unitOfWork.RollbackAsync(); // Rollback cơ sở dữ liệu về trạng thái trước đó.
                throw; // Tiếp tục bắn ngoại lệ lên.
            } // Kết thúc khối catch.
        }

        public async Task<bool> RecordRmaResolutionAsync(int ticketId, RmaResolutionUpdateDto request, Guid userId, bool isAdmin = false) // Định nghĩa phương thức ghi nhận kết quả bảo hành từ hãng.
        {
            var rma = await _rmaRepository.GetByTicketIdAsync(ticketId); // Lấy thực thể gửi hãng liên kết với phiếu dịch vụ sửa chữa.
            if (rma == null) // Nếu không tìm thấy thông tin gửi hãng RMA tương ứng.
                throw new InvalidOperationException("Không có phiếu RMA cho ticket này."); // Bắn ra ngoại lệ.

            var ticket = await _ticketRepository.GetByIdWithTrackingAsync(ticketId); // Lấy thực thể phiếu sửa chữa cần ghi nhận kết quả bảo hành hãng.
            if (ticket == null) // Nếu phiếu không tồn tại.
                throw new InvalidOperationException("Phiếu không tồn tại."); // Báo lỗi.

            CheckAssignment(ticket, userId, isAdmin); // Kiểm tra quyền xử lý phiếu của người thực hiện.

            byte toStatus = request.ManufacturerResolution switch // Xác định trạng thái đích tiếp theo của phiếu dựa trên kết quả giải quyết của hãng.
            {
                2 => (byte)8, // Nếu hãng đổi thiết bị mới 1-1 -> Chuyển sang trạng thái Đã đổi 1-1 (8).
                3 => (byte)1, // Nếu hãng từ chối bảo hành -> Chuyển ngược lại trạng thái Đang chẩn đoán (1) để xử lý sửa dịch vụ tính phí.
                _ => (byte)7  // Nếu hãng sửa chữa xong -> Chuyển sang trạng thái Đã nhận lại từ hãng (7).
            }; // Kết thúc chọn trạng thái đích.
            ValidateTransition(ticket.Status, toStatus); // Xác thực tính hợp lệ của luồng chuyển đổi trạng thái phiếu.

            await _unitOfWork.BeginTransactionAsync(); // Khởi chạy Transaction vì thao tác đổi mới 1-1 từ hãng đòi hỏi cập nhật nhiều bảng liên quan.
            try // Bắt đầu khối an toàn.
            {
                var now = DateTime.UtcNow; // Lấy thời điểm hiện tại.
                byte previousStatus = ticket.Status; // Lưu lại trạng thái trước đó của phiếu để ghi lịch sử.

                rma.ManufacturerResolution = request.ManufacturerResolution; // Ghi nhận mã phương án giải quyết của hãng.
                rma.ManufacturerNotes = request.ManufacturerNotes; // Ghi chép ghi chú giải quyết từ phía hãng (lý do từ chối, lỗi linh kiện...).
                rma.ReceivedBackDate = now; // Ghi nhận ngày nhận máy từ hãng gửi về cửa hàng.
                rma.ReceivedByEmployeeId = userId; // Ghi nhận Id nhân viên thực hiện nhận bàn giao máy từ hãng.

                if (request.ManufacturerResolution == (byte)2) // Nếu hãng chọn phương án đổi thiết bị mới 1-đổi-1.
                {
                    if (!request.ReplacementSerialId.HasValue) // Ràng buộc: Phải chỉ định cụ thể Id của số Serial mới thay thế do hãng cấp.
                        throw new InvalidOperationException("Phải cung cấp Serial thay thế khi hãng đã thay thế."); // Báo lỗi thiếu thông tin serial mới.

                    var newSerialId = request.ReplacementSerialId.Value; // Id của số Serial mới.
                    var oldSerial = ticket.Serial; // Thực thể Serial hỏng ban đầu.
                    var newSerial = await _serialRepository.GetByIdWithTrackingAsync(newSerialId); // Tra cứu thực thể số Serial thay thế mới từ DB.

                    if (newSerial == null || newSerial.Status != (byte)0) // Ràng buộc: Serial mới phải tồn tại và đang ở trạng thái khả dụng trong kho (0 - Available).
                        throw new InvalidOperationException("Serial thay thế không sẵn trong kho hoặc đã được giữ chỗ."); // Báo lỗi nếu serial mới không khả dụng.

                    if (newSerial.VariantId != oldSerial.VariantId) // Ràng buộc: Thiết bị mới phải cùng loại biến thể sản phẩm với thiết bị hỏng được bảo hành.
                        throw new InvalidOperationException("Serial thay thế phải cùng biến thể với serial hỏng."); // Báo lỗi lệch chủng loại sản phẩm.

                    var oldOrderSerial = await _dbContext.OrderSerials // Tìm bản ghi chi tiết xuất kho đơn hàng cũ của thiết bị hỏng ban đầu.
                        .FirstOrDefaultAsync(os => os.SerialId == oldSerial.Id); // Lọc theo Id của thiết bị cũ.
                    if (oldOrderSerial == null) // Nếu không tìm thấy thông tin xuất bán ban đầu của thiết bị cũ.
                        throw new InvalidOperationException("Không tìm thấy bản ghi xuất kho gốc của serial này."); // Bắn ra ngoại lệ để tránh sai lệch lịch sử bán hàng.

                    var oldWarranties = await _warrantyRepository.GetActiveBySerialIdAsync(oldSerial.Id); // Tra cứu lịch sử bản ghi bảo hành đang hoạt động của thiết bị cũ.
                    var oldEndDate = oldWarranties.FirstOrDefault()?.EndDate // Xác định thời hạn hết hạn bảo hành gốc để phục vụ việc chuyển tiếp bảo hành.
                        ?? oldSerial.SoldDate?.AddMonths(oldSerial.Variant.WarrantyMonth) // Nếu không có bản ghi bảo hành cũ thì tính từ ngày bán + tháng bảo hành mặc định.
                        ?? now; // Dự phòng cuối cùng là thời điểm hiện tại.

                    oldSerial.Status = (byte)4; // Đổi trạng thái thiết bị hỏng cũ sang Hỏng/Lỗi thu hồi (4 - Defective).

                    newSerial.Status = (byte)2; // Đổi trạng thái thiết bị thay thế mới sang Đã bán (2 - Sold).
                    newSerial.SoldDate = now; // Ghi nhận ngày bán thiết bị mới là hôm nay.
                    newSerial.OrderId = oldSerial.OrderId; // Gán liên kết đơn hàng gốc cũ cho thiết bị mới.

                    oldOrderSerial.SerialId = newSerialId; // Cập nhật lại số Serial xuất kho trong đơn hàng gốc từ máy cũ sang máy mới để bảo toàn hóa đơn.

                    if (oldWarranties.Count > 0) // Nếu có bản ghi bảo hành cũ đang chạy cho thiết bị cũ.
                    {
                        oldWarranties[0].Status = (byte)2; // Vô hiệu hóa bản ghi bảo hành cũ bằng cách đổi trạng thái sang Hủy/Thay thế (2 - Cancelled).
                    } // Kết thúc vô hiệu hóa.

                    var newWarranty = new Warranty // Khởi tạo bản ghi bảo hành mới cho thiết bị thay thế vừa kích hoạt.
                    {
                        SerialId = newSerialId, // Id thiết bị mới.
                        CustomerId = oldWarranties.FirstOrDefault()?.CustomerId, // Id khách hàng sở hữu.
                        OrderId = oldSerial.OrderId!.Value, // Đơn hàng mua gốc.
                        StartDate = now, // Ngày bắt đầu bảo hành mới tính từ hôm nay.
                        EndDate = oldEndDate, // Kế thừa nguyên vẹn ngày hết hạn bảo hành gốc của thiết bị cũ.
                        Status = (byte)0 // Trạng thái bảo hành hoạt động bình thường (0 - Active).
                    }; // Kết thúc khởi tạo.
                    await _warrantyRepository.AddAsync(newWarranty); // Lưu bản ghi bảo hành mới của thiết bị thay thế.

                    ticket.Status = (byte)8; // Chuyển trạng thái phiếu sửa chữa sang Đã đổi 1-1 (8).
                    ticket.ReplacementSerialId = newSerialId; // Lưu vết Id thiết bị thay thế mới vào phiếu dịch vụ.

                    await _logRepository.AddAsync(new SerialRepairLog // Ghi chép nhật ký lịch sử sửa chữa/bảo dưỡng cho thiết bị cũ.
                    {
                        SerialId = oldSerial.Id, // Id thiết bị cũ bị hỏng thu hồi.
                        TicketId = ticketId, // Liên kết phiếu sửa chữa.
                        ResolutionType = (byte)2, // Phương thức xử lý: RMA (2).
                        LoggedAt = now, // Mốc thời gian ghi nhận.
                        LoggedByEmployeeId = userId, // Nhân viên thực hiện.
                        Summary = $"Hãng thay thế sang serial {newSerial.SerialNumber}. Bảo hành kế thừa đến {oldEndDate:dd/MM/yyyy}.", // Nội dung tóm tắt kế thừa.
                        ReplacedBySerialId = newSerialId // Liên kết sang Id thiết bị mới thay thế.
                    }); // Kết thúc thêm nhật ký.
                } // Kết thúc khối xử lý đổi mới từ hãng.
                else // Nếu hãng sửa xong hoặc hãng từ chối bảo hành.
                {
                    ticket.Status = request.ManufacturerResolution == (byte)3 ? (byte)1 : (byte)7; // Quay về Đang chẩn đoán (1) nếu bị hãng từ chối, hoặc chuyển sang Nhận lại từ hãng (7) nếu đã sửa xong.
                } // Kết thúc.

                await _ticketRepository.AddStatusHistoryAsync(new ServiceTicketStatusHistory // Ghi nhận nhật ký lịch sử thay đổi trạng thái của phiếu.
                {
                    TicketId = ticketId, // Mã phiếu sửa chữa.
                    FromStatus = previousStatus, // Từ trạng thái cũ.
                    ToStatus = ticket.Status, // Sang trạng thái mới.
                    ChangedByEmployeeId = userId, // Người thực hiện.
                    ChangedAt = now, // Thời điểm.
                    Note = request.ManufacturerResolution switch // Tạo ghi chú phù hợp với kết quả của hãng.
                    {
                        2 => "Hãng thay thế, chuyển sang Đã đổi 1-1", // Đổi thiết bị.
                        3 => "Hãng từ chối, quay lại chẩn đoán", // Bị từ chối.
                        _ => "Nhận lại từ hãng" // Đã sửa xong.
                    } // Kết thúc switch.
                }); // Kết thúc lưu lịch sử.

                await _unitOfWork.SaveChangesAsync(); // Lưu các thay đổi xuống DB.
                await _unitOfWork.CommitAsync(); // Xác nhận hoàn tất Transaction.

                if (request.ManufacturerResolution == (byte)2) // Nếu xảy ra thay đổi cấu trúc thiết bị (đổi Serial).
                {
                    await _inventorySyncService.SyncStockBatchAsync(new[] { ticket.Serial.VariantId }); // Kích hoạt đồng bộ hóa số lượng tồn kho khả dụng của biến thể sản phẩm đó trong hệ thống.
                } // Kết thúc.

                return true; // Trả về ghi nhận thành công.
            } // Kết thúc khối try.
            catch // Nếu gặp lỗi.
            {
                await _unitOfWork.RollbackAsync(); // Khôi phục trạng thái DB ban đầu.
                throw; // Bắn lỗi lên tầng trên.
            } // Kết thúc khối catch.
        }

        public async Task<bool> Perform1For1SwapAsync(int ticketId, int newSerialId, Guid userId, bool isAdmin = false) // Định nghĩa phương thức đổi mới thiết bị 1-đổi-1 trực tiếp từ kho của cửa hàng.
        {
            var ticket = await _ticketRepository.GetByIdWithTrackingAsync(ticketId); // Lấy thực thể phiếu sửa chữa bảo hành cần xử lý đổi trả.
            if (ticket == null) // Nếu phiếu không tồn tại.
                throw new InvalidOperationException("Phiếu không tồn tại."); // Báo lỗi.

            CheckAssignment(ticket, userId, isAdmin); // Kiểm tra quyền hạn xử lý phiếu của kỹ thuật viên.

            if (ticket.Status != (byte)1 && ticket.Status != (byte)7) // Ràng buộc: Đổi 1-1 chỉ được thực hiện khi phiếu đang ở trạng thái chẩn đoán (1) hoặc đã nhận máy từ hãng về (7).
                throw new InvalidOperationException("Trạng thái phiếu không cho phép đổi 1-1."); // Báo lỗi trạng thái.

            if (ticket.ResolutionType != (byte)3 && ticket.ResolutionType != (byte)2) // Ràng buộc: Loại hướng giải quyết phải là Đổi mới (3 - Swap) hoặc gửi hãng bảo hành (2 - Rma).
                throw new InvalidOperationException("Loại giải pháp không phải đổi 1-1."); // Báo lỗi luồng nghiệp vụ.

            var liveWarranty = await WarrantyEvaluator.EvaluateAsync( // Thực hiện đánh giá lại hạn bảo hành thời gian thực tại thời điểm đổi máy (Tránh bảo hành hết hạn trong lúc chờ sửa).
                ticket.Serial, // Thiết bị hiện tại.
                ticket.Serial.Variant, // Biến thể đi kèm.
                _warrantyRepository); // Repository bảo hành.

            if (!liveWarranty.IsInWarranty) // Nếu tại thời điểm hiện tại thiết bị thực chất đã hết hạn bảo hành.
                throw new InvalidOperationException("Bảo hành đã hết hạn, không thể đổi 1-1."); // Ngăn chặn đổi thiết bị mới miễn phí.

            if (ticket.ReplacementSerialId.HasValue) // Kiểm tra xem phiếu này đã từng thực hiện đổi máy trước đây chưa.
                throw new InvalidOperationException("Phiếu này đã được đổi 1-1 trước đó."); // Tránh đổi máy nhiều lần trên một phiếu dịch vụ.

            ValidateTransition(ticket.Status, (byte)8); // Xác thực xem phiếu sửa chữa có thể chuyển sang trạng thái Đã đổi 1-1 (8) hay không.

            var oldSerial = await _serialRepository.GetByIdWithTrackingAsync(ticket.SerialId); // Lấy thực thể thiết bị hỏng hiện tại ở chế độ tracking.
            if (oldSerial == null) // Nếu thiết bị cũ không tồn tại.
                throw new InvalidOperationException("Serial cũ không tồn tại."); // Báo lỗi.

            var newSerial = await _serialRepository.GetByIdWithTrackingAsync(newSerialId); // Lấy thực thể thiết bị mới thay thế được chọn từ kho.
            if (newSerial == null || newSerial.Status != (byte)0) // Ràng buộc: Thiết bị thay thế phải có sẵn trong kho ở trạng thái Available (0).
                throw new InvalidOperationException("Serial thay thế không sẵn trong kho hoặc đã được giữ chỗ."); // Báo lỗi nếu thiết bị mới không khả dụng.

            if (newSerial.VariantId != oldSerial.VariantId) // Ràng buộc: Thiết bị mới phải cùng biến thể sản phẩm với thiết bị hỏng.
                throw new InvalidOperationException("Serial thay thế phải cùng biến thể với serial hỏng."); // Báo lỗi lệch biến thể.

            var oldOrderSerial = await _dbContext.OrderSerials // Lấy bản ghi chi tiết xuất kho ban đầu của thiết bị cũ trong đơn hàng gốc.
                .FirstOrDefaultAsync(os => os.SerialId == oldSerial.Id); // Lọc theo Id thiết bị cũ.
            if (oldOrderSerial == null) // Nếu không tìm thấy thông tin xuất kho.
                throw new InvalidOperationException("Không tìm thấy bản ghi xuất kho gốc của serial này."); // Bắn lỗi bảo toàn lịch sử bán hàng.

            var oldWarranties = await _warrantyRepository.GetActiveBySerialIdAsync(oldSerial.Id); // Lấy thông tin bảo hành đang chạy của thiết bị cũ.
            var oldEndDate = oldWarranties.FirstOrDefault()?.EndDate // Xác định ngày kết thúc bảo hành gốc của khách.
                ?? oldSerial.SoldDate?.AddMonths(oldSerial.Variant.WarrantyMonth) // Dự phòng.
                ?? DateTime.UtcNow; // Dự phòng mặc định.

            await _unitOfWork.BeginTransactionAsync(); // Khởi tạo Transaction bảo vệ dữ liệu do tác động nhiều bảng.
            try // Bắt đầu khối an toàn.
            {
                var now = DateTime.UtcNow; // Lấy thời gian UTC hiện tại.
                byte previousStatus = ticket.Status; // Lưu trạng thái phiếu trước khi cập nhật.

                oldSerial.Status = (byte)4; // 1. Thu hồi máy lỗi: Chuyển trạng thái thiết bị cũ sang Hỏng (4 - Defective).

                newSerial.Status = (byte)2; // 2. Xuất máy mới: Chuyển trạng thái thiết bị mới thay thế sang Đã bán (2 - Sold).
                newSerial.SoldDate = now; // Ghi nhận ngày kích hoạt xuất bán thiết bị mới là hôm nay.
                newSerial.OrderId = oldSerial.OrderId; // Liên kết thiết bị mới với đơn hàng mua ban đầu của khách.

                oldOrderSerial.SerialId = newSerialId; // 3. Hoán đổi liên kết: Cập nhật mã thiết bị xuất bán trong đơn hàng cũ thành thiết bị mới để khớp hóa đơn bán hàng gốc.

                if (oldWarranties.Count > 0) // 4. Hủy hiệu lực bảo hành của thiết bị cũ hỏng.
                {
                    oldWarranties[0].Status = (byte)2; // Chuyển trạng thái sang Hủy/Thay thế (2).
                } // Kết thúc.

                var newWarranty = new Warranty // 5. Tạo mới bản ghi bảo hành cho thiết bị thay thế vừa giao cho khách.
                {
                    SerialId = newSerialId, // Id thiết bị mới.
                    CustomerId = oldWarranties.FirstOrDefault()?.CustomerId, // Id khách sở hữu.
                    OrderId = oldSerial.OrderId!.Value, // Mã đơn hàng.
                    StartDate = now, // Ngày bắt đầu là hôm nay.
                    EndDate = oldEndDate, // Kế thừa nguyên vẹn ngày hết hạn bảo hành gốc của thiết bị cũ để tránh gian lận thời gian bảo hành.
                    Status = (byte)0 // Bảo hành hoạt động bình thường (0 - Active).
                }; // Kết thúc khởi tạo.
                await _warrantyRepository.AddAsync(newWarranty); // Lưu bảo hành mới.

                ticket.Status = (byte)8; // Cập nhật trạng thái phiếu dịch vụ sửa chữa sang Đã đổi 1-1 (8).
                ticket.ReplacementSerialId = newSerialId; // Lưu Id thiết bị thay thế mới vào phiếu sửa chữa.
                ticket.ResolutionType = (byte)3; // Chốt loại hướng giải quyết của phiếu là Đổi mới thiết bị (3 - Swap).
                ticket.ModifiedDate = now; // Ghi nhận thời gian cập nhật.

                await _ticketRepository.AddStatusHistoryAsync(new ServiceTicketStatusHistory // Lưu lịch sử biến động trạng thái phiếu sửa chữa.
                {
                    TicketId = ticketId, // Mã phiếu sửa chữa.
                    FromStatus = previousStatus, // Từ trạng thái cũ.
                    ToStatus = (byte)8, // Sang Đã đổi 1-1 (8).
                    ChangedByEmployeeId = userId, // Người thực hiện.
                    ChangedAt = now, // Thời gian.
                    Note = $"Đổi 1-1 sang serial {newSerial.SerialNumber}" // Ghi chú chi tiết mã số thiết bị mới.
                }); // Kết thúc lưu lịch sử.

                await _logRepository.AddAsync(new SerialRepairLog // Ghi chép lịch sử bảo dưỡng và thu hồi đối với thiết bị lỗi cũ.
                {
                    SerialId = oldSerial.Id, // Id thiết bị hỏng cũ.
                    TicketId = ticketId, // Mã phiếu sửa chữa liên quan.
                    ResolutionType = (byte)3, // Kiểu giải quyết: Đổi trả 1-1 (3).
                    LoggedAt = now, // Thời điểm.
                    LoggedByEmployeeId = userId, // Nhân viên ghi nhận.
                    Summary = $"Đổi 1-1 sang serial {newSerial.SerialNumber}. Bảo hành kế thừa đến {oldEndDate:dd/MM/yyyy}.", // Chi tiết nội dung.
                    ReplacedBySerialId = newSerialId // Trỏ sang Id thiết bị mới thay thế để dễ đối chiếu ngược.
                }); // Đóng bản ghi lịch sử.

                await _unitOfWork.SaveChangesAsync(); // Lưu toàn bộ thay đổi xuống DB.
                await _unitOfWork.CommitAsync(); // Xác nhận Transaction thành công.

                await _inventorySyncService.SyncStockBatchAsync(new[] { oldSerial.VariantId }); // Kích hoạt tiến trình đồng bộ số lượng tồn kho vật lý của biến thể sản phẩm bị đổi máy trong RAM.

                return true; // Trả về đổi thiết bị thành công.
            } // Kết thúc khối try.
            catch // Nếu lỗi.
            {
                await _unitOfWork.RollbackAsync(); // Khôi phục lại cơ sở dữ liệu về trạng thái sạch ban đầu.
                throw; // Bắn tiếp ngoại lệ lên.
            } // Kết thúc khối catch.
        }

        public async Task<bool> MarkInternalRepairCompletedAsync(int ticketId, ServiceTicketCompleteDto request, Guid userId, bool isAdmin = false) // Định nghĩa phương thức xác nhận hoàn tất sửa chữa nội bộ tại cửa hàng.
        {
            var ticket = await _ticketRepository.GetByIdWithTrackingAsync(ticketId); // Lấy thực thể phiếu sửa chữa cần hoàn tất ở chế độ tracking.
            if (ticket == null) // Nếu phiếu không tồn tại.
                throw new InvalidOperationException("Phiếu không tồn tại."); // Báo lỗi.

            CheckAssignment(ticket, userId, isAdmin); // Kiểm tra quyền hạn phụ trách của kỹ thuật viên đối với phiếu này.

            if (!new[] { (byte)5, (byte)7, (byte)8 }.Contains(ticket.Status)) // Ràng buộc: Phiếu phải đang ở trạng thái Đang sửa (5), Đã nhận lại từ hãng (7) hoặc Đã đổi 1-1 (8).
                throw new InvalidOperationException("Phiếu phải ở trạng thái sửa chữa hoặc đã nhận từ hãng."); // Báo lỗi nếu sai trạng thái quy trình.

            ValidateTransition(ticket.Status, (byte)9); // Xác thực xem phiếu sửa chữa có thể chuyển sang trạng thái đích là Hoàn tất (9 - Completed) hay không.

            await _unitOfWork.BeginTransactionAsync(); // Khởi tạo Transaction bảo vệ quy trình lưu trữ đồng thời.
            try // Bắt đầu khối try an toàn.
            {
                byte previousStatus = ticket.Status; // Chụp lại trạng thái trước khi thay đổi để lưu lịch sử chuyển trạng thái chính xác.

                ticket.Status = (byte)9; // Cập nhật trạng thái phiếu sửa chữa sang Hoàn tất (9).
                ticket.CompletedDate = DateTime.UtcNow; // Ghi nhận thời điểm hoàn thành sửa chữa thực tế.
                ticket.ModifiedDate = DateTime.UtcNow; // Cập nhật mốc thời gian sửa đổi gần nhất.

                await _ticketRepository.AddStatusHistoryAsync(new ServiceTicketStatusHistory // Thêm lịch sử chuyển đổi trạng thái của phiếu.
                {
                    TicketId = ticketId, // Mã phiếu sửa chữa.
                    FromStatus = previousStatus, // Trạng thái xuất phát.
                    ToStatus = (byte)9, // Trạng thái đích: Hoàn tất (9).
                    ChangedByEmployeeId = userId, // Kỹ thuật viên thực hiện.
                    ChangedAt = DateTime.UtcNow, // Thời điểm.
                    Note = "Hoàn tất sửa chữa" // Ghi chú.
                }); // Kết thúc lưu lịch sử.

                if (previousStatus != (byte)8) // Nghiệp vụ: Tránh ghi nhật ký sửa chữa trùng lặp nếu phiếu đã được lưu log chi tiết trong quá trình đổi thiết bị (Swap - 8).
                {
                    await _logRepository.AddAsync(new SerialRepairLog // Lưu vết lịch sử sửa chữa/bảo dưỡng cho thiết bị này.
                    {
                        SerialId = ticket.SerialId, // Id thiết bị sửa.
                        TicketId = ticketId, // Mã phiếu dịch vụ.
                        ResolutionType = ticket.ResolutionType, // Loại hướng giải quyết.
                        LoggedAt = DateTime.UtcNow, // Thời điểm.
                        LoggedByEmployeeId = userId, // Nhân viên.
                        Summary = request.Note ?? "Sửa chữa xong" // Chi tiết nội dung công việc đã xử lý (ví dụ: Vệ sinh, khò chip, cài lại Win...).
                    }); // Đóng bản ghi.
                } // Kết thúc kiểm tra tránh trùng.

                await _unitOfWork.SaveChangesAsync(); // Lưu các thay đổi xuống DB.
                await _unitOfWork.CommitAsync(); // Xác nhận Transaction.
                return true; // Trả về hoàn tất thành công.
            } // Kết thúc khối try.
            catch // Nếu gặp lỗi.
            {
                await _unitOfWork.RollbackAsync(); // Hủy bỏ thay đổi.
                throw; // Bắn tiếp ngoại lệ.
            } // Kết thúc khối catch.
        }

        public async Task<bool> MarkWaitingPartsAsync(int ticketId, Guid userId, bool isAdmin = false) // Định nghĩa phương thức chuyển trạng thái phiếu sang Chờ linh kiện.
        {
            var ticket = await _ticketRepository.GetByIdWithTrackingAsync(ticketId); // Lấy thực thể phiếu sửa chữa cần cập nhật.
            if (ticket == null) // Nếu không tìm thấy phiếu.
                throw new InvalidOperationException("Phiếu không tồn tại."); // Báo lỗi.

            CheckAssignment(ticket, userId, isAdmin); // Kiểm tra quyền phụ trách của kỹ thuật viên.

            if (ticket.Status != (byte)5) // Ràng buộc: Phiếu phải đang ở trạng thái Đang sửa (5).
                throw new InvalidOperationException("Phiếu phải ở trạng thái Đang sửa."); // Báo lỗi trạng thái.

            ValidateTransition(ticket.Status, (byte)4); // Xác thực xem phiếu có được phép chuyển từ Đang sửa (5) sang Chờ phụ tùng (4 - WaitingParts) hay không.

            ticket.Status = (byte)4; // Chuyển trạng thái phiếu sửa chữa sang Chờ phụ tùng (4).
            ticket.ModifiedDate = DateTime.UtcNow; // Ghi nhận thời gian cập nhật.

            await _ticketRepository.AddStatusHistoryAsync(new ServiceTicketStatusHistory // Thêm lịch sử chuyển đổi trạng thái của phiếu.
            {
                TicketId = ticketId, // Mã phiếu.
                FromStatus = (byte)5, // Từ Đang sửa.
                ToStatus = (byte)4, // Sang Chờ phụ tùng.
                ChangedByEmployeeId = userId, // Kỹ thuật viên thực hiện.
                ChangedAt = DateTime.UtcNow, // Thời điểm.
                Note = "Chờ phụ tùng" // Ghi chú lý do chuyển trạng thái.
            }); // Kết thúc lưu lịch sử.

            await _ticketRepository.SaveChangesAsync(); // Lưu các thay đổi xuống cơ sở dữ liệu.
            return true; // Trả về thành công.
        }

        public async Task<bool> ResumeRepairAsync(int ticketId, Guid userId, bool isAdmin = false) // Định nghĩa phương thức tiếp tục sửa chữa thiết bị sau khi nhận được linh kiện.
        {
            var ticket = await _ticketRepository.GetByIdWithTrackingAsync(ticketId); // Lấy thực thể phiếu sửa chữa cần tiếp tục xử lý.
            if (ticket == null) // Nếu phiếu không tồn tại.
                throw new InvalidOperationException("Phiếu không tồn tại."); // Báo lỗi.

            CheckAssignment(ticket, userId, isAdmin); // Kiểm tra quyền của nhân viên phụ trách.

            if (ticket.Status != (byte)4) // Ràng buộc: Phiếu phải đang ở trạng thái Chờ phụ tùng (4).
                throw new InvalidOperationException("Phiếu phải ở trạng thái Chờ phụ tùng."); // Báo lỗi trạng thái.

            ValidateTransition(ticket.Status, (byte)5); // Xác thực xem có được phép chuyển từ Chờ phụ tùng (4) sang Đang sửa (5) hay không.

            ticket.Status = (byte)5; // Chuyển trạng thái phiếu sửa chữa về lại Đang sửa (5).
            ticket.ModifiedDate = DateTime.UtcNow; // Ghi nhận thời điểm cập nhật.

            await _ticketRepository.AddStatusHistoryAsync(new ServiceTicketStatusHistory // Thêm lịch sử chuyển đổi trạng thái của phiếu.
            {
                TicketId = ticketId, // Mã phiếu.
                FromStatus = (byte)4, // Từ Chờ phụ tùng.
                ToStatus = (byte)5, // Sang Đang sửa.
                ChangedByEmployeeId = userId, // Người thực hiện.
                ChangedAt = DateTime.UtcNow, // Thời điểm.
                Note = "Tiếp tục sửa chữa" // Ghi chú hành động.
            }); // Kết thúc ghi nhận.

            await _ticketRepository.SaveChangesAsync(); // Lưu thay đổi vào DB.
            return true; // Trả về thành công.
        }

        public async Task<bool> StartRepairAsync(int ticketId, Guid userId, bool isAdmin = false) // Định nghĩa phương thức bắt đầu tiến trình sửa chữa bảo hành nội bộ.
        {
            var ticket = await _ticketRepository.GetByIdWithTrackingAsync(ticketId); // Lấy thực thể phiếu sửa chữa bảo hành cần bắt đầu sửa.
            if (ticket == null) // Nếu không tìm thấy phiếu.
                throw new InvalidOperationException("Phiếu không tồn tại."); // Báo lỗi.

            CheckAssignment(ticket, userId, isAdmin); // Kiểm tra quyền phụ trách của nhân viên.

            if (ticket.ResolutionType != (byte)1) // Ràng buộc: Phương thức này chỉ được gọi cho phiếu đi theo luồng Bảo hành nội bộ miễn phí (1 - InternalRepair).
                throw new InvalidOperationException("Chỉ áp dụng cho phiếu sửa chữa bảo hành nội bộ."); // Báo lỗi nếu sai luồng xử lý.

            ValidateTransition(ticket.Status, (byte)5); // Xác thực xem phiếu có được phép chuyển đổi sang trạng thái Đang sửa (5) hay không.
            byte prev = ticket.Status; // Lưu trạng thái trước khi sửa đổi.
            ticket.Status = (byte)5; // Chuyển trạng thái phiếu sửa chữa sang Đang sửa (5).
            ticket.ModifiedDate = DateTime.UtcNow; // Ghi nhận thời điểm chỉnh sửa gần nhất.

            await _ticketRepository.AddStatusHistoryAsync(new ServiceTicketStatusHistory // Thêm lịch sử chuyển đổi trạng thái của phiếu.
            {
                TicketId = ticketId, // Mã phiếu sửa chữa.
                FromStatus = prev, // Trạng thái cũ của phiếu (thường là Đang chẩn đoán - 1).
                ToStatus = (byte)5, // Sang Đang sửa (5).
                ChangedByEmployeeId = userId, // Người thực hiện.
                ChangedAt = DateTime.UtcNow, // Thời điểm.
                Note = "Bắt đầu sửa chữa bảo hành" // Nội dung lưu vết lịch sử.
            }); // Kết thúc.

            await _ticketRepository.SaveChangesAsync(); // Lưu các thay đổi xuống DB.
            return true; // Trả về thành công.
        }

        public async Task<ServiceInvoiceDetailDto?> IssueServiceInvoiceAsync(int ticketId, ServiceInvoiceCreateDto request, Guid userId, bool isAdmin = false) // Định nghĩa phương thức lập hóa đơn thanh toán dịch vụ (Service Invoice).
        {
            var ticket = await _ticketRepository.GetByIdWithDetailsAsync(ticketId); // Lấy thực thể phiếu sửa chữa kèm đầy đủ quan hệ liên quan.
            if (ticket == null) // Nếu phiếu không tồn tại.
                throw new InvalidOperationException("Phiếu không tồn tại."); // Báo lỗi.

            CheckAssignment(ticket, userId, isAdmin); // Kiểm tra quyền phụ trách của nhân viên đối với phiếu sửa chữa này.

            if (ticket.Status != (byte)9) // Ràng buộc pháp lý: Chỉ được phép xuất hóa đơn thanh toán khi thiết bị đã hoàn tất sửa chữa (9 - Completed).
                throw new InvalidOperationException("Phiếu phải ở trạng thái Hoàn tất."); // Báo lỗi nếu thiết bị chưa sửa xong.

            if (ticket.ResolutionType != (byte)4) // Ràng buộc nghiệp vụ: Chỉ phiếu sửa tính phí dịch vụ (4 - PaidRepair) mới cần xuất hóa đơn thanh toán.
                throw new InvalidOperationException("Chỉ sửa tính phí mới tạo hóa đơn."); // Báo lỗi nếu xuất hóa đơn cho thiết bị bảo hành miễn phí.

            if (await _invoiceRepository.InvoiceExistsForTicketAsync(ticketId)) // Ràng buộc: Mỗi phiếu sửa chữa tính phí chỉ được phép có tối đa 1 hóa đơn thanh toán dịch vụ.
                throw new InvalidOperationException("Hóa đơn cho phiếu này đã tồn tại."); // Ngăn chặn xuất hóa đơn trùng lặp gây thất thoát tài chính.

            var quotations = await _quotationRepository.GetByTicketIdAsync(ticketId); // Tra cứu danh sách toàn bộ báo giá liên kết với phiếu dịch vụ này.
            var acceptedQuote = quotations.FirstOrDefault(q => q.Status == (byte)1); // Tìm kiếm bản báo giá duy nhất đã được khách duyệt đồng ý sửa (1 - Approved).
            if (acceptedQuote == null) // Nếu không tìm thấy báo giá nào được duyệt.
                throw new InvalidOperationException("Không tìm thấy báo giá được duyệt."); // Báo lỗi vì không có căn cứ áp dụng đơn giá thanh toán dịch vụ.

            await _unitOfWork.BeginTransactionAsync(); // Khởi động Transaction bảo vệ tiến trình lưu hóa đơn và sao chép chi tiết phụ tùng đồng thời.
            try // Bắt đầu khối try an toàn.
            {
                var now = DateTime.UtcNow; // Lấy thời gian UTC hiện tại.
                var datePrefix = "SRV-" + now.ToString("yyyyMMdd"); // Tạo tiền tố mã hóa đơn tự động dạng SRV-yyyyMMdd.
                var lastCode = await _invoiceRepository.GetLastInvoiceCodeByDateAsync(datePrefix); // Tra cứu mã hóa đơn lớn nhất được xuất trong ngày hôm nay.
                int nextIndex = 1; // Khởi tạo chỉ số số thứ tự hóa đơn tiếp theo là 1.
                if (!string.IsNullOrEmpty(lastCode)) // Nếu trong ngày đã có hóa đơn được xuất.
                {
                    var suffix = lastCode.Substring(lastCode.LastIndexOf('-') + 1); // Cắt lấy phần số thứ tự ở cuối mã.
                    if (int.TryParse(suffix, out int lastIdx)) // Chuyển chuỗi số sang kiểu số nguyên.
                        nextIndex = lastIdx + 1; // Số thứ tự tiếp theo tăng thêm 1 đơn vị.
                } // Kết thúc tính số thứ tự.
                var invoiceCode = $"{datePrefix}-{nextIndex:D3}"; // Tạo mã hóa đơn hoàn chỉnh (ví dụ: SRV-20260531-001).

                var invoice = new ServiceInvoice // Khởi tạo thực thể hóa đơn dịch vụ mới.
                {
                    InvoiceCode = invoiceCode, // Gán mã hóa đơn tự sinh.
                    TicketId = ticketId, // Mã phiếu sửa chữa liên quan.
                    QuotationId = acceptedQuote.Id, // Liên kết với bảng báo giá được khách duyệt.
                    IssuedDate = now, // Ngày xuất hóa đơn.
                    IssuedByEmployeeId = userId, // Id nhân viên lập hóa đơn thanh toán.
                    LaborCost = acceptedQuote.LaborCost, // Kế thừa chi phí tiền công sửa chữa từ báo giá đã duyệt.
                    PartsTotal = acceptedQuote.PartsTotal, // Kế thừa tổng chi phí linh kiện từ báo giá đã duyệt.
                    GrandTotal = acceptedQuote.GrandTotal, // Kế thừa tổng số tiền thanh toán từ báo giá đã duyệt.
                    PaymentMethod = request.PaymentMethod, // Gán phương thức thanh toán khách sử dụng (ví dụ: Tiền mặt, Chuyển khoản).
                    PaymentStatus = (byte)0, // Mặc định trạng thái thanh toán hóa đơn mới tạo là Chưa thanh toán (0 - Unpaid).
                    Note = request.Note // Ghi chú hóa đơn nếu có.
                }; // Kết thúc khởi tạo hóa đơn.

                await _invoiceRepository.AddAsync(invoice); // Thêm hóa đơn dịch vụ vào DB.
                await _unitOfWork.SaveChangesAsync(); // Lưu thay đổi tạm thời để lấy Id hóa đơn.

                var quotationItems = await _dbContext.QuotationItems // Lấy danh sách toàn bộ các khoản mục linh kiện/dịch vụ từ báo giá đã duyệt.
                    .Where(qi => qi.QuotationId == acceptedQuote.Id) // Lọc theo Id báo giá được duyệt.
                    .ToListAsync(); // Chuyển đổi sang danh sách List.

                foreach (var qItem in quotationItems) // Duyệt qua từng linh kiện thay thế từ báo giá để sao chép vào hóa đơn chính thức.
                {
                    await _dbContext.ServiceInvoiceItems.AddAsync(new ServiceInvoiceItem // Thêm chi tiết khoản mục hóa đơn dịch vụ vào DB.
                    {
                        InvoiceId = invoice.Id, // Liên kết với hóa đơn cha vừa lập.
                        VariantId = qItem.VariantId, // Id biến thể linh kiện thay thế.
                        Description = qItem.Description, // Mô tả linh kiện/dịch vụ.
                        Quantity = qItem.Quantity, // Số lượng thay thế thực tế.
                        UnitPrice = qItem.UnitPrice, // Đơn giá thanh toán.
                        LineTotal = qItem.Quantity * qItem.UnitPrice // Tổng tiền của dòng chi tiết này.
                    }); // Kết thúc thêm dòng hóa đơn.
                } // Kết thúc vòng lặp.

                await _unitOfWork.SaveChangesAsync(); // Lưu toàn bộ thay đổi.
                await _unitOfWork.CommitAsync(); // Xác nhận hoàn tất Transaction thanh toán.

                var reloadedInvoice = await _invoiceRepository.GetByIdWithDetailsAsync(invoice.Id); // Tải lại thông tin hóa đơn dịch vụ đầy đủ chi tiết linh kiện.
                return MapServiceInvoiceToDto(reloadedInvoice); // Ánh xạ sang DTO chi tiết và trả về.
            } // Kết thúc khối try.
            catch // Nếu gặp sự cố.
            {
                await _unitOfWork.RollbackAsync(); // Khôi phục lại DB về trạng thái sạch để đảm bảo tính nhất quán tài chính.
                throw; // Bắn ngoại lệ.
            } // Kết thúc khối catch.
        }

        public async Task<bool> CancelTicketAsync(int ticketId, string reason, Guid userId) // Định nghĩa phương thức hủy bỏ phiếu dịch vụ sửa chữa.
        {
            var ticket = await _ticketRepository.GetByIdWithTrackingAsync(ticketId); // Lấy thực thể phiếu sửa chữa cần hủy ở chế độ tracking.
            if (ticket == null) // Nếu phiếu không tồn tại.
                throw new InvalidOperationException("Phiếu không tồn tại."); // Báo lỗi.

            if (new[] { (byte)3, (byte)9, (byte)10 }.Contains(ticket.Status)) // Ràng buộc: Không được phép hủy phiếu đã đóng cửa (Từ chối báo giá - 3, Hoàn tất - 9, hoặc Đã hủy - 10).
                throw new InvalidOperationException("Không thể thay đổi trạng thái phiếu đã đóng."); // Báo lỗi quy trình.

            ValidateTransition(ticket.Status, (byte)10); // Xác thực tính hợp lệ của luồng chuyển trạng thái phiếu sang Đã hủy (10 - Cancelled).

            byte previousStatus = ticket.Status; // Lưu trạng thái trước khi hủy.

            ticket.Status = (byte)10; // Đổi trạng thái phiếu dịch vụ sang Đã hủy (10).
            ticket.CancelReason = reason; // Ghi chép lý do hủy phiếu (khách đổi ý, thiết bị hỏng nặng không thể phục hồi...).
            ticket.CancelledAt = DateTime.UtcNow; // Ghi nhận thời điểm hủy phiếu.
            ticket.ModifiedDate = DateTime.UtcNow; // Cập nhật thời điểm sửa đổi.

            await _ticketRepository.AddStatusHistoryAsync(new ServiceTicketStatusHistory // Thêm lịch sử chuyển trạng thái.
            {
                TicketId = ticketId, // Mã phiếu sửa chữa.
                FromStatus = previousStatus, // Từ trạng thái cũ.
                ToStatus = (byte)10, // Sang Đã hủy (10).
                ChangedByEmployeeId = userId, // Người thực hiện.
                ChangedAt = DateTime.UtcNow, // Thời điểm.
                Note = reason // Ghi chép lý do hủy vào lịch sử chuyển trạng thái.
            }); // Kết thúc lưu lịch sử.

            await _ticketRepository.SaveChangesAsync(); // Lưu các thay đổi xuống cơ sở dữ liệu.
            return true; // Trả về hủy thành công.
        }

        public async Task<ServiceTicketDetailDto?> GetTicketByIdAsync(int id, Guid? currentUserId = null, bool isCustomer = false) // Định nghĩa phương thức lấy thông tin chi tiết phiếu sửa chữa theo Id.
        {
            var ticket = await _ticketRepository.GetByIdWithDetailsAsync(id); // Gọi repository lấy thông tin phiếu chi tiết kèm đầy đủ quan hệ.
            if (ticket == null) // Nếu không tìm thấy phiếu nào khớp Id.
                return null; // Trả về null.

            if (isCustomer) // Nếu người yêu cầu tra cứu là khách hàng (Customer).
            {
                if (!ticket.CustomerId.HasValue || ticket.CustomerId != currentUserId) // Phòng chống lỗ hổng bảo mật IDOR: Kiểm tra xem phiếu có thuộc sở hữu của khách hàng đang đăng nhập hay không.
                    throw new UnauthorizedAccessException("Phiếu này không thuộc về bạn."); // Bắn ngoại lệ từ chối quyền truy cập nếu xem phiếu của khách khác hoặc phiếu vãng lai không tài khoản.
            } // Kết thúc kiểm tra bảo mật.

            return MapToDetailDto(ticket); // Ánh xạ thực thể phiếu sang DTO chi tiết và trả về.
        }

        public async Task<(List<ServiceTicketListDto> Items, int TotalCount)> GetPagedTicketsAsync( // Định nghĩa phương thức lấy danh sách phiếu sửa chữa phân trang có bộ lọc (quản trị viên).
            string? keyword, byte? status, byte? resolutionType, Guid? assignedEmployeeId, Guid? customerId, // Bộ lọc theo từ khóa, trạng thái phiếu, loại hướng giải quyết, nhân viên phụ trách, khách hàng.
            DateTime? fromDate, DateTime? toDate, int pageNumber, int pageSize, string? sortBy, bool sortDescending) // Thời gian tiếp nhận, trang hiện tại, số phần tử trên trang, cấu hình sắp xếp.
        {
            var (items, totalCount) = await _ticketRepository.GetPagedListAsync( // Gọi repository thực hiện truy vấn danh sách phân trang thỏa mãn bộ lọc.
                keyword, status, resolutionType, assignedEmployeeId, customerId, // Bộ lọc đầu vào.
                fromDate, toDate, pageNumber, pageSize, sortBy, sortDescending); // Tham số phân trang.

            var dtos = items.Select(t => new ServiceTicketListDto // Ánh xạ danh sách thực thể thu được sang danh sách DTO rút gọn hiển thị bảng.
            {
                Id = t.Id, // Ánh xạ Id.
                TicketCode = t.TicketCode, // Ánh xạ mã phiếu.
                SerialNumber = t.Serial.SerialNumber, // Ánh xạ số Serial của máy.
                ProductName = t.Serial.Variant.Product.Name, // Ánh xạ tên dòng sản phẩm.
                IntakeDate = t.IntakeDate, // Ánh xạ ngày tiếp nhận.
                Status = t.Status, // Ánh xạ mã trạng thái.
                StatusLabel = GetStatusLabel(t.Status) // Chuyển đổi mã trạng thái sang nhãn tiếng Việt dễ đọc (ví dụ: "Đang sửa chữa").
            }).ToList(); // Đóng gói thành List.

            return (dtos, totalCount); // Trả về kết quả phân trang gồm danh sách DTO và tổng đếm.
        }

        public async Task<(List<ServiceTicketListDto> Items, int TotalCount)> GetMyTicketsAsync( // Định nghĩa phương thức lấy danh sách phiếu dịch vụ của cá nhân khách hàng/nhân viên đăng nhập.
            Guid userId, string? keyword, byte? status, int pageNumber, int pageSize, string? sortBy, bool sortDescending) // Id người dùng, từ khóa, trạng thái lọc, thông số phân trang.
        {
            var (items, totalCount) = await _ticketRepository.GetTicketsByOrderUserIdAsync( // Gọi repository lấy danh sách phiếu liên kết với Id người dùng được lọc.
                userId, keyword, status, pageNumber, pageSize, sortBy, sortDescending); // Truyền tham số lọc.

            var dtos = items.Select(t => new ServiceTicketListDto // Ánh xạ danh sách thực thể sang DTO rút gọn.
            {
                Id = t.Id, // Ánh xạ Id.
                TicketCode = t.TicketCode, // Ánh xạ mã phiếu.
                SerialNumber = t.Serial.SerialNumber, // Ánh xạ số Serial máy.
                ProductName = t.Serial.Variant.Product.Name, // Ánh xạ tên dòng sản phẩm.
                IntakeDate = t.IntakeDate, // Ánh xạ ngày tiếp nhận máy.
                Status = t.Status, // Ánh xạ mã trạng thái.
                StatusLabel = GetStatusLabel(t.Status) // Chuyển mã trạng thái sang nhãn tiếng Việt hiển thị giao diện khách.
            }).ToList(); // Đóng gói thành danh sách List.

            return (dtos, totalCount); // Trả về bộ kết quả phân trang.
        }

        public async Task<List<ServiceTicketStatusHistoryDto>> GetTicketHistoryAsync(int ticketId) // Định nghĩa phương thức lấy lịch sử nhật ký chuyển trạng thái của phiếu sửa chữa.
        {
            var ticket = await _ticketRepository.GetByIdWithDetailsAsync(ticketId); // Lấy thực thể phiếu sửa chữa kèm đầy đủ quan hệ chi tiết từ repository.
            if (ticket == null) // Nếu phiếu sửa chữa không tồn tại.
                return new List<ServiceTicketStatusHistoryDto>(); // Trả về danh sách rỗng.

            return ticket.StatusHistory // Lấy tập hợp lịch sử trạng thái của phiếu.
                .OrderByDescending(h => h.ChangedAt) // Sắp xếp theo mốc thời gian thay đổi mới nhất lên hàng đầu.
                .Select(h => new ServiceTicketStatusHistoryDto // Ánh xạ từng bản ghi lịch sử sang DTO hiển thị.
                {
                    Id = h.Id, // Id bản ghi lịch sử.
                    FromStatus = h.FromStatus, // Trạng thái nguồn.
                    FromStatusLabel = GetStatusLabel(h.FromStatus), // Nhãn tiếng Việt trạng thái nguồn.
                    ToStatus = h.ToStatus, // Trạng thái đích.
                    ToStatusLabel = GetStatusLabel(h.ToStatus), // Nhãn tiếng Việt trạng thái đích.
                    ChangedAt = h.ChangedAt, // Thời điểm xảy ra chuyển đổi.
                    Note = h.Note // Ghi chú thao tác lúc chuyển đổi (ví dụ: lý do hủy, người phân công...).
                }) // Kết thúc ánh xạ.
                .ToList(); // Đóng gói thành danh sách List.
        }

        public async Task<List<SerialRepairHistoryDto>> GetSerialRepairHistoryAsync(string serialNumber) // Định nghĩa phương thức tra cứu lịch sử các lần sửa chữa trong quá khứ của thiết bị theo số Serial.
        {
            var serial = await _serialRepository.GetBySerialNumberAsync(serialNumber); // Tra cứu số Serial trong hệ thống.
            if (serial == null) // Nếu không tìm thấy thiết bị nào tương ứng.
                return new List<SerialRepairHistoryDto>(); // Trả về danh sách lịch sử sửa chữa rỗng.

            var logs = await _logRepository.GetBySerialIdAsync(serial.Id); // Tra cứu toàn bộ các nhật ký sửa chữa (SerialRepairLogs) liên kết với Id thiết bị này trong quá khứ.
            return logs.Select(l => new SerialRepairHistoryDto // Ánh xạ danh sách nhật ký thực thể sang danh sách DTO lịch sử.
            {
                Id = l.Id, // Id nhật ký.
                SerialId = l.SerialId, // Id thiết bị.
                TicketId = l.TicketId, // Id phiếu sửa chữa thực hiện lần đó.
                TicketCode = l.Ticket?.TicketCode, // Mã phiếu sửa chữa liên quan (nếu có).
                ResolutionType = l.ResolutionType, // Loại hướng giải quyết của lần đó (ví dụ: bảo hành hãng, sửa nội bộ...).
                ResolutionTypeLabel = GetResolutionLabel(l.ResolutionType), // Nhãn tiếng Việt của loại giải quyết.
                LoggedAt = l.LoggedAt, // Ngày thực hiện sửa chữa ghi nhận.
                Summary = l.Summary, // Mô tả chi tiết lỗi phát hiện và nội dung sửa chữa đã thực hiện lần đó.
                ReplacedBySerialId = l.ReplacedBySerialId, // Id thiết bị mới thay thế nếu lần đó đi theo luồng đổi máy.
                ReplacedBySerialNumber = l.ReplacedBySerial?.SerialNumber // Mã số Serial của thiết bị thay thế mới.
            }).ToList(); // Đóng gói và trả về danh sách lịch sử.
        }

        private static void CheckAssignment(ServiceTicket ticket, Guid userId, bool isAdmin) // Định nghĩa phương thức tĩnh nội bộ kiểm tra quyền sửa chữa của kỹ thuật viên.
        {
            if (!isAdmin && ticket.AssignedEmployeeId.HasValue && ticket.AssignedEmployeeId != userId) // Nếu không phải quản trị viên và phiếu đã được giao cho kỹ thuật viên khác xử lý.
                throw new UnauthorizedAccessException("Bạn không phải kỹ thuật viên được giao phó cho phiếu này."); // Từ chối quyền truy cập thao tác trên phiếu của người khác.
        }

        private static void CheckAssignmentOrCustomer(ServiceTicket ticket, Guid userId, bool isAdmin) // Định nghĩa phương thức tĩnh kiểm tra quyền truy cập phiếu (cho phép khách sở hữu hoặc kỹ thuật viên phụ trách).
        {
            if (isAdmin) return; // Nếu là quản trị viên hệ thống thì bỏ qua kiểm tra, cho phép thao tác.
            if (ticket.CustomerId.HasValue && ticket.CustomerId == userId) return; // Nếu người thực hiện chính là khách hàng sở hữu phiếu dịch vụ này thì hợp lệ, bỏ qua kiểm tra.
            if (ticket.AssignedEmployeeId.HasValue && ticket.AssignedEmployeeId != userId) // Nếu phiếu giao cho nhân viên khác phụ trách và người thao tác không phải khách mua.
                throw new UnauthorizedAccessException("Bạn không phải kỹ thuật viên được giao phó cho phiếu này."); // Báo lỗi không có quyền truy cập.
        }

        private void ValidateTransition(byte currentStatus, byte targetStatus) // Định nghĩa phương thức kiểm tra tính hợp lệ của luồng chuyển trạng thái phiếu (State Machine).
        {
            var validTransitions = new[] // Khai báo danh sách các cặp chuyển đổi trạng thái hợp lệ trong hệ thống.
            {
                (0, 1), (0, 10), // Từ Received (0) sang Diagnosing (1) hoặc Cancelled (10).
                (1, 2), (1, 4), (1, 5), (1, 6), (1, 8), (1, 10), // Từ Diagnosing (1) sang SentQuotation (2), WaitingParts (4), Repairing (5), RmaSent (6), Swapped (8) hoặc Cancelled (10).
                (2, 4), (2, 5), (2, 3), (2, 10), // Từ SentQuotation (2) sang WaitingParts (4), Repairing (5), RejectedQuotation (3) hoặc Cancelled (10).
                (4, 5), (4, 10), // Từ WaitingParts (4) sang Repairing (5) hoặc Cancelled (10).
                (5, 4), (5, 9), (5, 10), // Từ Repairing (5) sang WaitingParts (4), Completed (9) hoặc Cancelled (10).
                (6, 1), (6, 7), (6, 10), // Từ RmaSent (6) sang Diagnosing (1) [bị từ chối bảo hành hãng], RmaReceived (7) hoặc Cancelled (10).
                (7, 9), (7, 8), (7, 10), // Từ RmaReceived (7) sang Completed (9), Swapped (8) hoặc Cancelled (10).
                (8, 9), (8, 10) // Từ Swapped (8) sang Completed (9) hoặc Cancelled (10).
            }; // Kết thúc danh sách chuyển đổi.

            if (!validTransitions.Contains((currentStatus, targetStatus))) // Nếu cặp chuyển đổi trạng thái hiện tại và đích không nằm trong danh sách hợp lệ.
                throw new InvalidOperationException($"Không thể chuyển trạng thái từ '{GetStatusLabel(currentStatus)}' sang '{GetStatusLabel(targetStatus)}'."); // Ngăn chặn chuyển đổi sai quy trình nghiệp vụ.
        }

        private ServiceTicketDetailDto MapToDetailDto(ServiceTicket ticket) // Định nghĩa phương thức nội bộ ánh xạ thực thể phiếu sang DTO chi tiết đầy đủ quan hệ.
        {
            return new ServiceTicketDetailDto // Khởi tạo DTO chi tiết phiếu sửa chữa.
            {
                Id = ticket.Id, // Ánh xạ Id.
                TicketCode = ticket.TicketCode, // Ánh xạ mã phiếu dịch vụ.
                SerialNumber = ticket.Serial.SerialNumber, // Ánh xạ số Serial thiết bị.
                ProductName = ticket.Serial.Variant.Product.Name, // Ánh xạ tên sản phẩm cha.
                SerialVariantId = ticket.Serial.VariantId, // Ánh xạ Id biến thể thiết bị.
                IntakeDate = ticket.IntakeDate, // Ánh xạ ngày tiếp nhận máy.
                Status = ticket.Status, // Ánh xạ mã trạng thái hiện tại.
                StatusLabel = GetStatusLabel(ticket.Status), // Ánh xạ nhãn hiển thị trạng thái tiếng Việt.
                ResolutionType = ticket.ResolutionType, // Ánh xạ loại hướng giải quyết.
                CustomerName = ticket.Customer?.Profile?.FullName ?? ticket.WalkInCustomerName, // Ánh xạ tên khách hàng liên kết hoặc khách vãng lai.
                HasScratches = ticket.HasScratches, // Ánh xạ snapshot trầy xước.
                HasDents = ticket.HasDents, // Ánh xạ snapshot vết móp.
                HasBurnMarks = ticket.HasBurnMarks, // Ánh xạ snapshot vết cháy nổ.
                HasMissingAccessories = ticket.HasMissingAccessories, // Ánh xạ snapshot thiếu phụ kiện.
                CosmeticNotes = ticket.CosmeticNotes, // Ánh xạ chi tiết ngoại quan.
                WasInWarrantyAtIntake = ticket.WasInWarrantyAtIntake, // Ánh xạ trạng thái bảo hành đầu vào.
                WarrantyEndDateAtIntake = ticket.WarrantyEndDateAtIntake, // Ánh xạ ngày hết hạn bảo hành lúc nhận máy.
                CustomerReportedIssue = ticket.CustomerReportedIssue, // Ánh xạ lỗi khách báo.
                DiagnosisFindings = ticket.DiagnosisFindings, // Ánh xạ phát hiện chẩn đoán lỗi.
                DiagnosedAt = ticket.DiagnosedAt, // Ánh xạ ngày chẩn đoán.
                AssignedEmployeeId = ticket.AssignedEmployeeId, // Ánh xạ Id kỹ thuật viên phụ trách sửa.
                StatusHistory = ticket.StatusHistory?.OrderByDescending(h => h.ChangedAt) // Ánh xạ danh sách lịch sử chuyển trạng thái đã sắp xếp thời gian giảm dần.
                    .Select(h => new ServiceTicketStatusHistoryDto // Ánh xạ từng bản ghi.
                    {
                        Id = h.Id, // Id lịch sử.
                        FromStatus = h.FromStatus, // Trạng thái xuất phát.
                        FromStatusLabel = GetStatusLabel(h.FromStatus), // Nhãn trạng thái xuất phát.
                        ToStatus = h.ToStatus, // Trạng thái đích.
                        ToStatusLabel = GetStatusLabel(h.ToStatus), // Nhãn trạng thái đích.
                        ChangedAt = h.ChangedAt, // Thời điểm chuyển.
                        Note = h.Note // Ghi chú chuyển đổi.
                    }).ToList() ?? new List<ServiceTicketStatusHistoryDto>(), // Khởi tạo danh sách mới nếu rỗng.
                Quotations = ticket.Quotations? // Ánh xạ danh sách báo giá liên kết.
                    .OrderByDescending(q => q.IssuedDate) // Sắp xếp theo ngày phát hành báo giá giảm dần.
                    .Select(q => MapQuotationToDto(q)) // Ánh xạ từng báo giá thực thể sang DTO.
                    .ToList() ?? new(), // Khởi tạo danh sách mới nếu rỗng.
                RmaShipment = ticket.RmaShipment != null ? MapRmaShipmentToDto(ticket.RmaShipment) : null, // Ánh xạ thông tin lô hàng gửi bảo hành hãng RMA (nếu có).
                Invoice = ticket.Invoice != null ? MapServiceInvoiceToDto(ticket.Invoice) : null // Ánh xạ hóa đơn thanh toán dịch vụ (nếu có).
            }; // Kết thúc khởi tạo DTO.
        }

        private string GetStatusLabel(byte status) => status switch // Định nghĩa hàm chuyển đổi mã trạng thái phiếu sang chuỗi nhãn tiếng Việt hiển thị giao diện.
        {
            0 => "Đã tiếp nhận", // Nhãn trạng thái 0.
            1 => "Đang chẩn đoán", // Nhãn trạng thái 1.
            2 => "Đã gửi báo giá", // Nhãn trạng thái 2.
            3 => "Khách từ chối báo giá", // Nhãn trạng thái 3.
            4 => "Chờ phụ tùng", // Nhãn trạng thái 4.
            5 => "Đang sửa chữa", // Nhãn trạng thái 5.
            6 => "Đã gửi hãng (RMA)", // Nhãn trạng thái 6.
            7 => "Đã nhận lại từ hãng", // Nhãn trạng thái 7.
            8 => "Đã đổi 1-1", // Nhãn trạng thái 8.
            9 => "Hoàn tất", // Nhãn trạng thái 9.
            10 => "Đã hủy", // Nhãn trạng thái 10.
            _ => "Không xác định" // Nhãn dự phòng.
        }; // Kết thúc switch.

        private string GetResolutionLabel(byte type) => type switch // Định nghĩa hàm chuyển đổi mã loại hướng giải quyết sang chuỗi tiếng Việt.
        {
            1 => "Sửa nội bộ", // Loại giải quyết 1.
            2 => "RMA", // Loại giải quyết 2.
            3 => "Đổi 1-1", // Loại giải quyết 3.
            4 => "Sửa tính phí", // Loại giải quyết 4.
            _ => "Chờ xác định" // Loại dự phòng.
        }; // Kết thúc switch.

        private string GetWarrantySourceLabel(byte source) => source switch // Định nghĩa hàm chuyển đổi mã nguồn xác định bảo hành sang chuỗi nhãn.
        {
            0 => "WarrantyRow", // Nguồn 0.
            1 => "ComputedFromSoldDate", // Nguồn 1.
            2 => "NoWarranty", // Nguồn 2.
            _ => "Unknown" // Dự phòng.
        }; // Kết thúc switch.

        private QuotationDetailDto MapQuotationToDto(Quotation quotation) => new() // Định nghĩa phương thức ánh xạ thực thể báo giá sang DTO chi tiết.
        {
            Id = quotation.Id, // Ánh xạ Id báo giá.
            TicketId = quotation.TicketId, // Ánh xạ Id phiếu sửa chữa liên quan.
            IssuedDate = quotation.IssuedDate, // Ánh xạ ngày lập báo giá.
            LaborCost = quotation.LaborCost, // Ánh xạ chi phí tiền công sửa chữa.
            PartsTotal = quotation.PartsTotal, // Ánh xạ tổng tiền linh kiện.
            GrandTotal = quotation.GrandTotal, // Ánh xạ tổng tiền thanh toán dự tính.
            Status = quotation.Status, // Ánh xạ mã trạng thái báo giá.
            StatusLabel = GetQuotationStatusLabel(quotation.Status), // Ánh xạ nhãn trạng thái tiếng Việt (ví dụ: "Chờ duyệt").
            CustomerDecidedAt = quotation.CustomerDecidedAt, // Ánh xạ ngày khách phê duyệt/từ chối.
            CustomerDecisionNote = quotation.CustomerDecisionNote, // Ánh xạ lý do khách từ chối.
            Items = quotation.Items?.Select(i => new QuotationItemDto // Ánh xạ danh sách các khoản mục chi tiết của báo giá.
            {
                Id = i.Id, // Id khoản mục.
                Description = i.Description, // Mô tả linh kiện/dịch vụ sửa chữa.
                Quantity = i.Quantity, // Số lượng thay thế.
                UnitPrice = i.UnitPrice, // Đơn giá.
                LineTotal = i.LineTotal // Tổng tiền khoản mục.
            }).ToList() ?? new() // Khởi tạo danh sách mới nếu rỗng.
        }; // Kết thúc.

        private RmaShipmentDetailDto MapRmaShipmentToDto(RmaShipment rma) => new() // Định nghĩa phương thức ánh xạ thực thể RMA gửi hãng sang DTO chi tiết.
        {
            Id = rma.Id, // Ánh xạ Id.
            TicketId = rma.TicketId, // Ánh xạ Id phiếu dịch vụ.
            CarrierName = rma.CarrierName, // Ánh xạ tên đơn vị vận chuyển hàng lên hãng.
            TrackingCode = rma.TrackingCode, // Ánh xạ mã vận đơn vận chuyển.
            ShippedDate = rma.ShippedDate, // Ánh xạ ngày bàn giao gửi hàng cho đơn vị vận chuyển.
            ReceivedBackDate = rma.ReceivedBackDate, // Ánh xạ ngày nhận lại hàng từ hãng về cửa hàng.
            ManufacturerResolution = rma.ManufacturerResolution, // Ánh xạ mã kết quả giải quyết của hãng.
            ManufacturerResolutionLabel = GetManufacturerResolutionLabel(rma.ManufacturerResolution), // Ánh xạ nhãn kết quả tiếng Việt của hãng (ví dụ: "Đã thay thế").
            ManufacturerNotes = rma.ManufacturerNotes // Ánh xạ ghi chú từ hãng sản xuất.
        }; // Kết thúc.

        private ServiceInvoiceDetailDto MapServiceInvoiceToDto(ServiceInvoice invoice) => new() // Định nghĩa phương thức ánh xạ thực thể hóa đơn dịch vụ sang DTO chi tiết.
        {
            Id = invoice.Id, // Ánh xạ Id hóa đơn.
            InvoiceCode = invoice.InvoiceCode, // Ánh xạ mã hóa đơn dịch vụ tự sinh.
            TicketId = invoice.TicketId, // Ánh xạ Id phiếu sửa chữa liên quan.
            QuotationId = invoice.QuotationId, // Ánh xạ Id báo giá được duyệt.
            IssuedByEmployeeId = invoice.IssuedByEmployeeId, // Ánh xạ Id nhân viên lập hóa đơn thanh toán.
            IssuedDate = invoice.IssuedDate, // Ánh xạ ngày xuất hóa đơn.
            LaborCost = invoice.LaborCost, // Ánh xạ tiền công sửa chữa.
            PartsTotal = invoice.PartsTotal, // Ánh xạ tổng tiền linh kiện.
            GrandTotal = invoice.GrandTotal, // Ánh xạ tổng tiền thanh toán hóa đơn.
            PaymentMethod = invoice.PaymentMethod, // Ánh xạ mã phương thức thanh toán.
            PaymentMethodLabel = GetPaymentMethodLabel(invoice.PaymentMethod), // Ánh xạ nhãn phương thức tiếng Việt (ví dụ: "Tiền mặt").
            PaymentStatus = invoice.PaymentStatus, // Ánh xạ mã trạng thái thanh toán.
            PaymentStatusLabel = GetPaymentStatusLabel(invoice.PaymentStatus), // Ánh xạ nhãn trạng thái tiếng Việt (ví dụ: "Đã thanh toán").
            Note = invoice.Note, // Ánh xạ ghi chú hóa đơn.
            Items = invoice.Items?.Select(i => new ServiceInvoiceItemDto // Ánh xạ danh sách các khoản mục chi tiết của hóa đơn dịch vụ.
            {
                Id = i.Id, // Id dòng chi tiết.
                Description = i.Description, // Mô tả linh kiện thay thế hoặc dịch vụ kỹ thuật.
                Quantity = i.Quantity, // Số lượng thay thế thực tế.
                UnitPrice = i.UnitPrice, // Đơn giá linh kiện/dịch vụ.
                LineTotal = i.LineTotal // Tổng tiền của dòng chi tiết này.
            }).ToList() ?? new() // Khởi tạo danh sách mới nếu rỗng.
        }; // Kết thúc.

        private string GetQuotationStatusLabel(byte status) => status switch // Định nghĩa hàm chuyển đổi mã trạng thái báo giá sang nhãn hiển thị tiếng Việt.
        {
            0 => "Chờ duyệt", // Mã trạng thái 0.
            1 => "Đã duyệt", // Mã trạng thái 1.
            2 => "Từ chối", // Mã trạng thái 2.
            3 => "Thay thế", // Mã trạng thái 3.
            _ => "Không xác định" // Mã dự phòng.
        }; // Kết thúc.

        private string GetManufacturerResolutionLabel(byte resolution) => resolution switch // Định nghĩa hàm chuyển kết quả giải quyết của hãng sang tiếng Việt.
        {
            0 => "Chưa xác định", // Mã kết quả 0.
            1 => "Đã sửa", // Mã kết quả 1.
            2 => "Đã thay thế", // Mã kết quả 2.
            3 => "Từ chối", // Mã kết quả 3.
            _ => "Không xác định" // Dự phòng.
        }; // Kết thúc.

        private string GetPaymentMethodLabel(byte method) => method switch // Định nghĩa hàm chuyển đổi mã phương thức thanh toán sang nhãn hiển thị tiếng Việt.
        {
            0 => "Tiền mặt", // Phương thức 0.
            1 => "Chuyển khoản", // Phương thức 1.
            2 => "VNPay", // Phương thức 2.
            _ => "Không xác định" // Dự phòng.
        }; // Kết thúc.

        private string GetPaymentStatusLabel(byte status) => status switch // Định nghĩa hàm chuyển đổi mã trạng thái thanh toán hóa đơn sang nhãn tiếng Việt.
        {
            0 => "Chưa thanh toán", // Trạng thái 0.
            1 => "Đã thanh toán", // Trạng thái 1.
            _ => "Không xác định" // Dự phòng.
        }; // Kết thúc.
    }
}

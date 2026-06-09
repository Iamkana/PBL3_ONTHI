using OfficeOpenXml; // Sử dụng thư viện EPPlus để thao tác với file Excel.
using OfficeOpenXml.Style; // Sử dụng các kiểu Style để định dạng dữ liệu trong bảng tính Excel.
using PBL3.Shared.DTOs.BuildPc; // Sử dụng các DTO cho cấu hình Build PC.
using System.Drawing; // Sử dụng System.Drawing để thao tác với màu sắc trong Excel.

namespace PBL3.Application.BuildPc // Khai báo namespace cho module Build PC.
{
    public class BuildPcService : IBuildPcService // Lớp BuildPcService triển khai từ giao diện IBuildPcService.
    {
        public Task<byte[]> ExportToExcelAsync(ExportBuildPcRequest request) // Phương thức xuất cấu hình PC ra file Excel dạng mảng byte.
        {
            ExcelPackage.License.SetNonCommercialPersonal("dangbathinh0901@gmail.com"); // Cấu hình bản quyền phi thương mại cho EPPlus.

            using var package = new ExcelPackage(); // Khởi tạo đối tượng ExcelPackage mới trong khối using.
            var ws = package.Workbook.Worksheets.Add("Cấu hình PC"); // Tạo một bảng tính mới mang tên "Cấu hình PC".

            ws.Cells[1, 1, 1, 9].Merge = true; // Gộp các ô từ cột 1 đến 9 của hàng 1 làm ô tiêu đề chính.
            ws.Cells[1, 1].Value = "CẤU HÌNH PC - HushStore"; // Thiết lập giá trị text tiêu đề.
            ws.Cells[1, 1].Style.Font.Bold = true; // Đặt font chữ in đậm cho tiêu đề.
            ws.Cells[1, 1].Style.Font.Size = 16; // Đặt kích thước font chữ là 16.
            ws.Cells[1, 1].Style.Font.Color.SetColor(Color.White); // Đặt màu chữ tiêu đề là trắng.
            ws.Cells[1, 1].Style.Fill.PatternType = ExcelFillStyle.Solid; // Đặt kiểu tô màu nền dạng Solid.
            ws.Cells[1, 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(192, 57, 43)); // Đặt màu nền tiêu đề là màu đỏ thẫm.
            ws.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; // Căn lề ngang ở giữa cho tiêu đề.
            ws.Row(1).Height = 32; // Thiết lập chiều cao của hàng tiêu đề 1 là 32.

            ws.Cells[2, 1, 2, 9].Merge = true; // Gộp các ô từ cột 1 đến 9 của hàng 2 để ghi ngày xuất cấu hình.
            ws.Cells[2, 1].Value = $"Ngày xuất: {DateTime.Now:dd/MM/yyyy HH:mm}"; // Thiết lập thời gian xuất cấu hình.
            ws.Cells[2, 1].Style.Font.Italic = true; // Thiết lập font chữ in nghiêng.
            ws.Cells[2, 1].Style.Font.Color.SetColor(Color.FromArgb(127, 140, 141)); // Thiết lập màu chữ xám cho thời gian.
            ws.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; // Căn lề giữa cho thông tin ngày xuất.

            string[] headers = { "STT", "Linh kiện", "Sản phẩm", "Phiên bản", "SKU", "Bảo hành", "Đơn giá", "SL", "Thành tiền" }; // Định nghĩa mảng tiêu đề các cột dữ liệu.
            for (int i = 0; i < headers.Length; i++) // Vòng lặp duyệt qua từng tiêu đề cột.
            {
                var cell = ws.Cells[3, i + 1]; // Lấy ô tương ứng tại hàng 3.
                cell.Value = headers[i]; // Gán giá trị tiêu đề cột.
                cell.Style.Font.Bold = true; // Đặt font in đậm cho tiêu đề cột.
                cell.Style.Font.Color.SetColor(Color.White); // Đặt màu chữ là màu trắng.
                cell.Style.Fill.PatternType = ExcelFillStyle.Solid; // Đặt kiểu tô màu nền dạng Solid.
                cell.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(44, 62, 80)); // Đặt màu nền là xanh thẫm.
                cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; // Căn giữa tiêu đề cột.
                cell.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.White); // Thiết lập đường viền mảnh màu trắng xung quanh ô.
            }
            ws.Row(3).Height = 22; // Đặt chiều cao hàng tiêu đề cột 3 là 22.

            decimal total = 0; // Khởi tạo biến tích lũy tổng chi phí.
            int row = 4; // Bắt đầu ghi dữ liệu từ hàng thứ 4.
            foreach (var item in request.Items) // Duyệt qua danh sách các linh kiện được chọn trong PC.
            {
                decimal lineTotal = item.UnitPrice * item.Quantity; // Tính tổng tiền cho linh kiện hiện tại (Số lượng x Đơn giá).
                total += lineTotal; // Cộng dồn thành tiền vào tổng chi phí.

                bool isEven = (row % 2 == 0); // Kiểm tra xem chỉ số hàng hiện tại là chẵn hay lẻ.
                var rowColor = isEven ? Color.FromArgb(245, 245, 245) : Color.White; // Tạo hiệu ứng sọc hàng (hàng chẵn xám nhạt, hàng lẻ trắng).

                ws.Cells[row, 1].Value = item.SlotIndex; // Gán số thứ tự vị trí linh kiện (STT).
                ws.Cells[row, 2].Value = item.SlotName; // Gán tên loại linh kiện (Ví dụ: CPU, VGA).
                ws.Cells[row, 3].Value = item.ProductName; // Gán tên sản phẩm.
                ws.Cells[row, 4].Value = item.VariantName; // Gán tên biến thể cụ thể.
                ws.Cells[row, 5].Value = item.Sku; // Gán mã SKU sản phẩm.
                ws.Cells[row, 6].Value = item.WarrantyMonth > 0 ? $"{item.WarrantyMonth} tháng" : "—"; // Gán thông tin bảo hành (nếu có).
                ws.Cells[row, 7].Value = item.UnitPrice; // Gán đơn giá sản phẩm.
                ws.Cells[row, 7].Style.Numberformat.Format = "#,##0"; // Định dạng tiền tệ cho đơn giá.
                ws.Cells[row, 8].Value = item.Quantity; // Gán số lượng.
                ws.Cells[row, 9].Value = lineTotal; // Gán tổng tiền của dòng linh kiện.
                ws.Cells[row, 9].Style.Numberformat.Format = "#,##0"; // Định dạng tiền tệ cho thành tiền.

                var dataRange = ws.Cells[row, 1, row, 9]; // Chọn vùng dữ liệu của hàng hiện tại từ cột 1 đến 9.
                dataRange.Style.Fill.PatternType = ExcelFillStyle.Solid; // Đặt kiểu tô màu nền dạng Solid.
                dataRange.Style.Fill.BackgroundColor.SetColor(rowColor); // Đặt màu nền sọc hàng tương ứng.
                dataRange.Style.Border.BorderAround(ExcelBorderStyle.Hair, Color.FromArgb(189, 195, 199)); // Đặt đường viền siêu mảnh màu xám xung quanh dòng dữ liệu.

                ws.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; // Căn giữa cột STT.
                ws.Cells[row, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; // Căn giữa cột Số lượng.
                ws.Cells[row, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right; // Căn phải cột Đơn giá.
                ws.Cells[row, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right; // Căn phải cột Thành tiền.

                row++; // Tăng chỉ số hàng lên 1 để chuẩn bị ghi dòng tiếp theo.
            }

            ws.Cells[row, 1, row, 8].Merge = true; // Gộp các ô từ cột 1 đến 8 ở dòng cuối để ghi nhãn tổng cộng.
            ws.Cells[row, 1].Value = "Tổng chi phí dự tính"; // Thiết lập giá trị text tổng cộng.
            ws.Cells[row, 1].Style.Font.Bold = true; // Đặt font in đậm cho nhãn tổng cộng.
            ws.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right; // Căn phải cho nhãn tổng cộng.
            ws.Cells[row, 9].Value = total; // Gán tổng tiền tích lũy của toàn bộ cấu hình PC.
            ws.Cells[row, 9].Style.Numberformat.Format = "#,##0"; // Định dạng tiền tệ.
            ws.Cells[row, 9].Style.Font.Bold = true; // Đặt font in đậm cho tổng số tiền.
            ws.Cells[row, 9].Style.Font.Color.SetColor(Color.FromArgb(192, 57, 43)); // Đặt chữ màu đỏ nổi bật cho tổng chi phí.
            ws.Cells[row, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right; // Căn phải cho số tiền.
            var totalRange = ws.Cells[row, 1, row, 9]; // Chọn vùng dòng tổng cộng từ cột 1 đến 9.
            totalRange.Style.Fill.PatternType = ExcelFillStyle.Solid; // Đặt kiểu tô màu nền dạng Solid.
            totalRange.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(235, 237, 239)); // Đặt màu nền là xám nhạt làm nổi bật dòng tổng kết.
            totalRange.Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.FromArgb(44, 62, 80)); // Vẽ khung viền dày bao quanh dòng tổng kết.

            ws.Column(1).Width = 6; // Đặt chiều rộng cho cột 1 (STT).
            ws.Column(2).Width = 20; // Đặt chiều rộng cho cột 2 (Linh kiện).
            ws.Column(3).Width = 40; // Đặt chiều rộng cho cột 3 (Sản phẩm).
            ws.Column(4).Width = 25; // Đặt chiều rộng cho cột 4 (Phiên bản).
            ws.Column(5).Width = 14; // Đặt chiều rộng cho cột 5 (SKU).
            ws.Column(6).Width = 12; // Đặt chiều rộng cho cột 6 (Bảo hành).
            ws.Column(7).Width = 14; // Đặt chiều rộng cho cột 7 (Đơn giá).
            ws.Column(8).Width = 6; // Đặt chiều rộng cho cột 8 (Số lượng).
            ws.Column(9).Width = 16; // Đặt chiều rộng cho cột 9 (Thành tiền).

            return Task.FromResult(package.GetAsByteArray()); // Xuất toàn bộ file Excel dưới dạng mảng byte và trả về dưới dạng Task.
        }
    }
}

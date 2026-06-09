using MudBlazor; // Sử dụng thư viện thành phần giao diện MudBlazor.

namespace Client.Shared.Components.ServiceTickets; // Thiết lập namespace Client.Shared.Components.ServiceTickets để tổ chức quản lý cấu trúc các lớp.

public static class ServiceTicketStatusHelper // Định nghĩa lớp tĩnh ServiceTicketStatusHelper cung cấp các phương thức hỗ trợ dùng chung.
{
    public static string GetLabel(byte status) => status switch // Thực thi dòng lệnh nghiệp vụ.
    {
        0 => "Đã tiếp nhận", // Ánh xạ giá trị mã trạng thái 0 sang kết quả: "Đã tiếp nhận".
        1 => "Đang chẩn đoán", // Ánh xạ giá trị mã trạng thái 1 sang kết quả: "Đang chẩn đoán".
        2 => "Đã gửi báo giá", // Ánh xạ giá trị mã trạng thái 2 sang kết quả: "Đã gửi báo giá".
        3 => "Khách từ chối báo giá", // Ánh xạ giá trị mã trạng thái 3 sang kết quả: "Khách từ chối báo giá".
        4 => "Chờ phụ tùng", // Ánh xạ giá trị mã trạng thái 4 sang kết quả: "Chờ phụ tùng".
        5 => "Đang sửa chữa", // Ánh xạ giá trị mã trạng thái 5 sang kết quả: "Đang sửa chữa".
        6 => "Đã gửi hãng (RMA)", // Ánh xạ giá trị mã trạng thái 6 sang kết quả: "Đã gửi hãng (RMA)".
        7 => "Đã nhận lại từ hãng", // Ánh xạ giá trị mã trạng thái 7 sang kết quả: "Đã nhận lại từ hãng".
        8 => "Đã đổi 1-1", // Ánh xạ giá trị mã trạng thái 8 sang kết quả: "Đã đổi 1-1".
        9 => "Hoàn tất", // Ánh xạ giá trị mã trạng thái 9 sang kết quả: "Hoàn tất".
        10 => "Đã hủy", // Ánh xạ giá trị mã trạng thái 10 sang kết quả: "Đã hủy".
        _ => "Không xác định" // Trường hợp mặc định không khớp các mã trên, trả về: "Không xác định".
    };

    public static Color GetColor(byte status) => status switch // Thực thi dòng lệnh nghiệp vụ.
    {
        0 => Color.Info, // Ánh xạ giá trị mã trạng thái 0 sang kết quả: Color.Info.
        1 => Color.Warning, // Ánh xạ giá trị mã trạng thái 1 sang kết quả: Color.Warning.
        2 => Color.Dark, // Ánh xạ giá trị mã trạng thái 2 sang kết quả: Color.Dark.
        3 => Color.Error, // Ánh xạ giá trị mã trạng thái 3 sang kết quả: Color.Error.
        4 => Color.Dark, // Ánh xạ giá trị mã trạng thái 4 sang kết quả: Color.Dark.
        5 => Color.Warning, // Ánh xạ giá trị mã trạng thái 5 sang kết quả: Color.Warning.
        6 => Color.Dark, // Ánh xạ giá trị mã trạng thái 6 sang kết quả: Color.Dark.
        7 => Color.Info, // Ánh xạ giá trị mã trạng thái 7 sang kết quả: Color.Info.
        8 => Color.Success, // Ánh xạ giá trị mã trạng thái 8 sang kết quả: Color.Success.
        9 => Color.Success, // Ánh xạ giá trị mã trạng thái 9 sang kết quả: Color.Success.
        10 => Color.Error, // Ánh xạ giá trị mã trạng thái 10 sang kết quả: Color.Error.
        _ => Color.Default // Trường hợp mặc định không khớp các mã trên, trả về: Color.Default.
    };

    public static string GetResolutionLabel(byte resolutionType) => resolutionType switch // Thực thi dòng lệnh nghiệp vụ.
    {
        0 => "Chưa xác định", // Ánh xạ giá trị mã trạng thái 0 sang kết quả: "Chưa xác định".
        1 => "Sửa nội bộ", // Ánh xạ giá trị mã trạng thái 1 sang kết quả: "Sửa nội bộ".
        2 => "Gửi hãng (RMA)", // Ánh xạ giá trị mã trạng thái 2 sang kết quả: "Gửi hãng (RMA)".
        3 => "Đổi 1-1", // Ánh xạ giá trị mã trạng thái 3 sang kết quả: "Đổi 1-1".
        4 => "Sửa tính phí", // Ánh xạ giá trị mã trạng thái 4 sang kết quả: "Sửa tính phí".
        5 => "Từ chối", // Ánh xạ giá trị mã trạng thái 5 sang kết quả: "Từ chối".
        6 => "Hủy", // Ánh xạ giá trị mã trạng thái 6 sang kết quả: "Hủy".
        _ => "Không xác định" // Trường hợp mặc định không khớp các mã trên, trả về: "Không xác định".
    };

    public static string GetManufacturerResolutionLabel(byte resolution) => resolution switch // Thực thi dòng lệnh nghiệp vụ.
    {
        0 => "Chưa xác định", // Ánh xạ giá trị mã trạng thái 0 sang kết quả: "Chưa xác định".
        1 => "Đã sửa", // Ánh xạ giá trị mã trạng thái 1 sang kết quả: "Đã sửa".
        2 => "Đã thay thế", // Ánh xạ giá trị mã trạng thái 2 sang kết quả: "Đã thay thế".
        3 => "Từ chối", // Ánh xạ giá trị mã trạng thái 3 sang kết quả: "Từ chối".
        _ => "Không xác định" // Trường hợp mặc định không khớp các mã trên, trả về: "Không xác định".
    };

    public static string GetQuotationStatusLabel(byte status) => status switch // Thực thi dòng lệnh nghiệp vụ.
    {
        0 => "Chờ duyệt", // Ánh xạ giá trị mã trạng thái 0 sang kết quả: "Chờ duyệt".
        1 => "Đã duyệt", // Ánh xạ giá trị mã trạng thái 1 sang kết quả: "Đã duyệt".
        2 => "Từ chối", // Ánh xạ giá trị mã trạng thái 2 sang kết quả: "Từ chối".
        3 => "Thay thế", // Ánh xạ giá trị mã trạng thái 3 sang kết quả: "Thay thế".
        _ => "Không xác định" // Trường hợp mặc định không khớp các mã trên, trả về: "Không xác định".
    };

    public static string GetWarrantySourceLabel(byte source) => source switch // Thực thi dòng lệnh nghiệp vụ.
    {
        0 => "Bản ghi bảo hành", // Ánh xạ giá trị mã trạng thái 0 sang kết quả: "Bản ghi bảo hành".
        1 => "Tính toán từ ngày bán", // Ánh xạ giá trị mã trạng thái 1 sang kết quả: "Tính toán từ ngày bán".
        2 => "Không còn bảo hành", // Ánh xạ giá trị mã trạng thái 2 sang kết quả: "Không còn bảo hành".
        _ => "Không xác định" // Trường hợp mặc định không khớp các mã trên, trả về: "Không xác định".
    };
}

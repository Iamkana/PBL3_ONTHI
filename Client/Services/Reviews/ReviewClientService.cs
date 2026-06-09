using System.Net.Http.Json; // Sử dụng thư viện hỗ trợ gửi nhận dữ liệu JSON qua HTTP.
using PBL3.Shared.DTOs.Common; // Sử dụng các DTO của module Common thuộc tầng Shared.
using PBL3.Shared.DTOs.Reviews; // Sử dụng các DTO của module Reviews thuộc tầng Shared.

namespace Client.Services.Reviews // Thiết lập namespace Client.Services.Reviews để tổ chức quản lý cấu trúc các lớp.
{
    public class ReviewClientService : IReviewClientService // Định nghĩa lớp ReviewClientService triển khai các dịch vụ hoặc kế thừa từ IReviewClientService.
    {
        private readonly HttpClient _httpClient; // Khai báo đối tượng HttpClient nội bộ dùng để thực hiện cuộc gọi API.
        private const string BaseUrl = "api/reviews"; // Khai báo hằng số BaseUrl có giá trị là "api/reviews".

        public ReviewClientService(HttpClient httpClient) // Hàm khởi tạo (Constructor) của lớp ReviewClientService tiêm các phụ thuộc: httpClient.
        {
            _httpClient = httpClient; // Thực hiện gán giá trị của biểu thức 'httpClient' cho biến/thuộc tính '_httpClient'.
        }

        public async Task<ApiResult<PagedResult<ReviewDto>>?> GetReviewsAsync( // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'GetReviewsAsync' không tham số trả về kết quả kiểu Task<ApiResult<PagedResult<ReviewDto>>?>.
            int productId, int page, int pageSize) // Thực thi dòng lệnh nghiệp vụ.
        {
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                return await _httpClient.GetFromJsonAsync<ApiResult<PagedResult<ReviewDto>>>( // Gọi phương thức GET bất đồng bộ tới URL '' nhận kết quả kiểu ApiResult<PagedResult<ReviewDto>>.
                    $"{BaseUrl}?productId={productId}&page={page}&pageSize={pageSize}"); // Thực thi dòng lệnh nghiệp vụ.
            }
            catch // Thực thi dòng lệnh nghiệp vụ.
            {
                return ApiResult<PagedResult<ReviewDto>>.Fail("Không thể tải danh sách đánh giá."); // Trả về giá trị của biểu thức 'ApiResult<PagedResult<ReviewDto>>.Fail("Không thể tải danh sách đánh giá.")'.
            }
        }

        public async Task<ApiResult<ReviewDto>?> CreateAsync(CreateReviewRequest request) // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'CreateAsync' nhận tham số (request) trả về kết quả kiểu Task<ApiResult<ReviewDto>?>.
        {
            var response = await _httpClient.PostAsJsonAsync(BaseUrl, request); // Gọi phương thức POST (gửi dữ liệu JSON) bất đồng bộ tới URL 'BaseUrl' và gán kết quả cho biến 'response'.
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                return await response.Content.ReadFromJsonAsync<ApiResult<ReviewDto>>(); // Trả về giá trị của biểu thức 'await response.Content.ReadFromJsonAsync<ApiResult<ReviewDto>>()'.
            }
            catch // Thực thi dòng lệnh nghiệp vụ.
            {
                var body = await response.Content.ReadAsStringAsync(); // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'body'.
                var hint = body.Length > 0 ? body[..Math.Min(body.Length, 300)] : $"HTTP {(int)response.StatusCode}"; // Thực hiện gán giá trị của biểu thức 'body.Length > 0 ? body[..Math.Min(body.Length, 300)] : $"HTTP {(int)response.StatusCode}"' cho biến/thuộc tính 'hint'.
                return ApiResult<ReviewDto>.Fail(hint); // Trả về giá trị của biểu thức 'ApiResult<ReviewDto>.Fail(hint)'.
            }
        }

        public async Task<ApiResult<bool>?> DeleteAsync(int reviewId) // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'DeleteAsync' nhận tham số (reviewId) trả về kết quả kiểu Task<ApiResult<bool>?>.
        {
            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var response = await _httpClient.DeleteAsync($"{BaseUrl}/{reviewId}"); // Gọi phương thức DELETE (xóa tài nguyên) bất đồng bộ tới URL '{BaseUrl}/{reviewId}' và gán kết quả cho biến 'response'.
                return await response.Content.ReadFromJsonAsync<ApiResult<bool>>(); // Trả về giá trị của biểu thức 'await response.Content.ReadFromJsonAsync<ApiResult<bool>>()'.
            }
            catch // Thực thi dòng lệnh nghiệp vụ.
            {
                return ApiResult<bool>.Fail("Không thể xóa đánh giá."); // Trả về giá trị của biểu thức 'ApiResult<bool>.Fail("Không thể xóa đánh giá.")'.
            }
        }
    }
}

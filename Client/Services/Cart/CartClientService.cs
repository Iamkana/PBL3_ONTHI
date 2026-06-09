using System.Net.Http.Json; // Sử dụng thư viện hỗ trợ gửi nhận dữ liệu JSON qua HTTP.
using PBL3.Shared.DTOs.Cart; // Sử dụng các DTO của module Cart thuộc tầng Shared.
using PBL3.Shared.DTOs.Common; // Sử dụng các DTO của module Common thuộc tầng Shared.

namespace Client.Services.Cart // Thiết lập namespace Client.Services.Cart để tổ chức quản lý cấu trúc các lớp.
{
    public class CartClientService(IHttpClientFactory httpClientFactory) : ICartClientService // Định nghĩa lớp nghiệp vụ CartClientService.
    {
        private readonly IHttpClientFactory _httpClientFactory = httpClientFactory; // Thực hiện gán giá trị của biểu thức 'httpClientFactory' cho biến/thuộc tính 'private readonly IHttpClientFactory _httpClientFactory'.

        public async Task<ApiResult<bool>> AddToCartAsync(int variantId, int quantity) // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'AddToCartAsync' nhận tham số (variantId, quantity) trả về kết quả kiểu Task<ApiResult<bool>>.
        {
            var client = _httpClientFactory.CreateClient("HushStoreAPI"); // Thực hiện gán giá trị của biểu thức '_httpClientFactory.CreateClient("HushStoreAPI")' cho biến/thuộc tính 'client'.
            var request = new AddToCartRequest { VariantId = variantId, Quantity = quantity }; // Thực hiện gán giá trị của biểu thức 'new AddToCartRequest { VariantId = variantId, Quantity = quantity }' cho biến/thuộc tính 'request'.
            var response = await client.PostAsJsonAsync("/api/cart", request); // Gọi phương thức POST (gửi dữ liệu JSON) bất đồng bộ tới URL '/api/cart' và gán kết quả cho biến 'response'.

            try // Bắt đầu khối giám sát lỗi (try) khi thực thi thao tác.
            {
                var result = await response.Content.ReadFromJsonAsync<ApiResult<CartResponse>>(); // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'result'.
                return new ApiResult<bool> // Trả về giá trị của biểu thức 'new ApiResult<bool>'.
                {
                    Success = result?.Success ?? response.IsSuccessStatusCode, // Thực hiện gán giá trị của biểu thức 'result?.Success ?? response.IsSuccessStatusCode,' cho biến/thuộc tính 'Success'.
                    Message = result?.Message, // Thực hiện gán giá trị của biểu thức 'result?.Message,' cho biến/thuộc tính 'Message'.
                    Data    = response.IsSuccessStatusCode // Thực hiện gán giá trị của biểu thức 'response.IsSuccessStatusCode' cho biến/thuộc tính 'Data'.
                };
            }
            catch // Thực thi dòng lệnh nghiệp vụ.
            {
                return new ApiResult<bool> // Trả về giá trị của biểu thức 'new ApiResult<bool>'.
                {
                    Success = false, // Thực hiện gán giá trị của biểu thức 'false,' cho biến/thuộc tính 'Success'.
                    Message = "Không thể thêm vào giỏ hàng. Vui lòng thử lại." // Thực hiện gán giá trị của biểu thức '"Không thể thêm vào giỏ hàng. Vui lòng thử lại."' cho biến/thuộc tính 'Message'.
                };
            }
        }

        public async Task<ApiResult<CartResponse>> GetMyCartAsync() // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'GetMyCartAsync' không tham số trả về kết quả kiểu Task<ApiResult<CartResponse>>.
        {
            var client = _httpClientFactory.CreateClient("HushStoreAPI"); // Thực hiện gán giá trị của biểu thức '_httpClientFactory.CreateClient("HushStoreAPI")' cho biến/thuộc tính 'client'.
            var response = await client.GetAsync("/api/cart"); // Chờ tác vụ bất đồng bộ hoàn thành và gán giá trị trả về cho biến 'response'.
            
            if (response.IsSuccessStatusCode) // Kiểm tra xem điều kiện 'response.IsSuccessStatusCode' có thỏa mãn hay không.
            {
                return await response.Content.ReadFromJsonAsync<ApiResult<CartResponse>>() // Trả về giá trị của biểu thức 'await response.Content.ReadFromJsonAsync<ApiResult<CartResponse>>()'.
                       ?? new ApiResult<CartResponse> { Success = false, Message = "Lỗi dữ liệu trả về" }; // Thực hiện gán giá trị của biểu thức 'false, Message = "Lỗi dữ liệu trả về" }' cho biến/thuộc tính '?? new ApiResult<CartResponse> { Success'.
            }
            
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized) // Kiểm tra xem điều kiện 'response.StatusCode == System.Net.HttpStatusCode.Unauthorized' có thỏa mãn hay không.
            {
                return new ApiResult<CartResponse> { Success = false, Message = "Unauthorized" }; // Trả về giá trị của biểu thức 'new ApiResult<CartResponse> { Success = false, Message = "Unauthorized" }'.
            }

            return new ApiResult<CartResponse> { Success = false, Message = "Không thể lấy thông tin giỏ hàng" }; // Trả về giá trị của biểu thức 'new ApiResult<CartResponse> { Success = false, Message = "Không thể lấy thông tin giỏ hàng" }'.
        }

        public async Task<ApiResult<CartResponse>> UpdateQuantityAsync(int cartItemId, int quantity) // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'UpdateQuantityAsync' nhận tham số (cartItemId, quantity) trả về kết quả kiểu Task<ApiResult<CartResponse>>.
        {
            var client = _httpClientFactory.CreateClient("HushStoreAPI"); // Thực hiện gán giá trị của biểu thức '_httpClientFactory.CreateClient("HushStoreAPI")' cho biến/thuộc tính 'client'.
            var request = new UpdateCartItemRequest { Quantity = quantity }; // Thực hiện gán giá trị của biểu thức 'new UpdateCartItemRequest { Quantity = quantity }' cho biến/thuộc tính 'request'.
            var response = await client.PutAsJsonAsync($"/api/cart/items/{cartItemId}", request); // Gọi phương thức PUT (cập nhật dữ liệu JSON) bất đồng bộ tới URL '/api/cart/items/{cartItemId}' và gán kết quả cho biến 'response'.
            
            if (response.IsSuccessStatusCode) // Kiểm tra xem điều kiện 'response.IsSuccessStatusCode' có thỏa mãn hay không.
            {
                return await response.Content.ReadFromJsonAsync<ApiResult<CartResponse>>() // Trả về giá trị của biểu thức 'await response.Content.ReadFromJsonAsync<ApiResult<CartResponse>>()'.
                       ?? new ApiResult<CartResponse> { Success = false, Message = "Lỗi dữ liệu trả về" }; // Thực hiện gán giá trị của biểu thức 'false, Message = "Lỗi dữ liệu trả về" }' cho biến/thuộc tính '?? new ApiResult<CartResponse> { Success'.
            }
            
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized) // Kiểm tra xem điều kiện 'response.StatusCode == System.Net.HttpStatusCode.Unauthorized' có thỏa mãn hay không.
            {
                return new ApiResult<CartResponse> { Success = false, Message = "Unauthorized" }; // Trả về giá trị của biểu thức 'new ApiResult<CartResponse> { Success = false, Message = "Unauthorized" }'.
            }

            return new ApiResult<CartResponse> { Success = false, Message = "Không thể cập nhật số lượng" }; // Trả về giá trị của biểu thức 'new ApiResult<CartResponse> { Success = false, Message = "Không thể cập nhật số lượng" }'.
        }

        public async Task<ApiResult<CartResponse>> RemoveItemAsync(int cartItemId) // Thực hiện xử lý bất đồng bộ phương thức nghiệp vụ 'RemoveItemAsync' nhận tham số (cartItemId) trả về kết quả kiểu Task<ApiResult<CartResponse>>.
        {
            var client = _httpClientFactory.CreateClient("HushStoreAPI"); // Thực hiện gán giá trị của biểu thức '_httpClientFactory.CreateClient("HushStoreAPI")' cho biến/thuộc tính 'client'.
            var response = await client.DeleteAsync($"/api/cart/items/{cartItemId}"); // Gọi phương thức DELETE (xóa tài nguyên) bất đồng bộ tới URL '/api/cart/items/{cartItemId}' và gán kết quả cho biến 'response'.
            
            if (response.IsSuccessStatusCode) // Kiểm tra xem điều kiện 'response.IsSuccessStatusCode' có thỏa mãn hay không.
            {
                return await response.Content.ReadFromJsonAsync<ApiResult<CartResponse>>() // Trả về giá trị của biểu thức 'await response.Content.ReadFromJsonAsync<ApiResult<CartResponse>>()'.
                       ?? new ApiResult<CartResponse> { Success = false, Message = "Lỗi dữ liệu trả về" }; // Thực hiện gán giá trị của biểu thức 'false, Message = "Lỗi dữ liệu trả về" }' cho biến/thuộc tính '?? new ApiResult<CartResponse> { Success'.
            }
            
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized) // Kiểm tra xem điều kiện 'response.StatusCode == System.Net.HttpStatusCode.Unauthorized' có thỏa mãn hay không.
            {
                return new ApiResult<CartResponse> { Success = false, Message = "Unauthorized" }; // Trả về giá trị của biểu thức 'new ApiResult<CartResponse> { Success = false, Message = "Unauthorized" }'.
            }

            return new ApiResult<CartResponse> { Success = false, Message = "Không thể xóa sản phẩm khỏi giỏ hàng" }; // Trả về giá trị của biểu thức 'new ApiResult<CartResponse> { Success = false, Message = "Không thể xóa sản phẩm khỏi giỏ hàng" }'.
        }
    }
}

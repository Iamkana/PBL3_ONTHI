using PBL3.Shared.DTOs.Common; // Sử dụng các DTO của module Common thuộc tầng Shared.
using PBL3.Shared.DTOs.Reviews; // Sử dụng các DTO của module Reviews thuộc tầng Shared.

namespace Client.Services.Reviews; // Thiết lập namespace Client.Services.Reviews để tổ chức quản lý cấu trúc các lớp.

public interface IReviewClientService // Định nghĩa giao diện (interface) IReviewClientService quy định các hàm tương tác của client.
{
    Task<ApiResult<PagedResult<ReviewDto>>?> GetReviewsAsync(int productId, int page, int pageSize); // Khai báo phương thức giao diện 'GetReviewsAsync' với tham số (productId, page, pageSize) có kết quả trả về kiểu Task<ApiResult<PagedResult<ReviewDto>>?>.
    Task<ApiResult<ReviewDto>?> CreateAsync(CreateReviewRequest request); // Khai báo phương thức giao diện 'CreateAsync' với tham số (request) có kết quả trả về kiểu Task<ApiResult<ReviewDto>?>.
    Task<ApiResult<bool>?> DeleteAsync(int reviewId); // Khai báo phương thức giao diện 'DeleteAsync' với tham số (reviewId) có kết quả trả về kiểu Task<ApiResult<bool>?>.
}

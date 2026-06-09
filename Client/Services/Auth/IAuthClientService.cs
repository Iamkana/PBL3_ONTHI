using PBL3.Shared.DTOs.Auth; // Sử dụng các DTO của module Auth thuộc tầng Shared.
using PBL3.Shared.DTOs.Common; // Sử dụng các DTO của module Common thuộc tầng Shared.
using PBL3.Shared.DTOs.Customers; // Sử dụng các DTO của module Customers thuộc tầng Shared.

namespace Client.Services.Auth; // Thiết lập namespace Client.Services.Auth để tổ chức quản lý cấu trúc các lớp.

public interface IAuthClientService // Định nghĩa giao diện (interface) IAuthClientService quy định các hàm tương tác của client.
{
    Task<ApiResult<TokenResponse>> LoginAsync(LoginRequest request); // Khai báo phương thức giao diện 'LoginAsync' với tham số (request) có kết quả trả về kiểu Task<ApiResult<TokenResponse>>.
    Task LogoutAsync(); // Khai báo phương thức giao diện 'LogoutAsync' không tham số có kết quả trả về kiểu Task.
    Task<ApiResult<bool>> RegisterAsync(RegisterCustomerRequest request); // Khai báo phương thức giao diện 'RegisterAsync' với tham số (request) có kết quả trả về kiểu Task<ApiResult<bool>>.
    Task<ApiResult<bool>> ChangePasswordAsync(ChangePasswordRequest request); // Khai báo phương thức giao diện 'ChangePasswordAsync' với tham số (request) có kết quả trả về kiểu Task<ApiResult<bool>>.
    Task RefreshSessionAsync(); // Khai báo phương thức giao diện 'RefreshSessionAsync' không tham số có kết quả trả về kiểu Task.
}

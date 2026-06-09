using System.Collections.Generic; // Nhập (import) namespace System.Collections.Generic để sử dụng các lớp bên trong.
using System.Threading.Tasks; // Nhập (import) namespace System.Threading.Tasks để sử dụng các lớp bên trong.
using PBL3.Shared.DTOs.Common; // Sử dụng các DTO của module Common thuộc tầng Shared.
using PBL3.Shared.DTOs.Customers; // Sử dụng các DTO của module Customers thuộc tầng Shared.

namespace Client.Services.Customer; // Thiết lập namespace Client.Services.Customer để tổ chức quản lý cấu trúc các lớp.

public interface IUserAddressClientService // Định nghĩa giao diện (interface) IUserAddressClientService quy định các hàm tương tác của client.
{
    Task<ApiResult<List<UserAddressDto>>> GetMyAddressesAsync(); // Khai báo phương thức giao diện 'GetMyAddressesAsync' không tham số có kết quả trả về kiểu Task<ApiResult<List<UserAddressDto>>>.
    Task<ApiResult<int>> AddAddressAsync(UserAddressDto request); // Khai báo phương thức giao diện 'AddAddressAsync' với tham số (request) có kết quả trả về kiểu Task<ApiResult<int>>.
}

using System; // Nhập thư viện hệ thống cơ bản.

namespace PBL3.Shared.DTOs.Customers // Định nghĩa namespace PBL3.Shared.DTOs.Customers quản lý cấu trúc code truyền tải và validator.
{
    public class UserAddressDto // Định nghĩa lớp DTO truyền tải dữ liệu UserAddressDto.
    {
        public int Id { get; set; } // Thuộc tính Id kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public Guid UserId { get; set; } // Thuộc tính UserId kiểu dữ liệu Guid lưu trữ thông tin truyền tải.
        public string ReceiverName { get; set; } = string.Empty; // Thuộc tính ReceiverName kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public string PhoneNumber { get; set; } = string.Empty; // Thuộc tính PhoneNumber kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public string AddressLine { get; set; } = string.Empty; // Thuộc tính AddressLine kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public string City { get; set; } = string.Empty; // Thuộc tính City kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public bool IsDefault { get; set; } // Thuộc tính IsDefault kiểu dữ liệu bool lưu trữ thông tin truyền tải.
    }
}

namespace PBL3.Shared.DTOs.ServiceTickets // Định nghĩa namespace PBL3.Shared.DTOs.ServiceTickets quản lý cấu trúc code truyền tải và validator.
{
    public class ServiceTicketIntakeEvaluationDto // Định nghĩa lớp DTO truyền tải dữ liệu ServiceTicketIntakeEvaluationDto.
    {
        public string SerialNumber { get; set; } = string.Empty; // Thuộc tính SerialNumber kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public string ProductName { get; set; } = string.Empty; // Thuộc tính ProductName kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public string VariantName { get; set; } = string.Empty; // Thuộc tính VariantName kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.

        public DateTime? SoldDate { get; set; } // Thuộc tính SoldDate kiểu dữ liệu DateTime? lưu trữ thông tin truyền tải.
        public bool IsInWarranty { get; set; } // Thuộc tính IsInWarranty kiểu dữ liệu bool lưu trữ thông tin truyền tải.
        public DateTime? WarrantyExpiresOn { get; set; } // Thuộc tính WarrantyExpiresOn kiểu dữ liệu DateTime? lưu trữ thông tin truyền tải.
        public DateTime? WarrantyEndDate => WarrantyExpiresOn; // Thực thi dòng lệnh nghiệp vụ.
        public string WarrantySource { get; set; } = string.Empty; // Thuộc tính WarrantySource kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.

        public Guid? CustomerId { get; set; } // Thuộc tính CustomerId kiểu dữ liệu Guid? lưu trữ thông tin truyền tải.
        public string? CustomerName { get; set; } // Thuộc tính CustomerName kiểu dữ liệu string? lưu trữ thông tin truyền tải.
        public string? CustomerEmail { get; set; } // Thuộc tính CustomerEmail kiểu dữ liệu string? lưu trữ thông tin truyền tải.

        public string? BlockingReason { get; set; } // Thuộc tính BlockingReason kiểu dữ liệu string? lưu trữ thông tin truyền tải.

        public List<string> AllowedBranches { get; set; } = new(); // Thuộc tính AllowedBranches kiểu dữ liệu List<string> lưu trữ thông tin truyền tải với giá trị mặc định là new().
    }
}

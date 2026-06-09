namespace PBL3.Shared.DTOs.Inventory // Định nghĩa namespace PBL3.Shared.DTOs.Inventory quản lý cấu trúc code truyền tải và validator.
{
    public class ProductSerialDetailDto // Định nghĩa lớp DTO truyền tải dữ liệu ProductSerialDetailDto.
    {
        public int Id { get; set; } // Thuộc tính Id kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public string SerialNumber { get; set; } = string.Empty; // Thuộc tính SerialNumber kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public byte Status { get; set; } // Thuộc tính Status kiểu dữ liệu byte lưu trữ thông tin truyền tải.
        public string StatusLabel { get; set; } = string.Empty; // Thuộc tính StatusLabel kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public DateTime CreatedDate { get; set; } // Thuộc tính CreatedDate kiểu dữ liệu DateTime lưu trữ thông tin truyền tải.
        public DateTime? SoldDate { get; set; } // Thuộc tính SoldDate kiểu dữ liệu DateTime? lưu trữ thông tin truyền tải.
        public int VariantId { get; set; } // Thuộc tính VariantId kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public string VariantName { get; set; } = string.Empty; // Thuộc tính VariantName kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public string SKU { get; set; } = string.Empty; // Thuộc tính SKU kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public int WarrantyMonth { get; set; } // Thuộc tính WarrantyMonth kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public decimal Price { get; set; } // Thuộc tính Price kiểu dữ liệu decimal lưu trữ thông tin truyền tải.
        public int ProductId { get; set; } // Thuộc tính ProductId kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public string ProductName { get; set; } = string.Empty; // Thuộc tính ProductName kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public int ImportReceiptId { get; set; } // Thuộc tính ImportReceiptId kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public string ReceiptCode { get; set; } = string.Empty; // Thuộc tính ReceiptCode kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public DateTime ImportDate { get; set; } // Thuộc tính ImportDate kiểu dữ liệu DateTime lưu trữ thông tin truyền tải.
        public string SupplierName { get; set; } = string.Empty; // Thuộc tính SupplierName kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public int? OrderId { get; set; } // Thuộc tính OrderId kiểu dữ liệu int? lưu trữ thông tin truyền tải.
        public string? OrderCode { get; set; } // Thuộc tính OrderCode kiểu dữ liệu string? lưu trữ thông tin truyền tải.
        public DateTime? OrderDate { get; set; } // Thuộc tính OrderDate kiểu dữ liệu DateTime? lưu trữ thông tin truyền tải.
        public byte? OrderStatus { get; set; } // Thuộc tính OrderStatus kiểu dữ liệu byte? lưu trữ thông tin truyền tải.
    }
}

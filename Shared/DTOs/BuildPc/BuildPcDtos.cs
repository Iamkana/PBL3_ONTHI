namespace PBL3.Shared.DTOs.BuildPc // Định nghĩa namespace PBL3.Shared.DTOs.BuildPc quản lý cấu trúc code truyền tải và validator.
{
    public class BuildPcItemDto // Định nghĩa lớp DTO truyền tải dữ liệu BuildPcItemDto.
    {
        public int SlotIndex { get; set; } // Thuộc tính SlotIndex kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public string SlotName { get; set; } = string.Empty; // Thuộc tính SlotName kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public int VariantId { get; set; } // Thuộc tính VariantId kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public string ProductName { get; set; } = string.Empty; // Thuộc tính ProductName kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public string VariantName { get; set; } = string.Empty; // Thuộc tính VariantName kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public string? ImageUrl { get; set; } // Thuộc tính ImageUrl kiểu dữ liệu string? lưu trữ thông tin truyền tải.
        public string Sku { get; set; } = string.Empty; // Thuộc tính Sku kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public int WarrantyMonth { get; set; } // Thuộc tính WarrantyMonth kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public decimal UnitPrice { get; set; } // Thuộc tính UnitPrice kiểu dữ liệu decimal lưu trữ thông tin truyền tải.
        public int Quantity { get; set; } = 1; // Thuộc tính Quantity kiểu dữ liệu int lưu trữ thông tin truyền tải với giá trị mặc định là 1.
    }

    public class ExportBuildPcRequest // Định nghĩa lớp DTO truyền tải dữ liệu ExportBuildPcRequest.
    {
        public List<BuildPcItemDto> Items { get; set; } = new(); // Thuộc tính Items kiểu dữ liệu List<BuildPcItemDto> lưu trữ thông tin truyền tải với giá trị mặc định là new().
    }
}

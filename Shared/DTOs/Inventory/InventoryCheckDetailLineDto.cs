namespace PBL3.Shared.DTOs.Inventory // Định nghĩa namespace PBL3.Shared.DTOs.Inventory quản lý cấu trúc code truyền tải và validator.
{
    public class InventoryCheckDetailLineDto // Định nghĩa lớp DTO truyền tải dữ liệu InventoryCheckDetailLineDto.
    {
        public int Id { get; set; } // Thuộc tính Id kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public int VariantId { get; set; } // Thuộc tính VariantId kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public string VariantName { get; set; } = string.Empty; // Thuộc tính VariantName kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public string SKU { get; set; } = string.Empty; // Thuộc tính SKU kiểu dữ liệu string lưu trữ thông tin truyền tải với giá trị mặc định là string.Empty.
        public int SystemQuantity { get; set; } // Thuộc tính SystemQuantity kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public int ActualQuantity { get; set; } // Thuộc tính ActualQuantity kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public int Difference { get; set; } // Thuộc tính Difference kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public int MatchedQuantity { get; set; } // Thuộc tính MatchedQuantity kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public int MissingQuantity { get; set; } // Thuộc tính MissingQuantity kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public int SurplusQuantity { get; set; } // Thuộc tính SurplusQuantity kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public int DefectiveQuantity { get; set; } // Thuộc tính DefectiveQuantity kiểu dữ liệu int lưu trữ thông tin truyền tải.
        public string? Reason { get; set; } // Thuộc tính Reason kiểu dữ liệu string? lưu trữ thông tin truyền tải.
    }
}

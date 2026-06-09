using PBL3.Shared.DTOs.Storefront; // Sử dụng các DTO của module Storefront thuộc tầng Shared.

namespace Client.Services.Comparison; // Thiết lập namespace Client.Services.Comparison để tổ chức quản lý cấu trúc các lớp.

public sealed class ComparisonAddResult // Thực thi dòng lệnh nghiệp vụ.
{
    public bool Success { get; init; } // Thực thi dòng lệnh nghiệp vụ.
    public bool Replaced { get; init; } // Thực thi dòng lệnh nghiệp vụ.
    public string? OldCategoryName { get; init; } // Thực thi dòng lệnh nghiệp vụ.
    public string? ErrorMessage { get; init; } // Thực thi dòng lệnh nghiệp vụ.

    public static ComparisonAddResult Added() // Thực thi dòng lệnh nghiệp vụ.
        => new() { Success = true }; // Thực thi dòng lệnh nghiệp vụ.

    public static ComparisonAddResult ReplacedCategory(string oldCategoryName) // Thực thi dòng lệnh nghiệp vụ.
        => new() { Success = true, Replaced = true, OldCategoryName = oldCategoryName }; // Thực thi dòng lệnh nghiệp vụ.

    public static ComparisonAddResult Fail(string message) // Thực thi dòng lệnh nghiệp vụ.
        => new() { Success = false, ErrorMessage = message }; // Thực thi dòng lệnh nghiệp vụ.

    public static ComparisonAddResult AlreadyExists() // Thực thi dòng lệnh nghiệp vụ.
        => new() { Success = false }; // Thực thi dòng lệnh nghiệp vụ.
}

public interface IComparisonService // Định nghĩa giao diện (interface) IComparisonService quy định các hàm tương tác của client.
{
    IReadOnlyList<ProductCardResponse> Items { get; } // Thực thi dòng lệnh nghiệp vụ.
    bool Contains(int productId); // Thực thi dòng lệnh nghiệp vụ.
    ComparisonAddResult TryAdd(ProductCardResponse product); // Thực thi dòng lệnh nghiệp vụ.
    void Remove(int productId); // Khai báo phương thức giao diện 'Remove' với tham số (productId) có kết quả trả về kiểu void.
    void Clear(); // Khai báo phương thức giao diện 'Clear' không tham số có kết quả trả về kiểu void.
    event Action OnChanged; // Thực thi dòng lệnh nghiệp vụ.
}

using PBL3.Shared.DTOs.Storefront; // Sử dụng các DTO của module Storefront thuộc tầng Shared.

namespace Client.Services.Comparison; // Thiết lập namespace Client.Services.Comparison để tổ chức quản lý cấu trúc các lớp.

public class ComparisonService : IComparisonService // Định nghĩa lớp ComparisonService triển khai các dịch vụ hoặc kế thừa từ IComparisonService.
{
    private readonly List<ProductCardResponse> _items = new(); // Thực hiện gán giá trị của biểu thức 'new()' cho biến/thuộc tính 'private readonly List<ProductCardResponse> _items'.

    public IReadOnlyList<ProductCardResponse> Items => _items; // Thực hiện gán giá trị của biểu thức '> _items' cho biến/thuộc tính 'public IReadOnlyList<ProductCardResponse> Items'.

    public event Action? OnChanged; // Thực thi dòng lệnh nghiệp vụ.

    public bool Contains(int productId) => _items.Any(p => p.Id == productId); // Thực thi dòng lệnh nghiệp vụ.

    public ComparisonAddResult TryAdd(ProductCardResponse product) // Thực thi dòng lệnh nghiệp vụ.
    {
        if (Contains(product.Id)) // Kiểm tra xem điều kiện 'Contains(product.Id' có thỏa mãn hay không.
            return ComparisonAddResult.AlreadyExists(); // Trả về giá trị của biểu thức 'ComparisonAddResult.AlreadyExists()'.

        if (_items.Count >= 3) // Kiểm tra xem điều kiện '_items.Count >= 3' có thỏa mãn hay không.
            return ComparisonAddResult.Fail("Chỉ có thể so sánh tối đa 3 sản phẩm cùng lúc."); // Trả về giá trị của biểu thức 'ComparisonAddResult.Fail("Chỉ có thể so sánh tối đa 3 sản phẩm cùng lúc.")'.

        if (_items.Count > 0 && _items[0].CategoryId != product.CategoryId) // Kiểm tra xem điều kiện '_items.Count > 0 && _items[0].CategoryId != product.CategoryId' có thỏa mãn hay không.
        {
            var oldCategoryName = _items[0].CategoryName; // Thực hiện gán giá trị của biểu thức '_items[0].CategoryName' cho biến/thuộc tính 'oldCategoryName'.
            _items.Clear(); // Thực thi dòng lệnh nghiệp vụ.
            _items.Add(product); // Thực thi dòng lệnh nghiệp vụ.
            OnChanged?.Invoke(); // Thực thi dòng lệnh nghiệp vụ.
            return ComparisonAddResult.ReplacedCategory(oldCategoryName); // Trả về giá trị của biểu thức 'ComparisonAddResult.ReplacedCategory(oldCategoryName)'.
        }

        _items.Add(product); // Thực thi dòng lệnh nghiệp vụ.
        OnChanged?.Invoke(); // Thực thi dòng lệnh nghiệp vụ.
        return ComparisonAddResult.Added(); // Trả về giá trị của biểu thức 'ComparisonAddResult.Added()'.
    }

    public void Remove(int productId) // Thực hiện xử lý phương thức nghiệp vụ 'Remove' nhận tham số (productId) trả về kết quả kiểu void.
    {
        _items.RemoveAll(p => p.Id == productId); // Thực thi dòng lệnh nghiệp vụ.
        OnChanged?.Invoke(); // Thực thi dòng lệnh nghiệp vụ.
    }

    public void Clear() // Thực hiện xử lý phương thức nghiệp vụ 'Clear' không tham số trả về kết quả kiểu void.
    {
        _items.Clear(); // Thực thi dòng lệnh nghiệp vụ.
        OnChanged?.Invoke(); // Thực thi dòng lệnh nghiệp vụ.
    }
}

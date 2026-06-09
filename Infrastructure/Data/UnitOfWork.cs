using Microsoft.EntityFrameworkCore.Storage; // Nhập thư viện hỗ trợ quản lý Transaction vật lý trong cơ sở dữ liệu.
using PBL3.Core.Interfaces; // Nhập namespace chứa các giao diện (interface) dùng chung của hệ thống.

namespace PBL3.Infrastructure.Data // Định nghĩa namespace PBL3.Infrastructure.Data quản lý cấu trúc code.
{
    /// <summary>
    /// Unit of Work implementation — bọc HushStoreDbContext.
    /// Quản lý IDbContextTransaction cho các nghiệp vụ phức tạp (nhập kho, đặt hàng...).
    /// </summary>
    public class UnitOfWork(HushStoreDbContext context) : IUnitOfWork // Định nghĩa lớp UnitOfWork sử dụng Primary Constructor nhận (HushStoreDbContext context) triển khai/kế thừa IUnitOfWork.
    {
        private readonly HushStoreDbContext _context = // Khai báo trường dữ liệu private readonly và bắt đầu gán giá trị.
            context ?? throw new ArgumentNullException(nameof(context)); // Kiểm tra null và ném ra ngoại lệ ArgumentNullException nếu đối tượng context bị null.
        private IDbContextTransaction? _transaction; // Khai báo biến/trường nội bộ _transaction có kiểu dữ liệu IDbContextTransaction.

        public async Task BeginTransactionAsync() // Thực hiện xử lý bất đồng bộ phương thức 'BeginTransactionAsync' không tham số trả về kiểu Task.
        {
            _transaction = await _context.Database.BeginTransactionAsync(); // Chờ tác vụ bất đồng bộ hoàn thành và gán kết quả cho biến '_transaction'.
        }

        public async Task CommitAsync() // Thực hiện xử lý bất đồng bộ phương thức 'CommitAsync' không tham số trả về kiểu Task.
        {
            if (_transaction == null) // Kiểm tra điều kiện: '_transaction == null'.
                throw new InvalidOperationException("Chưa có Transaction nào được mở."); // Thực thi dòng lệnh nghiệp vụ.

            await _transaction.CommitAsync(); // Thực thi dòng lệnh nghiệp vụ.
        }

        public async Task RollbackAsync() // Thực hiện xử lý bất đồng bộ phương thức 'RollbackAsync' không tham số trả về kiểu Task.
        {
            if (_transaction == null) // Kiểm tra điều kiện: '_transaction == null'.
                throw new InvalidOperationException("Chưa có Transaction nào được mở."); // Thực thi dòng lệnh nghiệp vụ.

            await _transaction.RollbackAsync(); // Thực thi dòng lệnh nghiệp vụ.
        }

        public async Task<int> SaveChangesAsync() // Thực hiện xử lý bất đồng bộ phương thức 'SaveChangesAsync' không tham số trả về kiểu Task<int>.
        {
            return await _context.SaveChangesAsync(); // Chờ và trả về giá trị của tác vụ bất đồng bộ '_context.SaveChangesAsync()'.
        }

        public void Dispose() // Thực hiện xử lý phương thức 'Dispose' không tham số trả về kiểu void.
        {
            _transaction?.Dispose(); // Giải phóng tài nguyên transaction nếu nó đang tồn tại.
            GC.SuppressFinalize(this); // Yêu cầu GC không gọi bộ dọn dẹp giải phóng đối tượng này nữa.
        }
    }
}

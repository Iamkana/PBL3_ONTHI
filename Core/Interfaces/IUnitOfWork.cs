namespace PBL3.Core.Interfaces // Định nghĩa namespace PBL3.Core.Interfaces để quản lý thực thể/giao diện.
{
    /// <summary>
    /// Unit of Work Pattern — Quản lý Transaction xuyên suốt nhiều Repository.
    /// Service inject interface này thay vì inject trực tiếp DbContext.
    /// </summary>
    public interface IUnitOfWork : IDisposable // Định nghĩa giao diện (interface) IUnitOfWork kế thừa từ IDisposable quy định hợp đồng cho tầng dữ liệu.
    {
        /// <summary>
        /// Bắt đầu một Database Transaction mới.
        /// </summary>
        Task BeginTransactionAsync(); // Định nghĩa phương thức bất đồng bộ 'BeginTransactionAsync' không tham số trả về kiểu Task.

        /// <summary>
        /// Commit Transaction hiện tại.
        /// </summary>
        Task CommitAsync(); // Định nghĩa phương thức bất đồng bộ 'CommitAsync' không tham số trả về kiểu Task.

        /// <summary>
        /// Rollback Transaction hiện tại.
        /// </summary>
        Task RollbackAsync(); // Định nghĩa phương thức bất đồng bộ 'RollbackAsync' không tham số trả về kiểu Task.

        /// <summary>
        /// Lưu tất cả thay đổi pending trong DbContext xuống Database.
        /// </summary>
        Task<int> SaveChangesAsync(); // Định nghĩa phương thức bất đồng bộ 'SaveChangesAsync' không tham số trả về kiểu Task<int>.
    }
}

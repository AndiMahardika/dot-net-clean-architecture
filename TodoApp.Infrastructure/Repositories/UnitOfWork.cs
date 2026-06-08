using TodoApp.Application.Interfaces;
using TodoApp.Infrastructure.Persistence;

namespace TodoApp.Infrastructure.Repositories;

// Implementasi IUnitOfWork.
// Bertugas mengelola repository dan menyimpan perubahan ke database.
public class UnitOfWork : IUnitOfWork
{
    // Instance DbContext yang digunakan oleh seluruh repository.
    private readonly AppDbContext _context;

    // Menyimpan instance TodoRepository.
    // Nullable (?) karena belum dibuat saat object UnitOfWork dibuat.
    private ITodoRepository? _todos;

    // Constructor menerima AppDbContext melalui Dependency Injection.
    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    // Property untuk mengakses TodoRepository.
    // Repository akan dibuat saat pertama kali digunakan (Lazy Loading).
    public ITodoRepository Todos => _todos ??= new TodoRepository(_context);

    // Menyimpan seluruh perubahan yang ada di DbContext ke database.
    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}
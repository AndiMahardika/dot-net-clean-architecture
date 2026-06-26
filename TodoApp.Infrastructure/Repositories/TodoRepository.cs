using Microsoft.EntityFrameworkCore;
using TodoApp.Application.Interfaces;
using TodoApp.Domain.Entities;
using TodoApp.Infrastructure.Persistence;

namespace TodoApp.Infrastructure.Repositories;

// Implementasi ITodoRepository menggunakan Entity Framework Core.
public class TodoRepository : ITodoRepository
{
    // DbContext digunakan untuk berinteraksi dengan database.
    private readonly AppDbContext _context;

    // Constructor menerima AppDbContext melalui Dependency Injection.
    public TodoRepository(AppDbContext context)
    {
        _context = context;
    }

    // Menambahkan data Todo baru ke database.
    public async Task CreateAsync(Todo todo)
    {
        _context.Todos.Add(todo);

        // Menyimpan perubahan ke database.
        await _context.SaveChangesAsync();
    }

    // Mengambil seluruh data Todo.
    public Task<List<Todo>> GetAllAsync()
        => _context.Todos.ToListAsync();

    // Mengambil Todo berdasarkan Id.
    // Mengembalikan null jika data tidak ditemukan.
    public async Task<Todo?> GetByIdAsync(int id)
    {
        return await _context.Todos.FindAsync(id);
    }

    // Memperbarui data Todo.
    public Task UpdateAsync(Todo todo)
    {
        _context.Todos.Update(todo);

        // Tidak perlu operasi async sehingga menggunakan CompletedTask.
        return Task.CompletedTask;
    }

    // Menghapus Todo berdasarkan Id.
    public async Task DeleteAsync(int id)
    {
        var todo = await GetByIdAsync(id);

        if (todo != null)
        {
            _context.Todos.Remove(todo);
        }
    }

    // Get all todos by id user
    public async Task<List<Todo>> GetByUserIdAsync(int userId)
    {
        return await _context.Todos.Where(t => t.UserId == userId).ToListAsync();
    }
}

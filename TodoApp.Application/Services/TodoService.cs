using TodoApp.Application.Interfaces;
using TodoApp.Domain.Entities;

namespace TodoApp.Application.Services;

// Service berisi business logic aplikasi Todo
// Service berkomunikasi dengan Repository melalui UnitOfWork
public class TodoService
{
    // Dependency Injection IUnitOfWork
    // Digunakan untuk mengakses repository dan menyimpan perubahan
    // Field readonly untuk menyimpan instance IUnitOfWork.
    // readonly berarti nilainya hanya bisa diisi saat deklarasi atau constructor
    // dan tidak bisa diubah lagi setelah object dibuat.
    private readonly IUnitOfWork _unitOfWork;

    public TodoService(IUnitOfWork unitOfWork)
    {
        // Menyimpan instance IUnitOfWork ke field private
        // agar dapat digunakan oleh seluruh method dalam TodoService.
        _unitOfWork = unitOfWork;
    }

    // Mengambil seluruh data Todo
    public async Task<List<Todo>> GetAllAsync()
    {
        return await _unitOfWork.Todos.GetAllAsync();
    }

    // Mengambil Todo berdasarkan Id
    public async Task<Todo?> GetByIdAsync(int id)
    {
        return await _unitOfWork.Todos.GetByIdAsync(id);
    }

    // Membuat Todo baru
    public async Task<Todo> CreateAsync(
        string title,
        string description = "",
        bool isCompleted = false)
    {
        // Membuat object Todo baru
        var todo = new Todo
        {
            Title = title,
            Description = description,
            IsCompleted = isCompleted
        };

        // Menambahkan ke DbContext melalui Repository
        await _unitOfWork.Todos.CreateAsync(todo);

        // Menyimpan perubahan ke database
        await _unitOfWork.SaveChangesAsync();

        return todo;
    }

    // Memperbarui Todo berdasarkan Id
    public async Task UpdateAsync(
        int id,
        string title,
        string description = "",
        bool isCompleted = false)
    {
        // Mencari data Todo
        var todo = await _unitOfWork.Todos.GetByIdAsync(id);

        // Validasi jika data tidak ditemukan
        if (todo == null)
        {
            throw new Exception("Todo not found");
        }

        // Mengubah nilai property
        todo.Title = title;
        todo.Description = description;
        todo.IsCompleted = isCompleted;

        // Menandai entity sebagai Updated
        await _unitOfWork.Todos.UpdateAsync(todo);

        // Menyimpan perubahan ke database
        await _unitOfWork.SaveChangesAsync();
    }

    // Menghapus Todo berdasarkan Id
    public async Task DeleteAsync(int id)
    {
        await _unitOfWork.Todos.DeleteAsync(id);

        // Menyimpan perubahan ke database
        await _unitOfWork.SaveChangesAsync();
    }
}

// Alur Arsitektur
//Controller / API
//       ↓
//   TodoService
//       ↓
//   IUnitOfWork
//       ↓
//ITodoRepository
//       ↓
//   AppDbContext
//       ↓
//   PostgreSQL

// Catatan
// `TodoService` berada di **Application Layer** dan berisi business logic.
// Service tidak mengetahui EF Core atau PostgreSQL secara langsung.
// Semua akses data dilakukan melalui `ITodoRepository`.
// `SaveChangesAsync()` dipanggil dari `IUnitOfWork` agar seluruh perubahan dapat disimpan dalam satu unit pekerjaan (Unit of Work).
// Pada project sederhana, `TodoService` sering langsung menggunakan `ITodoRepository`, namun pada Clean Architecture sering ditambahkan `IUnitOfWork` untuk mengelola transaksi dan koordinasi beberapa repository.
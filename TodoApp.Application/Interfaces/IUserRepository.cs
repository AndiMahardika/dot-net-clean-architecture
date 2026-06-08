using TodoApp.Domain.Entities;

namespace TodoApp.Application.Interfaces;

public interface IUserRepository
{
    // Mengambil semua data User
    Task<List<User>> GetAllAsync();

    // Mengambil data User berdasarkan Id
    Task<User?> GetByIdAsync(int id);

    // Mengambil data User berdasarkan Username
    Task<User?> GetByUsernameAsync(string username);

    // Menambahkan data User baru ke DbContext
    // Belum tersimpan ke database sampai SaveChangesAsync dipanggil
    Task CreateAsync(User user);

    // Memperbarui data User di DbContext
    Task UpdateAsync(User user);

    // Menghapus data User berdasarkan Id
    Task DeleteAsync(int id);
}
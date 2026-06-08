using TodoApp.Domain.Entities;

namespace TodoApp.Application.Interfaces;

// Kontrak Repository untuk operasi CRUD pada entity Todo
public interface ITodoRepository
{
    // Mengambil seluruh data Todo
    Task<List<Todo>> GetAllAsync();

    // Mengambil Todo berdasarkan Id
    // Mengembalikan null jika data tidak ditemukan
    Task<Todo?> GetByIdAsync(int id);

    // Menambahkan Todo baru ke DbContext
    // Belum tersimpan ke database sampai SaveChangesAsync dipanggil
    Task CreateAsync(Todo todo);

    // Memperbarui data Todo di DbContext
    Task UpdateAsync(Todo todo);

    // Menghapus Todo berdasarkan Id
    Task DeleteAsync(int id);
}

// Unit of Work digunakan untuk mengelompokkan repository
// dan menyimpan seluruh perubahan dalam satu transaksi
public interface IUnitOfWork
{
    // Akses ke repository Todo
    ITodoRepository Todos { get; }

    // Menyimpan seluruh perubahan ke database
    // Mengembalikan jumlah data yang berhasil diproses
    Task<int> SaveChangesAsync();
}


// Catatan

// `Task<T>` digunakan untuk method asynchronous yang mengembalikan nilai.
// `Task<Todo?>` berarti method akan mengembalikan objek `Todo` atau `null`.
// `Task` tanpa `<T>` berarti method asynchronous yang tidak mengembalikan nilai.
// `IUnitOfWork` membantu memastikan beberapa operasi repository dapat disimpan bersamaan melalui satu pemanggilan `SaveChangesAsync()`.
// Pada EF Core, `DbContext` sebenarnya sudah menerapkan konsep Unit of Work, namun pola ini sering digunakan pada Clean Architecture untuk mengurangi ketergantungan langsung terhadap EF Core.
// `Task` digunakan untuk menjalankan operasi **asynchronous (async)**, yaitu operasi yang membutuhkan waktu untuk selesai seperti akses database, API, atau file.
// Dengan `Task`, aplikasi tidak perlu menunggu proses selesai dan dapat mengerjakan tugas lain terlebih dahulu.

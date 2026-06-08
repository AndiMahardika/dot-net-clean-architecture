using Microsoft.EntityFrameworkCore;
using TodoApp.Domain.Entities;

namespace TodoApp.Infrastructure.Persistence;

// DbContext adalah representasi database pada EF Core.
// Bertugas mengelola koneksi, query, tracking entity,
// dan menyimpan perubahan ke database.
public class AppDbContext : DbContext
{
    // Constructor menerima konfigurasi database
    // melalui Dependency Injection.
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    // Merepresentasikan tabel Todos pada database.
    // EF Core akan membuat tabel berdasarkan entity Todo.
    public DbSet<Todo> Todos { get; set; }

    // Digunakan untuk konfigurasi entity menggunakan Fluent API.
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Menjalankan konfigurasi bawaan dari DbContext.
        base.OnModelCreating(modelBuilder);

        // Konfigurasi entity Todo.
        modelBuilder.Entity<Todo>(entity =>
        {
            // Nama tabel pada database.
            entity.ToTable("Todos");

            // Menentukan primary key.
            entity.HasKey(e => e.Id);

            // Kolom Title wajib diisi dan maksimal 100 karakter.
            entity.Property(e => e.Title)
                  .IsRequired()
                  .HasMaxLength(100);

            // Kolom Description wajib diisi dan maksimal 500 karakter.
            entity.Property(e => e.Description)
                  .IsRequired()
                  .HasMaxLength(500);

            // Kolom IsCompleted wajib memiliki nilai.
            entity.Property(e => e.IsCompleted)
                  .IsRequired();

            // Kolom CreatedAt wajib memiliki nilai.
            entity.Property(e => e.CreatedAt)
                  .IsRequired();

            // Kolom UpdatedAt wajib memiliki nilai.
            entity.Property(e => e.UpdatedAt)
                  .IsRequired();
        });
    }
}

// Komponen Penting

// DbContext
//public class AppDbContext : DbContext
// Merupakan kelas utama EF Core yang berfungsi sebagai jembatan antara aplikasi dan database.

// DbContextOptions
//public AppDbContext(DbContextOptions<AppDbContext> options)
// Berisi konfigurasi database seperti:
//- Connection String
//- Database Provider (PostgreSQL, Oracle, SQL Server)
//- Logging
//- Migration

// DbSet<T>
// public DbSet<Todo> Todos { get; set; }
// Merepresentasikan tabel database.

// DbSet<Todo>
// ↓
// Table Todos

// Contoh penggunaan:
//_context.Todos.Add(todo);
//_context.Todos.ToListAsync();

// OnModelCreating
// protected override void OnModelCreating(ModelBuilder modelBuilder)
// Digunakan untuk mengatur struktur tabel menggunakan Fluent API.

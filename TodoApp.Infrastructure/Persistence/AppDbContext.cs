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
      public DbSet<User> Users { get; set; }

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

            // Konfigurasi entity User.
            modelBuilder.Entity<User>(entity =>
            {
                  // Nama tabel pada database.
                  entity.ToTable("Users");

                  // Menentukan primary key.
                  entity.HasKey(e => e.Id);

                  // Kolom Username wajib diisi dan maksimal 50 karakter.
                  entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(50);

                  // Kolom Email wajib diisi dan maksimal 100 karakter.
                  entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(100);

                  // Kolom Password wajib diisi dan maksimal 100 karakter.
                  entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(100);
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

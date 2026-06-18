using TodoApp.Domain.Entities;

namespace TodoApp.Application.Interfaces;

public interface IPasswordHasher
{
    // Meng-hash password
    string Hash(string password);

    // Memverifikasi password
    bool Verify(string hash, string password);
}
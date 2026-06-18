using TodoApp.Application.Interfaces;
using BCrypt.Net;

public class BcryptPasswordHasher : IPasswordHasher
{
    public string Hash(string password)
    {
        // Menghasilkan hash menggunakan BCrypt
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public bool Verify(string passwordHash, string inputPassword)
    {
        // Memverifikasi input password dengan hash yang tersimpan
        return BCrypt.Net.BCrypt.Verify(inputPassword, passwordHash);
    }
}

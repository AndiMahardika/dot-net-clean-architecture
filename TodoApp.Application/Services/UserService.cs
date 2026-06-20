using TodoApp.Application.DTOs;
using TodoApp.Application.Interfaces;
using TodoApp.Domain.Entities;

namespace TodoApp.Application.Services;

public class UserService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenProvider _tokenProvider;

    public UserService(IUserRepository userRepository, IPasswordHasher passwordHasher, ITokenProvider tokenProvider)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenProvider = tokenProvider;
    }

    public async Task<List<User>> GetAllAsync() => await _userRepository.GetAllAsync();

    public async Task<User?> GetByIdAsync(int id) => await _userRepository.GetByIdAsync(id);

    public async Task<User?> GetByUsernameAsync(string name) => await _userRepository.GetByUsernameAsync(name);

    public async Task<User?> CreateAsync(string username, string email, string password)
    {
        string passwordHash = _passwordHasher.Hash(password);

        var user = new User
        {
            Username = username,
            Email = email,
            Password = passwordHash
        };

        await _userRepository.CreateAsync(user);

        return user;
    }

    public async Task UpdateAsync(int id, string username)
    {
        var user = await _userRepository.GetByIdAsync(id)
            ?? throw new Exception("User not found");

        user.Username = username;

        await _userRepository.UpdateAsync(user);
    }

    public async Task DeleteAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id)
            ?? throw new Exception("User not found");

        await _userRepository.DeleteAsync(user.Id);
    }

    public async Task<(User User, string Token)> LoginAsync(LoginRequest request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email.Trim()) ?? throw new Exception("User not found");

        bool isValidPassword = _passwordHasher.Verify(user.Password, request.Password);
        if (!isValidPassword)
        {
            throw new Exception("Invalid password");
        }

        var token = _tokenProvider.GenerateToken(user);

        return (user, token);
    }
}
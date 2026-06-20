using TodoApp.Domain.Entities;

namespace TodoApp.Application.Interfaces;

public interface ITokenProvider
{
    string GenerateToken(User user);
}
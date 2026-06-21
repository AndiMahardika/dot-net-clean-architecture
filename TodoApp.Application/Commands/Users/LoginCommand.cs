using MediatR;
using TodoApp.Application.DTOs;
using TodoApp.Application.Interfaces;
using TodoApp.Domain.Entities;

namespace TodoApp.Application.Commands.Users;

public record LoginCommand(string Email, string Password) : IRequest<(User User, string Token)>;

public class LoginCommandHandler : IRequestHandler<LoginCommand, (User User, string Token)>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenProvider _tokenProvider;

    public LoginCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher, ITokenProvider tokenProvider)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenProvider = tokenProvider;
    }

    public async Task<(User User, string Token)> Handle(LoginCommand request, CancellationToken cancellationToken)
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

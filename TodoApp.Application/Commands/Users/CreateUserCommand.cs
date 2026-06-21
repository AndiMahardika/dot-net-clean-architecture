using MediatR;
using TodoApp.Application.Interfaces;
using TodoApp.Domain.Entities;

namespace TodoApp.Application.Commands.Users;

public record CreateUserCommand(string Username, string Email, string Password) : IRequest<User>;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, User>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public CreateUserCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<User> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        string passwordHash = _passwordHasher.Hash(request.Password);

        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            Password = passwordHash
        };

        await _userRepository.CreateAsync(user);

        return user;
    }
}

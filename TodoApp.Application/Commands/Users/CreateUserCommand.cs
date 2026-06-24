using MediatR;
using TodoApp.Application.Interfaces;
using TodoApp.Domain.Entities;
using TodoApp.Application.EventHandlers.Users;

namespace TodoApp.Application.Commands.Users;

public record CreateUserCommand(string Username, string Email, string Password) : IRequest<User>;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, User>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IMediator _mediator;

    public CreateUserCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher, IMediator mediator)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _mediator = mediator;
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
        await _mediator.Publish(new UserCreatedEvent(user.Id, user.Email, user.Username), cancellationToken);

        return user;
    }
}

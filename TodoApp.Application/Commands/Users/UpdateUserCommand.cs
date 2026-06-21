using MediatR;
using TodoApp.Application.Interfaces;

namespace TodoApp.Application.Commands.Users;

public record UpdateUserCommand(int Id, string Username) : IRequest;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand>
{
    private readonly IUserRepository _userRepository;

    public UpdateUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.Id)
            ?? throw new Exception("User not found");

        user.Username = request.Username;

        await _userRepository.UpdateAsync(user);
    }
}

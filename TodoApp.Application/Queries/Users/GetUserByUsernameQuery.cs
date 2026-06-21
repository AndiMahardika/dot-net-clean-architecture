using MediatR;
using TodoApp.Application.Interfaces;
using TodoApp.Domain.Entities;

namespace TodoApp.Application.Queries.Users;

public record GetUserByUsernameQuery(string Username) : IRequest<User?>;

public class GetUserByUsernameQueryHandler : IRequestHandler<GetUserByUsernameQuery, User?>
{
    private readonly IUserRepository _userRepository;

    public GetUserByUsernameQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<User?> Handle(GetUserByUsernameQuery request, CancellationToken cancellationToken)
    {
        return await _userRepository.GetByUsernameAsync(request.Username);
    }
}

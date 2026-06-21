using MediatR;
using TodoApp.Application.Interfaces;
using TodoApp.Domain.Entities;

namespace TodoApp.Application.Queries;

// Query untuk mengambil Todo berdasarkan Id
// IRequest<Todo?> berarti query ini akan mengembalikan object Todo atau null
public record GetTodoByIdQuery(int Id) : IRequest<Todo?>;

// Handler untuk mengambil Todo berdasarkan Id
public class GetTodoByIdQueryHandler : IRequestHandler<GetTodoByIdQuery, Todo?>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetTodoByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Todo?> Handle(GetTodoByIdQuery request, CancellationToken cancellationToken)
    {
        return await _unitOfWork.Todos.GetByIdAsync(request.Id);
    }
}
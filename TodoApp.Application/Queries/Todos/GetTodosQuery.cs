using MediatR;
using TodoApp.Application.Interfaces;
using TodoApp.Domain.Entities;

namespace TodoApp.Application.Queries;

// Query untuk mengambil semua Todo
// IRequest<List<Todo>> berarti query ini akan mengembalikan list of Todo
public record GetTodosQuery(int UserId) : IRequest<List<Todo>>;

// Handler untuk mengambil semua Todo
public class GetTodosQueryHandler : IRequestHandler<GetTodosQuery, List<Todo>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetTodosQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<Todo>> Handle(GetTodosQuery request, CancellationToken cancellationToken)
    {
        return await _unitOfWork.Todos.GetByUserIdAsync(request.UserId);
    }
}
using MediatR;
using TodoApp.Application.Interfaces;
using TodoApp.Domain.Entities;

namespace TodoApp.Application.Commands;

// 1. Ini adalah Command-nya (berisi data yang dikirim dari API)
// IRequest<Todo> berarti command ini jika sukses akan mengembalikan object Todo
public record CreateTodoCommand(string Title, string Description, bool IsCompleted, int UserId) : IRequest<Todo>;

// 2. Ini adalah Handler-nya (berisi business logic dari TodoService lama)
public class CreateTodoCommandHandler : IRequestHandler<CreateTodoCommand, Todo>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateTodoCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Todo> Handle(CreateTodoCommand request, CancellationToken cancellationToken)
    {
        var todo = new Todo
        {
            Title = request.Title,
            Description = request.Description,
            IsCompleted = request.IsCompleted,
            UserId = request.UserId
        };

        await _unitOfWork.Todos.CreateAsync(todo);
        await _unitOfWork.SaveChangesAsync();

        return todo;
    }
}

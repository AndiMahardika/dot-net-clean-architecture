using MediatR;
using TodoApp.Application.Interfaces;

namespace TodoApp.Application.Commands;

// Command untuk memperbarui Todo
public record UpdateTodoCommand(
    int Id,
    string Title,
    string Description,
    bool IsCompleted,
    int UserId
) : IRequest;

// Handler untuk memperbarui Todo
public class UpdateTodoCommandHandler : IRequestHandler<UpdateTodoCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateTodoCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UpdateTodoCommand request, CancellationToken cancellationToken)
    {
        var todo = await _unitOfWork.Todos.GetByIdAsync(request.Id);

        if (todo == null)
        {
            throw new Exception("Todo not found");
        }

        todo.Title = request.Title;
        todo.Description = request.Description;
        todo.IsCompleted = request.IsCompleted;
        todo.UserId = request.UserId;

        await _unitOfWork.Todos.UpdateAsync(todo);
        await _unitOfWork.SaveChangesAsync();
    }
}
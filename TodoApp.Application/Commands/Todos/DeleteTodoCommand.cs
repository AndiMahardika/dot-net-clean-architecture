using MediatR;
using TodoApp.Application.Interfaces;

namespace TodoApp.Application.Commands;

// Command untuk menghapus Todo
public record DeleteTodoCommand(int Id) : IRequest;

// Handler untuk menghapus Todo
public class DeleteTodoCommandHandler : IRequestHandler<DeleteTodoCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteTodoCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteTodoCommand request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Todos.DeleteAsync(request.Id);
        await _unitOfWork.SaveChangesAsync();
    }
}
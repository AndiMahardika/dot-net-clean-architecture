namespace TodoApp.Application.DTOs;

public record CreateTodoRequest(string Title, string Description, bool IsCompleted, int UserId);
public record UpdateTodoRequest(string Title, string Description, bool IsCompleted, int UserId);
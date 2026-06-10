namespace TodoApp.Application.DTOs;

public record CreateUserRequest(string Username, string Email, string Password);
public record UpdateUserRequest(string Username);
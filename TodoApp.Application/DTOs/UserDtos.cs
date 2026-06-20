namespace TodoApp.Application.DTOs;

public record CreateUserRequest(string Username, string Email, string Password);
public record UpdateUserRequest(string Username);
public record LoginRequest(string Email, string Password);
public record UserResponse(int Id, string Username, string Email);
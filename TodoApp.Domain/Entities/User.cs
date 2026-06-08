namespace TodoApp.Domain.Entities;

public class User
{
    public int Id { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }

    // Relasi one-to-many dengan Todo
    // Satu user bisa punya banyak todo
    public ICollection<Todo> Todos { get; set; } = new List<Todo>();
}
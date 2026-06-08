namespace TodoApp.Domain.Entities;

public class Todo
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsCompleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Relasi many-to-one dengan User
    // Satu todo hanya milik satu user
    public int? UserId { get; set; }
    public User? User { get; set; } = null!;
}
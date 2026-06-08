using Microsoft.EntityFrameworkCore;
using TodoApp.Application.Interfaces;
using TodoApp.Application.Services;
using TodoApp.Infrastructure.Persistence;
using TodoApp.Infrastructure.Repositories;

// Membuat instance WebApplicationBuilder.
// Digunakan untuk konfigurasi service, middleware, dan aplikasi ASP.NET Core.
var builder = WebApplication.CreateBuilder(args);

// Mendaftarkan AppDbContext ke Dependency Injection (DI).
// UseNpgsql digunakan sebagai provider PostgreSQL.
// Connection string diambil dari appsettings.json.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Mendaftarkan ITodoRepository.
// Saat ITodoRepository diminta, ASP.NET Core akan membuat TodoRepository.
builder.Services.AddScoped<ITodoRepository, TodoRepository>();

// Mendaftarkan IUnitOfWork.
// Saat IUnitOfWork diminta, ASP.NET Core akan membuat UnitOfWork.
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Mendaftarkan TodoService ke Dependency Injection.
builder.Services.AddScoped<TodoService>();

// Membuat aplikasi berdasarkan seluruh konfigurasi yang telah didaftarkan.
var app = builder.Build();

app.MapGet("/todos", async (TodoService service) =>
{
    return await service.GetAllAsync();
});

app.MapPost("/todos", async (
    TodoService service,
    CreateTodoRequest request) =>
{
    var title = request.Title.Trim();
    if (string.IsNullOrEmpty(title))
    {
        return Results.BadRequest("Title is required");
    }
    var description = request.Description.Trim();
    var isCompleted = request.IsCompleted;
    var todo = await service.CreateAsync(title, description, isCompleted);

    return Results.Created($"/todos/{todo.Id}", todo);
});

app.MapGet("/todos/{id}", async (TodoService service, int id) =>
{
    var todo = await service.GetByIdAsync(id);
    if (todo == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(todo);
});

app.MapPut("/todos/{id}", async (
    TodoService service,
    int id,
    UpdateTodoRequest request) =>
{
    var title = request.Title.Trim();
    var description = request.Description.Trim();
    var isCompleted = request.IsCompleted;

    if (string.IsNullOrEmpty(title))
    {
        return Results.BadRequest("Title is required");
    }

    await service.UpdateAsync(id, title, description, isCompleted);

    return Results.Ok();
});

app.MapDelete("/todos/{id}", async (TodoService service, int id) =>
{
    await service.DeleteAsync(id);
    return Results.Ok();
});

app.Run();

public record CreateTodoRequest(string Title, string Description, bool IsCompleted);
public record UpdateTodoRequest(string Title, string Description, bool IsCompleted);
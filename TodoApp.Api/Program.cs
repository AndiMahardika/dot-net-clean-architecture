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
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Mendaftarkan IUnitOfWork.
// Saat IUnitOfWork diminta, ASP.NET Core akan membuat UnitOfWork.
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Mendaftarkan TodoService ke Dependency Injection.
builder.Services.AddScoped<TodoService>();
builder.Services.AddScoped<UserService>();

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
    var userId = request.UserId;
    var todo = await service.CreateAsync(title, description, isCompleted, userId);

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
    var userId = request.UserId;

    if (string.IsNullOrEmpty(title))
    {
        return Results.BadRequest("Title is required");
    }

    await service.UpdateAsync(id, title, description, isCompleted, userId);

    return Results.Ok();
});

app.MapDelete("/todos/{id}", async (TodoService service, int id) =>
{
    await service.DeleteAsync(id);
    return Results.Ok();
});

// User Endpoints
app.MapGet("/users", async (UserService service) =>
{
    return await service.GetAllAsync();
});

app.MapPost("/users", async (
    UserService service,
    CreateUserRequest request) =>
{
    var username = request.Username.Trim();
    var email = request.Email.Trim();
    var password = request.Password.Trim();

    if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
    {
        return Results.BadRequest("Username, email, and password are required");
    }

    var user = await service.CreateAsync(username, email, password);

    return Results.Created($"/users/{user?.Id}", user);
});

app.MapGet("/users/{id}", async (UserService service, int id) =>
{
    var user = await service.GetByIdAsync(id);
    if (user == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(user);
});

app.MapGet("/users/username/{username}", async (UserService service, string username) =>
{
    var user = await service.GetByUsernameAsync(username);
    if (user == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(user);
});

app.MapPut("/users/{id}", async (
    UserService service,
    int id,
    UpdateUserRequest request) =>
{
    var username = request.Username.Trim();

    if (string.IsNullOrEmpty(username))
    {
        return Results.BadRequest("Username is required");
    }

    await service.UpdateAsync(id, username);

    return Results.Ok();
});

app.MapDelete("/users/{id}", async (UserService service, int id) =>
{
    await service.DeleteAsync(id);
    return Results.Ok();
});

app.Run();

public record CreateTodoRequest(string Title, string Description, bool IsCompleted, int UserId);
public record UpdateTodoRequest(string Title, string Description, bool IsCompleted, int UserId);
public record CreateUserRequest(string Username, string Email, string Password);
public record UpdateUserRequest(string Username);
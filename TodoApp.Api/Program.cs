using Microsoft.EntityFrameworkCore;
using TodoApp.Application.Interfaces;
using TodoApp.Application.Services;
using TodoApp.Infrastructure.Persistence;
using TodoApp.Infrastructure.Repositories;
using FluentValidation;
using TodoApp.Application.Validators;
using TodoApp.Application.DTOs;
using TodoApp.Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
// Membuat instance WebApplicationBuilder.
// Digunakan untuk konfigurasi service, middleware, dan aplikasi ASP.NET Core.
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
// Konfigurasi Swagger agar mendukung penginputan Bearer Token di antarmukanya
builder.Services.AddSwaggerGen(c =>
{
    // Menambahkan definisi keamanan "Bearer" ke Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Masukkan token JWT dengan format: Bearer {token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    // Memberitahu Swagger bahwa antarmukanya harus mengirimkan token yang diinputkan
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

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
builder.Services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
builder.Services.AddScoped<ITokenProvider, JwtTokenProvider>();

// Mendaftarkan IUnitOfWork.
// Saat IUnitOfWork diminta, ASP.NET Core akan membuat UnitOfWork.
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Mendaftarkan TodoService ke Dependency Injection.
builder.Services.AddScoped<TodoService>();
builder.Services.AddScoped<UserService>();

// Mendaftarkan seluruh validator dari assembly TodoApp.Application
builder.Services.AddValidatorsFromAssemblyContaining<CreateTodoRequestValidator>();

// Mendaftarkan layanan Authentication menggunakan skema JWT Bearer
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // Konfigurasi aturan untuk memvalidasi token yang masuk dari request
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

// Mendaftarkan layanan Authorization (untuk mengatur izin akses)
builder.Services.AddAuthorization();

// Membuat aplikasi berdasarkan seluruh konfigurasi yang telah didaftarkan.
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Middleware: Mengekstrak token dari header HTTP dan mengidentifikasi User
app.UseAuthentication();

// Middleware: Menentukan apakah User tersebut memiliki izin untuk mengakses rute yang dituju
app.UseAuthorization();

app.MapGet("/todos", async (TodoService service) =>
{
    return await service.GetAllAsync();
}).RequireAuthorization();

app.MapPost("/todos", async (
    TodoService service,
    IValidator<CreateTodoRequest> validator,
    CreateTodoRequest request) =>
{
    var validationResult = await validator.ValidateAsync(request);

    if (!validationResult.IsValid)
    {
        return Results.BadRequest(validationResult.Errors);
    }

    var todo = await service.CreateAsync(
        request.Title.Trim(),
        request.Description.Trim(),
        request.IsCompleted,
        request.UserId!.Value
    );

    return Results.Created($"/todos/{todo.Id}", todo);
}).RequireAuthorization();

app.MapGet("/todos/{id}", async (TodoService service, int id) =>
{
    var todo = await service.GetByIdAsync(id);
    if (todo == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(todo);
}).RequireAuthorization();

app.MapPut("/todos/{id}", async (
    TodoService service,
    int id,
    IValidator<UpdateTodoRequest> validator,
    UpdateTodoRequest request) =>
{
    var validationResult = await validator.ValidateAsync(request);

    if (!validationResult.IsValid)
    {
        return Results.BadRequest(validationResult.Errors);
    }

    await service.UpdateAsync(
        id,
        request.Title.Trim(),
        request.Description.Trim(),
        request.IsCompleted,
        request.UserId!.Value
    );

    return Results.Ok();
}).RequireAuthorization();

app.MapDelete("/todos/{id}", async (TodoService service, int id) =>
{
    await service.DeleteAsync(id);
    return Results.Ok();
}).RequireAuthorization();

// User Endpoints
app.MapPost("/login", async (
    UserService service,
    IValidator<LoginRequest> validator,
    LoginRequest request) =>
{
    var validationResult = await validator.ValidateAsync(request);

    if (!validationResult.IsValid)
    {
        return Results.BadRequest(validationResult.Errors);
    }

    var result = await service.LoginAsync(request);

    var userResponse = new UserResponse(result.User.Id, result.User.Username, result.User.Email);

    return Results.Ok(new { user = userResponse, token = result.Token });
});

app.MapGet("/users", async (UserService service) =>
{
    return await service.GetAllAsync();
});

app.MapPost("/users", async (
    UserService service,
    IValidator<CreateUserRequest> validator,
    CreateUserRequest request) =>
{
    var validationResult = await validator.ValidateAsync(request);

    if (!validationResult.IsValid)
    {
        return Results.BadRequest(validationResult.Errors);
    }

    var user = await service.CreateAsync(request.Username.Trim(), request.Email.Trim(), request.Password.Trim());

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
    IValidator<UpdateUserRequest> validator,
    UpdateUserRequest request) =>
{
    var validationResult = await validator.ValidateAsync(request);

    if (!validationResult.IsValid)
    {
        return Results.BadRequest(validationResult.Errors);
    }

    await service.UpdateAsync(id, request.Username.Trim());

    return Results.Ok();
});

app.MapDelete("/users/{id}", async (UserService service, int id) =>
{
    await service.DeleteAsync(id);
    return Results.Ok();
});

app.Run();
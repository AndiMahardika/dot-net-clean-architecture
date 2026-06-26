using Microsoft.EntityFrameworkCore;
using TodoApp.Application.Interfaces;
using TodoApp.Infrastructure.Persistence;
using TodoApp.Infrastructure.Repositories;
using FluentValidation;
using TodoApp.Application.Validators;
using TodoApp.Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using TodoApp.Api.Endpoints;

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

// Mendaftarkan seluruh validator dari assembly TodoApp.Application
builder.Services.AddValidatorsFromAssemblyContaining<CreateTodoRequestValidator>();
// Mendaftarkan MediatR dan memberitahu agar mencari semua Handler di dalam TodoApp.Application
builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(typeof(TodoApp.Application.Commands.CreateTodoCommand).Assembly);
});


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

app.MapTodoEndpoints();
app.MapUserEndpoints();

app.Run();
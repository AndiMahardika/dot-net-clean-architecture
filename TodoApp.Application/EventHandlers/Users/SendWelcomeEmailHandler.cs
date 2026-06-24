using MediatR;

namespace TodoApp.Application.EventHandlers.Users;

public record UserCreatedEvent(int UserId, string Email, string Username) : INotification;

public class SendWelcomeEmailHandler : INotificationHandler<UserCreatedEvent>
{
    public async Task Handle(UserCreatedEvent notification, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Mengirim email selamat datang ke {notification.Email}");
    }
    // private readonly IEmailService _emailService;

    // public SendWelcomeEmailHandler(IEmailService emailService)
    // {
    //     _emailService = emailService;
    // }

    // public async Task Handle(UserCreatedEvent notification, CancellationToken cancellationToken)
    // {
    //     await _emailService.SendAsync(
    //         notification.Email,
    //         "Welcome to TodoApp",
    //         $"Hi {notification.Username}, welcome to TodoApp!"
    //     );
    // }
}

public class LogUserCreationHandler : INotificationHandler<UserCreatedEvent>
{
    public async Task Handle(UserCreatedEvent notification, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Pengguna baru dibuat: {notification.Email}");
    }
}
using FluentValidation;
using MediatR;
using TodoApp.Application.Commands.Users;
using TodoApp.Application.DTOs;
using TodoApp.Application.Queries.Users;

namespace TodoApp.Api.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        // Login endpoint
        app.MapPost("/login", async (
            IMediator mediator,
            IValidator<LoginRequest> validator,
            LoginRequest request) =>
        {
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                return Results.BadRequest(validationResult.Errors);
            }

            var result = await mediator.Send(new LoginCommand(request.Email, request.Password));

            var userResponse = new UserResponse(result.User.Id, result.User.Username, result.User.Email);

            return Results.Ok(new { user = userResponse, token = result.Token });
        });

        // User group
        var group = app.MapGroup("/users");

        group.MapGet("/", async (IMediator mediator) =>
        {
            return await mediator.Send(new GetUsersQuery());
        });

        group.MapPost("/", async (
            IMediator mediator,
            IValidator<CreateUserRequest> validator,
            CreateUserRequest request) =>
        {
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                return Results.BadRequest(validationResult.Errors);
            }

            var user = await mediator.Send(new CreateUserCommand(request.Username.Trim(), request.Email.Trim(), request.Password.Trim()));

            return Results.Created($"/users/{user?.Id}", user);
        });

        group.MapGet("/{id}", async (IMediator mediator, int id) =>
        {
            var user = await mediator.Send(new GetUserByIdQuery(id));
            if (user == null)
            {
                return Results.NotFound();
            }
            return Results.Ok(user);
        });

        group.MapGet("/username/{username}", async (IMediator mediator, string username) =>
        {
            var user = await mediator.Send(new GetUserByUsernameQuery(username));
            if (user == null)
            {
                return Results.NotFound();
            }
            return Results.Ok(user);
        });

        group.MapPut("/{id}", async (
            IMediator mediator,
            int id,
            IValidator<UpdateUserRequest> validator,
            UpdateUserRequest request) =>
        {
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                return Results.BadRequest(validationResult.Errors);
            }

            await mediator.Send(new UpdateUserCommand(id, request.Username.Trim()));

            return Results.Ok();
        });

        group.MapDelete("/{id}", async (IMediator mediator, int id) =>
        {
            await mediator.Send(new DeleteUserCommand(id));
            return Results.Ok();
        });
    }
}

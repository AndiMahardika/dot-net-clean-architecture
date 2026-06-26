using FluentValidation;
using MediatR;
using TodoApp.Application.Commands;
using TodoApp.Application.DTOs;
using TodoApp.Application.Queries;

namespace TodoApp.Api.Endpoints;

public static class TodoEndpoints
{
    public static void MapTodoEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/todos").RequireAuthorization();

        group.MapGet("/user/{userId}", async (IMediator mediator, int userId) =>
        {
            var todos = await mediator.Send(new GetTodosQuery(userId));
            return Results.Ok(todos);
        });

        group.MapPost("/", async (
            IMediator mediator,
            IValidator<CreateTodoRequest> validator,
            CreateTodoRequest request) =>
        {
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                return Results.BadRequest(validationResult.Errors);
            }

            var todo = await mediator.Send(new CreateTodoCommand(
                request.Title.Trim(),
                request.Description.Trim(),
                request.IsCompleted,
                request.UserId!.Value
            ));

            return Results.Created($"/todos/{todo.Id}", todo);
        });

        group.MapGet("/{id}", async (IMediator mediator, int id) =>
        {
            var todo = await mediator.Send(new GetTodoByIdQuery(id));
            if (todo == null)
            {
                return Results.NotFound();
            }
            return Results.Ok(todo);
        });

        group.MapPut("/{id}", async (
            IMediator mediator,
            int id,
            IValidator<UpdateTodoRequest> validator,
            UpdateTodoRequest request) =>
        {
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                return Results.BadRequest(validationResult.Errors);
            }

            await mediator.Send(new UpdateTodoCommand(
                id,
                request.Title.Trim(),
                request.Description.Trim(),
                request.IsCompleted,
                request.UserId!.Value
            ));

            return Results.Ok();
        });

        group.MapDelete("/{id}", async (IMediator mediator, int id) =>
        {
            await mediator.Send(new DeleteTodoCommand(id));
            return Results.Ok();
        });
    }
}

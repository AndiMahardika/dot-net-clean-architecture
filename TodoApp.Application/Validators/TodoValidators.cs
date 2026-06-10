using FluentValidation;
using TodoApp.Application.DTOs;

namespace TodoApp.Application.Validators;

public class CreateTodoRequestValidator : AbstractValidator<CreateTodoRequest>
{
    public CreateTodoRequestValidator()
    {
        RuleFor(x => x.Title).NotEmpty().WithMessage("Title is required");
        RuleFor(x => x.UserId).NotNull().WithMessage("User ID is required");
    }
}

public class UpdateTodoRequestValidator : AbstractValidator<UpdateTodoRequest>
{
    public UpdateTodoRequestValidator()
    {
        RuleFor(x => x.Title).NotEmpty().WithMessage("Title is required");
        RuleFor(x => x.UserId).NotNull().WithMessage("User ID is required");
    }
}
using Application.DTOs;
using FluentValidation;

public class CreateExpenseCategoryDTOValidator : AbstractValidator<CreateExpenseCategoryDTO>
{
    public CreateExpenseCategoryDTOValidator()
    {
        this.RuleFor(x => x.Title)
            .NotEmpty()
            .Length(5, 100)
            .WithMessage("Title length should be between 5 and 100 characters.");

        this.RuleFor(x => x.PlannedBudget)
            .GreaterThan(0)
            .WithMessage("Planned budget must be greater than 0.")
            .LessThan(100000000)
            .WithMessage("Planned budget must be lower than 100000000.");
    }
}
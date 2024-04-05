using Application.DTOs.ExpenseDTOs;
using FluentValidation;

namespace Application.Validators
{
    public class EditExpenseCategoryDTOValidator : AbstractValidator<EditExpenseCategoryDTO>
    {
        public EditExpenseCategoryDTOValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .Length(5, 100)
                .WithMessage("Name length should be between 5 and 100 characters.");

            RuleFor(x => x.PlannedBudget)
                .GreaterThan(0)
                .WithMessage("Planned budget must be greater than 0.")
                .LessThan(100000000)
                .WithMessage("Planned budget must be lower than 100000000.");
        }
    }
}

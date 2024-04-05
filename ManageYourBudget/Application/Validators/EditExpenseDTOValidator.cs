using Application.DTOs.ExpenseDTOs;
using FluentValidation;

namespace Application.Validators
{
    public class EditExpenseDTOValidator : AbstractValidator<EditExpenseDTO>
    {
        public EditExpenseDTOValidator()
        {
            RuleFor(x => x.ExpenseName)
                .NotEmpty()
                .WithMessage("Expense name cannot be empty.")
                .Length(5, 100)
                .WithMessage("Expense name length should be between 5 and 100 characters.");

            RuleFor(x => x.Amount)
                .GreaterThan(0)
                .WithMessage("Expense amount must be greater than 0.")
                .LessThan(100000000)
                .WithMessage("Expense amount must be lower than 100000000.");
        }
    }
}

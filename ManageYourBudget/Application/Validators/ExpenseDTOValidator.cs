using Application.DTOs.ExpenseDTOs;
using FluentValidation;

namespace Application.Validators
{
    public class ExpenseDTOValidator : AbstractValidator<ExpenseDTO>
    {
        public ExpenseDTOValidator()
        {
            RuleFor(x => x.ExpenseName)
                .NotEmpty()
                .Length(2, 100)
                .WithMessage("Title length should be between 2 and 100 characters.");

            RuleFor(x => x.Amount)
                .GreaterThan(0)
                .WithMessage("Expense amount must be greater than 0.")
                .LessThan(100000000)
                .WithMessage("Expense amount must be lower than 100000000.");
        }
    }
}
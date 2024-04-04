using FluentValidation;

namespace Application.DTOs.Validators
{
    public class ExpenseDTOValidator : AbstractValidator<ExpenseDTO>
    {
        public ExpenseDTOValidator()
        {
            this.RuleFor(x => x.ExpenseName)
                .NotEmpty()
                .Length(5, 100)
                .WithMessage("Title length should be between 5 and 100 characters.");

            this.RuleFor(x => x.Amount)
                .GreaterThan(0)
                .WithMessage("Expense amount must be greater than 0.")
                .LessThan(100000000)
                .WithMessage("Expense amount must be lower than 100000000.");
        }
    }
}
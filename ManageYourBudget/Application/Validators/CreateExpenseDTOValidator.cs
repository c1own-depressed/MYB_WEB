using Application.DTOs.ExpenseDTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators
{
    public class CreateExpenseDTOValidator : AbstractValidator<CreateExpenseDTO>
    {
        public CreateExpenseDTOValidator() {
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

using Application.DTOs.IncomeDTOs;
using FluentValidation;

namespace Application.Validators
{
    public class EditIncomeDTOValidator : AbstractValidator<EditIncomeDTO>
    {
        public EditIncomeDTOValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .Length(5, 100)
                .WithMessage("Name length should be between 5 and 100 characters.");

            RuleFor(x => x.Amount)
                .GreaterThan(0)
                .WithMessage("Amount must be greater than 0.")
                .LessThan(100000000)
                .WithMessage("Amount must be lower than 100000000.");
        }
    }
}

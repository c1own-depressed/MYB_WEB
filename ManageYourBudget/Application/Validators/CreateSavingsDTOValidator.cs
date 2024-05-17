using Application.DTOs.SavingsDTOs;
using FluentValidation;

namespace Application.Validators
{
    public class CreateSavingsDTOValidator : AbstractValidator<CreateSavingsDTO>
    {
        public CreateSavingsDTOValidator()
        {
            RuleFor(x => x.SavingsName)
                .NotEmpty()
                .Length(2, 100)
                .WithMessage("Title length should be between 2 and 100 characters.");

            RuleFor(x => x.Amount)
                .GreaterThan(0)
                .WithMessage("Savings amount must be greater than 0.")
                .LessThan(100000000)
                .WithMessage("Savings amount must be lower than 100000000.");
        }
    }
}

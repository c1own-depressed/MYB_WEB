using Application.DTOs.IncomeDTOs;
using FluentValidation;

namespace Application.Validators
{
    public class IncomeDTOValidator : AbstractValidator<IncomeDTO>
    {
        public IncomeDTOValidator()
        {
            RuleFor(x => x.IncomeName)
                .NotEmpty()
                .Length(1, 100)
                .WithMessage("Title length should be between 1 and 100 characters.");

            RuleFor(x => x.Amount)
                .GreaterThan(0)
                .WithMessage("Planned budget must be greater than 0.")
                .LessThan(100000000)
                .WithMessage("Planned budget must be lower than 100000000.");
        }
    }
}
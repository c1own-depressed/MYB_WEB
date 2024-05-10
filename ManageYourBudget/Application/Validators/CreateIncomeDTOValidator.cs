
using Application.DTOs.IncomeDTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators
{
    public class CreateIncomeDTOValidator : AbstractValidator<CreateIncomeDTO>
    {
        public CreateIncomeDTOValidator() {
            RuleFor(x => x.IncomeName)
                   .NotEmpty()
                   .Length(2, 100)
                   .WithMessage("Title length should be between 2 and 100 characters.");

            RuleFor(x => x.Amount)
                .GreaterThan(0)
                .WithMessage("Planned budget must be greater than 0.")
                .LessThan(100000000)
                .WithMessage("Planned budget must be lower than 100000000.");
        }
    }
}

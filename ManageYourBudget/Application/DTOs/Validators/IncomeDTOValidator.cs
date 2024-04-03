﻿using FluentValidation;

namespace Application.DTOs.Validators
{
    public class IncomeDTOValidator : AbstractValidator<IncomeDTO>
    {
        public IncomeDTOValidator()
        {
            this.RuleFor(x => x.IncomeName)
                .NotEmpty()
                .Length(5, 100)
                .WithMessage("Title length should be between 5 and 100 characters.");

            this.RuleFor(x => x.Amount)
                .GreaterThan(0)
                .WithMessage("Planned budget must be greater than 0.")
                .LessThan(100000000)
                .WithMessage("Planned budget must be lower than 100000000.");
        }
    }
}
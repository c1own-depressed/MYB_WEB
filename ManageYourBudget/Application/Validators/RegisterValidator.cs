using Application.DTOs;
using Domain.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators
{
    public class RegisterValidator : AbstractValidator<RegisterDTO>
    {
        private readonly UserManager<User> _userManager;

        public RegisterValidator(UserManager<User> userManager)
        {
            _userManager = userManager;

            RuleFor(model => model.Username)
                .NotEmpty().WithMessage("Username is required")
                .MustAsync(BeUniqueUsername).WithMessage("Username is already taken");

            RuleFor(model => model.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email address")
                .MustAsync(BeUniqueEmail).WithMessage("Email is already registered");

            RuleFor(model => model.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long");
        }

        private async Task<bool> BeUniqueUsername(string username, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByNameAsync(username);
            return user == null;
        }

        private async Task<bool> BeUniqueEmail(string email, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return user == null;
        }
    }
}

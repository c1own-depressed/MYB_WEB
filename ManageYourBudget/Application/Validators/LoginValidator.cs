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
    public class LoginValidator : AbstractValidator<LoginDTO>
    {
        private readonly SignInManager<User> _signInManager;

        public LoginValidator(SignInManager<User> signInManager)
        {
            _signInManager = signInManager;

            RuleFor(model => model.Username)
                .NotEmpty().WithMessage("Username is required")
                .MustAsync((model, username, cancellationToken) => BeValidUser(username, model.Password, cancellationToken))
                .WithMessage("Invalid username or password");

            RuleFor(model => model.Password)
                .NotEmpty().WithMessage("Password is required");
        }

        private async Task<bool> BeValidUser(string username, string password, CancellationToken cancellationToken)
        {
            var user = await _signInManager.UserManager.FindByNameAsync(username);
            if (user != null)
            {
                return await _signInManager.CheckPasswordSignInAsync(user, password, false) == SignInResult.Success;
            }
            return false;
        }
    }
}

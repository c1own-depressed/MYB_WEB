using Application.DTOs.AccountDTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Persistence.AuthService;
using System.Text.Encodings.Web;
using WebApp.Models;
using Domain.Entities;
using NuGet.Common;
using System.Security.Policy;

namespace WebApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly IAuthService _authService;

        public AccountController(ILogger<AccountController> logger, IAuthService authService)
        {
            _logger = logger;
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userLoginDTO = new UserLoginDTO
                {
                    Email = model.Email,
                    Password = model.Password,
                };

                var result = await _authService.AuthenticateUserAsync(userLoginDTO);

                if (result != null)
                {
                    _logger.LogInformation("User logged in successfully.");
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userRegisterationDTO = new UserRegistrationDTO
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    Password = model.Password,
                };

                var result = await _authService.RegisterUserAsync(userRegisterationDTO);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User registered successfully.");
                    return RedirectToAction("checkemail");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }


        [HttpGet]
        public IActionResult ResetPassword(string token, string email)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
            {
                // Display an error or redirect
                ModelState.AddModelError(string.Empty, "Invalid password reset token or email.");
                return View("Error"); // Or any appropriate view to display the error
            }

            var model = new ResetPasswordViewModel { Token = token, Email = email };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken] // Helps prevent CSRF attacks
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var resetPasswordDTO = new ResetPasswordDTO
            {
                Email = model.Email,
                Password = model.Password,
                ConfirmPassword = model.ConfirmPassword,
                Token = model.Token
            };

            var result = await _authService.ResetPasswordAsync(resetPasswordDTO);
            if (result.Succeeded)
            {
                _logger.LogInformation("Password reset successfully.");
                // Redirect to a confirmation page or login page
                return RedirectToAction("Login");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        public IActionResult CheckEmail()
        {
            return View();
        }

        public IActionResult ConfirmEmail()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _authService.ForgotPasswordAsync(model.Email);
            if (result.Succeeded)
            {
                return RedirectToAction("CheckEmail");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }
    }
}

using Application.DTOs.AccountDTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;

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

        public IActionResult CheckEmail()
        {
            return View();
        }

        public IActionResult ConfirmEmail()
        {
            return View();
        }
    }
}

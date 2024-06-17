using System.Diagnostics;
using System.Security.Claims;
using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;

namespace WebApp.Controllers
{
    public class SettingsPageController : Controller
    {
        private readonly ISettingsService _settingsService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public SettingsPageController(ISettingsService settingsService, IHttpContextAccessor httpContextAccessor, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _settingsService = settingsService;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> Index()
        {
            string userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userSettings = await _settingsService.GetUserSettingsAsync(userId);

            var model = new SettingsViewModel
            {
                Language = userSettings?.Language,
                IsLightTheme = userSettings?.IsLightTheme ?? false,
                Currency = userSettings?.Currency,
                UserName = User.Identity.Name // Отримання UserName з Claims
            };

            return View("~/Views/SettingsPage/Index.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> SaveSettings(SettingsViewModel model)
        {
            try
            {
                string userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                SettingsDTO settingsDTO = new SettingsDTO
                {
                    Id = userId,
                    Language = model.Language,
                    Currency = model.Currency,
                    IsLightTheme = model.IsLightTheme,
                };

                await _settingsService.SaveSettings(settingsDTO);

                // Оновлення UserName у таблиці aspnetusers
                if (!string.IsNullOrEmpty(model.UserName))
                {
                    var user = await _userManager.FindByIdAsync(userId);
                    if (user != null)
                    {
                        user.UserName = model.UserName;
                        var result = await _userManager.UpdateAsync(user);

                        if (result.Succeeded)
                        {
                            // Оновлення ClaimsPrincipal
                            var identity = (ClaimsIdentity)_httpContextAccessor.HttpContext.User.Identity;
                            var nameClaim = identity.FindFirst(ClaimTypes.Name);
                            if (nameClaim != null)
                            {
                                identity.RemoveClaim(nameClaim);
                                identity.AddClaim(new Claim(ClaimTypes.Name, model.UserName));
                            }
                            else
                            {
                                identity.AddClaim(new Claim(ClaimTypes.Name, model.UserName));
                            }

                            // Оновлення куків автентифікації
                            await _signInManager.SignOutAsync();
                            await _signInManager.SignInAsync(user, isPersistent: false);
                        }
                        else
                        {
                            // Обробка помилок оновлення
                            ModelState.AddModelError(string.Empty, "Failed to update user name.");
                            return View("~/Views/SettingsPage/Index.cshtml", model);
                        }
                    }
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Логування помилки
                Debug.WriteLine($"Error: {ex.Message}");
                return RedirectToAction("Index");
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

using System.Diagnostics;
using System.Security.Claims;
using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;

namespace WebApp.Controllers
{
    public class SettingsPageController : Controller
    {
        private readonly ISettingsService _settingsService;
        private readonly IThemeService _themeService;
        private string? selectedLanguage;
        private bool selectedTheme;
        private string? selectedCurrency;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SettingsPageController(ISettingsService settingsService, IHttpContextAccessor httpContextAccessor, IThemeService themeService)
        {
            _settingsService = settingsService;
            _httpContextAccessor = httpContextAccessor;
            _themeService = themeService;
        }

        public async Task<IActionResult> Index()
        {
            string userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userSettings = await _settingsService.GetUserSettingsAsync(userId);

            if (userSettings != null)
            {
                var model = new SettingsViewModel
                {
                    Language = userSettings.Language,
                    IsLightTheme = userSettings.IsLightTheme,
                    Currency = userSettings.Currency,
                };
                ViewBag.Theme = userSettings.IsLightTheme ? "Light" : "Dark";

                return View("~/Views/SettingsPage/Index.cshtml", model);
            }

            return View("~/Views/SettingsPage/Index.cshtml");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public async Task<IActionResult> SaveSettings(SettingsViewModel model)
        {
            try
            {
                string userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                selectedLanguage = model.Language;
                selectedTheme = model.IsLightTheme;
                selectedCurrency = model.Currency;

                SettingsDTO settingsDTO = new SettingsDTO
                {
                    Id = userId,
                    Language = selectedLanguage,
                    Currency = selectedCurrency,
                    IsLightTheme = selectedTheme,
                };

                await _settingsService.SaveSettings(settingsDTO);
                await _themeService.SaveUserThemeAsync(userId, model.IsLightTheme);
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return RedirectToAction("Index");
            }
        }
    }
}

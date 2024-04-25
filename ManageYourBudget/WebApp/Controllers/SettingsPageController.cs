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
        private readonly ILogger<SettingsPageController> _logger;
        private readonly ISettingsService _settingsService;
        private string? selectedLanguage;
        private bool selectedTheme;
        private string? selectedCurrency;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SettingsPageController(ILogger<SettingsPageController> logger, ISettingsService settingsService, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _settingsService = settingsService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IActionResult> Index()
        {
            string userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            try
            {
                var userSettings = await _settingsService.GetUserSettingsAsync(userId);

                if (userSettings != null)
                {
                    var model = new SettingsViewModel
                    {
                        Language = userSettings.Language,
                        IsLightTheme = userSettings.IsLightTheme,
                        Currency = userSettings.Currency,
                    };

                    this._logger.LogInformation($"User accessed SettingsPage with pre-filled settings. Currency: {model.Currency}");

                    return this.View("~/Views/SettingsPage/Index.cshtml", model);
                }

                this._logger.LogInformation("User accessed SettingsPage without pre-filled settings.");
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "An error occurred while fetching user settings.");
            }

            return this.View("~/Views/SettingsPage/Index.cshtml");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return this.View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public async Task<IActionResult> SaveSettings(SettingsViewModel model)
        {
            try
            {
                string userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                this.selectedLanguage = model.Language;
                this.selectedTheme = model.IsLightTheme;
                this.selectedCurrency = model.Currency;

                string id = userId;
                SettingsDTO settingsDTO = new SettingsDTO
                {
                    Id = id,
                    Language = this.selectedLanguage,
                    Currency = this.selectedCurrency,
                    IsLightTheme = this.selectedTheme,
                };

                await this._settingsService.SaveSettings(settingsDTO);
                this._logger.LogInformation("User settings saved successfully.");
                return this.RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "An error occurred while saving user settings.");
                return this.RedirectToAction("Index");
            }
        }
    }
}

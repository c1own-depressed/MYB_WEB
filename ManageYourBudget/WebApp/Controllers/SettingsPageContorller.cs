using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging; // Додано простір імен для ILogger
using System.Diagnostics;
using System.Threading.Tasks;
using WebApp.Models;
using Application.DTOs;
using Application.Interfaces;
using Application.Services;

namespace WebApp.Controllers
{
    public class SettingsController : Controller
    {
        private readonly ILogger<SettingsController> _logger;
        private readonly ISettingsService _settingsService;
        private string selectedLanguage;
        private bool selectedTheme;
        private string selectedCurrency;

        public SettingsController(ILogger<SettingsController> logger, ISettingsService settingsService)
        {
            _logger = logger;
            _settingsService = settingsService;
        }

        public async Task<IActionResult> Index()
        {
            int userId = 1;
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
                this.selectedLanguage = model.Language;
                this.selectedTheme = model.IsLightTheme;
                this.selectedCurrency = model.Currency;

                SettingsDTO settingsDTO = new SettingsDTO
                {
                    Language = this.selectedLanguage,
                    Currency = this.selectedCurrency,
                    IsLightTheme = this.selectedTheme,
                };

                settingsDTO.Id = 1;
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

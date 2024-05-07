using System.Diagnostics;
using System.Security.Claims;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;

namespace WebApp.Controllers
{
    public class FAQPageController : Controller
    {
        private readonly ILogger<FAQPageController> _logger;
        private readonly ISettingsService _settingsService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public FAQPageController(ILogger<FAQPageController> logger, ISettingsService settingsService, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _settingsService = settingsService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IActionResult> Index()
        {
            
            string userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId != null)
            {
                var temp = await _settingsService.GetUserSettingsAsync(userId);
                bool IsLight_Theme = temp.IsLightTheme;
                ViewBag.Theme = IsLight_Theme ? "Light" : "Dark";

                _logger.LogInformation("User accessed FAQPage.");
                return View("~/Views/FAQPage/Index.cshtml");
            }
            else
            {
                _logger.LogInformation("User accessed FAQPage.");
                return View("~/Views/FAQPage/Index.cshtml");
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

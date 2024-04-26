using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebApp.Controllers
{
    public class TipsController : Controller
    {
        private readonly ILogger<FAQPageController> _logger;
        private readonly ISettingsService _settingsService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TipsController(ILogger<FAQPageController> logger, ISettingsService settingsService, IHttpContextAccessor httpContextAccessor)
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
                var tips = GetTipsFromSomewhere();
                var tricks = GetTricksFromSomewhere();
                var model = new Tuple<List<string>, List<string>>(tips, tricks);
                return View("~/Views/TipsTricksPage/Index.cshtml", model);
            }
            else
            {
                var tips = GetTipsFromSomewhere();
                var tricks = GetTricksFromSomewhere();
                var model = new Tuple<List<string>, List<string>>(tips, tricks);
                return View("~/Views/TipsTricksPage/Index.cshtml", model);
            }
        }

        private List<string> GetTipsFromSomewhere()
        {
            return new List<string>
            {
                "Tip 1",
                "Tip 2",
                "Tip 3",
            };
        }

        private List<string> GetTricksFromSomewhere()
        {
            return new List<string>
            {
                "https://www.anaplan.com/blog/zbb-zero-based-budgeting-guide/",
                "https://www.nerdwallet.com/article/finance/zero-based-budgeting-explained",
                "https://www.anaplan.com/blog/zbb-zero-based-budgeting-guide/",
            };
        }
    }
}

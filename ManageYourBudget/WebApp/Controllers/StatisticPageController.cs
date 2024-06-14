using System.Security.Claims;
using System.Text;
using Application.DTOs.StatisticDTO;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using Application.Utils;

namespace WebApp.Controllers
{
    public class StatisticPageController : Controller
    {
        private readonly IStatisticService _statisticService;
        private readonly IExportDataService _exportDataService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISettingsService _settingsService;

        public StatisticPageController(IStatisticService statisticService, IHttpContextAccessor httpContextAccessor, ISettingsService settingsService, IExportDataService exportDataService)
        {
            _statisticService = statisticService;
            _httpContextAccessor = httpContextAccessor;
            _settingsService = settingsService;
            _exportDataService = exportDataService;
        }

        //public enum ExportFormat
        //{
        //    CSV,
        //    XML,
        //    XLSX
        //}

        public async Task<IActionResult> Index()
        {
            string userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var temp = await _settingsService.GetUserSettingsAsync(userId);
            bool isLightTheme = temp.IsLightTheme;
            ViewBag.Theme = isLightTheme ? "Light" : "Dark";

            //string aspNetCoreCookiesValue = _httpContextAccessor.HttpContext.Request.Cookies[".AspNetCore.Cookies"];

            try
            {
                var defaultStatistics = await _statisticService.GetAllData(DateTime.Now.AddMonths(-1), DateTime.Now, userId);

                List<StatisticViewModel> statistics = new List<StatisticViewModel>();

                for (int i = 0; i < defaultStatistics?.ExpensesStatistics?.Count; i++)
                {
                    var income = defaultStatistics?.IncomeStatistics?[i] ?? new IncomeStatisticDTO();
                    var expense = defaultStatistics?.ExpensesStatistics?[i] ?? new TotalExpensesDTO();
                    var saving = defaultStatistics?.SavingsStatistics?[i] ?? new SavedStatisticDTO();

                    var model = new StatisticViewModel
                    {
                        Incomes = income.TotalAmount,
                        SummaryExpenses = expense.TotalAmount,
                        Savings = saving.TotalAmount,
                        Date = income.Month,
                    };

                    statistics.Add(model);
                }

                return View("~/Views/StatisticPage/Index.cshtml", statistics);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Failed to retrieve default statistics.");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetStatistics(DateTime startDate, DateTime endDate, string category)
        {
            try
            {
                string userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var statistics = await _statisticService.GetAllData(startDate, endDate, userId);

                List<StatisticViewModel> viewModelList = new List<StatisticViewModel>();

                for (int i = 0; i < statistics?.ExpensesStatistics?.Count; i++)
                {
                    var income = statistics?.IncomeStatistics?[i] ?? new IncomeStatisticDTO();
                    var expense = statistics?.ExpensesStatistics?[i] ?? new TotalExpensesDTO();
                    var saving = statistics?.SavingsStatistics?[i] ?? new SavedStatisticDTO();

                    var model = new StatisticViewModel
                    {
                        Incomes = income.TotalAmount,
                        SummaryExpenses = expense.TotalAmount,
                        Savings = saving.TotalAmount,
                        Date = income.Month,
                    };

                    viewModelList.Add(model);
                }

                return View("~/Views/StatisticPage/Index.cshtml", viewModelList); // Pass view models to the view
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Failed to retrieve statistics: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> ExportStatistics(DateTime startDate, DateTime endDate, ExportFormat format)
        {
            string userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _exportDataService.ExportData(startDate, endDate, userId, format);

            if (!result.Success)
                return StatusCode(500, "Failed to export data.");

            return File(result.FileContents, result.ContentType, result.FileName);
        }
    }
}

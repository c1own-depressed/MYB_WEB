using System.Security.Claims;
using System.Text;
using Application.DTOs.StatisticDTO;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;

namespace WebApp.Controllers
{
    public class StatisticPageController : Controller
    {
        private readonly ILogger<StatisticPageController> _logger;
        private readonly IStatisticService _statisticService;
        private readonly IExportDataService _exportDataService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISettingsService _settingsService;

        public StatisticPageController(ILogger<StatisticPageController> logger, IStatisticService statisticService, IHttpContextAccessor httpContextAccessor, ISettingsService settingsService, IExportDataService exportDataService)
        {
            _logger = logger;
            _statisticService = statisticService;
            _httpContextAccessor = httpContextAccessor;
            _settingsService = settingsService;
            _exportDataService = exportDataService;
        }

        public enum ExportFormat
        {
            CSV,
            XML,
            XLSX
        }

        public async Task<IActionResult> Index()
        {
            string userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            string aspNetCoreCookiesValue = _httpContextAccessor.HttpContext.Request.Cookies[".AspNetCore.Cookies"];
            try
            {
                _logger.LogInformation("User accessed StatisticPage.");

                var defaultStatistics = await _statisticService.GetAllData(DateTime.Now.AddMonths(-1), DateTime.Now, userId);

                List<StatisticViewModel> statistics = new List<StatisticViewModel>();

                for (int i = 0; i < defaultStatistics?.ExpensesStatistics?.Count; i++)
                {
                    var income = defaultStatistics?.IncomeStatistics?[i];
                    var expense = defaultStatistics?.ExpensesStatistics?[i];
                    var saving = defaultStatistics?.SavingsStatistics?[i];

                    income ??= new IncomeStatisticDTO();
                    expense ??= new TotalExpensesDTO();
                    saving ??= new SavedStatisticDTO();

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
                _logger.LogError(ex, "Failed to retrieve default statistics.");
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
                    var income = statistics?.IncomeStatistics?[i];
                    var expense = statistics?.ExpensesStatistics?[i];
                    var saving = statistics?.SavingsStatistics?[i];

                    income ??= new IncomeStatisticDTO();
                    expense ??= new TotalExpensesDTO();
                    saving ??= new SavedStatisticDTO();

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
            try
            {
                byte[] fileContents;
                string contentType;
                string fileName;

                switch (format)
                {
                    case ExportFormat.CSV:
                        var csvData = await _exportDataService.ExportDataToCSV(startDate, endDate, userId);
                        fileContents = Encoding.UTF8.GetBytes(csvData);
                        contentType = "text/csv";
                        fileName = "statistics.csv";
                        break;
                    case ExportFormat.XML:
                        fileContents = await _exportDataService.ExportDataToXML(startDate, endDate, userId);
                        contentType = "application/xml";
                        fileName = "statistics.xml";
                        break;
                    case ExportFormat.XLSX:
                        fileContents = await _exportDataService.ExportDataToXLSX(startDate, endDate, userId);
                        contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        fileName = "statistics.xlsx";
                        break;
                    default:
                        return BadRequest("Unsupported format");
                }

                return File(fileContents, contentType, fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to export data.");
                return StatusCode(500, "Failed to export data.");
            }
        }
    }
}

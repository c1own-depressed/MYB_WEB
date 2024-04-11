using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using WebApp.Models;

namespace WebApp.Controllers
{
    public class StatisticPageController : Controller
    {
        private readonly ILogger<StatisticPageController> _logger;
        private readonly IStatisticService _statisticService;

        public StatisticPageController(ILogger<StatisticPageController> logger, IStatisticService statisticService)
        {
            _logger = logger;
            _statisticService = statisticService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                _logger.LogInformation("User accessed StatisticPage.");

                // Replace '1' with actual userId
                var defaultStatistics = await _statisticService.GetAllData(DateTime.Now.AddMonths(-1), DateTime.Now, 1);

                // Initialize the list to hold multiple StatisticViewModel instances
                List<StatisticViewModel> statistics = new List<StatisticViewModel>();

                // Populate view models from defaultStatistics
                foreach (var income in defaultStatistics.IncomeStatistics)
                {
                    var model = new StatisticViewModel
                    {
                        Incomes = income.TotalAmount,
                        Date = income.Month,

                    };

                    statistics.Add(model);
                }

                foreach (var expense in defaultStatistics.ExpensesStatistics)
                {
                    var model = new StatisticViewModel
                    {
                        SummaryExpenses = expense.TotalAmount
                    };

                    statistics.Add(model);
                }

                foreach (var savings in defaultStatistics.SavingsStatistics)
                {
                    var model = new StatisticViewModel
                    {
                        Savings = savings.TotalAmount
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

        [HttpPost]
        public async Task<IActionResult> GetStatistics(DateTime startDate, DateTime endDate, string category)
        {
            try
            {
                // Assuming 'category' is used to filter statistics (e.g., expenses by category)
                // You can adjust this logic based on your application's requirements

                var statistics = await _statisticService.GetAllData(startDate, endDate, 1); // Replace '1' with actual userId

                // Initialize the list to hold multiple StatisticViewModel instances
                List<StatisticViewModel> viewModelList = new List<StatisticViewModel>();

                // Populate view models from statistics
                foreach (var income in statistics.IncomeStatistics)
                {
                    var model = new StatisticViewModel
                    {
                        Incomes = income.TotalAmount
                    };

                    viewModelList.Add(model);
                }

                foreach (var expense in statistics.ExpensesStatistics)
                {
                    var model = new StatisticViewModel
                    {
                        SummaryExpenses = expense.TotalAmount
                    };

                    viewModelList.Add(model);
                }

                foreach (var saving in statistics.SavingsStatistics)
                {
                    var model = new StatisticViewModel
                    {
                        Savings = saving.TotalAmount
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
    }
}

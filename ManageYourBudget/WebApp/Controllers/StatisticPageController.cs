using Application.Interfaces;
using Domain.Entities;
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
                for (int i = 0; i < defaultStatistics.ExpensesStatistics.Count; i++)
                {
                    var income = defaultStatistics.IncomeStatistics[i];
                    var expense = defaultStatistics.ExpensesStatistics[i];
                    var saving = defaultStatistics.SavingsStatistics[i];

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

                for (int i = 0; i < statistics.ExpensesStatistics.Count; i++)
                {
                    var income = statistics.IncomeStatistics[i];
                    var expense = statistics.ExpensesStatistics[i];
                    var saving = statistics.SavingsStatistics[i];

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
    }
}

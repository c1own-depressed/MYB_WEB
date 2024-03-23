using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebApp.Models;

namespace WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IExpenseCategoryService _expenseCategoryService;
        private readonly IIncomeService _incomeService;

        public HomeController(ILogger<HomeController> logger, IExpenseCategoryService expenseCategoryService, IIncomeService incomeService)
        {
            _logger = logger;
            _expenseCategoryService = expenseCategoryService;
            _incomeService = incomeService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = 1;
            var categories = await _expenseCategoryService.GetExpenseCategoriesByUserIdAsync(userId);
            var incomes = await _incomeService.GetIncomesByUserIdAsync(userId);

            var model = new HomeViewModel
            {
                Categories = categories,
                Incomes = incomes,
                //Savings =                 
            };
            _logger.LogError("Home page opened");
            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public async Task<IActionResult> AddExpenseCategory([FromBody] CreateExpenseCategoryDTO model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state for AddExpenseCategory");
                return BadRequest(ModelState);
            }

            var (isSuccess, errorMessage) = await _expenseCategoryService.AddExpenseCategoryAsync(model);

            if (isSuccess)
            {
                _logger.LogInformation($"Category added: {model.Title} with budget {model.PlannedBudget}");
                return Ok();
            }
            else
            {
                _logger.LogError($"Failed to add category: {errorMessage}");
                return BadRequest(errorMessage);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddIncome([FromBody] IncomeDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var (isSuccess, errorMessage) = await _incomeService.AddIncomeAsync(model);

            if (isSuccess)
            {
                _logger.LogError($"Income added: {model.IncomeName} with budget {model.Amount}");
                return Ok();
            }
            else
            {
                _logger.LogError($"Failed to add income: {errorMessage}");
                return BadRequest(errorMessage);
            }
        }

    }
}

using Application.DTOs;
using Application.Interfaces;
using Application.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebApp.Models;

namespace WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IExpenseCategoryService _expenseCategoryService;

        public HomeController(ILogger<HomeController> logger, IExpenseCategoryService expenseCategoryService)
        {
            _logger = logger;
            _expenseCategoryService = expenseCategoryService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = 1;
            var categories = await _expenseCategoryService.GetExpenseCategoriesByUserIdAsync(userId);

            var model = new HomeViewModel
            {
                Categories = categories,
                
                //Income = ,
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
            if (!isSuccess)
            {
                _logger.LogError($"Failed to add category: {errorMessage}");
                return BadRequest(errorMessage);
            }

            _logger.LogInformation($"Category added: {model.Title} with budget {model.PlannedBudget}");
            return Ok();
        }
    }
}

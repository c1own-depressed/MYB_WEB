using Application.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebApp.Models;

namespace WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ExpenseCategoryService _expenseCategoryService;

        public HomeController(ILogger<HomeController> logger, ExpenseCategoryService expenseCategoryService)
        {
            _logger = logger;
            _expenseCategoryService = expenseCategoryService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = 1; // Assuming a hardcoded userId for demonstration. Replace with actual user's id.
            var categories = await _expenseCategoryService.GetExpenseCategoriesByUserIdAsync(userId);

            var model = new HomeViewModel
            {
                Categories = categories,
                
                Income = new IncomeViewModel 
                {
                    Name = "Main work"
                },
                Savings = new SavingsViewModel 
                {  
                    Name = "Travelling"
                }                
            };

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
    }
}

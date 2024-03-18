using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebApp.Models;

namespace WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var model = new HomeViewModel
            {
                Categories = new[]
                {
                    new CategoryViewModel
                    {
                        Name = "Restaurant",
                        PlannedBudget = 500,
                        RemainingBudget = 400
                    },
                    new CategoryViewModel
                    {
                        Name = "ATB",
                        PlannedBudget = 9900,
                        RemainingBudget = 8733
                    }
                },
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

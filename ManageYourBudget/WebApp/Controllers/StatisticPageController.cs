using Application.DTOs.IncomeDTOs;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
{
    public class StatisticPageController : Controller
    {
        private readonly ILogger<StatisticPageController> _logger;

        public StatisticPageController(ILogger<StatisticPageController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            _logger.LogInformation("User accessed StatisticPage.");
            return View("~/Views/StatisticPage/Index.cshtml");
        }

        //public getStatisticByDate()
        //{

        //}
    }
}

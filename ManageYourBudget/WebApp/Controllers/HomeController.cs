using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly LogController _logController;

        public HomeController(LogController logController)
        {
            _logController = logController;
        }

        public IActionResult Index()
        {
            // Виклик методу контролера LogControllers, який записує логи
            _logController.Index();

            return View();
        }
    }
}

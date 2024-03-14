using Microsoft.AspNetCore.Mvc;
using Serilog;


namespace WebApp.Controllers
{
    public class LogController : Controller
    {
        private readonly Serilog.ILogger _logger;

        public LogController()
        {
            _logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.Seq("http://localhost:5341")
                .CreateLogger();
        }

        public IActionResult Index()
        {
            // Приклад логування повідомлення рівня Information
            _logger.Information("Index method was called.");

            // Приклад логування повідомлення рівня Warning з параметрами
            _logger.Warning("Something unexpected happened in Index method. User: {UserId}", User.Identity.Name);

            return View();
        }
    }
}

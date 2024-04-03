using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
{
    public class TipsController : Controller
    {
        public IActionResult Index()
        {
            var tips = GetTipsFromSomewhere();
            var tricks = GetTricksFromSomewhere();
            var model = new Tuple<List<string>, List<string>>(tips, tricks);
            return View("~/Views/TipsTricksPage/Index.cshtml", model);
        }

        private List<string> GetTipsFromSomewhere()
        {
            return new List<string>
            {
                "Tip 1",
                "Tip 2",
                "Tip 3",
            };
        }

        private List<string> GetTricksFromSomewhere()
        {
            return new List<string>
            {
                "https://www.anaplan.com/blog/zbb-zero-based-budgeting-guide/",
                "https://www.nerdwallet.com/article/finance/zero-based-budgeting-explained",
                "https://www.anaplan.com/blog/zbb-zero-based-budgeting-guide/",
            };
        }
    }
}

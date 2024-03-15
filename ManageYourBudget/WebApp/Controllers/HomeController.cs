//-----------------------------------------------------------------------
// <copyright file="HomeController.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace WebApp.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Controller for managing home page.
    /// </summary>
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            _logger.LogInformation("Index method was called.");
            return View();
        }
    }
}

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
        private readonly LogController logController;

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeController"/> class.
        /// Constructor for HomeController.
        /// </summary>
        /// <param name="logController">The log controller instance.</param>
        public HomeController(LogController logController)
        {
            this.logController = logController;
        }

        /// <summary>
        /// Action method for rendering the home page.
        /// </summary>
        /// <returns>ViewResult representing the home page view.</returns>
        public IActionResult Index()
        {
            // Call the method of LogController which logs.
            this.logController.Index();

            return this.View();
        }
    }
}

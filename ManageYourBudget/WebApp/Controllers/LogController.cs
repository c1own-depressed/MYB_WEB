// <copyright file="LogController.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
namespace WebApp.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Serilog;

    /// <summary>
    /// Controller for logging activities.
    /// </summary>
    public class LogController : Controller
    {
        private readonly Serilog.ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogController"/> class.
        /// Constructor for LogController.
        /// </summary>
        public LogController()
        {
            // Configure Serilog logger
            this.logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.Seq("http://localhost:5341")
                .CreateLogger();
        }

        /// <summary>
        /// Action method for logging activities.
        /// </summary>
        /// <returns>ViewResult.</returns>
        public IActionResult Index()
        {
            // Example logging of an Information level message
            this.logger.Information("Index method was called.");

            // Example logging of a Warning level message with parameters
            this.logger.Warning("Something unexpected happened in Index method. User: {UserId}", this.User.Identity?.Name);

            return this.View();
        }
    }
}

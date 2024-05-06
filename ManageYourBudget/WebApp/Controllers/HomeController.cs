using System.Diagnostics;
using System.Security.Claims;
using Application.DTOs.ExpenseDTOs;
using Application.DTOs.IncomeDTOs;
using Application.DTOs.SavingsDTOs;
using Application.Interfaces;
using Application.Services;
using Application.Utils;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;

namespace WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IExpenseCategoryService _expenseCategoryService;
        private readonly IIncomeService _incomeService;
        private readonly ISavingsService _savingsService;
        private readonly IExpenseService _expenseService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISettingsService _settingsService;

        public HomeController(
            IExpenseCategoryService expenseCategoryService,
            IIncomeService incomeService,
            ISavingsService savingsService,
            IExpenseService expenseService,
            IHttpContextAccessor httpContextAccessor,
            ISettingsService settingsService)
        {
            _expenseCategoryService = expenseCategoryService;
            _incomeService = incomeService;
            _savingsService = savingsService;
            _expenseService = expenseService;
            _httpContextAccessor = httpContextAccessor;
            _settingsService = settingsService;
        }

        public async Task<IActionResult> Index()
        {
        if (!User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Login", "Account"); // Перенаправлення на сторінку входу
        }
        else
        {
            string userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var temp = await _settingsService.GetUserSettingsAsync(userId);
            bool IsLight_Theme = temp.IsLightTheme;
            ViewBag.Theme = IsLight_Theme ? "Light" : "Dark";
            var categories = await _expenseCategoryService.GetExpenseCategoriesByUserIdAsync(userId);
            var incomes = await _incomeService.GetIncomesByUserIdAsync(userId);
            var savings = await _savingsService.GetSavingsByUserIdAsync(userId);
            var model = new HomeViewModel
            {
                Categories = categories,
                Incomes = incomes,
                Savings = savings,
            };
            return View(model);
        }
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
            string userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!ModelState.IsValid)
            {

                return BadRequest(ModelState);
            }

            ServiceResult serviceResult = await _expenseCategoryService.AddExpenseCategoryAsync(model, userId);

            if (serviceResult.Success)
            {

                return Ok();
            }
            else
            {
                var errorMessages = string.Join("; ", serviceResult.Errors);
                return BadRequest(errorMessages);
            }
        }

        [HttpPost]
        public async Task<IActionResult> RemoveExpenseCategory(string categoryId)
        {
            ServiceResult serviceResult = await _expenseCategoryService.RemoveExpenseCategoryAsync(categoryId);

            if (serviceResult.Success)
            {

                return Ok();
            }
            else
            {
                var errorMessages = string.Join("; ", serviceResult.Errors);

                return BadRequest(errorMessages);
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditExpenseCategory([FromBody] EditExpenseCategoryDTO model)
        {
            if (!ModelState.IsValid)
            {

                return BadRequest(ModelState);
            }

            ServiceResult serviceResult = await _expenseCategoryService.EditExpenseCategoryAsync(model);

            if (serviceResult.Success)
            {

                return Ok();
            }
            else
            {
                var errorMessages = string.Join("; ", serviceResult.Errors);

                return BadRequest(errorMessages);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddIncome([FromBody] CreateIncomeDTO model)
        {
            string userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            ServiceResult serviceResult = await _incomeService.AddIncomeAsync(model, userId);

            if (serviceResult.Success)
            {
                return Ok();
            }
            else
            {
                var errorMessages = string.Join("; ", serviceResult.Errors);

                return BadRequest(errorMessages);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddSavings([FromBody] CreateSavingsDTO model)
        {
            string userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ServiceResult serviceResult = await _savingsService.AddSavingsAsync(model, userId);

            if (serviceResult.Success)
            {

                return Ok();
            }
            else
            {
                var errorMessages = string.Join("; ", serviceResult.Errors);

                return BadRequest(errorMessages);
            }
        }

        [HttpPost]
        public async Task<IActionResult> RemoveSavings(string savingsId)
        {
            ServiceResult serviceResult = await _savingsService.RemoveSavingsAsync(savingsId);

            if (serviceResult.Success)
            {

                return Ok();
            }
            else
            {
                var errorMessages = string.Join("; ", serviceResult.Errors);

                return BadRequest(errorMessages);
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditSavings([FromBody] EditSavingsDTO model)
        {
            if (!ModelState.IsValid)
            {

                return BadRequest(ModelState);
            }

            ServiceResult serviceResult = await _savingsService.EditSavingsAsync(model);

            if (serviceResult.Success)
            {

                return Ok();
            }
            else
            {
                var errorMessages = string.Join("; ", serviceResult.Errors);

                return BadRequest(errorMessages);
            }
        }

        public async Task<IActionResult> EditIncome([FromBody] EditIncomeDTO model)
        {
            if (!this.ModelState.IsValid)
            {

                return this.BadRequest(this.ModelState);
            }

            ServiceResult serviceResult = await this._incomeService.EditIncomeAsync(model);

            if (serviceResult.Success)
            {

                return this.Ok();
            }
            else
            {
                var errorMessages = string.Join("; ", serviceResult.Errors);

                return this.BadRequest(errorMessages);
            }
        }

        public async Task<IActionResult> RemoveIncome(string incomeId)
        {
            ServiceResult serviceResult = await this._incomeService.RemoveIncomeAsync(incomeId);

            if (serviceResult.Success)
            {

                return this.Ok();
            }
            else
            {
                var errorMessages = string.Join("; ", serviceResult.Errors);

                return this.BadRequest(errorMessages);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddExpense([FromBody] CreateExpenseDTO model) // Add Expense action
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ServiceResult serviceResult = await _expenseService.AddExpenseAsync(model);

            if (serviceResult.Success)
            {

                return Ok();
            }
            else
            {
                var errorMessages = string.Join("; ", serviceResult.Errors);

                return BadRequest(errorMessages);
            }
        }

        [HttpPost]
        public async Task<IActionResult> RemoveExpense(string expenseId) // Remove Expense action
        {
            ServiceResult serviceResult = await _expenseService.RemoveExpenseAsync(expenseId);

            if (serviceResult.Success)
            {

                return Ok();
            }
            else
            {
                var errorMessages = string.Join("; ", serviceResult.Errors);
                return BadRequest(errorMessages);
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditExpense([FromBody] EditExpenseDTO model) // Edit Expense action
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ServiceResult serviceResult = await _expenseService.EditExpenseAsync(model);

            if (serviceResult.Success)
            {
                return Ok();
            }
            else
            {
                var errorMessages = string.Join("; ", serviceResult.Errors);
                return BadRequest(errorMessages);
            }
        }
    }
}

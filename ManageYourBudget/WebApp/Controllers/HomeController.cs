using System.Diagnostics;
using Application.DTOs.ExpenseDTOs;
using Application.DTOs.IncomeDTOs;
using Application.DTOs.SavingsDTOs;
using Application.Interfaces;
using Application.Utils;
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

        public HomeController(
            ILogger<HomeController> logger,
            IExpenseCategoryService expenseCategoryService,
            IIncomeService incomeService,
            ISavingsService savingsService,
            IExpenseService expenseService)
        {
            _logger = logger;
            _expenseCategoryService = expenseCategoryService;
            _incomeService = incomeService;
            _savingsService = savingsService;
            _expenseService = expenseService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = 1;
            var categories = await _expenseCategoryService.GetExpenseCategoriesByUserIdAsync(userId);
            var incomes = await _incomeService.GetIncomesByUserIdAsync(userId);
            var savings = await _savingsService.GetSavingsByUserIdAsync(userId);
            var model = new HomeViewModel
            {
                Categories = categories,
                Incomes = incomes,
                Savings = savings,
            };

            _logger.LogError("Home page opened");
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

        [HttpPost]
        public async Task<IActionResult> AddExpenseCategory([FromBody] CreateExpenseCategoryDTO model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state for AddExpenseCategory");
                return BadRequest(ModelState);
            }

            ServiceResult serviceResult = await _expenseCategoryService.AddExpenseCategoryAsync(model);

            if (serviceResult.Success)
            {
                _logger.LogInformation($"Category added: {model.Title} with budget {model.PlannedBudget}");
                return Ok();
            }
            else
            {
                var errorMessages = string.Join("; ", serviceResult.Errors);
                _logger.LogError($"Failed to add category: {errorMessages}");
                return BadRequest(errorMessages);
            }
        }

        [HttpPost]
        public async Task<IActionResult> RemoveExpenseCategory(int categoryId)
        {
            ServiceResult serviceResult = await _expenseCategoryService.RemoveExpenseCategoryAsync(categoryId);

            if (serviceResult.Success)
            {
                _logger.LogInformation($"Category with ID {categoryId} removed.");
                return Ok();
            }
            else
            {
                var errorMessages = string.Join("; ", serviceResult.Errors);
                _logger.LogError($"Failed to remove category: {errorMessages}");
                return BadRequest(errorMessages);
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditExpenseCategory([FromBody] EditExpenseCategoryDTO model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state for EditExpenseCategory");
                return BadRequest(ModelState);
            }

            ServiceResult serviceResult = await _expenseCategoryService.EditExpenseCategoryAsync(model);

            if (serviceResult.Success)
            {
                _logger.LogInformation($"Category edited: ID {model.Id}, Name: {model.Name}, Planned Budget: {model.PlannedBudget}");
                return Ok();
            }
            else
            {
                var errorMessages = string.Join("; ", serviceResult.Errors);
                _logger.LogError($"Failed to edit category: {errorMessages}");
                return BadRequest(errorMessages);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddIncome([FromBody] IncomeDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ServiceResult serviceResult = await _incomeService.AddIncomeAsync(model);

            if (serviceResult.Success)
            {
                _logger.LogError($"Income added: {model.IncomeName} with budget {model.Amount}");
                return Ok();
            }
            else
            {
                var errorMessages = string.Join("; ", serviceResult.Errors);
                _logger.LogError($"Failed to add income: {errorMessages}");
                return BadRequest(errorMessages);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddSavings([FromBody] CreateSavingsDTO model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state for AddSavings");
                return BadRequest(ModelState);
            }

            ServiceResult serviceResult = await _savingsService.AddSavingsAsync(model);

            if (serviceResult.Success)
            {
                _logger.LogInformation($"Savings added: {model.SavingsName} with amount {model.Amount}");
                return Ok();
            }
            else
            {
                var errorMessages = string.Join("; ", serviceResult.Errors);
                _logger.LogError($"Failed to add savings: {errorMessages}");
                return BadRequest(errorMessages);
            }
        }

        [HttpPost]
        public async Task<IActionResult> RemoveSavings(int savingsId)
        {
            ServiceResult serviceResult = await _savingsService.RemoveSavingsAsync(savingsId);

            if (serviceResult.Success)
            {
                _logger.LogInformation($"Savings with ID {savingsId} removed.");
                return Ok();
            }
            else
            {
                var errorMessages = string.Join("; ", serviceResult.Errors);
                _logger.LogError($"Failed to remove savings: {errorMessages}");
                return BadRequest(errorMessages);
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditSavings([FromBody] EditSavingsDTO model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state for EditSavings");
                return BadRequest(ModelState);
            }

            ServiceResult serviceResult = await _savingsService.EditSavingsAsync(model);

            if (serviceResult.Success)
            {
                _logger.LogInformation($"Savings edited: ID {model.Id}, Name: {model.SavingsName}, Amount: {model.Amount}");
                return Ok();
            }
            else
            {
                var errorMessages = string.Join("; ", serviceResult.Errors);
                _logger.LogError($"Failed to edit savings: {errorMessages}");
                return BadRequest(errorMessages);
            }
        }

        public async Task<IActionResult> EditIncome([FromBody] EditIncomeDTO model)
        {
            if (!this.ModelState.IsValid)
            {
                this._logger.LogError("Invalid model state for EditIncome");
                return this.BadRequest(this.ModelState);
            }

            ServiceResult serviceResult = await this._incomeService.EditIncomeAsync(model);

            if (serviceResult.Success)
            {
                this._logger.LogInformation($"Income edited: ID {model.Id}, Name: {model.Name}, Amount: {model.Amount}");
                return this.Ok();
            }
            else
            {
                var errorMessages = string.Join("; ", serviceResult.Errors);
                this._logger.LogError($"Failed to edit income: {errorMessages}");
                return this.BadRequest(errorMessages);
            }
        }

        public async Task<IActionResult> RemoveIncome(int incomeId)
        {
            ServiceResult serviceResult = await this._incomeService.RemoveIncomeAsync(incomeId);

            if (serviceResult.Success)
            {
                this._logger.LogInformation($"Income with ID {incomeId} removed.");
                return this.Ok();
            }
            else
            {
                var errorMessages = string.Join("; ", serviceResult.Errors);
                this._logger.LogError($"Failed to remove income: {errorMessages}");
                return this.BadRequest(errorMessages);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddExpense([FromBody] ExpenseDTO model) // Add Expense action
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ServiceResult serviceResult = await _expenseService.AddExpenseAsync(model);

            if (serviceResult.Success)
            {
                _logger.LogError($"Expense added: {model.ExpenseName} with amount {model.Amount}");
                return Ok();
            }
            else
            {
                var errorMessages = string.Join("; ", serviceResult.Errors);
                _logger.LogError($"Failed to add expense: {errorMessages}");
                return BadRequest(errorMessages);
            }
        }

        [HttpPost]
        public async Task<IActionResult> RemoveExpense(int expenseId) // Remove Expense action
        {
            ServiceResult serviceResult = await _expenseService.RemoveExpenseAsync(expenseId);

            if (serviceResult.Success)
            {
                _logger.LogInformation($"Expense with ID {expenseId} removed.");
                return Ok();
            }
            else
            {
                var errorMessages = string.Join("; ", serviceResult.Errors);
                _logger.LogError($"Failed to remove expense: {errorMessages}");
                return BadRequest(errorMessages);
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditExpense([FromBody] EditExpenseDTO model) // Edit Expense action
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state for EditExpense");
                return BadRequest(ModelState);
            }

            ServiceResult serviceResult = await _expenseService.EditExpenseAsync(model);

            if (serviceResult.Success)
            {
                _logger.LogInformation($"Expense edited: ID {model.Id}, Name: {model.ExpenseName}, Amount: {model.Amount}");
                return Ok();
            }
            else
            {
                var errorMessages = string.Join("; ", serviceResult.Errors);
                _logger.LogError($"Failed to edit expense: {errorMessages}");
                return BadRequest(errorMessages);
            }
        }
    }
}

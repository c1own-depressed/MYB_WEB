using Application.DTOs.ExpenseDTOs;
using Application.Interfaces;
using Application.Utils;
using Application.Validators;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class ExpenseService : IExpenseService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ExpenseService> _logger;

        public ExpenseService(IUnitOfWork unitOfWork, ILogger<ExpenseService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ServiceResult> AddExpenseAsync(CreateExpenseDTO model)
        {
            _logger.LogInformation("Adding new expense.");

            try
            {
                var validator = new CreateExpenseDTOValidator();
                var validationResult = validator.Validate(model);

                if (!validationResult.IsValid)
                {
                    _logger.LogWarning("Expense validation failed.");
                    return new ServiceResult(success: false, errors: validationResult.Errors.Select(e => e.ErrorMessage));
                }

                var expense = new Expense
                {
                    Id = Guid.NewGuid().ToString(),
                    ExpenseName = model.ExpenseName,
                    Amount = model.Amount,
                    Date = model.Date,
                    CategoryId = model.CategoryId,
                };

                await _unitOfWork.Expenses.AddAsync(expense);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation("Expense added successfully.");
                return new ServiceResult(success: true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding expense.");
                throw; // Re-throw the exception for handling in upper layers
            }
        }

        public async Task<IEnumerable<ExpenseDTO>> GetExpensesByCategoryIdAsync(string categoryId)
        {
            _logger.LogInformation($"Fetching expenses for category with ID: {categoryId}");

            try
            {
                var expenses = await _unitOfWork.Expenses.GetExpensesByCategoryIdAsync(categoryId);

                var expenseDTOs = expenses.Select(expense => new ExpenseDTO
                {
                    Id = expense.Id,
                    ExpenseName = expense.ExpenseName,
                    Amount = expense.Amount,
                    Date = expense.Date,
                    CategoryId = expense.CategoryId,
                });

                _logger.LogInformation($"Successfully fetched {expenseDTOs.Count()} expenses.");
                return expenseDTOs;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while fetching expenses for category with ID: {categoryId}");
                throw; // Re-throw the exception for handling in upper layers
            }
        }

        public async Task<ServiceResult> EditExpenseAsync(EditExpenseDTO model)
        {
            _logger.LogInformation($"Editing expense with ID: {model.Id}");

            try
            {
                var validator = new EditExpenseDTOValidator();
                var validationResult = validator.Validate(model);

                if (!validationResult.IsValid)
                {
                    _logger.LogWarning("Expense edit validation failed.");
                    return new ServiceResult(success: false, errors: validationResult.Errors.Select(e => e.ErrorMessage));
                }

                var expense = await _unitOfWork.Expenses.GetByIdAsync(model.Id);

                if (expense == null)
                {
                    _logger.LogWarning($"Expense with ID: {model.Id} not found.");
                    return new ServiceResult(success: false, errors: new[] { "Expense not found." });
                }

                // Update expense properties with values from DTO
                expense.ExpenseName = model.ExpenseName;
                expense.Amount = model.Amount;

                await _unitOfWork.CompleteAsync();

                _logger.LogInformation($"Expense with ID: {model.Id} edited successfully.");
                return new ServiceResult(success: true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while editing expense with ID: {model.Id}");
                throw; // Re-throw the exception for handling in upper layers
            }
        }

        public async Task<ServiceResult> RemoveExpenseAsync(string expenseId)
        {
            _logger.LogInformation($"Removing expense with ID: {expenseId}");

            try
            {
                var expenseToRemove = await _unitOfWork.Expenses.GetByIdAsync(expenseId);

                if (expenseToRemove == null)
                {
                    _logger.LogWarning($"Expense with ID: {expenseId} not found.");
                    return new ServiceResult(success: false, errors: new[] { "Expense not found." });
                }

                // Delete the expense
                _unitOfWork.Expenses.Delete(expenseToRemove);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation($"Expense with ID: {expenseId} removed successfully.");
                return new ServiceResult(success: true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while removing expense with ID: {expenseId}");
                throw; // Re-throw the exception for handling in upper layers
            }
        }
    }
}
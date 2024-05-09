using Application.DTOs.ExpenseDTOs;
using Application.Interfaces;
using Application.Utils;
using Application.Validators;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services
{
    public class ExpenseCategoryService : IExpenseCategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IExpenseService _expenseService;
        private readonly ILogger<ExpenseCategoryService> _logger;

        public ExpenseCategoryService(IUnitOfWork unitOfWork, IExpenseService expenseService, ILogger<ExpenseCategoryService> logger)
        {
            _unitOfWork = unitOfWork;
            _expenseService = expenseService;
            _logger = logger;
        }

        public async Task<IEnumerable<ExpenseCategoryDTO>> GetExpenseCategoriesByUserIdAsync(string userId)
        {
            _logger.LogInformation($"Fetching expense categories for user with ID: {userId}");

            try
            {
                var expenseCategories = _unitOfWork.ExpenseCategories.GetExpenseCategoriesByUserId(userId);
                var expenseCategoryDTOs = new List<ExpenseCategoryDTO>();

                foreach (var category in expenseCategories)
                {
                    var expenses = await _expenseService.GetExpensesByCategoryIdAsync(category.Id);

                    var remainingBudget = category.Amount - expenses.Sum(e => e.Amount);

                    expenseCategoryDTOs.Add(new ExpenseCategoryDTO
                    {
                        Id = category.Id,
                        Name = category.CategoryName,
                        PlannedBudget = category.Amount,
                        RemainingBudget = remainingBudget,
                        Expenses = expenses,
                    });
                }

                _logger.LogInformation($"Successfully fetched {expenseCategoryDTOs.Count} expense categories.");
                return expenseCategoryDTOs;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while fetching expense categories for user with ID: {userId}");
                throw; // Re-throw the exception for handling in upper layers
            }
        }

        public async Task<ServiceResult> AddExpenseCategoryAsync(CreateExpenseCategoryDTO model, string userId)
        {
            _logger.LogInformation($"Adding new expense category for user with ID: {userId}");

            try
            {
                var validator = new CreateExpenseCategoryDTOValidator();
                var validationResult = validator.Validate(model);

                if (!validationResult.IsValid)
                {
                    _logger.LogWarning("Expense category validation failed.");
                    return new ServiceResult(success: false, errors: validationResult.Errors.Select(e => e.ErrorMessage));
                }

                var expenseCategory = new ExpenseCategory
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = userId,
                    CategoryName = model.Title,
                    Amount = model.PlannedBudget,
                };

                await _unitOfWork.ExpenseCategories.AddAsync(expenseCategory);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation($"Expense category added successfully for user with ID: {userId}");
                return new ServiceResult(success: true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while adding expense category for user with ID: {userId}");
                throw; // Re-throw the exception for handling in upper layers
            }
        }

        public async Task<ServiceResult> RemoveExpenseCategoryAsync(string categoryId)
        {
            _logger.LogInformation($"Removing expense category with ID: {categoryId}");

            try
            {
                var categoryToRemove = await _unitOfWork.ExpenseCategories.GetByIdAsync(categoryId);

                if (categoryToRemove == null)
                {
                    _logger.LogWarning($"Expense category with ID: {categoryId} not found.");
                    return new ServiceResult(success: false, errors: new[] { "Expense category not found." });
                }

                // Remove associated expenses
                var expensesToRemove = await _unitOfWork.Expenses.GetExpensesByCategoryIdAsync(categoryId);
                foreach (var expense in expensesToRemove)
                {
                    _unitOfWork.Expenses.Delete(expense);
                }

                // Delete the expense category
                _unitOfWork.ExpenseCategories.Delete(categoryToRemove);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation($"Expense category with ID: {categoryId} removed successfully.");
                return new ServiceResult(success: true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while removing expense category with ID: {categoryId}");
                throw; // Re-throw the exception for handling in upper layers
            }
        }

        public async Task<ServiceResult> EditExpenseCategoryAsync(EditExpenseCategoryDTO model)
        {
            _logger.LogInformation($"Editing expense category with ID: {model.Id}");

            try
            {
                var validator = new EditExpenseCategoryDTOValidator();
                var validationResult = validator.Validate(model);

                if (!validationResult.IsValid)
                {
                    _logger.LogWarning("Expense category edit validation failed.");
                    return new ServiceResult(success: false, errors: validationResult.Errors.Select(e => e.ErrorMessage));
                }

                var expenseCategory = await _unitOfWork.ExpenseCategories.GetByIdAsync(model.Id);

                if (expenseCategory == null)
                {
                    _logger.LogWarning($"Expense category with ID: {model.Id} not found.");
                    return new ServiceResult(success: false, errors: new[] { "Expense category not found." });
                }

                // Update expenseCategory properties with values from DTO
                expenseCategory.CategoryName = model.Name;
                expenseCategory.Amount = model.PlannedBudget;

                await _unitOfWork.CompleteAsync();

                _logger.LogInformation($"Expense category with ID: {model.Id} edited successfully.");
                return new ServiceResult(success: true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while editing expense category with ID: {model.Id}");
                throw; // Re-throw the exception for handling in upper layers
            }
        }
    }
}

using Application.DTOs;
using Application.DTOs.Validators;
using Application.Interfaces;
using Application.Utils;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services
{
    public class ExpenseCategoryService : IExpenseCategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IExpenseService _expenseService;

        public ExpenseCategoryService(IUnitOfWork unitOfWork, IExpenseService expenseService)
        {
            _unitOfWork = unitOfWork;
            _expenseService = expenseService;
        }

        public async Task<IEnumerable<ExpenseCategoryDTO>> GetExpenseCategoriesByUserIdAsync(int userId)
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

            return expenseCategoryDTOs;
        }

        public async Task<ServiceResult> AddExpenseCategoryAsync(CreateExpenseCategoryDTO model)
        {
            var validator = new CreateExpenseCategoryDTOValidator();
            var validationResult = validator.Validate(model);

            if (!validationResult.IsValid)
            {
                return new ServiceResult(success: false, errors: validationResult.Errors.Select(e => e.ErrorMessage));
            }

            var expenseCategory = new ExpenseCategory
            {
                UserId = 1, // TODO: Use actual user ID from session or request
                CategoryName = model.Title,
                Amount = model.PlannedBudget,
            };

            await _unitOfWork.ExpenseCategories.AddAsync(expenseCategory);
            await _unitOfWork.CompleteAsync();

            return new ServiceResult(success: true);
        }

        public async Task<ServiceResult> RemoveExpenseCategoryAsync(int categoryId)
        {
            var categoryToRemove = await _unitOfWork.ExpenseCategories.GetByIdAsync(categoryId);

            if (categoryToRemove == null)
            {
                return new ServiceResult(success: false, errors: new[] { "Expense category not found." });
            }

            // Remove associated expenses
            var expensesToRemove = await _unitOfWork.Expenses.GetExpensesByExpenseCategoryIdAsync(categoryId);
            foreach (var expense in expensesToRemove)
            {
                _unitOfWork.Expenses.Delete(expense);
            }

            // Delete the expense category
            _unitOfWork.ExpenseCategories.Delete(categoryToRemove);
            await _unitOfWork.CompleteAsync();

            return new ServiceResult(success: true);
        }

        public async Task<ServiceResult> EditExpenseCategoryAsync(EditExpenseCategoryDTO model)
        {
            var validator = new EditExpenseCategoryDTOValidator();
            var validationResult = validator.Validate(model);

            if (!validationResult.IsValid)
            {
                return new ServiceResult(success: false, errors: validationResult.Errors.Select(e => e.ErrorMessage));
            }

            var expenseCategory = await _unitOfWork.ExpenseCategories.GetByIdAsync(model.Id);

            if (expenseCategory == null)
            {
                return new ServiceResult(success: false, errors: new[] { "Expense category not found." });
            }

            // Update expenseCategory properties with values from DTO
            expenseCategory.CategoryName = model.Name;
            expenseCategory.Amount = model.PlannedBudget;

            await _unitOfWork.CompleteAsync();

            return new ServiceResult(success: true);
        }
    }
}


using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using System.Xml.Serialization;

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
                    Name = category.CategoryName,
                    PlannedBudget = category.Amount,
                    RemainingBudget = remainingBudget,
                    Expenses = expenses
                });
            }

            return expenseCategoryDTOs;
        }

        public async Task<(bool isSuccess, string errorMessage)> AddExpenseCategoryAsync(CreateExpenseCategoryDTO model)
        {
            if (model.PlannedBudget <= 0)
            {
                return (false, "Planned budget must be greater than 0.");
            }
            if (model.PlannedBudget > 99999999)
            {
                return (false, "Planned budget must be lower than 100000000.");
            }
            if (model.Title.Length < 5 || model.Title.Length > 100)
            {
                return (false, "Title length should be between 5 and 100 characters.");
            }

            var expenseCategory = new ExpenseCategory
            {
                UserId = 1, // TODO: Use actual user ID from session or request
                CategoryName = model.Title,
                Amount = model.PlannedBudget,
            };

            await _unitOfWork.ExpenseCategories.AddAsync(expenseCategory);
            await _unitOfWork.CompleteAsync();

            return (true, "");
        }
    }
}

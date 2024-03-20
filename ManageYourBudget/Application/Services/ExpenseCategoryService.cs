
using Application.DTOs;
using Application.Interfaces;
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
                    Name = category.CategoryName,
                    PlannedBudget = category.Amount,
                    RemainingBudget = remainingBudget,
                    Expenses = expenses
                });
            }

            return expenseCategoryDTOs;
        }
    }
}

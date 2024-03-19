
using Application.DTOs;
using Domain.Interfaces;

namespace Application.Services
{
    public class ExpenseCategoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ExpenseCategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<ExpenseCategoryDTO>> GetExpenseCategoriesByUserIdAsync(int userId)
        {
            var expenseCategories = _unitOfWork.ExpenseCategories.GetExpenseCategoriesByUserId(userId);
            var expenseCategoryViewModels = expenseCategories.Select(category => new ExpenseCategoryDTO
            {
                Name = category.CategoryName,
                PlannedBudget = category.Amount,
                RemainingBudget = 400  // TODO: calculate sum of the expenses
            });

            return expenseCategoryViewModels;
        }
    }
}

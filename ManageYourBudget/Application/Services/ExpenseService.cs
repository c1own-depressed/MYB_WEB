using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services
{
    public class ExpenseService : IExpenseService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ExpenseService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task AddExpense(ExpenseCategory expense)
        {
            _unitOfWork.ExpenseCategories.AddAsync(expense);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<IEnumerable<ExpenseDTO>> GetExpensesByCategoryIdAsync(int categoryId)
        {
            var expenses = await _unitOfWork.Expenses.GetExpensesByExpenseCategoryIdAsync(categoryId);

            var expenseDTOs = expenses.Select(expense => new ExpenseDTO
            {
                ExpenseName = expense.ExpenseName,
                Amount = expense.Amount,
            });

            return expenseDTOs;
        }
    }
}

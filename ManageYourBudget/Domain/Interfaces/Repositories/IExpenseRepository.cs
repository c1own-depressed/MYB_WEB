using Domain.Entities;

namespace Domain.Interfaces.Repositories
{
    public interface IExpenseRepository
    {
        Task<IEnumerable<Expense>> GetExpensesByExpenseCategoryIdAsync(int expenseCategoryId);
    }
}

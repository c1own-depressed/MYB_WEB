using Domain.Entities;

namespace Domain.Interfaces.Repositories
{
    public interface IExpenseRepository : IRepositoryBase<Expense>
    {
        Task<IEnumerable<Expense>> GetExpensesByExpenseCategoryIdAsync(int expenseCategoryId);
    }
}

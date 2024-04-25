using Domain.Entities;

namespace Domain.Interfaces.Repositories
{
    public interface IExpenseRepository : IRepositoryBase<Expense>
    {
        Task<IEnumerable<Expense>> GetExpensesByCategoryIdAsync(string expenseCategoryId);

        Task<IEnumerable<Expense>> GetAllExpensesByCategoryIdAndDateRangeAsync(string categoryId, DateTime from, DateTime to);
    }
}

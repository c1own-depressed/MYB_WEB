using Domain.Entities;

namespace Domain.Interfaces.Repositories
{
    public interface IExpenseCategoryRepository : IRepositoryBase<ExpenseCategory>
    {
        Task<IEnumerable<ExpenseCategory>> GetExpenseCategoriesByUserIdAsync(int userId);
        IEnumerable<ExpenseCategory> GetExpenseCategoriesByUserId(int userId);
    }
}

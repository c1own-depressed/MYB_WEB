using Domain.Entities;

namespace Domain.Interfaces.Repositories
{
    public interface IExpenseCategoryRepository : IRepositoryBase<ExpenseCategory>
    {
        Task<IEnumerable<ExpenseCategory>> GetExpenseCategoriesByUserIdAsync(string userId);
        
        IEnumerable<ExpenseCategory> GetExpenseCategoriesByUserId(string userId);
    }
}

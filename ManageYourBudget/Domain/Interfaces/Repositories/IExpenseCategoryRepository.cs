using Domain.Entities;
namespace Domain.Interfaces.Repositories
{
    public interface IExpenseCategoryRepository : IRepositoryBase<ExpenseCategory>
    {
        IEnumerable<ExpenseCategory> GetExpenseCategoriesByUserId(int userId);
    }
}

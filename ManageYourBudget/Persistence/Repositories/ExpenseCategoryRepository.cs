using Domain.Entities;
using Domain.Interfaces.Repositories;

namespace Persistence.Repositories
{
    public class ExpenseCategoryRepository : RepositoryBase<ExpenseCategory>, IExpenseCategoryRepository
    {
        public ExpenseCategoryRepository(MYBDbContext context)
            : base(context)
        {
        }

        public IEnumerable<ExpenseCategory> GetExpenseCategoriesByUserId(int userId)
        {
            return _context.Set<ExpenseCategory>().Where(category => category.UserId == userId).ToList();
        }
    }
}
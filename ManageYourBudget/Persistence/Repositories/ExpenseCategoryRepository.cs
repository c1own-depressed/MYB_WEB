using Domain.Entities;
using Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories
{
    public class ExpenseCategoryRepository : RepositoryBase<ExpenseCategory>, IExpenseCategoryRepository
    {
        public ExpenseCategoryRepository(MYBDbContext context)
            : base(context)
        {
        }

        public async Task<IEnumerable<ExpenseCategory>> GetExpenseCategoriesByUserIdAsync(string userId)
        {
            return await _context.Set<ExpenseCategory>().Where(category => category.UserId == userId).ToListAsync();
        }

        public IEnumerable<ExpenseCategory> GetExpenseCategoriesByUserId(string userId)
        {
            return _context.Set<ExpenseCategory>().Where(category => category.UserId == userId).ToList();
        }
    }
}
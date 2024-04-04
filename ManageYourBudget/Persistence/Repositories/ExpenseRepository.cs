using Domain.Entities;
using Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories
{
    public class ExpenseRepository : RepositoryBase<Expense>, IExpenseRepository
    {
        public ExpenseRepository(MYBDbContext context)
            : base(context)
        {
        }

        public async Task<IEnumerable<Expense>> GetExpensesByCategoryIdAsync(int expenseCategoryId)
        {
            return await _context.Set<Expense>().Where(expense => expense.CategoryId == expenseCategoryId).ToListAsync();
        }
    }
}

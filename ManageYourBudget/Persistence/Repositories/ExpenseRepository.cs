using Domain.Entities;
using Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using Persistence.Data;

namespace Persistence.Repositories
{
    public class ExpenseRepository : RepositoryBase<Expense>, IExpenseRepository
    {
        public ExpenseRepository(MYBDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Expense>> GetExpensesByExpenseCategoryIdAsync(int expenseCategoryId)
        {
            return await _context.Set<Expense>().Where(expense => expense.CategoryId == expenseCategoryId).ToListAsync();
        }
    }
}

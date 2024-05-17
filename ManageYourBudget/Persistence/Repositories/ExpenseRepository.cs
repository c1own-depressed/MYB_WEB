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

        public async Task<IEnumerable<Expense>> GetAllExpensesByCategoryIdAndDateRangeAsync(string categoryId, DateTime from, DateTime to)
        {
            return await _context.Set<Expense>().Where(expense => expense.Date > from && expense.Date < to && expense.CategoryId == categoryId).ToListAsync();
        }

        public async Task<IEnumerable<Expense>> GetExpensesByCategoryIdAsync(string expenseCategoryId)
        {
            return await _context.Set<Expense>().Where(expense => expense.CategoryId == expenseCategoryId).ToListAsync();
        }
    }
}

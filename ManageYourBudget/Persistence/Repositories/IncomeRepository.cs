using Domain.Entities;
using Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories
{
    public class IncomeRepository : RepositoryBase<Income>, IIncomeRepository
    {
        public IncomeRepository(MYBDbContext context)
            : base(context)
        {
        }

        public async Task<IEnumerable<Income>> GetIncomesByUserIdAsync(int userId)
        {
            return await _context.Set<Income>().Where(income => income.UserId == userId).ToListAsync();
        }
    }
}

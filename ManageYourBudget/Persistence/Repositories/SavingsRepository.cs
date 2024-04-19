using Domain.Entities;
using Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories
{
    internal class SavingsRepository : RepositoryBase<Savings>, ISavingsRepository
    {
        public SavingsRepository(MYBDbContext context)
            : base(context)
        {
        }

        public async Task<IEnumerable<Savings>> GetSavingsByUserIdAsync(string userId)
        {
            return await _context.Set<Savings>().Where(savings => savings.UserId == userId).ToListAsync();
        }
    }
}
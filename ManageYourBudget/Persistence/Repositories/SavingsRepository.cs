

using Domain.Entities;
using Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using Persistence.Data;

namespace Persistence.Repositories
{
    internal class SavingsRepository : RepositoryBase<Savings>, ISavingsRepository
    {
        public SavingsRepository(MYBDbContext context) : base(context)
        {
        }

        public IEnumerable<Savings> GetSavingsByUserId(int userId)
        {
            return _context.Set<Savings>().Where(savings => savings.UserId == userId).ToList();
        }
    }
}
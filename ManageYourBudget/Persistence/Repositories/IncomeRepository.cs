using Domain.Entities;
using Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using Persistence.Data;

namespace Persistence.Repositories
{
    public class IncomeRepository : RepositoryBase<Income>, IIncomeRepository
    {
        public IncomeRepository(MYBDbContext context) : base(context)
        {
        }

        public IEnumerable<Income> GetIncomesByUserId(int userId)
        {
            return _context.Set<Income>().Where(income => income.UserId == userId).ToList();
        }
    }
}

using Domain.Entities;
using Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories
{
    public class ReviewRepository : RepositoryBase<Review>, IReviewRepository
    {
        public ReviewRepository(DbContext context)
            : base(context)
        {
        }

        public async Task<IEnumerable<Review>> GetAllReviewsAsync()
        {
            return await _context.Set<Review>().ToListAsync();
        }
    }
}

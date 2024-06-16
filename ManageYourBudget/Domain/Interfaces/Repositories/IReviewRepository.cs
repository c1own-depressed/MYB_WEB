using Domain.Entities;

namespace Domain.Interfaces.Repositories
{
    public interface IReviewRepository : IRepositoryBase<Review>
    {
        Task<IEnumerable<Review>> GetAllReviewsAsync();
    }
}

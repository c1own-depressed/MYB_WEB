using Domain.Entities;


namespace Domain.Interfaces.Repositories
{
    public interface ISavingsRepository : IRepositoryBase<Savings>
    {
        Task<IEnumerable<Savings>> GetSavingsByUserIdAsync(int userId);
    }
}

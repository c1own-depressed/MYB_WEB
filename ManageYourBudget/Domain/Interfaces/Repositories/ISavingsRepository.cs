using Domain.Entities;


namespace Domain.Interfaces.Repositories
{
    public interface ISavingsRepository : IRepositoryBase<Savings>
    {
        IEnumerable<Savings> GetSavingsByUserId(int userId);
    }
}

using Domain.Entities;


namespace Domain.Interfaces.Repositories
{
    public interface ISavingsRepository
    {
        IEnumerable<Savings> GetSavingsByUserId(int userId);
    }
}

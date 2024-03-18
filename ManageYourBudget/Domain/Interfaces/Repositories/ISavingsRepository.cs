using Domain.Entities;


namespace Domain.Interfaces.Repositories
{
    public interface ISavingsRepository
    {
        IEnumerable<Savings> GetSavingsByUserId(int userId);
        //IEnumerable<Savings> GetSavings();
        //Savings GetSavingsByID(int savingsId);
        //void InsertSavings(Savings savings);
        //void DeleteSavings(int savingsID);
        //void UpdateSavings(Savings savings);
        //void Save();
    }
}

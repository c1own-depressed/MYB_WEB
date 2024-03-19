using Domain.Entities;


namespace Domain.Interfaces.Repositories
{
    public interface IIncomeRepository
    {
        IEnumerable<Income> GetIncomesByUserId(int userId);
    }
}

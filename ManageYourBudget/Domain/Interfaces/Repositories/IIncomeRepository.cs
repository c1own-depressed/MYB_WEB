using Domain.Entities;


namespace Domain.Interfaces.Repositories
{
    public interface IIncomeRepository : IRepositoryBase<Income>
    {
        IEnumerable<Income> GetIncomesByUserId(int userId);
    }
}

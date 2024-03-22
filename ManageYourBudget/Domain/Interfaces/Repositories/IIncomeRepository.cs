using Domain.Entities;


namespace Domain.Interfaces.Repositories
{
    public interface IIncomeRepository : IRepositoryBase<Income>
    {
        Task<IEnumerable<Income>> GetIncomesByUserIdAsync(int userId);
    }
}

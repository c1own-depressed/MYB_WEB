using Application.DTOs;
using Application.Utils;

namespace Application.Interfaces
{
    public interface IIncomeService
    {
        Task<IEnumerable<IncomeDTO>> GetIncomesByUserIdAsync(int userId);

        Task<ServiceResult> AddIncomeAsync(IncomeDTO income);


    }
}

using Application.DTOs.IncomeDTOs;
using Application.Utils;

namespace Application.Interfaces
{
    public interface IIncomeService
    {
        Task<IEnumerable<IncomeDTO>> GetIncomesByUserIdAsync(string userId);

        Task<ServiceResult> AddIncomeAsync(CreateIncomeDTO model, string userId);

        Task<ServiceResult> RemoveIncomeAsync(string incomeId);

        Task<ServiceResult> EditIncomeAsync(EditIncomeDTO model);
    }
}

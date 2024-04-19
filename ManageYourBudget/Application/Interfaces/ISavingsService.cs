using Application.DTOs.SavingsDTOs;
using Application.Utils;

namespace Application.Interfaces
{
    public interface ISavingsService
    {
        Task<IEnumerable<SavingsDTO>> GetSavingsByUserIdAsync(int userId);

        Task<ServiceResult> AddSavingsAsync(CreateSavingsDTO model);

        Task<ServiceResult> RemoveSavingsAsync(int savingsId);

        Task<ServiceResult> EditSavingsAsync(EditSavingsDTO model);
    }
}

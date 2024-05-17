using Application.DTOs.SavingsDTOs;
using Application.Utils;

namespace Application.Interfaces
{
    public interface ISavingsService
    {
        Task<IEnumerable<SavingsDTO>> GetSavingsByUserIdAsync(string userId);

        Task<ServiceResult> AddSavingsAsync(CreateSavingsDTO model, string userId);

        Task<ServiceResult> RemoveSavingsAsync(string savingsId);

        Task<ServiceResult> EditSavingsAsync(EditSavingsDTO model);
    }
}

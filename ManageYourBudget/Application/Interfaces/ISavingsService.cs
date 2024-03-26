using Application.DTOs;
using Application.Utils;

namespace Application.Interfaces
{
    public interface ISavingsService
    {
        Task<IEnumerable<SavingsDTO>> GetSavingsByUserIdAsync(int userId);

        Task<ServiceResult> AddSavingsAsync(CreateSavingsDTO model);

        Task<ServiceResult> RemoveSavingsAsync(int categoryId);

        Task<ServiceResult> EditSavingsAsync(EditSavingsDTO model);
    }
}

using Application.DTOs.ExpenseDTOs;
using Application.Utils;

namespace Application.Interfaces
{
    public interface IExpenseCategoryService
    {
        Task<IEnumerable<ExpenseCategoryDTO>> GetExpenseCategoriesByUserIdAsync(string userId);

        Task<ServiceResult> AddExpenseCategoryAsync(CreateExpenseCategoryDTO model, string userId);

        Task<ServiceResult> RemoveExpenseCategoryAsync(string categoryId);

        Task<ServiceResult> EditExpenseCategoryAsync(EditExpenseCategoryDTO model);
    }
}

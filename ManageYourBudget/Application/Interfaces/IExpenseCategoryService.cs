using Application.DTOs;
using Application.Utils;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IExpenseCategoryService
    {
        Task<IEnumerable<ExpenseCategoryDTO>> GetExpenseCategoriesByUserIdAsync(int userId);

        Task<ServiceResult> AddExpenseCategoryAsync(CreateExpenseCategoryDTO model);

        Task<ServiceResult> RemoveExpenseCategoryAsync(int categoryId);

        Task<ServiceResult> EditExpenseCategoryAsync(EditExpenseCategoryDTO model);
    }
}

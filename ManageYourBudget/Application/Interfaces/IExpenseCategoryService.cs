using Application.DTOs;


namespace Application.Interfaces
{
    public interface IExpenseCategoryService
    {
        Task<IEnumerable<ExpenseCategoryDTO>> GetExpenseCategoriesByUserIdAsync(int userId);
        Task<(bool isSuccess, string errorMessage)> AddExpenseCategoryAsync(CreateExpenseCategoryDTO model);
        Task<(bool isSuccess, string errorMessage)> RemoveExpenseCategoryAsync(int categoryId);
    }
}

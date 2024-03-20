using Application.DTOs;


namespace Application.Interfaces
{
    public interface IExpenseCategoryService
    {
        Task<IEnumerable<ExpenseCategoryDTO>> GetExpenseCategoriesByUserIdAsync(int userId);
    }
}

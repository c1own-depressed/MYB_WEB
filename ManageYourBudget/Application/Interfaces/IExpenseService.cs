using Application.DTOs;


namespace Application.Interfaces
{
    public interface IExpenseService
    {
        Task<IEnumerable<ExpenseDTO>> GetExpensesByCategoryIdAsync(int categoryId);
    }
}

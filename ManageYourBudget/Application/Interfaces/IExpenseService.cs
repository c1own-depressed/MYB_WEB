using Application.DTOs.ExpenseDTOs;
using Application.Utils;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IExpenseService
    {
        Task<IEnumerable<ExpenseDTO>> GetExpensesByCategoryIdAsync(int categoryId);

        Task<ServiceResult> AddExpenseAsync(ExpenseDTO expense);

        Task<ServiceResult> RemoveExpenseAsync(int expenseId);

        Task<ServiceResult> EditExpenseAsync(EditExpenseDTO model);
    }
}

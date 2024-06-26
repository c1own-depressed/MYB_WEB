﻿using Application.DTOs.ExpenseDTOs;
using Application.Utils;

namespace Application.Interfaces
{
    public interface IExpenseService
    {
        Task<IEnumerable<ExpenseDTO>> GetExpensesByCategoryIdAsync(string categoryId);

        Task<ServiceResult> AddExpenseAsync(CreateExpenseDTO expense);

        Task<ServiceResult> RemoveExpenseAsync(string expenseId);

        Task<ServiceResult> EditExpenseAsync(EditExpenseDTO model);
    }
}

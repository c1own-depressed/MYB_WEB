﻿using Application.DTOs;
using Application.Utils;

namespace Application.Interfaces
{
    public interface IIncomeService
    {
        Task<IEnumerable<IncomeDTO>> GetIncomesByUserIdAsync(int userId);

        Task<ServiceResult> AddIncomeAsync(IncomeDTO income);

        Task<ServiceResult> RemoveIncomeAsync(int incomeId);

        Task<ServiceResult> EditIncomeAsync(EditIncomeDTO model);
    }
}

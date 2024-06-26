﻿using Domain.Interfaces.Repositories;

namespace Domain.Interfaces
{
    public interface IUnitOfWork
    {
        IUserRepository Users { get; }

        IExpenseRepository Expenses { get; }

        IExpenseCategoryRepository ExpenseCategories { get; }

        ISavingsRepository Savings { get; }

        IIncomeRepository Incomes { get; }

        Task<int> CompleteAsync();
    }
}

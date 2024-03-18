﻿using Domain.Interfaces;
using Domain.Interfaces.Repositories;
using Persistence.Data;
using Persistence.Repositories;


namespace Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly MYBDbContext _context;
        private UserRepository? _userRepository;
        private ExpenseRepository? _expenseRepository;
        private ExpenseCategoryRepository? _expenseCategoryRepository;
        private IncomeRepository? _incomeRepository;
        private SavingsRepository? _savingsRepository;

        public UnitOfWork(MYBDbContext context)
        {
            _context = context;
        }

        public IUserRepository Users
        {
            get
            {
                if (_userRepository == null)
                {
                    _userRepository = new UserRepository(_context);
                }

                return _userRepository;
            }
        }

        public IExpenseRepository Expenses
        {
            get
            {
                if (_expenseRepository == null)
                {
                    _expenseRepository = new ExpenseRepository(_context);
                }

                return _expenseRepository;
            }
        }

        public IExpenseCategoryRepository ExpenseCategories
        {
            get
            {
                if (_expenseCategoryRepository == null)
                {
                    _expenseCategoryRepository = new ExpenseCategoryRepository(_context);
                }

                return _expenseCategoryRepository;
            }
        }

        public IIncomeRepository Incomes
        {
            get
            {
                if (_incomeRepository == null)
                {
                    _incomeRepository = new IncomeRepository(_context);
                }

                return _incomeRepository;
            }
        }

        public ISavingsRepository Savings
        {
            get
            {
                if (_savingsRepository == null)
                {
                    _savingsRepository = new SavingsRepository(_context);
                }

                return _savingsRepository;
            }
        }

        public async Task<int> CompleteAsync()
        {
            // Save changes across all repositories
            return await _context.SaveChangesAsync();  
        }

        private bool _disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }

            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
using Application.DTOs;
using Application.DTOs.Validators;
using Application.Interfaces;
using Application.Utils;
using Domain.Entities;
using Domain.Interfaces;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services
{
    public class ExpenseService : IExpenseService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ExpenseService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResult> AddExpenseAsync(ExpenseDTO model)
        {
            var validator = new ExpenseDTOValidator();
            var validationResult = validator.Validate(model);

            if (!validationResult.IsValid)
            {
                return new ServiceResult(success: false, errors: validationResult.Errors.Select(e => e.ErrorMessage));
            }

            var expense = new Expense
            {
                ExpenseName = model.ExpenseName,
                Amount = model.Amount,
                Date = model.Date,
                CategoryId = model.CategoryId
            };

            await _unitOfWork.Expenses.AddAsync(expense);
            await _unitOfWork.CompleteAsync();

            return new ServiceResult(success: true);
        }

        public async Task<IEnumerable<ExpenseDTO>> GetExpensesByCategoryIdAsync(int categoryId)
        {
            var expenses = await _unitOfWork.Expenses.GetExpensesByCategoryIdAsync(categoryId);

            var expenseDTOs = expenses.Select(expense => new ExpenseDTO
            {
                Id = expense.Id,
                ExpenseName = expense.ExpenseName,
                Amount = expense.Amount,
                Date = expense.Date,
                CategoryId = expense.CategoryId
            });

            return expenseDTOs;
        }


        public async Task<ServiceResult> EditExpenseAsync(EditExpenseDTO model)
        {
            var validator = new EditExpenseDTOValidator();
            var validationResult = validator.Validate(model);

            if (!validationResult.IsValid)
            {
                return new ServiceResult(success: false, errors: validationResult.Errors.Select(e => e.ErrorMessage));
            }

            var expense = await _unitOfWork.Expenses.GetByIdAsync(model.Id);

            if (expense == null)
            {
                return new ServiceResult(success: false, errors: new[] { "Expense not found." });
            }

            // Update expense properties with values from DTO
            expense.ExpenseName = model.ExpenseName;
            expense.Amount = model.Amount;

            await _unitOfWork.CompleteAsync();

            return new ServiceResult(success: true);
        }

        public async Task<ServiceResult> RemoveExpenseAsync(int expenseId)
        {
            var expenseToRemove = await _unitOfWork.Expenses.GetByIdAsync(expenseId);

            if (expenseToRemove == null)
            {
                return new ServiceResult(success: false, errors: new[] { "Expense not found." });
            }

            // Delete the expense
            _unitOfWork.Expenses.Delete(expenseToRemove);
            await _unitOfWork.CompleteAsync();

            return new ServiceResult(success: true);
        }
    }
}

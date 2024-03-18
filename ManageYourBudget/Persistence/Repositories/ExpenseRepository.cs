using Domain.Entities;
using Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories
{
    public class ExpenseRepository : RepositoryBase<Expense>, IExpenseRepository
    {
        public ExpenseRepository(DbContext context) : base(context)
        {
        }

        public IEnumerable<Expense> GetExpensesByExpenseCategoryId(int expenseCategoryId)
        {
            return _context.Set<Expense>().Where(expense => expense.CategoryId == expenseCategoryId).ToList();
        }

        //public IEnumerable<Expense> GetExpenses()
        //{
        //    return _context.Set<Expense>().ToList();
        //}

        //public Expense GetExpenseByID(int id)
        //{
        //    return _context.Set<Expense>().Find(keyValues: id);
        //}

        //public void InsertExpense(Expense expense)
        //{
        //    _context.Set<Expense>().Add(expense);
        //}

        //public void DeleteExpense(int expenseID)
        //{
        //    Expense expense = _context.Set<Expense>().Find(expenseID);
        //    _context.Set<Expense>().Remove(expense);
        //}

        //public void UpdateExpense(Expense expense)
        //{
        //    _context.Entry(expense).State = EntityState.Modified;
        //}

        //public void Save()
        //{
        //    _context.SaveChanges();
        //}
    }
}

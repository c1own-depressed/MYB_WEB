using Domain.Entities;
using Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using Persistence.Data;

namespace Persistence.Repositories
{
    public class ExpenseCategoryRepository : RepositoryBase<ExpenseCategory>, IExpenseCategoryRepository
    {
        public ExpenseCategoryRepository(MYBDbContext context) : base(context)
        {
        }

        public IEnumerable<ExpenseCategory> GetExpenseCategoriesByUserId(int userId)
        {
            return _context.Set<ExpenseCategory>().Where(category => category.UserId == userId).ToList();
        }

        //public IEnumerable<ExpenseCategory> GetExpenseCategories()
        //{
        //    return _context.Set<ExpenseCategory>().ToList();
        //}

        //public ExpenseCategory GetExpenseCategoryByID(int id)
        //{
        //    return _context.Set<ExpenseCategory>().Find(keyValues: id);
        //}

        //public void InsertExpenseCategory(ExpenseCategory expenseCategory)
        //{
        //    _context.Set<ExpenseCategory>().Add(expenseCategory);
        //}

        //public void DeleteExpenseCategory(int expenseCategoryID)
        //{
        //    ExpenseCategory expenseCategory = _context.Set<ExpenseCategory>().Find(expenseCategoryID);
        //    _context.Set<ExpenseCategory>().Remove(expenseCategory);
        //}

        //public void UpdateExpenseCategory(ExpenseCategory expenseCategory)
        //{
        //    _context.Entry(expenseCategory).State = EntityState.Modified;
        //}

        //public void Save()
        //{
        //    _context.SaveChanges();
        //}
    }
}
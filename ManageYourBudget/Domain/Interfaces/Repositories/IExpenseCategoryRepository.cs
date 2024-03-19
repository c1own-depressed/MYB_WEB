using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.Repositories
{
    public interface IExpenseCategoryRepository : IRepositoryBase<ExpenseCategory>
    {
        IEnumerable<ExpenseCategory> GetExpenseCategoriesByUserId(int userId);
        //IEnumerable<ExpenseCategory> GetExpenseCategories();
        //ExpenseCategory GetExpenseCategoryByID(int expenseCategoryId);
        //void InsertExpenseCategory(ExpenseCategory expenseCategory);
        //void DeleteExpenseCategory(int expenseCategoryId);
        //void UpdateExpenseCategory(ExpenseCategory expenseCategory);
        //void Save();
    }
}

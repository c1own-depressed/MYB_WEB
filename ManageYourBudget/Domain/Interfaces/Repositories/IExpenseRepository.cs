using Domain.Entities;

namespace Domain.Interfaces.Repositories
{
    public interface IExpenseRepository
    {
        IEnumerable<Expense> GetExpensesByExpenseCategoryId(int expenseCategoryId);
        //IEnumerable<Expense> GetExpenses();
        //Expense GetExpenseByID(int expenseId);
        //void InsertExpense(Expense expense);
        //void DeleteExpense(int expenseId);
        //void UpdateExpense(Expense expense);
        //void Save();
    }
}

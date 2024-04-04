using Application.DTOs;

namespace WebApp.Models
{
    public class HomeViewModel
    {
        public IEnumerable<SavingsDTO>? Savings { get; set; }

        public IEnumerable<IncomeDTO>? Incomes { get; set; }

        public IEnumerable<ExpenseCategoryDTO>? Categories { get; set; }

        public IEnumerable<ExpenseDTO>? Expenses { get; set; }
    }
}

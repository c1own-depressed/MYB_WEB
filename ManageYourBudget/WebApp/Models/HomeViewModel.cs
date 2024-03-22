using Application.DTOs;

namespace WebApp.Models
{
    public class HomeViewModel
    {
        // TODO:
        //public IEnumerable<SavingsDTO>? Savings { get; set; }
        public IEnumerable<IncomeDTO>? Incomes { get; set; }
        public IEnumerable<ExpenseCategoryDTO>? Categories { get; set; }

    }
}

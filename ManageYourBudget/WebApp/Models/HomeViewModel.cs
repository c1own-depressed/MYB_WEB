using Application.DTOs;

namespace WebApp.Models
{
    public class HomeViewModel
    {
        public SavingsViewModel? Savings { get; set; }
        public IncomeViewModel? Income { get; set; }
        //public ExpenseCategoryDTO[]? Categories { get; set; }
        public IEnumerable<ExpenseCategoryDTO>? Categories { get; set; }

    }
}

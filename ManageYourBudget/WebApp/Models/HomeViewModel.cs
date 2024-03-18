namespace WebApp.Models
{
    public class HomeViewModel
    {
        public SavingsViewModel? Savings { get; set; }
        public IncomeViewModel? Income { get; set; }
        public CategoryViewModel[]? Categories { get; set; }

    }
}

namespace WebApp.Models
{
    public class CategoryViewModel
    {
        public required string Name { get; set; }
        public double PlannedBudget { get; set; }
        public double RemainingBudget { get; set; }

        // TODO: Array of expenses with details
    }
}
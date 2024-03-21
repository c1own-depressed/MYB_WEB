namespace Application.DTOs
{
    public class CreateExpenseCategoryDTO
    {
        public required string Title { get; set; }
        public double PlannedBudget { get; set; }
    }
}

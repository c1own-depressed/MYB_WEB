namespace Application.DTOs
{
    public class CreateExpenseCategoryDTO
    {
        required public string Title { get; set; }

        public double PlannedBudget { get; set; }
    }
}

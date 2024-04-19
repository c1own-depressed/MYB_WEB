namespace Application.DTOs.ExpenseDTOs
{
    public class EditExpenseCategoryDTO
    {
        required public string Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public double PlannedBudget { get; set; }
    }
}

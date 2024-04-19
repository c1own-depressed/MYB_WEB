namespace Application.DTOs.ExpenseDTOs
{
    public class EditExpenseCategoryDTO
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public double PlannedBudget { get; set; }
    }
}

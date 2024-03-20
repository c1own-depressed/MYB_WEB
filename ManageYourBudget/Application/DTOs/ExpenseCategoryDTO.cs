namespace Application.DTOs
{
    public class ExpenseCategoryDTO
    {
        public required string Name { get; set; }
        public double PlannedBudget { get; set; }
        public double RemainingBudget { get; set; }
        public IEnumerable<ExpenseDTO> Expenses { get; set; } = Enumerable.Empty<ExpenseDTO>();
    }
}

namespace Application.DTOs.ExpenseDTOs
{
    public class ExpenseCategoryDTO
    {
        public int Id { get; set; }

        required public string Name { get; set; }

        public double PlannedBudget { get; set; }

        public double RemainingBudget { get; set; }

        public IEnumerable<ExpenseDTO> Expenses { get; set; } = Enumerable.Empty<ExpenseDTO>();
    }
}

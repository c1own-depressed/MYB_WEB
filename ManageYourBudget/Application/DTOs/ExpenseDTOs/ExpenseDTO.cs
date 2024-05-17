namespace Application.DTOs.ExpenseDTOs
{
    public class ExpenseDTO
    {
        required public string Id { get; set; }

        public string ExpenseName { get; set; } = string.Empty;

        public double Amount { get; set; }

        public DateTime Date { get; set; }

        required public string CategoryId { get; set; }
    }
}

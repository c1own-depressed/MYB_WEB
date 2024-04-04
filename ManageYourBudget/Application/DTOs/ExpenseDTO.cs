namespace Application.DTOs
{
    public class ExpenseDTO
    {
        public int Id { get; set; }

        public string ExpenseName { get; set; } = string.Empty;

        public double Amount { get; set; }

        public DateTime Date { get; set; }

        public int CategoryId { get; set; }
    }
}

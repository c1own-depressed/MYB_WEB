namespace Domain.Entities
{
    public class Expense
    {
        required public string Id { get; set; }

        public string ExpenseName { get; set; } = string.Empty;

        public double Amount { get; set; }

        public DateTime Date { get; set; }
      
        required public string CategoryId { get; set; }
    }
}

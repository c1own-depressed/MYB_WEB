namespace Domain.Entities
{
    public class ExpenseCategory
    {
        required public string Id { get; set; }

        public string CategoryName { get; set; } = string.Empty;

        public double Amount { get; set; }

        required public string UserId { get; set; }
    }
}

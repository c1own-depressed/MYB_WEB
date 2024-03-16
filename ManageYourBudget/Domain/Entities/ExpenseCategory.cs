namespace Domain.Entities
{
    public class ExpenseCategory
    {
        public int Id { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public double Amount { get; set; }
        public int UserId { get; set; }
    }
}

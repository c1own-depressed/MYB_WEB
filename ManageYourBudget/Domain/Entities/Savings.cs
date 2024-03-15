namespace Domain.Entities
{
    internal class Savings
    {
        public int Id { get; set; }
        public string SavingsName { get; set; } = string.Empty;
        public double Amount { get; set; }
        public DateTime Date { get; set; }
        public int UserId { get; set; }
    }
}

namespace Domain.Entities
{
    public class Income
    {
        public int Id { get; set; }

        public string IncomeName { get; set; } = string.Empty;

        public double Amount { get; set; }

        public int UserId { get; set; }

        public bool IsRegular { get; set; } = true;

        public DateTime Date { get; set; }
    }
}

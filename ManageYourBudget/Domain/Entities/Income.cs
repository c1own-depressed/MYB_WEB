namespace Domain.Entities
{
    public class Income
    {
        required public string Id { get; set; }

        public string IncomeName { get; set; } = string.Empty;

        public double Amount { get; set; }

        required public string UserId { get; set; }

        public bool IsRegular { get; set; } = true;

        public DateTime Date { get; set; }
    }
}

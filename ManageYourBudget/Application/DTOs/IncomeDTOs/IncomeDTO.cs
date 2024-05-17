namespace Application.DTOs.IncomeDTOs
{
    public class IncomeDTO
    {
        required public string Id { get; set; }

        public string IncomeName { get; set; } = string.Empty;

        public double Amount { get; set; }

        public string CurrencyEmblem { get; set; } = "$";

        public bool IsRegular { get; set; }

        public DateTime Date { get; set; }
    }
}

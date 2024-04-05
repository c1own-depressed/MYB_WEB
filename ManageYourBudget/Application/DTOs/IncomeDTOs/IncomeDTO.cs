namespace Application.DTOs.IncomeDTOs
{
    public class IncomeDTO
    {
        public int Id { get; set; }

        public string IncomeName { get; set; } = string.Empty;

        public double Amount { get; set; }

        public string CurrencyEmblem { get; set; } = "$";
    }
}

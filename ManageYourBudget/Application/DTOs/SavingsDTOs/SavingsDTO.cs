namespace Application.DTOs.SavingsDTOs
{
    public class SavingsDTO
    {
        required public string Id { get; set; }

        public string SavingsName { get; set; } = string.Empty;

        public double Amount { get; set; }

        public DateTime Date { get; set; }

        public string CurrencyEmblem { get; set; } = "$";

        required public string UserId { get; set; }

        public string Note { get; set; }
    }
}

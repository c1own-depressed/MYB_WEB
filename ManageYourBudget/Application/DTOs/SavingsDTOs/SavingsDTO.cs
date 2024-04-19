namespace Application.DTOs.SavingsDTOs
{
    public class SavingsDTO
    {
        public int Id { get; set; }

        public string SavingsName { get; set; } = string.Empty;

        public double Amount { get; set; }

        public DateTime Date { get; set; }

        public string CurrencyEmblem { get; set; } = "$";

        public int UserId { get; set; }
    }
}

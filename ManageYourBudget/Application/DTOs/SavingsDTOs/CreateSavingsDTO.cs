namespace Application.DTOs.SavingsDTOs
{
    public class CreateSavingsDTO
    {
        public string SavingsName { get; set; } = string.Empty;

        public DateTime Date { get; set; }

        public double Amount { get; set; }

        public string Note { get; set; }
    }
}

namespace Application.DTOs.SavingsDTOs
{
    public class EditSavingsDTO
    {
        required public string Id { get; set; }

        public string SavingsName { get; set; } = string.Empty;

        public double Amount { get; set; }

        public string Note { get; set; }
    }
}

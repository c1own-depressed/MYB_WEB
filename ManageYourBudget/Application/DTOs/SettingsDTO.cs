namespace Application.DTOs
{
    public class SettingsDTO
    {
        public int Id { get; set; }

        public bool IsLightTheme { get; set; }

        public string Language { get; set; } = string.Empty;

        public string Currency { get; set; } = string.Empty;
    }
}

using Application.DTOs;

namespace WebApp.Models
{
    public class SettingsViewModel
    {
        public bool IsLightTheme { get; set; }

        public string Language { get; set; } = string.Empty;

        public string Currency { get; set; } = string.Empty;
    }
}

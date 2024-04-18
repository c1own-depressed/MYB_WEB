using Microsoft.AspNetCore.Identity;

namespace Domain.Entities
{
    public class User : IdentityUser
    {
        public bool IsLightTheme { get; set; }

        public string Language { get; set; } = string.Empty;

        public string Currency { get; set; } = string.Empty;
    }
}

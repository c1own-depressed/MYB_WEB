namespace Domain.Entities
{
    public class User
    {
        public int Id { get; set; }

        public string Username { get; set; } = string.Empty;

        public string HashedPassword { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public bool IsLightTheme { get; set; }

        public string Language { get; set; } = string.Empty;

        public string Currency { get; set; } = string.Empty;
    }
}

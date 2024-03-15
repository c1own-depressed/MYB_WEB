namespace Domain.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string HashedPassword { get; set; }
        public string Email { get; set; } 
        public bool IsLightTheme { get; set; }
        public string Language { get; set; }
        public string Currency { get; set; }
    }
}

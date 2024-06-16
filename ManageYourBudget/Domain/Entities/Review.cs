namespace Domain.Entities
{
     public class Review
    {
        required public string Id { get; set; }

        public string UserName { get; set; } = string.Empty;

        public string Text { get; set; } = string.Empty;

        public int Rating { get; set; } = int.MinValue;

        public DateTime CreatedAt { get; set; } = DateTime.MinValue;
    }
}

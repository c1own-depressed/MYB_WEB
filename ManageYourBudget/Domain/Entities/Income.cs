namespace Domain.Entities
{
    internal class Income
    {
        public int Id { get; set; }
        public string IncomeName { get; set; } = string.Empty;
        public double Amount { get; set; }
        public int UserId {  get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class SavingsDTO
    {
        public int Id { get; set; }
        public string SavingsName { get; set; } = string.Empty;
        public double Amount { get; set; }
        public DateTime Date { get; set; }
        public int UserId { get; set; }
    }
}

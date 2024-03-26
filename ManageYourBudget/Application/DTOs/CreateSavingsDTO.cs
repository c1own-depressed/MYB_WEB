using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class CreateSavingsDTO
    {
        public string SavingsName { get; set; } = string.Empty;
        public double Amount { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class ExpenseDTO
    {
        public string ExpenseName { get; set; } = string.Empty;
        public double Amount { get; set; }
    }
}

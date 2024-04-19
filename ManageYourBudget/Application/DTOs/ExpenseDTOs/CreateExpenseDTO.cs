using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.ExpenseDTOs
{
    public class CreateExpenseDTO
    {
        public string ExpenseName { get; set; } = string.Empty;

        public double Amount { get; set; }

        public DateTime Date { get; set; }

        required public string CategoryId { get; set; }
    }
}

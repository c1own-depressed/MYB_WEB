using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class ExpenseCategoryDTO
    {
        public required string Name { get; set; }
        public double PlannedBudget { get; set; }
        public double RemainingBudget { get; set; }

        // TODO: Array of expenses with details
    }
}

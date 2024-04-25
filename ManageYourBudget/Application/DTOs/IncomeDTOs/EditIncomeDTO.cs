using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.IncomeDTOs
{
    public class EditIncomeDTO
    {
        required public string Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public double Amount { get; set; }

        public bool IsRegular { get; set; }

        public DateTime Date { get; set; }
    }
}

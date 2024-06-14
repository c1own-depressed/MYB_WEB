using Application.DTOs.ExpenseDTOs;
using Application.DTOs.IncomeDTOs;
using Application.DTOs.SavingsDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class HomeDTO
    {
        public IEnumerable<ExpenseCategoryDTO> Categories { get; set; }

        public IEnumerable<IncomeDTO> Incomes { get; set; }

        public IEnumerable<SavingsDTO> Savings { get; set; }
    }
}

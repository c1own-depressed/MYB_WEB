using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.StatisticDTO
{
    public class AllStatisticDataDTO
    {
        public List<IncomeStatisticDTO>? IncomeStatistics { get; set; }

        public List<TotalExpensesDTO>? ExpensesStatistics { get; set; }

        public List<SavedStatisticDTO>? SavingsStatistics { get; set; }
    }
}

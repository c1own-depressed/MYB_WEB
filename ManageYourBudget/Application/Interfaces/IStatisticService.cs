using Application.DTOs.StatisticDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IStatisticService
    {
        Task<IEnumerable<IncomeStatisticDTO>> getIncomesByDate(DateTime startMonth, DateTime endMonth, int UserId);
    }
}

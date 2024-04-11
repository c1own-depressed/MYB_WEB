using Application.DTOs.ExpenseDTOs;
using Application.DTOs.IncomeDTOs;
using Application.DTOs.SavingsDTOs;

namespace WebApp.Models
{
    public class StatisticViewModel
    {
        public IEnumerable<int>? Savings { get; set; }

        public IEnumerable<int>? Incomes { get; set; }

        public IEnumerable<int>? SummaryExpenses { get; set; }
    }
}
